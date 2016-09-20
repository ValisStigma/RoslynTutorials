using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace PublicFieldRefactoringExample
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(PublicFieldRefactoringExampleCodeRefactoringProvider)), Shared]
    internal class PublicFieldRefactoringExampleCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var node = root.FindNode(context.Span);

            var fieldDecl = node as FieldDeclarationSyntax;
            if(fieldDecl == null || fieldDecl.Modifiers.ToFullString().Contains("public") == false)
            {
                return;
            }

            bool mustRegisterAction = fieldDecl.Declaration.Variables.Any(i => char.IsLower(i.Identifier.ValueText.ToCharArray().First()));
            if (!mustRegisterAction)
            {
                return;
            }

            var action = CodeAction.Create("Make first char upper Case",
                c => RenameFieldAsync(context.Document, fieldDecl, c));
            context.RegisterRefactoring(action);

        }

        private async Task<Document> RenameFieldAsync(Document document, FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var root = await document.GetSyntaxRootAsync();

            var oldDeclarations = fieldDeclaration.Declaration.Variables;

            var listOfNewModifiedDeclarators = new SeparatedSyntaxList<VariableDeclaratorSyntax>();

            foreach (var declarator in oldDeclarations)
            {
                var tempString = ConvertName(declarator.Identifier.ToFullString());
                listOfNewModifiedDeclarators =
                    listOfNewModifiedDeclarators.Add(declarator.WithIdentifier(SyntaxFactory.ParseToken(tempString)));

            }

            var newDeclaration = fieldDeclaration.Declaration.WithVariables(listOfNewModifiedDeclarators);
            var newField = fieldDeclaration.WithDeclaration(newDeclaration);
            SyntaxNode newRoot = root.ReplaceNode(fieldDeclaration, newField);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
           
        }

        private string ConvertName(string oldName)
        {
            return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
        }
    }
}
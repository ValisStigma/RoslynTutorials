using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace DateTimeOffestAnalyzerExample
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeOffestAnalyzerExampleCodeFixProvider)), Shared]
    public class DateTimeOffestAnalyzerExampleCodeFixProvider : CodeFixProvider
    {
        private const string title = "Replace with DateTimeOffset";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DateTimeOffestAnalyzerExampleAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            //Reference to diagnostic to fix
            var diagnostic = context.Diagnostics.First();
            //Get location for diagnostic
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find syntax node on the span where there is a squiggle
            var node = root.FindNode(context.Span);

            //If node is not an identifiername return
            if (node is IdentifierNameSyntax == false)
            {
                return;
            }
            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ReplaceDateTimeAsync(context.Document, node, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ReplaceDateTimeAsync(Document document, SyntaxNode node,
            CancellationToken cancellationToken)
        {
            //Get root syntac node for current document
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            //Convert node in specialized kind
            var convertedNode = (IdentifierNameSyntax) node;

            //Create new SyntaxNode
            var newNode =
                convertedNode?.WithIdentifier(SyntaxFactory.ParseToken("DateTimeOffset"))
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());

            //Create new root syntax node for current document, replacing nodes

            var newRoot = root.ReplaceNode(node, newNode);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;

        }

        private async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            var identifierToken = typeDecl.Identifier;
            var newName = identifierToken.Text.ToUpperInvariant();

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }
    }
}
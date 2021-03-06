using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DateTimeOffestAnalyzerExample
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeOffestAnalyzerExampleAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DTA001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction((CompilationStartAnalysisContext ctx) =>
            {
                var requestedType = ctx.Compilation.GetTypeByMetadataName("Windows.Storage.StorageFile");
                if (requestedType == null)
                {
                    return;
                }
                ctx.RegisterSyntaxNodeAction(AnalyzeDateTime, SyntaxKind.IdentifierName);
            });
            context.RegisterSyntaxNodeAction(AnalyzeDateTime, SyntaxKind.IdentifierName);
        }

        private void AnalyzeDateTime(SyntaxNodeAnalysisContext context)
        {
            //Get Syntax Node to Analyze
            var root = context.Node;

            //break if not an IdentifierName syntax node
            if (!(root is IdentifierNameSyntax))
            {
                return;
            }

            //Convert to IdentifierNameSyntax
            root = (IdentifierNameSyntax) context.Node;


            //Get Symbol info for datetime type decl
            var dateSymbol = context.SemanticModel.GetSymbolInfo(root).Symbol as INamedTypeSymbol;

            //Break if no info
            if (dateSymbol == null)
            {
                return;
            }

            //if name of symbol is not datetime return
            if (dateSymbol.MetadataName != "DateTime")
            {
                return;
            }

            //Create Diagnostic at the node location with the specified message and rule info
            var diagn = Diagnostic.Create(Rule, root.GetLocation(), "Consider replacing with DateTimeOffset");

            //Report Diagnostic

            context.ReportDiagnostic(diagn);
        }
        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}

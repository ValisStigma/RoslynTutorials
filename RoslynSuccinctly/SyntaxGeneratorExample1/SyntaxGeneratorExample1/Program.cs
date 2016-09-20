
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;

namespace SyntaxGeneratorExample1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ws = MSBuildWorkspace.Create();
            const string language = "C#";
            var generator = SyntaxGenerator.GetGenerator(ws, language);

            var interfaceType = generator.DottedName("IDisposable");

            var methodBlock = generator.MethodDeclaration(
                "Dispose", accessibility: Accessibility.Public);

            var methodBlockWithInterface = generator.AsPublicInterfaceImplementation(methodBlock, interfaceType);

            var classBlock = generator.ClassDeclaration("Person",
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Abstract,
                members: new SyntaxNode[] {methodBlockWithInterface},
                interfaceTypes: new SyntaxNode[] {interfaceType})
                .NormalizeWhitespace();
            Console.WriteLine(classBlock.ToFullString());
            Console.Read();
        }
    }
}

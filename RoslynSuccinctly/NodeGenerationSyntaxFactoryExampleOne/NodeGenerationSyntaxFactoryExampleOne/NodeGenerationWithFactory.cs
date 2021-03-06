﻿
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NodeGenerationSyntaxFactoryExampleOne
{
    public class NodeGenerationWithFactory
    {
        public static void Main(string[] args)
        {
            var classBlock = SyntaxFactory.ClassDeclaration(@"Person")
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new[]
                        {
                            SyntaxFactory.Token(
                                SyntaxKind.AbstractKeyword
                                ),
                            SyntaxFactory.Token(
                                SyntaxKind.PublicKeyword
                                )
                        }
                        )
                )
                .WithKeyword(
                    SyntaxFactory.Token(
                        SyntaxKind.ClassKeyword
                        )
                )
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.IdentifierName(
                                    @"IDisposable"
                                    )
                                )
                            )
                        )
                        .WithColonToken(
                            SyntaxFactory.Token(
                                SyntaxKind.ColonToken
                                )
                        )
                )
                .WithOpenBraceToken(
                    SyntaxFactory.Token(
                        SyntaxKind.OpenBraceToken
                        )
                )
                .WithMembers(
                    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.MethodDeclaration(
                            SyntaxFactory.PredefinedType(
                                SyntaxFactory.Token(
                                    SyntaxKind.VoidKeyword
                                    )
                                ),
                            SyntaxFactory.Identifier(
                                @"IDispose"
                                )
                            )
                            .WithExplicitInterfaceSpecifier(
                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                    SyntaxFactory.IdentifierName(
                                        @"IDisposable"
                                        )
                                    )
                                    .WithDotToken(
                                        SyntaxFactory.Token(
                                            SyntaxKind.DotToken
                                            )
                                    )
                            )
                            .WithParameterList(
                                SyntaxFactory.ParameterList()
                                    .WithOpenParenToken(
                                        SyntaxFactory.Token(
                                            SyntaxKind.OpenParenToken
                                            )
                                    )
                                    .WithCloseParenToken(
                                        SyntaxFactory.Token(
                                            SyntaxKind.CloseParenToken
                                            )
                                    )
                            )
                            .WithBody(
                                SyntaxFactory.Block()
                                    .WithOpenBraceToken(
                                        SyntaxFactory.Token(
                                            SyntaxKind.OpenBraceToken
                                            )
                                    )
                                    .WithCloseBraceToken(
                                        SyntaxFactory.Token(
                                            SyntaxKind.CloseBraceToken
                                            )
                                    )
                            )
                        )
                )
                .WithCloseBraceToken(
                    SyntaxFactory.Token(
                        SyntaxKind.CloseBraceToken
                        )
                )
                .NormalizeWhitespace();

        }
    }
}

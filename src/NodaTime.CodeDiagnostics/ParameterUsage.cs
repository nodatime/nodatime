// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NodaTime.CodeDiagnostics
{
    /// <summary>
    /// The usage of a parameter within a syntax node.
    /// </summary>
    internal sealed class ParameterUsage
    {
        internal SyntaxNode UsageNode { get; private set; }
        internal IParameterSymbol CorrespondingParameter { get; private set; }

        internal IMethodSymbol InvokedMember { get; private set; }

        // TODO: Work out a way to handle multiple uses within a single invocation, e.g. x * x,
        // as well as making it easier to get usages in execution order, e.g. Foo(x, Bar(x)) should
        // know that Bar is used first.
        internal static ParameterUsage ForNode(SyntaxNode node, SemanticModel model, ISymbol parameter)
        {
            // TODO: Parameter dereferencing, e.g. myParameter.X?
            var symbol = model.GetSymbolInfo(node).Symbol;
            IMethodSymbol invokedMethod = symbol as IMethodSymbol;
            ArgumentListSyntax argumentList = null;
            switch (node.CSharpKind())
            {
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.ThisConstructorInitializer:
                    argumentList = ((ConstructorInitializerSyntax)node).ArgumentList;
                    break;
                case SyntaxKind.InvocationExpression:
                    var invocation = ((InvocationExpressionSyntax) node);
                    // Handle x.Foo() calls where x is the parameter.
                    if (invocation.Expression != null)
                    {
                        var invocationSymbol = model.GetSymbolInfo(invocation.Expression).Symbol;
                        if (ReferenceEquals(invocationSymbol, parameter))
                        {
                            return new ParameterUsage { UsageNode = node };
                        }
                    }
                    argumentList = ((InvocationExpressionSyntax)node).ArgumentList;
                    break;
                case SyntaxKind.SimpleMemberAccessExpression:
                    // Handle x.Foo calls where x is the parameter.
                    var accessSyntax = (MemberAccessExpressionSyntax) node;
                    var accessSymbol = model.GetSymbolInfo(accessSyntax.Expression).Symbol;
                    return ReferenceEquals(accessSymbol, parameter)
                        ? new ParameterUsage { UsageNode = node } : null;
                case SyntaxKind.ObjectCreationExpression:
                    argumentList = ((ObjectCreationExpressionSyntax)node).ArgumentList;
                    break;
                // TODO: Compound assignment operators?
                default:
                    // Listing all the operators separately is a pain.
                    var binaryExpression = node as BinaryExpressionSyntax;
                    if (binaryExpression == null || invokedMethod == null)
                    {
                        return null;
                    }
                    return ForArgument(binaryExpression.Left, 0, model, invokedMethod, parameter)
                           ?? ForArgument(binaryExpression.Right, 1, model, invokedMethod, parameter);
            }
            if (invokedMethod == null)
            {
                return null;
            }
            if (argumentList != null)
            {
                var arguments = argumentList.Arguments;
                for (int i = 0; i < arguments.Count; i++)
                {
                    var possibleResult = ForArgument(arguments[i].Expression, i, model, invokedMethod, parameter);
                    if (possibleResult != null)
                    {
                        return possibleResult;
                    }
                }
            }

            return null;
        }

        private static ParameterUsage ForArgument(SyntaxNode argument, int argumentNumber, SemanticModel model, IMethodSymbol invokedMethod, ISymbol parameter)
        {
            var argumentSymbol = model.GetSymbolInfo(argument).Symbol;
            if (!ReferenceEquals(argumentSymbol, parameter))
            {
                return null;
            }
            // TODO: We really need to work out how to cope with this sort of thing. (Params arrays,
            // named arguments...)
            var invokedParameters = invokedMethod.Parameters;
            if (invokedParameters.Length <= argumentNumber)
            {
                return null;
            }
            var correspondingParameter = invokedParameters[argumentNumber];
            return new ParameterUsage
            {
                UsageNode = argument,
                CorrespondingParameter = correspondingParameter,
                InvokedMember = invokedMethod
            };
        }
    }

}

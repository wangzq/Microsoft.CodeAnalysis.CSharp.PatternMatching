﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    public class VarTypePattern : TypePattern
    {
        private readonly Action<TypeSyntax> _action;

        public VarTypePattern(Action<TypeSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is TypeSyntax typed && !typed.IsVar;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            _action?.Invoke((TypeSyntax)node);
        }
    }

    public class VarTypePattern<TResult> : TypePattern<TResult>
    {
        private readonly Func<TResult, TypeSyntax, TResult> _action;

        public VarTypePattern(Func<TResult, TypeSyntax, TResult> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            return node is TypeSyntax typed && !typed.IsVar;
        }

        internal override TResult RunCallback(TResult result, SyntaxNode node, SemanticModel semanticModel)
        {
            if (_action != null)
                result = _action(result, (TypeSyntax)node);

            return result;
        }
    }
}

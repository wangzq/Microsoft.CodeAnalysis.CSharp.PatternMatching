using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.PatternMatching
{
    // #13
    public abstract partial class NamePattern : TypePattern
    {

        internal NamePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameSyntax typed))
                return false;


            return true;
        }
    }

    // #18
    public abstract partial class SimpleNamePattern : NamePattern
    {
        private readonly string _identifier;

        internal SimpleNamePattern(string identifier)
        {
            _identifier = identifier;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleNameSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }
    }

    // #29
    public partial class IdentifierNamePattern : SimpleNamePattern
    {
        private readonly Action<IdentifierNameSyntax> _action;

        internal IdentifierNamePattern(string identifier, Action<IdentifierNameSyntax> action)
            : base(identifier)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IdentifierNameSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IdentifierNameSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #45
    public partial class QualifiedNamePattern : NamePattern
    {
        private readonly NamePattern _left;
        private readonly SimpleNamePattern _right;
        private readonly Action<QualifiedNameSyntax> _action;

        internal QualifiedNamePattern(NamePattern left, SimpleNamePattern right, Action<QualifiedNameSyntax> action)
        {
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QualifiedNameSyntax typed))
                return false;

            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QualifiedNameSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #70
    public partial class GenericNamePattern : SimpleNamePattern
    {
        private readonly TypeArgumentListPattern _typeArgumentList;
        private readonly Action<GenericNameSyntax> _action;

        internal GenericNamePattern(string identifier, TypeArgumentListPattern typeArgumentList, Action<GenericNameSyntax> action)
            : base(identifier)
        {
            _typeArgumentList = typeArgumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GenericNameSyntax typed))
                return false;

            if (_typeArgumentList != null && !_typeArgumentList.Test(typed.TypeArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GenericNameSyntax)node;

            if (_typeArgumentList != null)
                _typeArgumentList.RunCallback(typed.TypeArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #90
    public partial class TypeArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<TypePattern> _arguments;
        private readonly Action<TypeArgumentListSyntax> _action;

        internal TypeArgumentListPattern(NodeListPattern<TypePattern> arguments, Action<TypeArgumentListSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #116
    public partial class AliasQualifiedNamePattern : NamePattern
    {
        private readonly IdentifierNamePattern _alias;
        private readonly SimpleNamePattern _name;
        private readonly Action<AliasQualifiedNameSyntax> _action;

        internal AliasQualifiedNamePattern(IdentifierNamePattern alias, SimpleNamePattern name, Action<AliasQualifiedNameSyntax> action)
        {
            _alias = alias;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AliasQualifiedNameSyntax typed))
                return false;

            if (_alias != null && !_alias.Test(typed.Alias, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AliasQualifiedNameSyntax)node;

            if (_alias != null)
                _alias.RunCallback(typed.Alias, semanticModel);
            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #142
    public abstract partial class TypePattern : ExpressionPattern
    {

        internal TypePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeSyntax typed))
                return false;


            return true;
        }
    }

    // #147
    public partial class PredefinedTypePattern : TypePattern
    {
        private readonly string _keyword;
        private readonly Action<PredefinedTypeSyntax> _action;

        internal PredefinedTypePattern(string keyword, Action<PredefinedTypeSyntax> action)
        {
            _keyword = keyword;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PredefinedTypeSyntax typed))
                return false;

            if (_keyword != null && _keyword != typed.Keyword.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PredefinedTypeSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #177
    public partial class ArrayTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly NodeListPattern<ArrayRankSpecifierPattern> _rankSpecifiers;
        private readonly Action<ArrayTypeSyntax> _action;

        internal ArrayTypePattern(TypePattern elementType, NodeListPattern<ArrayRankSpecifierPattern> rankSpecifiers, Action<ArrayTypeSyntax> action)
        {
            _elementType = elementType;
            _rankSpecifiers = rankSpecifiers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;
            if (_rankSpecifiers != null && !_rankSpecifiers.Test(typed.RankSpecifiers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);
            if (_rankSpecifiers != null)
                _rankSpecifiers.RunCallback(typed.RankSpecifiers, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #196
    public partial class ArrayRankSpecifierPattern : PatternNode
    {
        private readonly NodeListPattern<ExpressionPattern> _sizes;
        private readonly Action<ArrayRankSpecifierSyntax> _action;

        internal ArrayRankSpecifierPattern(NodeListPattern<ExpressionPattern> sizes, Action<ArrayRankSpecifierSyntax> action)
        {
            _sizes = sizes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayRankSpecifierSyntax typed))
                return false;

            if (_sizes != null && !_sizes.Test(typed.Sizes, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayRankSpecifierSyntax)node;

            if (_sizes != null)
                _sizes.RunCallback(typed.Sizes, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #206
    public partial class PointerTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly Action<PointerTypeSyntax> _action;

        internal PointerTypePattern(TypePattern elementType, Action<PointerTypeSyntax> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PointerTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PointerTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #226
    public partial class FunctionPointerTypePattern : TypePattern
    {
        private readonly FunctionPointerCallingConventionPattern _callingConvention;
        private readonly FunctionPointerParameterListPattern _parameterList;
        private readonly Action<FunctionPointerTypeSyntax> _action;

        internal FunctionPointerTypePattern(FunctionPointerCallingConventionPattern callingConvention, FunctionPointerParameterListPattern parameterList, Action<FunctionPointerTypeSyntax> action)
        {
            _callingConvention = callingConvention;
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerTypeSyntax typed))
                return false;

            if (_callingConvention != null && !_callingConvention.Test(typed.CallingConvention, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FunctionPointerTypeSyntax)node;

            if (_callingConvention != null)
                _callingConvention.RunCallback(typed.CallingConvention, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #251
    public partial class FunctionPointerParameterListPattern : PatternNode
    {
        private readonly NodeListPattern<FunctionPointerParameterPattern> _parameters;
        private readonly Action<FunctionPointerParameterListSyntax> _action;

        internal FunctionPointerParameterListPattern(NodeListPattern<FunctionPointerParameterPattern> parameters, Action<FunctionPointerParameterListSyntax> action)
        {
            _parameters = parameters;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FunctionPointerParameterListSyntax)node;

            if (_parameters != null)
                _parameters.RunCallback(typed.Parameters, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #274
    public partial class FunctionPointerCallingConventionPattern : PatternNode
    {
        private readonly FunctionPointerUnmanagedCallingConventionListPattern _unmanagedCallingConventionList;
        private readonly Action<FunctionPointerCallingConventionSyntax> _action;

        internal FunctionPointerCallingConventionPattern(FunctionPointerUnmanagedCallingConventionListPattern unmanagedCallingConventionList, Action<FunctionPointerCallingConventionSyntax> action)
        {
            _unmanagedCallingConventionList = unmanagedCallingConventionList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerCallingConventionSyntax typed))
                return false;

            if (_unmanagedCallingConventionList != null && !_unmanagedCallingConventionList.Test(typed.UnmanagedCallingConventionList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FunctionPointerCallingConventionSyntax)node;

            if (_unmanagedCallingConventionList != null)
                _unmanagedCallingConventionList.RunCallback(typed.UnmanagedCallingConventionList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #292
    public partial class FunctionPointerUnmanagedCallingConventionListPattern : PatternNode
    {
        private readonly NodeListPattern<FunctionPointerUnmanagedCallingConventionPattern> _callingConventions;
        private readonly Action<FunctionPointerUnmanagedCallingConventionListSyntax> _action;

        internal FunctionPointerUnmanagedCallingConventionListPattern(NodeListPattern<FunctionPointerUnmanagedCallingConventionPattern> callingConventions, Action<FunctionPointerUnmanagedCallingConventionListSyntax> action)
        {
            _callingConventions = callingConventions;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerUnmanagedCallingConventionListSyntax typed))
                return false;

            if (_callingConventions != null && !_callingConventions.Test(typed.CallingConventions, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FunctionPointerUnmanagedCallingConventionListSyntax)node;

            if (_callingConventions != null)
                _callingConventions.RunCallback(typed.CallingConventions, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #315
    public partial class FunctionPointerUnmanagedCallingConventionPattern : PatternNode
    {
        private readonly string _name;
        private readonly Action<FunctionPointerUnmanagedCallingConventionSyntax> _action;

        internal FunctionPointerUnmanagedCallingConventionPattern(string name, Action<FunctionPointerUnmanagedCallingConventionSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerUnmanagedCallingConventionSyntax typed))
                return false;

            if (_name != null && _name != typed.Name.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FunctionPointerUnmanagedCallingConventionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #327
    public partial class NullableTypePattern : TypePattern
    {
        private readonly TypePattern _elementType;
        private readonly Action<NullableTypeSyntax> _action;

        internal NullableTypePattern(TypePattern elementType, Action<NullableTypeSyntax> action)
        {
            _elementType = elementType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NullableTypeSyntax typed))
                return false;

            if (_elementType != null && !_elementType.Test(typed.ElementType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NullableTypeSyntax)node;

            if (_elementType != null)
                _elementType.RunCallback(typed.ElementType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #347
    public partial class TupleTypePattern : TypePattern
    {
        private readonly NodeListPattern<TupleElementPattern> _elements;
        private readonly Action<TupleTypeSyntax> _action;

        internal TupleTypePattern(NodeListPattern<TupleElementPattern> elements, Action<TupleTypeSyntax> action)
        {
            _elements = elements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleTypeSyntax typed))
                return false;

            if (_elements != null && !_elements.Test(typed.Elements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleTypeSyntax)node;

            if (_elements != null)
                _elements.RunCallback(typed.Elements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #369
    public partial class TupleElementPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<TupleElementSyntax> _action;

        internal TupleElementPattern(TypePattern type, string identifier, Action<TupleElementSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleElementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleElementSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #386
    public partial class OmittedTypeArgumentPattern : TypePattern
    {
        private readonly Action<OmittedTypeArgumentSyntax> _action;

        internal OmittedTypeArgumentPattern(Action<OmittedTypeArgumentSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedTypeArgumentSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedTypeArgumentSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #401
    public partial class RefTypePattern : TypePattern
    {
        private readonly TypePattern _type;
        private readonly Action<RefTypeSyntax> _action;

        internal RefTypePattern(TypePattern type, Action<RefTypeSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #418
    public abstract partial class ExpressionOrPatternPattern : PatternNode
    {

        internal ExpressionOrPatternPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionOrPatternSyntax typed))
                return false;


            return true;
        }
    }

    // #419
    public abstract partial class ExpressionPattern : ExpressionOrPatternPattern
    {

        internal ExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionSyntax typed))
                return false;


            return true;
        }
    }

    // #424
    public partial class ParenthesizedExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ParenthesizedExpressionSyntax> _action;

        internal ParenthesizedExpressionPattern(ExpressionPattern expression, Action<ParenthesizedExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #450
    public partial class TupleExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<ArgumentPattern> _arguments;
        private readonly Action<TupleExpressionSyntax> _action;

        internal TupleExpressionPattern(NodeListPattern<ArgumentPattern> arguments, Action<TupleExpressionSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TupleExpressionSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TupleExpressionSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #476
    public partial class PrefixUnaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _operand;
        private readonly Action<PrefixUnaryExpressionSyntax> _action;

        internal PrefixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern operand, Action<PrefixUnaryExpressionSyntax> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PrefixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PrefixUnaryExpressionSyntax)node;

            if (_operand != null)
                _operand.RunCallback(typed.Operand, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #512
    public partial class AwaitExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<AwaitExpressionSyntax> _action;

        internal AwaitExpressionPattern(ExpressionPattern expression, Action<AwaitExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AwaitExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AwaitExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #532
    public partial class PostfixUnaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _operand;
        private readonly Action<PostfixUnaryExpressionSyntax> _action;

        internal PostfixUnaryExpressionPattern(SyntaxKind kind, ExpressionPattern operand, Action<PostfixUnaryExpressionSyntax> action)
        {
            _kind = kind;
            _operand = operand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PostfixUnaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_operand != null && !_operand.Test(typed.Operand, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PostfixUnaryExpressionSyntax)node;

            if (_operand != null)
                _operand.RunCallback(typed.Operand, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #556
    public partial class MemberAccessExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly SimpleNamePattern _name;
        private readonly Action<MemberAccessExpressionSyntax> _action;

        internal MemberAccessExpressionPattern(SyntaxKind kind, ExpressionPattern expression, SimpleNamePattern name, Action<MemberAccessExpressionSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberAccessExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #583
    public partial class ConditionalAccessExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly ExpressionPattern _whenNotNull;
        private readonly Action<ConditionalAccessExpressionSyntax> _action;

        internal ConditionalAccessExpressionPattern(ExpressionPattern expression, ExpressionPattern whenNotNull, Action<ConditionalAccessExpressionSyntax> action)
        {
            _expression = expression;
            _whenNotNull = whenNotNull;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_whenNotNull != null && !_whenNotNull.Test(typed.WhenNotNull, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_whenNotNull != null)
                _whenNotNull.RunCallback(typed.WhenNotNull, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #608
    public partial class MemberBindingExpressionPattern : ExpressionPattern
    {
        private readonly SimpleNamePattern _name;
        private readonly Action<MemberBindingExpressionSyntax> _action;

        internal MemberBindingExpressionPattern(SimpleNamePattern name, Action<MemberBindingExpressionSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberBindingExpressionSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberBindingExpressionSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #628
    public partial class ElementBindingExpressionPattern : ExpressionPattern
    {
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ElementBindingExpressionSyntax> _action;

        internal ElementBindingExpressionPattern(BracketedArgumentListPattern argumentList, Action<ElementBindingExpressionSyntax> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementBindingExpressionSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementBindingExpressionSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #642
    public partial class RangeExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _leftOperand;
        private readonly ExpressionPattern _rightOperand;
        private readonly Action<RangeExpressionSyntax> _action;

        internal RangeExpressionPattern(ExpressionPattern leftOperand, ExpressionPattern rightOperand, Action<RangeExpressionSyntax> action)
        {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RangeExpressionSyntax typed))
                return false;

            if (_leftOperand != null && !_leftOperand.Test(typed.LeftOperand, semanticModel))
                return false;
            if (_rightOperand != null && !_rightOperand.Test(typed.RightOperand, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RangeExpressionSyntax)node;

            if (_leftOperand != null)
                _leftOperand.RunCallback(typed.LeftOperand, semanticModel);
            if (_rightOperand != null)
                _rightOperand.RunCallback(typed.RightOperand, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #667
    public partial class ImplicitElementAccessPattern : ExpressionPattern
    {
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ImplicitElementAccessSyntax> _action;

        internal ImplicitElementAccessPattern(BracketedArgumentListPattern argumentList, Action<ImplicitElementAccessSyntax> action)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitElementAccessSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitElementAccessSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #681
    public partial class BinaryExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _left;
        private readonly ExpressionPattern _right;
        private readonly Action<BinaryExpressionSyntax> _action;

        internal BinaryExpressionPattern(SyntaxKind kind, ExpressionPattern left, ExpressionPattern right, Action<BinaryExpressionSyntax> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BinaryExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BinaryExpressionSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #746
    public partial class AssignmentExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _left;
        private readonly ExpressionPattern _right;
        private readonly Action<AssignmentExpressionSyntax> _action;

        internal AssignmentExpressionPattern(SyntaxKind kind, ExpressionPattern left, ExpressionPattern right, Action<AssignmentExpressionSyntax> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AssignmentExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AssignmentExpressionSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #793
    public partial class ConditionalExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly ExpressionPattern _whenTrue;
        private readonly ExpressionPattern _whenFalse;
        private readonly Action<ConditionalExpressionSyntax> _action;

        internal ConditionalExpressionPattern(ExpressionPattern condition, ExpressionPattern whenTrue, ExpressionPattern whenFalse, Action<ConditionalExpressionSyntax> action)
        {
            _condition = condition;
            _whenTrue = whenTrue;
            _whenFalse = whenFalse;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConditionalExpressionSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_whenTrue != null && !_whenTrue.Test(typed.WhenTrue, semanticModel))
                return false;
            if (_whenFalse != null && !_whenFalse.Test(typed.WhenFalse, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConditionalExpressionSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_whenTrue != null)
                _whenTrue.RunCallback(typed.WhenTrue, semanticModel);
            if (_whenFalse != null)
                _whenFalse.RunCallback(typed.WhenFalse, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #829
    public abstract partial class InstanceExpressionPattern : ExpressionPattern
    {

        internal InstanceExpressionPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InstanceExpressionSyntax typed))
                return false;


            return true;
        }
    }

    // #834
    public partial class ThisExpressionPattern : InstanceExpressionPattern
    {
        private readonly Action<ThisExpressionSyntax> _action;

        internal ThisExpressionPattern(Action<ThisExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThisExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThisExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #849
    public partial class BaseExpressionPattern : InstanceExpressionPattern
    {
        private readonly Action<BaseExpressionSyntax> _action;

        internal BaseExpressionPattern(Action<BaseExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #864
    public partial class LiteralExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly Action<LiteralExpressionSyntax> _action;

        internal LiteralExpressionPattern(SyntaxKind kind, Action<LiteralExpressionSyntax> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LiteralExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LiteralExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #893
    public partial class MakeRefExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<MakeRefExpressionSyntax> _action;

        internal MakeRefExpressionPattern(ExpressionPattern expression, Action<MakeRefExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MakeRefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MakeRefExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #925
    public partial class RefTypeExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<RefTypeExpressionSyntax> _action;

        internal RefTypeExpressionPattern(ExpressionPattern expression, Action<RefTypeExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefTypeExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefTypeExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #957
    public partial class RefValueExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly TypePattern _type;
        private readonly Action<RefValueExpressionSyntax> _action;

        internal RefValueExpressionPattern(ExpressionPattern expression, TypePattern type, Action<RefValueExpressionSyntax> action)
        {
            _expression = expression;
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefValueExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefValueExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1000
    public partial class CheckedExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<CheckedExpressionSyntax> _action;

        internal CheckedExpressionPattern(SyntaxKind kind, ExpressionPattern expression, Action<CheckedExpressionSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CheckedExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1034
    public partial class DefaultExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<DefaultExpressionSyntax> _action;

        internal DefaultExpressionPattern(TypePattern type, Action<DefaultExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1066
    public partial class TypeOfExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<TypeOfExpressionSyntax> _action;

        internal TypeOfExpressionPattern(TypePattern type, Action<TypeOfExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeOfExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1098
    public partial class SizeOfExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<SizeOfExpressionSyntax> _action;

        internal SizeOfExpressionPattern(TypePattern type, Action<SizeOfExpressionSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SizeOfExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SizeOfExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1130
    public partial class InvocationExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly ArgumentListPattern _argumentList;
        private readonly Action<InvocationExpressionSyntax> _action;

        internal InvocationExpressionPattern(ExpressionPattern expression, ArgumentListPattern argumentList, Action<InvocationExpressionSyntax> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InvocationExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InvocationExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1149
    public partial class ElementAccessExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly Action<ElementAccessExpressionSyntax> _action;

        internal ElementAccessExpressionPattern(ExpressionPattern expression, BracketedArgumentListPattern argumentList, Action<ElementAccessExpressionSyntax> action)
        {
            _expression = expression;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElementAccessExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElementAccessExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1168
    public abstract partial class BaseArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<ArgumentPattern> _arguments;

        internal BaseArgumentListPattern(NodeListPattern<ArgumentPattern> arguments)
        {
            _arguments = arguments;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);
        }
    }

    // #1178
    public partial class ArgumentListPattern : BaseArgumentListPattern
    {
        private readonly Action<ArgumentListSyntax> _action;

        internal ArgumentListPattern(NodeListPattern<ArgumentPattern> arguments, Action<ArgumentListSyntax> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ArgumentListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1204
    public partial class BracketedArgumentListPattern : BaseArgumentListPattern
    {
        private readonly Action<BracketedArgumentListSyntax> _action;

        internal BracketedArgumentListPattern(NodeListPattern<ArgumentPattern> arguments, Action<BracketedArgumentListSyntax> action)
            : base(arguments)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedArgumentListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BracketedArgumentListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1230
    public partial class ArgumentPattern : PatternNode
    {
        private readonly NameColonPattern _nameColon;
        private readonly ExpressionPattern _expression;
        private readonly Action<ArgumentSyntax> _action;

        internal ArgumentPattern(NameColonPattern nameColon, ExpressionPattern expression, Action<ArgumentSyntax> action)
        {
            _nameColon = nameColon;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArgumentSyntax typed))
                return false;

            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArgumentSyntax)node;

            if (_nameColon != null)
                _nameColon.RunCallback(typed.NameColon, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1257
    public partial class NameColonPattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly Action<NameColonSyntax> _action;

        internal NameColonPattern(IdentifierNamePattern name, Action<NameColonSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameColonSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameColonSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1278
    public partial class DeclarationExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly VariableDesignationPattern _designation;
        private readonly Action<DeclarationExpressionSyntax> _action;

        internal DeclarationExpressionPattern(TypePattern type, VariableDesignationPattern designation, Action<DeclarationExpressionSyntax> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1293
    public partial class CastExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly ExpressionPattern _expression;
        private readonly Action<CastExpressionSyntax> _action;

        internal CastExpressionPattern(TypePattern type, ExpressionPattern expression, Action<CastExpressionSyntax> action)
        {
            _type = type;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CastExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CastExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1324
    public abstract partial class AnonymousFunctionExpressionPattern : ExpressionPattern
    {
        private readonly TokenListPattern _modifiers;

        internal AnonymousFunctionExpressionPattern(TokenListPattern modifiers)
        {
            _modifiers = modifiers;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousFunctionExpressionSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;

            return true;
        }
    }

    // #1348
    public partial class AnonymousMethodExpressionPattern : AnonymousFunctionExpressionPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<AnonymousMethodExpressionSyntax> _action;

        internal AnonymousMethodExpressionPattern(TokenListPattern modifiers, ParameterListPattern parameterList, Action<AnonymousMethodExpressionSyntax> action)
            : base(modifiers)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousMethodExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousMethodExpressionSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1385
    public abstract partial class LambdaExpressionPattern : AnonymousFunctionExpressionPattern
    {

        internal LambdaExpressionPattern(TokenListPattern modifiers)
            : base(modifiers)
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LambdaExpressionSyntax typed))
                return false;


            return true;
        }
    }

    // #1397
    public partial class SimpleLambdaExpressionPattern : LambdaExpressionPattern
    {
        private readonly ParameterPattern _parameter;
        private readonly Action<SimpleLambdaExpressionSyntax> _action;

        internal SimpleLambdaExpressionPattern(TokenListPattern modifiers, ParameterPattern parameter, Action<SimpleLambdaExpressionSyntax> action)
            : base(modifiers)
        {
            _parameter = parameter;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleLambdaExpressionSyntax typed))
                return false;

            if (_parameter != null && !_parameter.Test(typed.Parameter, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SimpleLambdaExpressionSyntax)node;

            if (_parameter != null)
                _parameter.RunCallback(typed.Parameter, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1438
    public partial class RefExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<RefExpressionSyntax> _action;

        internal RefExpressionPattern(ExpressionPattern expression, Action<RefExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RefExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RefExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1445
    public partial class ParenthesizedLambdaExpressionPattern : LambdaExpressionPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<ParenthesizedLambdaExpressionSyntax> _action;

        internal ParenthesizedLambdaExpressionPattern(TokenListPattern modifiers, ParameterListPattern parameterList, Action<ParenthesizedLambdaExpressionSyntax> action)
            : base(modifiers)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedLambdaExpressionSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedLambdaExpressionSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1485
    public partial class InitializerExpressionPattern : ExpressionPattern
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<ExpressionPattern> _expressions;
        private readonly Action<InitializerExpressionSyntax> _action;

        internal InitializerExpressionPattern(SyntaxKind kind, NodeListPattern<ExpressionPattern> expressions, Action<InitializerExpressionSyntax> action)
        {
            _kind = kind;
            _expressions = expressions;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InitializerExpressionSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expressions != null && !_expressions.Test(typed.Expressions, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InitializerExpressionSyntax)node;

            if (_expressions != null)
                _expressions.RunCallback(typed.Expressions, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1515
    public abstract partial class BaseObjectCreationExpressionPattern : ExpressionPattern
    {
        private readonly ArgumentListPattern _argumentList;
        private readonly InitializerExpressionPattern _initializer;

        internal BaseObjectCreationExpressionPattern(ArgumentListPattern argumentList, InitializerExpressionPattern initializer)
        {
            _argumentList = argumentList;
            _initializer = initializer;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseObjectCreationExpressionSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseObjectCreationExpressionSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);
        }
    }

    // #1533
    public partial class ImplicitObjectCreationExpressionPattern : BaseObjectCreationExpressionPattern
    {
        private readonly Action<ImplicitObjectCreationExpressionSyntax> _action;

        internal ImplicitObjectCreationExpressionPattern(ArgumentListPattern argumentList, InitializerExpressionPattern initializer, Action<ImplicitObjectCreationExpressionSyntax> action)
            : base(argumentList, initializer)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitObjectCreationExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ImplicitObjectCreationExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1558
    public partial class ObjectCreationExpressionPattern : BaseObjectCreationExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly Action<ObjectCreationExpressionSyntax> _action;

        internal ObjectCreationExpressionPattern(ArgumentListPattern argumentList, InitializerExpressionPattern initializer, TypePattern type, Action<ObjectCreationExpressionSyntax> action)
            : base(argumentList, initializer)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ObjectCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ObjectCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1588
    public partial class WithExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<WithExpressionSyntax> _action;

        internal WithExpressionPattern(ExpressionPattern expression, InitializerExpressionPattern initializer, Action<WithExpressionSyntax> action)
        {
            _expression = expression;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WithExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WithExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1600
    public partial class AnonymousObjectMemberDeclaratorPattern : PatternNode
    {
        private readonly NameEqualsPattern _nameEquals;
        private readonly ExpressionPattern _expression;
        private readonly Action<AnonymousObjectMemberDeclaratorSyntax> _action;

        internal AnonymousObjectMemberDeclaratorPattern(NameEqualsPattern nameEquals, ExpressionPattern expression, Action<AnonymousObjectMemberDeclaratorSyntax> action)
        {
            _nameEquals = nameEquals;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectMemberDeclaratorSyntax typed))
                return false;

            if (_nameEquals != null && !_nameEquals.Test(typed.NameEquals, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectMemberDeclaratorSyntax)node;

            if (_nameEquals != null)
                _nameEquals.RunCallback(typed.NameEquals, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1616
    public partial class AnonymousObjectCreationExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<AnonymousObjectMemberDeclaratorPattern> _initializers;
        private readonly Action<AnonymousObjectCreationExpressionSyntax> _action;

        internal AnonymousObjectCreationExpressionPattern(NodeListPattern<AnonymousObjectMemberDeclaratorPattern> initializers, Action<AnonymousObjectCreationExpressionSyntax> action)
        {
            _initializers = initializers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AnonymousObjectCreationExpressionSyntax typed))
                return false;

            if (_initializers != null && !_initializers.Test(typed.Initializers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AnonymousObjectCreationExpressionSyntax)node;

            if (_initializers != null)
                _initializers.RunCallback(typed.Initializers, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1648
    public partial class ArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly ArrayTypePattern _type;
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ArrayCreationExpressionSyntax> _action;

        internal ArrayCreationExpressionPattern(ArrayTypePattern type, InitializerExpressionPattern initializer, Action<ArrayCreationExpressionSyntax> action)
        {
            _type = type;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrayCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1673
    public partial class ImplicitArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ImplicitArrayCreationExpressionSyntax> _action;

        internal ImplicitArrayCreationExpressionPattern(InitializerExpressionPattern initializer, Action<ImplicitArrayCreationExpressionSyntax> action)
        {
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitArrayCreationExpressionSyntax typed))
                return false;

            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitArrayCreationExpressionSyntax)node;

            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1710
    public partial class StackAllocArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly TypePattern _type;
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<StackAllocArrayCreationExpressionSyntax> _action;

        internal StackAllocArrayCreationExpressionPattern(TypePattern type, InitializerExpressionPattern initializer, Action<StackAllocArrayCreationExpressionSyntax> action)
        {
            _type = type;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StackAllocArrayCreationExpressionSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (StackAllocArrayCreationExpressionSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1735
    public partial class ImplicitStackAllocArrayCreationExpressionPattern : ExpressionPattern
    {
        private readonly InitializerExpressionPattern _initializer;
        private readonly Action<ImplicitStackAllocArrayCreationExpressionSyntax> _action;

        internal ImplicitStackAllocArrayCreationExpressionPattern(InitializerExpressionPattern initializer, Action<ImplicitStackAllocArrayCreationExpressionSyntax> action)
        {
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ImplicitStackAllocArrayCreationExpressionSyntax typed))
                return false;

            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ImplicitStackAllocArrayCreationExpressionSyntax)node;

            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1767
    public abstract partial class QueryClausePattern : PatternNode
    {

        internal QueryClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryClauseSyntax typed))
                return false;


            return true;
        }
    }

    // #1769
    public abstract partial class SelectOrGroupClausePattern : PatternNode
    {

        internal SelectOrGroupClausePattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectOrGroupClauseSyntax typed))
                return false;


            return true;
        }
    }

    // #1771
    public partial class QueryExpressionPattern : ExpressionPattern
    {
        private readonly FromClausePattern _fromClause;
        private readonly QueryBodyPattern _body;
        private readonly Action<QueryExpressionSyntax> _action;

        internal QueryExpressionPattern(FromClausePattern fromClause, QueryBodyPattern body, Action<QueryExpressionSyntax> action)
        {
            _fromClause = fromClause;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryExpressionSyntax typed))
                return false;

            if (_fromClause != null && !_fromClause.Test(typed.FromClause, semanticModel))
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryExpressionSyntax)node;

            if (_fromClause != null)
                _fromClause.RunCallback(typed.FromClause, semanticModel);
            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1776
    public partial class QueryBodyPattern : PatternNode
    {
        private readonly NodeListPattern<QueryClausePattern> _clauses;
        private readonly SelectOrGroupClausePattern _selectOrGroup;
        private readonly QueryContinuationPattern _continuation;
        private readonly Action<QueryBodySyntax> _action;

        internal QueryBodyPattern(NodeListPattern<QueryClausePattern> clauses, SelectOrGroupClausePattern selectOrGroup, QueryContinuationPattern continuation, Action<QueryBodySyntax> action)
        {
            _clauses = clauses;
            _selectOrGroup = selectOrGroup;
            _continuation = continuation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryBodySyntax typed))
                return false;

            if (_clauses != null && !_clauses.Test(typed.Clauses, semanticModel))
                return false;
            if (_selectOrGroup != null && !_selectOrGroup.Test(typed.SelectOrGroup, semanticModel))
                return false;
            if (_continuation != null && !_continuation.Test(typed.Continuation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryBodySyntax)node;

            if (_clauses != null)
                _clauses.RunCallback(typed.Clauses, semanticModel);
            if (_selectOrGroup != null)
                _selectOrGroup.RunCallback(typed.SelectOrGroup, semanticModel);
            if (_continuation != null)
                _continuation.RunCallback(typed.Continuation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1782
    public partial class FromClausePattern : QueryClausePattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly ExpressionPattern _expression;
        private readonly Action<FromClauseSyntax> _action;

        internal FromClausePattern(TypePattern type, string identifier, ExpressionPattern expression, Action<FromClauseSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FromClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FromClauseSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1799
    public partial class LetClausePattern : QueryClausePattern
    {
        private readonly string _identifier;
        private readonly ExpressionPattern _expression;
        private readonly Action<LetClauseSyntax> _action;

        internal LetClausePattern(string identifier, ExpressionPattern expression, Action<LetClauseSyntax> action)
        {
            _identifier = identifier;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LetClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (LetClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1815
    public partial class JoinClausePattern : QueryClausePattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly ExpressionPattern _inExpression;
        private readonly ExpressionPattern _leftExpression;
        private readonly ExpressionPattern _rightExpression;
        private readonly JoinIntoClausePattern _into;
        private readonly Action<JoinClauseSyntax> _action;

        internal JoinClausePattern(TypePattern type, string identifier, ExpressionPattern inExpression, ExpressionPattern leftExpression, ExpressionPattern rightExpression, JoinIntoClausePattern into, Action<JoinClauseSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _inExpression = inExpression;
            _leftExpression = leftExpression;
            _rightExpression = rightExpression;
            _into = into;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinClauseSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_inExpression != null && !_inExpression.Test(typed.InExpression, semanticModel))
                return false;
            if (_leftExpression != null && !_leftExpression.Test(typed.LeftExpression, semanticModel))
                return false;
            if (_rightExpression != null && !_rightExpression.Test(typed.RightExpression, semanticModel))
                return false;
            if (_into != null && !_into.Test(typed.Into, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinClauseSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_inExpression != null)
                _inExpression.RunCallback(typed.InExpression, semanticModel);
            if (_leftExpression != null)
                _leftExpression.RunCallback(typed.LeftExpression, semanticModel);
            if (_rightExpression != null)
                _rightExpression.RunCallback(typed.RightExpression, semanticModel);
            if (_into != null)
                _into.RunCallback(typed.Into, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1841
    public partial class JoinIntoClausePattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<JoinIntoClauseSyntax> _action;

        internal JoinIntoClausePattern(string identifier, Action<JoinIntoClauseSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is JoinIntoClauseSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (JoinIntoClauseSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1853
    public partial class WhereClausePattern : QueryClausePattern
    {
        private readonly ExpressionPattern _condition;
        private readonly Action<WhereClauseSyntax> _action;

        internal WhereClausePattern(ExpressionPattern condition, Action<WhereClauseSyntax> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhereClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhereClauseSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1860
    public partial class OrderByClausePattern : QueryClausePattern
    {
        private readonly NodeListPattern<OrderingPattern> _orderings;
        private readonly Action<OrderByClauseSyntax> _action;

        internal OrderByClausePattern(NodeListPattern<OrderingPattern> orderings, Action<OrderByClauseSyntax> action)
        {
            _orderings = orderings;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderByClauseSyntax typed))
                return false;

            if (_orderings != null && !_orderings.Test(typed.Orderings, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderByClauseSyntax)node;

            if (_orderings != null)
                _orderings.RunCallback(typed.Orderings, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1867
    public partial class OrderingPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<OrderingSyntax> _action;

        internal OrderingPattern(SyntaxKind kind, ExpressionPattern expression, Action<OrderingSyntax> action)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OrderingSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OrderingSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1876
    public partial class SelectClausePattern : SelectOrGroupClausePattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<SelectClauseSyntax> _action;

        internal SelectClausePattern(ExpressionPattern expression, Action<SelectClauseSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SelectClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SelectClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1883
    public partial class GroupClausePattern : SelectOrGroupClausePattern
    {
        private readonly ExpressionPattern _groupExpression;
        private readonly ExpressionPattern _byExpression;
        private readonly Action<GroupClauseSyntax> _action;

        internal GroupClausePattern(ExpressionPattern groupExpression, ExpressionPattern byExpression, Action<GroupClauseSyntax> action)
        {
            _groupExpression = groupExpression;
            _byExpression = byExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GroupClauseSyntax typed))
                return false;

            if (_groupExpression != null && !_groupExpression.Test(typed.GroupExpression, semanticModel))
                return false;
            if (_byExpression != null && !_byExpression.Test(typed.ByExpression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (GroupClauseSyntax)node;

            if (_groupExpression != null)
                _groupExpression.RunCallback(typed.GroupExpression, semanticModel);
            if (_byExpression != null)
                _byExpression.RunCallback(typed.ByExpression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1894
    public partial class QueryContinuationPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly QueryBodyPattern _body;
        private readonly Action<QueryContinuationSyntax> _action;

        internal QueryContinuationPattern(string identifier, QueryBodyPattern body, Action<QueryContinuationSyntax> action)
        {
            _identifier = identifier;
            _body = body;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is QueryContinuationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_body != null && !_body.Test(typed.Body, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (QueryContinuationSyntax)node;

            if (_body != null)
                _body.RunCallback(typed.Body, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1907
    public partial class OmittedArraySizeExpressionPattern : ExpressionPattern
    {
        private readonly Action<OmittedArraySizeExpressionSyntax> _action;

        internal OmittedArraySizeExpressionPattern(Action<OmittedArraySizeExpressionSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OmittedArraySizeExpressionSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (OmittedArraySizeExpressionSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1922
    public partial class InterpolatedStringExpressionPattern : ExpressionPattern
    {
        private readonly NodeListPattern<InterpolatedStringContentPattern> _contents;
        private readonly Action<InterpolatedStringExpressionSyntax> _action;

        internal InterpolatedStringExpressionPattern(NodeListPattern<InterpolatedStringContentPattern> contents, Action<InterpolatedStringExpressionSyntax> action)
        {
            _contents = contents;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringExpressionSyntax typed))
                return false;

            if (_contents != null && !_contents.Test(typed.Contents, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringExpressionSyntax)node;

            if (_contents != null)
                _contents.RunCallback(typed.Contents, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1943
    public partial class IsPatternExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly PatternPattern _pattern;
        private readonly Action<IsPatternExpressionSyntax> _action;

        internal IsPatternExpressionPattern(ExpressionPattern expression, PatternPattern pattern, Action<IsPatternExpressionSyntax> action)
        {
            _expression = expression;
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IsPatternExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (IsPatternExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1965
    public partial class ThrowExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ThrowExpressionSyntax> _action;

        internal ThrowExpressionPattern(ExpressionPattern expression, Action<ThrowExpressionSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowExpressionSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ThrowExpressionSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1972
    public partial class WhenClausePattern : PatternNode
    {
        private readonly ExpressionPattern _condition;
        private readonly Action<WhenClauseSyntax> _action;

        internal WhenClausePattern(ExpressionPattern condition, Action<WhenClauseSyntax> action)
        {
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhenClauseSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (WhenClauseSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1979
    public abstract partial class PatternPattern : ExpressionOrPatternPattern
    {

        internal PatternPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PatternSyntax typed))
                return false;


            return true;
        }
    }

    // #1980
    public partial class DiscardPatternPattern : PatternPattern
    {
        private readonly Action<DiscardPatternSyntax> _action;

        internal DiscardPatternPattern(Action<DiscardPatternSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DiscardPatternSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DiscardPatternSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #1986
    public partial class DeclarationPatternPattern : PatternPattern
    {
        private readonly TypePattern _type;
        private readonly VariableDesignationPattern _designation;
        private readonly Action<DeclarationPatternSyntax> _action;

        internal DeclarationPatternPattern(TypePattern type, VariableDesignationPattern designation, Action<DeclarationPatternSyntax> action)
        {
            _type = type;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DeclarationPatternSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DeclarationPatternSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #1994
    public partial class VarPatternPattern : PatternPattern
    {
        private readonly VariableDesignationPattern _designation;
        private readonly Action<VarPatternSyntax> _action;

        internal VarPatternPattern(VariableDesignationPattern designation, Action<VarPatternSyntax> action)
        {
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VarPatternSyntax typed))
                return false;

            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VarPatternSyntax)node;

            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2001
    public partial class RecursivePatternPattern : PatternPattern
    {
        private readonly TypePattern _type;
        private readonly PositionalPatternClausePattern _positionalPatternClause;
        private readonly PropertyPatternClausePattern _propertyPatternClause;
        private readonly VariableDesignationPattern _designation;
        private readonly Action<RecursivePatternSyntax> _action;

        internal RecursivePatternPattern(TypePattern type, PositionalPatternClausePattern positionalPatternClause, PropertyPatternClausePattern propertyPatternClause, VariableDesignationPattern designation, Action<RecursivePatternSyntax> action)
        {
            _type = type;
            _positionalPatternClause = positionalPatternClause;
            _propertyPatternClause = propertyPatternClause;
            _designation = designation;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RecursivePatternSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_positionalPatternClause != null && !_positionalPatternClause.Test(typed.PositionalPatternClause, semanticModel))
                return false;
            if (_propertyPatternClause != null && !_propertyPatternClause.Test(typed.PropertyPatternClause, semanticModel))
                return false;
            if (_designation != null && !_designation.Test(typed.Designation, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RecursivePatternSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_positionalPatternClause != null)
                _positionalPatternClause.RunCallback(typed.PositionalPatternClause, semanticModel);
            if (_propertyPatternClause != null)
                _propertyPatternClause.RunCallback(typed.PropertyPatternClause, semanticModel);
            if (_designation != null)
                _designation.RunCallback(typed.Designation, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2011
    public partial class PositionalPatternClausePattern : PatternNode
    {
        private readonly NodeListPattern<SubpatternPattern> _subpatterns;
        private readonly Action<PositionalPatternClauseSyntax> _action;

        internal PositionalPatternClausePattern(NodeListPattern<SubpatternPattern> subpatterns, Action<PositionalPatternClauseSyntax> action)
        {
            _subpatterns = subpatterns;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PositionalPatternClauseSyntax typed))
                return false;

            if (_subpatterns != null && !_subpatterns.Test(typed.Subpatterns, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PositionalPatternClauseSyntax)node;

            if (_subpatterns != null)
                _subpatterns.RunCallback(typed.Subpatterns, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2021
    public partial class PropertyPatternClausePattern : PatternNode
    {
        private readonly NodeListPattern<SubpatternPattern> _subpatterns;
        private readonly Action<PropertyPatternClauseSyntax> _action;

        internal PropertyPatternClausePattern(NodeListPattern<SubpatternPattern> subpatterns, Action<PropertyPatternClauseSyntax> action)
        {
            _subpatterns = subpatterns;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PropertyPatternClauseSyntax typed))
                return false;

            if (_subpatterns != null && !_subpatterns.Test(typed.Subpatterns, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (PropertyPatternClauseSyntax)node;

            if (_subpatterns != null)
                _subpatterns.RunCallback(typed.Subpatterns, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2031
    public partial class SubpatternPattern : PatternNode
    {
        private readonly NameColonPattern _nameColon;
        private readonly PatternPattern _pattern;
        private readonly Action<SubpatternSyntax> _action;

        internal SubpatternPattern(NameColonPattern nameColon, PatternPattern pattern, Action<SubpatternSyntax> action)
        {
            _nameColon = nameColon;
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SubpatternSyntax typed))
                return false;

            if (_nameColon != null && !_nameColon.Test(typed.NameColon, semanticModel))
                return false;
            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SubpatternSyntax)node;

            if (_nameColon != null)
                _nameColon.RunCallback(typed.NameColon, semanticModel);
            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2036
    public partial class ConstantPatternPattern : PatternPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ConstantPatternSyntax> _action;

        internal ConstantPatternPattern(ExpressionPattern expression, Action<ConstantPatternSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstantPatternSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstantPatternSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2046
    public partial class ParenthesizedPatternPattern : PatternPattern
    {
        private readonly PatternPattern _pattern;
        private readonly Action<ParenthesizedPatternSyntax> _action;

        internal ParenthesizedPatternPattern(PatternPattern pattern, Action<ParenthesizedPatternSyntax> action)
        {
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedPatternSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedPatternSyntax)node;

            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2056
    public partial class RelationalPatternPattern : PatternPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<RelationalPatternSyntax> _action;

        internal RelationalPatternPattern(ExpressionPattern expression, Action<RelationalPatternSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RelationalPatternSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (RelationalPatternSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2071
    public partial class TypePatternPattern : PatternPattern
    {
        private readonly TypePattern _type;
        private readonly Action<TypePatternSyntax> _action;

        internal TypePatternPattern(TypePattern type, Action<TypePatternSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypePatternSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypePatternSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2079
    public partial class BinaryPatternPattern : PatternPattern
    {
        private readonly SyntaxKind _kind;
        private readonly PatternPattern _left;
        private readonly PatternPattern _right;
        private readonly Action<BinaryPatternSyntax> _action;

        internal BinaryPatternPattern(SyntaxKind kind, PatternPattern left, PatternPattern right, Action<BinaryPatternSyntax> action)
        {
            _kind = kind;
            _left = left;
            _right = right;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BinaryPatternSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_left != null && !_left.Test(typed.Left, semanticModel))
                return false;
            if (_right != null && !_right.Test(typed.Right, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BinaryPatternSyntax)node;

            if (_left != null)
                _left.RunCallback(typed.Left, semanticModel);
            if (_right != null)
                _right.RunCallback(typed.Right, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2089
    public partial class UnaryPatternPattern : PatternPattern
    {
        private readonly PatternPattern _pattern;
        private readonly Action<UnaryPatternSyntax> _action;

        internal UnaryPatternPattern(PatternPattern pattern, Action<UnaryPatternSyntax> action)
        {
            _pattern = pattern;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UnaryPatternSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UnaryPatternSyntax)node;

            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2097
    public abstract partial class InterpolatedStringContentPattern : PatternNode
    {

        internal InterpolatedStringContentPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringContentSyntax typed))
                return false;


            return true;
        }
    }

    // #2098
    public partial class InterpolatedStringTextPattern : InterpolatedStringContentPattern
    {
        private readonly Action<InterpolatedStringTextSyntax> _action;

        internal InterpolatedStringTextPattern(Action<InterpolatedStringTextSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolatedStringTextSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolatedStringTextSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2107
    public partial class InterpolationPattern : InterpolatedStringContentPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly InterpolationAlignmentClausePattern _alignmentClause;
        private readonly InterpolationFormatClausePattern _formatClause;
        private readonly Action<InterpolationSyntax> _action;

        internal InterpolationPattern(ExpressionPattern expression, InterpolationAlignmentClausePattern alignmentClause, InterpolationFormatClausePattern formatClause, Action<InterpolationSyntax> action)
        {
            _expression = expression;
            _alignmentClause = alignmentClause;
            _formatClause = formatClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_alignmentClause != null && !_alignmentClause.Test(typed.AlignmentClause, semanticModel))
                return false;
            if (_formatClause != null && !_formatClause.Test(typed.FormatClause, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_alignmentClause != null)
                _alignmentClause.RunCallback(typed.AlignmentClause, semanticModel);
            if (_formatClause != null)
                _formatClause.RunCallback(typed.FormatClause, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2119
    public partial class InterpolationAlignmentClausePattern : PatternNode
    {
        private readonly ExpressionPattern _value;
        private readonly Action<InterpolationAlignmentClauseSyntax> _action;

        internal InterpolationAlignmentClausePattern(ExpressionPattern value, Action<InterpolationAlignmentClauseSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationAlignmentClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationAlignmentClauseSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2124
    public partial class InterpolationFormatClausePattern : PatternNode
    {
        private readonly Action<InterpolationFormatClauseSyntax> _action;

        internal InterpolationFormatClausePattern(Action<InterpolationFormatClauseSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterpolationFormatClauseSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (InterpolationFormatClauseSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2135
    public partial class GlobalStatementPattern : MemberDeclarationPattern
    {
        private readonly StatementPattern _statement;
        private readonly Action<GlobalStatementSyntax> _action;

        internal GlobalStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, StatementPattern statement, Action<GlobalStatementSyntax> action)
            : base(attributeLists, modifiers)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GlobalStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (GlobalStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2145
    public abstract partial class StatementPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;

        internal StatementPattern(NodeListPattern<AttributeListPattern> attributeLists)
        {
            _attributeLists = attributeLists;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StatementSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (StatementSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
        }
    }

    // #2151
    public partial class BlockPattern : StatementPattern
    {
        private readonly NodeListPattern<StatementPattern> _statements;
        private readonly Action<BlockSyntax> _action;

        internal BlockPattern(NodeListPattern<AttributeListPattern> attributeLists, NodeListPattern<StatementPattern> statements, Action<BlockSyntax> action)
            : base(attributeLists)
        {
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BlockSyntax typed))
                return false;

            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BlockSyntax)node;

            if (_statements != null)
                _statements.RunCallback(typed.Statements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2162
    public partial class LocalFunctionStatementPattern : StatementPattern
    {
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly ParameterListPattern _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly Action<LocalFunctionStatementSyntax> _action;

        internal LocalFunctionStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern returnType, string identifier, TypeParameterListPattern typeParameterList, ParameterListPattern parameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, Action<LocalFunctionStatementSyntax> action)
            : base(attributeLists)
        {
            _modifiers = modifiers;
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalFunctionStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (LocalFunctionStatementSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2190
    public partial class LocalDeclarationStatementPattern : StatementPattern
    {
        private readonly TokenListPattern _modifiers;
        private readonly VariableDeclarationPattern _declaration;
        private readonly Action<LocalDeclarationStatementSyntax> _action;

        internal LocalDeclarationStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<LocalDeclarationStatementSyntax> action)
            : base(attributeLists)
        {
            _modifiers = modifiers;
            _declaration = declaration;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LocalDeclarationStatementSyntax typed))
                return false;

            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (LocalDeclarationStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2210
    public partial class VariableDeclarationPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly NodeListPattern<VariableDeclaratorPattern> _variables;
        private readonly Action<VariableDeclarationSyntax> _action;

        internal VariableDeclarationPattern(TypePattern type, NodeListPattern<VariableDeclaratorPattern> variables, Action<VariableDeclarationSyntax> action)
        {
            _type = type;
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_variables != null)
                _variables.RunCallback(typed.Variables, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2216
    public partial class VariableDeclaratorPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly BracketedArgumentListPattern _argumentList;
        private readonly EqualsValueClausePattern _initializer;
        private readonly Action<VariableDeclaratorSyntax> _action;

        internal VariableDeclaratorPattern(string identifier, BracketedArgumentListPattern argumentList, EqualsValueClausePattern initializer, Action<VariableDeclaratorSyntax> action)
        {
            _identifier = identifier;
            _argumentList = argumentList;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDeclaratorSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (VariableDeclaratorSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);
            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2227
    public partial class EqualsValueClausePattern : PatternNode
    {
        private readonly ExpressionPattern _value;
        private readonly Action<EqualsValueClauseSyntax> _action;

        internal EqualsValueClausePattern(ExpressionPattern value, Action<EqualsValueClauseSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EqualsValueClauseSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (EqualsValueClauseSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2234
    public abstract partial class VariableDesignationPattern : PatternNode
    {

        internal VariableDesignationPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is VariableDesignationSyntax typed))
                return false;


            return true;
        }
    }

    // #2236
    public partial class SingleVariableDesignationPattern : VariableDesignationPattern
    {
        private readonly string _identifier;
        private readonly Action<SingleVariableDesignationSyntax> _action;

        internal SingleVariableDesignationPattern(string identifier, Action<SingleVariableDesignationSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SingleVariableDesignationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SingleVariableDesignationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2242
    public partial class DiscardDesignationPattern : VariableDesignationPattern
    {
        private readonly Action<DiscardDesignationSyntax> _action;

        internal DiscardDesignationPattern(Action<DiscardDesignationSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DiscardDesignationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DiscardDesignationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2248
    public partial class ParenthesizedVariableDesignationPattern : VariableDesignationPattern
    {
        private readonly NodeListPattern<VariableDesignationPattern> _variables;
        private readonly Action<ParenthesizedVariableDesignationSyntax> _action;

        internal ParenthesizedVariableDesignationPattern(NodeListPattern<VariableDesignationPattern> variables, Action<ParenthesizedVariableDesignationSyntax> action)
        {
            _variables = variables;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParenthesizedVariableDesignationSyntax typed))
                return false;

            if (_variables != null && !_variables.Test(typed.Variables, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ParenthesizedVariableDesignationSyntax)node;

            if (_variables != null)
                _variables.RunCallback(typed.Variables, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2259
    public partial class ExpressionStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ExpressionStatementSyntax> _action;

        internal ExpressionStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, Action<ExpressionStatementSyntax> action)
            : base(attributeLists)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExpressionStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ExpressionStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2268
    public partial class EmptyStatementPattern : StatementPattern
    {
        private readonly Action<EmptyStatementSyntax> _action;

        internal EmptyStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, Action<EmptyStatementSyntax> action)
            : base(attributeLists)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EmptyStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EmptyStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2275
    public partial class LabeledStatementPattern : StatementPattern
    {
        private readonly string _identifier;
        private readonly StatementPattern _statement;
        private readonly Action<LabeledStatementSyntax> _action;

        internal LabeledStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, string identifier, StatementPattern statement, Action<LabeledStatementSyntax> action)
            : base(attributeLists)
        {
            _identifier = identifier;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LabeledStatementSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (LabeledStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2298
    public partial class GotoStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<GotoStatementSyntax> _action;

        internal GotoStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, SyntaxKind kind, ExpressionPattern expression, Action<GotoStatementSyntax> action)
            : base(attributeLists)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is GotoStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (GotoStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2346
    public partial class BreakStatementPattern : StatementPattern
    {
        private readonly Action<BreakStatementSyntax> _action;

        internal BreakStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, Action<BreakStatementSyntax> action)
            : base(attributeLists)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BreakStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BreakStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2356
    public partial class ContinueStatementPattern : StatementPattern
    {
        private readonly Action<ContinueStatementSyntax> _action;

        internal ContinueStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, Action<ContinueStatementSyntax> action)
            : base(attributeLists)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ContinueStatementSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ContinueStatementSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2366
    public partial class ReturnStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ReturnStatementSyntax> _action;

        internal ReturnStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, Action<ReturnStatementSyntax> action)
            : base(attributeLists)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ReturnStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ReturnStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2377
    public partial class ThrowStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ThrowStatementSyntax> _action;

        internal ThrowStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, Action<ThrowStatementSyntax> action)
            : base(attributeLists)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ThrowStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ThrowStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2388
    public partial class YieldStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly ExpressionPattern _expression;
        private readonly Action<YieldStatementSyntax> _action;

        internal YieldStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, SyntaxKind kind, ExpressionPattern expression, Action<YieldStatementSyntax> action)
            : base(attributeLists)
        {
            _kind = kind;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is YieldStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (YieldStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2404
    public partial class WhileStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly StatementPattern _statement;
        private readonly Action<WhileStatementSyntax> _action;

        internal WhileStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern condition, StatementPattern statement, Action<WhileStatementSyntax> action)
            : base(attributeLists)
        {
            _condition = condition;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is WhileStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (WhileStatementSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2419
    public partial class DoStatementPattern : StatementPattern
    {
        private readonly StatementPattern _statement;
        private readonly ExpressionPattern _condition;
        private readonly Action<DoStatementSyntax> _action;

        internal DoStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, StatementPattern statement, ExpressionPattern condition, Action<DoStatementSyntax> action)
            : base(attributeLists)
        {
            _statement = statement;
            _condition = condition;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DoStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (DoStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2440
    public partial class ForStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly NodeListPattern<ExpressionPattern> _incrementors;
        private readonly StatementPattern _statement;
        private readonly Action<ForStatementSyntax> _action;

        internal ForStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern condition, NodeListPattern<ExpressionPattern> incrementors, StatementPattern statement, Action<ForStatementSyntax> action)
            : base(attributeLists)
        {
            _condition = condition;
            _incrementors = incrementors;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_incrementors != null && !_incrementors.Test(typed.Incrementors, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ForStatementSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_incrementors != null)
                _incrementors.RunCallback(typed.Incrementors, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2469
    public abstract partial class CommonForEachStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly StatementPattern _statement;

        internal CommonForEachStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, StatementPattern statement)
            : base(attributeLists)
        {
            _expression = expression;
            _statement = statement;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CommonForEachStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (CommonForEachStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
        }
    }

    // #2489
    public partial class ForEachStatementPattern : CommonForEachStatementPattern
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<ForEachStatementSyntax> _action;

        internal ForEachStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, StatementPattern statement, TypePattern type, string identifier, Action<ForEachStatementSyntax> action)
            : base(attributeLists, expression, statement)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachStatementSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ForEachStatementSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2521
    public partial class ForEachVariableStatementPattern : CommonForEachStatementPattern
    {
        private readonly ExpressionPattern _variable;
        private readonly Action<ForEachVariableStatementSyntax> _action;

        internal ForEachVariableStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, StatementPattern statement, ExpressionPattern variable, Action<ForEachVariableStatementSyntax> action)
            : base(attributeLists, expression, statement)
        {
            _variable = variable;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ForEachVariableStatementSyntax typed))
                return false;

            if (_variable != null && !_variable.Test(typed.Variable, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ForEachVariableStatementSyntax)node;

            if (_variable != null)
                _variable.RunCallback(typed.Variable, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2557
    public partial class UsingStatementPattern : StatementPattern
    {
        private readonly StatementPattern _statement;
        private readonly Action<UsingStatementSyntax> _action;

        internal UsingStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, StatementPattern statement, Action<UsingStatementSyntax> action)
            : base(attributeLists)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingStatementSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (UsingStatementSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2579
    public partial class FixedStatementPattern : StatementPattern
    {
        private readonly VariableDeclarationPattern _declaration;
        private readonly StatementPattern _statement;
        private readonly Action<FixedStatementSyntax> _action;

        internal FixedStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, VariableDeclarationPattern declaration, StatementPattern statement, Action<FixedStatementSyntax> action)
            : base(attributeLists)
        {
            _declaration = declaration;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FixedStatementSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (FixedStatementSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2594
    public partial class CheckedStatementPattern : StatementPattern
    {
        private readonly SyntaxKind _kind;
        private readonly BlockPattern _block;
        private readonly Action<CheckedStatementSyntax> _action;

        internal CheckedStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, SyntaxKind kind, BlockPattern block, Action<CheckedStatementSyntax> action)
            : base(attributeLists)
        {
            _kind = kind;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CheckedStatementSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (CheckedStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2604
    public partial class UnsafeStatementPattern : StatementPattern
    {
        private readonly BlockPattern _block;
        private readonly Action<UnsafeStatementSyntax> _action;

        internal UnsafeStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, BlockPattern block, Action<UnsafeStatementSyntax> action)
            : base(attributeLists)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UnsafeStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (UnsafeStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2612
    public partial class LockStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly StatementPattern _statement;
        private readonly Action<LockStatementSyntax> _action;

        internal LockStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, StatementPattern statement, Action<LockStatementSyntax> action)
            : base(attributeLists)
        {
            _expression = expression;
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is LockStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (LockStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2627
    public partial class IfStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _condition;
        private readonly StatementPattern _statement;
        private readonly ElseClausePattern _else;
        private readonly Action<IfStatementSyntax> _action;

        internal IfStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern condition, StatementPattern statement, ElseClausePattern @else, Action<IfStatementSyntax> action)
            : base(attributeLists)
        {
            _condition = condition;
            _statement = statement;
            _else = @else;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IfStatementSyntax typed))
                return false;

            if (_condition != null && !_condition.Test(typed.Condition, semanticModel))
                return false;
            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;
            if (_else != null && !_else.Test(typed.Else, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (IfStatementSyntax)node;

            if (_condition != null)
                _condition.RunCallback(typed.Condition, semanticModel);
            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);
            if (_else != null)
                _else.RunCallback(typed.Else, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2684
    public partial class ElseClausePattern : PatternNode
    {
        private readonly StatementPattern _statement;
        private readonly Action<ElseClauseSyntax> _action;

        internal ElseClausePattern(StatementPattern statement, Action<ElseClauseSyntax> action)
        {
            _statement = statement;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ElseClauseSyntax typed))
                return false;

            if (_statement != null && !_statement.Test(typed.Statement, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ElseClauseSyntax)node;

            if (_statement != null)
                _statement.RunCallback(typed.Statement, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2703
    public partial class SwitchStatementPattern : StatementPattern
    {
        private readonly ExpressionPattern _expression;
        private readonly NodeListPattern<SwitchSectionPattern> _sections;
        private readonly Action<SwitchStatementSyntax> _action;

        internal SwitchStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, ExpressionPattern expression, NodeListPattern<SwitchSectionPattern> sections, Action<SwitchStatementSyntax> action)
            : base(attributeLists)
        {
            _expression = expression;
            _sections = sections;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchStatementSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;
            if (_sections != null && !_sections.Test(typed.Sections, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (SwitchStatementSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);
            if (_sections != null)
                _sections.RunCallback(typed.Sections, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2767
    public partial class SwitchSectionPattern : PatternNode
    {
        private readonly NodeListPattern<SwitchLabelPattern> _labels;
        private readonly NodeListPattern<StatementPattern> _statements;
        private readonly Action<SwitchSectionSyntax> _action;

        internal SwitchSectionPattern(NodeListPattern<SwitchLabelPattern> labels, NodeListPattern<StatementPattern> statements, Action<SwitchSectionSyntax> action)
        {
            _labels = labels;
            _statements = statements;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchSectionSyntax typed))
                return false;

            if (_labels != null && !_labels.Test(typed.Labels, semanticModel))
                return false;
            if (_statements != null && !_statements.Test(typed.Statements, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchSectionSyntax)node;

            if (_labels != null)
                _labels.RunCallback(typed.Labels, semanticModel);
            if (_statements != null)
                _statements.RunCallback(typed.Statements, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2790
    public abstract partial class SwitchLabelPattern : PatternNode
    {

        internal SwitchLabelPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchLabelSyntax typed))
                return false;


            return true;
        }
    }

    // #2810
    public partial class CasePatternSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly PatternPattern _pattern;
        private readonly WhenClausePattern _whenClause;
        private readonly Action<CasePatternSwitchLabelSyntax> _action;

        internal CasePatternSwitchLabelPattern(PatternPattern pattern, WhenClausePattern whenClause, Action<CasePatternSwitchLabelSyntax> action)
        {
            _pattern = pattern;
            _whenClause = whenClause;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CasePatternSwitchLabelSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;
            if (_whenClause != null && !_whenClause.Test(typed.WhenClause, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CasePatternSwitchLabelSyntax)node;

            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);
            if (_whenClause != null)
                _whenClause.RunCallback(typed.WhenClause, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2834
    public partial class CaseSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly ExpressionPattern _value;
        private readonly Action<CaseSwitchLabelSyntax> _action;

        internal CaseSwitchLabelPattern(ExpressionPattern value, Action<CaseSwitchLabelSyntax> action)
        {
            _value = value;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CaseSwitchLabelSyntax typed))
                return false;

            if (_value != null && !_value.Test(typed.Value, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CaseSwitchLabelSyntax)node;

            if (_value != null)
                _value.RunCallback(typed.Value, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2857
    public partial class DefaultSwitchLabelPattern : SwitchLabelPattern
    {
        private readonly Action<DefaultSwitchLabelSyntax> _action;

        internal DefaultSwitchLabelPattern(Action<DefaultSwitchLabelSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultSwitchLabelSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultSwitchLabelSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #2874
    public partial class SwitchExpressionPattern : ExpressionPattern
    {
        private readonly ExpressionPattern _governingExpression;
        private readonly NodeListPattern<SwitchExpressionArmPattern> _arms;
        private readonly Action<SwitchExpressionSyntax> _action;

        internal SwitchExpressionPattern(ExpressionPattern governingExpression, NodeListPattern<SwitchExpressionArmPattern> arms, Action<SwitchExpressionSyntax> action)
        {
            _governingExpression = governingExpression;
            _arms = arms;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchExpressionSyntax typed))
                return false;

            if (_governingExpression != null && !_governingExpression.Test(typed.GoverningExpression, semanticModel))
                return false;
            if (_arms != null && !_arms.Test(typed.Arms, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchExpressionSyntax)node;

            if (_governingExpression != null)
                _governingExpression.RunCallback(typed.GoverningExpression, semanticModel);
            if (_arms != null)
                _arms.RunCallback(typed.Arms, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2888
    public partial class SwitchExpressionArmPattern : PatternNode
    {
        private readonly PatternPattern _pattern;
        private readonly WhenClausePattern _whenClause;
        private readonly ExpressionPattern _expression;
        private readonly Action<SwitchExpressionArmSyntax> _action;

        internal SwitchExpressionArmPattern(PatternPattern pattern, WhenClausePattern whenClause, ExpressionPattern expression, Action<SwitchExpressionArmSyntax> action)
        {
            _pattern = pattern;
            _whenClause = whenClause;
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SwitchExpressionArmSyntax typed))
                return false;

            if (_pattern != null && !_pattern.Test(typed.Pattern, semanticModel))
                return false;
            if (_whenClause != null && !_whenClause.Test(typed.WhenClause, semanticModel))
                return false;
            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (SwitchExpressionArmSyntax)node;

            if (_pattern != null)
                _pattern.RunCallback(typed.Pattern, semanticModel);
            if (_whenClause != null)
                _whenClause.RunCallback(typed.WhenClause, semanticModel);
            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2898
    public partial class TryStatementPattern : StatementPattern
    {
        private readonly BlockPattern _block;
        private readonly NodeListPattern<CatchClausePattern> _catches;
        private readonly FinallyClausePattern _finally;
        private readonly Action<TryStatementSyntax> _action;

        internal TryStatementPattern(NodeListPattern<AttributeListPattern> attributeLists, BlockPattern block, NodeListPattern<CatchClausePattern> catches, FinallyClausePattern @finally, Action<TryStatementSyntax> action)
            : base(attributeLists)
        {
            _block = block;
            _catches = catches;
            _finally = @finally;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TryStatementSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;
            if (_catches != null && !_catches.Test(typed.Catches, semanticModel))
                return false;
            if (_finally != null && !_finally.Test(typed.Finally, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (TryStatementSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);
            if (_catches != null)
                _catches.RunCallback(typed.Catches, semanticModel);
            if (_finally != null)
                _finally.RunCallback(typed.Finally, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2908
    public partial class CatchClausePattern : PatternNode
    {
        private readonly CatchDeclarationPattern _declaration;
        private readonly CatchFilterClausePattern _filter;
        private readonly BlockPattern _block;
        private readonly Action<CatchClauseSyntax> _action;

        internal CatchClausePattern(CatchDeclarationPattern declaration, CatchFilterClausePattern filter, BlockPattern block, Action<CatchClauseSyntax> action)
        {
            _declaration = declaration;
            _filter = filter;
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchClauseSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;
            if (_filter != null && !_filter.Test(typed.Filter, semanticModel))
                return false;
            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchClauseSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
            if (_filter != null)
                _filter.RunCallback(typed.Filter, semanticModel);
            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2917
    public partial class CatchDeclarationPattern : PatternNode
    {
        private readonly TypePattern _type;
        private readonly string _identifier;
        private readonly Action<CatchDeclarationSyntax> _action;

        internal CatchDeclarationPattern(TypePattern type, string identifier, Action<CatchDeclarationSyntax> action)
        {
            _type = type;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2930
    public partial class CatchFilterClausePattern : PatternNode
    {
        private readonly ExpressionPattern _filterExpression;
        private readonly Action<CatchFilterClauseSyntax> _action;

        internal CatchFilterClausePattern(ExpressionPattern filterExpression, Action<CatchFilterClauseSyntax> action)
        {
            _filterExpression = filterExpression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CatchFilterClauseSyntax typed))
                return false;

            if (_filterExpression != null && !_filterExpression.Test(typed.FilterExpression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CatchFilterClauseSyntax)node;

            if (_filterExpression != null)
                _filterExpression.RunCallback(typed.FilterExpression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2943
    public partial class FinallyClausePattern : PatternNode
    {
        private readonly BlockPattern _block;
        private readonly Action<FinallyClauseSyntax> _action;

        internal FinallyClausePattern(BlockPattern block, Action<FinallyClauseSyntax> action)
        {
            _block = block;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FinallyClauseSyntax typed))
                return false;

            if (_block != null && !_block.Test(typed.Block, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (FinallyClauseSyntax)node;

            if (_block != null)
                _block.RunCallback(typed.Block, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2952
    public partial class CompilationUnitPattern : PatternNode
    {
        private readonly NodeListPattern<ExternAliasDirectivePattern> _externs;
        private readonly NodeListPattern<UsingDirectivePattern> _usings;
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;
        private readonly Action<CompilationUnitSyntax> _action;

        internal CompilationUnitPattern(NodeListPattern<ExternAliasDirectivePattern> externs, NodeListPattern<UsingDirectivePattern> usings, NodeListPattern<AttributeListPattern> attributeLists, NodeListPattern<MemberDeclarationPattern> members, Action<CompilationUnitSyntax> action)
        {
            _externs = externs;
            _usings = usings;
            _attributeLists = attributeLists;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is CompilationUnitSyntax typed))
                return false;

            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (CompilationUnitSyntax)node;

            if (_externs != null)
                _externs.RunCallback(typed.Externs, semanticModel);
            if (_usings != null)
                _usings.RunCallback(typed.Usings, semanticModel);
            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #2966
    public partial class ExternAliasDirectivePattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<ExternAliasDirectiveSyntax> _action;

        internal ExternAliasDirectivePattern(string identifier, Action<ExternAliasDirectiveSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExternAliasDirectiveSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExternAliasDirectiveSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3001
    public partial class UsingDirectivePattern : PatternNode
    {
        private readonly NamePattern _name;
        private readonly Action<UsingDirectiveSyntax> _action;

        internal UsingDirectivePattern(NamePattern name, Action<UsingDirectiveSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is UsingDirectiveSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (UsingDirectiveSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3015
    public abstract partial class MemberDeclarationPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;

        internal MemberDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MemberDeclarationSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (MemberDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
        }
    }

    // #3030
    public partial class NamespaceDeclarationPattern : MemberDeclarationPattern
    {
        private readonly NamePattern _name;
        private readonly NodeListPattern<ExternAliasDirectivePattern> _externs;
        private readonly NodeListPattern<UsingDirectivePattern> _usings;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;
        private readonly Action<NamespaceDeclarationSyntax> _action;

        internal NamespaceDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, NamePattern name, NodeListPattern<ExternAliasDirectivePattern> externs, NodeListPattern<UsingDirectivePattern> usings, NodeListPattern<MemberDeclarationPattern> members, Action<NamespaceDeclarationSyntax> action)
            : base(attributeLists, modifiers)
        {
            _name = name;
            _externs = externs;
            _usings = usings;
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NamespaceDeclarationSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_externs != null && !_externs.Test(typed.Externs, semanticModel))
                return false;
            if (_usings != null && !_usings.Test(typed.Usings, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (NamespaceDeclarationSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_externs != null)
                _externs.RunCallback(typed.Externs, semanticModel);
            if (_usings != null)
                _usings.RunCallback(typed.Usings, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3054
    public partial class AttributeListPattern : PatternNode
    {
        private readonly AttributeTargetSpecifierPattern _target;
        private readonly NodeListPattern<AttributePattern> _attributes;
        private readonly Action<AttributeListSyntax> _action;

        internal AttributeListPattern(AttributeTargetSpecifierPattern target, NodeListPattern<AttributePattern> attributes, Action<AttributeListSyntax> action)
        {
            _target = target;
            _attributes = attributes;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeListSyntax typed))
                return false;

            if (_target != null && !_target.Test(typed.Target, semanticModel))
                return false;
            if (_attributes != null && !_attributes.Test(typed.Attributes, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeListSyntax)node;

            if (_target != null)
                _target.RunCallback(typed.Target, semanticModel);
            if (_attributes != null)
                _attributes.RunCallback(typed.Attributes, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3082
    public partial class AttributeTargetSpecifierPattern : PatternNode
    {
        private readonly string _identifier;
        private readonly Action<AttributeTargetSpecifierSyntax> _action;

        internal AttributeTargetSpecifierPattern(string identifier, Action<AttributeTargetSpecifierSyntax> action)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeTargetSpecifierSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeTargetSpecifierSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3099
    public partial class AttributePattern : PatternNode
    {
        private readonly NamePattern _name;
        private readonly AttributeArgumentListPattern _argumentList;
        private readonly Action<AttributeSyntax> _action;

        internal AttributePattern(NamePattern name, AttributeArgumentListPattern argumentList, Action<AttributeSyntax> action)
        {
            _name = name;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3111
    public partial class AttributeArgumentListPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeArgumentPattern> _arguments;
        private readonly Action<AttributeArgumentListSyntax> _action;

        internal AttributeArgumentListPattern(NodeListPattern<AttributeArgumentPattern> arguments, Action<AttributeArgumentListSyntax> action)
        {
            _arguments = arguments;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentListSyntax typed))
                return false;

            if (_arguments != null && !_arguments.Test(typed.Arguments, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentListSyntax)node;

            if (_arguments != null)
                _arguments.RunCallback(typed.Arguments, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3134
    public partial class AttributeArgumentPattern : PatternNode
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<AttributeArgumentSyntax> _action;

        internal AttributeArgumentPattern(ExpressionPattern expression, Action<AttributeArgumentSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AttributeArgumentSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AttributeArgumentSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3149
    public partial class NameEqualsPattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly Action<NameEqualsSyntax> _action;

        internal NameEqualsPattern(IdentifierNamePattern name, Action<NameEqualsSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is NameEqualsSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (NameEqualsSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3164
    public partial class TypeParameterListPattern : PatternNode
    {
        private readonly NodeListPattern<TypeParameterPattern> _parameters;
        private readonly Action<TypeParameterListSyntax> _action;

        internal TypeParameterListPattern(NodeListPattern<TypeParameterPattern> parameters, Action<TypeParameterListSyntax> action)
        {
            _parameters = parameters;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterListSyntax)node;

            if (_parameters != null)
                _parameters.RunCallback(typed.Parameters, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3187
    public partial class TypeParameterPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly string _identifier;
        private readonly Action<TypeParameterSyntax> _action;

        internal TypeParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, string identifier, Action<TypeParameterSyntax> action)
        {
            _attributeLists = attributeLists;
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3208
    public abstract partial class BaseTypeDeclarationPattern : MemberDeclarationPattern
    {
        private readonly string _identifier;
        private readonly BaseListPattern _baseList;

        internal BaseTypeDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList)
            : base(attributeLists, modifiers)
        {
            _identifier = identifier;
            _baseList = baseList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_baseList != null && !_baseList.Test(typed.BaseList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BaseTypeDeclarationSyntax)node;

            if (_baseList != null)
                _baseList.RunCallback(typed.BaseList, semanticModel);
        }
    }

    // #3242
    public abstract partial class TypeDeclarationPattern : BaseTypeDeclarationPattern
    {
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly NodeListPattern<MemberDeclarationPattern> _members;

        internal TypeDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _members = members;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeDeclarationSyntax typed))
                return false;

            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;
            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (TypeDeclarationSyntax)node;

            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);
            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);
        }
    }

    // #3263
    public partial class ClassDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<ClassDeclarationSyntax> _action;

        internal ClassDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<ClassDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ClassDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3293
    public partial class StructDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<StructDeclarationSyntax> _action;

        internal StructDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<StructDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is StructDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (StructDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3323
    public partial class InterfaceDeclarationPattern : TypeDeclarationPattern
    {
        private readonly Action<InterfaceDeclarationSyntax> _action;

        internal InterfaceDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, Action<InterfaceDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is InterfaceDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (InterfaceDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3353
    public partial class RecordDeclarationPattern : TypeDeclarationPattern
    {
        private readonly ParameterListPattern _parameterList;
        private readonly Action<RecordDeclarationSyntax> _action;

        internal RecordDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, NodeListPattern<MemberDeclarationPattern> members, ParameterListPattern parameterList, Action<RecordDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList, typeParameterList, constraintClauses, members)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is RecordDeclarationSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (RecordDeclarationSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3378
    public partial class EnumDeclarationPattern : BaseTypeDeclarationPattern
    {
        private readonly NodeListPattern<EnumMemberDeclarationPattern> _members;
        private readonly Action<EnumDeclarationSyntax> _action;

        internal EnumDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, BaseListPattern baseList, NodeListPattern<EnumMemberDeclarationPattern> members, Action<EnumDeclarationSyntax> action)
            : base(attributeLists, modifiers, identifier, baseList)
        {
            _members = members;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumDeclarationSyntax typed))
                return false;

            if (_members != null && !_members.Test(typed.Members, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EnumDeclarationSyntax)node;

            if (_members != null)
                _members.RunCallback(typed.Members, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3414
    public partial class DelegateDeclarationPattern : MemberDeclarationPattern
    {
        private readonly TypePattern _returnType;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly ParameterListPattern _parameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly Action<DelegateDeclarationSyntax> _action;

        internal DelegateDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern returnType, string identifier, TypeParameterListPattern typeParameterList, ParameterListPattern parameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, Action<DelegateDeclarationSyntax> action)
            : base(attributeLists, modifiers)
        {
            _returnType = returnType;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _parameterList = parameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DelegateDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (DelegateDeclarationSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3456
    public partial class EnumMemberDeclarationPattern : MemberDeclarationPattern
    {
        private readonly string _identifier;
        private readonly EqualsValueClausePattern _equalsValue;
        private readonly Action<EnumMemberDeclarationSyntax> _action;

        internal EnumMemberDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, string identifier, EqualsValueClausePattern equalsValue, Action<EnumMemberDeclarationSyntax> action)
            : base(attributeLists, modifiers)
        {
            _identifier = identifier;
            _equalsValue = equalsValue;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EnumMemberDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_equalsValue != null && !_equalsValue.Test(typed.EqualsValue, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EnumMemberDeclarationSyntax)node;

            if (_equalsValue != null)
                _equalsValue.RunCallback(typed.EqualsValue, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3468
    public partial class BaseListPattern : PatternNode
    {
        private readonly NodeListPattern<BaseTypePattern> _types;
        private readonly Action<BaseListSyntax> _action;

        internal BaseListPattern(NodeListPattern<BaseTypePattern> types, Action<BaseListSyntax> action)
        {
            _types = types;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseListSyntax typed))
                return false;

            if (_types != null && !_types.Test(typed.Types, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseListSyntax)node;

            if (_types != null)
                _types.RunCallback(typed.Types, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3486
    public abstract partial class BaseTypePattern : PatternNode
    {
        private readonly TypePattern _type;

        internal BaseTypePattern(TypePattern type)
        {
            _type = type;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseTypeSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseTypeSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
        }
    }

    // #3494
    public partial class SimpleBaseTypePattern : BaseTypePattern
    {
        private readonly Action<SimpleBaseTypeSyntax> _action;

        internal SimpleBaseTypePattern(TypePattern type, Action<SimpleBaseTypeSyntax> action)
            : base(type)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is SimpleBaseTypeSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (SimpleBaseTypeSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3500
    public partial class PrimaryConstructorBaseTypePattern : BaseTypePattern
    {
        private readonly ArgumentListPattern _argumentList;
        private readonly Action<PrimaryConstructorBaseTypeSyntax> _action;

        internal PrimaryConstructorBaseTypePattern(TypePattern type, ArgumentListPattern argumentList, Action<PrimaryConstructorBaseTypeSyntax> action)
            : base(type)
        {
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PrimaryConstructorBaseTypeSyntax typed))
                return false;

            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (PrimaryConstructorBaseTypeSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3506
    public partial class TypeParameterConstraintClausePattern : PatternNode
    {
        private readonly IdentifierNamePattern _name;
        private readonly NodeListPattern<TypeParameterConstraintPattern> _constraints;
        private readonly Action<TypeParameterConstraintClauseSyntax> _action;

        internal TypeParameterConstraintClausePattern(IdentifierNamePattern name, NodeListPattern<TypeParameterConstraintPattern> constraints, Action<TypeParameterConstraintClauseSyntax> action)
        {
            _name = name;
            _constraints = constraints;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintClauseSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;
            if (_constraints != null && !_constraints.Test(typed.Constraints, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeParameterConstraintClauseSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);
            if (_constraints != null)
                _constraints.RunCallback(typed.Constraints, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3532
    public abstract partial class TypeParameterConstraintPattern : PatternNode
    {

        internal TypeParameterConstraintPattern()
        {
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeParameterConstraintSyntax typed))
                return false;


            return true;
        }
    }

    // #3537
    public partial class ConstructorConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly Action<ConstructorConstraintSyntax> _action;

        internal ConstructorConstraintPattern(Action<ConstructorConstraintSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorConstraintSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorConstraintSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3561
    public partial class ClassOrStructConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly SyntaxKind _kind;
        private readonly Action<ClassOrStructConstraintSyntax> _action;

        internal ClassOrStructConstraintPattern(SyntaxKind kind, Action<ClassOrStructConstraintSyntax> action)
        {
            _kind = kind;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ClassOrStructConstraintSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ClassOrStructConstraintSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3581
    public partial class TypeConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly TypePattern _type;
        private readonly Action<TypeConstraintSyntax> _action;

        internal TypeConstraintPattern(TypePattern type, Action<TypeConstraintSyntax> action)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is TypeConstraintSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (TypeConstraintSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3592
    public partial class DefaultConstraintPattern : TypeParameterConstraintPattern
    {
        private readonly Action<DefaultConstraintSyntax> _action;

        internal DefaultConstraintPattern(Action<DefaultConstraintSyntax> action)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DefaultConstraintSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (DefaultConstraintSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3604
    public abstract partial class BaseFieldDeclarationPattern : MemberDeclarationPattern
    {
        private readonly VariableDeclarationPattern _declaration;

        internal BaseFieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration)
            : base(attributeLists, modifiers)
        {
            _declaration = declaration;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseFieldDeclarationSyntax typed))
                return false;

            if (_declaration != null && !_declaration.Test(typed.Declaration, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BaseFieldDeclarationSyntax)node;

            if (_declaration != null)
                _declaration.RunCallback(typed.Declaration, semanticModel);
        }
    }

    // #3610
    public partial class FieldDeclarationPattern : BaseFieldDeclarationPattern
    {
        private readonly Action<FieldDeclarationSyntax> _action;

        internal FieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<FieldDeclarationSyntax> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (FieldDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3619
    public partial class EventFieldDeclarationPattern : BaseFieldDeclarationPattern
    {
        private readonly Action<EventFieldDeclarationSyntax> _action;

        internal EventFieldDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, VariableDeclarationPattern declaration, Action<EventFieldDeclarationSyntax> action)
            : base(attributeLists, modifiers, declaration)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventFieldDeclarationSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EventFieldDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3631
    public partial class ExplicitInterfaceSpecifierPattern : PatternNode
    {
        private readonly NamePattern _name;
        private readonly Action<ExplicitInterfaceSpecifierSyntax> _action;

        internal ExplicitInterfaceSpecifierPattern(NamePattern name, Action<ExplicitInterfaceSpecifierSyntax> action)
        {
            _name = name;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ExplicitInterfaceSpecifierSyntax typed))
                return false;

            if (_name != null && !_name.Test(typed.Name, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ExplicitInterfaceSpecifierSyntax)node;

            if (_name != null)
                _name.RunCallback(typed.Name, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3638
    public abstract partial class BaseMethodDeclarationPattern : MemberDeclarationPattern
    {
        private readonly ParameterListPattern _parameterList;

        internal BaseMethodDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList)
            : base(attributeLists, modifiers)
        {
            _parameterList = parameterList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseMethodDeclarationSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BaseMethodDeclarationSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);
        }
    }

    // #3660
    public partial class MethodDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _returnType;
        private readonly ExplicitInterfaceSpecifierPattern _explicitInterfaceSpecifier;
        private readonly string _identifier;
        private readonly TypeParameterListPattern _typeParameterList;
        private readonly NodeListPattern<TypeParameterConstraintClausePattern> _constraintClauses;
        private readonly Action<MethodDeclarationSyntax> _action;

        internal MethodDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, TypePattern returnType, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, string identifier, TypeParameterListPattern typeParameterList, NodeListPattern<TypeParameterConstraintClausePattern> constraintClauses, Action<MethodDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList)
        {
            _returnType = returnType;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _identifier = identifier;
            _typeParameterList = typeParameterList;
            _constraintClauses = constraintClauses;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is MethodDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_typeParameterList != null && !_typeParameterList.Test(typed.TypeParameterList, semanticModel))
                return false;
            if (_constraintClauses != null && !_constraintClauses.Test(typed.ConstraintClauses, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (MethodDeclarationSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                _explicitInterfaceSpecifier.RunCallback(typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_typeParameterList != null)
                _typeParameterList.RunCallback(typed.TypeParameterList, semanticModel);
            if (_constraintClauses != null)
                _constraintClauses.RunCallback(typed.ConstraintClauses, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3699
    public partial class OperatorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _returnType;
        private readonly Action<OperatorDeclarationSyntax> _action;

        internal OperatorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, TypePattern returnType, Action<OperatorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList)
        {
            _returnType = returnType;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is OperatorDeclarationSyntax typed))
                return false;

            if (_returnType != null && !_returnType.Test(typed.ReturnType, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (OperatorDeclarationSyntax)node;

            if (_returnType != null)
                _returnType.RunCallback(typed.ReturnType, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3760
    public partial class ConversionOperatorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly TypePattern _type;
        private readonly Action<ConversionOperatorDeclarationSyntax> _action;

        internal ConversionOperatorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, TypePattern type, Action<ConversionOperatorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConversionOperatorDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ConversionOperatorDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3800
    public partial class ConstructorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly string _identifier;
        private readonly ConstructorInitializerPattern _initializer;
        private readonly Action<ConstructorDeclarationSyntax> _action;

        internal ConstructorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, string identifier, ConstructorInitializerPattern initializer, Action<ConstructorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList)
        {
            _identifier = identifier;
            _initializer = initializer;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_initializer != null && !_initializer.Test(typed.Initializer, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ConstructorDeclarationSyntax)node;

            if (_initializer != null)
                _initializer.RunCallback(typed.Initializer, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3828
    public partial class ConstructorInitializerPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly ArgumentListPattern _argumentList;
        private readonly Action<ConstructorInitializerSyntax> _action;

        internal ConstructorInitializerPattern(SyntaxKind kind, ArgumentListPattern argumentList, Action<ConstructorInitializerSyntax> action)
        {
            _kind = kind;
            _argumentList = argumentList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ConstructorInitializerSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_argumentList != null && !_argumentList.Test(typed.ArgumentList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ConstructorInitializerSyntax)node;

            if (_argumentList != null)
                _argumentList.RunCallback(typed.ArgumentList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3849
    public partial class DestructorDeclarationPattern : BaseMethodDeclarationPattern
    {
        private readonly string _identifier;
        private readonly Action<DestructorDeclarationSyntax> _action;

        internal DestructorDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, ParameterListPattern parameterList, string identifier, Action<DestructorDeclarationSyntax> action)
            : base(attributeLists, modifiers, parameterList)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is DestructorDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (DestructorDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3882
    public abstract partial class BasePropertyDeclarationPattern : MemberDeclarationPattern
    {
        private readonly TypePattern _type;
        private readonly ExplicitInterfaceSpecifierPattern _explicitInterfaceSpecifier;
        private readonly AccessorListPattern _accessorList;

        internal BasePropertyDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList)
            : base(attributeLists, modifiers)
        {
            _type = type;
            _explicitInterfaceSpecifier = explicitInterfaceSpecifier;
            _accessorList = accessorList;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BasePropertyDeclarationSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;
            if (_explicitInterfaceSpecifier != null && !_explicitInterfaceSpecifier.Test(typed.ExplicitInterfaceSpecifier, semanticModel))
                return false;
            if (_accessorList != null && !_accessorList.Test(typed.AccessorList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BasePropertyDeclarationSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
            if (_explicitInterfaceSpecifier != null)
                _explicitInterfaceSpecifier.RunCallback(typed.ExplicitInterfaceSpecifier, semanticModel);
            if (_accessorList != null)
                _accessorList.RunCallback(typed.AccessorList, semanticModel);
        }
    }

    // #3898
    public partial class PropertyDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly string _identifier;
        private readonly Action<PropertyDeclarationSyntax> _action;

        internal PropertyDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, string identifier, Action<PropertyDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is PropertyDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (PropertyDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3923
    public partial class ArrowExpressionClausePattern : PatternNode
    {
        private readonly ExpressionPattern _expression;
        private readonly Action<ArrowExpressionClauseSyntax> _action;

        internal ArrowExpressionClausePattern(ExpressionPattern expression, Action<ArrowExpressionClauseSyntax> action)
        {
            _expression = expression;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ArrowExpressionClauseSyntax typed))
                return false;

            if (_expression != null && !_expression.Test(typed.Expression, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (ArrowExpressionClauseSyntax)node;

            if (_expression != null)
                _expression.RunCallback(typed.Expression, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3933
    public partial class EventDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly string _identifier;
        private readonly Action<EventDeclarationSyntax> _action;

        internal EventDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, string identifier, Action<EventDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _identifier = identifier;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is EventDeclarationSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (EventDeclarationSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #3955
    public partial class IndexerDeclarationPattern : BasePropertyDeclarationPattern
    {
        private readonly BracketedParameterListPattern _parameterList;
        private readonly Action<IndexerDeclarationSyntax> _action;

        internal IndexerDeclarationPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier, AccessorListPattern accessorList, BracketedParameterListPattern parameterList, Action<IndexerDeclarationSyntax> action)
            : base(attributeLists, modifiers, type, explicitInterfaceSpecifier, accessorList)
        {
            _parameterList = parameterList;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IndexerDeclarationSyntax typed))
                return false;

            if (_parameterList != null && !_parameterList.Test(typed.ParameterList, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (IndexerDeclarationSyntax)node;

            if (_parameterList != null)
                _parameterList.RunCallback(typed.ParameterList, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3979
    public partial class AccessorListPattern : PatternNode
    {
        private readonly NodeListPattern<AccessorDeclarationPattern> _accessors;
        private readonly Action<AccessorListSyntax> _action;

        internal AccessorListPattern(NodeListPattern<AccessorDeclarationPattern> accessors, Action<AccessorListSyntax> action)
        {
            _accessors = accessors;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorListSyntax typed))
                return false;

            if (_accessors != null && !_accessors.Test(typed.Accessors, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorListSyntax)node;

            if (_accessors != null)
                _accessors.RunCallback(typed.Accessors, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #3989
    public partial class AccessorDeclarationPattern : PatternNode
    {
        private readonly SyntaxKind _kind;
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly Action<AccessorDeclarationSyntax> _action;

        internal AccessorDeclarationPattern(SyntaxKind kind, NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, Action<AccessorDeclarationSyntax> action)
        {
            _kind = kind;
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is AccessorDeclarationSyntax typed))
                return false;

            if (_kind != SyntaxKind.None && !typed.IsKind(_kind))
                return false;
            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (AccessorDeclarationSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #4038
    public abstract partial class BaseParameterListPattern : PatternNode
    {
        private readonly NodeListPattern<ParameterPattern> _parameters;

        internal BaseParameterListPattern(NodeListPattern<ParameterPattern> parameters)
        {
            _parameters = parameters;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseParameterListSyntax typed))
                return false;

            if (_parameters != null && !_parameters.Test(typed.Parameters, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseParameterListSyntax)node;

            if (_parameters != null)
                _parameters.RunCallback(typed.Parameters, semanticModel);
        }
    }

    // #4048
    public partial class ParameterListPattern : BaseParameterListPattern
    {
        private readonly Action<ParameterListSyntax> _action;

        internal ParameterListPattern(NodeListPattern<ParameterPattern> parameters, Action<ParameterListSyntax> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ParameterListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #4067
    public partial class BracketedParameterListPattern : BaseParameterListPattern
    {
        private readonly Action<BracketedParameterListSyntax> _action;

        internal BracketedParameterListPattern(NodeListPattern<ParameterPattern> parameters, Action<BracketedParameterListSyntax> action)
            : base(parameters)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BracketedParameterListSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (BracketedParameterListSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #4086
    public abstract partial class BaseParameterPattern : PatternNode
    {
        private readonly NodeListPattern<AttributeListPattern> _attributeLists;
        private readonly TokenListPattern _modifiers;
        private readonly TypePattern _type;

        internal BaseParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type)
        {
            _attributeLists = attributeLists;
            _modifiers = modifiers;
            _type = type;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is BaseParameterSyntax typed))
                return false;

            if (_attributeLists != null && !_attributeLists.Test(typed.AttributeLists, semanticModel))
                return false;
            if (_modifiers != null && !_modifiers.Test(typed.Modifiers, semanticModel))
                return false;
            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            var typed = (BaseParameterSyntax)node;

            if (_attributeLists != null)
                _attributeLists.RunCallback(typed.AttributeLists, semanticModel);
            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);
        }
    }

    // #4102
    public partial class ParameterPattern : BaseParameterPattern
    {
        private readonly string _identifier;
        private readonly EqualsValueClausePattern _default;
        private readonly Action<ParameterSyntax> _action;

        internal ParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, string identifier, EqualsValueClausePattern @default, Action<ParameterSyntax> action)
            : base(attributeLists, modifiers, type)
        {
            _identifier = identifier;
            _default = @default;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is ParameterSyntax typed))
                return false;

            if (_identifier != null && _identifier != typed.Identifier.Text)
                return false;
            if (_default != null && !_default.Test(typed.Default, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (ParameterSyntax)node;

            if (_default != null)
                _default.RunCallback(typed.Default, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    // #4127
    public partial class FunctionPointerParameterPattern : BaseParameterPattern
    {
        private readonly Action<FunctionPointerParameterSyntax> _action;

        internal FunctionPointerParameterPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, Action<FunctionPointerParameterSyntax> action)
            : base(attributeLists, modifiers, type)
        {
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is FunctionPointerParameterSyntax typed))
                return false;


            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (FunctionPointerParameterSyntax)node;


            if (_action != null)
                _action(typed);
        }
    }

    // #4144
    public partial class IncompleteMemberPattern : MemberDeclarationPattern
    {
        private readonly TypePattern _type;
        private readonly Action<IncompleteMemberSyntax> _action;

        internal IncompleteMemberPattern(NodeListPattern<AttributeListPattern> attributeLists, TokenListPattern modifiers, TypePattern type, Action<IncompleteMemberSyntax> action)
            : base(attributeLists, modifiers)
        {
            _type = type;
            _action = action;
        }

        internal override bool Test(SyntaxNode node, SemanticModel semanticModel)
        {
            if (!base.Test(node, semanticModel))
                return false;
            if (!(node is IncompleteMemberSyntax typed))
                return false;

            if (_type != null && !_type.Test(typed.Type, semanticModel))
                return false;

            return true;
        }

        internal override void RunCallback(SyntaxNode node, SemanticModel semanticModel)
        {
            base.RunCallback(node, semanticModel);

            var typed = (IncompleteMemberSyntax)node;

            if (_type != null)
                _type.RunCallback(typed.Type, semanticModel);

            if (_action != null)
                _action(typed);
        }
    }

    partial class Pattern
    {
        public static IdentifierNamePattern IdentifierName(string identifier = null, Action<IdentifierNameSyntax> action = null)
        {
            return new IdentifierNamePattern(identifier, action);
        }
        public static QualifiedNamePattern QualifiedName(NamePattern left = null, SimpleNamePattern right = null, Action<QualifiedNameSyntax> action = null)
        {
            return new QualifiedNamePattern(left, right, action);
        }
        public static GenericNamePattern GenericName(string identifier = null, TypeArgumentListPattern typeArgumentList = null, Action<GenericNameSyntax> action = null)
        {
            return new GenericNamePattern(identifier, typeArgumentList, action);
        }
        public static TypeArgumentListPattern TypeArgumentList(IEnumerable<TypePattern> arguments = null, Action<TypeArgumentListSyntax> action = null)
        {
            return new TypeArgumentListPattern(NodeList(arguments), action);
        }

        public static TypeArgumentListPattern TypeArgumentList(params TypePattern[] arguments)
        {
            return new TypeArgumentListPattern(NodeList(arguments), null);
        }
        public static AliasQualifiedNamePattern AliasQualifiedName(IdentifierNamePattern alias = null, SimpleNamePattern name = null, Action<AliasQualifiedNameSyntax> action = null)
        {
            return new AliasQualifiedNamePattern(alias, name, action);
        }
        public static PredefinedTypePattern PredefinedType(string keyword = null, Action<PredefinedTypeSyntax> action = null)
        {
            return new PredefinedTypePattern(keyword, action);
        }
        public static ArrayTypePattern ArrayType(TypePattern elementType = null, IEnumerable<ArrayRankSpecifierPattern> rankSpecifiers = null, Action<ArrayTypeSyntax> action = null)
        {
            return new ArrayTypePattern(elementType, NodeList(rankSpecifiers), action);
        }
        public static ArrayRankSpecifierPattern ArrayRankSpecifier(IEnumerable<ExpressionPattern> sizes = null, Action<ArrayRankSpecifierSyntax> action = null)
        {
            return new ArrayRankSpecifierPattern(NodeList(sizes), action);
        }

        public static ArrayRankSpecifierPattern ArrayRankSpecifier(params ExpressionPattern[] sizes)
        {
            return new ArrayRankSpecifierPattern(NodeList(sizes), null);
        }
        public static PointerTypePattern PointerType(TypePattern elementType = null, Action<PointerTypeSyntax> action = null)
        {
            return new PointerTypePattern(elementType, action);
        }
        public static FunctionPointerTypePattern FunctionPointerType(FunctionPointerCallingConventionPattern callingConvention = null, FunctionPointerParameterListPattern parameterList = null, Action<FunctionPointerTypeSyntax> action = null)
        {
            return new FunctionPointerTypePattern(callingConvention, parameterList, action);
        }
        public static FunctionPointerParameterListPattern FunctionPointerParameterList(IEnumerable<FunctionPointerParameterPattern> parameters = null, Action<FunctionPointerParameterListSyntax> action = null)
        {
            return new FunctionPointerParameterListPattern(NodeList(parameters), action);
        }

        public static FunctionPointerParameterListPattern FunctionPointerParameterList(params FunctionPointerParameterPattern[] parameters)
        {
            return new FunctionPointerParameterListPattern(NodeList(parameters), null);
        }
        public static FunctionPointerCallingConventionPattern FunctionPointerCallingConvention(FunctionPointerUnmanagedCallingConventionListPattern unmanagedCallingConventionList = null, Action<FunctionPointerCallingConventionSyntax> action = null)
        {
            return new FunctionPointerCallingConventionPattern(unmanagedCallingConventionList, action);
        }
        public static FunctionPointerUnmanagedCallingConventionListPattern FunctionPointerUnmanagedCallingConventionList(IEnumerable<FunctionPointerUnmanagedCallingConventionPattern> callingConventions = null, Action<FunctionPointerUnmanagedCallingConventionListSyntax> action = null)
        {
            return new FunctionPointerUnmanagedCallingConventionListPattern(NodeList(callingConventions), action);
        }

        public static FunctionPointerUnmanagedCallingConventionListPattern FunctionPointerUnmanagedCallingConventionList(params FunctionPointerUnmanagedCallingConventionPattern[] callingConventions)
        {
            return new FunctionPointerUnmanagedCallingConventionListPattern(NodeList(callingConventions), null);
        }
        public static FunctionPointerUnmanagedCallingConventionPattern FunctionPointerUnmanagedCallingConvention(string name = null, Action<FunctionPointerUnmanagedCallingConventionSyntax> action = null)
        {
            return new FunctionPointerUnmanagedCallingConventionPattern(name, action);
        }
        public static NullableTypePattern NullableType(TypePattern elementType = null, Action<NullableTypeSyntax> action = null)
        {
            return new NullableTypePattern(elementType, action);
        }
        public static TupleTypePattern TupleType(IEnumerable<TupleElementPattern> elements = null, Action<TupleTypeSyntax> action = null)
        {
            return new TupleTypePattern(NodeList(elements), action);
        }

        public static TupleTypePattern TupleType(params TupleElementPattern[] elements)
        {
            return new TupleTypePattern(NodeList(elements), null);
        }
        public static TupleElementPattern TupleElement(TypePattern type = null, string identifier = null, Action<TupleElementSyntax> action = null)
        {
            return new TupleElementPattern(type, identifier, action);
        }
        public static OmittedTypeArgumentPattern OmittedTypeArgument(Action<OmittedTypeArgumentSyntax> action = null)
        {
            return new OmittedTypeArgumentPattern(action);
        }
        public static RefTypePattern RefType(TypePattern type = null, Action<RefTypeSyntax> action = null)
        {
            return new RefTypePattern(type, action);
        }
        public static ParenthesizedExpressionPattern ParenthesizedExpression(ExpressionPattern expression = null, Action<ParenthesizedExpressionSyntax> action = null)
        {
            return new ParenthesizedExpressionPattern(expression, action);
        }
        public static TupleExpressionPattern TupleExpression(IEnumerable<ArgumentPattern> arguments = null, Action<TupleExpressionSyntax> action = null)
        {
            return new TupleExpressionPattern(NodeList(arguments), action);
        }

        public static TupleExpressionPattern TupleExpression(params ArgumentPattern[] arguments)
        {
            return new TupleExpressionPattern(NodeList(arguments), null);
        }
        public static PrefixUnaryExpressionPattern PrefixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern operand = null, Action<PrefixUnaryExpressionSyntax> action = null)
        {
            return new PrefixUnaryExpressionPattern(kind, operand, action);
        }
        public static AwaitExpressionPattern AwaitExpression(ExpressionPattern expression = null, Action<AwaitExpressionSyntax> action = null)
        {
            return new AwaitExpressionPattern(expression, action);
        }
        public static PostfixUnaryExpressionPattern PostfixUnaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern operand = null, Action<PostfixUnaryExpressionSyntax> action = null)
        {
            return new PostfixUnaryExpressionPattern(kind, operand, action);
        }
        public static MemberAccessExpressionPattern MemberAccessExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, SimpleNamePattern name = null, Action<MemberAccessExpressionSyntax> action = null)
        {
            return new MemberAccessExpressionPattern(kind, expression, name, action);
        }
        public static ConditionalAccessExpressionPattern ConditionalAccessExpression(ExpressionPattern expression = null, ExpressionPattern whenNotNull = null, Action<ConditionalAccessExpressionSyntax> action = null)
        {
            return new ConditionalAccessExpressionPattern(expression, whenNotNull, action);
        }
        public static MemberBindingExpressionPattern MemberBindingExpression(SimpleNamePattern name = null, Action<MemberBindingExpressionSyntax> action = null)
        {
            return new MemberBindingExpressionPattern(name, action);
        }
        public static ElementBindingExpressionPattern ElementBindingExpression(BracketedArgumentListPattern argumentList = null, Action<ElementBindingExpressionSyntax> action = null)
        {
            return new ElementBindingExpressionPattern(argumentList, action);
        }
        public static RangeExpressionPattern RangeExpression(ExpressionPattern leftOperand = null, ExpressionPattern rightOperand = null, Action<RangeExpressionSyntax> action = null)
        {
            return new RangeExpressionPattern(leftOperand, rightOperand, action);
        }
        public static ImplicitElementAccessPattern ImplicitElementAccess(BracketedArgumentListPattern argumentList = null, Action<ImplicitElementAccessSyntax> action = null)
        {
            return new ImplicitElementAccessPattern(argumentList, action);
        }
        public static BinaryExpressionPattern BinaryExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern left = null, ExpressionPattern right = null, Action<BinaryExpressionSyntax> action = null)
        {
            return new BinaryExpressionPattern(kind, left, right, action);
        }
        public static AssignmentExpressionPattern AssignmentExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern left = null, ExpressionPattern right = null, Action<AssignmentExpressionSyntax> action = null)
        {
            return new AssignmentExpressionPattern(kind, left, right, action);
        }
        public static ConditionalExpressionPattern ConditionalExpression(ExpressionPattern condition = null, ExpressionPattern whenTrue = null, ExpressionPattern whenFalse = null, Action<ConditionalExpressionSyntax> action = null)
        {
            return new ConditionalExpressionPattern(condition, whenTrue, whenFalse, action);
        }
        public static ThisExpressionPattern ThisExpression(Action<ThisExpressionSyntax> action = null)
        {
            return new ThisExpressionPattern(action);
        }
        public static BaseExpressionPattern BaseExpression(Action<BaseExpressionSyntax> action = null)
        {
            return new BaseExpressionPattern(action);
        }
        public static LiteralExpressionPattern LiteralExpression(SyntaxKind kind = default(SyntaxKind), Action<LiteralExpressionSyntax> action = null)
        {
            return new LiteralExpressionPattern(kind, action);
        }
        public static MakeRefExpressionPattern MakeRefExpression(ExpressionPattern expression = null, Action<MakeRefExpressionSyntax> action = null)
        {
            return new MakeRefExpressionPattern(expression, action);
        }
        public static RefTypeExpressionPattern RefTypeExpression(ExpressionPattern expression = null, Action<RefTypeExpressionSyntax> action = null)
        {
            return new RefTypeExpressionPattern(expression, action);
        }
        public static RefValueExpressionPattern RefValueExpression(ExpressionPattern expression = null, TypePattern type = null, Action<RefValueExpressionSyntax> action = null)
        {
            return new RefValueExpressionPattern(expression, type, action);
        }
        public static CheckedExpressionPattern CheckedExpression(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<CheckedExpressionSyntax> action = null)
        {
            return new CheckedExpressionPattern(kind, expression, action);
        }
        public static DefaultExpressionPattern DefaultExpression(TypePattern type = null, Action<DefaultExpressionSyntax> action = null)
        {
            return new DefaultExpressionPattern(type, action);
        }
        public static TypeOfExpressionPattern TypeOfExpression(TypePattern type = null, Action<TypeOfExpressionSyntax> action = null)
        {
            return new TypeOfExpressionPattern(type, action);
        }
        public static SizeOfExpressionPattern SizeOfExpression(TypePattern type = null, Action<SizeOfExpressionSyntax> action = null)
        {
            return new SizeOfExpressionPattern(type, action);
        }
        public static InvocationExpressionPattern InvocationExpression(ExpressionPattern expression = null, ArgumentListPattern argumentList = null, Action<InvocationExpressionSyntax> action = null)
        {
            return new InvocationExpressionPattern(expression, argumentList, action);
        }
        public static ElementAccessExpressionPattern ElementAccessExpression(ExpressionPattern expression = null, BracketedArgumentListPattern argumentList = null, Action<ElementAccessExpressionSyntax> action = null)
        {
            return new ElementAccessExpressionPattern(expression, argumentList, action);
        }
        public static ArgumentListPattern ArgumentList(IEnumerable<ArgumentPattern> arguments = null, Action<ArgumentListSyntax> action = null)
        {
            return new ArgumentListPattern(NodeList(arguments), action);
        }

        public static ArgumentListPattern ArgumentList(params ArgumentPattern[] arguments)
        {
            return new ArgumentListPattern(NodeList(arguments), null);
        }
        public static BracketedArgumentListPattern BracketedArgumentList(IEnumerable<ArgumentPattern> arguments = null, Action<BracketedArgumentListSyntax> action = null)
        {
            return new BracketedArgumentListPattern(NodeList(arguments), action);
        }

        public static BracketedArgumentListPattern BracketedArgumentList(params ArgumentPattern[] arguments)
        {
            return new BracketedArgumentListPattern(NodeList(arguments), null);
        }
        public static ArgumentPattern Argument(NameColonPattern nameColon = null, ExpressionPattern expression = null, Action<ArgumentSyntax> action = null)
        {
            return new ArgumentPattern(nameColon, expression, action);
        }
        public static NameColonPattern NameColon(IdentifierNamePattern name = null, Action<NameColonSyntax> action = null)
        {
            return new NameColonPattern(name, action);
        }
        public static DeclarationExpressionPattern DeclarationExpression(TypePattern type = null, VariableDesignationPattern designation = null, Action<DeclarationExpressionSyntax> action = null)
        {
            return new DeclarationExpressionPattern(type, designation, action);
        }
        public static CastExpressionPattern CastExpression(TypePattern type = null, ExpressionPattern expression = null, Action<CastExpressionSyntax> action = null)
        {
            return new CastExpressionPattern(type, expression, action);
        }
        public static AnonymousMethodExpressionPattern AnonymousMethodExpression(IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, Action<AnonymousMethodExpressionSyntax> action = null)
        {
            return new AnonymousMethodExpressionPattern(TokenList(modifiers), parameterList, action);
        }
        public static SimpleLambdaExpressionPattern SimpleLambdaExpression(IEnumerable<string> modifiers = null, ParameterPattern parameter = null, Action<SimpleLambdaExpressionSyntax> action = null)
        {
            return new SimpleLambdaExpressionPattern(TokenList(modifiers), parameter, action);
        }
        public static RefExpressionPattern RefExpression(ExpressionPattern expression = null, Action<RefExpressionSyntax> action = null)
        {
            return new RefExpressionPattern(expression, action);
        }
        public static ParenthesizedLambdaExpressionPattern ParenthesizedLambdaExpression(IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, Action<ParenthesizedLambdaExpressionSyntax> action = null)
        {
            return new ParenthesizedLambdaExpressionPattern(TokenList(modifiers), parameterList, action);
        }
        public static InitializerExpressionPattern InitializerExpression(SyntaxKind kind = default(SyntaxKind), IEnumerable<ExpressionPattern> expressions = null, Action<InitializerExpressionSyntax> action = null)
        {
            return new InitializerExpressionPattern(kind, NodeList(expressions), action);
        }

        public static InitializerExpressionPattern InitializerExpression(SyntaxKind kind, params ExpressionPattern[] expressions)
        {
            return new InitializerExpressionPattern(kind, NodeList(expressions), null);
        }
        public static ImplicitObjectCreationExpressionPattern ImplicitObjectCreationExpression(ArgumentListPattern argumentList = null, InitializerExpressionPattern initializer = null, Action<ImplicitObjectCreationExpressionSyntax> action = null)
        {
            return new ImplicitObjectCreationExpressionPattern(argumentList, initializer, action);
        }
        public static ObjectCreationExpressionPattern ObjectCreationExpression(ArgumentListPattern argumentList = null, InitializerExpressionPattern initializer = null, TypePattern type = null, Action<ObjectCreationExpressionSyntax> action = null)
        {
            return new ObjectCreationExpressionPattern(argumentList, initializer, type, action);
        }
        public static WithExpressionPattern WithExpression(ExpressionPattern expression = null, InitializerExpressionPattern initializer = null, Action<WithExpressionSyntax> action = null)
        {
            return new WithExpressionPattern(expression, initializer, action);
        }
        public static AnonymousObjectMemberDeclaratorPattern AnonymousObjectMemberDeclarator(NameEqualsPattern nameEquals = null, ExpressionPattern expression = null, Action<AnonymousObjectMemberDeclaratorSyntax> action = null)
        {
            return new AnonymousObjectMemberDeclaratorPattern(nameEquals, expression, action);
        }
        public static AnonymousObjectCreationExpressionPattern AnonymousObjectCreationExpression(IEnumerable<AnonymousObjectMemberDeclaratorPattern> initializers = null, Action<AnonymousObjectCreationExpressionSyntax> action = null)
        {
            return new AnonymousObjectCreationExpressionPattern(NodeList(initializers), action);
        }

        public static AnonymousObjectCreationExpressionPattern AnonymousObjectCreationExpression(params AnonymousObjectMemberDeclaratorPattern[] initializers)
        {
            return new AnonymousObjectCreationExpressionPattern(NodeList(initializers), null);
        }
        public static ArrayCreationExpressionPattern ArrayCreationExpression(ArrayTypePattern type = null, InitializerExpressionPattern initializer = null, Action<ArrayCreationExpressionSyntax> action = null)
        {
            return new ArrayCreationExpressionPattern(type, initializer, action);
        }
        public static ImplicitArrayCreationExpressionPattern ImplicitArrayCreationExpression(InitializerExpressionPattern initializer = null, Action<ImplicitArrayCreationExpressionSyntax> action = null)
        {
            return new ImplicitArrayCreationExpressionPattern(initializer, action);
        }
        public static StackAllocArrayCreationExpressionPattern StackAllocArrayCreationExpression(TypePattern type = null, InitializerExpressionPattern initializer = null, Action<StackAllocArrayCreationExpressionSyntax> action = null)
        {
            return new StackAllocArrayCreationExpressionPattern(type, initializer, action);
        }
        public static ImplicitStackAllocArrayCreationExpressionPattern ImplicitStackAllocArrayCreationExpression(InitializerExpressionPattern initializer = null, Action<ImplicitStackAllocArrayCreationExpressionSyntax> action = null)
        {
            return new ImplicitStackAllocArrayCreationExpressionPattern(initializer, action);
        }
        public static QueryExpressionPattern QueryExpression(FromClausePattern fromClause = null, QueryBodyPattern body = null, Action<QueryExpressionSyntax> action = null)
        {
            return new QueryExpressionPattern(fromClause, body, action);
        }
        public static QueryBodyPattern QueryBody(IEnumerable<QueryClausePattern> clauses = null, SelectOrGroupClausePattern selectOrGroup = null, QueryContinuationPattern continuation = null, Action<QueryBodySyntax> action = null)
        {
            return new QueryBodyPattern(NodeList(clauses), selectOrGroup, continuation, action);
        }
        public static FromClausePattern FromClause(TypePattern type = null, string identifier = null, ExpressionPattern expression = null, Action<FromClauseSyntax> action = null)
        {
            return new FromClausePattern(type, identifier, expression, action);
        }
        public static LetClausePattern LetClause(string identifier = null, ExpressionPattern expression = null, Action<LetClauseSyntax> action = null)
        {
            return new LetClausePattern(identifier, expression, action);
        }
        public static JoinClausePattern JoinClause(TypePattern type = null, string identifier = null, ExpressionPattern inExpression = null, ExpressionPattern leftExpression = null, ExpressionPattern rightExpression = null, JoinIntoClausePattern into = null, Action<JoinClauseSyntax> action = null)
        {
            return new JoinClausePattern(type, identifier, inExpression, leftExpression, rightExpression, into, action);
        }
        public static JoinIntoClausePattern JoinIntoClause(string identifier = null, Action<JoinIntoClauseSyntax> action = null)
        {
            return new JoinIntoClausePattern(identifier, action);
        }
        public static WhereClausePattern WhereClause(ExpressionPattern condition = null, Action<WhereClauseSyntax> action = null)
        {
            return new WhereClausePattern(condition, action);
        }
        public static OrderByClausePattern OrderByClause(IEnumerable<OrderingPattern> orderings = null, Action<OrderByClauseSyntax> action = null)
        {
            return new OrderByClausePattern(NodeList(orderings), action);
        }

        public static OrderByClausePattern OrderByClause(params OrderingPattern[] orderings)
        {
            return new OrderByClausePattern(NodeList(orderings), null);
        }
        public static OrderingPattern Ordering(SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<OrderingSyntax> action = null)
        {
            return new OrderingPattern(kind, expression, action);
        }
        public static SelectClausePattern SelectClause(ExpressionPattern expression = null, Action<SelectClauseSyntax> action = null)
        {
            return new SelectClausePattern(expression, action);
        }
        public static GroupClausePattern GroupClause(ExpressionPattern groupExpression = null, ExpressionPattern byExpression = null, Action<GroupClauseSyntax> action = null)
        {
            return new GroupClausePattern(groupExpression, byExpression, action);
        }
        public static QueryContinuationPattern QueryContinuation(string identifier = null, QueryBodyPattern body = null, Action<QueryContinuationSyntax> action = null)
        {
            return new QueryContinuationPattern(identifier, body, action);
        }
        public static OmittedArraySizeExpressionPattern OmittedArraySizeExpression(Action<OmittedArraySizeExpressionSyntax> action = null)
        {
            return new OmittedArraySizeExpressionPattern(action);
        }
        public static InterpolatedStringExpressionPattern InterpolatedStringExpression(IEnumerable<InterpolatedStringContentPattern> contents = null, Action<InterpolatedStringExpressionSyntax> action = null)
        {
            return new InterpolatedStringExpressionPattern(NodeList(contents), action);
        }

        public static InterpolatedStringExpressionPattern InterpolatedStringExpression(params InterpolatedStringContentPattern[] contents)
        {
            return new InterpolatedStringExpressionPattern(NodeList(contents), null);
        }
        public static IsPatternExpressionPattern IsPatternExpression(ExpressionPattern expression = null, PatternPattern pattern = null, Action<IsPatternExpressionSyntax> action = null)
        {
            return new IsPatternExpressionPattern(expression, pattern, action);
        }
        public static ThrowExpressionPattern ThrowExpression(ExpressionPattern expression = null, Action<ThrowExpressionSyntax> action = null)
        {
            return new ThrowExpressionPattern(expression, action);
        }
        public static WhenClausePattern WhenClause(ExpressionPattern condition = null, Action<WhenClauseSyntax> action = null)
        {
            return new WhenClausePattern(condition, action);
        }
        public static DiscardPatternPattern DiscardPattern(Action<DiscardPatternSyntax> action = null)
        {
            return new DiscardPatternPattern(action);
        }
        public static DeclarationPatternPattern DeclarationPattern(TypePattern type = null, VariableDesignationPattern designation = null, Action<DeclarationPatternSyntax> action = null)
        {
            return new DeclarationPatternPattern(type, designation, action);
        }
        public static VarPatternPattern VarPattern(VariableDesignationPattern designation = null, Action<VarPatternSyntax> action = null)
        {
            return new VarPatternPattern(designation, action);
        }
        public static RecursivePatternPattern RecursivePattern(TypePattern type = null, PositionalPatternClausePattern positionalPatternClause = null, PropertyPatternClausePattern propertyPatternClause = null, VariableDesignationPattern designation = null, Action<RecursivePatternSyntax> action = null)
        {
            return new RecursivePatternPattern(type, positionalPatternClause, propertyPatternClause, designation, action);
        }
        public static PositionalPatternClausePattern PositionalPatternClause(IEnumerable<SubpatternPattern> subpatterns = null, Action<PositionalPatternClauseSyntax> action = null)
        {
            return new PositionalPatternClausePattern(NodeList(subpatterns), action);
        }

        public static PositionalPatternClausePattern PositionalPatternClause(params SubpatternPattern[] subpatterns)
        {
            return new PositionalPatternClausePattern(NodeList(subpatterns), null);
        }
        public static PropertyPatternClausePattern PropertyPatternClause(IEnumerable<SubpatternPattern> subpatterns = null, Action<PropertyPatternClauseSyntax> action = null)
        {
            return new PropertyPatternClausePattern(NodeList(subpatterns), action);
        }

        public static PropertyPatternClausePattern PropertyPatternClause(params SubpatternPattern[] subpatterns)
        {
            return new PropertyPatternClausePattern(NodeList(subpatterns), null);
        }
        public static SubpatternPattern Subpattern(NameColonPattern nameColon = null, PatternPattern pattern = null, Action<SubpatternSyntax> action = null)
        {
            return new SubpatternPattern(nameColon, pattern, action);
        }
        public static ConstantPatternPattern ConstantPattern(ExpressionPattern expression = null, Action<ConstantPatternSyntax> action = null)
        {
            return new ConstantPatternPattern(expression, action);
        }
        public static ParenthesizedPatternPattern ParenthesizedPattern(PatternPattern pattern = null, Action<ParenthesizedPatternSyntax> action = null)
        {
            return new ParenthesizedPatternPattern(pattern, action);
        }
        public static RelationalPatternPattern RelationalPattern(ExpressionPattern expression = null, Action<RelationalPatternSyntax> action = null)
        {
            return new RelationalPatternPattern(expression, action);
        }
        public static TypePatternPattern TypePattern(TypePattern type = null, Action<TypePatternSyntax> action = null)
        {
            return new TypePatternPattern(type, action);
        }
        public static BinaryPatternPattern BinaryPattern(SyntaxKind kind = default(SyntaxKind), PatternPattern left = null, PatternPattern right = null, Action<BinaryPatternSyntax> action = null)
        {
            return new BinaryPatternPattern(kind, left, right, action);
        }
        public static UnaryPatternPattern UnaryPattern(PatternPattern pattern = null, Action<UnaryPatternSyntax> action = null)
        {
            return new UnaryPatternPattern(pattern, action);
        }
        public static InterpolatedStringTextPattern InterpolatedStringText(Action<InterpolatedStringTextSyntax> action = null)
        {
            return new InterpolatedStringTextPattern(action);
        }
        public static InterpolationPattern Interpolation(ExpressionPattern expression = null, InterpolationAlignmentClausePattern alignmentClause = null, InterpolationFormatClausePattern formatClause = null, Action<InterpolationSyntax> action = null)
        {
            return new InterpolationPattern(expression, alignmentClause, formatClause, action);
        }
        public static InterpolationAlignmentClausePattern InterpolationAlignmentClause(ExpressionPattern value = null, Action<InterpolationAlignmentClauseSyntax> action = null)
        {
            return new InterpolationAlignmentClausePattern(value, action);
        }
        public static InterpolationFormatClausePattern InterpolationFormatClause(Action<InterpolationFormatClauseSyntax> action = null)
        {
            return new InterpolationFormatClausePattern(action);
        }
        public static GlobalStatementPattern GlobalStatement(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, StatementPattern statement = null, Action<GlobalStatementSyntax> action = null)
        {
            return new GlobalStatementPattern(NodeList(attributeLists), TokenList(modifiers), statement, action);
        }
        public static BlockPattern Block(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<StatementPattern> statements = null, Action<BlockSyntax> action = null)
        {
            return new BlockPattern(NodeList(attributeLists), NodeList(statements), action);
        }

        public static BlockPattern Block(params StatementPattern[] statements)
        {
            return new BlockPattern(null, NodeList(statements), null);
        }
        public static LocalFunctionStatementPattern LocalFunctionStatement(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern returnType = null, string identifier = null, TypeParameterListPattern typeParameterList = null, ParameterListPattern parameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, Action<LocalFunctionStatementSyntax> action = null)
        {
            return new LocalFunctionStatementPattern(NodeList(attributeLists), TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), action);
        }
        public static LocalDeclarationStatementPattern LocalDeclarationStatement(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<LocalDeclarationStatementSyntax> action = null)
        {
            return new LocalDeclarationStatementPattern(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public static VariableDeclarationPattern VariableDeclaration(TypePattern type = null, IEnumerable<VariableDeclaratorPattern> variables = null, Action<VariableDeclarationSyntax> action = null)
        {
            return new VariableDeclarationPattern(type, NodeList(variables), action);
        }

        public static VariableDeclarationPattern VariableDeclaration(params VariableDeclaratorPattern[] variables)
        {
            return new VariableDeclarationPattern(null, NodeList(variables), null);
        }
        public static VariableDeclaratorPattern VariableDeclarator(string identifier = null, BracketedArgumentListPattern argumentList = null, EqualsValueClausePattern initializer = null, Action<VariableDeclaratorSyntax> action = null)
        {
            return new VariableDeclaratorPattern(identifier, argumentList, initializer, action);
        }
        public static EqualsValueClausePattern EqualsValueClause(ExpressionPattern value = null, Action<EqualsValueClauseSyntax> action = null)
        {
            return new EqualsValueClausePattern(value, action);
        }
        public static SingleVariableDesignationPattern SingleVariableDesignation(string identifier = null, Action<SingleVariableDesignationSyntax> action = null)
        {
            return new SingleVariableDesignationPattern(identifier, action);
        }
        public static DiscardDesignationPattern DiscardDesignation(Action<DiscardDesignationSyntax> action = null)
        {
            return new DiscardDesignationPattern(action);
        }
        public static ParenthesizedVariableDesignationPattern ParenthesizedVariableDesignation(IEnumerable<VariableDesignationPattern> variables = null, Action<ParenthesizedVariableDesignationSyntax> action = null)
        {
            return new ParenthesizedVariableDesignationPattern(NodeList(variables), action);
        }

        public static ParenthesizedVariableDesignationPattern ParenthesizedVariableDesignation(params VariableDesignationPattern[] variables)
        {
            return new ParenthesizedVariableDesignationPattern(NodeList(variables), null);
        }
        public static ExpressionStatementPattern ExpressionStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, Action<ExpressionStatementSyntax> action = null)
        {
            return new ExpressionStatementPattern(NodeList(attributeLists), expression, action);
        }
        public static EmptyStatementPattern EmptyStatement(IEnumerable<AttributeListPattern> attributeLists = null, Action<EmptyStatementSyntax> action = null)
        {
            return new EmptyStatementPattern(NodeList(attributeLists), action);
        }
        public static LabeledStatementPattern LabeledStatement(IEnumerable<AttributeListPattern> attributeLists = null, string identifier = null, StatementPattern statement = null, Action<LabeledStatementSyntax> action = null)
        {
            return new LabeledStatementPattern(NodeList(attributeLists), identifier, statement, action);
        }
        public static GotoStatementPattern GotoStatement(IEnumerable<AttributeListPattern> attributeLists = null, SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<GotoStatementSyntax> action = null)
        {
            return new GotoStatementPattern(NodeList(attributeLists), kind, expression, action);
        }
        public static BreakStatementPattern BreakStatement(IEnumerable<AttributeListPattern> attributeLists = null, Action<BreakStatementSyntax> action = null)
        {
            return new BreakStatementPattern(NodeList(attributeLists), action);
        }
        public static ContinueStatementPattern ContinueStatement(IEnumerable<AttributeListPattern> attributeLists = null, Action<ContinueStatementSyntax> action = null)
        {
            return new ContinueStatementPattern(NodeList(attributeLists), action);
        }
        public static ReturnStatementPattern ReturnStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, Action<ReturnStatementSyntax> action = null)
        {
            return new ReturnStatementPattern(NodeList(attributeLists), expression, action);
        }
        public static ThrowStatementPattern ThrowStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, Action<ThrowStatementSyntax> action = null)
        {
            return new ThrowStatementPattern(NodeList(attributeLists), expression, action);
        }
        public static YieldStatementPattern YieldStatement(IEnumerable<AttributeListPattern> attributeLists = null, SyntaxKind kind = default(SyntaxKind), ExpressionPattern expression = null, Action<YieldStatementSyntax> action = null)
        {
            return new YieldStatementPattern(NodeList(attributeLists), kind, expression, action);
        }
        public static WhileStatementPattern WhileStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern condition = null, StatementPattern statement = null, Action<WhileStatementSyntax> action = null)
        {
            return new WhileStatementPattern(NodeList(attributeLists), condition, statement, action);
        }
        public static DoStatementPattern DoStatement(IEnumerable<AttributeListPattern> attributeLists = null, StatementPattern statement = null, ExpressionPattern condition = null, Action<DoStatementSyntax> action = null)
        {
            return new DoStatementPattern(NodeList(attributeLists), statement, condition, action);
        }
        public static ForStatementPattern ForStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern condition = null, IEnumerable<ExpressionPattern> incrementors = null, StatementPattern statement = null, Action<ForStatementSyntax> action = null)
        {
            return new ForStatementPattern(NodeList(attributeLists), condition, NodeList(incrementors), statement, action);
        }

        public static ForStatementPattern ForStatement(params ExpressionPattern[] incrementors)
        {
            return new ForStatementPattern(null, null, NodeList(incrementors), null, null);
        }
        public static ForEachStatementPattern ForEachStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, StatementPattern statement = null, TypePattern type = null, string identifier = null, Action<ForEachStatementSyntax> action = null)
        {
            return new ForEachStatementPattern(NodeList(attributeLists), expression, statement, type, identifier, action);
        }
        public static ForEachVariableStatementPattern ForEachVariableStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, StatementPattern statement = null, ExpressionPattern variable = null, Action<ForEachVariableStatementSyntax> action = null)
        {
            return new ForEachVariableStatementPattern(NodeList(attributeLists), expression, statement, variable, action);
        }
        public static UsingStatementPattern UsingStatement(IEnumerable<AttributeListPattern> attributeLists = null, StatementPattern statement = null, Action<UsingStatementSyntax> action = null)
        {
            return new UsingStatementPattern(NodeList(attributeLists), statement, action);
        }
        public static FixedStatementPattern FixedStatement(IEnumerable<AttributeListPattern> attributeLists = null, VariableDeclarationPattern declaration = null, StatementPattern statement = null, Action<FixedStatementSyntax> action = null)
        {
            return new FixedStatementPattern(NodeList(attributeLists), declaration, statement, action);
        }
        public static CheckedStatementPattern CheckedStatement(IEnumerable<AttributeListPattern> attributeLists = null, SyntaxKind kind = default(SyntaxKind), BlockPattern block = null, Action<CheckedStatementSyntax> action = null)
        {
            return new CheckedStatementPattern(NodeList(attributeLists), kind, block, action);
        }
        public static UnsafeStatementPattern UnsafeStatement(IEnumerable<AttributeListPattern> attributeLists = null, BlockPattern block = null, Action<UnsafeStatementSyntax> action = null)
        {
            return new UnsafeStatementPattern(NodeList(attributeLists), block, action);
        }
        public static LockStatementPattern LockStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, StatementPattern statement = null, Action<LockStatementSyntax> action = null)
        {
            return new LockStatementPattern(NodeList(attributeLists), expression, statement, action);
        }
        public static IfStatementPattern IfStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern condition = null, StatementPattern statement = null, ElseClausePattern @else = null, Action<IfStatementSyntax> action = null)
        {
            return new IfStatementPattern(NodeList(attributeLists), condition, statement, @else, action);
        }
        public static ElseClausePattern ElseClause(StatementPattern statement = null, Action<ElseClauseSyntax> action = null)
        {
            return new ElseClausePattern(statement, action);
        }
        public static SwitchStatementPattern SwitchStatement(IEnumerable<AttributeListPattern> attributeLists = null, ExpressionPattern expression = null, IEnumerable<SwitchSectionPattern> sections = null, Action<SwitchStatementSyntax> action = null)
        {
            return new SwitchStatementPattern(NodeList(attributeLists), expression, NodeList(sections), action);
        }

        public static SwitchStatementPattern SwitchStatement(params SwitchSectionPattern[] sections)
        {
            return new SwitchStatementPattern(null, null, NodeList(sections), null);
        }
        public static SwitchSectionPattern SwitchSection(IEnumerable<SwitchLabelPattern> labels = null, IEnumerable<StatementPattern> statements = null, Action<SwitchSectionSyntax> action = null)
        {
            return new SwitchSectionPattern(NodeList(labels), NodeList(statements), action);
        }
        public static CasePatternSwitchLabelPattern CasePatternSwitchLabel(PatternPattern pattern = null, WhenClausePattern whenClause = null, Action<CasePatternSwitchLabelSyntax> action = null)
        {
            return new CasePatternSwitchLabelPattern(pattern, whenClause, action);
        }
        public static CaseSwitchLabelPattern CaseSwitchLabel(ExpressionPattern value = null, Action<CaseSwitchLabelSyntax> action = null)
        {
            return new CaseSwitchLabelPattern(value, action);
        }
        public static DefaultSwitchLabelPattern DefaultSwitchLabel(Action<DefaultSwitchLabelSyntax> action = null)
        {
            return new DefaultSwitchLabelPattern(action);
        }
        public static SwitchExpressionPattern SwitchExpression(ExpressionPattern governingExpression = null, IEnumerable<SwitchExpressionArmPattern> arms = null, Action<SwitchExpressionSyntax> action = null)
        {
            return new SwitchExpressionPattern(governingExpression, NodeList(arms), action);
        }

        public static SwitchExpressionPattern SwitchExpression(params SwitchExpressionArmPattern[] arms)
        {
            return new SwitchExpressionPattern(null, NodeList(arms), null);
        }
        public static SwitchExpressionArmPattern SwitchExpressionArm(PatternPattern pattern = null, WhenClausePattern whenClause = null, ExpressionPattern expression = null, Action<SwitchExpressionArmSyntax> action = null)
        {
            return new SwitchExpressionArmPattern(pattern, whenClause, expression, action);
        }
        public static TryStatementPattern TryStatement(IEnumerable<AttributeListPattern> attributeLists = null, BlockPattern block = null, IEnumerable<CatchClausePattern> catches = null, FinallyClausePattern @finally = null, Action<TryStatementSyntax> action = null)
        {
            return new TryStatementPattern(NodeList(attributeLists), block, NodeList(catches), @finally, action);
        }
        public static CatchClausePattern CatchClause(CatchDeclarationPattern declaration = null, CatchFilterClausePattern filter = null, BlockPattern block = null, Action<CatchClauseSyntax> action = null)
        {
            return new CatchClausePattern(declaration, filter, block, action);
        }
        public static CatchDeclarationPattern CatchDeclaration(TypePattern type = null, string identifier = null, Action<CatchDeclarationSyntax> action = null)
        {
            return new CatchDeclarationPattern(type, identifier, action);
        }
        public static CatchFilterClausePattern CatchFilterClause(ExpressionPattern filterExpression = null, Action<CatchFilterClauseSyntax> action = null)
        {
            return new CatchFilterClausePattern(filterExpression, action);
        }
        public static FinallyClausePattern FinallyClause(BlockPattern block = null, Action<FinallyClauseSyntax> action = null)
        {
            return new FinallyClausePattern(block, action);
        }
        public static CompilationUnitPattern CompilationUnit(IEnumerable<ExternAliasDirectivePattern> externs = null, IEnumerable<UsingDirectivePattern> usings = null, IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<MemberDeclarationPattern> members = null, Action<CompilationUnitSyntax> action = null)
        {
            return new CompilationUnitPattern(NodeList(externs), NodeList(usings), NodeList(attributeLists), NodeList(members), action);
        }
        public static ExternAliasDirectivePattern ExternAliasDirective(string identifier = null, Action<ExternAliasDirectiveSyntax> action = null)
        {
            return new ExternAliasDirectivePattern(identifier, action);
        }
        public static UsingDirectivePattern UsingDirective(NamePattern name = null, Action<UsingDirectiveSyntax> action = null)
        {
            return new UsingDirectivePattern(name, action);
        }
        public static NamespaceDeclarationPattern NamespaceDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, NamePattern name = null, IEnumerable<ExternAliasDirectivePattern> externs = null, IEnumerable<UsingDirectivePattern> usings = null, IEnumerable<MemberDeclarationPattern> members = null, Action<NamespaceDeclarationSyntax> action = null)
        {
            return new NamespaceDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), name, NodeList(externs), NodeList(usings), NodeList(members), action);
        }
        public static AttributeListPattern AttributeList(AttributeTargetSpecifierPattern target = null, IEnumerable<AttributePattern> attributes = null, Action<AttributeListSyntax> action = null)
        {
            return new AttributeListPattern(target, NodeList(attributes), action);
        }

        public static AttributeListPattern AttributeList(params AttributePattern[] attributes)
        {
            return new AttributeListPattern(null, NodeList(attributes), null);
        }
        public static AttributeTargetSpecifierPattern AttributeTargetSpecifier(string identifier = null, Action<AttributeTargetSpecifierSyntax> action = null)
        {
            return new AttributeTargetSpecifierPattern(identifier, action);
        }
        public static AttributePattern Attribute(NamePattern name = null, AttributeArgumentListPattern argumentList = null, Action<AttributeSyntax> action = null)
        {
            return new AttributePattern(name, argumentList, action);
        }
        public static AttributeArgumentListPattern AttributeArgumentList(IEnumerable<AttributeArgumentPattern> arguments = null, Action<AttributeArgumentListSyntax> action = null)
        {
            return new AttributeArgumentListPattern(NodeList(arguments), action);
        }

        public static AttributeArgumentListPattern AttributeArgumentList(params AttributeArgumentPattern[] arguments)
        {
            return new AttributeArgumentListPattern(NodeList(arguments), null);
        }
        public static AttributeArgumentPattern AttributeArgument(ExpressionPattern expression = null, Action<AttributeArgumentSyntax> action = null)
        {
            return new AttributeArgumentPattern(expression, action);
        }
        public static NameEqualsPattern NameEquals(IdentifierNamePattern name = null, Action<NameEqualsSyntax> action = null)
        {
            return new NameEqualsPattern(name, action);
        }
        public static TypeParameterListPattern TypeParameterList(IEnumerable<TypeParameterPattern> parameters = null, Action<TypeParameterListSyntax> action = null)
        {
            return new TypeParameterListPattern(NodeList(parameters), action);
        }

        public static TypeParameterListPattern TypeParameterList(params TypeParameterPattern[] parameters)
        {
            return new TypeParameterListPattern(NodeList(parameters), null);
        }
        public static TypeParameterPattern TypeParameter(IEnumerable<AttributeListPattern> attributeLists = null, string identifier = null, Action<TypeParameterSyntax> action = null)
        {
            return new TypeParameterPattern(NodeList(attributeLists), identifier, action);
        }
        public static ClassDeclarationPattern ClassDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<ClassDeclarationSyntax> action = null)
        {
            return new ClassDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static ClassDeclarationPattern ClassDeclaration(params MemberDeclarationPattern[] members)
        {
            return new ClassDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static StructDeclarationPattern StructDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<StructDeclarationSyntax> action = null)
        {
            return new StructDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static StructDeclarationPattern StructDeclaration(params MemberDeclarationPattern[] members)
        {
            return new StructDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static InterfaceDeclarationPattern InterfaceDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, Action<InterfaceDeclarationSyntax> action = null)
        {
            return new InterfaceDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), action);
        }

        public static InterfaceDeclarationPattern InterfaceDeclaration(params MemberDeclarationPattern[] members)
        {
            return new InterfaceDeclarationPattern(null, null, null, null, null, null, NodeList(members), null);
        }
        public static RecordDeclarationPattern RecordDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, IEnumerable<MemberDeclarationPattern> members = null, ParameterListPattern parameterList = null, Action<RecordDeclarationSyntax> action = null)
        {
            return new RecordDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, typeParameterList, NodeList(constraintClauses), NodeList(members), parameterList, action);
        }

        public static RecordDeclarationPattern RecordDeclaration(params MemberDeclarationPattern[] members)
        {
            return new RecordDeclarationPattern(null, null, null, null, null, null, NodeList(members), null, null);
        }
        public static EnumDeclarationPattern EnumDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, BaseListPattern baseList = null, IEnumerable<EnumMemberDeclarationPattern> members = null, Action<EnumDeclarationSyntax> action = null)
        {
            return new EnumDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, baseList, NodeList(members), action);
        }

        public static EnumDeclarationPattern EnumDeclaration(params EnumMemberDeclarationPattern[] members)
        {
            return new EnumDeclarationPattern(null, null, null, null, NodeList(members), null);
        }
        public static DelegateDeclarationPattern DelegateDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern returnType = null, string identifier = null, TypeParameterListPattern typeParameterList = null, ParameterListPattern parameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, Action<DelegateDeclarationSyntax> action = null)
        {
            return new DelegateDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), returnType, identifier, typeParameterList, parameterList, NodeList(constraintClauses), action);
        }
        public static EnumMemberDeclarationPattern EnumMemberDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, string identifier = null, EqualsValueClausePattern equalsValue = null, Action<EnumMemberDeclarationSyntax> action = null)
        {
            return new EnumMemberDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), identifier, equalsValue, action);
        }
        public static BaseListPattern BaseList(IEnumerable<BaseTypePattern> types = null, Action<BaseListSyntax> action = null)
        {
            return new BaseListPattern(NodeList(types), action);
        }

        public static BaseListPattern BaseList(params BaseTypePattern[] types)
        {
            return new BaseListPattern(NodeList(types), null);
        }
        public static SimpleBaseTypePattern SimpleBaseType(TypePattern type = null, Action<SimpleBaseTypeSyntax> action = null)
        {
            return new SimpleBaseTypePattern(type, action);
        }
        public static PrimaryConstructorBaseTypePattern PrimaryConstructorBaseType(TypePattern type = null, ArgumentListPattern argumentList = null, Action<PrimaryConstructorBaseTypeSyntax> action = null)
        {
            return new PrimaryConstructorBaseTypePattern(type, argumentList, action);
        }
        public static TypeParameterConstraintClausePattern TypeParameterConstraintClause(IdentifierNamePattern name = null, IEnumerable<TypeParameterConstraintPattern> constraints = null, Action<TypeParameterConstraintClauseSyntax> action = null)
        {
            return new TypeParameterConstraintClausePattern(name, NodeList(constraints), action);
        }
        public static ConstructorConstraintPattern ConstructorConstraint(Action<ConstructorConstraintSyntax> action = null)
        {
            return new ConstructorConstraintPattern(action);
        }
        public static ClassOrStructConstraintPattern ClassOrStructConstraint(SyntaxKind kind = default(SyntaxKind), Action<ClassOrStructConstraintSyntax> action = null)
        {
            return new ClassOrStructConstraintPattern(kind, action);
        }
        public static TypeConstraintPattern TypeConstraint(TypePattern type = null, Action<TypeConstraintSyntax> action = null)
        {
            return new TypeConstraintPattern(type, action);
        }
        public static DefaultConstraintPattern DefaultConstraint(Action<DefaultConstraintSyntax> action = null)
        {
            return new DefaultConstraintPattern(action);
        }
        public static FieldDeclarationPattern FieldDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<FieldDeclarationSyntax> action = null)
        {
            return new FieldDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public static EventFieldDeclarationPattern EventFieldDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, VariableDeclarationPattern declaration = null, Action<EventFieldDeclarationSyntax> action = null)
        {
            return new EventFieldDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), declaration, action);
        }
        public static ExplicitInterfaceSpecifierPattern ExplicitInterfaceSpecifier(NamePattern name = null, Action<ExplicitInterfaceSpecifierSyntax> action = null)
        {
            return new ExplicitInterfaceSpecifierPattern(name, action);
        }
        public static MethodDeclarationPattern MethodDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, TypePattern returnType = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, string identifier = null, TypeParameterListPattern typeParameterList = null, IEnumerable<TypeParameterConstraintClausePattern> constraintClauses = null, Action<MethodDeclarationSyntax> action = null)
        {
            return new MethodDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, NodeList(constraintClauses), action);
        }
        public static OperatorDeclarationPattern OperatorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, TypePattern returnType = null, Action<OperatorDeclarationSyntax> action = null)
        {
            return new OperatorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, returnType, action);
        }
        public static ConversionOperatorDeclarationPattern ConversionOperatorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, TypePattern type = null, Action<ConversionOperatorDeclarationSyntax> action = null)
        {
            return new ConversionOperatorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, type, action);
        }
        public static ConstructorDeclarationPattern ConstructorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, string identifier = null, ConstructorInitializerPattern initializer = null, Action<ConstructorDeclarationSyntax> action = null)
        {
            return new ConstructorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, identifier, initializer, action);
        }
        public static ConstructorInitializerPattern ConstructorInitializer(SyntaxKind kind = default(SyntaxKind), ArgumentListPattern argumentList = null, Action<ConstructorInitializerSyntax> action = null)
        {
            return new ConstructorInitializerPattern(kind, argumentList, action);
        }
        public static DestructorDeclarationPattern DestructorDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, ParameterListPattern parameterList = null, string identifier = null, Action<DestructorDeclarationSyntax> action = null)
        {
            return new DestructorDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), parameterList, identifier, action);
        }
        public static PropertyDeclarationPattern PropertyDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, string identifier = null, Action<PropertyDeclarationSyntax> action = null)
        {
            return new PropertyDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, action);
        }
        public static ArrowExpressionClausePattern ArrowExpressionClause(ExpressionPattern expression = null, Action<ArrowExpressionClauseSyntax> action = null)
        {
            return new ArrowExpressionClausePattern(expression, action);
        }
        public static EventDeclarationPattern EventDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, string identifier = null, Action<EventDeclarationSyntax> action = null)
        {
            return new EventDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, identifier, action);
        }
        public static IndexerDeclarationPattern IndexerDeclaration(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, ExplicitInterfaceSpecifierPattern explicitInterfaceSpecifier = null, AccessorListPattern accessorList = null, BracketedParameterListPattern parameterList = null, Action<IndexerDeclarationSyntax> action = null)
        {
            return new IndexerDeclarationPattern(NodeList(attributeLists), TokenList(modifiers), type, explicitInterfaceSpecifier, accessorList, parameterList, action);
        }
        public static AccessorListPattern AccessorList(IEnumerable<AccessorDeclarationPattern> accessors = null, Action<AccessorListSyntax> action = null)
        {
            return new AccessorListPattern(NodeList(accessors), action);
        }

        public static AccessorListPattern AccessorList(params AccessorDeclarationPattern[] accessors)
        {
            return new AccessorListPattern(NodeList(accessors), null);
        }
        public static AccessorDeclarationPattern AccessorDeclaration(SyntaxKind kind = default(SyntaxKind), IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, Action<AccessorDeclarationSyntax> action = null)
        {
            return new AccessorDeclarationPattern(kind, NodeList(attributeLists), TokenList(modifiers), action);
        }
        public static ParameterListPattern ParameterList(IEnumerable<ParameterPattern> parameters = null, Action<ParameterListSyntax> action = null)
        {
            return new ParameterListPattern(NodeList(parameters), action);
        }

        public static ParameterListPattern ParameterList(params ParameterPattern[] parameters)
        {
            return new ParameterListPattern(NodeList(parameters), null);
        }
        public static BracketedParameterListPattern BracketedParameterList(IEnumerable<ParameterPattern> parameters = null, Action<BracketedParameterListSyntax> action = null)
        {
            return new BracketedParameterListPattern(NodeList(parameters), action);
        }

        public static BracketedParameterListPattern BracketedParameterList(params ParameterPattern[] parameters)
        {
            return new BracketedParameterListPattern(NodeList(parameters), null);
        }
        public static ParameterPattern Parameter(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, string identifier = null, EqualsValueClausePattern @default = null, Action<ParameterSyntax> action = null)
        {
            return new ParameterPattern(NodeList(attributeLists), TokenList(modifiers), type, identifier, @default, action);
        }
        public static FunctionPointerParameterPattern FunctionPointerParameter(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, Action<FunctionPointerParameterSyntax> action = null)
        {
            return new FunctionPointerParameterPattern(NodeList(attributeLists), TokenList(modifiers), type, action);
        }
        public static IncompleteMemberPattern IncompleteMember(IEnumerable<AttributeListPattern> attributeLists = null, IEnumerable<string> modifiers = null, TypePattern type = null, Action<IncompleteMemberSyntax> action = null)
        {
            return new IncompleteMemberPattern(NodeList(attributeLists), TokenList(modifiers), type, action);
        }
    }
}

﻿namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticGenericAttributeRecordBuilder : IRecordBuilder<ISemanticGenericAttributeRecord>
{
    public abstract void WithT1(ITypeSymbol t1, ExpressionSyntax syntax);
    public abstract void WithT2(ITypeSymbol t2, ExpressionSyntax syntax);
}
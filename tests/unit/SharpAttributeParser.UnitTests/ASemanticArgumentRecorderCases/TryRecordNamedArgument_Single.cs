﻿namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using System;

using Xunit;

public class TryRecordNamedArgument_Single
{
    private static bool Target(ASemanticArgumentRecorder recorder, string parameterName, object? value) => recorder.TryRecordNamedArgument(parameterName, value);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetNullParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var exception = Record.Exception(() => Target(recorder, parameterName, value));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetNullTypeSymbol();

        var actual = Target(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Null(recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        var parameterName = Datasets.GetValidParameterName();
        var value = Datasets.GetValidTypeSymbol();

        var actual = Target(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }
}

﻿namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System;

using Xunit;

public class TryRecordGenericArgument
{
    private static bool Target(ASyntacticArgumentRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol value, Location location) => recorder.TryRecordGenericArgument(parameter, value, location);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetNullTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => Target(recorder, parameter, value, location));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullValue_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetNullTypeSymbol();
        var location = Datasets.GetValidLocation();

        var exception = Record.Exception(() => Target(recorder, parameter, value, location));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullLocation_ArgumentNullException()
    {
        SyntacticArgumentRecorder recorder = new(StringComparer.OrdinalIgnoreCase);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetNullLocation();

        var exception = Record.Exception(() => Target(recorder, parameter, value, location));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Matching_True_ArgumentRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var actual = Target(recorder, parameter, value, location);

        Assert.True(actual);

        Assert.Equal(value, recorder.TGeneric);
        Assert.Equal(location, recorder.TGenericLocation);
    }

    [Fact]
    public void NotMatching_False_ArgumentNotRecorded()
    {
        var comparerMock = ComparerMock.GetComparer(false);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        var parameter = Datasets.GetMockedTypeParameter();
        var value = Datasets.GetValidTypeSymbol();
        var location = Datasets.GetValidLocation();

        var actual = Target(recorder, parameter, value, location);

        Assert.False(actual);

        Assert.Null(recorder.TGeneric);
        Assert.Null(recorder.TGenericLocation);
    }
}

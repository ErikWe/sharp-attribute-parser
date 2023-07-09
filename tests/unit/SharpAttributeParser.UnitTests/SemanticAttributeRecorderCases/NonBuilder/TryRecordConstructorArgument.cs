﻿namespace SharpAttributeParser.SemanticAttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class TryRecordConstructorArgument
{
    private ISemanticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordConstructorArgument(ISemanticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISemanticAttributeRecorder recorder, IParameterSymbol parameter, object? argument) => recorder.TryRecordConstructorArgument(parameter, argument);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISemanticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISemanticAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), null);

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<ISemanticAttributeMapper<string>> mapperMock = new();

        var argumentRecorded = false;

        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<string>())).Returns(tryMapConstructorParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), null);

        Assert.Equal(returnValue, outcome);

        Assert.True(argumentRecorded);

        ISemanticAttributeArgumentRecorder? tryMapConstructorParameter() => new SemanticAttributeArgumentRecorder((object? argument) =>
        {
            argumentRecorded = true;

            return returnValue;
        });
    }
}
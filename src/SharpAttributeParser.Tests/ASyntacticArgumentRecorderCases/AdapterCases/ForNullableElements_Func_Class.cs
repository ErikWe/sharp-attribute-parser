﻿namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases.AdapterCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

using Xunit;

public class ForNullableElements_Func_Class
{
    private static bool TryRecordConstructorArgument(ASyntacticArgumentRecorder recorder, string parameterName, IReadOnlyList<object?>? value) => recorder.TryRecordConstructorArgument(parameterName, value, Location.None, Array.Empty<Location>());

    [Fact]
    public void String_String_True_RecorderPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { "1", "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new[] { "1", "2" }, recorder.Value);
    }

    [Fact]
    public void String_Enum_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { StringComparison.OrdinalIgnoreCase, "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void String_NullString_True_RecorderPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { null, "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.True(actual);

        Assert.Equal(new string?[] { null, "2" }, recorder.Value);
    }

    [Fact]
    public void String_Null_False_RecorderNotPopulated()
    {
        StringRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object>? value = null;

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    [Fact]
    public void FalseRecorder_False_RecorderPopulated()
    {
        FalseRecorder recorder = new();

        var parameterName = "Value";
        IReadOnlyList<object?> value = new object?[] { "1", "2" };

        var actual = TryRecordConstructorArgument(recorder, parameterName, value);

        Assert.False(actual);

        Assert.Equal(new[] { "1", "2" }, recorder.Value);
    }

    private sealed class StringRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<string?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableElements<string>(RecordStringArray).Invoke);
        }

        private bool RecordStringArray(IReadOnlyList<string?> value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return true;
        }
    }

    private sealed class FalseRecorder : ASyntacticArgumentRecorder
    {
        public IReadOnlyList<string?>? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Value", Adapters.ForNullableElements<string>(RecordStringArray).Invoke);
        }

        private bool RecordStringArray(IReadOnlyList<string?> value, Location collectionLocation, IReadOnlyList<Location> elementLocations)
        {
            Value = value;
            ValueRecorded = true;

            return false;
        }
    }
}

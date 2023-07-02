﻿namespace SharpAttributeParser.ASemanticArgumentRecorderCases.AdapterCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class For_Action_Single
{
    private static DSemanticSingleRecorder Target<T>(ISemanticAdapterProvider adapters, Action<T> recorder) where T : notnull => adapters.For(recorder);

    [Fact]
    public void NullDelegate_ArgumentNullExceptionWhenUsed()
    {
        NullDelegateRecorder recorder = new();

        var exception = Record.Exception(() => RecordArgument(recorder, null));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Enum_SameType_True_Recorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Enum_DifferentEnumType_False_NotRecorded()
    {
        var value = StringSplitOptions.TrimEntries;

        FalseAndNotRecorded<StringComparison, StringSplitOptions>(value);
    }

    [Fact]
    public void Enum_Int_False_NotRecorded()
    {
        var value = 3;

        FalseAndNotRecorded<StringComparison, int>(value);
    }

    [Fact]
    public void Int_SameType_True_Recorded()
    {
        var value = 3;

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void Int_NullableWithValue_True_Recorded()
    {
        var expected = 3;

        var value = (int?)expected;

        TrueAndRecorded(expected, value);
    }

    [Fact]
    public void Int_Enum_False_NotRecorded()
    {
        var value = StringComparison.Ordinal;

        FalseAndNotRecorded<int, StringComparison>(value);
    }

    [Fact]
    public void Int_Double_False_NotRecorded()
    {
        var value = 3.14;

        FalseAndNotRecorded<int, double>(value);
    }

    [Fact]
    public void Int_String_False_NotRecorded()
    {
        var value = "3";

        FalseAndNotRecorded<int, string>(value);
    }

    [Fact]
    public void Int_Null_False_NotRecorded()
    {
        int? value = null;

        FalseAndNotRecorded<int, int?>(value);
    }

    [Fact]
    public void Double_Int_False_NotRecorded()
    {
        var value = 5;

        FalseAndNotRecorded<double, int>(value);
    }

    [Fact]
    public void NullableIntArray_NullElement_True_Recorded()
    {
        var value = new int?[] { null, 3 };

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void NullableIntArray_NullCollection_False_NotRecorded()
    {
        IReadOnlyList<int?>? value = null;

        FalseAndNotRecorded<IReadOnlyList<int?>, IReadOnlyList<int?>?>(value);
    }

    [Fact]
    public void String_SameType_True_Recorded()
    {
        var value = "1";

        TrueAndRecorded(value, value);
    }

    [Fact]
    public void String_Enum_False_NotRecorded()
    {
        var value = StringComparison.OrdinalIgnoreCase;

        FalseAndNotRecorded<string, StringComparison>(value);
    }

    [Fact]
    public void String_Null_False_NotRecorded()
    {
        string? value = null;

        FalseAndNotRecorded<string, string?>(value);
    }

    [AssertionMethod]
    private static void TrueAndRecorded<T1, T2>(T1 expected, T2 value) where T1 : notnull
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value);

        Assert.True(actual);

        Assert.Equal(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }

    [AssertionMethod]
    private static void FalseAndNotRecorded<T1, T2>(T2? value) where T1 : notnull
    {
        Recorder<T1> recorder = new();

        var actual = RecordArgument(recorder, value);

        Assert.False(actual);

        Assert.False(recorder.ValueRecorded);
    }

    private static bool RecordArgument(ASemanticArgumentRecorder recorder, object? value) => recorder.TryRecordNamedArgument(string.Empty, value);

    private sealed class NullDelegateRecorder : ASemanticArgumentRecorder
    {
        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<string>(Adapters, null!));
        }
    }

    private sealed class Recorder<T> : ASemanticArgumentRecorder where T : notnull
    {
        public T? Value { get; private set; }
        public bool ValueRecorded { get; private set; }

        protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (string.Empty, Target<T>(Adapters, RecordValue));
        }

        private void RecordValue(T value)
        {
            Value = value;
            ValueRecorded = true;
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static class Preconditions
{

#if NET6_0_OR_GREATER

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(argument, parameterName);
        return argument;
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DoThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = default)
    {
        DoThrowIfNull(argument, parameterName);
        return argument;
    }

#endif

#if NET8_0_OR_GREATER

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNegative<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
        return value;
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be greater than or equal to {other}.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be less than or equal to {other}.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ThrowIfNegative(sbyte value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ThrowIfNegative(short value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ThrowIfNegative(long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ThrowIfNegative(float value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ThrowIfNegative(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ThrowIfNegative(decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value must be positive or zero.");
        }
        return value;
    }

#endif

}
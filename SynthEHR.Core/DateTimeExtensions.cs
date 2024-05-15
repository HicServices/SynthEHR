using System;

namespace SynthEHR;

/// <summary>
/// Extension methods for the <see cref="DateTime"/> class
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the maximum of <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static DateTime Max(this DateTime a, DateTime b)
    {
            return new DateTime(Math.Max(a.Ticks, b.Ticks));
        }

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the minimum of <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static DateTime Min(this DateTime a, DateTime b)
    {
            return new DateTime(Math.Min(a.Ticks, b.Ticks));
        }

}
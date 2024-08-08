// <copyright file="Normal.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;

namespace SynthEHR.Statistics.Distributions;

/// <summary>
/// Continuous Univariate Normal distribution, also known as Gaussian distribution.
/// For details about this distribution, see
/// <a href="http://en.wikipedia.org/wiki/Normal_distribution">Wikipedia - Normal distribution</a>.
/// </summary>
internal sealed class Normal : IContinuousDistribution
{
    private Random _random;

    private readonly double _mean;
    private readonly double _stdDev;

    /// <summary>
    /// Initializes a new instance of the Normal class. This is a normal distribution with mean 0.0
    /// and standard deviation 1.0. The distribution will
    /// be initialized with the default <seealso cref="System.Random"/> random number generator.
    /// </summary>
    public Normal()
        : this(0.0,1.0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Normal class. This is a normal distribution with mean 0.0
    /// and standard deviation 1.0. The distribution will
    /// be initialized with the default <seealso cref="System.Random"/> random number generator.
    /// </summary>
    /// <param name="randomSource">The random number generator which is used to draw random samples.</param>
    public Normal(Random randomSource)
        : this(0.0,1.0,randomSource)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Normal class with a particular mean and standard deviation. The distribution will
    /// be initialized with the default <seealso cref="System.Random"/> random number generator.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="stddev">The standard deviation (σ) of the normal distribution. Range: σ ≥ 0.</param>
    public Normal(double mean,double stddev)
    {
        if (!IsValidParameterSet(mean,stddev))
        {
            throw new ArgumentException("Invalid parameterization for the distribution.");
        }

        _random = new Random();
        _mean = mean;
        _stdDev = stddev;
    }

    /// <summary>
    /// Initializes a new instance of the Normal class with a particular mean and standard deviation. The distribution will
    /// be initialized with the default <seealso cref="System.Random"/> random number generator.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="stddev">The standard deviation (σ) of the normal distribution. Range: σ ≥ 0.</param>
    /// <param name="randomSource">The random number generator which is used to draw random samples.</param>
    public Normal(double mean,double stddev,Random randomSource)
    {
        if (!IsValidParameterSet(mean,stddev))
        {
            throw new ArgumentException("Invalid parametrization for the distribution.");
        }

        _random = randomSource ?? new Random();
        _mean = mean;
        _stdDev = stddev;
    }

    /// <summary>
    /// Constructs a normal distribution from a mean and standard deviation.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="stddev">The standard deviation (σ) of the normal distribution. Range: σ ≥ 0.</param>
    /// <param name="randomSource">The random number generator which is used to draw random samples. Optional, can be null.</param>
    /// <returns>a normal distribution.</returns>
    public static Normal WithMeanStdDev(double mean,double stddev,Random randomSource = null)
    {
        return new Normal(mean,stddev,randomSource);
    }

    /// <summary>
    /// Constructs a normal distribution from a mean and variance.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="var">The variance (σ^2) of the normal distribution.</param>
    /// <param name="randomSource">The random number generator which is used to draw random samples. Optional, can be null.</param>
    /// <returns>A normal distribution.</returns>
    public static Normal WithMeanVariance(double mean,double var,Random randomSource = null)
    {
        return new Normal(mean,Math.Sqrt(var),randomSource);
    }

    /// <summary>
    /// Constructs a normal distribution from a mean and precision.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="precision">The precision of the normal distribution.</param>
    /// <param name="randomSource">The random number generator which is used to draw random samples. Optional, can be null.</param>
    /// <returns>A normal distribution.</returns>
    public static Normal WithMeanPrecision(double mean,double precision,Random randomSource = null)
    {
        return new Normal(mean,1.0/Math.Sqrt(precision),randomSource);
    }

    /// <summary>
    /// A string representation of the distribution.
    /// </summary>
    /// <returns>a string representation of the distribution.</returns>
    public override string ToString()
    {
        return $"Normal(μ = {_mean}, σ = {_stdDev})";
    }

    /// <summary>
    /// Tests whether the provided values are valid parameters for this distribution.
    /// </summary>
    /// <param name="mean">The mean (μ) of the normal distribution.</param>
    /// <param name="stddev">The standard deviation (σ) of the normal distribution. Range: σ ≥ 0.</param>
    public static bool IsValidParameterSet(double mean,double stddev)
    {
        return stddev >= 0.0 && !double.IsNaN(mean);
    }

    /// <summary>
    /// Gets the mean (μ) of the normal distribution.
    /// </summary>
    public double Mean => _mean;

    /// <summary>
    /// Gets the standard deviation (σ) of the normal distribution. Range: σ ≥ 0.
    /// </summary>
    public double StdDev => _stdDev;

    /// <summary>
    /// Gets the variance of the normal distribution.
    /// </summary>
    public double Variance => _stdDev*_stdDev;

    /// <summary>
    /// Gets the precision of the normal distribution.
    /// </summary>
    public double Precision => 1.0/(_stdDev*_stdDev);

    /// <summary>
    /// Gets the random number generator which is used to draw random samples.
    /// </summary>
    public Random RandomSource
    {
        get => _random;
        set => _random = value ?? new Random();
    }

    /// <summary>
    /// Gets the entropy of the normal distribution.
    /// </summary>
    public double Entropy => Math.Log(_stdDev) + LogSqrt2PiE;

    private const double LogSqrt2PiE = 1.4189385332046727417803297364056176398613974736378d;

    /// <summary>
    /// Gets the skewness of the normal distribution.
    /// </summary>
    public double Skewness => 0.0;

    /// <summary>
    /// Gets the mode of the normal distribution.
    /// </summary>
    public double Mode => _mean;

    /// <summary>
    /// Gets the median of the normal distribution.
    /// </summary>
    public double Median => _mean;

    /// <summary>
    /// Gets the minimum of the normal distribution.
    /// </summary>
    public double Minimum => double.NegativeInfinity;

    /// <summary>
    /// Gets the maximum of the normal distribution.
    /// </summary>
    public double Maximum => double.PositiveInfinity;

    /// <summary>
    /// Computes the probability density of the distribution (PDF) at x, i.e. ∂P(X ≤ x)/∂x.
    /// </summary>
    /// <param name="x">The location at which to compute the density.</param>
    /// <returns>the density at <paramref name="x"/>.</returns>
    public double Density(double x)
    {
        var d = (x - _mean)/_stdDev;
        return Math.Exp(-0.5*d*d)/(Sqrt2Pi*_stdDev);
    }

    private const double Sqrt2Pi = 2.5066282746310005024157652848110452530069867406099d;

    /// <summary>
    /// Computes the log probability density of the distribution (lnPDF) at x, i.e. ln(∂P(X ≤ x)/∂x).
    /// </summary>
    /// <param name="x">The location at which to compute the log density.</param>
    /// <returns>the log density at <paramref name="x"/>.</returns>
    public double DensityLn(double x)
    {
        var d = (x - _mean)/_stdDev;
        return (-0.5*d*d) - Math.Log(_stdDev) - LogSqrt2Pi;
    }

    private const double LogSqrt2Pi = 0.91893853320467274178032973640561763986139747363778;

    /// <summary>
    /// Generates a sample from the normal distribution using the <i>Box-Muller</i> algorithm.
    /// </summary>
    /// <returns>a sample from the distribution.</returns>
    public double Sample()
    {
        return SampleUnchecked(_random,_mean,_stdDev);
    }

    internal static double SampleUnchecked(Random rnd,double mean,double stddev)
    {
        double x;
        while (!PolarTransform(rnd.NextDouble(),rnd.NextDouble(),out x,out _))
        {
        }

        return mean + (stddev*x);
    }

    internal static IEnumerable<double> SamplesUnchecked(Random rnd,double mean,double stddev)
    {
        while (true)
        {
            if (!PolarTransform(rnd.NextDouble(),rnd.NextDouble(),out var x,out var y))
            {
                continue;
            }

            yield return mean + (stddev*x);
            yield return mean + (stddev*y);
        }
    }

    private const double InvPi = 0.31830988618379067153776752674502872406891929148091d;

    private static bool PolarTransform(double a,double b,out double x,out double y)
    {
        var v1 = (2.0*a) - 1.0;
        var v2 = (2.0*b) - 1.0;
        var r = (v1*v1) + (v2*v2);
        if (r >= 1.0 || r == 0.0)
        {
            x = 0;
            y = 0;
            return false;
        }

        var fac = Math.Sqrt(-2.0*Math.Log(r)/r);
        x = v1*fac;
        y = v2*fac;
        return true;
    }
}
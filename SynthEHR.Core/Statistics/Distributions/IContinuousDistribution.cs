﻿// <copyright file="IContinuousDistribution.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2014 Math.NET
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

namespace SynthEHR.Statistics.Distributions;

/// <summary>
/// Continuous Univariate Probability Distribution.
/// </summary>
internal interface IContinuousDistribution : IUnivariateDistribution
{
    /// <summary>
    /// Gets the mode of the distribution.
    /// </summary>
    double Mode { get; }

    /// <summary>
    /// Gets the smallest element in the domain of the distribution which can be represented by a double.
    /// </summary>
    double Minimum { get; }

    /// <summary>
    /// Gets the largest element in the domain of the distribution which can be represented by a double.
    /// </summary>
    double Maximum { get; }

    /// <summary>
    /// Draws a random sample from the distribution.
    /// </summary>
    /// <returns>a sample from the distribution.</returns>
    double Sample();
}
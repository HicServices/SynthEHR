using System;
using System.Threading;

namespace SynthEHR.Datasets;

/// <include file='../../Datasets.doc.xml' path='Datasets/UltraWide'/>
/// <inheritdoc/>
public sealed class UltraWide(Random rand) : DataGenerator(rand)
{
    int autonum = 1;

    /// <inheritdoc/>
    public override object[] GenerateTestDataRow(Person p)
    {
            r.Next();

            var array = new object[20_000];
            array[0] = Interlocked.Increment(ref autonum);
            array[1] = p.CHI;

            for (var i = 2; i < array.Length; i++)
            {
                array[i] = i switch
                {
                    < 4_000 => r.Next(),
                    < 8_000 => r.NextDouble(),
                    < 16_000 => GetRandomSentence(r),
                    < 19_500 => GetRandomGPCode(r),
                    _ => GetGaussianInt(50, 50000)
                };
            }

            return array;
        }

    /// <inheritdoc/>
    protected override string[] GetHeaders()
    {
            var array = new string[20_000];
            array[0] = "id";
            array[1] = "chi";

            for (var i = 2; i < array.Length; i++)
            {
                array[i] = $"col{i}";
            }

            return array;
        }
}
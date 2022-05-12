using System;
using System.Threading;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Generates a dataset with 20,000 columns of data in a veriety of formats.  This
    /// is useful for testing very wide tables that are too wide for most RDBMS without 
    /// special treatment (e.g. SPARSE columns).
    /// </summary>
    public class UltraWide : DataGenerator
    {
        int autonum = 1;

        /// <inheritdoc/>
        public UltraWide(Random rand) : base(rand)
        {
        }

        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            r.Next();

            var array = new object[20_000];
            array[0] = Interlocked.Increment(ref autonum);
            array[1] = p.CHI;

            for (int i = 2; i < array.Length; i++)
            {
                if (i < 4_000)
                    array[i] = r.Next();
                else
                if (i < 8_000)
                    array[i] = r.NextDouble();
                else
                if (i < 16_000)
                    array[i] = GetRandomSentence(r);
                else
                if (i < 19_500)
                    array[i] = GetRandomGPCode(r);
                else
                    array[i] = GetGaussianInt(50, 50000);
            }

            return array;
        }

        /// <inheritdoc/>
        protected override string[] GetHeaders()
        {
            var array = new string[20_000];
            array[0] = "id";
            array[1] = "chi";

            for (int i = 2; i < array.Length; i++)
            {
                array[i] = $"col{i}";
            }

            return array;
        }
    }
}

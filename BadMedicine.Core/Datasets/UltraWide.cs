using System;
using System.Threading;

namespace BadMedicine.Datasets
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/UltraWide'/>
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

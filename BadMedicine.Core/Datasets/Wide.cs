using System;
using System.Threading;

namespace BadMedicine.Datasets
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/Wide'/>
    public class Wide : DataGenerator
    {
        int autonum = 1;

        /// <inheritdoc/>
        public Wide(Random rand) : base(rand)
        {
        }

        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            r.Next();

            var array = new object[980];
            array[0] = Interlocked.Increment(ref autonum);
            array[1] = p.CHI;

            for (var i = 2; i < array.Length; i++)
            {
                array[i] = i switch
                {
                    < 100 => r.Next(),
                    < 200 => r.NextDouble(),
                    < 300 => GetRandomSentence(r),
                    < 400 => GetRandomGPCode(r),
                    _ => GetGaussianInt(50, 50000)
                };
            }

            return array;
        }

        /// <inheritdoc/>
        protected override string[] GetHeaders()
        {
            var array = new string[980];
            array[0] = "id";
            array[1] = "chi";
            
            for (var i = 2; i < array.Length; i++)
            {
                array[i] = $"col{i}";
            }

            return array;
        }
    }
}

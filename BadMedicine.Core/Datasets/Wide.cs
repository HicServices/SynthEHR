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

            for (int i = 2; i < array.Length; i++)
            {
                if(i<100)
                    array[i] = r.Next();
                else
                if (i < 200)
                    array[i] = r.NextDouble();
                else
                if (i < 300)
                    array[i] = GetRandomSentence(r);
                else
                if (i < 400)
                    array[i] = GetRandomGPCode(r);
                else
                    array[i] = GetGaussianInt(50,50000);
            }

            return array;
        }

        /// <inheritdoc/>
        protected override string[] GetHeaders()
        {
            var array = new string[980];
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

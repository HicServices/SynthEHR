using System;
using System.IO;

namespace BadMedicine.Datasets
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/Births'/>
    public class Births : DataGenerator
    {
        const int MinAge = 18;
        const int MaxAge = 55;

        /// <inheritdoc/>
        public Births(Random rand) : base(rand)
        {
        }

        /// <inheritdoc/>
        public override bool IsEligible(Person p)
        {
            // if died must have lived for at least 18 years
            if(p.DateOfDeath.HasValue)
                return p.DateOfDeath.Value.Subtract(p.DateOfBirth) > TimeSpan.FromDays(MinAge * 366); // lets round up for leap years

            //if alive must be older than minimum age to give birth
            return p.DateOfBirth < DateTime.Now.AddYears(MinAge);

        }

        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            object[] results = new object[8];
            
            results[0] = p.CHI;
            results[1] = r.Next(2) == 0 ? 'T': 'F';
            
            var youngest = p.DateOfBirth.AddYears(MinAge);
            var oldest =  p.DateOfDeath ?? p.DateOfBirth.AddYears(55);
            
            // No future dates
            oldest = oldest > DateTime.Now ? DateTime.Now : oldest;

            // If they died younger than 18 or were born less than 18 years into the past
            var birthDate = youngest > oldest ? youngest : GetRandomDate(youngest,oldest,r);
            results[2] = birthDate;

            // Partner CHI
            results[3] = p.GetRandomCHI(r);

            // One in 30 are twins
            if(r.Next(30) == 0)
                results[4] = p.GetRandomCHI(r);

            // One in 1000 are triplets ( 1/30 * 1/34)
            if(results[4] != null && r.Next(34) == 0)
                results[5] = p.GetRandomCHI(r);

            return results;
        }

        /// <inheritdoc/>
        protected override string[] GetHeaders()
        {
            return new string[]
            {
                "MotherCHI",                        //0
                "Healthboard",                      //1
                "Date",                             //2
                "PartnerCHI",                       //3
                "BabyCHI1",                         //4
                "BabyCHI2",                         //5
                "BabyCHI3",                         //6
            };
        }
    }
}
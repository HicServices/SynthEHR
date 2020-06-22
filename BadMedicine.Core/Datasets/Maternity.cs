using System;
using System.IO;

namespace BadMedicine.Datasets
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity'/>
    public class Maternity : DataGenerator
    {

        /// <inheritdoc/>
        public Maternity(Random rand) : base(rand)
        {
        }

        /// <summary>
        /// Returns true if the person is Female and lived to be older than <see cref="MaternityRecord.MinAge"/> (e.g. 18).  Considers current DateTime and <see cref="Person.DateOfDeath"/>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool IsEligible(Person p)
        {
            if( p.Gender != 'F')
                return false;

            // if died must have lived for at least 18 years
            if(p.DateOfDeath.HasValue)
                return p.DateOfDeath.Value.Subtract(p.DateOfBirth) > TimeSpan.FromDays(MaternityRecord.MinAge * 366); // lets round up for leap years

            //if alive must be older than minimum age to give birth
            return p.DateOfBirth < Now.AddYears(MaternityRecord.MinAge);
        }

        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            var record = new MaternityRecord(p,r);

            object[] results = new object[11];
            
            results[0] = p.CHI;
            results[1] = r.Next(2) == 0 ? 'T': 'F';
            
            results[2] = record.Date;

            // Partner CHI
            results[3] = p.GetRandomCHI(r);

            // One in 30 are twins
            if(r.Next(30) == 0)
                results[4] = p.GetRandomCHI(r);

            // One in 1000 are triplets ( 1/30 * 1/34)
            if(results[4] != null && r.Next(34) == 0)
                results[5] = p.GetRandomCHI(r);

            results[7] = record.SendingLocation;
            results[8] = Guid.NewGuid().ToString();
            results[9] = record.Location;
            results[10] = record.MaritalStatus;

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
                "SendingLocation",                  //7
                "EpisodeRecordKey",                 //8
                "Location",                         //9
                "MaritalStatus",                    //10

            };
        }
    }
}
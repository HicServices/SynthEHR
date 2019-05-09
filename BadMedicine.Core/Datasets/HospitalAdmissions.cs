using System;
using System.IO;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Generates synthetic random data that is representative of patient hospital admissions data
    /// </summary>
    public class HospitalAdmissions:DataGenerator
    {
        /// <inheritdoc/>
        public HospitalAdmissions(Random rand) : base(rand)
        {
        }

        public override object[] GenerateTestDataRow(Person p)
        {
            var episode = new HospitalAdmissionsRecord(p,p.DateOfBirth, r);

            return new object[]
            {
                episode.Person.CHI,
                episode.Person.DateOfBirth,
                episode.AdmissionDate,
                episode.DischargeDate,
                episode.Condition1,
                episode.Condition2,
                episode.Condition3,
                episode.Condition4,
                GetRandomSentence(r)
            };
        }

        protected override string[] GetHeaders()
        {
            return new string[]
            {
                "chi",
                "dob",
                "admission_date",
                "discharge_date",
                "Condition1",
                "Condition2",
                "Condition3",
                "Condition4",
                "Comment"
            };
        }
    }
}

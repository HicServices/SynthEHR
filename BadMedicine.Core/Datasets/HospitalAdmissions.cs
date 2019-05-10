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

        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            var episode = new HospitalAdmissionsRecord(p,p.DateOfBirth, r);

            return new object[]
            {
                episode.Person.CHI,
                episode.Person.DateOfBirth,
                episode.AdmissionDate,
                episode.DischargeDate,
                episode.MainCondition,
                episode.OtherCondition1,
                episode.OtherCondition2,
                episode.OtherCondition3,
                GetRandomSentence(r)
            };
        }

        /// <inheritdoc/>
        protected override string[] GetHeaders()
        {
            return new string[]
            {
                "chi",
                "DateOfBirth",
                "AdmissionDate",
                "DischargeDate",
                "MainCondition",
                "OtherCondition1",
                "OtherCondition2",
                "OtherCondition3",
                "Comment"
            };
        }
    }
}

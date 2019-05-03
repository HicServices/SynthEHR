using System.IO;

namespace BadMedicine.TestData.Exercises
{
    /// <summary>
    /// Generates synthetic random data that is representative of patient hospital admissions data
    /// </summary>
    public class HospitalAdmissionsExerciseTestData:ExerciseTestDataGenerator
    {
        public override object[] GenerateTestDataRow(TestPerson p)
        {
            var episode = new TestAdmission(p,p.DateOfBirth, r);

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

        protected override void WriteHeaders(StreamWriter sw)
        {
            string[] headers =
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

            sw.WriteLine(string.Join(",",headers));
        }
    }
}

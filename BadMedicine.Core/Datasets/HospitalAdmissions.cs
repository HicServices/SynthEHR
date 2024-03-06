using System;

namespace BadMedicine.Datasets;

/// <summary>
/// Generates synthetic random data that is representative of patient hospital admissions data
/// </summary>
/// <inheritdoc/>
public class HospitalAdmissions(Random rand) : DataGenerator(rand)
{

    /// <inheritdoc/>
    public override object[] GenerateTestDataRow(Person p)
    {
            var episode = new HospitalAdmissionsRecord(p,p.DateOfBirth, r);

            return
            [
                episode.Person.CHI,
                episode.Person.DateOfBirth,
                episode.AdmissionDate,
                episode.DischargeDate,
                episode.MainCondition,
                episode.OtherCondition1,
                episode.OtherCondition2,
                episode.OtherCondition3,
                GetRandomSentence(r),
                episode.MainOperation,
                episode.MainOperationB,
                episode.OtherOperation1,
                episode.OtherOperation1B,
                episode.OtherOperation2,
                episode.OtherOperation2B,
                episode.OtherOperation3,
                episode.OtherOperation3B
            ];
        }

    /// <inheritdoc/>
    protected override string[] GetHeaders() =>
    [
        "chi",
        "DateOfBirth",
        "AdmissionDate",
        "DischargeDate",
        "MainCondition",
        "OtherCondition1",
        "OtherCondition2",
        "OtherCondition3",
        "Comment",
        "MainOperation",
        "MainOperationB",
        "OtherOperation1",
        "OtherOperation1B",
        "OtherOperation2",
        "OtherOperation2B",
        "OtherOperation3",
        "OtherOperation3B"
    ];
}
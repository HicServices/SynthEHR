using System;

namespace BadMedicine.Datasets;

/// <include file='../../Datasets.doc.xml' path='Datasets/Maternity'/>
/// <inheritdoc/>
public sealed class Maternity(Random rand) : DataGenerator(rand)
{

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

            var results = new object[12];

            results[0] = p.CHI;
            results[1] = r.Next(2) == 0 ? 'T': 'F';

            results[2] = record.Date;

            // Partner CHI
            results[3] = new Person(r).GetRandomCHI(r);

            // Baby CHIs
            results[4] = record.BabyChi[0];
            results[5] = record.BabyChi[1];
            results[6] = record.BabyChi[2];

            results[7] = record.SendingLocation;
            results[8] = Guid.NewGuid().ToString();
            results[9] = record.Location;
            results[10] = record.MaritalStatus;
            results[11] = record.Specialty;

            return results;
        }

    /// <inheritdoc/>
    protected override string[] GetHeaders() =>
    [
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
        "Specialty"                        //11

    ];
}
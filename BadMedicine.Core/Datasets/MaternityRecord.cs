using System;
using System.Data;

namespace BadMedicine.Datasets;

/// <summary>
/// Describes a single maternity event for a specific <see cref="Person"/>
/// </summary>
public sealed class MaternityRecord
{
    /// <summary>
    /// Youngest age of mother to generate
    /// </summary>
    public const int MinAge = 18;

    /// <summary>
    /// Oldest age of mother to generate
    /// </summary>
    public const int MaxAge = 55;

    private static readonly BucketList<string> _locations = [];
    private static readonly BucketList<string> _maritalStatusOld = [];
    private static readonly BucketList<string> _maritalStatusNew = [];
    private static readonly BucketList<string> _specialties = [];

    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="Location"]'/>
    public string Location { get; set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="SendingLocation"]'/>
    public string SendingLocation { get; set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="Date"]'/>
    public DateTime Date { get; set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="MaritalStatus"]'/>
    public object MaritalStatus { get; set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="Specialty"]'/>
    public string Specialty { get; internal set; }

    /// <summary>
    /// The person on whom the maternity action is performed
    /// </summary>
    public Person Person { get; set; }

    /// <summary>
    /// Chi numbers of up to 3 babies involved.  Always contains 3 elements with nulls e.g. if twins then first 2 elements are populated and third is null.
    /// </summary>
    public string[] BabyChi { get; } = new string[3];


    /// <summary>
    /// The date at which the data collector stopped using numeric marital status codes (in favour of alphabetical)
    /// </summary>
    private static readonly DateTime MaritalStatusSwitchover = new(2001, 1, 1);

    /// <summary>
    /// Generates a new random biochemistry test.
    /// </summary>
    /// <param name="p">The person who is undergoing maternity activity.  Should be Female and of a sufficient age that the operation could have taken place during their lifetime (see <see cref="Maternity.IsEligible(BadMedicine.Person)"/></param>
    /// <param name="r"></param>
    public MaternityRecord(Person p, Random r)
    {
        Person = p;

        var youngest = p.DateOfBirth.AddYears(MinAge);
        var oldest = p.DateOfDeath ?? p.DateOfBirth.AddYears(MaxAge);

        // No future dates
        oldest = oldest > DataGenerator.Now ? DataGenerator.Now : oldest;

        // If they died younger than 18 or were born less than 18 years into the past
        Date = youngest > oldest ? oldest : DataGenerator.GetRandomDate(youngest, oldest, r);

        Location = _locations.GetRandom(r);
        SendingLocation = _locations.GetRandom(r);
        MaritalStatus = Date < MaritalStatusSwitchover ? _maritalStatusOld.GetRandom(r) : _maritalStatusNew.GetRandom(r);

        BabyChi[0] = new Person(r) { DateOfBirth = Date }.GetRandomCHI(r);

        // One in 30 are twins
        if (r.Next(30) == 0)
        {
            BabyChi[1] = new Person(r) { DateOfBirth = Date }.GetRandomCHI(r);

            // One in 1000 are triplets (1/30 * 1/34)
            if (r.Next(34) == 0)
                BabyChi[2] = new Person(r) { DateOfBirth = Date }.GetRandomCHI(r);
        }

        Specialty = _specialties.GetRandom(r);
    }

    static MaternityRecord()
    {
        using var dt = new DataTable();
        dt.BeginLoadData();
        DataGenerator.EmbeddedCsvToDataTable(typeof(Maternity), "Maternity.csv", dt);

        foreach (DataRow row in dt.Rows)
        {
            AddRow(row, "Location", _locations);
            AddRow(row, "MaritalStatusNumeric", _maritalStatusOld);
            AddRow(row, "MaritalStatusAlpha", _maritalStatusNew);
            AddRow(row, "Specialty", _specialties);
        }
    }

    private static void AddRow(DataRow row, string key, BucketList<string> bucketList)
    {
        var val = Convert.ToString(row[key]);
        var freq = row[$"{key}_RecordCount"];

        if (string.IsNullOrWhiteSpace(freq.ToString()))
            return;

        bucketList.Add(Convert.ToInt32(freq), val);
    }
}
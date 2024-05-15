using System;
using SynthEHR;
using SynthEHR.Datasets;
using NUnit.Framework;

namespace SynthEHRTests;

internal sealed class TestBiochemistry
{
    [Test]
    public void Test_CreateABiochemistryRecord()
    {
        // Originally a seed of 500 found this second most common lab result; the sort optimisation means a seed of 504 finds the same but with a different LabNumber.
        var record = new BiochemistryRecord(new Random(504));

        Assert.Multiple(() =>
        {
            Assert.That(record.ArithmeticComparator, Is.EqualTo("NULL"));
            Assert.That(record.Healthboard, Is.EqualTo("T"));
            Assert.That(record.Interpretation, Is.EqualTo("IF HIGH RISK- EXCLUDE CKD1/2. CHECK FOR HAEMATURIA AND PROTEINURIA"));
            Assert.That(record.LabNumber, Is.EqualTo("BC57969"));
            Assert.That(record.QuantityUnit, Is.EqualTo("NULL"));
            Assert.That(record.ReadCodeValue, Is.EqualTo("451G."));
            Assert.That(record.SampleType, Is.EqualTo("Blood"));
            Assert.That(record.TestCode, Is.EqualTo("eCOM"));
            Assert.That(record.QuantityUnit, Is.EqualTo("NULL"));
        });
    }
    [Test]
    public void TestSeed_Biochemistry_SingleRow()
    {
        var record1 = new BiochemistryRecord(new Random(500));
        var record2 = new BiochemistryRecord(new Random(500));

        Assert.Multiple(() =>
        {
            Assert.That(record1.ArithmeticComparator, Is.EqualTo(record2.ArithmeticComparator));
            Assert.That(record1.LabNumber, Is.EqualTo(record2.LabNumber));
            Assert.That(record1.TestCode, Is.EqualTo(record2.TestCode));
        });
    }
    [Test]
    public void TestSeed_Biochemistry()
    {
        var r1 = new Random(500);
        var r2 = new Random(500);
        var r3 = new Random(500);


        var persons1 = new PersonCollection();
        var persons2 = new PersonCollection();
        var persons3 = new PersonCollection();

        //take one from each random number generator
        persons1.GeneratePeople(500,r1);
        persons2.GeneratePeople(500,r2);
        persons3.GeneratePeople(500,r3);

        var generator1 = DataGeneratorFactory.Create<Biochemistry>(r1);
        var generator2 = DataGeneratorFactory.Create<Biochemistry>(r2);
        var generator3 = new Biochemistry(r3);


        for(var i = 0 ; i < 500;i++)
        {
            var row1 = generator1.GenerateTestDataRow(persons1.People[r1.Next(persons1.People.Length)]);
            var row2 = generator2.GenerateTestDataRow(persons2.People[r2.Next(persons2.People.Length)]);
            var row3 = generator3.GenerateTestDataRow(persons3.People[r3.Next(persons3.People.Length)]);

            for(var cell = 0;cell < row1.Length;cell++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(row2[cell], Is.EqualTo(row1[cell]));
                    Assert.That(row3[cell], Is.EqualTo(row2[cell]));
                });
            }
        }
    }
}
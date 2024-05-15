using SynthEHR;
using SynthEHR.Datasets;
using NUnit.Framework;
using System;
using System.Data;
using System.Linq;

namespace SynthEHRTests;

internal sealed class MaternityTests
{
    [Test]
    public void Test_IsEligible()
    {
        var r = new Random(100);
        var pc = new PersonCollection();
        pc.GeneratePeople(100,r);

        var m = new Maternity(r);

        Assert.Multiple(() =>
        {
            Assert.That(pc.People.Where(m.IsEligible).All(static e => e.Gender == 'F'), Is.True);
            Assert.That(pc.People.Count(m.IsEligible), Is.LessThanOrEqualTo(47), "Expected less than 50:50 due to restrictions on both Gender and age");
        });
    }

    [Test]
    public void Test_GetDataTable()
    {
        var r = new Random(100);
        var m = new Maternity(r);

        var pc = new PersonCollection();
        pc.GeneratePeople(100,r);

        //get 50k records
        var dt = m.GetDataTable(pc,50000);
        Assert.That(dt.Rows, Has.Count.EqualTo(50000));

        var countFromPopularLocation = dt.Rows.Cast<DataRow>().Count(static row => row["SendingLocation"].Equals("T101H"));
        var countFromRareLocation = dt.Rows.Cast<DataRow>().Count(static row => row["SendingLocation"].Equals("T306H"));

        Assert.Multiple(() =>
        {
            Assert.That(countFromPopularLocation, Is.GreaterThan(0));
            Assert.That(countFromRareLocation, Is.GreaterThan(0));
        });

        // should be more from popular location
        Assert.That(countFromPopularLocation, Is.GreaterThan(countFromRareLocation));

        // like a lot more!
        Assert.That(countFromPopularLocation, Is.GreaterThan(countFromRareLocation * 10));
    }

    [Test]
    public void Test_MaternityRecord()
    {
        var r = new Random(100);
        var m = new Maternity(r);

        var pc = new PersonCollection();
        pc.GeneratePeople(100,r);

        var eligible = pc.People.Where(m.IsEligible);
        var dict = eligible.ToDictionary(static k=>k,v=>new MaternityRecord(v,r));

        Assert.Multiple(() =>
        {
            //gave birth after being born themselves
            Assert.That(dict.All(static kv => kv.Key.DateOfBirth < kv.Value.Date), Is.True);

            //no future dates
            Assert.That(dict.All(static kv => kv.Value.Date <= DataGenerator.Now), Is.True);
        });



    }

}
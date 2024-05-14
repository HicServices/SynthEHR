using System;
using SynthEHR;
using SynthEHR.Datasets;
using NUnit.Framework;

namespace SynthEHRTests;

internal sealed class WideTests
{
    [Test]
    public void TestWide()
    {
        var r1 = new Random(500);

        var persons = new PersonCollection();
        persons.GeneratePeople(500, r1);

        var generator = DataGeneratorFactory.Create<Wide>(r1);

        using var dt = generator.GetDataTable(persons, 500);
        Assert.Multiple(() =>
        {
            Assert.That(dt.Columns, Has.Count.EqualTo(980));
            Assert.That(dt.Rows, Has.Count.EqualTo(500));
        });
    }
}
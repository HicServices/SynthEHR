using System;
using System.Linq;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests;

internal sealed class TestGetDataTable
{
    [Test]
    public void Test_GetDataTableForAllDatasets_ReturnsCorrectRowCount()
    {
        //there should be some supported datasets
        Assert.That(DataGeneratorFactory.GetAvailableGenerators().Count(), Is.GreaterThan(0));

        var r = new Random(500);

        var people = new PersonCollection();
        people.GeneratePeople(5000,r);

        //each dataset
        foreach (var dt in DataGeneratorFactory.GetAvailableGenerators().Select(t => DataGeneratorFactory.Create(t.Type, r))
                     .Select(instance => instance.GetDataTable(people, 500)))
        {
            Assert.That(dt.Rows, Has.Count.EqualTo(500));
        }
    }
}
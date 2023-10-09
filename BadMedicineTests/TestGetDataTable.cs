using System;
using System.Linq;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests;

class TestGetDataTable
{
    [Test]
    public void Test_GetDataTableForAllDatasets_ReturnsCorrectRowCount()
    {
        var factory = new DataGeneratorFactory();

        //there should be some supported datasets
        Assert.Greater(factory.GetAvailableGenerators().Count(),0);

        var r = new Random(500);

        var people = new PersonCollection();
        people.GeneratePeople(5000,r);

        //each dataset
        foreach (var t in factory.GetAvailableGenerators())
        {
            try
            {
                var instance = factory.Create(t, r);
                var dt = instance.GetDataTable(people,500);
                Assert.AreEqual(500,dt.Rows.Count);
            }
            catch (Exception e)
            {
                throw new Exception($"Test failed for Type '{t.Name}'",e);

            }
        }
    }
}
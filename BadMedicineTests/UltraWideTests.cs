using System;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests;

class UltraWideTests
{
    [Test]
    public void TestUltraWide()
    {
        var r1 = new Random(500);

        var persons = new PersonCollection();
        persons.GeneratePeople(500, r1);

        var f = new DataGeneratorFactory();
        var generator = f.Create<UltraWide>(r1);

        using var dt = generator.GetDataTable(persons, 500);
        Assert.AreEqual(20000, dt.Columns.Count);
        Assert.AreEqual(500, dt.Rows.Count);
    }
}
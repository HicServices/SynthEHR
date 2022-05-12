using System;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests
{
    class WideTests
    {
        [Test]
        public void TestWide()
        {
            var r1 = new Random(500);

            PersonCollection persons = new PersonCollection();
            persons.GeneratePeople(500, r1);

            var f = new DataGeneratorFactory();
            var generator = f.Create<Wide>(r1);

            using var dt = generator.GetDataTable(persons, 500);
            Assert.AreEqual(980,dt.Columns.Count);
            Assert.AreEqual(500, dt.Rows.Count);
        }
    }
}

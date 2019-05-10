using System;
using System.Collections.Generic;
using System.Text;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests
{
    class TestBiochemistry
    {
        [Test]
        public void Test_CreateABiochemistryRecord()
        {
            var record = new BiochemistryRecord(new Random(500)); 

            Assert.AreEqual("NULL",record.ArithmeticComparator);
            Assert.AreEqual("T",record.Healthboard);
            Assert.AreEqual("IF HIGH RISK- EXCLUDE CKD1/2. CHECK FOR HAEMATURIA AND PROTEINURIA",record.Interpretation);
            Assert.AreEqual("BC262017",record.LabNumber);
            Assert.AreEqual("NULL",record.QuantityUnit);
            Assert.AreEqual("451G.",record.ReadCodeValue);
            Assert.AreEqual("Blood",record.SampleType);
            Assert.AreEqual("eCOM",record.TestCode);
            Assert.AreEqual("NULL",record.QuantityUnit);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BadMedicine;
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
        [Test]
        public void TestSeed_Biochemistry_SingleRow()
        {
            var record1 = new BiochemistryRecord(new Random(500)); 
            var record2 = new BiochemistryRecord(new Random(500)); 

            Assert.AreEqual(record2.ArithmeticComparator,record1.ArithmeticComparator);
            Assert.AreEqual(record2.LabNumber,record1.LabNumber);
            Assert.AreEqual(record2.TestCode,record1.TestCode);
        }
        [Test]
        public void TestSeed_Biochemistry()
        {
            var r1 = new Random(500);
            var r2 = new Random(500);
            var r3 = new Random(500);


            PersonCollection persons1 = new PersonCollection();
            PersonCollection persons2 = new PersonCollection();
            PersonCollection persons3 = new PersonCollection();

            //take one from each random number generator
            persons1.GeneratePeople(500,r1);
            persons2.GeneratePeople(500,r2);
            persons3.GeneratePeople(500,r3);
            
            var f = new DataGeneratorFactory();
            var generator1 = f.Create<Biochemistry>(r1);
            var generator2 = f.Create<Biochemistry>(r2);
            var generator3 = new Biochemistry(r3);

            
            for(int i = 0 ; i < 500;i++)
            {
                var row1 = generator1.GenerateTestDataRow(persons1.People[r1.Next(persons1.People.Length)]);
                var row2 = generator2.GenerateTestDataRow(persons2.People[r2.Next(persons2.People.Length)]);
                var row3 = generator3.GenerateTestDataRow(persons3.People[r3.Next(persons3.People.Length)]);

                for(int cell = 0;cell < row1.Length;cell++)
                {
                    Assert.AreEqual(row1[cell],row2[cell]);
                    Assert.AreEqual(row2[cell],row3[cell]);
                }
            }
            
            


        }
    }
}

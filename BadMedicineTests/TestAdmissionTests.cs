using System;
using System.Diagnostics;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests.TestData
{
    public class TestAdmissionTests
    {
        [Test]
        public void Test_GetRandomIcdCode()
        {
            var r = new Random(100);

            var person = new Person(r);
            var a = new HospitalAdmissionsRecord(person,person.DateOfBirth,r);

            Assert.IsNotNull(a.Person.CHI);
            Assert.IsNotNull(a.Person.DateOfBirth);
            Assert.IsNotNull(a.Person.Address.Line1);
            Assert.IsNotNull(a.Person.Address.Postcode);
            Assert.IsNotNull(a.AdmissionDate);
            Assert.IsNotNull(a.DischargeDate);
            Assert.IsNotNull(a.Condition1);
            Assert.IsNotNull(a.Condition2);
            Assert.IsNotNull(a.Condition3);
            Assert.IsNull(a.Condition4);   
        }

        [Test]
        public void Test_Performance()
        {
            var r = new Random(100);
            var person = new Person(r);

            var swSetup = Stopwatch.StartNew();
            //first is always slow
            var first= new HospitalAdmissionsRecord(person,person.DateOfBirth,r);    

            Console.WriteLine("Setup took" + swSetup.ElapsedMilliseconds + "ms");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                var a = new HospitalAdmissionsRecord(person,person.DateOfBirth,r);    
            }

            Console.WriteLine("Remainder took:" + sw.ElapsedMilliseconds +"ms");
            
        }
        
    }
}

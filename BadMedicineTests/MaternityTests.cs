using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BadMedicineTests
{
    class MaternityTests
    {
        [Test]
        public void Test_IsEligible()
        {
            var r = new Random(100);
            var pc = new PersonCollection();
            pc.GeneratePeople(100,r);
            
            var m = new Maternity(r);
            
            Assert.IsTrue(pc.People.Where(m.IsEligible).All(e=>e.Gender == 'F'));                       
            Assert.LessOrEqual(pc.People.Count(m.IsEligible),47, "Expected less than 50:50 due to restrictions on both Gender and age");
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
            Assert.AreEqual(50000,dt.Rows.Count);

            int countFromPopularLocation = dt.Rows.Cast<DataRow>().Count(row => row["SendingLocation"].Equals("T101H"));
            int countFromRareLocation = dt.Rows.Cast<DataRow>().Count(row => row["SendingLocation"].Equals("T306H"));

            Assert.Greater(countFromPopularLocation,0);
            Assert.Greater(countFromRareLocation,0);

            // should be more from popular location
            Assert.Greater(countFromPopularLocation , countFromRareLocation);

            // like a lot more!
            Assert.Greater(countFromPopularLocation , countFromRareLocation * 10);
        }
    }
}

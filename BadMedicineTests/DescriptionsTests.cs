using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadMedicineTests
{
    class DescriptionsTests
    {
        [Test]
        public void Test_GetDescription_Dataset()
        {
            var desc = new Descriptions();

            StringAssert.Contains("Tayside and Fife labs biochemistry data",desc.Get<Biochemistry>());
        }

        [Test]
        public void Test_GetDescription_Field()
        {
            var desc = new Descriptions();

            StringAssert.Contains("Health Board code",desc.Get<Biochemistry>("Healthboard"));
            StringAssert.Contains("Health Board code",desc.Get("Biochemistry","Healthboard"));
        }

        [Test]
        public void Test_GetDescription_FieldNotFound()
        {
            var desc = new Descriptions();

            Assert.IsNull(desc.Get<Biochemistry>("ZappyZappyZappy"));
            Assert.IsNull(desc.Get("Biochemistry","ZappyZappyZappy"));
        }
        [Test]
        public void Test_GetDescription_FieldNotFoundAndDatasetNotFound()
        {
            var desc = new Descriptions();
            Assert.IsNull(desc.Get("Happy","ZappyZappyZappy"));

            Assert.IsEmpty(desc.GetAll("Happy"));
        }

        /// <summary>
        /// This tests when the field is in CommonFields not under the dataset
        /// </summary>
        [Test]
        public void Test_GetDescription_ChiField()
        {
            var desc = new Descriptions();
            StringAssert.Contains("Community Health Index (CHI) number is a unique personal identifier ",desc.Get<Biochemistry>("chi"));
            StringAssert.Contains("Community Health Index (CHI) number is a unique personal identifier ",desc.Get("Biochemistry","chi"));
        }

        [Test]
        public void Test_GetAllDescriptions_Biochemistry()
        {
            var desc = new Descriptions();

            Assert.GreaterOrEqual(desc.GetAll<Biochemistry>().ToArray().Length,5);
            Assert.GreaterOrEqual(desc.GetAll("Biochemistry").ToArray().Length,5);
            Assert.GreaterOrEqual(desc.GetAll("Common").ToArray().Length,1);


            foreach(var kvp in desc.GetAll<Biochemistry>())
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(kvp.Key), "Found null key");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(kvp.Value),"Found null description for" + kvp.Key);
            }
        }

        [Test]
        public void Test_GetAllDescriptions_AllDatasets()
        {
            var desc = new Descriptions();

            var factory = new DataGeneratorFactory();
            foreach(var g in factory.GetAvailableGenerators())
            {
                Assert.IsNotNull(desc.Get(g.Name),"Dataset '{0}' did not have a summary tag",g.Name);
                Assert.IsNotEmpty(desc.GetAll(g.Name),"Dataset '{0}' did not have at least one column description",g.Name);
            }
        }
    }
}

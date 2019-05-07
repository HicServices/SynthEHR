using System;
using System.Collections.Generic;
using System.Text;
using BadMedicine;
using NUnit.Framework;

namespace BadMedicineTests
{
    class BucketListTests
    {
        [Test]
        public void Test_BucketList_OneElment()
        {
            BucketList<string> list = new BucketList<string>(new Random(100));
            list.Add(1,"fish");

            Assert.AreEqual("fish",list.GetRandom());
            Assert.AreEqual("fish",list.GetRandom(new []{0}));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_BucketList_TwoElments(bool passIndices)
        {
            BucketList<string> list = new BucketList<string>(new Random(100));
            
            //we expect twice as many blue as red
            list.Add(1,"red");
            list.Add(2,"blue");

            int countRed = 0;
            int countBlue = 0;
            
            for (int i = 0; i < 1000; i++)
            {
                switch (passIndices ? list.GetRandom() : list.GetRandom(new[] {0, 1}))
                {
                    case "red": 
                        countRed++;
                        break;
                    case "blue":
                        countBlue++;
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }

            Assert.AreEqual(311,countRed);
            Assert.AreEqual(689,countBlue);

        }
    }
}

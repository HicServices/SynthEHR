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
            var r = new Random(100);

            BucketList<string> list = new BucketList<string>();
            list.Add(1,"fish");

            Assert.AreEqual("fish",list.GetRandom(r));
            Assert.AreEqual("fish",list.GetRandom(new []{0},r));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_BucketList_TwoElments(bool passIndices)
        {
            var r = new Random(100);
            BucketList<string> list = new BucketList<string>();
            
            //we expect twice as many blue as red
            list.Add(1,"red");
            list.Add(2,"blue");

            int countRed = 0;
            int countBlue = 0;
            
            for (int i = 0; i < 1000; i++)
            {
                switch (passIndices ? list.GetRandom(r) : list.GetRandom(new[] {0, 1},r))
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

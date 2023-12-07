using System;
using BadMedicine;
using NUnit.Framework;

namespace BadMedicineTests;

internal sealed class BucketListTests
{
    private static readonly int[] ZeroOne = [0, 1];
    private static readonly int[] Zero = [0];

    [Test]
    public void Test_BucketList_OneElement()
    {
        var r = new Random(100);

        var list = new BucketList<string>
        {
            { 1, "fish" }
        };

        Assert.Multiple(() =>
        {
            Assert.That(list.GetRandom(r), Is.EqualTo("fish"));
            Assert.That(list.GetRandom(Zero, r), Is.EqualTo("fish"));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Test_BucketList_TwoElements(bool passIndices)
    {
        var r = new Random(100);
        var list = new BucketList<string>
        {
            //we expect twice as many blue as red
            { 1, "red" },
            { 2, "blue" }
        };

        var countRed = 0;
        var countBlue = 0;

        for (var i = 0; i < 1000; i++)
        {
            switch (passIndices ? list.GetRandom(r) : list.GetRandom(ZeroOne,r))
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

        Assert.Multiple(() =>
        {
            Assert.That(countRed, Is.EqualTo(311));
            Assert.That(countBlue, Is.EqualTo(689));
        });

    }
}
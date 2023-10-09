using BadMedicine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BadMedicineTests;

class PersonTests
{
    [Test]
    public void TestPerson_Seed()
    {
        var r1 = new Random(500);
        var r2 = new Random(500);

        var p1 = new Person(r1);
        var p2 = new Person(r2);

        Assert.AreEqual(p1.Forename,p2.Forename);
        Assert.AreEqual(p1.Surname,p2.Surname);
        Assert.AreEqual(p1.DateOfBirth,p2.DateOfBirth);
        Assert.AreEqual(p1.DateOfDeath,p2.DateOfDeath);
        Assert.AreEqual(p1.CHI,p2.CHI);
        Assert.AreEqual(p1.PreviousAddress?.Line1,p2.PreviousAddress?.Line1);
        Assert.AreEqual(p1.PreviousAddress?.Line2,p2.PreviousAddress?.Line2);
        Assert.AreEqual(p1.PreviousAddress?.Line3,p2.PreviousAddress?.Line3);
        Assert.AreEqual(p1.PreviousAddress?.Line4,p2.PreviousAddress?.Line4);
        Assert.AreEqual(p1.PreviousAddress?.Postcode,p2.PreviousAddress?.Postcode);


        Assert.AreEqual(p1.Address.Line1,p2.Address.Line1);
        Assert.AreEqual(p1.Address.Line2,p2.Address.Line2);
        Assert.AreEqual(p1.Address.Line3,p2.Address.Line3);
        Assert.AreEqual(p1.Address.Line4,p2.Address.Line4);
        Assert.AreEqual(p1.Address.Postcode,p2.Address.Postcode);
    }

    [Test]
    public void TestPersonCollection_Seed()
    {
        var r1 = new Random(500);
        var r2 = new Random(500);

        var p1 = new Person(r1);
        var p2 = new Person(r2);

        Assert.AreEqual(p1.Forename,p2.Forename);
        Assert.AreEqual(p1.Surname,p2.Surname);
        Assert.AreEqual(p1.DateOfBirth,p2.DateOfBirth);
        Assert.AreEqual(p1.DateOfDeath,p2.DateOfDeath);
        Assert.AreEqual(p1.CHI,p2.CHI);
        Assert.AreEqual(p1.PreviousAddress?.Line1,p2.PreviousAddress?.Line1);
        Assert.AreEqual(p1.PreviousAddress?.Line2,p2.PreviousAddress?.Line2);
        Assert.AreEqual(p1.PreviousAddress?.Line3,p2.PreviousAddress?.Line3);
        Assert.AreEqual(p1.PreviousAddress?.Line4,p2.PreviousAddress?.Line4);
        Assert.AreEqual(p1.PreviousAddress?.Postcode,p2.PreviousAddress?.Postcode);


        Assert.AreEqual(p1.Address.Line1,p2.Address.Line1);
        Assert.AreEqual(p1.Address.Line2,p2.Address.Line2);
        Assert.AreEqual(p1.Address.Line3,p2.Address.Line3);
        Assert.AreEqual(p1.Address.Line4,p2.Address.Line4);
        Assert.AreEqual(p1.Address.Postcode,p2.Address.Postcode);
    }
}
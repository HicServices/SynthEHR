using SynthEHR;
using NUnit.Framework;
using System;

namespace SynthEHRTests;

internal sealed class PersonTests
{
    [Test]
    public void TestPerson_Seed()
    {
        var r1 = new Random(500);
        var r2 = new Random(500);

        var p1 = new Person(r1);
        var p2 = new Person(r2);

        Assert.Multiple(() =>
        {
            Assert.That(p2.Forename, Is.EqualTo(p1.Forename));
            Assert.That(p2.Surname, Is.EqualTo(p1.Surname));
            Assert.That(p2.DateOfBirth, Is.EqualTo(p1.DateOfBirth));
            Assert.That(p2.DateOfDeath, Is.EqualTo(p1.DateOfDeath));
            Assert.That(p2.CHI, Is.EqualTo(p1.CHI));
            Assert.That(p2.PreviousAddress?.Line1, Is.EqualTo(p1.PreviousAddress?.Line1));
            Assert.That(p2.PreviousAddress?.Line2, Is.EqualTo(p1.PreviousAddress?.Line2));
            Assert.That(p2.PreviousAddress?.Line3, Is.EqualTo(p1.PreviousAddress?.Line3));
            Assert.That(p2.PreviousAddress?.Line4, Is.EqualTo(p1.PreviousAddress?.Line4));
            Assert.That(p2.PreviousAddress?.Postcode, Is.EqualTo(p1.PreviousAddress?.Postcode));


            Assert.That(p2.Address.Line1, Is.EqualTo(p1.Address.Line1));
            Assert.That(p2.Address.Line2, Is.EqualTo(p1.Address.Line2));
            Assert.That(p2.Address.Line3, Is.EqualTo(p1.Address.Line3));
            Assert.That(p2.Address.Line4, Is.EqualTo(p1.Address.Line4));
            Assert.That(p2.Address.Postcode, Is.EqualTo(p1.Address.Postcode));
        });
    }

    [Test]
    public void TestPersonCollection_Seed()
    {
        var r1 = new Random(500);
        var r2 = new Random(500);

        var p1 = new Person(r1);
        var p2 = new Person(r2);

        Assert.Multiple(() =>
        {
            Assert.That(p2.Forename, Is.EqualTo(p1.Forename));
            Assert.That(p2.Surname, Is.EqualTo(p1.Surname));
            Assert.That(p2.DateOfBirth, Is.EqualTo(p1.DateOfBirth));
            Assert.That(p2.DateOfDeath, Is.EqualTo(p1.DateOfDeath));
            Assert.That(p2.CHI, Is.EqualTo(p1.CHI));
            Assert.That(p2.PreviousAddress?.Line1, Is.EqualTo(p1.PreviousAddress?.Line1));
            Assert.That(p2.PreviousAddress?.Line2, Is.EqualTo(p1.PreviousAddress?.Line2));
            Assert.That(p2.PreviousAddress?.Line3, Is.EqualTo(p1.PreviousAddress?.Line3));
            Assert.That(p2.PreviousAddress?.Line4, Is.EqualTo(p1.PreviousAddress?.Line4));
            Assert.That(p2.PreviousAddress?.Postcode, Is.EqualTo(p1.PreviousAddress?.Postcode));


            Assert.That(p2.Address.Line1, Is.EqualTo(p1.Address.Line1));
            Assert.That(p2.Address.Line2, Is.EqualTo(p1.Address.Line2));
            Assert.That(p2.Address.Line3, Is.EqualTo(p1.Address.Line3));
            Assert.That(p2.Address.Line4, Is.EqualTo(p1.Address.Line4));
            Assert.That(p2.Address.Postcode, Is.EqualTo(p1.Address.Postcode));
        });
    }
}
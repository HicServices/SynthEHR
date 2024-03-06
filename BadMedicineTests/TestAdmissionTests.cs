using System;
using System.Diagnostics;
using BadMedicine;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests;

public sealed class TestAdmissionTests
{
    [Test]
    public void Test_GetRandomIcdCode()
    {
        //Seed the random generator if you want to always produce the same randomisation
        var r = new Random(100);

        //Create a new person
        var person = new Person(r);

        //Create test data for that person
        var a = new HospitalAdmissionsRecord(person,person.DateOfBirth,r);

        Assert.Multiple(() =>
        {
            Assert.That(a.Person.CHI, Is.Not.Null);
            Assert.That(a.Person.Address.Line1, Is.Not.Null);
            Assert.That(a.Person.Address.Postcode, Is.Not.Null);
            Assert.That(a.MainCondition, Is.Not.Null);
        });
    }

    [Test]
    public void Test_Performance()
    {
        var r = new Random(100);
        var person = new Person(r);

        var swSetup = Stopwatch.StartNew();
        //first is always slow
        _= new HospitalAdmissionsRecord(person,person.DateOfBirth,r);

        Console.WriteLine($"Setup took{swSetup.ElapsedMilliseconds}ms");

        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 100000; i++)
        {
            _=new HospitalAdmissionsRecord(person,person.DateOfBirth,r);
        }

        Console.WriteLine($"Remainder took:{sw.ElapsedMilliseconds}ms");

    }

}
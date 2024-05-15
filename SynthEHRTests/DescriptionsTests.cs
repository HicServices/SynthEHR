using SynthEHR;
using SynthEHR.Datasets;
using NUnit.Framework;
using System.Linq;

namespace SynthEHRTests;

internal sealed class DescriptionsTests
{
    [Test]
    public void Test_GetDescription_Dataset()
    {
        Assert.That(Descriptions.Get<Biochemistry>(), Does.Contain("Tayside and Fife labs biochemistry data"));
    }

    [Test]
    public void Test_GetDescription_Field()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Descriptions.Get<Biochemistry>("Healthboard"), Does.Contain("Health Board code"));
            Assert.That(Descriptions.Get("Biochemistry", "Healthboard"), Does.Contain("Health Board code"));
        });
    }

    [Test]
    public void Test_GetDescription_FieldNotFound()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Descriptions.Get<Biochemistry>("ZappyZappyZappy"), Is.Null);
            Assert.That(Descriptions.Get("Biochemistry", "ZappyZappyZappy"), Is.Null);
        });
    }
    [Test]
    public void Test_GetDescription_FieldNotFoundAndDatasetNotFound()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Descriptions.Get("Happy", "ZappyZappyZappy"), Is.Null);

            Assert.That(Descriptions.GetAll("Happy"), Is.Empty);
        });
    }

    /// <summary>
    /// This tests when the field is in CommonFields not under the dataset
    /// </summary>
    [Test]
    public void Test_GetDescription_ChiField()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Descriptions.Get<Biochemistry>("chi"), Does.Contain("Community Health Index (CHI) number is a unique personal identifier "));
            Assert.That(Descriptions.Get("Biochemistry", "chi"), Does.Contain("Community Health Index (CHI) number is a unique personal identifier "));
        });
    }

    [Test]
    public void Test_GetAllDescriptions_Biochemistry()
    {
        Assert.Multiple(static () =>
        {
            Assert.That(Descriptions.GetAll<Biochemistry>().ToArray(), Has.Length.GreaterThanOrEqualTo(5));
            Assert.That(Descriptions.GetAll("Biochemistry").ToArray(), Has.Length.GreaterThanOrEqualTo(5));
            Assert.That(Descriptions.GetAll("Common").ToArray(), Is.Not.Empty);
        });


        foreach (var kvp in Descriptions.GetAll<Biochemistry>())
        {
            Assert.Multiple(() =>
            {
                Assert.That(!string.IsNullOrWhiteSpace(kvp.Key), Is.True, "Found null key");
                Assert.That(!string.IsNullOrWhiteSpace(kvp.Value), Is.True, $"Found null description for{kvp.Key}");
            });
        }
    }

    [Test]
    public void Test_GetAllDescriptions_AllDatasets()
    {
        foreach(var g in DataGeneratorFactory.GetAvailableGenerators().Select(static t=>t.Type))
        {
            Assert.Multiple(() =>
            {
                Assert.That(Descriptions.Get(g.Name), Is.Not.Null, $"Dataset '{g.Name}' did not have a summary tag");
                Assert.That($"Dataset '{g.Name}' did not have at least one column description", Is.Not.Empty);
            });
        }
    }
}
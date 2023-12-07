using System.Linq;
using BadMedicine.Datasets;
using NUnit.Framework;

namespace BadMedicineTests;

file sealed class DataGeneratorFactoryTests
{
    [Test]
    public void GeneratorList()
    {
        //find all generators in the assembly - only use reflection here, to keep BadMedicine.Core AOT-clean
        var fromReflection = typeof(IDataGenerator).Assembly.GetExportedTypes()
            .Where(static t => typeof(IDataGenerator).IsAssignableFrom(t)
                               && !t.IsAbstract
                               && t.IsClass).Select(static t=>new DataGeneratorFactory.GeneratorType(t));

        Assert.That(DataGeneratorFactory.GetAvailableGenerators(),Is.EquivalentTo(fromReflection));
    }
}
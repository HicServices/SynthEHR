using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BadMedicine.Datasets;

/// <summary>
/// Finds Types and Creates instances of <see cref="IDataGenerator"/> implementations
/// </summary>
public class DataGeneratorFactory
{
    /// <summary>
    /// Finds all concrete implementations of <see cref="IDataGenerator"/>.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetAvailableGenerators()
    {
        //find all generators in the assembly
        return typeof(IDataGenerator).Assembly.GetExportedTypes()
            .Where(static t => typeof(IDataGenerator).IsAssignableFrom(t)
                               && !t.IsAbstract
                               && t.IsClass);
    }

    /// <summary>
    /// Creates a new instance of the generic <see cref="IDataGenerator"/> Type initialized with the given seed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(Random seed) where T : IDataGenerator
    {
        return (T)Create(typeof(T), seed);
    }

    /// <summary>
    /// Creates a new instance of a <see cref="IDataGenerator"/> of Type <paramref name="type"/> initialized with the given seed
    /// </summary>
    /// <param name="type"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static IDataGenerator Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, Random seed)
    {
        return (IDataGenerator)Activator.CreateInstance(type, seed);
    }
}
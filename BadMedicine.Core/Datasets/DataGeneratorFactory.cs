using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Finds Types and Creates instances of <see cref="IDataGenerator"/> implementations
    /// </summary>
    public class DataGeneratorFactory
    {
        /// <summary>
        /// Finds all concrete implementations of <see cref="IDataGenerator"/> that are exported from the BadMedicine library (MEF).
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetAvailableGenerators()
        {
            //find all generators in the assembly
            return typeof(IDataGenerator).Assembly.GetExportedTypes()
                .Where(t => typeof(IDataGenerator).IsAssignableFrom(t)
                            && !t.IsAbstract
                            && t.IsClass);
        }

        /// <summary>
        /// Creates a new instance of the generic <see cref="IDataGenerator"/> Type initialized with the given seed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seed"></param>
        /// <returns></returns>
        public T Create<T>(Random seed) where T:IDataGenerator
        {
            return (T) Create(typeof(T), seed);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="IDataGenerator"/> of Type <paramref name="type"/> initialized with the given seed
        /// </summary>
        /// <param name="type"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public IDataGenerator Create(Type type, Random seed)
        {
            return (IDataGenerator) Activator.CreateInstance(type, seed);
        }
    }
}

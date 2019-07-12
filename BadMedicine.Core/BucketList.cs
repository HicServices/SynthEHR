using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadMedicine
{
    /// <summary>
    /// Picks random object of Type T based on a specified probability for each element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BucketList<T>
    {
        
        readonly List<T> _buckets = new List<T>();
        readonly List<int> _probabilities = new List<int>();

        private int? _total = null;

        /// <summary>
        /// Returns a random bucket (based on the probability of each bucket)
        /// </summary>
        /// <returns></returns>
        public T GetRandom(Random r)
        {
            //cache the total
            _total = _total??_probabilities.Sum();

            int toPick = r.Next(0, _total.Value);

            for (int i = 0; i < _probabilities.Count; i++)
            {
                toPick -= _probabilities[i];
                if (toPick < 0)
                    return _buckets[i];
            }

            throw new Exception("Could not GetRandom");
        }
        

        /// <summary>
        /// Returns a random bucket from the element indices provided (based on the probability of each bucket)
        /// </summary>
        /// <param name="usingOnlyIndices"></param>
        /// <returns></returns>
        public T GetRandom(IEnumerable<int> usingOnlyIndices, Random r)
        {
            var idx = usingOnlyIndices.ToList();

            int total = idx.Sum(t=>_probabilities[t]);
            
            int toPick = r.Next(0, total);

            foreach (int i in idx)
            {
                toPick -= _probabilities[i];
                if (toPick < 0)
                    return _buckets[i];
            }
            
            throw new Exception("Could not GetRandom");
        }

        

        /// <summary>
        /// Adds a new bucket to the list which will be returned using the total <paramref name="probability"/> ratio (relative
        /// to the other buckets).
        /// </summary>
        /// <param name="probability"></param>
        /// <param name="toAdd"></param>
        public void Add(int probability, T toAdd)
        {
            _probabilities.Add(probability);
            _buckets.Add(toAdd);
            _total = null;
        }
    }
}

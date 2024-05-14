using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SynthEHR;

/// <summary>
/// Picks random object of Type T based on a specified probability for each element.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class BucketList<T> : IEnumerable<(T item,int probability)>
{
    private Lazy<int> _total;
    private readonly List<(T item, int probability)> _list=[];

    /// <summary>
    /// Construct an empty BucketList
    /// </summary>
    public BucketList()
    {
        _total = new Lazy<int>(GetTotal,LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private int GetTotal() => _list.Sum(static t => t.probability);

    /// <summary>
    /// Returns a random bucket (based on the probability of each bucket)
    /// </summary>
    /// <returns></returns>
    public T GetRandom(Random r)
    {
        var toPick = r.Next(0, _total.Value);

        for (var i = 0; i < _list.Count; i++)
        {
            toPick -= _list[i].probability;
            if (toPick < 0)
                return _list[i].item;
        }

        throw new Exception("Could not GetRandom");
    }


    /// <summary>
    /// Returns a random bucket from the element indices provided (based on the probability of each bucket)
    /// </summary>
    /// <param name="usingOnlyIndices"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public T GetRandom(IEnumerable<int> usingOnlyIndices, Random r)
    {
        var idx = usingOnlyIndices.ToList();

        var total = idx.Sum(t => _list[t].probability);

        var toPick = r.Next(0, total);

        foreach (var i in idx)
        {
            toPick -= _list[i].probability;
            if (toPick < 0)
                return _list[i].item;
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
        _list.Add((toAdd,probability));
        if (_total.IsValueCreated)
            _total = new Lazy<int>(GetTotal, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <inheritdoc/>
    public IEnumerator<(T item, int probability)> GetEnumerator()
    {
        return ((IEnumerable<(T item, int probability)>)_list).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
}
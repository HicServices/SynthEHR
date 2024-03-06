using BadMedicine.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace BadMedicine;

/// <summary>
/// Provides descriptions for what is in each dataset column (at runtime).  This is the same information that appears in xmldocs.
/// </summary>
public static class Descriptions
{
    private static readonly XDocument Doc;
    private static readonly XNamespace Ns;

    /// <summary>
    /// Load the XML data at init
    /// </summary>
    static Descriptions()
    {
        using var stream = typeof(Descriptions).Assembly.GetManifestResourceStream("BadMedicine.Datasets.doc.xml") ??
                           throw new MissingManifestResourceException(
                               "BadMedicine.Datasets.doc.xml");
        Doc = XDocument.Load(stream);
        Ns = Doc.Root?.GetDefaultNamespace() ?? throw new MissingManifestResourceException("BadMedicine.Datasets.doc.xml not valid");
    }

    /// <summary>
    /// Returns all columns with descriptions in the dataset (does not include Common fields e.g. chi)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> GetAll<T>()
    {
        return GetAll(typeof(T).Name);
    }

    /// <summary>
    /// Returns all columns with descriptions in the dataset (does not include Common fields e.g. chi - unless you pass "Common" for <paramref name="dataset"/>)
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> GetAll(string dataset)
    {
        var ds = Doc.Root?.Element(Ns + dataset);

        if (ds == null)
            yield break;

        foreach (var x in ds.Elements(Ns + "Field"))
            yield return new KeyValuePair<string, string>(x.Attribute(Ns + "name")?.Value, x.Value);
    }


    /// <summary>
    /// Returns the description of the dataset (or null if no metadata exists)
    /// </summary>
    /// <param name="dataset">Dataset you want the description of</param>
    /// <returns></returns>
    public static string Get(string dataset)
    {
        return Doc.Root?.Element(Ns + dataset)?.Element(Ns + "summary")?.Value;
    }


    /// <summary>
    /// Returns the description of the dataset <typeparamref name="T"/> (or null if no metadata exists)
    /// </summary>
    /// <typeparam name="T">Dataset you want the description of</typeparam>
    /// <returns></returns>
    public static string Get<T>() where T : IDataGenerator
    {
        return Get(typeof(T).Name);
    }

    /// <summary>
    /// Returns the description of the column <paramref name="field"/> in dataset <typeparamref name="T"/> (or null if no metadata exists)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string Get<T>(string field) where T : IDataGenerator
    {
        return Get(typeof(T).Name, field);
    }

    /// <summary>
    /// Returns the description of the column <paramref name="field"/> in the <paramref name="dataset"/> (or null if no metadata exists)
    /// </summary>
    /// <param name="dataset"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string Get(string dataset, string field)
    {
        return
            //return from the dataset fields
            Doc.Root?.Element(Ns + dataset)?.Elements(Ns + "Field")

            //the one whose name attribute matches
            .SingleOrDefault(e => string.Equals(e.Attribute(Ns + "name")?.Value, field, StringComparison.CurrentCultureIgnoreCase))?.Value

            //or from Common if it's not in the dataset.
            ?? (dataset != "Common" ? Get("Common", field) : null); //null if we are already trying Common

    }
}
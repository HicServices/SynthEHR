using BadMedicine.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BadMedicine
{
    /// <summary>
    /// Provides descriptions for what is in each dataset column (at runtime).  This is the same information that appears in xmldocs.
    /// </summary>
    public class Descriptions
    {
        private XDocument _doc;
        private XNamespace _ns;

        /// <summary>
        /// Creates new instance ready to provide column descriptions from embedded resource file
        /// </summary>
        public Descriptions()
        {
            using(var stream = typeof(Descriptions).Assembly.GetManifestResourceStream("BadMedicine.Datasets.doc.xml"))
            {
                _doc = XDocument.Load(stream);
                _ns = _doc.Root.GetDefaultNamespace();
            }
        }

        /// <summary>
        /// Returns all columns with descriptions in the dataset (does not include Common fields e.g. chi)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string,string>> GetAll<T>()
        {
            return GetAll(typeof(T).Name);
        }

        /// <summary>
        /// Returns all columns with descriptions in the dataset (does not include Common fields e.g. chi - unless you pass "Common" for <paramref name="dataset"/>)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string,string>> GetAll(string dataset)
        {
            var ds = _doc.Root?.Element(_ns + dataset);

            if(ds == null)
                yield break;

            foreach(var x in ds.Elements(_ns + "Field"))
                yield return new KeyValuePair<string,string>(x.Attribute(_ns + "name").Value,x.Value);
        }


        /// <summary>
        /// Returns the description of the dataset (or null if no metadata exists)
        /// </summary>
        /// <param name="dataset">Dataset you want the description of</param>
        /// <returns></returns>
        public string Get(string dataset)
        {
            return _doc.Root?.Element(_ns + dataset)?.Element(_ns + "summary")?.Value;
        }


        /// <summary>
        /// Returns the description of the dataset <typeparamref name="T"/> (or null if no metadata exists)
        /// </summary>
        /// <typeparam name="T">Dataset you want the description of</typeparam>
        /// <returns></returns>
        public string Get<T>() where T:IDataGenerator
        {
            return Get(typeof(T).Name);
        }

        /// <summary>
        /// Returns the description of the column <paramref name="field"/> in dataset <typeparamref name="T"/> (or null if no metadata exists)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Get<T>(string field) where T:IDataGenerator
        {
            return Get(typeof(T).Name,field);
        }

        /// <summary>
        /// Returns the description of the column <paramref name="field"/> in the <paramref name="dataset"/> (or null if no metadata exists)
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Get(string dataset,string field)
        {
            return
                //return from the dataset fields
                _doc.Root.Element(_ns + dataset)?.Elements(_ns + "Field")?

                //the one whose name attribute matches
                .SingleOrDefault(e=>string.Equals(e.Attribute(_ns + "name")?.Value,field,StringComparison.CurrentCultureIgnoreCase))?.Value

                //or from Common if it's not in the dataset.
                ?? (dataset != "Common" ? Get("Common",field):null); //null if we are already trying Common

        }
    }
}

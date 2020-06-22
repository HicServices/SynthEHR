using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BadMedicine.Datasets
{
    class MaternityRecord
    {
        
        static object oLockInitialize = new object();
        private static bool initialized;
        private  static BucketList<string> _locations;

        
        /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="SendingLocation"]'/>
        public string SendingLocation {get;set;}

        /// <summary>
        /// Generates a new random biochemistry test.
        /// </summary>
        /// <param name="r"></param>
        public MaternityRecord(Random r)
        {
            lock (oLockInitialize)
            {
                if (!initialized)
                    Initialize();
                initialized = true;
            }

            SendingLocation = _locations.GetRandom(r);
        }

        private void Initialize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RecordCount",typeof(int));
            
            DataGenerator.EmbeddedCsvToDataTable(typeof(Maternity),"Location.csv",dt);
             
            _locations = new BucketList<string>();

            foreach (DataRow row in dt.Rows)
                _locations.Add((int)row["RecordCount"], (string)row["Location"]);
        }
    }
}

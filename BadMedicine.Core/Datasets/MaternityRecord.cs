using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BadMedicine.Datasets
{
    class MaternityRecord
    {
        public const int MinAge = 18;
        public const int MaxAge = 55;

        static object oLockInitialize = new object();
        private static bool initialized;
        private  static BucketList<string> _locations;
        private static BucketList<string> _maritalStatusOld;
        private static BucketList<string> _maritalStatusNew;
        
        /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="Location"]'/>
        public string Location {get;set;}
        
        /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="SendingLocation"]'/>
        public string SendingLocation {get;set;}

        /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="Date"]'/>
        public DateTime Date { get; set; }

        /// <include file='../../Datasets.doc.xml' path='Datasets/Maternity/Field[@name="MaritalStatus"]'/>
        public object MaritalStatus { get; set; }

        /// <summary>
        /// The person on whom the maternity action is performed
        /// </summary>
        public Person Person {get;set;}

        /// <summary>
        /// The date at which the data collector stopped using numeric marital status codes (in favour of alphabetical)
        /// </summary>
        private static DateTime MaritalStatusSwitchover = new DateTime(2001,1,1);

        /// <summary>
        /// Generates a new random biochemistry test.
        /// </summary>
        /// <param name="p">The person who is undergoing maternity activity.  Should be Female and of a sufficient age that the operation could have taken place during thier lifetime (see <see cref="Maternity.IsEligible(Person)"/></param>
        /// <param name="r"></param>
        public MaternityRecord(Person p,Random r)
        {
            lock (oLockInitialize)
            {
                if (!initialized)
                    Initialize();
                initialized = true;
            }
            Person = p;
            
            var youngest = p.DateOfBirth.AddYears(MinAge);
            var oldest =  p.DateOfDeath ?? p.DateOfBirth.AddYears(55);
            
            // No future dates
            oldest = oldest > DateTime.Now ? DataGenerator.Now : oldest;

            // If they died younger than 18 or were born less than 18 years into the past
            Date = youngest > oldest ? youngest : DataGenerator.GetRandomDate(youngest,oldest,r);

            Location = _locations.GetRandom(r);
            SendingLocation = _locations.GetRandom(r);
            MaritalStatus = Date < MaritalStatusSwitchover ? _maritalStatusOld.GetRandom(r) : _maritalStatusNew.GetRandom(r);

        }

        private void Initialize()
        {
            DataTable dt = new DataTable();
            
            DataGenerator.EmbeddedCsvToDataTable(typeof(Maternity),"Maternity.csv",dt);
             
            _locations = new BucketList<string>();
            _maritalStatusNew = new BucketList<string>();
            _maritalStatusOld = new BucketList<string>();

            foreach (DataRow row in dt.Rows)
            {
                AddRow(row,"Location",_locations);
                AddRow(row,"MaritalStatusNumeric",_maritalStatusOld);
                AddRow(row,"MaritalStatusAlpha",_maritalStatusNew);
                
            }
                
        }

        private void AddRow(DataRow row, string key, BucketList<string> bucketList)
        {
            var val = Convert.ToString(row[key]);
            var freq = row[key + "_RecordCount"];

            if(string.IsNullOrWhiteSpace(freq.ToString()))
                return;
            else
                bucketList.Add(Convert.ToInt32(freq),val);
        }
    }
}

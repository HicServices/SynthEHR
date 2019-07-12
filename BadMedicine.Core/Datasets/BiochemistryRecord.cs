// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Data class representing a single row in <see cref="Biochemistry"/> (use if you want to use randomly generated data directly
    /// rather than generate it into a file).
    /// </summary>
    public class BiochemistryRecord
    {
        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="LabNumber"]'/>
        public string LabNumber;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="SampleType"]'/>
        public string SampleType;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="TestCode"]'/>
        public string TestCode;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Result"]'/>
        public string Result;
        
        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="ReadCodeValue"]'/>
        public string ReadCodeValue;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Healthboard"]'/>
        public string Healthboard;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="ArithmeticComparator"]'/>
        public string ArithmeticComparator;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Interpretation"]'/>
        public string Interpretation;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="QuantityUnit"]'/>
        public string QuantityUnit;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="RangeHighValue"]'/>
        public string RangeHighValue;

        /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="RangeLowValue"]'/>
        public string RangeLowValue;

        static object oLockInitialize = new object();
        private static bool initialized;

        private  static BucketList<BiochemistryRandomDataRow> _bucketList;

        /// <summary>
        /// Generates a new random biochemistry test.
        /// </summary>
        /// <param name="r"></param>
        public BiochemistryRecord(Random r)
        {
            lock (oLockInitialize)
            {
                if (!initialized)
                    Initialize();
                initialized = true;
            }

            //get a random row from the lookup table - based on its representation within our biochemistry dataset
            var row = _bucketList.GetRandom(r);
            LabNumber = GetRandomLabNumber(r);
            TestCode = row.LocalClinicalCodeValue;
            SampleType = row.SampleName;

            Result = row.GetQVResult(r);

            ArithmeticComparator = row.ArithmeticComparator;
            Interpretation = row.Interpretation;
            QuantityUnit = row.QuantityUnit;
            RangeHighValue = row.RangeHighValue.HasValue ? row.RangeHighValue.ToString():"NULL";
            RangeLowValue = row.RangeLowValue.HasValue ? row.RangeLowValue.ToString():"NULL";
            
            Healthboard = row.hb_extract;
            ReadCodeValue = row.ReadCodeValue;
        }

        

        private string GetRandomLabNumber(Random r )
        {
            if(r.Next(0,2)==0)
                return "CC" + r.Next(0, 1000000);

            return "BC" + r.Next(0, 1000000);
        }
        private void Initialize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RecordCount",typeof(int));
            
            DataGenerator.EmbeddedCsvToDataTable(typeof(BiochemistryRecord),"Biochemistry.csv",dt);
             
            _bucketList = new BucketList<BiochemistryRandomDataRow>();

            foreach (DataRow row in dt.Rows)
                _bucketList.Add((int)row["RecordCount"], new BiochemistryRandomDataRow(row));

        }
        
        private class BiochemistryRandomDataRow
        {
            public string LocalClinicalCodeValue;
            public string ReadCodeValue;
            public string hb_extract;
            public string SampleName;
            public string ArithmeticComparator;
            public string Interpretation;
            public string QuantityUnit;
            public double? RangeHighValue;
            public double? RangeLowValue;
            public double? QVAverage;
            public double? QVStandardDev;

            public BiochemistryRandomDataRow(DataRow row)
            {
                LocalClinicalCodeValue  =(string) row["LocalClinicalCodeValue"];
                ReadCodeValue           =(string) row["ReadCodeValue"];
                hb_extract              =(string) row["hb_extract"];
                SampleName              =(string) row["SampleName"];
                ArithmeticComparator    =(string) row["ArithmeticComparator"];
                Interpretation          =(string) row["Interpretation"];
                QuantityUnit            =(string) row["QuantityUnit"];
                
                RangeHighValue = double.TryParse(row["RangeHighValue"].ToString(),out var rangeLow) ? rangeLow:(double?) null;
                RangeLowValue = double.TryParse(row["RangeLowValue"].ToString(),out var rangeHigh) ? rangeHigh:(double?) null;
                
                QVAverage = double.TryParse(row["QVAverage"].ToString(),out var min) ? min:(double?) null;
                QVStandardDev = double.TryParse(row["QVStandardDev"].ToString(),out var dev) ? dev:(double?) null;

            }

            /// <summary>
            /// Returns a new QV value using the <see cref="QVAverage"/> and <see cref="QVStandardDev"/> seeded with the provided
            /// <paramref name="r"/>.  Returns null if <see cref="QVAverage"/> or <see cref="QVStandardDev"/> are null.
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            internal string GetQVResult(Random r)
            {
                if(QVAverage.HasValue && QVStandardDev.HasValue)
                   return new Normal(QVAverage.Value, QVStandardDev.Value,r).Sample().ToString();
                 
                return null;
            }
        }
    }
}
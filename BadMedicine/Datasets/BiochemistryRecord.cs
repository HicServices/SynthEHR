// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Data class representing a single row in <see cref="Biochemistry"/> (use if you want to use randomly generated data directly
    /// rather than generate it into a file).
    /// </summary>
    public class BiochemistryRecord
    {
        /// <summary>
        /// every row in data table has a weigth (the number of records in our bichemistry with this sample type, this dictionary lets you input
        /// a record number 0-maxWeight and be returned an appropriate row from the table based on its weighting
        /// </summary>
        private static Dictionary<int, int> weightToRow;
        private static readonly int maxWeight = -1;
        private static DataTable lookupTable;

        public string Sample_type;
        public string Test_code;
        public string Result;
        public string Units;
        public string ReadCodeValue;
        public string ReadCodeDescription;

        static BiochemistryRecord()
        {
            lookupTable = DataGenerator.EmbeddedCsvToDataTable(typeof(BiochemistryRecord),"Biochemistry.csv");
             
            weightToRow = new Dictionary<int, int>();

            int currentWeight = 0;
            for (int i = 0; i < lookupTable.Rows.Count; i++)
            {
                currentWeight += int.Parse(lookupTable.Rows[i]["frequency"].ToString());
                weightToRow.Add(currentWeight, i);
            }

            maxWeight = currentWeight;
        }

        
        /// <summary>
        /// Generates a new random biochemistry test.
        /// </summary>
        /// <param name="r"></param>
        public BiochemistryRecord(Random r)
        {
            //get a random row from the lookup table - based on its representation within our biochemistry dataset
            DataRow row = GetRandomRowUsingWeight(r);

            Test_code = row["Test_code"].ToString();
            Sample_type = row["Sample_type"].ToString();

            var hasMin = double.TryParse(row["minResult"].ToString(),out var min);
            var hasMax = double.TryParse(row["maxResult"].ToString(),out var max);

            if(hasMin && hasMax)
                Result = ((r.NextDouble() * (max - min)) + min).ToString("#.##");
            else
                Result = "NULL";

            Units = row["Units"].ToString();
            ReadCodeValue = row["Read_code"].ToString();
            ReadCodeDescription = row["Description"].ToString();

        }

        private DataRow GetRandomRowUsingWeight(Random r)
        {
            int weightToGet = r.Next(maxWeight);

            //get the first key with a cumulative frequency above the one you are trying to get
            int row =  weightToRow.First(kvp => kvp.Key > weightToGet).Value;
            
            return lookupTable.Rows[row];
        }

    }
}
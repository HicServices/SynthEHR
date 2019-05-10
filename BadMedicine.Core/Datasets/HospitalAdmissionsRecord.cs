// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Random record for when a <see cref="BadMedicine.Person"/> entered hospital.  Basic logic is implemented here to ensure that <see cref="DischargeDate"/>
    /// is after <see cref="AdmissionDate"/> and that the person was alive at the time.    
    /// </summary>
    public class HospitalAdmissionsRecord
    {
        private static DataTable lookupTable;
        
        /// <summary>
        /// Random date indicating the time that the patient attended hospital
        /// </summary>
        public DateTime AdmissionDate{get; private set; }

        /// <summary>
        /// Random date indicating the time/date patient was discharged from hospital (will be up to 10 days after <see cref="AdmissionDate"/>)
        /// </summary>
        public DateTime DischargeDate { get; private set; }

        /// <summary>
        /// ICD Code based on the distribution of ICD codes in real hospital admissions.  This is the primary condition leading to the hospitalisation.
        /// </summary>
        public string MainCondition { get; private set; }

        /// <summary>
        /// ICD Code based on the distribution of ICD codes in real hospital admissions.  This is a secondary condition leading to the hospitalisation.  These
        /// codes tend to be more general codes than <see cref="MainCondition"/>.
        /// </summary>
        public string OtherCondition1 { get; private set; }

        /// <summary>
        /// ICD Code based on the distribution of ICD codes in real hospital admissions.  This is a secondary condition  leading to the hospitalisation.  These
        /// codes tend to be more general codes than <see cref="MainCondition"/>.
        /// </summary>
        public string OtherCondition2 { get; private set; }

        /// <summary>
        /// ICD Code based on the distribution of ICD codes in real hospital admissions.  This is a secondary condition leading to the hospitalisation.  These
        /// codes tend to be more general codes than <see cref="MainCondition"/>.
        /// </summary>
        public string OtherCondition3 { get; private set; }

        /// <summary>
        /// The <see cref="Person"/> being admitted to hospital
        /// </summary>
        public Person Person { get; set; }

        static object oLockInitialize = new object();
        private static bool initialized = false;
        
        /// <summary>
        /// Maps ColumnAppearingIn to each month we might want to generate random data in (Between <see cref="MinimumDate"/> and <see cref="MaximumDate"/>)
        /// to the row numbers which were active at that time (based on AverageMonthAppearing and StandardDeviationMonthAppearing)
        /// </summary>
        private static Dictionary<string, Dictionary<int, List<int>>> ICD10MonthHashMap;

        /// <summary>
        /// Maps Row(Key) to the CountAppearances/TestCode
        /// </summary>
        private static BucketList<string> ICD10Rows;
        
        /// <summary>
        /// The earliest date from which to generate records (matches HIC aggregate data collected)
        /// </summary>
        public static readonly DateTime MinimumDate = new DateTime(1983,1,1);

        /// <summary>
        /// The latest date to which to generate records (matches HIC aggregate data collected)
        /// </summary>
        public static readonly DateTime MaximumDate = new DateTime(2018,1,1);

        /// <summary>
        /// Creates a new record for the given <paramref name="person"/> 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="afterDateX"></param>
        /// <param name="r"></param>
        public HospitalAdmissionsRecord(Person person, DateTime afterDateX, Random r)
        {
            lock (oLockInitialize)
            {
                if (!initialized)
                    Initialize(r);
                initialized = true;
            }

            Person = person;
            if (person.DateOfBirth > afterDateX)
                afterDateX = person.DateOfBirth;
                
            AdmissionDate = DataGenerator.GetRandomDate(afterDateX.Max(MinimumDate), MaximumDate, r);
            
            DischargeDate = AdmissionDate.AddHours(r.Next(240));//discharged after random number of hours between 0 and 240 = 10 days

            //Condition 1 always populated
            MainCondition = GetRandomICDCode("MAIN_CONDITION",r);

            //50% chance of condition 2 as well as 1
            if(r.Next(2) == 0)
            {
                OtherCondition1 = GetRandomICDCode("OTHER_CONDITION_1",r);

                //25% chance of condition 3 too
                if (r.Next(2) == 0)
                {
                    OtherCondition2 = GetRandomICDCode("OTHER_CONDITION_2",r);
                    
                    //12.5% chance of all conditions
                    if (r.Next(2) == 0)
                        OtherCondition3 = GetRandomICDCode("OTHER_CONDITION_3",r);

                    //1.25% chance of dirty data = the text 'Nul'
                    if(r.Next(10) ==0)
                        OtherCondition3 = "Nul";
                }
            }
        }

        private void Initialize(Random random)
        {
            ICD10Rows = new BucketList<string>(random);

            DataTable dt = new DataTable();
            dt.Columns.Add("AverageMonthAppearing", typeof(double));
            dt.Columns.Add("StandardDeviationMonthAppearing", typeof(double));
            dt.Columns.Add("CountAppearances", typeof(int));

            lookupTable = DataGenerator.EmbeddedCsvToDataTable(typeof(HospitalAdmissionsRecord), "HospitalAdmissions.csv",dt);
            
            ICD10MonthHashMap = new Dictionary<string, Dictionary<int, List<int>>>
            {
                {"MAIN_CONDITION", new Dictionary<int, List<int>>()},
                {"OTHER_CONDITION_1", new Dictionary<int,  List<int>>()},
                {"OTHER_CONDITION_2", new Dictionary<int,  List<int>>()},
                {"OTHER_CONDITION_3", new Dictionary<int,  List<int>>()}
            };


            //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)

            //get all the months we might be asked for
            int from = (MinimumDate.Year - 1900) * 12 + MinimumDate.Month;
            int to = (MaximumDate.Year - 1900) * 12 + MaximumDate.Month;


            foreach (var columnKey in ICD10MonthHashMap.Keys)
            {
                for (int i = from; i <= to; i++)
                {
                    ICD10MonthHashMap[columnKey].Add(i, new List<int>());
                }
            }

            int rowCount = 0;

            //for each row in the sample data
            foreach (DataRow row in lookupTable.Rows)
            {
                //calculate 2 standard deviations in months
                int monthFrom = Convert.ToInt32((double) row["AverageMonthAppearing"] - (2 * (double) row["StandardDeviationMonthAppearing"]));
                int monthTo = Convert.ToInt32((double) row["AverageMonthAppearing"] + (2 * (double) row["StandardDeviationMonthAppearing"]));

                //2 standard deviations might take us beyond the beginning or start so only build hashmap for dates we will be asked for
                monthFrom = Math.Max(monthFrom, from);
                monthTo = Math.Min(monthTo, to);

                //for each month add row to the hashmap (for the correct column and month in the range)
                for (int i = monthFrom; i <= monthTo; i++)
                {
                    if(monthFrom < from)
                        continue;

                    if(monthTo > to)
                        break;
                    
                    ICD10MonthHashMap[(string)row["ColumnAppearingIn"]][i].Add(rowCount);
                }

                ICD10Rows.Add((int) row["CountAppearances"], (string) row["TestCode"]); 
                rowCount++;
            }
            
        }
        
        private string GetRandomICDCode(string field, Random random)
        {
            
            //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)
            int monthsSinceZeroDay = (AdmissionDate.Year - 1900) * 12 + AdmissionDate.Month;

            return ICD10Rows.GetRandom(ICD10MonthHashMap[field][monthsSinceZeroDay]);
        }
    }
}

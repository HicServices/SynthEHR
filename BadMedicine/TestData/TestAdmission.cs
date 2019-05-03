// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using BadMedicine.TestData.Exercises;

namespace BadMedicine.TestData
{
    /// <summary>
    /// Random record for when a <see cref="TestPerson"/> entered hospital.  Basic logic is implemented here to ensure that <see cref="DischargeDate"/>
    /// is after <see cref="AdmissionDate"/> and that the person was alive at the time.    
    /// </summary>
    public class TestAdmission
    {
        private static DataTable lookupTable;
        public const string AdmissionDateDescription = "The time and date that the (fictional) patient attended the hospital";
        public const string DischargeDateDescription = "The time and date that the (fictional) patient departed the hospital";
        public const string Condition1Description = "The primary condition that the (fictional) patient is suffering from, conditions 2-4 are optional but condition1 is always populated.  This is an ICD10 code which can be found in the ICD10 lookup";
        public const string Condition2To4Description = "See Condition1";
        

        /// <summary>
        /// Random date indicating the time that the patient attended hospital
        /// </summary>
        public DateTime AdmissionDate{get; private set; }

        /// <summary>
        /// Random date indicating the time/date patient was discharged from hospital (will be up to 10 days after <see cref="AdmissionDate"/>)
        /// </summary>
        public DateTime DischargeDate { get; private set; }

        /// <summary>
        /// A random ICD Code based on the distribution of ICD codes in real hospital admissions
        /// </summary>
        public string Condition1 { get; private set; }
        public string Condition2 { get; private set; }
        public string Condition3 { get; private set; }
        public string Condition4 { get; private set; }

        public TestPerson Person { get; set; }

        static TestAdmission()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AverageMonthAppearing", typeof(double));
            dt.Columns.Add("StandardDeviationMonthAppearing", typeof(double));
            dt.Columns.Add("CountAppearances", typeof(int));

            lookupTable = ExerciseTestDataGenerator.EmbeddedCsvToDataTable(typeof(TestAdmission), "HospitalAdmissionCodesFrequency.csv",dt);

            BuildICD10Hashmap();
        }

        private static Dictionary<string, Dictionary<int, SortedList<string, int>>> ICD10Hashmap;

        private static void BuildICD10Hashmap()
        {
            ICD10Hashmap = new Dictionary<string, Dictionary<int, SortedList<string, int>>>();

            ICD10Hashmap.Add("MAIN_CONDITION",new Dictionary<int, SortedList<string, int>>());
            ICD10Hashmap.Add("OTHER_CONDITION_1",new Dictionary<int, SortedList<string, int>>());
            ICD10Hashmap.Add("OTHER_CONDITION_2",new Dictionary<int, SortedList<string, int>>());
            ICD10Hashmap.Add("OTHER_CONDITION_3",new Dictionary<int, SortedList<string, int>>());

            //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)

            //get all the months we might be asked for
            int from = (MinimumDate.Year - 1900) * 12 + MinimumDate.Month;
            int to = (MaximumDate.Year - 1900) * 12 + MaximumDate.Month;


            foreach (var columnKey in ICD10Hashmap.Keys)
            {
                for (int i = from; i <= to; i++)
                {
                    ICD10Hashmap[columnKey].Add(i, new SortedList<string, int>());
                }
            }

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
                    
                    ICD10Hashmap[(string)row["ColumnAppearingIn"]][i].Add((string)row["TestCode"], (int)row["CountAppearances"]);
                }
                
            }
        }

        public static readonly DateTime MinimumDate = new DateTime(1983,1,1);
        public static readonly DateTime MaximumDate = new DateTime(2018,1,1);

        public TestAdmission(TestPerson person, DateTime afterDateX, Random r)
        {
            Person = person;
            if (person.DateOfBirth > afterDateX)
                afterDateX = person.DateOfBirth;
                
            AdmissionDate = GetRandomDate(afterDateX.Max(MinimumDate), MaximumDate, r);
            
            DischargeDate = AdmissionDate.AddHours(r.Next(240));//discharged after random number of hours between 0 and 240 = 10 days

            //Condition 1 always populated
            Condition1 = GetRandomICDCode("MAIN_CONDITION",r);

            //50% chance of condition 2 as well as 1
            if(r.Next(2) == 0)
            {
                Condition2 = GetRandomICDCode("OTHER_CONDITION_1",r);

                //25% chance of condition 3 too
                if (r.Next(2) == 0)
                {
                    Condition3 = GetRandomICDCode("OTHER_CONDITION_2",r);
                    
                    //12.5% chance of all conditions
                    if (r.Next(2) == 0)
                        Condition4 = GetRandomICDCode("OTHER_CONDITION_3",r);

                    //1.25% chance of dirty data = the text 'Nul'
                    if(r.Next(10) ==0)
                        Condition4 = "Nul";
                }
            }
        }
        
        public static DateTime GetRandomDate(DateTime from, DateTime to, Random r)
        {
            var range = to - from;

            var randTimeSpan = new TimeSpan((long) (r.NextDouble()*range.Ticks));

            return from + randTimeSpan;
        }

        private string GetRandomICDCode(string field, Random random)
        {
            
            //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)
            int monthsSinceZeroDay = (AdmissionDate.Year - 1900) * 12 + AdmissionDate.Month;

            int total = ICD10Hashmap[field][monthsSinceZeroDay].Values.Sum();
            
            int toPick = random.Next(0, total);

            foreach (var kvp in ICD10Hashmap[field][monthsSinceZeroDay])
            {
                toPick -= kvp.Value;
                if (toPick < 0)
                    return kvp.Key;
            }

            throw new Exception("Could not pick a valid ICD code for the AdmissionDate " + AdmissionDate);
        }
    }
}

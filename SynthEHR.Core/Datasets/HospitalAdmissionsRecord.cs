// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;

namespace SynthEHR.Datasets;

/// <summary>
/// Random record for when a <see cref="SynthEHR.Person"/> entered hospital.  Basic logic is implemented here to ensure that <see cref="DischargeDate"/>
/// is after <see cref="AdmissionDate"/> and that the person was alive at the time.
/// </summary>
public sealed class HospitalAdmissionsRecord
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="AdmissionDate"]'/>
    public DateTime AdmissionDate { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="DischargeDate"]'/>
    public DateTime DischargeDate { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="MainCondition"]'/>
    public string MainCondition { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherCondition1"]'/>
    public string OtherCondition1 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherCondition2"]'/>
    public string OtherCondition2 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherCondition3"]'/>
    public string OtherCondition3 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="MainOperation"]'/>
    public string MainOperation { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="MainOperationB"]'/>
    public string MainOperationB { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation1"]'/>
    public string OtherOperation1 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation1B"]'/>
    public string OtherOperation1B { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation2"]'/>
    public string OtherOperation2 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation2B"]'/>
    public string OtherOperation2B { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation3"]'/>
    public string OtherOperation3 { get; private set; }

    /// <include file='../../Datasets.doc.xml' path='Datasets/HospitalAdmissions/Field[@name="OtherOperation3B"]'/>
    public string OtherOperation3B { get; private set; }

    /// <summary>
    /// The <see cref="Person"/> being admitted to hospital
    /// </summary>
    public Person Person { get; set; }

    /// <summary>
    /// Maps ColumnAppearingIn to each month we might want to generate random data in (Between <see cref="MinimumDate"/> and <see cref="MaximumDate"/>)
    /// to the row numbers which were active at that time (based on AverageMonthAppearing and StandardDeviationMonthAppearing)
    /// </summary>
    private static readonly Dictionary<string, Dictionary<int, List<int>>> ICD10MonthHashMap;

    /// <summary>
    /// Maps Row(Key) to the CountAppearances/TestCode
    /// </summary>
    private static readonly BucketList<string> ICD10Rows;

    /// <summary>
    /// Maps a given MAIN_CONDITION code (doesn't cover other conditions) to popular operations for that condition.  The string array is always length 8 and corresponds to
    /// MAIN_OPERATION,MAIN_OPERATION_B,OTHER_OPERATION_1,OTHER_OPERATION_1B,OTHER_OPERATION_2,OTHER_OPERATION_2B,OTHER_OPERATION_3,OTHER_OPERATION_3B
    /// </summary>
    private static readonly Dictionary<string, BucketList<string[]>> ConditionsToOperationsMap = [];

    /// <summary>
    /// The earliest date from which to generate records (matches HIC aggregate data collected)
    /// </summary>
    public static readonly DateTime MinimumDate = new(1983, 1, 1);

    /// <summary>
    /// The latest date to which to generate records (matches HIC aggregate data collected)
    /// </summary>
    public static readonly DateTime MaximumDate = new(2018, 1, 1);

    /// <summary>
    /// Creates a new record for the given <paramref name="person"/>
    /// </summary>
    /// <param name="person"></param>
    /// <param name="afterDateX"></param>
    /// <param name="r"></param>
    public HospitalAdmissionsRecord(Person person, DateTime afterDateX, Random r)
    {
        Person = person;
        if (person.DateOfBirth > afterDateX)
            afterDateX = person.DateOfBirth;

        AdmissionDate = DataGenerator.GetRandomDate(afterDateX.Max(MinimumDate), MaximumDate, r);

        DischargeDate = AdmissionDate.AddHours(r.Next(240));//discharged after random number of hours between 0 and 240 = 10 days

        //Condition 1 always populated
        MainCondition = GetRandomICDCode("MAIN_CONDITION", r);

        //50% chance of condition 2 as well as 1
        if (r.Next(2) == 0)
        {
            OtherCondition1 = GetRandomICDCode("OTHER_CONDITION_1", r);

            //25% chance of condition 3 too
            if (r.Next(2) == 0)
            {
                OtherCondition2 = GetRandomICDCode("OTHER_CONDITION_2", r);

                //12.5% chance of all conditions
                if (r.Next(2) == 0)
                    OtherCondition3 = GetRandomICDCode("OTHER_CONDITION_3", r);

                //1.25% chance of dirty data = the text 'Nul'
                if (r.Next(10) == 0)
                    OtherCondition3 = "Nul";
            }
        }

        //if the condition is one that is often treated in a specific way
        if (!ConditionsToOperationsMap.TryGetValue(MainCondition, out var operationsList)) return;

        var operations = operationsList.GetRandom(r);

        MainOperation = operations[0];
        MainOperationB = operations[1];
        OtherOperation1 = operations[2];
        OtherOperation1B = operations[3];
        OtherOperation2 = operations[4];
        OtherOperation2B = operations[5];
        OtherOperation3 = operations[6];
        OtherOperation3B = operations[7];
    }

    static HospitalAdmissionsRecord()
    {
        ICD10Rows = [];

        using var dt = new DataTable();
        dt.BeginLoadData();
        dt.Columns.Add("AverageMonthAppearing", typeof(double));
        dt.Columns.Add("StandardDeviationMonthAppearing", typeof(double));
        dt.Columns.Add("CountAppearances", typeof(int));

        var lookupTable = DataGenerator.EmbeddedCsvToDataTable(typeof(HospitalAdmissionsRecord), "HospitalAdmissions.csv", dt);

        ICD10MonthHashMap = new Dictionary<string, Dictionary<int, List<int>>>
            {
                {"MAIN_CONDITION", new Dictionary<int, List<int>>()},
                {"OTHER_CONDITION_1", new Dictionary<int,  List<int>>()},
                {"OTHER_CONDITION_2", new Dictionary<int,  List<int>>()},
                {"OTHER_CONDITION_3", new Dictionary<int,  List<int>>()}
            };


        //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)

        //get all the months we might be asked for
        var from = (MinimumDate.Year - 1900) * 12 + MinimumDate.Month;
        var to = (MaximumDate.Year - 1900) * 12 + MaximumDate.Month;


        foreach (var columnKey in ICD10MonthHashMap.Keys)
        {
            for (var i = from; i <= to; i++)
            {
                ICD10MonthHashMap[columnKey].Add(i, []);
            }
        }

        var rowCount = 0;

        //for each row in the sample data
        foreach (DataRow row in lookupTable.Rows)
        {
            //calculate 2 standard deviations in months
            var monthFrom = Convert.ToInt32((double)row["AverageMonthAppearing"] - 2 * (double)row["StandardDeviationMonthAppearing"]);
            var monthTo = Convert.ToInt32((double)row["AverageMonthAppearing"] + 2 * (double)row["StandardDeviationMonthAppearing"]);

            //2 standard deviations might take us beyond the beginning or start so only build hashmap for dates we will be asked for
            monthFrom = Math.Max(monthFrom, from);
            monthTo = Math.Min(monthTo, to);

            //for each month add row to the hashmap (for the correct column and month in the range)
            for (var i = monthFrom; i <= monthTo; i++)
            {
                if (monthFrom < from)
                    continue;

                if (monthTo > to)
                    break;

                ICD10MonthHashMap[(string)row["ColumnAppearingIn"]][i].Add(rowCount);
            }

            ICD10Rows.Add((int)row["CountAppearances"], (string)row["TestCode"]);
            rowCount++;
        }

        using var operationsTable = new DataTable();
        operationsTable.BeginLoadData();
        operationsTable.Columns.Add("CountOfRecords", typeof(int));

        DataGenerator.EmbeddedCsvToDataTable(typeof(HospitalAdmissionsRecord), "HospitalAdmissionsOperations.csv", operationsTable);
        operationsTable.EndLoadData();

        foreach (DataRow r in operationsTable.Rows)
        {
            var key = (string)r["MAIN_CONDITION"];
            if (!ConditionsToOperationsMap.TryGetValue(key, out var conditionOps))
                ConditionsToOperationsMap.Add(key, conditionOps = []);

            conditionOps.Add((int)r["CountOfRecords"], [
                    r["MAIN_OPERATION"] as string,
                r["MAIN_OPERATION_B"] as string,
                r["OTHER_OPERATION_1"] as string,
                r["OTHER_OPERATION_1B"] as string,
                r["OTHER_OPERATION_2"] as string,
                r["OTHER_OPERATION_2B"] as string,
                r["OTHER_OPERATION_3"] as string,
                r["OTHER_OPERATION_3B"] as string
                    ]);
        }
    }


    private string GetRandomICDCode(string field, Random random)
    {

        //The number of months since 1/1/1900 (this is the measure of field AverageMonthAppearing)
        var monthsSinceZeroDay = (AdmissionDate.Year - 1900) * 12 + AdmissionDate.Month;

        return ICD10Rows.GetRandom(ICD10MonthHashMap[field][monthsSinceZeroDay], random);
    }

}
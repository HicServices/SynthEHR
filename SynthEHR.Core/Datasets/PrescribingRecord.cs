// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SynthEHR.Datasets;

/// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing'/>
public sealed class PrescribingRecord
{
    /// <summary>
    /// every row in data table has a weight (the number of records in our biochemistry with this sample type, this dictionary lets you input
    /// a record number 0-maxWeight and be returned an appropriate row from the table based on its weighting
    /// </summary>
    private static readonly Dictionary<int, int> WeightToRow;
    private static readonly int MaxWeight;
    private static readonly DataTable LookupTable;

    static PrescribingRecord()
    {
        LookupTable = DataGenerator.EmbeddedCsvToDataTable(typeof(PrescribingRecord), "Prescribing.csv");

        WeightToRow = [];

        var currentWeight = 0;
        for (var i = 0; i < LookupTable.Rows.Count; i++)
        {
            var frequency = int.Parse(LookupTable.Rows[i]["frequency"].ToString());

            if (frequency == 0)
                continue;

            currentWeight += frequency;

            WeightToRow.Add(currentWeight, i);
        }

        MaxWeight = currentWeight;
    }

    /// <summary>
    /// Generates a new random prescription record
    /// </summary>
    /// <param name="r"></param>
    public PrescribingRecord(Random r)
    {
        //get a random row from the lookup table - based on its representation within our biochemistry dataset
        var row = GetRandomRowUsingWeight(r);

        ResSeqNo = row["res_seqno"].ToString();
        Name = row["name"].ToString();
        FormulationCode = row["formulation_code"].ToString();
        Strength = row["strength"].ToString();
        StrengthNumerical = row["orig_strength"].ToString() == "NULL" ? null : Convert.ToDouble(row["orig_strength"].ToString());
        MeasureCode = row["measure_code"].ToString();
        BnfCode = row["BNF_Code"].ToString();
        FormattedBnfCode = row["formatted_BNF_Code"].ToString();
        BnfDescription = row["BNF_Description"].ToString();
        ApprovedName = row["Approved_Name"].ToString();

        var hasMin = double.TryParse(row["minQuantity"].ToString(), out var min);
        var hasMax = double.TryParse(row["maxQuantity"].ToString(), out var max);

        if (hasMin && hasMax)
            Quantity = ((int)(r.NextDouble() * (max - min) + min)).ToString();//it is a number
        else
            if (r.Next(0, 2) == 0)
            Quantity = row["minQuantity"].ToString();//it isn't a number, randomly select max or min
        else
            Quantity = row["maxQuantity"].ToString();

    }

    private static DataRow GetRandomRowUsingWeight(Random r)
    {
        var weightToGet = r.Next(MaxWeight);

        //get the first key with a cumulative frequency above the one you are trying to get
        var row = WeightToRow.First(kvp => kvp.Key > weightToGet).Value;

        return LookupTable.Rows[row];
    }

    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="ResSeqNo"]'/>
    public string ResSeqNo;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="Name"]'/>
    public string Name;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="FormulationCode"]'/>
    public string FormulationCode;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="Strength"]'/>
    public string Strength;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="StrengthNumerical"]'/>
    public double? StrengthNumerical;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="MeasureCode"]'/>
    public string MeasureCode;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="BnfCode"]'/>
    public string BnfCode;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="FormattedBnfCode"]'/>
    public string FormattedBnfCode;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="BnfDescription"]'/>
    public string BnfDescription;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="ApprovedName"]'/>
    public string ApprovedName;
    /// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing/Field[@name="Quantity"]'/>
    public string Quantity;
}
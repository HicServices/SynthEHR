// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace BadMedicine.Datasets;

/// <summary>
/// Data class representing a single row in <see cref="Biochemistry"/> (use if you want to use randomly generated data directly
/// rather than generate it into a file).
/// </summary>
public sealed class BiochemistryRecord
{
    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="LabNumber"]'/>
    public readonly string LabNumber;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="SampleType"]'/>
    public readonly string SampleType;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="TestCode"]'/>
    public readonly string TestCode;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Result"]'/>
    public readonly string Result;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="ReadCodeValue"]'/>
    public readonly string ReadCodeValue;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Healthboard"]'/>
    public readonly string Healthboard;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="ArithmeticComparator"]'/>
    public readonly string ArithmeticComparator;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="Interpretation"]'/>
    public readonly string Interpretation;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="QuantityUnit"]'/>
    public readonly string QuantityUnit;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="RangeHighValue"]'/>
    public readonly string RangeHighValue;

    /// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry/Field[@name="RangeLowValue"]'/>
    public readonly string RangeLowValue;

    private static readonly BucketList<BiochemistryRandomDataRow> BucketList;
    static BiochemistryRecord()
    {
        using var dt = new DataTable();
        dt.Columns.Add("RecordCount", typeof(int));

        DataGenerator.EmbeddedCsvToDataTable(typeof(BiochemistryRecord), "Biochemistry.csv", dt);

        BucketList = [];

        foreach (var row in dt.Rows.Cast<DataRow>().OrderByDescending(static row => (int)row["RecordCount"]))
            BucketList.Add((int)row["RecordCount"], new BiochemistryRandomDataRow(row));
    }

    /// <summary>
    /// Generates a new random biochemistry test.
    /// </summary>
    /// <param name="r"></param>
    public BiochemistryRecord(Random r)
    {
        //get a random row from the lookup table - based on its representation within our biochemistry dataset
        var row = BucketList.GetRandom(r);
        LabNumber = GetRandomLabNumber(r);
        TestCode = row.LocalClinicalCodeValue;
        SampleType = row.SampleName;

        Result = row.GetQVResult(r);

        ArithmeticComparator = row.ArithmeticComparator;
        Interpretation = row.Interpretation;
        QuantityUnit = row.QuantityUnit;
        RangeHighValue = row.RangeHighValue.HasValue ? row.RangeHighValue.ToString() : "NULL";
        RangeLowValue = row.RangeLowValue.HasValue ? row.RangeLowValue.ToString() : "NULL";

        Healthboard = row.hb_extract;
        ReadCodeValue = row.ReadCodeValue;
    }



    private static string GetRandomLabNumber(Random r)
    {
        return r.Next(0, 2) == 0 ? $"CC{r.Next(0, 1000000)}" : $"BC{r.Next(0, 1000000)}";
    }

    private sealed class BiochemistryRandomDataRow(DataRow row)
    {
        public readonly string LocalClinicalCodeValue = (string)row["LocalClinicalCodeValue"];
        public readonly string ReadCodeValue = (string)row["ReadCodeValue"];
        public readonly string hb_extract = (string)row["hb_extract"];
        public readonly string SampleName = (string)row["SampleName"];
        public readonly string ArithmeticComparator = (string)row["ArithmeticComparator"];
        public readonly string Interpretation = (string)row["Interpretation"];
        public readonly string QuantityUnit = (string)row["QuantityUnit"];
        public double? RangeHighValue = double.TryParse(row["RangeHighValue"].ToString(), out var rangeLow) ? rangeLow : null;
        public double? RangeLowValue = double.TryParse(row["RangeLowValue"].ToString(), out var rangeHigh) ? rangeHigh : null;
        private readonly double? QVAverage = double.TryParse(row["QVAverage"].ToString(), out var min) ? min : null;
        private readonly double? QVStandardDev = double.TryParse(row["QVStandardDev"].ToString(), out var dev) ? dev : null;

        /// <summary>
        /// Returns a new QV value using the <see cref="QVAverage"/> and <see cref="QVStandardDev"/> seeded with the provided
        /// <paramref name="r"/>.  Returns null if <see cref="QVAverage"/> or <see cref="QVStandardDev"/> are null.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        internal string GetQVResult(Random r)
        {
            return !QVAverage.HasValue || !QVStandardDev.HasValue
                ? null
                : new Normal(QVAverage.Value, QVStandardDev.Value, r).Sample().ToString(CultureInfo.CurrentCulture);
        }
    }
}
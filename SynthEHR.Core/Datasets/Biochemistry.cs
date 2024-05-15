// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;

namespace SynthEHR.Datasets;

/// <include file='../../Datasets.doc.xml' path='Datasets/Biochemistry'/>
/// <inheritdoc/>
public sealed class Biochemistry(Random rand) : DataGenerator(rand)
{

    /// <inheritdoc/>
    public override object[] GenerateTestDataRow(Person p)
    {
            var results = new object[13];

            var randomSample = new BiochemistryRecord(r);

            results[0] = p.CHI;
            results[1] = randomSample.Healthboard;
            results[2] = p.GetRandomDateDuringLifetime(r);
            results[3] = randomSample.SampleType;
            results[4] = randomSample.TestCode;
            results[5] = randomSample.Result;
            results[6] = randomSample.LabNumber;
            results[7] = randomSample.QuantityUnit;
            results[8] = randomSample.ReadCodeValue;
            results[9] = randomSample.ArithmeticComparator;
            results[10] = randomSample.Interpretation;
            results[11] = randomSample.RangeHighValue;
            results[12] = randomSample.RangeLowValue;

            return results;

        }

    /// <inheritdoc/>
    protected override string[] GetHeaders() =>
    [
        "chi",                              //0
        "Healthboard",                      //1
        "SampleDate",                       //2
        "SampleType",                       //3
        "TestCode",                         //4
        "Result",                           //5
        "Labnumber",                        //6
        "QuantityUnit",                     //7
        "ReadCodeValue",                    //8
        "ArithmeticComparator",             //9
        "Interpretation",                   //10
        "RangeHighValue",                   //11
        "RangeLowValue" //12

    ];
}
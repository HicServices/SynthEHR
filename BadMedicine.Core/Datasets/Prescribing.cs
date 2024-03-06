// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;

namespace BadMedicine.Datasets;

/// <include file='../../Datasets.doc.xml' path='Datasets/Prescribing'/>
/// <inheritdoc/>
public class Prescribing(Random rand) : DataGenerator(rand)
{

    /// <summary>
    /// Creates a new demography record (GP registration) for the <paramref name="p"/>
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public override object[] GenerateTestDataRow(Person p)
    {
            var values = new object[12];

            var prescription = new PrescribingRecord(r);

            values[0] = p.CHI;
            values[1] = p.GetRandomDateDuringLifetime(r);
            values[2] = prescription.Quantity;
            values[3] = prescription.Strength;
            values[4] = prescription.StrengthNumerical;
            values[5] = prescription.FormulationCode;
            values[6] = prescription.MeasureCode;
            values[7] = prescription.Name;
            values[8] = prescription.ApprovedName;
            values[9] = prescription.BnfCode;
            values[10] = prescription.FormattedBnfCode;
            values[11] = prescription.BnfDescription;

            return values;
        }

    /// <inheritdoc/>
    protected override string[] GetHeaders() =>
    [
        "chi",
        "PrescribedDate",
        "Quantity",
        "Strength",
        "StrengthNumerical",
        "FormulationCode",
        "MeasureCode",
        "Name",
        "ApprovedName",
        "BnfCode",
        "FormattedBnfCode",
        "BnfDescription"
    ];
}
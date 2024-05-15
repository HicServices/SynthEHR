// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Data;
using System.IO;

namespace SynthEHR.Datasets;

/// <summary>
/// Interface for classes which generate test data to disk.
/// </summary>
public interface IDataGenerator
{
    /// <summary>
    /// Periodically fired when writing out rows
    /// </summary>
    event EventHandler<RowsGeneratedEventArgs> RowsGenerated;

    /// <summary>
    /// Create the dataset in the given file location using person identifiers in the <paramref name="cohort"/>
    /// </summary>
    /// <param name="cohort">All people in the test data cohort, allows linkage between different randomly generated test datasets</param>
    /// <param name="target">The file that will be created</param>
    /// <param name="numberOfRecords">The number of fake data records that should appear in the file created</param>
    void GenerateTestDataFile(IPersonCollection cohort, FileInfo target, int numberOfRecords);

    /// <summary>
    /// Returns a single row of data for writing to the output CSV.  This can include string elements with newlines, quotes etc.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    object[] GenerateTestDataRow(Person p);

    /// <summary>
    /// Create the dataset in memory using the person identifiers in the <paramref name="cohort"/>
    /// </summary>
    /// <param name="cohort">All people in the test data cohort, allows linkage between different randomly generated test datasets</param>
    /// <param name="numberOfRecords">The number of fake data records that should be in the table returned</param>
    /// <returns></returns>
    DataTable GetDataTable(IPersonCollection cohort, int numberOfRecords);
}
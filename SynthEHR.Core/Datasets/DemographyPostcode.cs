// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

namespace SynthEHR.Datasets;

/// <summary>
/// Data model for a UK postcode
/// </summary>
/// <remarks>
/// Creates a new UK Postcode with it's associated Ward/District (See <see cref="Ward"/> and <see cref="District"/>)
/// </remarks>
/// <param name="value"></param>
/// <param name="ward"></param>
/// <param name="district"></param>
public class DemographyPostcode(string value, string ward, string district)
{
    /// <summary>
    /// The full postcode e.g. "DD8 3PZ"
    /// </summary>
    public string Value { get; set; } = value;

    /// <summary>
    /// The region associated with the postcode e.g. "Angus"
    /// </summary>
    public string Ward { get; set; } = ward;

    /// <summary>
    /// The district associated with the postcode e.g. "Brechin and Edzell"
    /// </summary>
    public string District { get; set; } = district;
}
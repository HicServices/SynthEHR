// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

namespace BadMedicine.TestData
{
    /// <summary>
    /// Data model for a UK postcode
    /// </summary>
    public class Postcode
    {
        /// <summary>
        /// The full postcode e.g. "DD8 3PZ"
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The region associated with the postcode e.g. "Angus"
        /// </summary>
        public string Ward { get; set; }

        /// <summary>
        /// The district associated with the postcode e.g. "Brechin and Edzell"
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Creates a new UK Postcode with it's associated Ward/District (See <see cref="Ward"/> and <see cref="District"/>)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ward"></param>
        /// <param name="district"></param>
        public Postcode( string value, string ward, string district)
        {
            Value = value;
            Ward = ward;
            District = district;
        }
    }
}
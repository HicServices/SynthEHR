// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;

namespace SynthEHR.Datasets;

/// <summary>
/// Data class describing an appointment including a guid identifier
/// </summary>
/// <remarks>
/// Creates a new randomly generated appointment within the lifetime of the <paramref name="testPerson"/>
/// </remarks>
/// <param name="testPerson"></param>
/// <param name="r"></param>
public sealed class Appointment(Person testPerson, Random r)
{
    /// <summary>
    /// Globally unique identifier for this appointment
    /// </summary>
    public string Identifier { get; set; } = $"APPT_{Guid.NewGuid()}";

    /// <summary>
    /// Random date within the lifetime of the <see cref="Person"/> used for construction
    /// </summary>
    public DateTime StartDate { get; set; } = testPerson.GetRandomDateDuringLifetime(r);
}
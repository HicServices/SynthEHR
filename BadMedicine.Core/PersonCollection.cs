// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace BadMedicine
{
    /// <inheritdoc/>
    public class PersonCollection:IPersonCollection
    {
        /// <inheritdoc/>
        public Person[] People { get; private set; }

        public HashSet<string> AlreadyGeneratedCHIs = new HashSet<string>();
        public HashSet<string> AlreadyGeneratedANOCHIs = new HashSet<string>();

        /// <inheritdoc/>
        public void GeneratePeople(int numberOfUniqueIndividuals, Random random)
        {
            People = new Person[numberOfUniqueIndividuals];

            for (int i = 0; i < numberOfUniqueIndividuals; i++)
                People[i]=new Person(random,this);
        }
    }
}

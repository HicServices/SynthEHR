// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.IO;

namespace BadMedicine.Datasets
{
    /// <summary>
    /// Test about biochemistry lab tests e.g. CRE (Creatinine). 
    /// </summary>
    public class Biochemistry: DataGenerator
    {
        /// <inheritdoc/>
        public override object[] GenerateTestDataRow(Person p)
        {
            object[] results = new object[10];

            BiochemistryRecord randomSample = new BiochemistryRecord(r);
            
            results[0] = p.CHI;
            results[1] = GetRandomLetter(true, r);
            results[2] = p.GetRandomDateDuringLifetime(r);
            results[3] = randomSample.Sample_type;
            results[4] = randomSample.Test_code;
            results[5] = randomSample.Result;
            results[6] = GetRandomLabNumber();
            results[7] = randomSample.Units;
            results[8] = randomSample.ReadCodeValue;
            results[9] = randomSample.ReadCodeDescription;

            return results;

        }

        /// <inheritdoc/>
        protected override void WriteHeaders(StreamWriter sw)
        {
            string[] h =
            {
                "chi",                              //0
                "hb_extract",                       //1
                "Sample_date",                      //2
                "Sample_type",                      //3
                "Test_code",                        //4
                "Result",                           //5
                "Labnumber",                        //6
                "Units",                            //7
                "ReadCodeValue",                    //8
                "ReadCodeDescription"               //9
            };

            sw.WriteLine(string.Join(",",h));
        }

        private string GetRandomLabNumber()
        {
            if(r.Next(0,2)==0)
                return "CC" + r.Next(0, 1000000);

            return "BC" + r.Next(0, 1000000);
        }
    }
}

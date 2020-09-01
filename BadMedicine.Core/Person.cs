// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;
using BadMedicine.Datasets;

namespace BadMedicine
{
    /// <summary>
    /// Randomly generated person for whom datasets can be built
    /// </summary>
    public class Person : IEquatable<Person>
    {
        
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="Forename"]'/>
        public string Forename { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="Surname"]'/>
        public string Surname { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Common/Field[@name="chi"]'/>
        public string CHI { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="ANOCHI"]'/>
        public string ANOCHI { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="DateOfBirth"]'/>
        public DateTime DateOfBirth = new DateTime();
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="DateOfDeath"]'/>
        public DateTime? DateOfDeath;
        
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="Gender"]'/>
        public char Gender { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="Address"]'/>
        public DemographyAddress Address { get; set; }
        /// <include file='../Datasets.doc.xml' path='Datasets/Demography/Field[@name="PreviousAddress"]'/>
        public DemographyAddress PreviousAddress { get; set; }
        
        public const int MinimumYearOfBirth = 1914;
        public const int MaximumYearOfBirth = 2014;

        /// <summary>
        /// The collection to which the patient belongs, may be null
        /// </summary>
        private PersonCollection _parent;

        /// <summary>
        /// Generates a new random person using the seeded random.  This overload ensures that the <see cref="Person"/> generated
        /// does not already exist in the <paramref name="collection"/> (in terms of CHI / ANOCHI numbers).
        /// </summary>
        /// <param name="r"></param>
        /// <param name="collection"></param>
        public Person(Random r,PersonCollection collection):this(r)
        {
            _parent = collection;
        }

        /// <summary>
        /// Generates a new random person using the seeded random
        /// </summary>
        /// <param name="r"></param>
        public Person(Random r)
        {
            switch (r.Next(2))
            {
                case 0:
                    Gender = 'F';
                    break;
                case 1:
                    Gender = 'M';
                    break;
            }

            Forename = GetRandomForename(r);
            Surname = GetRandomSurname(r);

            DateOfBirth = DataGenerator.GetRandomDate(new DateTime(MinimumYearOfBirth,1,1),new DateTime(MaximumYearOfBirth,1,1),r);
            
            //1 in 10 patients is dead
            if (r.Next(10) == 0)
                DateOfDeath = DataGenerator.GetRandomDateAfter(DateOfBirth, r);
            else
                DateOfDeath = null;

            CHI = GetNovelCHI(r);

            ANOCHI = GetNovelANOCHI(r);

            Address = new DemographyAddress(r);

            //one in 10 people doesn't have a previous address
            if(r.Next(10) != 0)
                PreviousAddress = new DemographyAddress(r);
        }

        /// <summary>
        /// Returns a random first name based on the <see cref="Person.Gender"/>
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string GetRandomForename(Random r)
        {
            return Gender == 'F' ? CommonGirlForenames[r.Next(100)] : CommonBoyForenames[r.Next(100)];
        }

        /// <summary>
        /// Returns a random date after the patients date of birth (and before their death if they are dead).
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public DateTime GetRandomDateDuringLifetime(Random r)
        {
            if (DateOfDeath == null)
                return DataGenerator.GetRandomDateAfter(DateOfBirth, r);

            return DataGenerator.GetRandomDate(DateOfBirth, (DateTime)DateOfDeath, r);
        }

        /// <summary>
        /// Returns a random surname from a list of common surnames
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string GetRandomSurname(Random r)
        {
            return CommonSurnames[r.Next(100)];
        }
        
        /// <summary>
        /// If the person died before onDate it returns NULL (as of onDate we did not know when the person would die).  if onDate is > date of death it 
        /// returns the date of death (we knew when they died - you cannot predict the future but you can remember the past)
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public DateTime? GetDateOfDeathOrNullOn(DateTime onDate)
        {
            //patient is alive today
            if (DateOfDeath == null)
                return null;
            
            //retrospective
            if (onDate >= DateOfDeath)
                return DateOfDeath;

            //we cannot predict the future, they are dead today but you are pretending the date is onDate
            return null;
        }

        /// <summary>
        /// Returns a new random ANOCHI which does not exist in <see cref="_parent"/> (if we have one)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private string GetNovelANOCHI(Random r)
        {
            string anochi;
            anochi = GenerateANOCHI(r);

            while(_parent != null && _parent.AlreadyGeneratedANOCHIs.Contains(anochi))
                anochi = GenerateANOCHI(r);            
            
            return anochi;

        }

        /// <summary>
        /// Returns a new random CHI which does not exist in <see cref="_parent"/> (if we have one)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private string GetNovelCHI(Random r)
        {
            string chi;
            chi = GetRandomCHI(r);

            while(_parent != null && _parent.AlreadyGeneratedCHIs.Contains(chi))
                chi = GetRandomCHI(r);        
            
            return chi;
        }

        private string GenerateANOCHI(Random r)
        {
            var toreturn = new StringBuilder();

            for (int i = 0; i < 10; i++)
                toreturn.Append(r.Next(10));

            toreturn.Append("_A");
            return toreturn.ToString();
        }

        /// <summary>
        /// Returns a randomly generated CHI number for the patient.  The first 6 digits will match the patients <see cref="DateOfBirth"/> and
        /// the second to last digit will match the <see cref="Gender"/>.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string GetRandomCHI( Random r)
        {
            string toreturn = DateOfBirth.ToString("ddMMyy" + r.Next(10, 99));

            int genderDigit = r.Next(10);

            switch (Gender)
            {
                //odd last number for girls
                case 'F' when genderDigit % 2 == 0:
                    genderDigit = 1;
                    break;
                //even last number for guys
                case 'M' when genderDigit % 2 == 1:
                    genderDigit = 2;
                    break;
            }

            int checkDigit = r.Next(0, 9);

            return toreturn + genderDigit + checkDigit;
        }

        public bool Equals(Person other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Forename, other.Forename) && string.Equals(Surname, other.Surname) && string.Equals(CHI, other.CHI) && string.Equals(ANOCHI, other.ANOCHI) && Gender == other.Gender && Equals(Address, other.Address) && Equals(PreviousAddress, other.PreviousAddress);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Forename != null ? Forename.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Surname != null ? Surname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CHI != null ? CHI.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ANOCHI != null ? ANOCHI.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Gender.GetHashCode();
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PreviousAddress != null ? PreviousAddress.GetHashCode() : 0);
                return hashCode;
            }
        }

        private static readonly string[] CommonGirlForenames = new[]
        {
            "AMELIA",
            "OLIVIA",
            "EMILY",
            "AVA",
            "ISLA",
            "JESSICA",
            "POPPY",
            "ISABELLA",
            "SOPHIE",
            "MIA",
            "RUBY",
            "LILY",
            "GRACE",
            "EVIE",
            "SOPHIA",
            "ELLA",
            "SCARLETT",
            "CHLOE",
            "ISABELLE",
            "FREYA",
            "CHARLOTTE",
            "SIENNA",
            "DAISY",
            "PHOEBE",
            "MILLIE",
            "EVA",
            "ALICE",
            "LUCY",
            "FLORENCE",
            "SOFIA",
            "LAYLA",
            "LOLA",
            "HOLLY",
            "IMOGEN",
            "MOLLY",
            "MATILDA",
            "LILLY",
            "ROSIE",
            "ELIZABETH",
            "ERIN",
            "MAISIE",
            "LEXI",
            "ELLIE",
            "HANNAH",
            "EVELYN",
            "ABIGAIL",
            "ELSIE",
            "SUMMER",
            "MEGAN",
            "JASMINE",
            "MAYA",
            "AMELIE",
            "LACEY",
            "WILLOW",
            "EMMA",
            "BELLA",
            "ELEANOR",
            "ESME",
            "ELIZA",
            "GEORGIA",
            "HARRIET",
            "GRACIE",
            "ANNABELLE",
            "EMILIA",
            "AMBER",
            "IVY",
            "BROOKE",
            "ROSE",
            "ANNA",
            "ZARA",
            "LEAH",
            "MOLLIE",
            "MARTHA",
            "FAITH",
            "HOLLIE",
            "AMY",
            "BETHANY",
            "VIOLET",
            "KATIE",
            "MARYAM",
            "FRANCESCA",
            "JULIA",
            "MARIA",
            "DARCEY",
            "ISABEL",
            "TILLY",
            "MADDISON",
            "VICTORIA",
            "ISOBEL",
            "NIAMH",
            "SKYE",
            "MADISON",
            "DARCY",
            "AISHA",
            "BEATRICE",
            "SARAH",
            "ZOE",
            "PAIGE",
            "HEIDI",
            "LYDIA",
            "SARA"
        };

        private static readonly string[] CommonBoyForenames = new[]
        {
            "OLIVER",
            "JACK",
            "HARRY",
            "JACOB",
            "CHARLIE",
            "THOMAS",
            "OSCAR",
            "WILLIAM",
            "JAMES",
            "GEORGE",
            "ALFIE",
            "JOSHUA",
            "NOAH",
            "ETHAN",
            "MUHAMMAD",
            "ARCHIE",
            "LEO",
            "HENRY",
            "JOSEPH",
            "SAMUEL",
            "RILEY",
            "DANIEL",
            "MOHAMMED",
            "ALEXANDER",
            "MAX",
            "LUCAS",
            "MASON",
            "LOGAN",
            "ISAAC",
            "BENJAMIN",
            "DYLAN",
            "JAKE",
            "EDWARD",
            "FINLEY",
            "FREDDIE",
            "HARRISON",
            "TYLER",
            "SEBASTIAN",
            "ZACHARY",
            "ADAM",
            "THEO",
            "JAYDEN",
            "ARTHUR",
            "TOBY",
            "LUKE",
            "LEWIS",
            "MATTHEW",
            "HARVEY",
            "HARLEY",
            "DAVID",
            "RYAN",
            "TOMMY",
            "MICHAEL",
            "REUBEN",
            "NATHAN",
            "BLAKE",
            "MOHAMMAD",
            "JENSON",
            "BOBBY",
            "LUCA",
            "CHARLES",
            "FRANKIE",
            "DEXTER",
            "KAI",
            "ALEX",
            "CONNOR",
            "LIAM",
            "JAMIE",
            "ELIJAH",
            "STANLEY",
            "LOUIE",
            "JUDE",
            "CALLUM",
            "HUGO",
            "LEON",
            "ELLIOT",
            "LOUIS",
            "THEODORE",
            "GABRIEL",
            "OLLIE",
            "AARON",
            "FREDERICK",
            "EVAN",
            "ELLIOTT",
            "OWEN",
            "TEDDY",
            "FINLAY",
            "CALEB",
            "IBRAHIM",
            "RONNIE",
            "FELIX",
            "AIDEN",
            "CAMERON",
            "AUSTIN",
            "KIAN",
            "RORY",
            "SETH",
            "ROBERT",
            "MAVERIC MCNULTY", //these two deliberately have spaces in them to break validation rules in the documentation
            "FRANKIE HOLLYWOOD"
        };


        private static readonly string[] CommonSurnames = new[]
        {
            "Smith",
            "Jones",
            "Taylor",
            "Williams",
            "Brown",
            "Davies",
            "Evans",
            "Wilson",
            "Thomas",
            "Roberts",
            "Johnson",
            "Lewis",
            "Walker",
            "Robinson",
            "Wood",
            "Thompson",
            "White",
            "Watson",
            "Jackson",
            "Wright",
            "Green",
            "Harris",
            "Cooper",
            "King",
            "Lee",
            "Martin",
            "Clarke",
            "James",
            "Morgan",
            "Hughes",
            "Edwards",
            "Hill",
            "Moore",
            "Clark",
            "Harrison",
            "Scott",
            "Young",
            "Morris",
            "Hall",
            "Ward",
            "Turner",
            "Carter",
            "Phillips",
            "Mitchell",
            "Patel",
            "Adams",
            "Campbell",
            "Anderson",
            "Allen",
            "Cook",
            "Bailey",
            "Parker",
            "Miller",
            "Davis",
            "Murphy",
            "Price",
            "Bell",
            "Baker",
            "Griffith",
            "Kelly",
            "Simpson",
            "Marshall",
            "Collins",
            "Bennett",
            "Cox",
            "Richards",
            "Fox",
            "Gray",
            "Rose",
            "Chapman",
            "Hunt",
            "Robertson",
            "Shaw",
            "Reynolds",
            "Lloyd",
            "Ellis",
            "Richards",
            "Russell",
            "Wilkinson",
            "Khan",
            "Graham",
            "Stewart",
            "Reid",
            "Murray",
            "Powell",
            "Palmer",
            "Holmes",
            "Rogers",
            "Stevens",
            "Walsh",
            "Hunter",
            "Thomson",
            "Matthews",
            "Ross",
            "Owen",
            "Mason",
            "Knight",
            "Kennedy",
            "Butler",
            "Saunders"
        };
    }
}
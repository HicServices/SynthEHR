// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MathNet.Numerics.Distributions;

namespace BadMedicine.Datasets;

/// <summary>
/// Base class for all randomly generated datasets.  Handles generating random datatypes and writing
/// out to csv etc.
/// </summary>
public abstract class DataGenerator : IDataGenerator
{
    /// <inheritdoc/>
    public event EventHandler<RowsGeneratedEventArgs> RowsGenerated;

    /// <summary>
    /// Use for all your random needs to ensure Seed injection support.
    /// </summary>
    protected Random r;

    /// <summary>
    /// Use this instead of DateTime.Now to ensure reproducible datasets when using the same seeded random
    /// </summary>
    public static DateTime Now { get; } = new(2019, 7, 5, 23, 59, 59);

    /// <summary>
    /// Creates a new instance which uses the provided <paramref name="rand"/> as a seed for generating data
    /// </summary>
    /// <param name="rand"></param>
    protected DataGenerator(Random rand)
    {
        r = rand;
        _normalDist = new Normal(0, 0.3, r);
    }

    /// <summary>
    /// Returns true if it is eligible to generate rows in the dataset for the given <paramref name="p"/>
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public virtual bool IsEligible(Person p)
    {
        return true;
    }

    /// <inheritdoc/>
    public void GenerateTestDataFile(IPersonCollection cohort, FileInfo target, int numberOfRecords)
    {
        using var sw = new StreamWriter(target.FullName);
        WriteHeaders(sw);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using (var writer = new CsvWriter(sw, CultureInfo.CurrentCulture))
        {
            int linesWritten;
            for (linesWritten = 0; linesWritten < numberOfRecords; linesWritten++)
            {
                foreach (var o in GenerateTestDataRow(GetRandomEligiblePerson(cohort.People, r)))
                    writer.WriteField(o);

                writer.NextRecord();

                if (linesWritten % 1000 != 0) continue;

                RowsGenerated?.Invoke(this, new RowsGeneratedEventArgs(linesWritten + 1, stopwatch.Elapsed, false));
                sw.Flush();//flush every 1000
            }

            //tell them about the last line written
            RowsGenerated?.Invoke(this, new RowsGeneratedEventArgs(linesWritten, stopwatch.Elapsed, true));
        }

        stopwatch.Stop();
    }


    /// <summary>
    /// Returns a random <see cref="Person"/> that <see cref="IsEligible"/> for this dataset. If nobody is eligible then returns a random person.
    /// </summary>
    /// <param name="people"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public Person GetRandomEligiblePerson(Person[] people, Random r)
    {
        if (people.Length == 0)
            throw new ArgumentException("Must pass at least 1 person to GetRandomEligiblePerson", nameof(people));

        var eligible = people.Where(IsEligible).ToArray();

        return
            eligible.Length != 0 ? eligible[r.Next(eligible.Length)]
                //if nobody is eligible then everyone is!
                : people[r.Next(people.Length)];
    }

    /// <inheritdoc/>
    public virtual DataTable GetDataTable(IPersonCollection cohort, int numberOfRecords)
    {
        var dt = new DataTable();

        foreach (var h in GetHeaders())
            dt.Columns.Add(h);

        for (var i = 0; i < numberOfRecords; i++)
            dt.Rows.Add(GenerateTestDataRow(GetRandomEligiblePerson(cohort.People, r)));

        return dt;
    }

    /// <inheritdoc/>
    public abstract object[] GenerateTestDataRow(Person p);

    /// <inheritdoc/>
    protected abstract string[] GetHeaders();

    /// <summary>
    /// Outputs the top line of the CSV (column headers)
    /// </summary>
    /// <param name="sw"></param>
    private void WriteHeaders(StreamWriter sw)
    {
        sw.WriteLine(string.Join(",", GetHeaders()));
    }

    private readonly Normal _normalDist;

    /// <summary>
    /// Concatenates between <paramref name="min"/> and <paramref name="max"/> calls to the <paramref name="generator"/>
    /// </summary>
    /// <param name="r"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="generator"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    protected static string Concat(Random r, int min, int max, Func<string> generator, string separator)
    {
        var sb = new StringBuilder();

        var to = r.Next(min, max);
        for (var i = 0; i < to; i++)
            sb.Append(generator() + separator);

        return sb.ToString().Trim();
    }



    /// <summary>
    /// returns random number between -1 and 1 with normal distribution (more numbers near 0 than near 1/-1).  The standard
    /// deviation is 0.3.  Any values outside the range (5 in 10,000 or so) are adjusted to -1 or 1.
    /// </summary>
    /// <returns></returns>
    public double GetGaussian()
    {
        return Math.Min(Math.Max(-1, _normalDist.Sample()), 1);
    }

    /// <summary>
    /// Returns a random date inclusive of the lower bound and exclusive of the upper bound.
    /// </summary>
    /// <param name="from">inclusive lower bound</param>
    /// <param name="to">exclusive upper bound</param>
    /// <param name="r">seeded random</param>
    /// <returns></returns>
    public static DateTime GetRandomDate(DateTime from, DateTime to, Random r)
    {
        var range = to - from;

        var randTimeSpan = new TimeSpan((long)(r.NextDouble() * range.Ticks));

        return from + randTimeSpan;
    }

    /// <summary>
    /// Returns a date after (or on) <paramref name="afterDate"/>.  In order to preserve randomisation seeding a constant
    /// value in 2019 is used instead of DateTime.Now (ensures that data generated doesn't vary with the same seed).
    /// </summary>
    /// <param name="afterDate"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static DateTime GetRandomDateAfter(DateTime afterDate, Random r)
    {
        return GetRandomDate(afterDate, Now, r);
    }

    /// <summary>
    /// returns random number between lowerBoundary and upperBoundary with a gaussian distribution around the middle
    /// </summary>
    /// <param name="upperBoundary">Highest number that should be generated</param>
    /// <param name="digits">The number of decimal places to have in the number</param>
    /// <param name="lowerBoundary">Lowest number that should be generated</param>
    /// <returns></returns>
    public double GetGaussian(double lowerBoundary, double upperBoundary, int digits = 2)
    {
        if (upperBoundary < lowerBoundary)
            throw new ArgumentException("lower must be lower than upper boundary");

        var distributionZeroToOne = (GetGaussian() + 1) / 2;

        var range = upperBoundary - lowerBoundary;
        return Math.Round(distributionZeroToOne * range + lowerBoundary, digits);
    }

    /// <inheritdoc cref="GetGaussian(double,double,int)"/>
    protected int GetGaussianInt(double lowerBoundary, double upperBoundary)
    {
        return (int)GetGaussian(lowerBoundary, upperBoundary);
    }


    /// <summary>
    /// returns <paramref name="swapFor"/> if <paramref name="swapIfIn"/> contains the input <paramref name="randomInt"/> (otherwise returns the input)
    /// </summary>
    /// <param name="randomInt"></param>
    /// <param name="swapIfIn"></param>
    /// <param name="swapFor"></param>
    /// <returns></returns>
    protected static int Swap(int randomInt, IEnumerable<int> swapIfIn, int swapFor)
    {
        return swapIfIn.Contains(randomInt) ? swapFor : randomInt;
    }

    /// <summary>
    /// Returns a random double or string value that represents a double e.g. "2.1".  In future this might return
    /// floats with e specification e.g. "1.7E+3"
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static object GetRandomDouble(Random r) =>
        r.Next(0, 3) switch
        {
            0 => r.Next(100),
            1 => Math.Round(r.NextDouble(), 2),
            2 => $"{r.Next(10)}.{r.Next(10)}",
            _ => throw new NotImplementedException()
        };

    /// <summary>
    /// Returns a random 'GPCode'.  This is a letter followed by up to 3 digits.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static string GetRandomGPCode(Random r)
    {
        return GetRandomLetter(true, r).ToString() + r.Next(0, 999);
    }

    /// <summary>
    /// Gets a random letter (A - Z)
    /// </summary>
    /// <param name="upperCase"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    protected static char GetRandomLetter(bool upperCase, Random r) => (char)((upperCase ? 'A' : 'a') + r.Next(0, 26));

    /// <summary>
    /// Returns a random 'status' for a CHI or sometimes null.  Values include 'C' (current), 'H' (historical), 'L'(legacy?) and 'R'(retracted?)
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    protected static object GetRandomCHIStatus(Random r) =>
        r.Next(0, 5) switch
        {
            0 => 'C',
            1 => 'H',
            2 => null,
            3 => 'L',
            4 => 'R',
            _ => throw new InvalidOperationException("Random violated parameter constraints")
        };

    /// <summary>
    /// Reads an embedded resource csv file that sits side by side (in terms of namespace) with the <paramref name="requestingType"/>.  Will also work
    /// if you have an embedded resource file called "Aggregates.zip" which contains the <paramref name="resourceFileName"/>.
    /// 
    /// </summary>
    /// <param name="requestingType"></param>
    /// <param name="resourceFileName"></param>
    /// <param name="dt">Optional - provide if you want to strongly type certain Columns.  New columns will be added to this table
    /// if unmatched columns are read from the csv.</param>
    /// <returns></returns>
    public static DataTable EmbeddedCsvToDataTable(Type requestingType, string resourceFileName, DataTable dt = null)
    {
        var lookup = GetResourceStream(requestingType, resourceFileName) ?? throw new Exception($"Could not find embedded resource file {resourceFileName}");
        var toReturn = dt ?? new DataTable();

        using var r = new CsvReader(new StreamReader(lookup), new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," });
        r.Read();
        r.ReadHeader();

        foreach (var header in (r.HeaderRecord ?? []).Where(header => !toReturn.Columns.Contains(header)))
            toReturn.Columns.Add(header);

        r.Read();

        do
        {
            var row = toReturn.Rows.Add();
            foreach (DataColumn col in toReturn.Columns)
            {
                row[col] = r[col.ColumnName];
            }
        } while (r.Read());

        return toReturn;
    }

    private static Stream GetResourceStream(Type requestingType, string resourceFileName)
    {
        var toFind = $"{requestingType.Namespace}.{resourceFileName}";
        //is there an unzipped resource available?
        var toReturn = requestingType.Assembly.GetManifestResourceStream(toFind);

        // if so, return it
        if (toReturn != null) return toReturn;

        // if not, see if there is a zipped resource file in the namespaces
        var toFindZip = $"{requestingType.Namespace}.Aggregates.zip";
        var zip = requestingType.Assembly.GetManifestResourceStream(toFindZip);

        var memoryStream = new MemoryStream();

        //containing a file named resourceFileNamed
        if (zip == null) return null;

        using var archive = new ZipArchive(zip);
        var entry = archive.GetEntry(resourceFileName);
        if (entry == null) return null;

        using var s = entry.Open();
        s.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// Returns a random sentence.  There are 391 available.  They were created by https://randomwordgenerator.com/sentence.php
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    protected static string GetRandomSentence(Random r)
    {
        return r.Next(391) switch
        {
            0 => "A mad prize ghosts the attractive romantic.",
            1 => "I often see the time 11:11 or 12:34 on clocks.",
            2 => "Malls are great places to shop; I can find everything I need under one roof.",
            3 => "Christmas is coming.",
            4 => "I will never be this young again. Ever. Oh damn' I just got older.",
            5 => "This is a Japanese doll.",
            6 => "We have never been to Asia, nor have we visited Africa.",
            7 => "She was too short to see over the fence.",
            8 => "Hurry!",
            9 => "If I don't like something, I'll stay away from it.",
            10 => "Wednesday is hump day, but has anyone asked the camel if he's happy about it?",
            11 => "She folded her handkerchief neatly.",
            12 => "I checked to make sure that he was still alive.",
            13 => "He didn't want to go to the dentist, yet he went anyway.",
            14 => "There was no ice cream in the freezer, nor did they have money to go to the store.",
            15 =>
                "Sometimes, all you need to do is completely make an ass of yourself and laugh it off to realise that life isn't so bad after all.",
            16 =>
                "If the Easter Bunny and the Tooth Fairy had babies would they take your teeth and leave chocolate for you?",
            17 => "Cats are good pets, for they are clean and are not noisy.",
            18 => "The body may perhaps compensates for the loss of a true metaphysics.",
            19 => "Please wait outside of the house.",
            20 => "The mysterious diary records the voice.",
            21 => "There were white out conditions in the town; subsequently, the roads were impassable.",
            22 => "I love eating toasted cheese and tuna sandwiches.",
            23 => "Two seats were vacant.",
            24 => "The clock within this blog and the clock on my laptop are 1 hour different from each other.",
            25 => "She did her best to help him.",
            26 => "We need to rent a room for our party.",
            27 =>
                "Someone I know recently combined Maple Syrup & buttered Popcorn thinking it would taste like caramel popcorn. It didn't and they don't recommend anyone else do it either.",
            28 => "The river stole the gods.",
            29 => "Joe made the sugar cookies; Susan decorated them.",
            30 => "He told us a very exciting adventure story.",
            31 => "He said he was not there yesterday; however, many people saw him there.",
            32 => "I really want to go to work, but I am too sick to drive.",
            33 => "A glittering gem is not enough.",
            34 => "Abstraction is often one floor above you.",
            35 =>
                "Sometimes it is better to just walk away from things and go back to them later when you're in a better frame of mind.",
            36 => "Mary plays the piano.",
            37 => "She did not cheat on the test, for it was not the right thing to do.",
            38 => "I would have gotten the promotion, but my attendance wasn't good enough.",
            39 => "I want more detailed information.",
            40 => "It was getting dark, and we weren't there yet.",
            41 => "She borrowed the book from him many years ago and hasn't yet returned it.",
            42 =>
                "I was very proud of my nickname throughout high school but today- I couldn't be any different to what my nickname was.",
            43 => "Wow, does that work?",
            44 => "When I was little I had a car door slammed shut on my hand. I still remember it quite vividly.",
            45 => "The waves were crashing on the shore; it was a lovely sight.",
            46 => "If Purple People Eaters are real' where do they find purple people to eat?",
            47 => "Where do random thoughts come from?",
            48 => "They got there early, and they got really good seats.",
            49 => "Everyone was busy, so I went to the movie alone.",
            50 => "I am never at home on Sundays.",
            51 => "Should we start class now, or should we wait for everyone to get here?",
            52 => "The quick brown fox jumps over the lazy dog.",
            53 => "A song can make or ruin a person's day if they let it get to them.",
            54 => "I want to buy a onesie' but know it won't suit me.",
            55 => "Italy is my favorite country; in fact, I plan to spend two weeks there next year.",
            56 => "I hear that Nancy is very pretty.",
            57 =>
                "What was the person thinking when they discovered cow's milk was fine for human consumption' and why did they do it in the first place!?",
            58 => "She advised him to come back at once.",
            59 => "He ran out of money, so he had to stop playing poker.",
            60 => "My Mum tries to be cool by saying that she likes all the same things that I do.",
            61 => "The sky is clear; the stars are twinkling.",
            62 =>
                "She works two jobs to make ends meet; at least, that was her reason for not having time to join us.",
            63 => "I'd rather be a bird than a fish.",
            64 => "He turned in the research paper on Friday; otherwise, he would have not passed the class.",
            65 => "The memory we used to share is no longer coherent.",
            66 => "Lets all be unique together until we realise we are all the same.",
            67 => "I am happy to take your donation; any amount will be greatly appreciated.",
            68 => "The old apple revels in its authority.",
            69 => "Let me help you with your baggage.",
            70 => "Sixty-Four comes asking for bread.",
            71 => "I am counting my calories, yet I really want dessert.",
            72 => "How was the math test?",
            73 => "If you like tuna and tomato sauce- try combining the two. It's really not as bad as it sounds.",
            74 =>
                "Last Friday in three week's time I saw a spotted striped blue worm shake hands with a legless lizard.",
            75 => "She wrote him a long letter, but he didn't read it.",
            76 => "Don't step on the broken glass.",
            77 => "Check back tomorrow; I will see if the book has arrived.",
            78 => "I currently have 4 windows open up' and I don't know why.",
            79 => "Tom got a small piece of pie.",
            80 => "Is it free?",
            81 => "She only paints with bold colors; she does not like pastels.",
            82 => "Yeah, I think it's a good environment for learning English.",
            83 => "This is the last random sentence I will be writing and I am going to stop mid-sent",
            84 => "We have a lot of rain in June.",
            85 => "She always speaks to him in a loud voice.",
            86 => "The lake is a long way from here.",
            87 => "Writing a list of random sentences is harder than I initially thought it would be.",
            88 => "I think I will buy the red car, or I will lease the blue one.",
            89 => "A purple pig and a green donkey flew a kite in the middle of the night and ended up sunburnt.",
            90 => "The stranger officiates the meal.",
            91 => "The shooter says goodbye to his love.",
            92 => "The book is in front of the table.",
            93 => "Rock music approaches at high velocity.",
            94 => "He told us a very exciting adventure story.",
            95 => "We have a lot of rain in June.",
            96 => "Abstraction is often one floor above you.",
            97 => "I am happy to take your donation; any amount will be greatly appreciated.",
            98 => "I hear that Nancy is very pretty.",
            99 => "I want more detailed information.",
            100 =>
                "Sometimes, all you need to do is completely make an ass of yourself and laugh it off to realise that life isn't so bad after all.",
            101 => "Italy is my favorite country; in fact, I plan to spend two weeks there next year.",
            102 => "I currently have 4 windows open up' and I don't know why.",
            103 => "The shooter says goodbye to his love.",
            104 => "Everyone was busy, so I went to the movie alone.",
            105 => "She was too short to see over the fence.",
            106 => "I think I will buy the red car, or I will lease the blue one.",
            107 => "Yeah, I think it's a good environment for learning English.",
            108 => "The book is in front of the table.",
            109 => "Writing a list of random sentences is harder than I initially thought it would be.",
            110 => "The clock within this blog and the clock on my laptop are 1 hour different from each other.",
            111 => "I am never at home on Sundays.",
            112 => "The quick brown fox jumps over the lazy dog.",
            113 => "I love eating toasted cheese and tuna sandwiches.",
            114 => "How was the math test?",
            115 => "Rock music approaches at high velocity.",
            116 => "She advised him to come back at once.",
            117 => "There were white out conditions in the town; subsequently, the roads were impassable.",
            118 => "I am counting my calories, yet I really want dessert.",
            119 => "She did her best to help him.",
            120 => "The waves were crashing on the shore; it was a lovely sight.",
            121 => "The lake is a long way from here.",
            122 => "Lets all be unique together until we realise we are all the same.",
            123 => "Let me help you with your baggage.",
            124 =>
                "Someone I know recently combined Maple Syrup & buttered Popcorn thinking it would taste like caramel popcorn. It didn't and they don't recommend anyone else do it either.",
            125 => "Christmas is coming.",
            126 => "The stranger officiates the meal.",
            127 => "Joe made the sugar cookies; Susan decorated them.",
            128 => "I often see the time 11:11 or 12:34 on clocks.",
            129 => "Don't step on the broken glass.",
            130 => "The sky is clear; the stars are twinkling.",
            131 => "There was no ice cream in the freezer, nor did they have money to go to the store.",
            132 => "If you like tuna and tomato sauce- try combining the two. It's really not as bad as it sounds.",
            133 => "If Purple People Eaters are real' where do they find purple people to eat?",
            134 => "It was getting dark, and we weren't there yet.",
            135 => "Where do random thoughts come from?",
            136 => "The river stole the gods.",
            137 =>
                "Last Friday in three week's time I saw a spotted striped blue worm shake hands with a legless lizard.",
            138 => "Sixty-Four comes asking for bread.",
            139 => "When I was little I had a car door slammed shut on my hand. I still remember it quite vividly.",
            140 => "He turned in the research paper on Friday; otherwise, he would have not passed the class.",
            141 =>
                "She works two jobs to make ends meet; at least, that was her reason for not having time to join us.",
            142 =>
                "What was the person thinking when they discovered cow's milk was fine for human consumption' and why did they do it in the first place!?",
            143 => "He said he was not there yesterday; however, many people saw him there.",
            144 => "This is the last random sentence I will be writing and I am going to stop mid-sent",
            145 => "Check back tomorrow; I will see if the book has arrived.",
            146 => "I really want to go to work, but I am too sick to drive.",
            147 => "Mary plays the piano.",
            148 => "Should we start class now, or should we wait for everyone to get here?",
            149 => "They got there early, and they got really good seats.",
            150 => "A glittering gem is not enough.",
            151 => "She only paints with bold colors; she does not like pastels.",
            152 => "The memory we used to share is no longer coherent.",
            153 => "If I don't like something, I'll stay away from it.",
            154 => "A song can make or ruin a person's day if they let it get to them.",
            155 => "My Mum tries to be cool by saying that she likes all the same things that I do.",
            156 => "She borrowed the book from him many years ago and hasn't yet returned it.",
            157 => "Hurry!",
            158 => "I checked to make sure that he was still alive.",
            159 => "Two seats were vacant.",
            160 => "This is a Japanese doll.",
            161 => "She folded her handkerchief neatly.",
            162 => "He didn't want to go to the dentist, yet he went anyway.",
            163 => "I want to buy a onesie' but know it won't suit me.",
            164 => "Tom got a small piece of pie.",
            165 => "Please wait outside of the house.",
            166 => "He ran out of money, so he had to stop playing poker.",
            167 => "Wow, does that work?",
            168 => "I'd rather be a bird than a fish.",
            169 => "She wrote him a long letter, but he didn't read it.",
            170 => "We need to rent a room for our party.",
            171 => "She always speaks to him in a loud voice.",
            172 => "Malls are great places to shop; I can find everything I need under one roof.",
            173 => "Cats are good pets, for they are clean and are not noisy.",
            174 => "We have never been to Asia, nor have we visited Africa.",
            175 => "Is it free?",
            176 => "I will never be this young again. Ever. Oh damn' I just got older.",
            177 =>
                "I was very proud of my nickname throughout high school but today- I couldn't be any different to what my nickname was.",
            178 => "The body may perhaps compensates for the loss of a true metaphysics.",
            179 => "The mysterious diary records the voice.",
            180 => "I would have gotten the promotion, but my attendance wasn't good enough.",
            181 => "Wednesday is hump day, but has anyone asked the camel if he's happy about it?",
            182 =>
                "If the Easter Bunny and the Tooth Fairy had babies would they take your teeth and leave chocolate for you?",
            183 =>
                "Sometimes it is better to just walk away from things and go back to them later when you're in a better frame of mind.",
            184 => "She did not cheat on the test, for it was not the right thing to do.",
            185 => "A purple pig and a green donkey flew a kite in the middle of the night and ended up sunburnt.",
            186 => "The old apple revels in its authority.",
            187 => "Tom got a small piece of pie.",
            188 => "I will never be this young again. Ever. Oh damn' I just got older.",
            189 => "Should we start class now, or should we wait for everyone to get here?",
            190 => "He told us a very exciting adventure story.",
            191 => "They got there early, and they got really good seats.",
            192 => "The clock within this blog and the clock on my laptop are 1 hour different from each other.",
            193 => "Two seats were vacant.",
            194 =>
                "What was the person thinking when they discovered cow's milk was fine for human consumption' and why did they do it in the first place!?",
            195 =>
                "Last Friday in three week's time I saw a spotted striped blue worm shake hands with a legless lizard.",
            196 => "Please wait outside of the house.",
            197 => "Everyone was busy, so I went to the movie alone.",
            198 => "Yeah, I think it's a good environment for learning English.",
            199 =>
                "Someone I know recently combined Maple Syrup & buttered Popcorn thinking it would taste like caramel popcorn. It didn't and they don't recommend anyone else do it either.",
            200 => "There was no ice cream in the freezer, nor did they have money to go to the store.",
            201 => "My Mum tries to be cool by saying that she likes all the same things that I do.",
            202 => "We have never been to Asia, nor have we visited Africa.",
            203 => "Malls are great places to shop; I can find everything I need under one roof.",
            204 => "She borrowed the book from him many years ago and hasn't yet returned it.",
            205 => "I want more detailed information.",
            206 => "It was getting dark, and we weren't there yet.",
            207 => "A purple pig and a green donkey flew a kite in the middle of the night and ended up sunburnt.",
            208 => "The body may perhaps compensates for the loss of a true metaphysics.",
            209 => "He turned in the research paper on Friday; otherwise, he would have not passed the class.",
            210 => "How was the math test?",
            211 => "She folded her handkerchief neatly.",
            212 => "She only paints with bold colors; she does not like pastels.",
            213 => "This is the last random sentence I will be writing and I am going to stop mid-sent",
            214 => "The sky is clear; the stars are twinkling.",
            215 => "I love eating toasted cheese and tuna sandwiches.",
            216 => "Hurry!",
            217 => "The old apple revels in its authority.",
            218 => "I'd rather be a bird than a fish.",
            219 => "If I don't like something, I'll stay away from it.",
            220 => "I currently have 4 windows open up' and I don't know why.",
            221 => "Abstraction is often one floor above you.",
            222 => "Wow, does that work?",
            223 => "The book is in front of the table.",
            224 => "Writing a list of random sentences is harder than I initially thought it would be.",
            225 =>
                "If the Easter Bunny and the Tooth Fairy had babies would they take your teeth and leave chocolate for you?",
            226 => "I checked to make sure that he was still alive.",
            227 => "She always speaks to him in a loud voice.",
            228 => "I am happy to take your donation; any amount will be greatly appreciated.",
            229 => "Wednesday is hump day, but has anyone asked the camel if he's happy about it?",
            230 => "Italy is my favorite country; in fact, I plan to spend two weeks there next year.",
            231 => "A glittering gem is not enough.",
            232 => "Joe made the sugar cookies; Susan decorated them.",
            233 => "The stranger officiates the meal.",
            234 => "He said he was not there yesterday; however, many people saw him there.",
            235 => "Cats are good pets, for they are clean and are not noisy.",
            236 => "If you like tuna and tomato sauce- try combining the two. It's really not as bad as it sounds.",
            237 =>
                "Sometimes, all you need to do is completely make an ass of yourself and laugh it off to realise that life isn't so bad after all.",
            238 => "Christmas is coming.",
            239 => "Let me help you with your baggage.",
            240 => "Sixty-Four comes asking for bread.",
            241 => "I hear that Nancy is very pretty.",
            242 => "There were white out conditions in the town; subsequently, the roads were impassable.",
            243 => "The river stole the gods.",
            244 => "He ran out of money, so he had to stop playing poker.",
            245 => "I am counting my calories, yet I really want dessert.",
            246 => "She did not cheat on the test, for it was not the right thing to do.",
            247 => "This is a Japanese doll.",
            248 => "She was too short to see over the fence.",
            249 => "Check back tomorrow; I will see if the book has arrived.",
            250 => "She advised him to come back at once.",
            251 => "Don't step on the broken glass.",
            252 => "I think I will buy the red car, or I will lease the blue one.",
            253 => "Where do random thoughts come from?",
            254 => "She did her best to help him.",
            255 =>
                "Sometimes it is better to just walk away from things and go back to them later when you're in a better frame of mind.",
            256 => "The quick brown fox jumps over the lazy dog.",
            257 => "A song can make or ruin a person's day if they let it get to them.",
            258 => "I am never at home on Sundays.",
            259 => "When I was little I had a car door slammed shut on my hand. I still remember it quite vividly.",
            260 => "I often see the time 11:11 or 12:34 on clocks.",
            261 => "The waves were crashing on the shore; it was a lovely sight.",
            262 => "We need to rent a room for our party.",
            263 => "He didn't want to go to the dentist, yet he went anyway.",
            264 => "We have a lot of rain in June.",
            265 => "The lake is a long way from here.",
            266 => "I really want to go to work, but I am too sick to drive.",
            267 =>
                "She works two jobs to make ends meet; at least, that was her reason for not having time to join us.",
            268 => "I want to buy a onesie' but know it won't suit me.",
            269 => "Mary plays the piano.",
            270 => "Is it free?",
            271 => "The mysterious diary records the voice.",
            272 => "Lets all be unique together until we realise we are all the same.",
            273 => "I would have gotten the promotion, but my attendance wasn't good enough.",
            274 => "The memory we used to share is no longer coherent.",
            275 => "She wrote him a long letter, but he didn't read it.",
            276 =>
                "I was very proud of my nickname throughout high school but today- I couldn't be any different to what my nickname was.",
            277 => "The shooter says goodbye to his love.",
            278 => "If Purple People Eaters are real' where do they find purple people to eat?",
            279 => "Rock music approaches at high velocity.",
            280 => "I often see the time 11:11 or 12:34 on clocks.",
            281 =>
                "What was the person thinking when they discovered cow's milk was fine for human consumption' and why did they do it in the first place!?",
            282 => "Christmas is coming.",
            283 => "A song can make or ruin a person's day if they let it get to them.",
            284 => "Where do random thoughts come from?",
            285 => "We have a lot of rain in June.",
            286 => "The memory we used to share is no longer coherent.",
            287 =>
                "If the Easter Bunny and the Tooth Fairy had babies would they take your teeth and leave chocolate for you?",
            288 => "She advised him to come back at once.",
            289 => "The mysterious diary records the voice.",
            290 => "Let me help you with your baggage.",
            291 => "Mary plays the piano.",
            292 => "He ran out of money, so he had to stop playing poker.",
            293 => "She only paints with bold colors; she does not like pastels.",
            294 => "Everyone was busy, so I went to the movie alone.",
            295 => "Sixty-Four comes asking for bread.",
            296 => "Check back tomorrow; I will see if the book has arrived.",
            297 => "The quick brown fox jumps over the lazy dog.",
            298 => "Abstraction is often one floor above you.",
            299 => "I want to buy a onesie' but know it won't suit me.",
            300 => "Should we start class now, or should we wait for everyone to get here?",
            301 => "Lets all be unique together until we realise we are all the same.",
            302 => "The shooter says goodbye to his love.",
            303 => "She borrowed the book from him many years ago and hasn't yet returned it.",
            304 => "I think I will buy the red car, or I will lease the blue one.",
            305 => "This is a Japanese doll.",
            306 => "The sky is clear; the stars are twinkling.",
            307 => "She wrote him a long letter, but he didn't read it.",
            308 =>
                "I was very proud of my nickname throughout high school but today- I couldn't be any different to what my nickname was.",
            309 =>
                "She works two jobs to make ends meet; at least, that was her reason for not having time to join us.",
            310 => "If Purple People Eaters are real' where do they find purple people to eat?",
            311 => "She folded her handkerchief neatly.",
            312 => "She was too short to see over the fence.",
            313 => "I am counting my calories, yet I really want dessert.",
            314 => "Joe made the sugar cookies; Susan decorated them.",
            315 => "A glittering gem is not enough.",
            316 => "My Mum tries to be cool by saying that she likes all the same things that I do.",
            317 => "I hear that Nancy is very pretty.",
            318 => "He turned in the research paper on Friday; otherwise, he would have not passed the class.",
            319 => "Please wait outside of the house.",
            320 => "The lake is a long way from here.",
            321 => "Hurry!",
            322 => "He said he was not there yesterday; however, many people saw him there.",
            323 => "I checked to make sure that he was still alive.",
            324 =>
                "Someone I know recently combined Maple Syrup & buttered Popcorn thinking it would taste like caramel popcorn. It didn't and they don't recommend anyone else do it either.",
            325 => "Wednesday is hump day, but has anyone asked the camel if he's happy about it?",
            326 => "I will never be this young again. Ever. Oh damn' I just got older.",
            327 => "He told us a very exciting adventure story.",
            328 => "This is the last random sentence I will be writing and I am going to stop mid-sent",
            329 => "They got there early, and they got really good seats.",
            330 => "Malls are great places to shop; I can find everything I need under one roof.",
            331 => "The waves were crashing on the shore; it was a lovely sight.",
            332 => "If you like tuna and tomato sauce- try combining the two. It's really not as bad as it sounds.",
            333 => "She did not cheat on the test, for it was not the right thing to do.",
            334 => "Don't step on the broken glass.",
            335 => "I currently have 4 windows open up' and I don't know why.",
            336 => "The clock within this blog and the clock on my laptop are 1 hour different from each other.",
            337 =>
                "Sometimes it is better to just walk away from things and go back to them later when you're in a better frame of mind.",
            338 => "Is it free?",
            339 => "We have never been to Asia, nor have we visited Africa.",
            340 => "There were white out conditions in the town; subsequently, the roads were impassable.",
            341 => "The old apple revels in its authority.",
            342 => "She always speaks to him in a loud voice.",
            343 => "We need to rent a room for our party.",
            344 => "The river stole the gods.",
            345 => "The body may perhaps compensates for the loss of a true metaphysics.",
            346 => "The book is in front of the table.",
            347 => "Tom got a small piece of pie.",
            348 => "Writing a list of random sentences is harder than I initially thought it would be.",
            349 => "It was getting dark, and we weren't there yet.",
            350 => "The stranger officiates the meal.",
            351 => "I would have gotten the promotion, but my attendance wasn't good enough.",
            352 => "I love eating toasted cheese and tuna sandwiches.",
            353 => "I want more detailed information.",
            354 => "There was no ice cream in the freezer, nor did they have money to go to the store.",
            355 => "He didn't want to go to the dentist, yet he went anyway.",
            356 => "Two seats were vacant.",
            357 =>
                "Last Friday in three week's time I saw a spotted striped blue worm shake hands with a legless lizard.",
            358 => "Rock music approaches at high velocity.",
            359 => "Yeah, I think it's a good environment for learning English.",
            360 =>
                "Sometimes, all you need to do is completely make an ass of yourself and laugh it off to realise that life isn't so bad after all.",
            361 => "Cats are good pets, for they are clean and are not noisy.",
            362 => "When I was little I had a car door slammed shut on my hand. I still remember it quite vividly.",
            363 => "If I don't like something, I'll stay away from it.",
            364 => "I really want to go to work, but I am too sick to drive.",
            365 => "A purple pig and a green donkey flew a kite in the middle of the night and ended up sunburnt.",
            366 => "I am happy to take your donation; any amount will be greatly appreciated.",
            367 => "I'd rather be a bird than a fish.",
            368 => "How was the math test?",
            369 => "Italy is my favorite country; in fact, I plan to spend two weeks there next year.",
            370 => "I am never at home on Sundays.",
            380 => "Wow, does that work?",
            390 => "She did her best to help him.",
            _ => null
        };
    }

    /// <summary>
    /// Writes out all lookup tables for all datasets.  These are tables which map codes to descriptions.
    /// </summary>
    /// <param name="dir"></param>
    public static void WriteLookups(DirectoryInfo dir)
    {

        File.WriteAllText(Path.Combine(dir.FullName, "z_chiStatus.csv"),
@"Code,Description
""C"",""The current record - it contains the approved CHI Number and which contains the GP with whom the patient is currently registered or was last registered with before transfer out of Scotland or death.""
""R"",""Redundant records are former records that were cancelled from use, mainly because the date of birth within the number was incorrect.""
""L"",""Local copy records are records copied by Trusts (usually) from the current record's CHI to another CHI, keeping the same CHI Number but allowing the Trust to access locally, without accessing other CHIs.  This has allowed Trusts to record a local temporary address on the CHI and is useful where the local CHI needs to be in step with a local demographic database.""
""Y"",""Local copy records are records copied by Trusts (usually) from the current record's CHI to another CHI, keeping the same CHI Number but allowing the Trust to access locally, without accessing other CHIs.  This has allowed Trusts to record a local temporary address on the CHI and is useful where the local CHI needs to be in step with a local demographic database.""
""H"",""Historical records are previously used records for the same patient on different CHIs.   These will have different CHI Numbers if transferred across Scotland before 1997, when they had to be registered again with a different CHI Number.  If transferred from 1997 onwards, they should have been transferred electronically from one CHI to another, keeping the same CHI Number. ""
""D"",""Deleted"""
);




        File.WriteAllText(Path.Combine(dir.FullName, "z_Healthboards.csv"),
@"
Code,Description
A,Ayrshire and Arran
B,Borders
C,Argyle and Clyde
D,State Hospital
E,England
F,Fife
G,Greater Glasgow
H,Highland
I,Inverness
J,Junderland
K,Krief
L,Lanarkshire
M,Metropolitan Area
N,Grampian
O,Orkney
P,Pitlochry
Q,Queensferry
R,Retired
S,Lothian
T,Tayside
U,Unknown
V,Forth Valley
W,Western Isles
X,Common Service Agency
Y,Dumfries and Galloway
Z,Shetland");


        File.WriteAllText(Path.Combine(dir.FullName, "z_PCStenosis.csv"),
@"Code,CodeValueDescription
1,Normal
2,Minimal disease
3,30% < 50%
4,50% < 70%
5,70% < 99%
6,Occluded
9,Unsure");

        File.WriteAllText(Path.Combine(dir.FullName, "z_ICStenosisLookup.csv"),
@"Code Type Description,Code,CodeValueDescription
%stenosis Carotid Artery Scan,1,Normal
%stenosis Carotid Artery Scan,2,Minimal disease
%stenosis Carotid Artery Scan,3,30% < 50%
%stenosis Carotid Artery Scan,4,50% < 70%
%stenosis Carotid Artery Scan,5,70% < 99%
%stenosis Carotid Artery Scan,6,Occluded
%stenosis Carotid Artery Scan,8,See report text
%stenosis Carotid Artery Scan,9,Unsure");

        File.WriteAllText(Path.Combine(dir.FullName, "z_VertflowLookup.csv"),
@"Code,CodeValueDescription
1,Cephalad
2,Reversed
3,Not Detected
4,See report text");

        File.WriteAllText(Path.Combine(dir.FullName, "z_StenosisLookup.csv"),
@"Code,CodeValueDescription
1,Normal
2,Minimum
3,Moderate
4,Severe
5,Occluded
9,Not seen
6,See report text");

        File.WriteAllText(Path.Combine(dir.FullName, "z_PlaqueLookup.csv"),
@"Code,CodeValueDescription
1,I
2,II
3,III
4,IV
9,Nil
8,Not applicable");

        File.WriteAllText(Path.Combine(dir.FullName, "z_Specialty.csv"),
@"Code,Specialty
A1,General Medicine
A11,Acute Medicine
A2,Cardiology
A21,Paediatric Cardiology
A3,Clinical Genetics
A4,Tropical Medicine
A5,Clinical Pharmacology & Therapeutics
A6,Infectious Diseases
A7,Dermatology
A8,Endocrinology & Diabetes
A81,Endocrinology
A82,Diabetes
A9,Gastroenterology
AA,Genito-Urinary Medicine
AB,Geriatric Medicine
AC,Homeopathy
AD,Medical Oncology
AF,Paediatrics
AFA,Community Child Health
AG,Renal Medicine
AH,Neurology
AJ,Integrative Care
AK,Occupational Medicine
AM,Palliative Medicine
AN,Public Health Medicine
AP,Rehabilitation Medicine
AQ,Respiratory Medicine
AR,Rheumatology
AS,Sport & Exercise Medicine
AT,Medical Ophthalmology
AV,Clinical Neurophysiology
AW,Allergy
C1,General Surgery
C11,General Surgery (excl Vascular, Maxillofacial)
C12,Vascular Surgery
C13,Oral and Maxillofacial Surgery
C14,Major Trauma
C2,Accident & Emergency
C3,Anaesthetics
C31,Pain Management
C4,Cardiothoracic Surgery
C41,Cardiac Surgery
C42,Thoracic Surgery
C5,Ear, Nose & Throat (ENT)
C51,Audiological Medicine
C6,Neurosurgery
C7,Ophthalmology
C8,Trauma and Orthopaedic Surgery
C9,Plastic Surgery
C91,Cleft Lip and Palate Surgery
CA,Paediatric Surgery
CB,Urology
CC,Intensive Care Medicine
D1,Community Dental Practice
D2,General Dental Practice
D3,Oral Surgery
D4,Oral Medicine
D5,Orthodontics
D6,Restorative Dentistry
D61,Restorative Dentistry - Endodontics
D62,Restorative Dentistry - Periodontics
D63,Restorative Dentistry - Prosthodontics
D7,Dental Public Health
D8,Paediatric Dentistry
D9,Oral Pathology
DA,Oral Microbiology
DB,Dental & Maxillofacial Radiology
DC,Surgical Dentistry
DD,Fixed & Removable Prosthodontics
DE,Special Care Dentistry
E1,General Practice
E11,GP Obstetrics
E12,GP Other than Obstetrics
F1,Obstetrics & Gynaecology
F1A,Well Woman Service
F1B,Family Planning Service
F2,Gynaecology
F3,Obstetrics
F31,Obstetrics Ante-Natal
F32,Obstetrics Post-Natal
F4,Community Sexual & Reproductive Health
G1,General Psychiatry (Mental Illness)
G1A,Community Psychiatry
G2,Child & Adolescent Psychiatry
G21,Child Psychiatry
G22,Adolescent Psychiatry
G3,Forensic Psychiatry
G4,Psychiatry of Old Age
G5,Learning Disability
G6,Psychotherapy
G61,Behavioural Psychotherapy
G62,Child and Adolescent Psychotherapy
G63,Adult Psychotherapy
H1,Clinical Radiology
H1A,Breast Screening Service
H2,Clinical Oncology
J1,Histopathology
J2,Blood Transfusion
J3,Chemical Pathology
J4,Haematology
J5,Immunology
J6,Medical Microbiology & Virology
J61,Microbiology
J62,Virology
J7,Diagnostic Neuropathology
J8,Forensic Histopathology
J9,Paediatric and Perinatal Pathology
R1,Chiropody/Podiatry
R11,Surgical Podiatry
R2,Clinical psychology
R3,Dietetics
R4,Occupational Therapy
R41,Industrial therapists
R5,Physiotherapy
R6,Speech and Language Therapy
R7,Ambulancemen/women – Accident & Emergency
R8,Audiological science
R81,Hearing aids
R82,Audiometry
R9,Medical physics
RA,Pharmacy
RB,Physiology
RC,Dental Hygiene
RD,Dental Surgery Assistance
RE,Physiological Measurement
RF,Prosthetics/orthotics
RF1,Prosthetics
RF2,Orthotics
RG,Dispensing optometry
RH,Optometry
RJ,Orthoptics
RK,Diagnostic radiography
RK1,Electroencephalography
RK2,Electrocardiography
RK3,Ultrasonics
RK4,Nuclear medicine
RL,Therapeutic radiography
RM,Medical photography
RP,Paramedics
RS,Dental therapy
RT,Pharmaceutical Medicine
RU,Arts Therapies
RU1,Art Therapy
RU2,Drama Therapy
RU3,Music Therapy
RU4,Dance Therapy
RU5,Mistletoe Therapy
RU6,Acupuncture
RU7,Bowen Therapy
RU8,Counselling
T1,General nursing
T11,School nursing
T2,Midwifery
T21,Community Midwifery
T3,Mental health nursing
T31,Community psychiatric nursing
T4,Learning disability nursing
T41,Community learning disability nursing
T5,Community nursing (district nursing)
T6,Health visiting
T7,Sick children's nursing
T8,Nursery nursing
XSU,Unspecified
XX,Others"
            );


        File.WriteAllText(Path.Combine(dir.FullName, "z_MaritalStatus.csv"),
@"Code,Meaning
A,Never married nor registered civil partnership
B,Married
C,Registered civil partnership
D,Separated, but still married
E,Separated, but still in civil partnership
F,Divorced
G,Dissolved civil partnership
H,Widowed
J,Surviving civil partner
Y,Other
Z,Not known");
    }

}
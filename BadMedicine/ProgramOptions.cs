using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace BadMedicine
{
    class ProgramOptions
    {
        [Value(0,HelpText = "Output directory to create CSV files in",Required=true)]
        public string OutputDirectory { get; set; }

        [Value(1,HelpText = "The number of unique patient identifiers to generate up front and then use in test data",Default = 500)]
        public int NumberOfPatients { get;set; }

        [Value(2,HelpText = "The number of rows to generate in test dataset(s)",Default = 2000)]
        public int NumberOfRows {get; set; }
        
        [Option('d',"The dataset to generate, must be a class name e.g. 'TestDemography'")]
        public string Dataset{get; set; }

        [Option('l', "Generate lookup tables for some codes used in the test data")]
        public bool Lookups { get; set; }

        [Option('s',"Seeds the random number generator with a specific number",Default = -1)]
        public int Seed { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return 
                    new Example("Generate test data", new ProgramOptions { OutputDirectory = @"c:/temp" });

                yield return
                    new Example("Generate a custom amount of data", new ProgramOptions { OutputDirectory = @"c:/temp",NumberOfPatients = 5000, NumberOfRows = 20000});
                    
                yield return
                    new Example("Generate only a single dataset", new ProgramOptions { OutputDirectory = @"c:/temp",NumberOfPatients = 5000, NumberOfRows = 20000, Dataset = "TestDemography"});
            }
        }

    }
}

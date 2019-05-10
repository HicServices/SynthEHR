using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BadMedicine.Datasets;
using CommandLine;

namespace BadMedicine
{
    class Program
    {
        private static int returnCode;

        public static int Main(string[] args)
        {
            returnCode = 0;

            CommandLine.Parser.Default.ParseArguments<ProgramOptions>(args)
                .WithParsed<ProgramOptions>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<ProgramOptions>((errs) => HandleParseError(errs));


            return returnCode;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            returnCode = 500;
        }

        private static void RunOptionsAndReturnExitCode(ProgramOptions opts)
        {

            if (opts.NumberOfPatients <= 0)
                opts.NumberOfPatients = 500;
            if (opts.NumberOfRows <= 0)
                opts.NumberOfRows = 2000;

            var dir = Directory.CreateDirectory(opts.OutputDirectory);

            try
            {
                //if user wants to write out the lookups generate those too
                if(opts.Lookups)
                    DataGenerator.WriteLookups(dir);

                Random r = opts.Seed == -1 ? new Random() : new Random(opts.Seed);

                //create a cohort of people
                IPersonCollection identifiers = new PersonCollection();
                identifiers.GeneratePeople(opts.NumberOfPatients,r);

                var factory = new DataGeneratorFactory();
                var generators = factory.GetAvailableGenerators().ToList();
            
                //if the user only wants to extract a single dataset
                if(!string.IsNullOrEmpty(opts.Dataset))
                {
                    var match = generators.FirstOrDefault(g => g.Name.Equals(opts.Dataset));
                    if(match == null)
                    {
                        Console.WriteLine("Could not find dataset called '" + opts.Dataset + "'");
                        Console.WriteLine("Generators found were:" + Environment.NewLine + string.Join(Environment.NewLine,generators.Select(g=>g.Name)));
                        returnCode = 2;
                        return;
                    }

                    generators = new List<Type>(new []{match});
                }

                

                //for each generator
                foreach (var g in generators)
                {
                    var instance = factory.Create(g,r);

                    var targetFile = new FileInfo(Path.Combine(dir.FullName, g.Name + ".csv"));
                    instance.GenerateTestDataFile(identifiers,targetFile,opts.NumberOfRows);
                }
            
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnCode = 2;
                return;
            }

            returnCode = 0;
        }
    }
}

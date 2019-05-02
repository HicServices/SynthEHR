using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BadMedicine.TestData.Exercises;
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
            var dir = Directory.CreateDirectory(opts.OutputDirectory);

            try
            {
                //if user wants to write out the lookups generate those too
                if(opts.Lookups)
                    ExerciseTestDataGenerator.WriteLookups(dir);

                //create a cohort of people
                IExerciseTestIdentifiers identifiers = new ExerciseTestIdentifiers();
                identifiers.GeneratePeople(opts.NumberOfPatients);

                //find all generators in the assembly
                var generators = typeof(IExerciseTestDataGenerator).Assembly.GetExportedTypes()
                    .Where(t => typeof(IExerciseTestDataGenerator).IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t.IsClass).ToList();
            
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
                    var instance = (IExerciseTestDataGenerator)Activator.CreateInstance(g);

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

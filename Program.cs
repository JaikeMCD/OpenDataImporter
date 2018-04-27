﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommandLine;

namespace Mcd.OpenData
{
    public class Options
    {
        [Option('s', "scratch", HelpText = "Run scratch experiment.")]
        public bool Scratch { get; set; }

        [Option('i', "input", Required = true, HelpText = "Config file.")]
        public string InputFile { get; set; }

        [Option('d', "dry-run", HelpText = "Perform import without updating.")]
        public bool DryRun { get; set; }

        [Option('f', "force", HelpText = "Force update to current resource revision.")]
        public bool Force { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                               .WithParsed(options => Run(options))
                               .WithNotParsed(errors => HandleErrors(errors));
        }

        static void HandleErrors(IEnumerable<Error> errors)
        {
        }


        static void Run(Options options)
        {
            OpenDataImporter importer = new OpenDataImporter(options);

            try
            {
                importer.Import();
            }
            catch(Exception e)
            {
                Console.WriteLine("Importer failed: {0}", e.Message);
            }
        }
    }
}

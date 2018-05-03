using System;
using System.Collections.Generic;
using CommandLine;

namespace Mcd.OpenData
{
    [Verb("list", HelpText = "List all OpenDataSources")]
    public class ListOptions
    { }

    [Verb("info", HelpText = "Show info for OpenDataSource")]
    public class InfoOptions
    {
        [Value(0, MetaName = "id", HelpText = "OpenDataSource ID", Required = true)]
        public int? ID { get; set; }
    }

    [Verb("updatescript", HelpText = "Loads conversion script")]
    public class UpdateScriptOptions
    {
        [Option('i', "id", HelpText = "OpenDataSource ID", Required = true)]
        public int ID { get; set; }

        [Value(0, MetaName = "scriptpath", HelpText = "Path to .csx", Required = true)]
        public string ScriptPath { get; set; }
    }

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Config file.")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file.")]
        public string OutputFile { get; set; }

        [Option('d', "dry-run", HelpText = "Perform import without updating.")]
        public bool DryRun { get; set; }

        [Option('f', "force", HelpText = "Force update to current resource revision.")]
        public bool Force { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<ListOptions, InfoOptions, UpdateScriptOptions>(args)
                .MapResult(
                (ListOptions opts) => Utilities.Source.List(opts),
                (InfoOptions opts) => Utilities.Source.Info(opts),
                (UpdateScriptOptions opts) => Utilities.ImportScript.Update(opts),
                errs => 1
                );

            /*
            var result = Parser.Default.ParseArguments<Options>(args)
                               .WithParsed(options => OpenDataImporter.Run(options))
                               .WithNotParsed(errors => HandleErrors(errors));
                               */

            return result;
        }

        static void HandleErrors(IEnumerable<Error> errors)
        {
        }
    }
}

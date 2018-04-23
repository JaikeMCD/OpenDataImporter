using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommandLine;

namespace Mcd.OpenData
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Config file.")]
        public string InputFile { get; set; }
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
            //Console.WriteLine(errors);
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

            //RunAsync().GetAwaiter().GetResult();
        }

        /*
        static async Task RunAsync()
        {
            try
            {
                CKANClient ckan = new CKANClient(null);

                CKANResource resource = await ckan.GetResourceAsync(null);

                Console.WriteLine("success: {0}", null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return;
        }
        */
    }
}

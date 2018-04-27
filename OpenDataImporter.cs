using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Mcd.OpenData
{
    public class OpenDataImporter
    {
        public Options Options { get; set; }

        protected OpenDataSource source;

        protected CKAN.Client ckan;
        protected CKAN.Resource resource;

        public OpenDataImporter(Options options)
        {
            Options = options;
        }

        public void Import()
        {
            ImportAsync().GetAwaiter().GetResult();
        }

        public async Task ImportAsync()
        {
            // Run scratch experiment

            if (Options.Scratch)
            {
                Scratch();
                return;
            }

            // Load source from config file.

            Console.WriteLine("Loading OpenDataSource from config: {0}", Options.InputFile);

            source = OpenDataSource.ReadJson(Options.InputFile);

            if (source == null)
            {
                throw new Exception("Error opening config file.");
            }

            // Get Resource from CKAN.

            Console.WriteLine("Getting CKAN resource {0} from {1}", source.ResourceId, source.BaseAddress);

            ckan = new CKAN.Client(source.BaseAddress);

            resource = await ckan.GetResourceAsync(source.ResourceId);

            if (!resource.success)
            {
                throw new Exception("Error retrieving CKAN resource info.");
            }

            // Check resource revision.

            bool current = resource.result.revision_id == source.RevisionId;
            bool update = !current || Options.Force;

            if (current)
                Console.WriteLine("Last import matches current revision {0}.", source.RevisionId);

            if (Options.Force)
                Console.WriteLine("Forcing update.");

            if (update)
                Console.WriteLine("Updating to revision {0}", resource.result.revision_id);
            else
                return;

            // Perform Import

            if (update && resource.result.url != null)
            {
                Console.WriteLine("Downloading resource {0}", resource.result.url);

                var stream = await WebUtils.DownloadStreamAsync(resource.result.url);

                Console.WriteLine("Importing from CSV");

                var csv = CsvImporter.Import(stream);



                ConvertCsv(csv);
            }

            // Update data source

            if (!Options.DryRun)
                source.Update(Options.InputFile, resource);
        }

        public void ConvertCsv(CsvImporter csv)
        {
            var scriptSource = @"
                dst.Title = src.OFFICE_TYPE;
                dst.Location = src.SITE_NAME;
                dst.Field1 = src.ADDRESS;
                dst.Field2 = src.SUBURB;
                dst.Field3 = src.STATE;
                dst.Field4 = src.POSTCODE;
            ";

            Console.WriteLine("Compiling conversion script.");

            ImportScript import = ImportScript.Create(scriptSource);

            var odrecords = import.ConvertRecords(csv.Records);
        }

        public void Scratch()
        {
            
        }
    }
}

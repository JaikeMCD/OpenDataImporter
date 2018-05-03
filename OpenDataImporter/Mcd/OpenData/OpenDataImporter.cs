using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mcd.OpenData
{
    public class OpenDataImporter
    {
        public Options Options { get; set; }

        protected OpenDataSource source;

        protected ImportScript importScript;

        protected CKAN.Client ckan;
        protected CKAN.Resource resource;

        protected StreamReader downloadStream;

        protected List<dynamic> sourceRecords;
        protected List<OpenDataRecord> records;

        public static void Run(Options options)
        {
            try
            {
                var importer = new OpenDataImporter(options);

                importer.ImportAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("Importer failed: {0}", e.Message);
            }
            
        }

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

            LoadConfig();

            // Compile import script

            CompileImportScript();

            // Get Resource from CKAN.

            await GetCKANResource();

            // Check resource revision.

            if (!CheckResourceUpdate())
                return;

            // Download Resource

            await DownloadResource();

            // Import resource records

            ImportRecords();

            // Write converted records

            WriteRecords();

            // Update data source

            UpdateDataSource();

            // Pause

            Console.ReadKey();
        }

        public void LoadConfig()
        {
            Console.WriteLine("Loading OpenDataSource from config: {0}", Options.InputFile);

            source = OpenDataSource.ReadJson(Options.InputFile);

            if (source == null)
            {
                throw new Exception("Error opening config file.");
            }
        }

        public void CompileImportScript()
        {
            Console.WriteLine("Compiling conversion script {0}", source.ImportScript);

            importScript = ImportScript.Read(source.ImportScript);
            importScript.CompileScriptRunner();
        }

        public async Task GetCKANResource()
        {
            Console.WriteLine("Getting CKAN resource {0} from {1}", source.ResourceId, source.BaseAddress);

            ckan = new CKAN.Client(source.BaseAddress);
            resource = await ckan.GetResourceAsync(source.ResourceId);

            if (!resource.success)
            {
                throw new Exception("Error retrieving CKAN resource info.");
            }
        }

        public bool CheckResourceUpdate()
        {
            bool current = resource.result.revision_id == source.RevisionId;
            bool update = !current || Options.Force;

            if (current)
                Console.WriteLine("Last import matches current revision {0}.", source.RevisionId);

            if (Options.Force)
                Console.WriteLine("Update forced.");

            if (update)
                Console.WriteLine("Updating to revision {0}", resource.result.revision_id);

            return update;
        }

        public async Task DownloadResource()
        {
            if (resource.result.url == null)
                throw new Exception("No valid URL.");

            Console.WriteLine("Downloading resource {0}", resource.result.url);

            downloadStream = await WebUtils.DownloadStreamAsync(resource.result.url);
        }

        public void ImportRecords()
        {
            Console.WriteLine("Importing CSV records.");

            var csv = CsvImporter.Import(downloadStream);
            sourceRecords = csv.Records;

            if (sourceRecords.Count == 0)
                throw new Exception("0 records to convert.");

            records = importScript.ConvertRecords(sourceRecords).ToList();

            Console.WriteLine("{0} records converted.", records.Count);
        }

        public void WriteRecords()
        {
            if (Options.DryRun)
                return;
            
            Console.WriteLine("Writing records to {0}", Options.OutputFile);

            var output = new OpenDataOutput(records);
            output.WriteJson(Options.OutputFile);
        }

        public void UpdateDataSource()
        {
            if (Options.DryRun)
                return;

            source.Update(Options.InputFile, resource);
        }

        public void Scratch()
        {
            var t = new string[] { "one", "two ", "", "three" };

            var x = t.Select(s => s.Trim());
            var y = x.Where(s => !String.IsNullOrEmpty(s));
        }
    }
}

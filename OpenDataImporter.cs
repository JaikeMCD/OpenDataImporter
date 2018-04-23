using System;
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
            // Load source from config file.

            Console.WriteLine("Loading OpenDataSource from config: {0}", Options.InputFile);

            source = OpenDataSource.ReadJson(Options.InputFile);

            if (source == null)
            {
                throw new Exception("Error opening config file.");
            }

            // Get Resource from CKAN.

            Console.WriteLine("Getting CKAN resource info: {0} {1}", source.BaseAddress, source.ResourceId);

            ckan = new CKAN.Client(source.BaseAddress);

            resource = await ckan.GetResourceAsync(source.ResourceId);

            if (!resource.success)
            {
                throw new Exception("Error retrieving CKAN resource info.");
            }

            // Update data source

            source.Update(Options.InputFile, resource);
        }
    }
}

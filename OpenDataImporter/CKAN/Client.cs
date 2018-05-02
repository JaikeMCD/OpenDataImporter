using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Mcd.OpenData.CKAN
{
    public class Resource
    {
        public class Result
        {
            public Guid package_id { get; set; }
            public Guid id { get; set; }
            public Guid revision_id { get; set; }
            public Uri url { get; set; }
            public string format { get; set; }
            public DateTime last_modified { get; set; }
            public bool datastore_active { get; set; }
        }

        public bool success { get; set; }
        public Result result { get; set; }
    }

    public class Client
    {
        private HttpClient client = new HttpClient();

        public Client(Uri baseAddress)
        {
            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Resource> GetResourceAsync(Guid resourceId)
        {
            string query = String.Format("resource_show?id={0}", resourceId);

            HttpResponseMessage response = await client.GetAsync(query);

            response.EnsureSuccessStatusCode();

            Resource resource = await response.Content.ReadAsAsync<Resource>();
            

            return resource;
        }
    }
}

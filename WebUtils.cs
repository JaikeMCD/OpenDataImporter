using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mcd.OpenData
{
    public class WebUtils
    {

        public static async Task DownloadAsync(Uri requestUri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
        }

        public static async Task<StreamReader> DownloadStreamAsync(Uri requestUri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                return new StreamReader(stream);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
    }
}

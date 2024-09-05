using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TaxCollectorPlugin;

namespace TaxCollectorPlugin.Helpers
{
    public class UniversalisClient : IDisposable
    {
        private readonly TCPlugin Plugin;

        private static readonly HttpClient Client = new();

        public static async Task<string> GetTaxRates(string world) {
            try
            {
                // Define the API endpoint with the world parameter
                string url = $"https://universalis.app/api/v2/tax-rates?world={world}";
                //string url = $"https://universalis.app/api/v2/tax-rates?world=leviathan";

                // Send GET request
                HttpResponseMessage response = await Client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw an exception if the status code is not 2xx

                // Get the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Optionally, you can parse the JSON response if you want specific fields
                var jsonDocument = JsonDocument.Parse(responseBody);

                // Example: extract some fields (you can customize this to fit your needs)
                string formattedResponse = jsonDocument.RootElement.ToString();

                // Return the formatted string
                return formattedResponse;

                // If you want to extract specific fields, you can do so like this:
                // string taxRate = jsonDocument.RootElement.GetProperty("taxRate").ToString();
                // Console.WriteLine($"Tax Rate: {taxRate}");
            }
            catch (HttpRequestException e)
            {
                return $"Request error: {e.Message}";
            }
        }

        public void Dispose() {
        }
    }

}

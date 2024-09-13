using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaxCollectorPlugin.Helpers
{
    public class UniversalisClient : IDisposable
    {
        private static readonly HttpClient Client = new();

        public static async Task<Dictionary<string, int>> GetTaxRates(string world) {
            try
            {
                // Define the API endpoint with the world parameter
                string url = $"https://universalis.app/api/v2/tax-rates?world={world}";

                // Send GET request
                HttpResponseMessage response = await Client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw an exception if the status code is not 2xx

                // Get the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(responseBody);

                // Extract tax rates into a dictionary
                var taxRates = new Dictionary<string, int>();

                foreach (var property in jsonDocument.RootElement.EnumerateObject())
                {
                    // Add each city and its tax rate to the dictionary
                    taxRates[property.Name] = property.Value.GetInt32();
                }

                // Return the dictionary of tax rates
                return taxRates;
            }
            catch (HttpRequestException e)
            {
                // Handle the error by returning an empty dictionary or logging it
                Console.WriteLine("Sending empty dictionary, something went wrong. " + e.Message);
                return new Dictionary<string, int>(); // Empty dictionary on error
            }
        }

        public void Dispose() {
        }
    }

}

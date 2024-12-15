using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ComputerVisionApiClient
{
    private static readonly string endpoint = "";
    private static readonly string apiKey = "";

    public static async Task<string> AnalyzeImageAsync(string imageUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                Console.WriteLine("Setting subscription key in header...");
                
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.BaseAddress = new Uri(endpoint);

                Console.WriteLine("Creating request body...");
                
                var requestBody = new
                {
                    url = imageUrl
                };

                var content = new StringContent(
                    JObject.FromObject(requestBody).ToString(),
                    Encoding.UTF8,
                    "application/json");

               
                string uri = "/vision/v3.2/analyze?visualFeatures=Description,Tags,Objects,Color,Categories";

                Console.WriteLine("Sending POST request...");
                HttpResponseMessage response = await client.PostAsync(uri, content);

                Console.WriteLine("Reading response body...");
                string responseBody = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Response status code: {response.StatusCode}");
                    Console.WriteLine($"Response body: {responseBody}");
                }

                response.EnsureSuccessStatusCode();

                Console.WriteLine("Request successful.");
                
                dynamic analysis = JObject.Parse(responseBody);
                var descriptions = analysis.description.captions;
                var tags = analysis.tags;
                var objects = analysis.objects;
                var color = analysis.color;

                Console.WriteLine("Description and Tags:");
                foreach (var caption in descriptions)
                {
                    Console.WriteLine($"Description: {caption.text}");
                }

                Console.WriteLine("\nTags related to rice planting:");
                foreach (var tag in tags)
                {
                    Console.WriteLine($"Tag: {tag.name}");
                }

                string insightMessage = GenerateFarmInsight(color, tags);

                return insightMessage;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }

    private static string GenerateFarmInsight(dynamic color, dynamic tags)
    {
    // Default 
    string insight = "The field is looking good!";

    Console.WriteLine("Dominant Colors:");
    foreach (var dominantColor in color.dominantColors)
    {
        Console.WriteLine(dominantColor); 
    }

    if (color.dominantColors != null)
    {
        List<string> normalizedColors = new List<string>();

        foreach (var dominantColor in color.dominantColors)
            {
                normalizedColors.Add(dominantColor.ToString().Trim().ToLower());
            }

            if (normalizedColors.Contains("yellow"))
            {
                insight = "The field might be stressed. Consider watering the areas showing yellow.";
            }

            if (normalizedColors.Contains("red"))
            {
                Console.WriteLine("Red detected! Updating insight.");
                insight = "The field is extremely stressed. Investigate the area and check irrigation, fertilizer, or pests.";
            }

            if (normalizedColors.Contains("pink"))
            {
                Console.WriteLine("Pink detected! Updating insight.");
                insight = "The field may be experiencing temperature or nutrient stress. Consider evaluating environmental conditions.";
            }

            if (normalizedColors.Contains("green"))
            {
                Console.WriteLine("Green detected! Updating insight.");
                insight = "The field appears healthy with good vegetation coverage. Continue monitoring growth.";
            }

            if (normalizedColors.Contains("blue"))
            {
                Console.WriteLine("Blue detected! Updating insight.");
                insight = "Blue tones might indicate waterlogged areas or high moisture levels. Ensure proper drainage.";
            }

            if (normalizedColors.Contains("purple"))
            {
                Console.WriteLine("Purple detected! Updating insight.");
                insight = "Purple tones could indicate phosphorus deficiency or cold stress. Consider soil testing.";
            }

            if (normalizedColors.Contains("dark green"))
            {
                Console.WriteLine("Dark Green detected! Updating insight.");
                insight = "The field shows high vegetative health. Monitor for pest or disease pressures that may develop.";
            }

            if (normalizedColors.Contains("light green"))
            {
                Console.WriteLine("Light Green detected! Updating insight.");
                insight = "The field shows early signs of growth. Ensure optimal conditions for continued development.";
            }

            if (normalizedColors.Contains("brown"))
            {
                Console.WriteLine("Brown detected! Updating insight.");
                insight = "Brown areas may indicate areas with poor vegetation or soil issues. Consider checking irrigation or soil health.";
            }
            
    }

    if (tags != null && tags.Count > 0)
    {
        foreach (var tag in tags)
        {
            string tagName = tag.name.ToString().ToLower(); 

            if (tagName.Contains("ndvi"))
            {
                insight += " NDVI analysis suggests monitoring the vegetation health closely. Areas with lower values might need more attention for growth.";
            }
            if (tagName.Contains("healthy vegetation"))
            {
                insight += " The field is showing healthy vegetation. Growth is progressing well.";
            }
            if (tagName.Contains("stressed vegetation"))
            {
                insight += " Stressed vegetation detected. Investigate water, nutrient, or pest-related issues.";
            }
            if (tagName.Contains("water-stressed"))
            {
                insight += " Water-stressed areas detected. Consider adjusting irrigation to prevent crop damage.";
            }
            if (tagName.Contains("soil moisture"))
            {
                insight += " Soil moisture levels are critical. Ensure proper irrigation to avoid dehydration or overwatering.";
            }
            if (tagName.Contains("heat stress"))
            {
                insight += " Heat stress detected in certain areas. Consider increasing irrigation or adding shade to vulnerable crops.";
            }
            if (tagName.Contains("cold stress"))
            {
                insight += " Cold stress detected. Ensure crops are protected from low temperatures to prevent damage.";
            }
            if (tagName.Contains("soil erosion"))
            {
                insight += " Soil erosion detected. Implement erosion control measures to protect crops and soil quality.";
            }
            if (tagName.Contains("soil fertility"))
            {
                insight += " Soil fertility levels suggest the need for additional fertilization in certain areas.";
            }
            if (tagName.Contains("pest infestation"))
            {
                insight += " Pest infestation detected. Monitor the affected areas and take necessary action to protect crops.";
            }
            if (tagName.Contains("disease outbreak"))
            {
                insight += " Disease outbreak detected. Evaluate the affected areas for preventive measures.";
            }
            if (tagName.Contains("drought"))
            {
                insight += " Drought conditions detected. Consider implementing water-saving techniques or switching to drought-resistant crops.";
            }
            if (tagName.Contains("flood risk"))
            {
                insight += " Flood risk detected in certain areas. Ensure proper drainage and flood prevention measures.";
            }
            if (tagName.Contains("temperature anomaly"))
            {
                insight += " Temperature anomaly detected. Monitor crop health closely for signs of stress due to abnormal weather.";
            }
            if (tagName.Contains("precipitation"))
            {
                insight += " Precipitation patterns suggest upcoming rainfall. Plan irrigation and field activities accordingly.";
            }
            if (tagName.Contains("field boundary"))
            {
                insight += " Field boundaries detected. Ensure proper planning of field activities within these areas.";
            }
            if (tagName.Contains("harvest timing"))
            {
                insight += " Harvest timing is approaching. Monitor crop growth closely for optimal harvest conditions.";
            }
        }
    }
    Console.WriteLine("Final Insight: " + insight);
    return insight;
}
}

public class Program
{
    public static async Task Main(string[] args)
    {
        string imageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSZsUzeeiTx-a-sjnz33LdUkP6jCRx3DL63Fw&s"; // Replace with your image URL
        try
        {
            Console.WriteLine("Starting image analysis...");
            string analysisResult = await ComputerVisionApiClient.AnalyzeImageAsync(imageUrl);
            Console.WriteLine("Analysis result:");
            Console.WriteLine(analysisResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
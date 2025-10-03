using FlowVision.lib.Classes;
using FlowVision.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Client for interacting with the OmniParser FastAPI /parse/ endpoint.
/// </summary>
public class OmniParserClient
{
    // Set the API endpoint for the FastAPI server.
    private const string ApiEndpoint = "/parse/"; 
    private readonly HttpClient _httpClient;

    public OmniParserClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Processes a screenshot provided as a Base64-encoded image string and deserializes the JSON response.
    /// </summary>
    /// <param name="base64Image">A Base64-encoded image string.</param>
    /// <returns>An instance of MyCustomObject representing the API response.</returns>
    public async Task<OmniparserResponse> ProcessScreenshotAsync(string base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
            throw new ArgumentException("Base64 image data is required", nameof(base64Image));

        // Get server URL from LocalOmniParserManager (auto-managed)
        string serverUrl = LocalOmniParserManager.GetServerUrl();
        
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new InvalidOperationException("OmniParser server URL is not configured.");

        // Build the JSON payload expected by the API.
        var payload = new { base64_image = base64Image };
        string jsonPayload = JsonConvert.SerializeObject(payload);

        // Create the HTTP content.
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Increase timeout to 10 minutes.
        _httpClient.Timeout = TimeSpan.FromMinutes(10);
        
        // Properly concatenate the server URL with the API endpoint
        string fullUrl = serverUrl.TrimEnd('/') + ApiEndpoint;
        HttpResponseMessage response = await _httpClient.PostAsync(fullUrl, content);
        response.EnsureSuccessStatusCode();

        // Read and deserialize the response content into MyCustomObject.
        string responseContent = await response.Content.ReadAsStringAsync();
        OmniparserResponse result = JsonConvert.DeserializeObject<OmniparserResponse>(responseContent);
        return result;
    }
}

/// <summary>
/// Represents the top-level object returned by the API.
/// </summary>
public class OmniparserResponse
{
    [JsonProperty("som_image_base64")]
    public string SomImageBase64 { get; set; }
    [JsonProperty("parsed_content_list")]
    public List<ParsedContent> ParsedContentList { get; set; }
    [JsonProperty("latency")]
    public double Latency { get; set; }
}

/// <summary>
/// Represents each item in the parsed_content_list.
/// </summary>
public class ParsedContent
{
    [JsonProperty("type")]
    public string Type { get; set; }

    // Using a double array for the bounding box values.
    [JsonProperty("bbox")]
    public double[] BBox { get; set; }

    [JsonProperty("interactivity")]
    public bool Interactivity { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }
}

/*
 * Future Stuff
 * 
 * int x = Cursor.Position.X;
int y = Cursor.Position.Y;
int size = 10; // Arbitrary size

System.Drawing.Graphics graphics = CreateGraphics();
System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(x - (size / 2), y - (size / 2), size, size);
graphics.DrawRectangle(System.Drawing.Pens.Red, rectangle);
 */
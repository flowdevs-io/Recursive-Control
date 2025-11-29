using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Represents each item in the parsed UI content list.
    /// Used for screen capture and OmniParser results.
    /// </summary>
    public class ParsedContent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Bounding box values [x1, y1, x2, y2] in pixels
        /// </summary>
        [JsonProperty("bbox")]
        public double[] BBox { get; set; }

        [JsonProperty("interactivity")]
        public bool Interactivity { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }

    /// <summary>
    /// Response from OmniParser processing
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
}

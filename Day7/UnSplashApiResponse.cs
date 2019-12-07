using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Day7
{
    public class UnSplashApiResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<Results> Results { get; set; }
    }

    public class Results
    {
        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }
    }

    public class Links
    {
        [JsonProperty(PropertyName = "download")]
        public string Download { get; set; }

    }
}

using Newtonsoft.Json;

namespace Day9.Models
{
    internal class GitHubIssueEvent
    {
        [JsonProperty(PropertyName ="action")]
        public string action { get; set; }

        [JsonProperty(PropertyName = "issue")]
        public Issue Issue { get; set; }

        [JsonProperty(PropertyName = "repository")]
        public Repository Repository { get; set; }
    }

    internal class Issue
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }
    }

    internal class User
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
    }

    internal class Repository
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
    }
}
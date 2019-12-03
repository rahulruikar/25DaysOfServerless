using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Day3.Models
{
    public sealed class GitPushEvent
    {
        [JsonProperty(PropertyName = "repository")]
        public Repository Repository { get; set; }

        [JsonProperty(PropertyName = "commits")]
        public List<CommitItem> Commits { get; set; }
    }

    public sealed class Repository
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }
    }

    public sealed class CommitItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "author")]
        public Author Author { get; set; }
    }

    public sealed class Author
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
    }
}

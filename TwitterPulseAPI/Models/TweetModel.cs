using System;
using System.Collections.Generic;

namespace TwitterPulseAPI.Models
{
    public class TweetModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public Entity entities { get; set; } = new Entity();
        public DateTimeOffset dateLogged { get; set; } = DateTimeOffset.Now;
        public bool ContainsOneOrMoreUrl { get; set; }
        public bool ContainsOneOrMorePhotoUrl { get; set; }

        public class Entity
        {
            public List<Hashtag> hashtags { get; set; } = new List<Hashtag>();
            public List<TweetUrl> urls { get; set; } = new List<TweetUrl>();
        }
    }

    public class TweetUrl
    {
        public int start { get; set; }
        public int end { get; set; }
        public string url { get; set; } //"https://t.co/OHOpCa9FWM" twitter shortened url
        public string expanded_url { get; set; } //"https://twitter.com/ilyanirzhr/status/1365121992429502466"
        public string display_url { get; set; } //"twitter.com/ilyanirzhr/sta…" use to get domain
        public string domain { get; set; }
    }

    public class Hashtag
    {
        public int start { get; set; }
        public int end { get; set; }
        public string tag { get; set; }
    }
}

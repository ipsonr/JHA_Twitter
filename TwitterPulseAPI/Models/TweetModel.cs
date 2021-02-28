using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitterPulseAPI.Models
{
    public class TweetModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public Entity entities { get; set; } = new Entity();
        public DateTimeOffset dateLogged { get; set; } = DateTimeOffset.Now;
        public bool ContainsOneOrMoreUrl { get; set; }
        //{ 
        //    get
        //    {
        //        return text.Contains("http");//TODO use regex?
        //    }
        //}
        public bool ContainsOneOrMorePhotoUrl { get; set; }
        //{
        //    get
        //    {
        //        return text.Contains("pic.twitter.com", StringComparison.OrdinalIgnoreCase) || text.Contains("instagram.com", StringComparison.OrdinalIgnoreCase);//TODO use regex?
        //    }
        //}

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
        public string url { get; set; } //"https://t.co/OHOpCa9FWM"
        public string expanded_url { get; set; } //"https://twitter.com/ilyanirzhr/status/1365121992429502466"
        public string display_url { get; set; } //"twitter.com/ilyanirzhr/sta…" use to get domain
        public string domain { get; set; }
        //{
        //    get
        //    {
        //        var regex = new Regex(@"/[^/ | \\B]+/g");
        //        string match = regex.Match(display_url).Value;

        //        return match ?? null;
        //    }
        //}
    }

    public class Hashtag
    {
        public int start { get; set; }
        public int end { get; set; }
        public string tag { get; set; }
    }
}

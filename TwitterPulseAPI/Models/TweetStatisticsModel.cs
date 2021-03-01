using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterPulseAPI.Models
{
    public class TweetStatisticsModel
    {
        //public List<TweetModel> Tweets { get; set; }
        public int CountTweetsReceived { get; set; } // #1 Total number of tweets received 
        public double AverageTweetsPerSecond // #2 Average tweets per hour/minute/second
        {
            get
            {
                return (double)CountTweetsReceived / ElapsedTime.TotalSeconds;
            }
        } 
        public Dictionary<string, int> HashtagsUsed { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> EmojisUsed { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UrlDomainsUsed { get; set; } = new Dictionary<string, int>();
        public int CountTweetsWithEmojis { get; set; }
        public int CountTweetsWithUrls { get; set; }
        public int CountTweetsWithPhotoUrls { get; set; }
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset EndTime { get; set; }
        public TimeSpan ElapsedTime 
        {
            get
            {
                return EndTime - StartTime;
            }
        } 
        // TODO make this a method and pass in the number to take (e.g. 5) as a parameter, or expose as a property
        public Dictionary<string, int> Top5Hashtags // #5 Top hashtags 
        {
            get
            {
                return HashtagsUsed.OrderByDescending(x => x.Value).Take(5).ToDictionary(y => y.Key, z => z.Value);
            }
        }
        public Dictionary<string, int> Top5Emojis // #3 Top emojis in tweets* 
        {
            get
            {
                return EmojisUsed.OrderByDescending(x => x.Value).Take(5).ToDictionary(y => y.Key, z => z.Value);
            }
        }
        public Dictionary<string, int> Top5UrlDomains // #8 Top domains of urls in tweets 
        {
            get
            {
                return UrlDomainsUsed.OrderByDescending(x => x.Value).Take(5).ToDictionary(y => y.Key, z => z.Value);
            }
        }
        public void UpdateHashtagsUsed(Dictionary<string, int> tweetHashtags)
        {
            foreach (var hashtag in tweetHashtags)
            {
                if (!HashtagsUsed.ContainsKey(hashtag.Key))
                    HashtagsUsed.Add(hashtag.Key, hashtag.Value);
                else
                    HashtagsUsed[hashtag.Key] += hashtag.Value;
            }
        }
        public void UpdateDomainsUsed(Dictionary<string, int> tweetDomains)
        {
            foreach (var domain in tweetDomains)
            {
                if (!UrlDomainsUsed.ContainsKey(domain.Key))
                    UrlDomainsUsed.Add(domain.Key, domain.Value);
                else
                    UrlDomainsUsed[domain.Key] += domain.Value;
            }
        }
        public void UpdateEmojisUsed(Dictionary<string, int> tweetEmojis)
        {
            foreach (var emoji in tweetEmojis)
            {
                if (!EmojisUsed.ContainsKey(emoji.Key))
                    EmojisUsed.Add(emoji.Key, emoji.Value);
                else
                    EmojisUsed[emoji.Key] += emoji.Value;
            }
        }
        public double PercentageWithEmojis // #4 Percent of tweets that contains emojis 
        {
            get { return (double)100 * CountTweetsWithEmojis / CountTweetsReceived; }
        }
        public double PercentageWithUrls // #6 Percent of tweets that contain a url 
        {
            get { return (double)100 * CountTweetsWithUrls / CountTweetsReceived; }
        }
        public double PercentageWithPhotoUrls // #7 Percent of tweets that contain a photo url 
        {
            get { return (double)100 * CountTweetsWithPhotoUrls / CountTweetsReceived; }
        }
    }
}

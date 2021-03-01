using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseAPI.Services
{
    public class InMemoryTweetService : ITweetService
    {
        private readonly IConfiguration _configuration;
        public InMemoryTweetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<TweetModel> _tweets { get; set; }
        public Stream SyncedStream { get; set; }
        public TweetStatisticsModel _statsModel { get; set; }
        public void Add(TweetModel tweetModel)
        {
            _tweets.Add(tweetModel);
        }

        public void Add(List<TweetModel> tweetModels)
        {
            _tweets.AddRange(tweetModels);
        }

        public IEnumerable<TweetModel> GetSampleTweets(int numberOfTweets = 0)
        {
            if (numberOfTweets == 0)
                return _tweets?? null;
            else
                return _tweets?.Take(numberOfTweets);
        }

        public async Task<TweetStatisticsModel> PopulateTweetsAndGetStats()//TODO move to repository
        {
            _statsModel = new TweetStatisticsModel();
            string myUrl = _configuration.GetValue<string>("SampleUrl");
            var httpClient = new HttpClient();
            string bearerToken = _configuration.GetValue<string>("BearerToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            try
            {
                using (httpClient)
                {
                    var myStream = await httpClient.GetStreamAsync(myUrl);
                    SyncedStream = Stream.Synchronized(myStream);
                    using (myStream)
                    {
                        StreamReader myReader = new StreamReader(myStream, System.Text.Encoding.UTF8);
                        var myTweetLine = myReader.ReadLine();//does data have line breaks? - yes
                        /*
{{"data":{"id":"1363700900435435523","text":"@nuckyzuu แงกอดกอดดด"}}
{"data":{ "id":"1363700900431204352","text":"道民だからこの時期の暑さが違和感で日差しが🌞上海暑いな。夏大丈夫か😅"} }
{"data":{"id":"1363704956327399424","text":"Grateful is an attitude that makes life easier."}}
                         */
                        while (!String.IsNullOrEmpty(myTweetLine))
                        {
                            var newTweet = GetTweetFromResponse(myTweetLine);
                            if (newTweet != null)
                            {
                                // TODO push tweets to buffered file or memory to not slow down stream
                                // then process file to get stats
                                //myTweets.Add(newTweet);
                                UpdateStats(_statsModel, newTweet);
                            }
                            myTweetLine = myReader.ReadLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return _statsModel;
        }
        public async Task PopulateTweets()//TODO move to repository for EF version
        {
            //List<TweetModel> myTweets = new List<TweetModel>();
            _statsModel = new TweetStatisticsModel();
            string myUrl = _configuration.GetValue<string>("SampleUrl");
            var httpClient = new HttpClient();
            string bearerToken = _configuration.GetValue<string>("BearerToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            try
            {
                using (httpClient)
                {
                    var myStream = await httpClient.GetStreamAsync(myUrl);
                    SyncedStream = Stream.Synchronized(myStream);
                    //StringBuilder jsonStringBuilder = new StringBuilder();
                    using (myStream)
                    {
                        StreamReader myReader = new StreamReader(myStream, System.Text.Encoding.UTF8);
                        var myTweetLine = myReader.ReadLine();//does data have line breaks? - yes
                        /*
{{"data":{"id":"1363700900435435523","text":"@nuckyzuu แงกอดกอดดด"}}
{"data":{ "id":"1363700900431204352","text":"道民だからこの時期の暑さが違和感で日差しが🌞上海暑いな。夏大丈夫か😅"} }
{"data":{"id":"1363704956327399424","text":"Grateful is an attitude that makes life easier."}}
                         */
                        while (!String.IsNullOrEmpty(myTweetLine))
                        {
                            myTweetLine = myReader.ReadLine();
                        }
                    }
                }
                //_tweets = myTweets;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                //_tweets = null;
            }
            //return _statsModel;
        }

        public void UpdateStats(TweetStatisticsModel statsModel, TweetModel newTweet)
        {
            statsModel.CountTweetsReceived += 1;
            var hashTags = GetHashtags(newTweet);
            var emojis = GetEmojis(newTweet);
            var urlsDictionary = GetDomainsDictionary(newTweet);
            if (hashTags.Count > 0)
                statsModel.UpdateHashtagsUsed(hashTags);

            if (emojis.Count > 0)
            {
                statsModel.UpdateEmojisUsed(emojis);
                statsModel.CountTweetsWithEmojis += 1;
            }

            if (urlsDictionary.Count > 0)
            {
                statsModel.CountTweetsWithUrls += 1;
                if (newTweet.ContainsOneOrMorePhotoUrl)
                    statsModel.CountTweetsWithPhotoUrls += 1;

                statsModel.UpdateDomainsUsed(urlsDictionary);
            }
            // will update end time for every tweet, and end with the time of the final one
            statsModel.EndTime = DateTimeOffset.Now; 
        }

        private Dictionary<string, int> GetHashtags(TweetModel newTweet)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            try
            {
                foreach (var hashtag in newTweet.entities.hashtags)
                {
                    if (!result.ContainsKey(hashtag.tag))
                        result.Add(hashtag.tag, 1);
                    else
                        result[hashtag.tag] += 1;
                }
            }
            catch (Exception ex) { var test = ex.Message; }
            return result;
        }

        private Dictionary<string, int> GetEmojis(TweetModel newTweet)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            try
            {
                string emojiRegex = _configuration.GetValue<string>("EmojiRegex");
                var regex = new Regex(emojiRegex);
                var matches = regex.Matches(newTweet.text);
                foreach (Match match in matches)
                {
                    if (!result.ContainsKey(match.Value))
                        result.Add(match.Value, 1);
                    else
                        result[match.Value] += 1;
                }
            }
            catch { }
            return result;
        }

        private Dictionary<string, int> GetDomainsDictionary(TweetModel newTweet)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            try
            {
                foreach (var url in newTweet.entities.urls)
                {
                    if (!result.ContainsKey(url.domain))
                        result.Add(url.domain, 1);
                    else
                        result[url.domain] += 1;
                }
            }
            catch (Exception ex) { var test = ex.Message; }
            return result;
        }

        public static TweetModel GetTweetFromResponse(string responseString)
        {
            try
            {
                var myTweetString = responseString.Substring(8, responseString.Length - 9);
                var tweetModel = JsonConvert.DeserializeObject<TweetModel>(myTweetString.ToString());

                var regex = new Regex(@"[^/]*");
                string match = String.Empty;
                if (tweetModel.entities.urls.Count > 0)
                {
                    foreach (var url in tweetModel.entities.urls)
                    {
                        match = regex.Match(url.display_url).Value;
                        url.domain = match ?? null;

                        if (match.Contains("pic.twitter.com", StringComparison.OrdinalIgnoreCase)
                            || match.Contains("twitpic.com", StringComparison.OrdinalIgnoreCase)
                            || match.Contains("instagram.com", StringComparison.OrdinalIgnoreCase))
                            tweetModel.ContainsOneOrMorePhotoUrl = true;
                    }
                }

                return tweetModel;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public async Task<TweetStatisticsModel> GetStatisticsAsync()
        {
            //List<TweetModel> myTweets = new List<TweetModel>();
            _statsModel = new TweetStatisticsModel();
            try
            {
                using (SyncedStream)
                {
                    StreamReader mySyncedReader = new StreamReader(SyncedStream, System.Text.Encoding.UTF8);
                    var mySyncedTweetLine = mySyncedReader.ReadLineAsync();
                    while (!String.IsNullOrEmpty(mySyncedTweetLine.Result))
                    {
                        var newTweet = GetTweetFromResponse(mySyncedTweetLine.Result);
                        if (newTweet != null)
                        {
                            // TODO push tweets to buffered file or memory to not slow down stream
                            // then process file to get stats
                            //myTweets.Add(newTweet);
                            UpdateStats(_statsModel, newTweet);
                        }
                        mySyncedTweetLine = mySyncedReader.ReadLineAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return _statsModel;
        }
    }
}

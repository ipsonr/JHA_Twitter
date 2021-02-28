using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseAPI.Services
{
    public class InMemoryTweetService : ITweetService
    {
        List<TweetModel> _tweets;
        TweetStatisticsModel _statsModel;
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

        //public static async Task<List<TweetModel>> PopulateTweets()
        public async Task<TweetStatisticsModel> PopulateTweets()//TODO move to repository
        {
            List<TweetModel> myTweets = new List<TweetModel>();
            _statsModel = new TweetStatisticsModel();
            string myUrl = "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=entities";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", @"AAAAAAAAAAAAAAAAAAAAABLlMwEAAAAAvdNPda7LiFxx7BsOHpB0LVMxysY%3DF0E3sqgHbGZC7igFDetGvRXOoMtcChZUrxGLl4pqraAQA5YMEy");

            try
            {
                using (httpClient)
                {
                    var myStream = await httpClient.GetStreamAsync(myUrl);
                    var syncStream = Stream.Synchronized(myStream);
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
                            var newTweet = GetTweetFromResponse(myTweetLine);
                            if (newTweet != null)
                            {
                                // TODO push tweets to buffered file or memory to not slow down stream
                                // then process file to get stats
                                myTweets.Add(newTweet);
                                UpdateStats(_statsModel, newTweet);
                            }
                            myTweetLine = myReader.ReadLine();
                        }
                    }
                    using (syncStream)
                    {
                        StreamReader myReader = new StreamReader(syncStream, System.Text.Encoding.UTF8);
                        var myTweetLine = myReader.ReadLine();//does data have line breaks? - yes
                    }
                }
                _tweets = myTweets;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                _tweets = null;
            }
            return _statsModel;
        }

        private void UpdateStats(TweetStatisticsModel statsModel, TweetModel newTweet)
        {
            statsModel.CountTweetsReceived += 1;
            var hashTags = GetHashtags(newTweet);
            var emojis = GetEmojis(newTweet);
            var urlsDictionary = GetDomainsDictionary(newTweet);
            //var photoUrls = GetPhotoUrls(newTweet);
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

            statsModel.EndTime = DateTimeOffset.Now; // will update end time for every tweet, and end with the time of the final one
        }

        private Dictionary<string, int> GetHashtags(TweetModel newTweet)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            try
            {
                //var regex = new Regex(@"#\w+");
                //var matches = regex.Matches(newTweet.text);
                //foreach (Match match in matches)
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
                var emojiRegex = @"\uD83D(?:\uDC68(?:\uD83C(?:[\uDFFB-\uDFFF]\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFF]|[\uDDAF-\uDDB3\uDDBC\uDDBD])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|[\uDFFB-\uDFFF])|\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68|\uD83D(?:(?:[\uDC68\uDC69]\u200D\uD83D)?(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83E[\uDDAF-\uDDB3\uDDBC\uDDBD]))?|\uDC69(?:\uD83C(?:[\uDFFB-\uDFFF]\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFB-\uDFFF]|[\uDDAF-\uDDB3\uDDBC\uDDBD])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|[\uDFFB-\uDFFF])|\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\uD83D(?:(?:\uDC69\u200D\uD83D)?(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83E[\uDDAF-\uDDB3\uDDBC\uDDBD]))?|(?:\uDD75(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)?|\uDC6F)(?:\u200D[\u2640\u2642]\uFE0F?)?|[\uDC6E\uDC70\uDC71\uDC73\uDC77\uDC81\uDC82\uDC86\uDC87\uDE45-\uDE47\uDE4B\uDE4D\uDE4E\uDEA3\uDEB4-\uDEB6](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|\uDC41(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?)?|\uDE36(?:\u200D\uD83C\uDF2B\uFE0F?)?|\uDC15(?:\u200D\uD83E\uDDBA)?|\uDC3B(?:\u200D\u2744\uFE0F?)?|\uDE2E(?:\u200D\uD83D\uDCA8)?|\uDE35(?:\u200D\uD83D\uDCAB)?|[\uDC42\uDC43\uDC46-\uDC50\uDC66\uDC67\uDC6B-\uDC6D\uDC72\uDC74-\uDC76\uDC78\uDC7C\uDC83\uDC85\uDC8F\uDC91\uDCAA\uDD7A\uDD95\uDD96\uDE4C\uDE4F\uDEC0\uDECC](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD74\uDD90]\uD83C[\uDFFB-\uDFFF]|\uDC08(?:\u200D\u2B1B)?|[\uDC3F\uDCFD\uDD49\uDD4A\uDD6F\uDD70\uDD73\uDD74\uDD76-\uDD79\uDD87\uDD8A-\uDD8D\uDD90\uDDA5\uDDA8\uDDB1\uDDB2\uDDBC\uDDC2-\uDDC4\uDDD1-\uDDD3\uDDDC-\uDDDE\uDDE1\uDDE3\uDDE8\uDDEF\uDDF3\uDDFA\uDECB\uDECD-\uDECF\uDEE0-\uDEE5\uDEE9\uDEF0\uDEF3]\uFE0F?|[\uDC00-\uDC07\uDC09-\uDC14\uDC16-\uDC3A\uDC3C-\uDC3E\uDC40\uDC44\uDC45\uDC51-\uDC65\uDC6A\uDC79-\uDC7B\uDC7D-\uDC80\uDC84\uDC88-\uDC8E\uDC90\uDC92-\uDCA9\uDCAB-\uDCFC\uDCFF-\uDD3D\uDD4B-\uDD4E\uDD50-\uDD67\uDDA4\uDDFB-\uDE2D\uDE2F-\uDE34\uDE37-\uDE44\uDE48-\uDE4A\uDE80-\uDEA2\uDEA4-\uDEB3\uDEB7-\uDEBF\uDEC1-\uDEC5\uDED0-\uDED2\uDED5-\uDED7\uDEEB\uDEEC\uDEF4-\uDEFC\uDFE0-\uDFEB])|\uD83E(?:\uDDD1(?:\uD83C(?:[\uDFFB-\uDFFF]\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF-\uDDB3\uDDBC\uDDBD])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|[\uDFFB-\uDFFF])|\u200D(?:\uD83E(?:\uDD1D\u200D\uD83E\uDDD1|[\uDDAF-\uDDB3\uDDBC\uDDBD])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|[\uDD26\uDD35\uDD37-\uDD39\uDD3D\uDD3E\uDDB8\uDDB9\uDDCD-\uDDCF\uDDD4\uDDD6-\uDDDD](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|[\uDD3C\uDDDE\uDDDF](?:\u200D[\u2640\u2642]\uFE0F?)?|[\uDD0C\uDD0F\uDD18-\uDD1C\uDD1E\uDD1F\uDD30-\uDD34\uDD36\uDD77\uDDB5\uDDB6\uDDBB\uDDD2\uDDD3\uDDD5](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD0D\uDD0E\uDD10-\uDD17\uDD1D\uDD20-\uDD25\uDD27-\uDD2F\uDD3A\uDD3F-\uDD45\uDD47-\uDD76\uDD78\uDD7A-\uDDB4\uDDB7\uDDBA\uDDBC-\uDDCB\uDDD0\uDDE0-\uDDFF\uDE70-\uDE74\uDE78-\uDE7A\uDE80-\uDE86\uDE90-\uDEA8\uDEB0-\uDEB6\uDEC0-\uDEC2\uDED0-\uDED6])|\uD83C(?:\uDFF4(?:\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67|\uDC73\uDB40\uDC63\uDB40\uDC74|\uDC77\uDB40\uDC6C\uDB40\uDC73)\uDB40\uDC7F|\u200D\u2620\uFE0F?)?|[\uDFC3\uDFC4\uDFCA](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|[\uDFCB\uDFCC](?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)(?:\u200D[\u2640\u2642]\uFE0F?)?|\uDFF3(?:\uFE0F(?:\u200D(?:\u26A7\uFE0F?|\uD83C\uDF08))?|\u200D(?:\u26A7\uFE0F?|\uD83C\uDF08))?|(?:[\uDFCB\uDFCC]\u200D[\u2640\u2642]|[\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37\uDF21\uDF24-\uDF2C\uDF36\uDF7D\uDF96\uDF97\uDF99-\uDF9B\uDF9E\uDF9F\uDFCD\uDFCE\uDFD4-\uDFDF\uDFF5\uDFF7])\uFE0F?|[\uDF85\uDFC2\uDFC7](?:\uD83C[\uDFFB-\uDFFF])?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDC04\uDCCF\uDD8E\uDD91-\uDD9A\uDE01\uDE1A\uDE2F\uDE32-\uDE36\uDE38-\uDE3A\uDE50\uDE51\uDF00-\uDF20\uDF2D-\uDF35\uDF37-\uDF7C\uDF7E-\uDF84\uDF86-\uDF93\uDFA0-\uDFC1\uDFC5\uDFC6\uDFC8\uDFC9\uDFCB\uDFCC\uDFCF-\uDFD3\uDFE0-\uDFF0\uDFF8-\uDFFF])|\u26F9(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)(?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|\u2764(?:\uFE0F(?:\u200D(?:\uD83D\uDD25|\uD83E\uDE79))?|\u200D(?:\uD83D\uDD25|\uD83E\uDE79))?|[\#\*0-9]\uFE0F?\u20E3|[\u261D\u270C\u270D]\uD83C[\uDFFB-\uDFFF]|[\u270A\u270B](?:\uD83C[\uDFFB-\uDFFF])?|[\u00A9\u00AE\u203C\u2049\u2122\u2139\u2194-\u2199\u21A9\u21AA\u2328\u23CF\u23ED-\u23EF\u23F1\u23F2\u23F8-\u23FA\u24C2\u25AA\u25AB\u25B6\u25C0\u25FB\u25FC\u2600-\u2604\u260E\u2611\u2618\u261D\u2620\u2622\u2623\u2626\u262A\u262E\u262F\u2638-\u263A\u2640\u2642\u265F\u2660\u2663\u2665\u2666\u2668\u267B\u267E\u2692\u2694-\u2697\u2699\u269B\u269C\u26A0\u26A7\u26B0\u26B1\u26C8\u26CF\u26D1\u26D3\u26E9\u26F0\u26F1\u26F4\u26F7\u26F8\u2702\u2708\u2709\u270C\u270D\u270F\u2712\u2714\u2716\u271D\u2721\u2733\u2734\u2744\u2747\u2763\u27A1\u2934\u2935\u2B05-\u2B07\u3030\u303D\u3297\u3299]\uFE0F?|[\u231A\u231B\u23E9-\u23EC\u23F0\u23F3\u25FD\u25FE\u2614\u2615\u2648-\u2653\u267F\u2693\u26A1\u26AA\u26AB\u26BD\u26BE\u26C4\u26C5\u26CE\u26D4\u26EA\u26F2\u26F3\u26F5\u26FA\u26FD\u2705\u2728\u274C\u274E\u2753-\u2755\u2757\u2795-\u2797\u27B0\u27BF\u2B1B\u2B1C\u2B50\u2B55]";
                //var emojiRegex = @"emoji\w+";
                var regex = new Regex(emojiRegex);//TODO need different regex
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
                //var regex = new Regex(@"(http|https)://[^/\\B]+");//TODO test to see if different regex is needed?
                //var matches = regex.Matches(newTweet.text);

                //var test = String.Empty;
                //if (matches?.Count == 0 && newTweet.text.Contains("http"))
                //    test = newTweet.text;

                //foreach (Match match in matches)
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

        private static TweetModel GetTweetFromResponse(string responseString)
        {
            try
            {
                var myTweetString = responseString.Substring(8, responseString.Length - 9);
                //return JsonConvert.DeserializeObject<TweetModel>(myTweetString.ToString());
                var tweetModel = JsonConvert.DeserializeObject<TweetModel>(myTweetString.ToString());

                var regex = new Regex(@"[^/]*");
                //var regex = new Regex(@"[^/\\B]*/g");
                string match = String.Empty;
                //var uri = new Uri("http://www.test.com/");
                if (tweetModel.entities.urls.Count > 0)
                {
                    foreach (var url in tweetModel.entities.urls)
                    {
                        //uri = new Uri(url.expanded_url);
                        match = regex.Match(url.display_url).Value;
                        //url.domain = uri.Host ?? null;
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
            try
            {
                //await PopulateTweets();
                //TweetStatisticsModel model = new TweetStatisticsModel();
                TweetStatisticsModel model = await PopulateTweets();
                return model;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
    }
}

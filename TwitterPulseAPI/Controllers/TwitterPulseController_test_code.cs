using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Text;

namespace TwitterPulseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterPulseController_test_code : ControllerBase
    {
        private readonly ILogger<TwitterPulseController> _logger;
        public List<AccountModel> Accounts { get; set; }
        public TwitterPulseController_test_code(ILogger<TwitterPulseController> logger)
        {
            _logger = logger;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public IEnumerable<TweetModel> Get()
        {
            return GetTweets().Result;
            //return GetTwitterUsers();
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new AccountModel
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
        }

        //private List<TwitterUserModel> GetTwitterUsers()
        private async Task<List<TweetModel>> GetTweets()
        {
            List<TweetModel> myTweets = new List<TweetModel>();
            //string myUrl = "https://frontiercodingtests.azurewebsites.net/api/accounts/getall";
            //string myUrl = "https://api.twitter.com/2/users/by/username/ipsonr";
            /*{
            "data": {
                "id": "2896077921",
                    "name": "Ron Ipson",
                    "username": "ipsonr"
                }
            }*/

            string myUrl = "https://api.twitter.com/2/tweets/sample/stream";
            /*{
                "data": [
                          {
                            "author_id": "2244994945",
                            "created_at": "2020-02-14T19:00:55.000Z",
                            "id": "1228393702244134912",
                            "text": "What did the developer write in their Valentine’s card?\n  \nwhile(true) {\n    I = Love(You);  \n}"
                          },
                        {
                            "author_id": "2244994945",
                            "created_at": "2020-02-12T17:09:56.000Z",
                            "id": "1227640996038684673",
                            "text": "Doctors: Googling stuff online does not make you a doctor\n\nDevelopers: https://t.co/mrju5ypPkb"
                        },
                        {
                            "author_id": "2244994945",
                            "created_at": "2019-11-27T20:26:41.000Z",
                            "id": "1199786642791452673",
                            "text": "C#"
                        }
                    ],
                "includes": {
                    "users": [
                              {
                                "created_at": "2013-12-14T04:35:55.000Z",
                                "id": "2244994945",
                                "name": "Twitter Dev",
                                "username": "TwitterDev"
                              }
                            ]
                        }
            }*/


            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", @"AAAAAAAAAAAAAAAAAAAAABLlMwEAAAAAvdNPda7LiFxx7BsOHpB0LVMxysY%3DF0E3sqgHbGZC7igFDetGvRXOoMtcChZUrxGLl4pqraAQA5YMEy");

            //using (var client = new HttpClient())
            try
            {
                using (httpClient)
                {
                    //var response = httpClient.GetAsync(myUrl).Result;
                    //var response = await httpClient.GetAsync(myUrl);
                    //if (response.IsSuccessStatusCode)
                    //{
                        //var responseContent = response.Content;
                        //string responseString = responseContent.ReadAsStringAsync().Result;
                        //accounts = JsonConvert.DeserializeObject<List<TwitterUserModel>>(responseString);
                    //}
                    var myStream = await httpClient.GetStreamAsync(myUrl);
                    StringBuilder jsonStringBuilder = new StringBuilder();
                    //using (Stream myStream = response.Content.ReadAsStreamAsync().Result)
                    //using (myStream)
                    //char[] myBuffer = new char[1];
                    using (myStream)
                    {
                        StreamReader myReader = new StreamReader(myStream, System.Text.Encoding.UTF8);
                        //jsonString = myReader.ReadToEnd();
                        var myTweetLine = myReader.ReadLine();//does data have line breaks? - yes
                        /*jsonStringBuilder = 
{{"data":{"id":"1363700900435435523","text":"@nuckyzuu แงกอดกอดดด"}}
{"data":{ "id":"1363700900431204352","text":"道民だからこの時期の暑さが違和感で日差しが🌞上海暑いな。夏大丈夫か😅"} }
{"data":{"id":"1363704956327399424","text":"Grateful is an attitude that makes life easier."}}                        
                         */
                        if (!String.IsNullOrEmpty(myTweetLine))
                        {
                            var newTweet = GetTweetFromResponse(myTweetLine);
                            myTweets.Add(newTweet);
                        }

                        /*while (numChars > 0)
                        {
                            jsonStringBuilder.Append(myBuffer);
                            if (jsonStringBuilder.Length >= 200)
                                break;
                            numChars = myReader.Read(myBuffer);
                        }*/
                    }
                    //accounts = JsonConvert.DeserializeObject<List<TwitterUserModel>>(jsonStringBuilder.ToString());
                }
                return myTweets;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        private TweetModel GetTweetFromResponse(string responseString)
        {
            try
            {
                var myTweetString = responseString.Substring(8, responseString.Length - 9);
                return JsonConvert.DeserializeObject<TweetModel>(myTweetString.ToString());
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
    }
}

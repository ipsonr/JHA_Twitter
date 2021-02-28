using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITweetService _dataservice;
        public IEnumerable<TweetModel> SampleTweets { get; set; }
        public TweetStatisticsModel TweetStatistics { get; set; }
        //public IndexModel(ILogger<IndexModel> logger, ITweetService dataservice)
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            //_dataservice = dataservice;
            _dataservice = new InMemoryTweetService();
        }

        //public void OnGet()
        public async Task OnGet()
        {
            //_datastore.PopulateTweets();
            //SampleTweets = _datastore.GetSampleTweets(10);
            TweetStatistics = await _dataservice.GetStatisticsAsync();
            //TweetStatistics = _dataservice.GetStatisticsAsync().Result;
        }
    }
}

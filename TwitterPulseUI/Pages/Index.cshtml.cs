using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITweetService _dataservice;
        public IEnumerable<TweetModel> SampleTweets { get; set; }
        public TweetStatisticsModel TweetStatistics { get; set; }
        public IndexModel(ILogger<IndexModel> logger, ITweetService dataservice)
        {
            _logger = logger;
            _dataservice = dataservice;
        }

        public async Task OnGetAsync()
        {
            //SampleTweets = _datastore.GetSampleTweets(10);//for testing

            //this method works but it processes each tweet as it is read, slowing down the thread 
            TweetStatistics = await _dataservice.PopulateTweetsAndGetStats();

            //need to do thread line reads and processing of lines on separate threads
            //_dataservice.PopulateTweets();
            //await _dataservice.PopulateTweets();
            //TweetStatistics = await _dataservice.GetStatisticsAsync();
            //TweetStatistics = _dataservice.GetStatisticsAsync().Result;
        }
    }
}

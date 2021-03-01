using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;

namespace TwitterPulseAPI.Services.Interfaces
{
    public interface ITweetService
    {
        IEnumerable<TweetModel> GetSampleTweets(int numberOfTweets = 0);
        void Add(TweetModel tweetModel);
        Task PopulateTweets();
        Task<TweetStatisticsModel> GetStatisticsAsync();
        Task<TweetStatisticsModel> PopulateTweetsAndGetStats();
    }
}

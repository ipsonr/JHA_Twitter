using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseAPI.Services
{
    public class FileBasedTweetService : ITweetService
    {
        public void Add(TweetModel tweetModel)
        {
            throw new NotImplementedException();//TODO would build this if saving to a file on disk
        }

        public IEnumerable<TweetModel> GetSampleTweets(int numberOfTweets)
        {
            throw new NotImplementedException();
        }

        public TweetStatisticsModel GetStatisticsAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopulateTweets()
        {
            throw new NotImplementedException();
        }

        public Task<TweetStatisticsModel> PopulateTweetsAndGetStats()
        {
            throw new NotImplementedException();
        }

        Task<TweetStatisticsModel> ITweetService.GetStatisticsAsync()
        {
            throw new NotImplementedException();
        }
    }
}

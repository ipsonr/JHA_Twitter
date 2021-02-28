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
        Task<TweetStatisticsModel> PopulateTweets();
        Task<TweetStatisticsModel> GetStatisticsAsync();
        //TweetModel Get(int id);//would only be needed if doing full CRUD operations
        //void Update(TweetModel tweetModel);
        //void Delete(int id);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TwitterPulseAPI.Models;
using TwitterPulseAPI.Services.Interfaces;

namespace TwitterPulseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterPulseController : ControllerBase
    {
        private readonly ITweetService _datastore;
        private readonly ILogger<TwitterPulseController> _logger;
        public TwitterPulseController(ILogger<TwitterPulseController> logger, ITweetService datastore)
        {
            _logger = logger;
            _datastore = datastore;
        }

        [HttpGet]
        public IEnumerable<TweetModel> Get(int numberOfTweets)
        {
            return _datastore.GetSampleTweets(numberOfTweets);
        }
    }
}

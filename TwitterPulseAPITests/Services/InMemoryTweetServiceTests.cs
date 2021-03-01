using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TwitterPulseAPI.Models;
using Microsoft.Extensions.Configuration;
using Moq;

namespace TwitterPulseAPI.Services.Tests
{
    [TestClass()]
    public class InMemoryTweetServiceTests
    {
        InMemoryTweetService service; // = null; // new InMemoryTweetService(configuration);
        public InMemoryTweetServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"SampleUrl", "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=entities"},
                {"BearerToken", "AAAAAAAAAAAAAAAAAAAAABLlMwEAAAAAvdNPda7LiFxx7BsOHpB0LVMxysY%3DF0E3sqgHbGZC7igFDetGvRXOoMtcChZUrxGLl4pqraAQA5YMEy"},
                { "EmojiRegex", "\\uD83D(?:\\uDC68(?:\\uD83C(?:[\\uDFFB-\\uDFFF]\\u200D(?:\\u2764\\uFE0F?\\u200D\\uD83D(?:\\uDC8B\\u200D\\uD83D)?\\uDC68\\uD83C[\\uDFFB-\\uDFFF]|\\uD83E(?:\\uDD1D\\u200D\\uD83D\\uDC68\\uD83C[\\uDFFB-\\uDFFF]|[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83D[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92])|[\\uDFFB-\\uDFFF])|\\u200D(?:\\u2764\\uFE0F?\\u200D\\uD83D(?:\\uDC8B\\u200D\\uD83D)?\\uDC68|\\uD83D(?:(?:[\\uDC68\\uDC69]\\u200D\\uD83D)?(?:\\uDC66(?:\\u200D\\uD83D\\uDC66)?|\\uDC67(?:\\u200D\\uD83D[\\uDC66\\uDC67])?)|[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83E[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD]))?|\\uDC69(?:\\uD83C(?:[\\uDFFB-\\uDFFF]\\u200D(?:\\u2764\\uFE0F?\\u200D\\uD83D(?:\\uDC8B\\u200D\\uD83D[\\uDC68\\uDC69]|[\\uDC68\\uDC69])\\uD83C[\\uDFFB-\\uDFFF]|\\uD83E(?:\\uDD1D\\u200D\\uD83D[\\uDC68\\uDC69]\\uD83C[\\uDFFB-\\uDFFF]|[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83D[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92])|[\\uDFFB-\\uDFFF])|\\u200D(?:\\u2764\\uFE0F?\\u200D\\uD83D(?:\\uDC8B\\u200D\\uD83D[\\uDC68\\uDC69]|[\\uDC68\\uDC69])|\\uD83D(?:(?:\\uDC69\\u200D\\uD83D)?(?:\\uDC66(?:\\u200D\\uD83D\\uDC66)?|\\uDC67(?:\\u200D\\uD83D[\\uDC66\\uDC67])?)|[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83E[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD]))?|(?:\\uDD75(?:\\uD83C[\\uDFFB-\\uDFFF]|\\uFE0F)?|\\uDC6F)(?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|[\\uDC6E\\uDC70\\uDC71\\uDC73\\uDC77\\uDC81\\uDC82\\uDC86\\uDC87\\uDE45-\\uDE47\\uDE4B\\uDE4D\\uDE4E\\uDEA3\\uDEB4-\\uDEB6](?:\\uD83C[\\uDFFB-\\uDFFF](?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\uDC41(?:\\uFE0F(?:\\u200D\\uD83D\\uDDE8\\uFE0F?)?|\\u200D\\uD83D\\uDDE8\\uFE0F?)?|\\uDE36(?:\\u200D\\uD83C\\uDF2B\\uFE0F?)?|\\uDC15(?:\\u200D\\uD83E\\uDDBA)?|\\uDC3B(?:\\u200D\\u2744\\uFE0F?)?|\\uDE2E(?:\\u200D\\uD83D\\uDCA8)?|\\uDE35(?:\\u200D\\uD83D\\uDCAB)?|[\\uDC42\\uDC43\\uDC46-\\uDC50\\uDC66\\uDC67\\uDC6B-\\uDC6D\\uDC72\\uDC74-\\uDC76\\uDC78\\uDC7C\\uDC83\\uDC85\\uDC8F\\uDC91\\uDCAA\\uDD7A\\uDD95\\uDD96\\uDE4C\\uDE4F\\uDEC0\\uDECC](?:\\uD83C[\\uDFFB-\\uDFFF])?|[\\uDD74\\uDD90]\\uD83C[\\uDFFB-\\uDFFF]|\\uDC08(?:\\u200D\\u2B1B)?|[\\uDC3F\\uDCFD\\uDD49\\uDD4A\\uDD6F\\uDD70\\uDD73\\uDD74\\uDD76-\\uDD79\\uDD87\\uDD8A-\\uDD8D\\uDD90\\uDDA5\\uDDA8\\uDDB1\\uDDB2\\uDDBC\\uDDC2-\\uDDC4\\uDDD1-\\uDDD3\\uDDDC-\\uDDDE\\uDDE1\\uDDE3\\uDDE8\\uDDEF\\uDDF3\\uDDFA\\uDECB\\uDECD-\\uDECF\\uDEE0-\\uDEE5\\uDEE9\\uDEF0\\uDEF3]\\uFE0F?|[\\uDC00-\\uDC07\\uDC09-\\uDC14\\uDC16-\\uDC3A\\uDC3C-\\uDC3E\\uDC40\\uDC44\\uDC45\\uDC51-\\uDC65\\uDC6A\\uDC79-\\uDC7B\\uDC7D-\\uDC80\\uDC84\\uDC88-\\uDC8E\\uDC90\\uDC92-\\uDCA9\\uDCAB-\\uDCFC\\uDCFF-\\uDD3D\\uDD4B-\\uDD4E\\uDD50-\\uDD67\\uDDA4\\uDDFB-\\uDE2D\\uDE2F-\\uDE34\\uDE37-\\uDE44\\uDE48-\\uDE4A\\uDE80-\\uDEA2\\uDEA4-\\uDEB3\\uDEB7-\\uDEBF\\uDEC1-\\uDEC5\\uDED0-\\uDED2\\uDED5-\\uDED7\\uDEEB\\uDEEC\\uDEF4-\\uDEFC\\uDFE0-\\uDFEB])|\\uD83E(?:\\uDDD1(?:\\uD83C(?:[\\uDFFB-\\uDFFF]\\u200D(?:\\u2764\\uFE0F?\\u200D(?:\\uD83D\\uDC8B\\u200D)?\\uD83E\\uDDD1\\uD83C[\\uDFFB-\\uDFFF]|\\uD83E(?:\\uDD1D\\u200D\\uD83E\\uDDD1\\uD83C[\\uDFFB-\\uDFFF]|[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF84\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83D[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92])|[\\uDFFB-\\uDFFF])|\\u200D(?:\\uD83E(?:\\uDD1D\\u200D\\uD83E\\uDDD1|[\\uDDAF-\\uDDB3\\uDDBC\\uDDBD])|[\\u2695\\u2696\\u2708]\\uFE0F?|\\uD83C[\\uDF3E\\uDF73\\uDF7C\\uDF84\\uDF93\\uDFA4\\uDFA8\\uDFEB\\uDFED]|\\uD83D[\\uDCBB\\uDCBC\\uDD27\\uDD2C\\uDE80\\uDE92]))?|[\\uDD26\\uDD35\\uDD37-\\uDD39\\uDD3D\\uDD3E\\uDDB8\\uDDB9\\uDDCD-\\uDDCF\\uDDD4\\uDDD6-\\uDDDD](?:\\uD83C[\\uDFFB-\\uDFFF](?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\u200D[\\u2640\\u2642]\\uFE0F?)?|[\\uDD3C\\uDDDE\\uDDDF](?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|[\\uDD0C\\uDD0F\\uDD18-\\uDD1C\\uDD1E\\uDD1F\\uDD30-\\uDD34\\uDD36\\uDD77\\uDDB5\\uDDB6\\uDDBB\\uDDD2\\uDDD3\\uDDD5](?:\\uD83C[\\uDFFB-\\uDFFF])?|[\\uDD0D\\uDD0E\\uDD10-\\uDD17\\uDD1D\\uDD20-\\uDD25\\uDD27-\\uDD2F\\uDD3A\\uDD3F-\\uDD45\\uDD47-\\uDD76\\uDD78\\uDD7A-\\uDDB4\\uDDB7\\uDDBA\\uDDBC-\\uDDCB\\uDDD0\\uDDE0-\\uDDFF\\uDE70-\\uDE74\\uDE78-\\uDE7A\\uDE80-\\uDE86\\uDE90-\\uDEA8\\uDEB0-\\uDEB6\\uDEC0-\\uDEC2\\uDED0-\\uDED6])|\\uD83C(?:\\uDFF4(?:\\uDB40\\uDC67\\uDB40\\uDC62\\uDB40(?:\\uDC65\\uDB40\\uDC6E\\uDB40\\uDC67|\\uDC73\\uDB40\\uDC63\\uDB40\\uDC74|\\uDC77\\uDB40\\uDC6C\\uDB40\\uDC73)\\uDB40\\uDC7F|\\u200D\\u2620\\uFE0F?)?|[\\uDFC3\\uDFC4\\uDFCA](?:\\uD83C[\\uDFFB-\\uDFFF](?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\u200D[\\u2640\\u2642]\\uFE0F?)?|[\\uDFCB\\uDFCC](?:\\uD83C[\\uDFFB-\\uDFFF]|\\uFE0F)(?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\uDFF3(?:\\uFE0F(?:\\u200D(?:\\u26A7\\uFE0F?|\\uD83C\\uDF08))?|\\u200D(?:\\u26A7\\uFE0F?|\\uD83C\\uDF08))?|(?:[\\uDFCB\\uDFCC]\\u200D[\\u2640\\u2642]|[\\uDD70\\uDD71\\uDD7E\\uDD7F\\uDE02\\uDE37\\uDF21\\uDF24-\\uDF2C\\uDF36\\uDF7D\\uDF96\\uDF97\\uDF99-\\uDF9B\\uDF9E\\uDF9F\\uDFCD\\uDFCE\\uDFD4-\\uDFDF\\uDFF5\\uDFF7])\\uFE0F?|[\\uDF85\\uDFC2\\uDFC7](?:\\uD83C[\\uDFFB-\\uDFFF])?|\\uDDE6\\uD83C[\\uDDE8-\\uDDEC\\uDDEE\\uDDF1\\uDDF2\\uDDF4\\uDDF6-\\uDDFA\\uDDFC\\uDDFD\\uDDFF]|\\uDDE7\\uD83C[\\uDDE6\\uDDE7\\uDDE9-\\uDDEF\\uDDF1-\\uDDF4\\uDDF6-\\uDDF9\\uDDFB\\uDDFC\\uDDFE\\uDDFF]|\\uDDE8\\uD83C[\\uDDE6\\uDDE8\\uDDE9\\uDDEB-\\uDDEE\\uDDF0-\\uDDF5\\uDDF7\\uDDFA-\\uDDFF]|\\uDDE9\\uD83C[\\uDDEA\\uDDEC\\uDDEF\\uDDF0\\uDDF2\\uDDF4\\uDDFF]|\\uDDEA\\uD83C[\\uDDE6\\uDDE8\\uDDEA\\uDDEC\\uDDED\\uDDF7-\\uDDFA]|\\uDDEB\\uD83C[\\uDDEE-\\uDDF0\\uDDF2\\uDDF4\\uDDF7]|\\uDDEC\\uD83C[\\uDDE6\\uDDE7\\uDDE9-\\uDDEE\\uDDF1-\\uDDF3\\uDDF5-\\uDDFA\\uDDFC\\uDDFE]|\\uDDED\\uD83C[\\uDDF0\\uDDF2\\uDDF3\\uDDF7\\uDDF9\\uDDFA]|\\uDDEE\\uD83C[\\uDDE8-\\uDDEA\\uDDF1-\\uDDF4\\uDDF6-\\uDDF9]|\\uDDEF\\uD83C[\\uDDEA\\uDDF2\\uDDF4\\uDDF5]|\\uDDF0\\uD83C[\\uDDEA\\uDDEC-\\uDDEE\\uDDF2\\uDDF3\\uDDF5\\uDDF7\\uDDFC\\uDDFE\\uDDFF]|\\uDDF1\\uD83C[\\uDDE6-\\uDDE8\\uDDEE\\uDDF0\\uDDF7-\\uDDFB\\uDDFE]|\\uDDF2\\uD83C[\\uDDE6\\uDDE8-\\uDDED\\uDDF0-\\uDDFF]|\\uDDF3\\uD83C[\\uDDE6\\uDDE8\\uDDEA-\\uDDEC\\uDDEE\\uDDF1\\uDDF4\\uDDF5\\uDDF7\\uDDFA\\uDDFF]|\\uDDF4\\uD83C\\uDDF2|\\uDDF5\\uD83C[\\uDDE6\\uDDEA-\\uDDED\\uDDF0-\\uDDF3\\uDDF7-\\uDDF9\\uDDFC\\uDDFE]|\\uDDF6\\uD83C\\uDDE6|\\uDDF7\\uD83C[\\uDDEA\\uDDF4\\uDDF8\\uDDFA\\uDDFC]|\\uDDF8\\uD83C[\\uDDE6-\\uDDEA\\uDDEC-\\uDDF4\\uDDF7-\\uDDF9\\uDDFB\\uDDFD-\\uDDFF]|\\uDDF9\\uD83C[\\uDDE6\\uDDE8\\uDDE9\\uDDEB-\\uDDED\\uDDEF-\\uDDF4\\uDDF7\\uDDF9\\uDDFB\\uDDFC\\uDDFF]|\\uDDFA\\uD83C[\\uDDE6\\uDDEC\\uDDF2\\uDDF3\\uDDF8\\uDDFE\\uDDFF]|\\uDDFB\\uD83C[\\uDDE6\\uDDE8\\uDDEA\\uDDEC\\uDDEE\\uDDF3\\uDDFA]|\\uDDFC\\uD83C[\\uDDEB\\uDDF8]|\\uDDFD\\uD83C\\uDDF0|\\uDDFE\\uD83C[\\uDDEA\\uDDF9]|\\uDDFF\\uD83C[\\uDDE6\\uDDF2\\uDDFC]|[\\uDC04\\uDCCF\\uDD8E\\uDD91-\\uDD9A\\uDE01\\uDE1A\\uDE2F\\uDE32-\\uDE36\\uDE38-\\uDE3A\\uDE50\\uDE51\\uDF00-\\uDF20\\uDF2D-\\uDF35\\uDF37-\\uDF7C\\uDF7E-\\uDF84\\uDF86-\\uDF93\\uDFA0-\\uDFC1\\uDFC5\\uDFC6\\uDFC8\\uDFC9\\uDFCB\\uDFCC\\uDFCF-\\uDFD3\\uDFE0-\\uDFF0\\uDFF8-\\uDFFF])|\\u26F9(?:(?:\\uD83C[\\uDFFB-\\uDFFF]|\\uFE0F)(?:\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\u200D[\\u2640\\u2642]\\uFE0F?)?|\\u2764(?:\\uFE0F(?:\\u200D(?:\\uD83D\\uDD25|\\uD83E\\uDE79))?|\\u200D(?:\\uD83D\\uDD25|\\uD83E\\uDE79))?|[\\#\\*0-9]\\uFE0F?\\u20E3|[\\u261D\\u270C\\u270D]\\uD83C[\\uDFFB-\\uDFFF]|[\\u270A\\u270B](?:\\uD83C[\\uDFFB-\\uDFFF])?|[\\u00A9\\u00AE\\u203C\\u2049\\u2122\\u2139\\u2194-\\u2199\\u21A9\\u21AA\\u2328\\u23CF\\u23ED-\\u23EF\\u23F1\\u23F2\\u23F8-\\u23FA\\u24C2\\u25AA\\u25AB\\u25B6\\u25C0\\u25FB\\u25FC\\u2600-\\u2604\\u260E\\u2611\\u2618\\u261D\\u2620\\u2622\\u2623\\u2626\\u262A\\u262E\\u262F\\u2638-\\u263A\\u2640\\u2642\\u265F\\u2660\\u2663\\u2665\\u2666\\u2668\\u267B\\u267E\\u2692\\u2694-\\u2697\\u2699\\u269B\\u269C\\u26A0\\u26A7\\u26B0\\u26B1\\u26C8\\u26CF\\u26D1\\u26D3\\u26E9\\u26F0\\u26F1\\u26F4\\u26F7\\u26F8\\u2702\\u2708\\u2709\\u270C\\u270D\\u270F\\u2712\\u2714\\u2716\\u271D\\u2721\\u2733\\u2734\\u2744\\u2747\\u2763\\u27A1\\u2934\\u2935\\u2B05-\\u2B07\\u3030\\u303D\\u3297\\u3299]\\uFE0F?|[\\u231A\\u231B\\u23E9-\\u23EC\\u23F0\\u23F3\\u25FD\\u25FE\\u2614\\u2615\\u2648-\\u2653\\u267F\\u2693\\u26A1\\u26AA\\u26AB\\u26BD\\u26BE\\u26C4\\u26C5\\u26CE\\u26D4\\u26EA\\u26F2\\u26F3\\u26F5\\u26FA\\u26FD\\u2705\\u2728\\u274C\\u274E\\u2753-\\u2755\\u2757\\u2795-\\u2797\\u27B0\\u27BF\\u2B1B\\u2B1C\\u2B50\\u2B55]" }
                };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            service = new InMemoryTweetService(configuration);
        }

        //Arrange
        string tweet = @"{""data"":{""id"":""1363704956327399424"",""text"":""Grateful is an attitude that makes life easier.""}}";
        string tweetBad = @"{""data"":{""id"":""123"",""text"":""Missing closing bracket.""}";
        string tweet0 = @"{""data"":{""entities"":{""urls"":[{""start"":21,""end"":44,""url"":""https://t.co/GDnLvFP4lj"",""expanded_url"":""https://twitter.com/onechicagoedits/status/1366230843690221571/photo/1"",""display_url"":""pic.twitter.com/GDnLvFP4lj""}],""mentions"":[{""start"":3,""end"":19,""username"":""onechicagoedits""}]},""id"":""1366242321524789248"",""text"":""RT @onechicagoedits: https://t.co/GDnLvFP4lj""}}";
        string tweet2 = @"{""data"":{ ""id"":""1363700900431204352"",""text"":""道民だからこの時期の暑さが違和感で日差しが🌞上海暑いな。夏大丈夫か😅""} }";
        string tweet3 = @"{""data"":{""id"":""1366213489857994758"",""text"":""@774FF14 ありがっとん！✨\\n今までにない体験だったのでめちゃ楽しかったです！😏🎶"",""entities"":{""mentions"":[{""start"":0,""end"":8,""username"":""774FF14""}]}}}";
        string tweet4 = @"{""data"":{""entities"":{""hashtags"":[{""start"":168,""end"":180,""tag"":""Modi_MSP_Do""},{""start"":182,""end"":203,""tag"":""बिहार_बेरोजगारी_दिवस""}],""mentions"":[{""start"":0,""end"":9,""username"":""prk_0143""},{""start"":10,""end"":25,""username"":""Mustak_Malarna""},{""start"":26,""end"":36,""username"":""Ms_Marmat""},{""start"":37,""end"":49,""username"":""Tribal_Rj29""},{""start"":50,""end"":62,""username"":""RAJUUPMAN14""},{""start"":63,""end"":77,""username"":""Lucky_meena93""},{""start"":78,""end"":93,""username"":""PriyaAmbedkar1""},{""start"":94,""end"":109,""username"":""varsha20475929""},{""start"":110,""end"":126,""username"":""GanpatSingh1535""},{""start"":127,""end"":139,""username"":""Rp_Sattawan""},{""start"":140,""end"":150,""username"":""YogeshM39""},{""start"":151,""end"":167,""username"":""RaviSha61295214""}]},""id"":""1366213489853784066"",""text"":""@prk_0143 @Mustak_Malarna @Ms_Marmat @Tribal_Rj29 @RAJUUPMAN14 @Lucky_meena93 @PriyaAmbedkar1 @varsha20475929 @GanpatSingh1535 @Rp_Sattawan @YogeshM39 @RaviSha61295214 #Modi_MSP_Do \\n#बिहार_बेरोजगारी_दिवस""}}";
        string tweet5 = @"{""data"":{""entities"":{""urls"":[{""start"":38,""end"":61,""url"":""https://t.co/iwrr46NHso"",""expanded_url"":""https://twitter.com/kamquat/status/1366203664205930496/photo/1"",""display_url"":""pic.twitter.com/iwrr46NHso""}],""hashtags"":[{""start"":34,""end"":37,""tag"":""原神""}],""mentions"":[{""start"":3,""end"":11,""username"":""kamquat""}]},""id"":""1366213489891573763"",""text"":""RT @kamquat: shady dr baizhu 👁👄👁 \\n#原神 https://t.co/iwrr46NHso""}}";
        string tweet6 = @"{""data"":{""entities"":{""hashtags"":[{""start"":20,""end"":27,""tag"":""BBB211""},{""start"":28,""end"":34,""tag"":""BBB21""}]},""id"":""1366213494081748996"",""text"":""Os cambito da pocah #BBB211 #BBB21""}}";

        [TestMethod()]
        public void GetTweetFromResponseTests()
        {
            //Arrange

            //Act
            var myTweetModel = InMemoryTweetService.GetTweetFromResponse(tweet);
            var myTweetModelBad = InMemoryTweetService.GetTweetFromResponse(tweetBad);
            var myTweetModel0 = InMemoryTweetService.GetTweetFromResponse(tweet0);
            var myTweetModel2 = InMemoryTweetService.GetTweetFromResponse(tweet2);
            var myTweetModel3 = InMemoryTweetService.GetTweetFromResponse(tweet3);
            var myTweetModel4 = InMemoryTweetService.GetTweetFromResponse(tweet4);
            var myTweetModel5 = InMemoryTweetService.GetTweetFromResponse(tweet5);
            var myTweetModel6 = InMemoryTweetService.GetTweetFromResponse(tweet6);

            //Assert
            Assert.IsNull(myTweetModelBad);

            Assert.IsNotNull(myTweetModel);
            Assert.AreEqual(0, myTweetModel.entities.hashtags.Count);
            Assert.AreEqual(0, myTweetModel.entities.urls.Count);

            Assert.IsNotNull(myTweetModel0);
            Assert.AreEqual(0, myTweetModel0.entities.hashtags.Count);
            Assert.AreEqual(1, myTweetModel0.entities.urls.Count);

            Assert.IsNotNull(myTweetModel2);
            Assert.AreEqual(0, myTweetModel2.entities.hashtags.Count);
            Assert.AreEqual(0, myTweetModel2.entities.urls.Count);

            Assert.IsNotNull(myTweetModel3);
            Assert.AreEqual(0, myTweetModel3.entities.hashtags.Count);
            Assert.AreEqual(0, myTweetModel3.entities.urls.Count);

            Assert.IsNotNull(myTweetModel4);
            Assert.AreEqual(2, myTweetModel4.entities.hashtags.Count);
            Assert.AreEqual(0, myTweetModel4.entities.urls.Count);

            Assert.IsNotNull(myTweetModel5);
            Assert.AreEqual(1, myTweetModel5.entities.hashtags.Count);
            Assert.AreEqual(1, myTweetModel5.entities.urls.Count);

            Assert.IsNotNull(myTweetModel6);
            Assert.AreEqual(2, myTweetModel6.entities.hashtags.Count);
            Assert.AreEqual(0, myTweetModel6.entities.urls.Count);

            //Act
            TweetStatisticsModel model = new TweetStatisticsModel();
            service.UpdateStats(model,myTweetModel);
            //Asert
            Assert.AreEqual(1, model.CountTweetsReceived);
            Assert.AreEqual(0, model.CountTweetsWithEmojis);
            Assert.AreEqual(0, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(0, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel0);
            //Asert
            Assert.AreEqual(2, model.CountTweetsReceived);
            Assert.AreEqual(0, model.CountTweetsWithEmojis);
            Assert.AreEqual(1, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(1, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel2);
            //Asert
            Assert.AreEqual(3, model.CountTweetsReceived);
            Assert.AreEqual(1, model.CountTweetsWithEmojis);
            Assert.AreEqual(1, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(1, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel3);
            //Asert
            Assert.AreEqual(4, model.CountTweetsReceived);
            Assert.AreEqual(2, model.CountTweetsWithEmojis);
            Assert.AreEqual(1, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(1, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel4);
            //Asert
            Assert.AreEqual(5, model.CountTweetsReceived);
            Assert.AreEqual(2, model.CountTweetsWithEmojis);
            Assert.AreEqual(1, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(1, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel5);
            //Asert
            Assert.AreEqual(6, model.CountTweetsReceived);
            Assert.AreEqual(3, model.CountTweetsWithEmojis);
            Assert.AreEqual(2, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(2, model.CountTweetsWithUrls);

            //Act
            service.UpdateStats(model, myTweetModel6);
            //Asert
            Assert.AreEqual(7, model.CountTweetsReceived);
            Assert.AreEqual(3, model.CountTweetsWithEmojis);
            Assert.AreEqual(2, model.CountTweetsWithPhotoUrls);
            Assert.AreEqual(2, model.CountTweetsWithUrls);

            Assert.AreEqual(7, model.EmojisUsed.Count);
            Assert.AreEqual(5, model.HashtagsUsed.Count);
            Assert.AreEqual(1, model.UrlDomainsUsed.Count);
        }

    }
}
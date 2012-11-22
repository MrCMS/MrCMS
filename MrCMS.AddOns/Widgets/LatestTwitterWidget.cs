using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using LinqToTwitter;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class LatestTwitterWidget : Widget
    {
        [DisplayName("User Name")]
        public virtual string UserName { get; set; }
        public virtual int Count { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var tweets = HttpContext.Current.Cache["latest-twitter"] as List<Tweet>;
            if (tweets == null)
            {
                using (var twitterContext = new TwitterContext())
                {
                    tweets = twitterContext.Status.Where(
                        tweet => tweet.Type == StatusType.User && tweet.ScreenName == UserName).Take
                        (Count).ToList().Select(status => new Tweet(status)).ToList();


                    foreach (var tweet in tweets)
                    {
                        tweet.Text = tweet.Text.ParseTweet().ToString();
                    }
                    


                    HttpContext.Current.Cache.Insert("latest-twitter", tweets, null, DateTime.MaxValue,
                                                     new TimeSpan(0, 10, 0), CacheItemPriority.AboveNormal, null);

                }
            }
            return tweets;
        }
    }

    public class Tweet
    {
        public Tweet(Status status)
        {
            ScreenName = status.ScreenName;
            Text = status.Text;
            CreatedAt = status.CreatedAt;
        }

        public string ScreenName { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

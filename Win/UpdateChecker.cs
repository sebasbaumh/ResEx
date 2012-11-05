using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ResEx.Win
{
    public class UpdateChecker
    {
        public enum UpdateCheckResult
        {
            Nothing,
            NewRelease,
            NewBeta
        }

        public static UpdateCheckResult Check(string inputUri, bool includeBetas, DateTime releaseDateOfCurrentVersion)
        {
            return Check(XmlReader.Create(inputUri), includeBetas, releaseDateOfCurrentVersion);
        }

        public static UpdateCheckResult Check(XmlReader reader, bool includeBetas, DateTime releaseDateOfCurrentVersion)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            if (feed == null) throw new InvalidOperationException("Failed to load update feed. Loading returned null.");
            var items = feed.Items.Where(p => p.Title.Text.StartsWith("Released:") && p.PublishDate > releaseDateOfCurrentVersion);
            if (!includeBetas) items = items.Where(p => !p.Title.Text.Contains("Beta"));
            var lastItem = items.OrderByDescending(p => p.PublishDate).FirstOrDefault();
            if (lastItem == null)
            {
                return UpdateCheckResult.Nothing;
            }
            else
            {
                return lastItem.Title.Text.Contains("Beta") ? UpdateCheckResult.NewBeta : UpdateCheckResult.NewRelease;
            }
        }
    }
}

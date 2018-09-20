using System;
using System.Configuration;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
    public class Configuration
    {
        /// <summary>
        /// Thumbprint of certificate used to connect to xConnect endpoint
        /// </summary>
        public string Thumbprint => ConfigurationManager.AppSettings["xConnectCertificateThumbprint"];
        /// <summary>
        /// Base URL of xConnect installation. Used for opening connections.
        /// </summary>
        public string XConnectUrl => ConfigurationManager.ConnectionStrings["xConnectUrl"].ConnectionString;

        /// <summary>
        /// Timeout for xConnect connection
        /// </summary>
        public string XConnectTimeout => ConfigurationManager.AppSettings["xConnectTimeout"];

        /// <summary>
        /// Base Twitter Identifier to be used when generating new contacts
        /// </summary>
        public  string TwitterIdentifier =>ConfigurationManager.AppSettings["TwitterIdentifier"];

        /// <summary>
        /// The Sitecore Item ID of the "Other Event" channel in your Sitecore database:
        ///   PATH: /sitecore/system/Marketing Control Panel/Taxonomies/Channel/Offline/Event/Other event
        /// </summary>
        public  string OtherEventChannelId => ConfigurationManager.AppSettings["OtherEventChannelId"];

        /// <summary>
        /// The Sitecore Item ID of the "Instant Demo" goal in your Sitecore database:
        ///   PATH: /sitecore/system/Marketing Control Panel/Goals/Instant Demo
        /// </summary>
        public  string InstantDemoGoalId => ConfigurationManager.AppSettings["InstantDemoGoalId"];

        /// <summary>
        /// The display name for the instant demo goal. Stored in the definition in the Reference Data tables.
        /// </summary>
        public  string InstantDemoGoalName => ConfigurationManager.AppSettings["InstantDemoGoalName"];

        /// <summary>
        /// The definition type name for Sitecore Goals
        /// </summary>
        public  string GoalTypeName => ConfigurationManager.AppSettings["GoalTypeName"];

        /// <summary>
        /// Search parameters for starting interaction searches (year, month, day)
        /// </summary>

        public int SearchYear => Convert.ToInt32(ConfigurationManager.AppSettings["SearchYear"]);
        public  int SearchMonth  => Convert.ToInt32(ConfigurationManager.AppSettings["SearchMonth"]);
        public  int SearchStartDay  => Convert.ToInt32(ConfigurationManager.AppSettings["SearchStartDay"]); 
        public  int SearchDays  => Convert.ToInt32(ConfigurationManager.AppSettings["SearchDays"]); 
    }
}

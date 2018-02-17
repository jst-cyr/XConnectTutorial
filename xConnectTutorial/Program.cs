using System;
using System.Threading.Tasks;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System.Linq;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This Program and related classes is largely inspired and based off of the work of Martina Wehlander in the Sitecore xConnect Tutorials:
	/// <see cref="https://doc.sitecore.net/developers/xp/getting-started/#tutorials-xconnect"/>
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Thumbprint of certificate used to connect to xConnect endpoint
		/// </summary>
		public const string Thumbprint = "1D48BBD0ABB629F283787BEDD1227184473D1B76";

		/// <summary>
		/// Base URL of xConnect installation. Used for opening connections.
		/// </summary>
		public const string XConnectUrl = "https://sc9.xconnect.local";

		/// <summary>
		/// Base Twitter Identifier to be used when generating new contacts
		/// </summary>
		public const string TwitterIdentifier = "myrtlesitecore";

		/// <summary>
		/// The Sitecore Item ID of the "Other Event" channel in your Sitecore database:
		///   PATH: /sitecore/system/Marketing Control Panel/Taxonomies/Channel/Offline/Event/Other event
		/// </summary>
		public const string OtherEventChannelId = "670BB98B-B352-40C1-99C8-880BF2AA4C54";

		/// <summary>
		/// The Sitecore Item ID of the "Instant Demo" goal in your Sitecore database:
		///   PATH: /sitecore/system/Marketing Control Panel/Goals/Instant Demo
		/// </summary>
		public const string InstantDemoGoalId = "28A7C944-B8B6-45AD-A635-6F72E8F81F69";

		/// <summary>
		/// The display name for the instant demo goal. Stored in the definition in the Reference Data tables.
		/// </summary>
		public const string InstantDemoGoalName = "Instant Demo";

		/// <summary>
		/// The definition type name for Sitecore Goals
		/// </summary>
		public const string GoalTypeName = "Sitecore XP Goal";

		/// <summary>
		/// Search parameters for starting interaction searches (year, month, day)
		/// </summary>
		public const int SearchYear = 2018;
		public const int SearchMonth = 1;
		public const int SearchStartDay = 20;
		public const int SearchDays = 10;

		private static void Main(string[] args)
		{
			MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("");
			Console.WriteLine("END OF PROGRAM.");
			Console.WriteLine("Press any key to close the console.");
			Console.ReadKey();
		}

		private static async Task MainAsync(string[] args)
		{
			//Initialize required handlers
			var outputHandler = new OutputHandler();
			var interactionManager = new InteractionManager() { Logger = outputHandler };
			var contactManager = new ContactManager() { Logger = outputHandler };
			var referenceDataManager = new ReferenceDataManager() { Logger = outputHandler };

			//Initialize IP information which will be used for tracking events.
			var ipInfo = new IpInfo("127.0.0.1");
			ipInfo.BusinessName = "Home";

			/**
			 * TUTORIAL: Building the configuration used to connect to xConnect
			 */
			var cfg = new ConfigurationBuilder().GetClientConfiguration(XConnectUrl, XConnectUrl, XConnectUrl, Thumbprint);
			
			//Test configuration
			try
			{
				await cfg.InitializeAsync();

				// Print xConnect validation message if configuration is valid
				outputHandler.WriteValidationMessage();
			}
			catch (XdbModelConflictException ce)
			{
				outputHandler.WriteError("Error initializing configuration", ce);
				return;
			}


			/**
			 * TUTORIAL: Create and retrieve a contact
			 */
			//Create a contact
			var twitterId = TwitterIdentifier + Guid.NewGuid().ToString("N");
			var identifier = await contactManager.CreateContact(cfg, twitterId);

			//Retrieve a contact that was created
			var contact = await contactManager.GetContact(cfg, twitterId);


			/**
			 * TUTORIAL: Register a goal for the created contact
			 */
			var interaction = await interactionManager.RegisterGoalInteraction(cfg, contact, OtherEventChannelId, InstantDemoGoalId, ipInfo);


			/**
			 * TUTORIAL: Reference Data Manager
			 */
			var definition = await referenceDataManager.GetDefinition(GoalTypeName, InstantDemoGoalId, XConnectUrl, Thumbprint);
			if (definition == null)
			{
				definition = await referenceDataManager.CreateDefinition(GoalTypeName, InstantDemoGoalId, InstantDemoGoalName, XConnectUrl, Thumbprint);
			}


			/**
			 * TUTORIAL: Get a contact with its list of interactions
			 */
			//Get a contact with the interactions
			contact = await contactManager.GetContactWithInteractions(cfg, twitterId, DateTime.MinValue, DateTime.MaxValue);


			/**
			 * TUTORIAL: Search Interactions
			 */
			//Find all interactions created in a specific date range. Note that dates are required in UTC or local time
			var startDate = new DateTime(SearchYear, SearchMonth, SearchStartDay).ToUniversalTime();
			var endDate = startDate.AddDays(SearchDays);
			var interactions = await interactionManager.SearchInteractionsByDate(cfg, startDate, endDate);


			/**
			 * TUTORIAL: Expand a single interaction search results
			 */
			//Look for the first result in the search results
			var interactionResult = interactions != null ? interactions.LastOrDefault() : null;
			//If the search result has sufficient contact and interaction ID data present, request the expanded details about the interaction
			if (interactionResult != null && interactionResult.Contact != null && interactionResult.Contact.Id.HasValue && interactionResult.Id.HasValue) {
				var triggeredGoal = await interactionManager.GetInteraction(cfg, interactionResult.Contact.Id.Value, interactionResult.Id.Value);
			}
		}
	}
}
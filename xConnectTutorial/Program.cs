using System;
using System.Threading.Tasks;
using Sitecore.XConnect.Client;

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
		/// The Sitecore Item ID of the "Instat Demo" goal in your Sitecore database:
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
			//Get output handler
			var outputHandler = new OutputHandler();

			//Build a configuration to use to connect to xConnect
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

			//Create a contact
			var twitterId = TwitterIdentifier + Guid.NewGuid().ToString("N");
			var identifier = await ContactManager.CreateContact(cfg, twitterId, outputHandler);

			//Get the contact that was created
			var contact = await ContactManager.GetContact(cfg, twitterId, outputHandler);

			//Create an interaction for the contact
			var interaction = await InteractionManager.RegisterGoalInteraction(cfg, contact, OtherEventChannelId, InstantDemoGoalId, outputHandler);

			//Ensure our goal is defined in the Reference Data database
			var definition = await ReferenceDataManager.GetDefinition(GoalTypeName, InstantDemoGoalId, XConnectUrl, Thumbprint, outputHandler);
			if (definition == null)
			{
				definition = await ReferenceDataManager.CreateDefinition(GoalTypeName, InstantDemoGoalId, InstantDemoGoalName, XConnectUrl, Thumbprint, outputHandler);
			}

			//Get a contact with the interactions
			contact = await ContactManager.GetContactWithInteractions(cfg, twitterId, DateTime.MinValue, DateTime.MaxValue, outputHandler);
		}
	}
}
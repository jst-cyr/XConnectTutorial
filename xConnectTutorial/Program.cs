using System;
using System.Threading.Tasks;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This Program and related classes is largely inspired and based off of the work of Martina Wehlander in the Sitecore xConnect Tutorials:
	/// <see cref="https://doc.sitecore.net/developers/xp/getting-started/#tutorials-xconnect"/>
	/// </summary>
	internal class Program
	{
		


        
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
            var configuration = new Configuration();
            //Initialize IP information which will be used for tracking events.
            var ipInfo = new IpInfo("127.0.0.1") {BusinessName = "Home"};

		    //Build a configuration to use to connect to xConnect
			var cfg = new ConfigurationBuilder().GetClientConfiguration(configuration.XConnectUrl, configuration.XConnectUrl, configuration.XConnectUrl, configuration.Thumbprint);
			
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
			var twitterId = configuration.TwitterIdentifier + Guid.NewGuid().ToString("N");
			var identifier = await contactManager.CreateContact(cfg, twitterId);

			//Get the contact that was created
			var contact = await contactManager.GetContact(cfg, twitterId);

			//Create an interaction for the contact
			var interaction = await interactionManager.RegisterGoalInteraction(cfg, contact, configuration.OtherEventChannelId, configuration.InstantDemoGoalId, ipInfo);

			//Ensure our goal is defined in the Reference Data database
			var definition = await referenceDataManager.GetDefinition(configuration.GoalTypeName, configuration.InstantDemoGoalId, configuration.XConnectUrl, configuration.Thumbprint) ??
			                 await referenceDataManager.CreateDefinition(configuration.GoalTypeName, configuration.InstantDemoGoalId, configuration.InstantDemoGoalName, configuration.XConnectUrl, configuration.Thumbprint);

		    //Get a contact with the interactions
			contact = await contactManager.GetContactWithInteractions(cfg, twitterId, DateTime.MinValue, DateTime.MaxValue);

			//Find all interactions created in a specific date range. Note that dates are required in UTC or local time
			var startDate = new DateTime(configuration.SearchYear, configuration.SearchMonth, configuration.SearchStartDay).ToUniversalTime();
			var endDate = startDate.AddDays(configuration.SearchDays);
			var interactions = await interactionManager.SearchInteractionsByDate(cfg, startDate, endDate);
		}
	}
}
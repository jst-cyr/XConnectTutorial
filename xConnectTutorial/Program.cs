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

			/**
			 * TUTORIAL: Building the configuration used to connect to xConnect
			 */
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


			/**
			 * TUTORIAL: Create and retrieve a contact
			 */
			//Create a contact
			var twitterId = configuration.TwitterIdentifier + Guid.NewGuid().ToString("N");
			var identifier = await contactManager.CreateContact(cfg, twitterId);

			//Retrieve a contact that was created
			var contact = await contactManager.GetContact(cfg, twitterId);

			/**
			 * TUTORIAL: Update an existing contact
			*/
			//Update the personal information about a Contact
			PersonalInformation updatedPersonalInformation = new PersonalInformation()
			{
				JobTitle = "Senior Programmer Writer"
			};
			var updatedContact = await contactManager.UpdateContact(cfg, twitterId, updatedPersonalInformation);


			/**
			 * TUTORIAL: Register a goal for the created contact
			 */
			var interaction = await interactionManager.RegisterGoalInteraction(cfg, contact, configuration.OtherEventChannelId, configuration.InstantDemoGoalId, ipInfo);


			/**
			 * TUTORIAL: Reference Data Manager
			 */
			var definition = await referenceDataManager.GetDefinition(configuration.GoalTypeName, configuration.InstantDemoGoalId, configuration.XConnectUrl, configuration.Thumbprint);
			if (definition == null)
			{
				definition = await referenceDataManager.CreateDefinition(configuration.GoalTypeName, configuration.InstantDemoGoalId, configuration.InstantDemoGoalName, configuration.XConnectUrl, configuration.Thumbprint);
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
			var startDate = new DateTime(configuration.SearchYear, configuration.SearchMonth, configuration.SearchStartDay).ToUniversalTime();
			var endDate = startDate.AddDays(configuration.SearchDays);
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
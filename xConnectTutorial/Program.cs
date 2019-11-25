using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.XConnect;
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

			/**
			 * TUTORIAL: Delete a single Contact from the database
			 */
			var deletedContact = await contactManager.DeleteContact(cfg, twitterId);


			/**
			 * TUTORIAL: Create a batch of Contacts with old interactions, then find all contacts with no interactions since the configured search period. Then delete these inactive Contacts!
			 */
			await DeletingMultipleContactsTutorial(cfg);
		}


		/// <summary>
		/// Create a batch of Contacts with old interactions, then find all contacts with no interactions since the configured search period. Then delete these inactive Contacts!
		/// NOTE: This is not optimized for performance. This is built to showcase independent steps and pull together multiple lessons on working with multiple Contact records.
		/// </summary>
		/// <param name="cfg">The configuration used to open connections to xConnect</param>
		public static async Task DeletingMultipleContactsTutorial(XConnectClientConfiguration cfg)
		{
			//Initialize required handlers
			var configuration = new Configuration();
			var outputHandler = new OutputHandler();
			var interactionManager = new InteractionManager() { Logger = outputHandler };
			var contactManager = new ContactManager() { Logger = outputHandler };
			var searchContactsTutorial = new SearchContactsTutorial() { Logger = outputHandler };

			//PART 1: Generate the list of 5 contact twitter IDs and then create the Contacts in one call to xConnect.
			// This shows you how to do a batch of operations. Note that while this example does all one type of Contact, you can mix anonymous and identified contacts in a single call.
			var generatedTwitterIds = new List<string>()
			{
				configuration.TwitterIdentifier + Guid.NewGuid().ToString("N"),
				configuration.TwitterIdentifier + Guid.NewGuid().ToString("N"),
				configuration.TwitterIdentifier + Guid.NewGuid().ToString("N"),
				configuration.TwitterIdentifier + Guid.NewGuid().ToString("N"),
				configuration.TwitterIdentifier + Guid.NewGuid().ToString("N")
			};
			var contactIdentifiers = await contactManager.CreateMultipleContacts(cfg, generatedTwitterIds);

			//PART 2: Find all contacts that have no interactions since the specified end date
			// This shows one example of using the search to find data that is 'old' or 'not relevant'. Note that this pulls from the index, hence needing to wait.
			var startDate = new DateTime(configuration.SearchYear, configuration.SearchMonth, configuration.SearchStartDay).ToUniversalTime();
			var endDate = startDate.AddDays(configuration.SearchDays);
			var expiredContactIds = new List<Guid>();

			//WAITING: Because searching relies on the index, we need to be sure that we actually got the results we expected. 
			//If the index hasn't updated yet, this call might return 'zero results' and then we wouldn't be able to delete the data.
			//NOTE: Because this is a tutorial, I'm allowing the infinite loop because you can terminate the console, but you may want to limit the number of tries here.
			while (expiredContactIds.Count < contactIdentifiers.Count)
			{
				expiredContactIds = await searchContactsTutorial.GetContactIdsByLastActivity(cfg, endDate);

				//Add a wait if we didn't find anything
				if(expiredContactIds.Count < contactIdentifiers.Count)
				{
					int waitSeconds = 3;
					outputHandler.WriteWaitMessage(waitSeconds, "No results found in index. Waiting for index to update.");
				}
			}

			//PART 3: Load Contact details for all matching Contacts
			// This shows an example of retrieving the data about multiple contacts in a single call
			var expiredContacts = await contactManager.GetMultipleContacts(cfg, expiredContactIds);

			//PART 4: Delete all contacts identified
			// This shows an example of using the Delete API but for a list of multiple specified contacts
			await contactManager.DeleteMultipleContacts(cfg, expiredContacts);
		}
	}
}
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This tutorial shows how to create a single Contact
	/// </summary>
	public class CreateContactTutorial : BaseTutorial
	{
		/// <summary>
		/// Create a contact
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterId">The identifier of the contact to create</param>'
		public virtual async Task<ContactIdentifier> CreateContact(XConnectClientConfiguration cfg, string twitterId)
		{
			// Identifier for a 'known' contact
			var identifier = new ContactIdentifier("twitter", twitterId, ContactIdentifierType.Known);
			var identifiers = new ContactIdentifier[] { identifier };

			// Print out the identifier that is going to be used
			Logger.WriteLine("Creating Contact with Identifier:" + identifier.Identifier);

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					// Create a new contact object from the identifier
					var knownContact = new Contact(identifiers);

					//Add personal information
					var personalInfoFacet = new PersonalInformation() { FirstName = "Myrtle", LastName = "McSitecore", JobTitle = "Programmer Writer", Birthdate = DateTime.Now.Date };
					//Set the personal info on the contact
					client.SetFacet<PersonalInformation>(knownContact, PersonalInformation.DefaultFacetKey, personalInfoFacet);

					//Add to the client
					client.AddContact(knownContact);

					// Submit contact
					await client.SubmitAsync();

					// Get the last batch that was executed
					Logger.WriteOperations(client.LastBatch);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception creating contact", ex);
				}
			}

			return identifier;
		}
	}
}

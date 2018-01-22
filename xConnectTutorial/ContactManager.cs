using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows examples of how to manage Contacts through xConnect
	/// </summary>
	public class ContactManager
	{
		/// <summary>
		/// Create a contact
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterId">The identifier of the contact to create</param>'
		/// <param name="outputHandler">The object handling output</param>
		public static async Task<ContactIdentifier> CreateContact(XConnectClientConfiguration cfg, string twitterId, OutputHandler outputHandler)
		{
			// Identifier for a 'known' contact
			var identifier = new ContactIdentifier("twitter", twitterId, ContactIdentifierType.Known);
			var identifiers = new ContactIdentifier[] { identifier };

			// Print out the identifier that is going to be used
			outputHandler.WriteLine("Creating Contact with Identifier:" + identifier.Identifier);

			// Create a new contact object from the identifier
			Contact knownContact = new Contact(identifiers);

			//Add personal information
			PersonalInformation personalInfoFacet = new PersonalInformation() { FirstName = "Myrtle", LastName = "McSitecore", JobTitle = "Programmer Writer", Birthdate = DateTime.Now.Date };

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Set the personal info on the contact
					client.SetFacet<PersonalInformation>(knownContact, PersonalInformation.DefaultFacetKey, personalInfoFacet);

					//Add to the client
					client.AddContact(knownContact);

					// Submit contact and interaction - a total of two operations
					await client.SubmitAsync();

					// Get the last batch that was executed
					outputHandler.WriteOperations(client.LastBatch);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					outputHandler.WriteError("Exception creating contact", ex);
				}
			}

			return identifier;
		}

		/// <summary>
		/// Retrieve a created contact
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterId">The identifier of the contact to create</param>
		/// <param name="outputHandler">The object handling output</param>
		/// <returns>The matching contact object</returns>
		public static async Task<Contact> GetContact(XConnectClientConfiguration cfg, string twitterId, OutputHandler outputHandler)
		{
			Contact existingContact = null;

			outputHandler.WriteLine("Retrieving Contact with Identifier:" + twitterId);

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					// Get a known contact
					IdentifiedContactReference reference = new IdentifiedContactReference("twitter", twitterId);
					existingContact = await client.GetAsync<Contact>(reference, new ContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey }));
					if (existingContact == null)
					{
						outputHandler.WriteLine("No contact found with ID '{0}'", twitterId);
						return null;
					}
					outputHandler.WriteLine("Contact ID: " + existingContact.Id.ToString());

					//Get the contact name
					PersonalInformation existingContactFacet = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
					if (existingContactFacet != null)
					{
						outputHandler.WriteLine("Contact Name: {0} {1}", existingContactFacet.FirstName, existingContactFacet.LastName);
						outputHandler.WriteLine("Contact Job Title: {0}", existingContactFacet.JobTitle);
						outputHandler.WriteLine("Contact Birth Date: {0}", (existingContactFacet.Birthdate.HasValue ? existingContactFacet.Birthdate.Value.Date.ToString("yyyy-MM-dd") : "[N/A]"));
					}
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					outputHandler.WriteError("Exception retrieving contact", ex);
				}
			}

			return existingContact;
		}
	}
}

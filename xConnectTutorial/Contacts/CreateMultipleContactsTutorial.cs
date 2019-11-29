using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows examples of how to create multiple Contacts through xConnect
	/// </summary>
	public class CreateMultipleContactsTutorial : BaseTutorial
	{
		/// <summary>
		/// Create a contact
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterIdentifiers">The list of identifiers for each contact</param>'
		public virtual async Task<List<ContactIdentifier>> CreateMultipleContacts(XConnectClientConfiguration cfg, List<string> twitterIdentifiers)
		{
			//Ensure collection is non-null
			if (twitterIdentifiers == null)
			{
				twitterIdentifiers = new List<string>();
			}

			// Print out the identifier that is going to be used
			Logger.WriteLine("Creating Multiple Contacts [{0}]", twitterIdentifiers.Count);

			//Build up the list of ContactIdentifiers that will be used
			List<ContactIdentifier> contactIdentifiers = new List<ContactIdentifier>();
			foreach (var twitterId in twitterIdentifiers)
			{
				var identifier = new ContactIdentifier("twitter", twitterId, ContactIdentifierType.Known);
				contactIdentifiers.Add(identifier);
			}

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Create all the contact objects and add to the client
					foreach (var contactIdentifier in contactIdentifiers)
					{
						// Create a new contact object from the identifier
						var knownContact = new Contact(new ContactIdentifier[] { contactIdentifier });

						//Add personal information
						var personalInfoFacet = new PersonalInformation() { FirstName = "Myrtle", LastName = contactIdentifier.Identifier, JobTitle = "Programmer Writer", Birthdate = DateTime.Now.Date };

						//Set the personal info on the contact
						client.SetFacet<PersonalInformation>(knownContact, PersonalInformation.DefaultFacetKey, personalInfoFacet);

						//Add to the client
						client.AddContact(knownContact);
					}

					// Submit contact
					await client.SubmitAsync();

					// Get the last batch that was executed
					Logger.WriteOperations(client.LastBatch);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception creating contacts", ex);
				}
			}

			return contactIdentifiers;
		}
	}
}

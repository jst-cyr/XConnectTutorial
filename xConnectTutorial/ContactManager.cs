using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using Sitecore.TechnicalMarketing.xConnectTutorial.Extensions;
using System;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows examples of how to manage Contacts through xConnect
	/// </summary>
	public class ContactManager
	{
		public OutputHandler Logger { get; set; }

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

		/// <summary>
		/// Retrieve a created contact
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterId">The identifier of the contact to create</param>
		/// <returns>The matching contact object</returns>
		public virtual async Task<Contact> GetContact(XConnectClientConfiguration cfg, string twitterId)
		{
			return await GetContactWithInteractions(cfg, twitterId, null, null);
		}

		/// <summary>
		/// Retrieve a created contact with interactions within the range provided
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="twitterId">The identifier of the contact to create</param>
		/// <param name="interactionStartTime">The start range of interactions to return</param>
		/// <param name="interactionEndTime">The end range of interactions to return</param>
		/// <returns>The matching contact object</returns>
		public virtual async Task<Contact> GetContactWithInteractions(XConnectClientConfiguration cfg, string twitterId, DateTime? interactionStartTime, DateTime? interactionEndTime)
		{
			Contact existingContact = null;

			Logger.WriteLine("Retrieving Contact with Identifier:" + twitterId);

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
                    var contactOptions = new ContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey });

                    //Add interaction range if necessary
                    if (interactionStartTime.HasValue || interactionEndTime.HasValue)
					{
						contactOptions.Interactions = new RelatedInteractionsExpandOptions(IpInfo.DefaultFacetKey)
						{
							StartDateTime = interactionStartTime,
							EndDateTime = interactionEndTime
						};
					}

                    //Build up options for the query
                    var reference = new IdentifiedContactReference("twitter", twitterId);
                    // Get a known contact
                    existingContact = await client.GetAsync<Contact>(reference, contactOptions);
                    if (existingContact == null)
					{
						Logger.WriteLine("No contact found with ID '{0}'", twitterId);
						return null;
					}

					//Output information about the contact
					Logger.WriteContact(existingContact);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception retrieving contact", ex);
				}
			}

			return existingContact;
		}

		/// <summary>
		/// Given an existing identifier and new personal information, find the contact and update the facet
		/// </summary>
		/// <param name="cfg">The configuration to use to load the Contact</param>
		/// <param name="twitterId">The identifier for the contact</param>
		/// <param name="updatedPersonalInformation">The new information to store</param>
		/// <returns></returns>
		public virtual async Task<Contact> UpdateContact(XConnectClientConfiguration cfg, string twitterId, PersonalInformation updatedPersonalInformation)
		{
			Logger.WriteLine("Updating personal information about Contact with Identifier:" + twitterId);

			//If no updated information was provided, we can shortcut exit and save the connections
			if (updatedPersonalInformation == null)
				return null;

			//Get the existing contact that we want to update
			var existingContact = await GetContact(cfg, twitterId);

			if(existingContact != null)
			{
				//Get the existing personal information that needs to be updated
				var personalInfoFacet = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
				if(personalInfoFacet != null)
				{
					//Check for any changes. No need to send updates if it is the same!
					bool hasFacetChanged = personalInfoFacet.HasChanged(updatedPersonalInformation);

					//If any change has occurred, make the update.
					if (hasFacetChanged)
					{
						//Update the current facet data with the new pieces
						personalInfoFacet.Update(updatedPersonalInformation);

						//Open a client connection and make the update
						using (var client = new XConnectClient(cfg))
						{
							try { 
								client.SetFacet<PersonalInformation>(existingContact, PersonalInformation.DefaultFacetKey, personalInfoFacet);
							}
							catch (XdbExecutionException ex)
							{
								Logger.WriteError("Exception updating personal information", ex);
							}
						}
					}
				}
				else
				{
					//If there is no personal information facet, we need to send all the data
					using (var client = new XConnectClient(cfg))
					{
						try
						{
							client.SetFacet<PersonalInformation>(existingContact, PersonalInformation.DefaultFacetKey, updatedPersonalInformation);
						}
						catch (XdbExecutionException ex)
						{
							Logger.WriteError("Exception creating personal information for the Contact", ex);
						}
					}

					//Update the current facet data with the new pieces
					personalInfoFacet = updatedPersonalInformation;
				}

				//Output information about the updated contact
				Logger.WriteLine("Updated contact information:");
				Logger.WriteContact(existingContact);
			}
			else
			{
				Logger.WriteLine("WARNING: No Contact found with Identifier:" + twitterId + ". Cannot update personal information.");
			}

			return existingContact;
		}

		/// <summary>
		/// Given an existing identifier, find and delete the Contact
		/// </summary>
		/// <param name="cfg">The configuration to use to load the Contact</param>
		/// <param name="twitterId">The identifier for the contact</param>
		public virtual async Task<Contact> DeleteContact(XConnectClientConfiguration cfg, string twitterId)
		{
			Logger.WriteLine("Deleting Contact with Identifier:" + twitterId);

			//Get the existing contact that we want to update
			var existingContact = await GetContact(cfg, twitterId);
			if(existingContact != null)
			{
				//If there is no personal information facet, we need to send all the data
				using (var client = new XConnectClient(cfg))
				{
					try
					{
						var result = client.DeleteContact(existingContact);
						await client.SubmitAsync();

						Logger.WriteLine(">> Contact successfully deleted.");
					}
					catch (XdbExecutionException ex)
					{
						Logger.WriteError("Exception deleting the Contact", ex);
					}
				}
			}
			else
			{
				Logger.WriteLine("WARNING: No Contact found with Identifier:" + twitterId + ". Cannot delete Contact.");
			}

			return existingContact;
		}
	}
}

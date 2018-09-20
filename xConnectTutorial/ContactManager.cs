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
	}
}

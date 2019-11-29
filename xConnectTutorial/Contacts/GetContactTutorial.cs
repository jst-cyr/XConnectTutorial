using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows examples of how to get a single Contact through xConnect
	/// </summary>
	public class GetContactTutorial : BaseTutorial
	{
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

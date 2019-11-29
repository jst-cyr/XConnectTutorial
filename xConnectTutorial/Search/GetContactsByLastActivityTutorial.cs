using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows examples of how to search Contacts based on their last activity
	/// </summary>
	public class GetContactsByLastActivityTutorial
	{
		public OutputHandler Logger { get; set; }

		/// <summary>
		/// Find ID values for all contacts that have not had any activity since a specified DateTime. 
		/// If a bound is not provided, the logic assumes current time - 30 days.
		/// 
		/// Based on documentation example: https://doc.sitecore.com/developers/92/sitecore-experience-platform/en/search-contacts.html
		/// </summary>
		/// <param name="cfg">The client configuration for connecting</param>
		/// <param name="lastActivity">The time to look for the contact's last activity</param>
		/// <returns>The matching contact object</returns>
		public virtual async Task<List<System.Guid>> GetContactIdsByLastActivity(XConnectClientConfiguration cfg, DateTime? lastActivity)
		{
			//Create a collection that will store the IDs of the results
			List<System.Guid> matchingContactIds = new List<System.Guid>();

			//Establish a timebound to search for. If not provided, default to a value.
			DateTime searchStartTime = lastActivity.HasValue ? lastActivity.Value : DateTime.Now.AddDays(-30);
			Logger.WriteLine("Retrieving all Contacts without interactions since:" + searchStartTime.ToShortDateString());

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Set up options to restrict the number of interactions we return for each contact
                    var contactExpandOptions = new ContactExpandOptions();
					contactExpandOptions.Interactions = new RelatedInteractionsExpandOptions() { Limit = 20 };

					//Create a queryable to search by the date
					IAsyncQueryable<Contact> queryable = client.Contacts
															.Where(c => (!c.Interactions.Any()) || !c.Interactions.Any(x => x.StartDateTime > searchStartTime))
															.WithExpandOptions(contactExpandOptions);

					//Invoke the query
					var enumerator = await queryable.GetBatchEnumerator(10);

					//Collect all the matching contact IDs
					while(await enumerator.MoveNext())
					{
						foreach(var contact in enumerator.Current)
						{
							if(contact.Id.HasValue)
								matchingContactIds.Add(contact.Id.Value);
						}
					}


					Logger.WriteLine(">> Total number of matches found '{0}'", matchingContactIds.Count);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception executing search", ex);
				}
			}

			return matchingContactIds;
		}
	}
}

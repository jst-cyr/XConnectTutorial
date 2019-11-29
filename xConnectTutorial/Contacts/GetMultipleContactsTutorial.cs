using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This tutorial shows how to retrieve multiple Contacts at a time
	/// </summary>
	public class GetMultipleContactsTutorial : BaseTutorial
	{
		/// <summary>
		/// Retrieves the Contacts from xConnect for each matching contactID
		/// </summary>
		/// <param name="cfg">The configuration to use to open connections to xConnect</param>
		/// <param name="contactIds">The list of contacts to retrieve</param>
		/// <returns></returns>
		public virtual async Task<List<Contact>> GetMultipleContacts(XConnectClientConfiguration cfg, List<Guid> contactIds)
		{
			if (contactIds == null) { contactIds = new List<Guid>(); }
			Logger.WriteLine("Getting Multiple Contacts: [{0}]", contactIds.Count);

			var contacts = new List<Contact>();

			// Initialize a client using the validated configuration
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Configure the options to extract the personal information facet
					var contactOptions = new ContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey });

					//Get a list of reference objects for the contactIds provided
					var references = new List<IEntityReference<Contact>>();
					foreach (var contactId in contactIds)
					{
						references.Add(new ContactReference(contactId));
					}

					//Get all the matches
					var contactsResult = await client.GetAsync<Contact>(references, contactOptions);

					//Get the Contact objects from the results
					foreach (var result in contactsResult)
					{
						if (result.Exists)
						{
							contacts.Add(result.Entity);
						}
					}

					//Output information about the contact list
					if (client.LastBatch != null) { Logger.WriteOperations(client.LastBatch); }
					Logger.WriteLine("> Retrieved {0} matching Contacts.", contacts.Count);
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception retrieving contact", ex);
				}
			}

			return contacts;
		}
	}
}

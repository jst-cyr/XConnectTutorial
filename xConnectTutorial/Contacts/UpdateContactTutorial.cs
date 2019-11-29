using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using Sitecore.TechnicalMarketing.xConnectTutorial.Extensions;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This tutorial shows how to update data about a Contact
	/// </summary>
	public class UpdateContactTutorial : BaseTutorial
	{
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
			var contactLoader = new GetContactTutorial() { Logger = this.Logger };
			var existingContact = await contactLoader.GetContact(cfg, twitterId);

			if (existingContact != null)
			{
				//Get the existing personal information that needs to be updated
				var personalInfoFacet = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
				if (personalInfoFacet != null)
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
							try
							{
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
	}
}

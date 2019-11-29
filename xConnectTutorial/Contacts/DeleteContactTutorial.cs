using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This tutorial shows how to delete a single Contact
	/// </summary>
	public class DeleteContactTutorial : BaseTutorial
	{

		/// <summary>
		/// Given an existing identifier, find and delete the Contact
		/// </summary>
		/// <param name="cfg">The configuration to use to load the Contact</param>
		/// <param name="twitterId">The identifier for the contact</param>
		public virtual async Task<Contact> DeleteContact(XConnectClientConfiguration cfg, string twitterId)
		{
			Logger.WriteLine("Deleting Contact with Identifier:" + twitterId);

			//Get the existing contact that we want to delete
			var contactLoader = new GetContactTutorial() { Logger = this.Logger };
			var existingContact = await contactLoader.GetContact(cfg, twitterId);
			if (existingContact != null)
			{
				using (var client = new XConnectClient(cfg))
				{
					try
					{
						//Add the delete operation onto the client for the specified contact and execute
						client.DeleteContact(existingContact);
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

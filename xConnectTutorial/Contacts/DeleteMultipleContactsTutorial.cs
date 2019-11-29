using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This tutorial shows how to delete multiple Contacts
	/// </summary>
	public class DeleteMultipleContactsTutorial : BaseTutorial
	{
		/// <summary>
		/// Given a list of Contact GUIDs, delete all of them
		/// </summary>
		/// <param name="cfg">The configuration to use to load the Contact</param>
		/// <param name="contacts">The contacts to delete</param>
		public virtual async Task DeleteMultipleContacts(XConnectClientConfiguration cfg, List<Contact> contacts)
		{
			if (contacts == null) { contacts = new List<Contact>(); }
			Logger.WriteLine("Deleting Multiple Contacts: [{0}]", contacts.Count);

			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Add an operation to the task for each contact in the list
					foreach (var contact in contacts)
					{
						client.DeleteContact(contact);
					}

					await client.SubmitAsync();

					if (client.LastBatch != null) { Logger.WriteOperations(client.LastBatch); }
					Logger.WriteLine(">> {0} Contacts successfully deleted.", contacts.Count);
				}
				catch (XdbExecutionException ex)
				{
					Logger.WriteError("Exception deleting the Contacts", ex);
				}
			}
		}
	}
}

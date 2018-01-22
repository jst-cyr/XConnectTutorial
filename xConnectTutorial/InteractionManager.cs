using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class shows an example of working with interaction objects and the xConnect API
	/// </summary>
	public class InteractionManager
	{
		/// <summary>
		/// Create an interaction for the specified contact loaded from xConnect
		/// </summary>
		/// <param name="cfg">The client configuration</param>
		/// <param name="contact">The contact to create an interaction for</param>
		/// <param name="channelId">The channel to create an interaction on</param>
		/// <param name="goalId">The ID of the goal for the interaction event</param>
		/// <param name="outputHandler">The handler for output</param>
		/// <returns></returns>
		public static async Task<Interaction> RegisterGoalInteraction(XConnectClientConfiguration cfg, Contact contact, string channelId, string goalId, OutputHandler outputHandler)
		{
			outputHandler.WriteLine("Creating interaction for contact with ID: '{0}'. Channel: '{1}'. Goal: '{2}'", contact.Id, channelId, goalId);
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Instantiate the interaction details
					Interaction interaction = new Interaction(contact, InteractionInitiator.Brand, Guid.Parse(channelId), "");

					//Create the event - all interactions must have at least one event
					var xConnectEvent = new Goal(Guid.Parse(goalId), DateTime.UtcNow);
					interaction.Events.Add(xConnectEvent);

					//Add the interaction to the client
					client.AddInteraction(interaction);

					//Submit the interaction
					await client.SubmitAsync();

					// Get the last batch that was executed
					outputHandler.WriteOperations(client.LastBatch);

					return interaction;
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					outputHandler.WriteError("Exception creating interaction", ex);
				}
			}

			return null;
		}
	}
}

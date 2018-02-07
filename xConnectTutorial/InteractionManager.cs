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
		public OutputHandler Logger { get; set; }

		/// <summary>
		/// Create an interaction for the specified contact loaded from xConnect
		/// </summary>
		/// <param name="cfg">The xConnect client configuration to use to make connections</param>
		/// <param name="contact">The contact to create an interaction for</param>
		/// <param name="channelId">The channel to create an interaction on</param>
		/// <param name="goalId">The ID of the goal for the interaction event</param>
		/// <returns></returns>
		public virtual async Task<Interaction> RegisterGoalInteraction(XConnectClientConfiguration cfg, Contact contact, string channelId, string goalId)
		{
			Logger.WriteLine("Creating interaction for contact with ID: '{0}'. Channel: '{1}'. Goal: '{2}'", contact.Id, channelId, goalId);
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
					Logger.WriteOperations(client.LastBatch);

					return interaction;
				}
				catch (XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception creating interaction", ex);
				}
			}

			return null;
		}

		/// <summary>
		/// Executes a search against the API to look for interactions falling between two particular dates.
		/// </summary>
		/// <param name="cfg">The xConnect client configuration to use to make connections</param>
		/// <param name="startDate">The earliest start point for returned interactions</param>
		/// <param name="endDate">The latest end point for returned interactions</param>
		/// <returns></returns>
		public virtual async Task<IAsyncEntityBatchEnumerator<Interaction>> SearchInteractionsByDate(XConnectClientConfiguration cfg, DateTime startDate, DateTime endDate)
		{
			Logger.WriteLine("Searching for all interactions between {0} and {1}", startDate, endDate);
			using (var client = new XConnectClient(cfg))
			{
				try { 
					//Build the query to be triggered
					var queryable = client.Interactions.Where(i => i.StartDateTime >= startDate && i.EndDateTime <= endDate);

					//Execute the search using the date boundaries provided
					var enumerable = await queryable.GetBatchEnumerator(10);

					//Output the data that was retrieved
					while(await enumerable.MoveNext())
					{
						foreach(var interaction in enumerable.Current)
						{
							Logger.WriteInteraction(interaction);
						}
					}

					//Return the set of interactions to the calling application
					return enumerable;
				}
				catch(XdbExecutionException ex)
				{
					// Deal with exception
					Logger.WriteError("Exception executing search operation", ex);
				}
			}

			return null;
		}
	}
}

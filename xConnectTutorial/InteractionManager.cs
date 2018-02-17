using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
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
		/// <param name="ipInfo">The ip address the goal was triggered from</param>
		/// <returns></returns>
		public virtual async Task<Interaction> RegisterGoalInteraction(XConnectClientConfiguration cfg, Contact contact, string channelId, string goalId, IpInfo ipInfo)
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

					//Add in IP information for where the goal was triggered from
					if(ipInfo != null) { 
						client.SetFacet<IpInfo>(interaction, IpInfo.DefaultFacetKey, ipInfo);
					}

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
		/// You can explicitly request a single interaction, but you must provide both the contact ID and the interaction ID.
		/// Most external systems won't have this data, but it could be useful if you want to load details of an interaction results from a search
		/// and return additional facet data.
		/// </summary>
		/// <param name="cfg">The xConnect client configuration to use to make connections</param>
		/// <param name="contactId">The contact the interaction is associated with</param>
		/// <param name="interactionId">The interaction event identifier</param>
		/// <returns></returns>
		public virtual async Task<Interaction> GetInteraction(XConnectClientConfiguration cfg, Guid contactId, Guid interactionId)
		{
			Logger.WriteLine("Retrieving interaction for contact with ID: '{0}'. Interaction ID: '{1}'.", contactId, interactionId);
			using (var client = new XConnectClient(cfg))
			{
				try
				{
					//Get the interaction reference that will be used to execute the lookup
					var interactionReference = new InteractionReference(contactId, interactionId);

					//Define the facets that should be expanded
					var expandOptions = new InteractionExpandOptions(new string[] { IpInfo.DefaultFacetKey });
					expandOptions.Contact = new RelatedContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey });
					expandOptions.Expand<WebVisit>();

					//Query the client for the interaction
					var interaction = await client.GetAsync<Interaction>(interactionReference, expandOptions);

					//Show the information about the interaction
					Logger.WriteInteraction(interaction);

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
		public virtual async Task<List<Interaction>> SearchInteractionsByDate(XConnectClientConfiguration cfg, DateTime startDate, DateTime endDate)
		{
			Logger.WriteLine("Searching for all interactions between {0} and {1}", startDate, endDate);
			using (var client = new XConnectClient(cfg))
			{
				try { 
					//Build the query to be triggered
					var queryable = client.Interactions.Where(i => i.StartDateTime >= startDate && i.EndDateTime <= endDate);

					//Execute the search using the date boundaries provided
					var enumerable = await queryable.GetBatchEnumerator(10);

					//Output the data that was retrieved and collect into a list to return
					var interactions = new List<Interaction>();
					while(await enumerable.MoveNext())
					{
						foreach(var interaction in enumerable.Current)
						{
							interactions.Add(interaction);
							Logger.WriteInteraction(interaction);
						}
					}

					//Return the set of interactions to the calling application
					return interactions;
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

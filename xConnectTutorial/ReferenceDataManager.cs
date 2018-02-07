using Microsoft.Extensions.Logging;
using Sitecore.Xdb.ReferenceData.Client;
using Sitecore.Xdb.ReferenceData.Core;
using Sitecore.Xdb.ReferenceData.Core.Converter;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This shows an example of connecting to the reference data service from a console application
	/// </summary>
	public class ReferenceDataManager
	{
		public OutputHandler Logger { get; set; }

		/// <summary>
		/// Retrieve a specific definition from the reference data
		/// </summary>
		/// <param name="definitionTypeName">The name of the definition type</param>
		/// <param name="moniker">The moniker of the definition, refers to the Sitecore GUID of the item</param>
		/// <param name="referenceDataHost">The host URL of the reference data service</param>
		/// <param name="thumbprint">The thumbprint for the endpoint certificate</param>
		/// <returns></returns>
		public virtual async Task<Definition<string, string>> GetDefinition(string definitionTypeName, string moniker, string referenceDataHost, string thumbprint)
		{
			using (var client = BuildClient(referenceDataHost, thumbprint))
			{
				try
				{
					//Build the search criteria for the definition retrieval
					var definitionType = await client.EnsureDefinitionTypeAsync(definitionTypeName);
					var criteria = new DefinitionCriteria(moniker, definitionType);

					var definition = await client.GetDefinitionAsync<string, string>(criteria, false);

					if (definition != null)
					{
						Logger.WriteLine("Definition found: {0}", definition.Key.Moniker);
					}
					else
					{
						Logger.WriteLine("No definition found with moniker: {0} of type {1}", moniker, definitionTypeName);
					}

					return definition;
				}
				catch (Exception ex)
				{
					Logger.WriteError("Error retrieving definition", ex);
				}
			}

			return null;
		}

		/// <summary>
		/// Retrieve a specific definition from the reference data
		/// </summary>
		/// <param name="definitionTypeName">The name of the definition type</param>
		/// <param name="moniker">The moniker of the definition, refers to the Sitecore GUID of the item</param>
		/// <param name="definitionName">The name of the definition (to be used in culture data)</param>
		/// <param name="referenceDataHost">The host URL of the reference data service</param>
		/// <param name="thumbprint">The thumbprint for the endpoint certificate</param>
		/// <returns></returns>
		public virtual async Task<Definition<string, string>> CreateDefinition(string definitionTypeName, string moniker, string definitionName, string referenceDataHost, string thumbprint)
		{
			using (var client = BuildClient(referenceDataHost, thumbprint))
			{
				try
				{
					//Get the definition type that the definition will be associated to
					var definitionType = await client.EnsureDefinitionTypeAsync(definitionTypeName);

					//Define the data about the definition
					var definitionKey = new DefinitionKey(moniker, definitionType, 1);
					var definition = new Definition<string, string>(definitionKey)
					{
						IsActive = true,
						CommonData = "Some common data",
						CultureData =
						{
							{ new CultureInfo("en"), definitionName }
						}
					};

					//Culture invariant data - has culture, but culture is unknown
					definition.CultureData[CultureInfo.InvariantCulture] = definitionName;

					//Create the definition
					var result = await client.SaveAsync(definition);

					Logger.WriteSaveResult(result);

					return definition;
				}
				catch (Exception ex)
				{
					Logger.WriteError("Error creating definition", ex);
				}
			}

			return null;
		}

		/// <summary>
		/// Create an instance of the client
		/// </summary>
		/// <param name="referenceDataHost">The host URL of the reference data service</param>
		/// <param name="thumbprint">The thumbprint for the endpoint certificate</param>
		/// <returns>A new instance of the client</returns>
		public virtual ReferenceDataHttpClient BuildClient(string referenceDataHost, string thumbprint)
		{
			var converter = new DefinitionEnvelopeJsonConverter();

			var handlers = new ConfigurationBuilder().GetReferenceDataHandlers(thumbprint);

			var logger = new Logger<ReferenceDataHttpClient>(new LoggerFactory());

			return new ReferenceDataHttpClient(converter,
				new Uri(referenceDataHost),
				handlers,
				logger);
		}
	}
}

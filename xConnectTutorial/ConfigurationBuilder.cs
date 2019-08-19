using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using System;
using System.Collections.Generic;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// This class can be used to build xConnect client configurations, or extended for your own needs.
	/// </summary>
	public class ConfigurationBuilder
	{
		/// <summary>
		/// Builds a client configuration object for connecting to xConnect.
		/// Includes certificate and timeout modifiers, and sets up client connections.
		/// </summary>
		/// <param name="collectionHost">The host name where the collection endpoint is located</param>
		/// <param name="searchHost">The host name where the search endpoint is located</param>
		/// <param name="configHost">The host name where configuration info should be retrieved</param>
		/// <param name="thumbprint">The thumbprint for the certificate to connect with</param>
		/// <returns>A new configuration instance</returns>
		public virtual XConnectClientConfiguration GetClientConfiguration(string collectionHost, string searchHost, string configHost, string thumbprint)
		{
			//Set up the certificate used to connect to xConnect endpoints
			var options = CertificateHttpClientHandlerModifierOptions.Parse("StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=" + thumbprint);
			var certificateModifier = new CertificateHttpClientHandlerModifier(options);

			//Set up timeout modifier for the client
			var timeoutClientModifier = new TimeoutHttpClientModifier(new TimeSpan(0, 0, 20));
            var clientModifiers = new List<IHttpClientModifier>
            {
                timeoutClientModifier
            };

            //Initialize the clients. Each requires the certificate in order to open the connection
            var collectionClient = new CollectionWebApiClient(new Uri(collectionHost + "/odata"), clientModifiers, new[] { certificateModifier });
			var searchClient = new SearchWebApiClient(new Uri(searchHost + "/odata"), clientModifiers, new[] { certificateModifier });
			var configurationClient = new ConfigurationWebApiClient(new Uri(configHost + "/configuration"), clientModifiers, new[] { certificateModifier });

			//Create the configuration object with all clients
			var cfg = new XConnectClientConfiguration(
				new XdbRuntimeModel(CollectionModel.Model), collectionClient, searchClient, configurationClient);

			return cfg;
		}

		/// <summary>
		/// Gets the handler configurations for the Reference Data client
		/// </summary>
		/// <param name="thumbprint"></param>
		/// <returns></returns>
		public virtual IEnumerable<IHttpClientModifier> GetReferenceDataHandlers(string thumbprint)
		{
			// Valid certificate thumbprints must be passed in
			var options = CertificateHttpClientHandlerModifierOptions.Parse("StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=" + thumbprint);

			// Optional timeout modifier
			IHttpClientModifier[] handlers = { (IHttpClientModifier)new CertificateHttpClientHandlerModifier(options) };

			return handlers;
		}
	}
}

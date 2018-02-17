using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.Xdb.ReferenceData.Core.Results;
using System;
using System.Collections.Generic;

namespace Sitecore.TechnicalMarketing.xConnectTutorial
{
	/// <summary>
	/// Example class used to handle outputting things to the console
	/// </summary>
	public class OutputHandler
	{
		/// <summary>
		/// Simple output to the console
		/// </summary>
		/// <param name="value"></param>
		public void WriteLine(string value)
		{
			Console.WriteLine(value);
		}

		/// <summary>
		/// Output formatted string
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		public void WriteLine(string format, object arg0)
		{
			Console.WriteLine(format, arg0);
		}

		/// <summary>
		/// Output formatted string
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg"></param>
		public void WriteLine(string format, params object[] arg)
		{
			Console.WriteLine(format, arg);
		}

		/// <summary>
		/// Outputs an error
		/// </summary>
		/// <param name="contextMessage">The contextual message to proceed error details</param>
		/// <param name="ex">The error details</param>
		public void WriteError(string contextMessage, Exception ex)
		{
			WriteLine("{0} : {1}", contextMessage, ex.Message);
		}

		/// <summary>
		/// Banner message for showing success of configuration
		/// </summary>
		public void WriteValidationMessage()
		{
			var arr = new[]
			{
						@"            ______                                                       __     ",
						@"           /      \                                                     |  \    ",
						@" __    __ |  $$$$$$\  ______   _______   _______    ______    _______  _| $$_   ",
						@"|  \  /  \| $$   \$$ /      \ |       \ |       \  /      \  /       \|   $$ \  ",
						@"\$$\/  $$| $$      |  $$$$$$\| $$$$$$$\| $$$$$$$\|  $$$$$$\|  $$$$$$$ \$$$$$$   ",
						@" >$$  $$ | $$   __ | $$  | $$| $$  | $$| $$  | $$| $$    $$| $$        | $$ __  ",
						@" /  $$$$\ | $$__/  \| $$__/ $$| $$  | $$| $$  | $$| $$$$$$$$| $$_____   | $$|  \",
						@"|  $$ \$$\ \$$    $$ \$$    $$| $$  | $$| $$  | $$ \$$     \ \$$     \   \$$  $$",
						@" \$$   \$$  \$$$$$$   \$$$$$$  \$$   \$$ \$$   \$$  \$$$$$$$  \$$$$$$$    \$$$$ "
					};
			Console.WindowWidth = 160;
			foreach (string line in arr)
				Console.WriteLine(line);
		}

		/// <summary>
		/// Method for outputting a list of operation results
		/// </summary>
		/// <param name="operations"></param>
		public void WriteOperations(IReadOnlyCollection<Sitecore.XConnect.Operations.IXdbOperation> operations)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("RESULTS...");

			// Loop through operations and check status
			foreach (var operation in operations)
			{
				Console.WriteLine("{0} {1} Operation: {2}", operation.OperationType, operation.Target.GetType().ToString(), operation.Status);
			}

			//Reset color
			Console.ForegroundColor = ConsoleColor.White;
		}

		/// <summary>
		/// Method for outputting a save result from the Reference Data API
		/// </summary>
		/// <param name="result">The result information</param>
		public void WriteSaveResult(SaveDefinitionOperationResult result)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("RESULTS...");

			Console.WriteLine("{0} - {1}", result.Status, result.Message);

			Console.ForegroundColor = ConsoleColor.White;
		}

		/// <summary>
		/// Method for outputting an interaction object 
		/// </summary>
		/// <param name="interaction">The interaction data</param>
		public void WriteInteraction(Interaction interaction)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Interaction ID: {0}", interaction.Id);
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine(" > Start: {0} to End: {1}", interaction.StartDateTime, interaction.EndDateTime);
			Console.WriteLine(" > Channel ID: {0}", interaction.ChannelId);
			Console.WriteLine(" > Contact ID: {0}", interaction.Contact.Id);

			var ipInfoFacet = interaction.GetFacet<IpInfo>(IpInfo.DefaultFacetKey);
			if(ipInfoFacet != null)
			{
				Console.WriteLine(" > IP Information - Address: {0}, Business: {1}", ipInfoFacet.IpAddress, ipInfoFacet.BusinessName);
			}

			var contact = interaction.Contact as Contact;
			if(contact != null) { 
				WriteContact(contact);
			}
		}

		/// <summary>
		/// Method for outputting Contact details
		/// </summary>
		/// <param name="contact">The contact data to extract</param>
		public void WriteContact(Contact contact)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Contact ID: {0}", contact.Id.ToString());
			Console.ForegroundColor = ConsoleColor.White;

			PersonalInformation personalInfoFacet = contact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
			if (personalInfoFacet != null)
			{
				Console.WriteLine(" > Contact Name: {0} {1}", personalInfoFacet.FirstName, personalInfoFacet.LastName);
				Console.WriteLine(" > Contact Job Title: {0}", personalInfoFacet.JobTitle);
				Console.WriteLine(" > Contact Birth Date: {0}", (personalInfoFacet.Birthdate.HasValue ? personalInfoFacet.Birthdate.Value.Date.ToString("yyyy-MM-dd") : "[N/A]"));
			}

			//Write out interaction data
			if (contact.Interactions != null)
			{
				Console.WriteLine(" > Interactions:");
				foreach (var interaction in contact.Interactions)
				{
					Console.WriteLine(" >> Interaction ID: {0}", interaction.Id);
					Console.WriteLine(" >>> Start: {0} to End: {1}", interaction.StartDateTime, interaction.EndDateTime);
					Console.WriteLine(" >>> Channel ID: {0}", interaction.ChannelId);
				}
			}
		}
	}
}

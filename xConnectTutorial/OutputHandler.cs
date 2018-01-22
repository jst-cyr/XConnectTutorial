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
				Console.WriteLine(operation.OperationType + operation.Target.GetType().ToString() + " Operation: " + operation.Status);
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
	}
}

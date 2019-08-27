using System;
using System.Reflection;
using System.Linq;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.TechnicalMarketing.xConnectTutorial.Extensions
{
	public static class PersonalInformationExtensions
	{
		/// <summary>
		/// Checks the current data against the updated personal information to look for any changes.
		/// </summary>
		/// <param name="updatedPersonalInformation"></param>
		/// <returns></returns>
		public static bool HasChanged(this PersonalInformation personalInformation, PersonalInformation updatedPersonalInformation)
		{
			//If no updates, nothing has changed
			if (updatedPersonalInformation == null) { return false; }

			//If a birthdate value is provided and existing is blank or different, there is a change
			if (updatedPersonalInformation.Birthdate.HasValue && (!personalInformation.Birthdate.HasValue || updatedPersonalInformation.Birthdate.Value != personalInformation.Birthdate.Value))
			{
				return true;
			}

			//For each String property, do a comparison check to see if a change has happened.
			if ((personalInformation.HasNewValue(personalInformation.FirstName, updatedPersonalInformation.FirstName)) ||
					(personalInformation.HasNewValue(personalInformation.Gender, updatedPersonalInformation.Gender)) ||
					(personalInformation.HasNewValue(personalInformation.JobTitle, updatedPersonalInformation.JobTitle)) ||
					(personalInformation.HasNewValue(personalInformation.LastName, updatedPersonalInformation.LastName)) ||
					(personalInformation.HasNewValue(personalInformation.MiddleName, updatedPersonalInformation.MiddleName)) ||
					(personalInformation.HasNewValue(personalInformation.Nickname, updatedPersonalInformation.Nickname)) ||
					(personalInformation.HasNewValue(personalInformation.PreferredLanguage, updatedPersonalInformation.PreferredLanguage)) ||
					(personalInformation.HasNewValue(personalInformation.Suffix, updatedPersonalInformation.Suffix)) ||
					(personalInformation.HasNewValue(personalInformation.Title, updatedPersonalInformation.Title))
				)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// This extension does a comparison check, but also assumes that the current value should trump an 'empty' value from the update.
		/// This prevents imports from 'erasing' existing content, if desired.
		/// </summary>
		/// <param name="currentValue">The current property value</param>
		/// <param name="personalInformation">The current personal information object</param>
		/// <param name="updatedValue">The proposed new property value</param>
		/// <returns></returns>
		public static bool HasNewValue(this PersonalInformation personalInformation, string currentValue, string updatedValue)
		{
			return !String.IsNullOrWhiteSpace(updatedValue) && !updatedValue.Equals(currentValue);
		}

		/// <summary>
		/// Updates the current object with the new data from the updated facet
		/// </summary>
		/// <param name="personalInformation">The existing data</param>
		/// <param name="updatedPersonalInformation">The new data</param>
		public static void Update(this PersonalInformation personalInformation, PersonalInformation updatedPersonalInformation)
		{
			//If no updates, nothing has changed
			if (updatedPersonalInformation == null) { return; }

			//If a birthdate value is provided and existing is blank or different, there is a change
			if (updatedPersonalInformation.Birthdate.HasValue && (!personalInformation.Birthdate.HasValue || updatedPersonalInformation.Birthdate.Value != personalInformation.Birthdate.Value))
			{
				personalInformation.Birthdate = updatedPersonalInformation.Birthdate;
			}

			//Loop over all string properties
			foreach (var stringProp in typeof(PersonalInformation).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x=>x.PropertyType == typeof(String))){
				var currentValue = (String) stringProp.GetValue(personalInformation);
				var updatedValue = (String) stringProp.GetValue(updatedPersonalInformation);

				if (personalInformation.HasNewValue(currentValue, updatedValue))
				{
					stringProp.SetValue(personalInformation, updatedValue);
				}
			}
		}
	}
}

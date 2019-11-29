# XConnectTutorial
A tutorial repository based on the Sitecore documentation to walk users through common interactions with the xConnect APIs.

Based on code from Martina Welander in the "Getting Started" tutorials and other great documentation: https://doc.sitecore.net/developers/xp/getting-started/#tutorials-xconnect

## Version support
**Latest version:** Sitecore XP 9.2.0
**Available releases:** v9.0.2


## Getting Started
**Program.cs** kicks off the console program and delegates to many other classes which contain the examples of how to interact with the APIs. 

IMPORTANT: For 'easy mode', run Visual Studio in administrator mode, otherwise you are likely to have issues accessing your certificates.

Modify the following configuration values in the app.config file:

 - **Thumbprint**: This is the Thumbprint for your xConnect certificate, as found in your xConnect configuration files.
 - **XConnectUrl**: This is the URL to your xConnect instance. This code base assumes a single endpoint for all services, but you can extend the program to use different endpoints easily.
 - **TwitterIdentifier**: A default identifier is configured for creating new contacts. All contacts are created with this identifier + a generated GUID for uniqueness.
 - **OtherEventChannelId**: This is an item ID in your Sitecore database for the "Other Event" channel. Ensure this matches to your Sitecore ID at the path provided in the code comments.
 - **InstantDemoGoalId**: This is the item ID in your Sitecore database for the "Instant Demo" goal. Ensure this matches to your Sitecore ID at the path provided in the code comments.
 - **SearchYear**, **SearchMonth**, **SearchStartDay**: This is the start day configuration for searching for interactions. Specify the year, month, date you want the search to start from.
 - **SearchDays**: This is the number of days to search for when searching interactions.
 
From here you can start the console app from within Visual Studio and you should be able to step through the various examples and see output in the console window with results.

## The Tutorials
1. **Building the configuration for connecting to xConnect**: This shows a class that could be used to quickly build up the object you need for instantiating all xConnect client calls.
2. **Create and Retrieve a contact**: In this code sample, you see how to create a single new Contact and then retrieve that Contact using the identifier.
3. **Update an existing contact**: In this code sample, we find a Contact by their identifier and then update some personal information on the facet and save it back to xConnect.
4. **Register a goal**: This is a code sample for creating a new interaction for a specified Contact. In this tutorial a brand new goal is registered for the Contact we have created.
5. **Reference Data Manager**: In this sample we show how to use the reference data manager to extract the definition of a goal, and if it is not there, create the definition.
6. **Get the Contact with it's interactions**: The code in this sample is similar to loading a single contact, but now permits adding date time bounds to search for associated interactions that can be loaded with the Contact. In this case, we load the new registered goal when loading the created Contact.
7. **Search Interactions**: Instead of loading a contact, this searches the interactions collection and finds all interactions within the configured search date span.
8. **Expanding an interaction search result**: In this example, we show taking a search result and then querying xConnect to get the full details of the registered goal based on the information on the search result.
9. **Delete a single Contact**: Showcasing the Delete API introduced in 9.2, this sample deletes our new Contact and all the interactions created so far.
10. **Deleting multiple inactive Contacts**: This is the most 'complex' of the tutorials here with multiple stages. This will show creating a new batch of Contacts, then finding all the contacts with no interactions since a given date, then deleting all those inactive Contacts from the database.

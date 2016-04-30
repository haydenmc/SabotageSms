#Sabotage
Sabotage is an SMS-based clone of [The Resistance](https://en.wikipedia.org/wiki/The_Resistance_(game)) board game.
You can play with 5 - 10 of your friends. The goal is to weed out which of your friends are Saboteurs before it's too late!

#Building and Running
Sabotage is written using ASP Core. To run ASP Core applications, you'll need to install `dnvm` and a runtime.
See [this page](https://docs.asp.net/en/latest/getting-started/) for instructions on how to do this for your platform of choice.

Once you have `dnvm` and a runtime installed, simply `cd` into the `SabotageSms` directory, run `dnu restore` to restore all packages, and `dnx web` to build and run.

#Configuration
Configuration is managed through `appsettings.json`. See `appsettings.default.json` for default values.
These values can also be provided by environment variables of the same name.

##Nexmo Configuration
Set `OutgoingSmsProvider` to `"Nexmo"`. You will need to provide `ApiKeys:NexmoApiKey`, `ApiKeys:NexmoApiSecret`, and `ApiKeys:NexmoSourceNumber` in your configuration.<br />
Your callback URL for receiving incoming texts will be `/api/Nexmo`.

##Plivo Configuration 
Set `OutgoingSmsProvider` to `"Plivo"`. You will need to provide `ApiKeys:PlivoAuthId`, `ApiKeys:PlivoAuthToken`, and `ApiKeys:PlivoSourceNumber`in your configuration.<br />
Your callback URL for receiving incoming texts will be `/api/Plivo`.
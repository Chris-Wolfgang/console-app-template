# ConsoleAppTemplate  


## Description  


## Table of Contents  
1. [Description](#description)  
2. [Contents](#contents)  
3. [To Do](#to-do)  
4. [Project](#project)  
5. [Program.cs](#programcs)  
6. [Commandline Arguments](#commandline-arguments)  


# TODOs / Checklist

There are a number of TODO in the code. Some of them are things you will need to do immediately,  
for example, implementing "Your code here". Others are things that you should do before releasing,  
your application. For example, adding descriptions to your application and sub commands. There are  
some things that you may never do, for example,  

To view the list of TODOs, in Visual Studio, under the View menu, select Task List.  


## Project  

1. Set the version of .Net that you want to use
1. Run package restore to get the latest packages - While the packages were updated when this template was made, and periodically updated, it is still best to run update before you get started
1. Set the version number in the project file. See [Semver.org](https://semver.org/) for more information on symantec versioning.  



## Program.cs  

1. Add a name for the application  
1. Add a description of the application  
1. Determine if you are using sub commands or not and update the OnExecute method accordingly. See Sub Commands section on this file for more information this.
1. If you are not using sub commands, and using the OnExecute method with async code, consider changing the singature to `Task<int> OnExecuteAsync (...)`
1. Determine if you are using a single `appSettings.json` file or one per environment. See Config Files section in this file for more information on this.

[!NOTE]
**NOTE** : This template is designed so the application returns 0 on success and a value greater than 0 on failure.
This tells the operating system that the application has succeeded or failed. This is important for applications 
that are run as part of a pipeline or in a container. It is recommended that you follow this approach and use either
int OnExecute or Task&lt;int&gt; OnExecuteAsync and return 0 if the command succeeds or a value greater than 0 if 
it fails.
[!NOTE]



## Setup Config Files

This template supports using a single appSettings.config file for all environments or 
sepearate config files, one for each environment. 

1. If using a single file for all environements 
	a. You will use the file named `appsettings.json` in the root of your project. 
	a. You can safely delete the other json files, `appsettings.*.json`, i.e. `appsettings.Development.json`, `appsettings.Production.json` etc.
	a. In the Program.cs file make sure that the line UseSingleEnvironment() is uncommented and that the line UseMultiEnvironment() is either commented out or removed

1. If using one file per environment
	a. you can safely delete the file named `appsettings.json` in the root of your project. 
	a. You will keep the other json files, `appsettings.*.json`, i.e. `appsettings.Development.json`, `appsettings.Production.json` etc.
	a. Create a file named for each environment you want to use, i.e. `appsettings.Test.json`, `appsettings.Local.json` etc.
	a. Make sure each file is set to copy to output directory., by right clicking on the file and selecting Properties
	a. Make sure the Windows environment variable `DOTNET_ENVIRONMENT` is set to the environment you want to use. 
	If the variabled does not exist, in blank or contains a value for which you don't have a config file for, an error will occur.
	a. In the Program.cs file make sure that the line UseMultiEnvironment() is uncommented and that the line UseSingleEnvironment() is either commented out or removed

1. When loading settings from a config file, you can add an `IConfiguration` parameter to the OnExecute method and access the settings individually.
	```csharp
	Task<int> OnExecuteAsync(IConfiguration config)
	{
		// Get the setting from the config file
		var setting = config["SettingName"];
	
		// Your code here
	}
	```

	However, it is recommended that you group related settings into a section in the config file, create a class to hold all the values, and then setup the dependecy injection to load the config file into the class
	
	AppSettings.json
	```json
	{
		"Smtp": {
			"Host": "smtp.example.com",
			"Port": "25",
			"UserName": "user",
			"Password": "P4$$w0rd",
			"Timeout": 1000
		}
	}
	```
		
	Class to store the SMTP settings from the config file
	```csharp
	internal class SmtpSettings
	{
		public string Host { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public int Timeout { get; set; }
	}	
	```

	Configuring dependency inject to load the settings from the config file into the SmptSettings so it can be used later
 	```csharp
	.ConfigureServices((context, collection) =>
    {
        collection.AddSingleton<SampleConfiguration>(provider =>
        {
            // Get the configuration from the host builder
            var config = provider.GetService<IConfiguration>();
                        
            // Load SampleConfiguration section from the config file
            var sc =  config.GetSection("SampleConfiguration").Get<SampleConfiguration>();
                return sc;
        });
    })
	```

	Using the SmtpSettings in the OnExecute method
	```csharp
	Task<int> OnExecuteAsync(SmtpSettings smtpSettings)
	{

		var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
		{
			Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
			Timeout = smtpSettings.Timeout
		};

		// Your code here
	}
	```

## LaunchSettings.json

The launchSettings.json file is used to configure the application when running in Visual Studio. 
You can specify multiple configurations, and each configuration can have its own set of command line arguments.

In the example below there are three profiles. 
1. The first profile will run the program with `-?` option to display the help
1. The second profile will run the `export` subcommand with `-?` option to display the help for the export subcommand
1. The third profile will run the `export` subcommand with the command line argument `export-file.csv` and the commandline option `delimiter`

```json 
{
	"profiles": {
		"-?": {
			"commandName": "Project",
			"commandLineArgs": "-?"
		},
		"export -?": {
			"commandName": "Project",
			"commandLineArgs": "export -?"
		},
		"export": {
			"commandName": "Project",
			"commandLineArgs": "export-file.csv --delimiter=|"
		}
	}
}
```

You can select the profile you want to run in Visual Studio by selecting it from the dropdown list in the toolbar.
![Selecting lauchSetting profile](Assets/Running-Subcommands.png)


# Configuring Your App for Use with the Commandline 


This template uses the McMaster CommandLineUtils library to parse commandline arguments.


## Sub Commands
This template supports sub commands. A sub command is a command that is executed as part of another command. 
For example the dotnet.exe command has sub commands like `dotnet new`, `dotnet build`, `dotnet run` etc. In this case
new, build and run are sub commands of the dotnet command, each supports its own set of command line arguments 
and even other sub commands. 

To configure a sub command, you will need to do the following:
1. Create a class for the sub command in the Command folder
1. You can name the class whatever you want, but it is recommended to name it a verb followed by the word Command. i.e. ExportCommand, FormatCommand, etc.
1. Add the [Command] attribute to the class and set the Name and Description properties
1. Add the [SubCommand] attribute to the Program class. i.e. `[Subcommand(typeof(SampleCommand))]`
1. Add one of the following methods to the class
	a. void OnExecute
	a. int OnExecute
	a. Task OnExecuteAsync
	a. Task&lt;int&gt; OnExecuteAsync
1. Add properties to the class with the `[Argument]` and `[Option]` attributes the command line arguments to the class. 
	```csharp 
	internal class SampleCommand
	{
		[Argument(0, Description = "The name and path of the source file")]
		public string SourcePath { get; set; }

		[Argument(1, Description = "TThe name and path of the destination file")]
		public string DestinationPath { get; set; }

		[Option("-c|--count", Description = "The rows to parse.")]
		public int Count { get; set; }
	
		[Option("-v|--verbose", Description = "Enable verbose output.")]
		public bool Verbose { get; set; }

		public void OnExecute()
		{
			// Your code here
		}
	}

	```


## Response Files

This template supports using response files. A response file is a text file that contains command line arguments.
A response file allows users to save frequently used combinations of command line arguments in a file, and then use that file to run the application.
A user can create multiple response files for different purposes, and then use the appropriate response file when running the application.
	



# Credits
This template uses the following libraries:

- McMaster CommandLineUtils https://natemcmaster.github.io/CommandLineUtils/
- Serilog https://serilog.net/
- 
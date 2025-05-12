# ConsoleAppTemplate  


## Description  


## Table of Contents  
1. [Description](#description)  
2. [Contents](#contents)  
3. [To Do](#to-do)  
4. [Project](#project)  
5. [Program.cs](#programcs)  
6. [Commandline Arguments](#commandline-arguments)  


# To Do / Checklist

There are a number of TODO in the code. Some of them are things you will need to do immediately,  
for example, implementing "Your code here". Others are things that you should do before releasing,  
your application. For example, adding descriptions to your application and sub commands. There are  
some things that you may never do, for example,  

To view the list of TODOs, in Visual Studio, under the View menu, select Task List.  

## Project  

- [ ] Set the version of .Net that you want to use
- [ ] Run package restore to get the latest packages - While the packages were updated when this template was made, and periodically updated, it is still best to run update before you get started
- [ ] Set the version number in the project file. See [Semver.org](https://semver.org/) for more information on symantec versioning.  
- [ ] 

## Program.cs  

- [ ] Add a name for the application  
- [ ] Add a description of the application  
- [ ] Determine if you are using sub commands or not and update the OnExecute method accordingly.
- [ ] If you are not using sub commands, and using the OnExecute method with async code, consider changing the singature to `Task<int> OnExecuteAsync (...)`

# Configuring Commandline Arguments

This template uses the McMaster CommandLineUtils library to parse commandline arguments.


# Credits
This template uses the following libraries:

- McMaster CommandLineUtils https://natemcmaster.github.io/CommandLineUtils/
- Serilog https://serilog.net/
- 
# Govtech-todo

This is a GovTech assignment to create a script/application that scans a specific directory and returns all the files that contains the word "TODO".

Application is a console application written in C# using Microsoft Visual Studio 2019. Main implementation can be found in the todo\Program.cs file.

## Build Instruction
Application is built in .NetCore 3.1 can be compiled in Visual Studio 2019.
Open the Assignment.sln in Visual Studio 2019 to compile and execute.

There is a self contain version in the executable folder built for win x64 environment

## Usage
<img src="https://github.com/ahpehgit/govtech-todo/blob/main/demo.gif" width="450px">

Start the application and a menu option will be shown:
 - 1 Change scan directory
 - 2 Scan directory
 - 3 Exit
 
1. can change the directory to scan the files from
2. will start scanning the directories for files

There is a Config.txt that contains 2 keys. Path and Ignore.
- Path - Specifies the root directory in which you want to start scanning the files from.\
- Ignore - Specifies the extensions of files which you do not want to check. You can specify multiple extension type and separate them by semi-colons.

Eg. \
Path=E:\\SomeFolder\SomeSubFolder\
Ignore=.svn;Logs;bin;obj

Note: The Assignment.exe and the Config.txt need to be residing in the same folder in order to execute.

# Govtech-todo

This is a GovTech assignment to create a script/application that scans a specific directory and returns all the files that contains the word "TODO".
Application is a console programs written in C# using Microsoft Visual Studio 2019.

# Usage

There is a Config.txt that contains 2 keys. Path and Ignore.
Path - Specifies the root directory in which you want to start scanning the files from.
Ignore - Specifies the extensions of files which you do not want to check. You can specify multiple extension type and separate them by semi-colons.

Eg. 
﻿Path=E:\\DotNetProjects\\SM3\\ClientBackend
Ignore=.svn;Logs;bin;obj

To run the app, please ensure that the Assignment.exe and the Config.txt are residing in the same folder.
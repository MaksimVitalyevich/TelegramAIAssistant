# TelegramAIAssistant

## Project Description:
My personal friendly AI-Bot to have a nice conversation. His main goal is to process various commands, using InlineKeyboardMarkup as his Main Menu. He helps users to:
1. Edit chats;
2. Open useful websites;
3. Tell facts or jokes;
4. Offer some mini-games;
5. Edit user profiles.
Telegramm-Bot was written on C# by used API's: "Telegram-Bot API", "OpenAI API", "Configuration Manager". As a bonus - he can log all messages into determined directory, by help of my library "LoggerLibrary"!

## Functionality:
-- Sending automatic messages & edited messages;
-- Executing commands, like: 'help', 'start' etc.;
-- Integration with "OpenAI API";
-- Log support.

## Installing & Run:
### ===1. Visual Studio 2022===
https://visualstudio.microsoft.com/ru/vs/community/
### ===2. Depencies===
### ===2.1 Install via .NET CLI (recommended)
Run following commands in terminal:
dotnet add package Telegram.Bot
dotnet add package OpenAI
dotnet add package System.Configuration
### ===2.2 Install via Packager Manager Console (for Visual Studio)
If you are using VisualStudio, open "Tools->NuGet Package Manager->Package Manager Console" and run
Install-Package Telegram.Bot
Install-Package OpenAI
Install-Package System.Configuration
For custom library "Logger" (LoggerLibrary), make sure it's added to your project references.

## Tokens:
Q: *Where to store tokens?*
A: 1st option - For secure token storage, you need to create "Environment Variables" inside your PC. You can do this by command "setx <TOKEN_NAME> <TOKEN_KEY>" or simply create Variables through
"SystemOptions->Additional->Environment Variables...->Create... <NAME> <VALUE>" when press "OK".
2nd option - Creating your personal "app.config" file! In parameter "connectionStrings" you must add: Name, Providername (if needed), connectionString.

*Release Version*
* First release

*Project Author*:
* GitHub: https://github.com/MaksimVitalyevich


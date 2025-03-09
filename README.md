# TelegramAIAssistant (CS Console Microsoft DotNET Version 9.0)

# Link to my TeleBot:
t.me/gpt098maks_bot

# --ChangeLog-- Updated to Version 2.5!
# -- Added:
 - Processing files. Now bot can analyze text files & generate short response, what he understood from it;
 - CommandManager class. Now bot's functionality depends on such manager to handle upcoming updates;
 - AsyncCmdList class. Whole list of asynchronized command-classes;
 - HistoryManager class. manager to edit various bot chats;
 - Now project depencies are stored in GlobalUsings file, including custom projects;
 - anti-spam check! Now both "/start" & "/help" cannot be executed more than 5 times!
# -- Fixes:
 - Now bot uses DeepInfra API instead of OpenAI!
 - Chat commands now work without exceptions;
 - command "/profile" now correctly show user's profile;
 - AI_Telebot class now only contains main logic and error handler;
 - inline "assistant_mode" command now responds correctly, while pressed;
 - Edited jokes & facts. Added more of them;
 - Overall code cleanup. Deleted unneeded switch blocks.

# Project Description:
My personal friendly AI-Bot to have a nice conversation. His main goal is to process various commands, using InlineKeyboardMarkup as his Main Menu. He helps users to:
1. Edit chats;
2. Open useful websites;
3. Tell facts or jokes;
4. Offer some mini-games;
5. Edit user profiles;
6. Analyze Documents/Files.
Telegramm-Bot was written on C# by used API's: "Telegram-Bot API", "DeepInfra API", "Configuration Manager". As a bonus - he can log all messages into determined directory, by help of my library "LoggerLibrary"!
*Why i used DeepInfra API instead of OpenAI?*
Because DeepInfra offers free-to-use daily requests on estimated max 2048 tokens! OpenAI, unfortunately, requires payment upon registrating any API key & even having free quota is not enough for public demonstration. On DeepInfra.com you can easily find tons of demo AI-models for public use!

# Used Patterns:
Update, Factory, State, Builder, Command

# Functionality:
-- Sending automatic messages & edited messages;
-- Executing commands, like: 'help', 'start' etc.;
-- Integration with "OpenAI API";
-- Log support.

# Installing & Run:
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

# Tokens:
Q: *Where to store tokens?*
A: 1st option - For secure token storage, you need to create "Environment Variables" inside your PC. You can do this by command "setx <TOKEN_NAME> <TOKEN_KEY>" or simply create Variables through
"SystemOptions->Additional->Environment Variables...->Create... <NAME> <VALUE>" when press "OK".
2nd option - Creating your personal "app.config" file! In parameter "connectionStrings" you must add: Name, Providername (if needed), connectionString.

# Known issues:
1) Procedural generation can give unknown symbols, that stumble normal reading sometimes. To fix that, ask bot to generate short responses.
2) Overall text may appear unreadable, with chance 5/100%. It can be fixed by resending same message twice.

# Screenshots:
1. Console Test:
![ConsoleTest1](https://github.com/user-attachments/assets/9e65055a-f19b-4b3d-b272-97585c31521a)
![ConsoleTest2](https://github.com/user-attachments/assets/8460b2b6-50c5-4ec2-9299-572065a61d5d)

2. Log Example:
![LogText](https://github.com/user-attachments/assets/7b7b05b1-7af4-49f8-8b87-e20a90560b29)

3. Telegramm-Bot Test:
![bot_start](https://github.com/user-attachments/assets/615b7abe-e925-42ca-b550-754eaeec548c)
![bot_mainmenu](https://github.com/user-attachments/assets/6c6f113d-5d2c-47a7-b912-c5fe643f4a2a)
![bot_helpmenu](https://github.com/user-attachments/assets/363b7fa6-5187-4bd3-bdd5-7cd3cd2f77b5)
![bot_assistantinlinemenu](https://github.com/user-attachments/assets/adbce435-44bf-4c32-a1cd-b75c7bda690b)
![bot_aimodeactive](https://github.com/user-attachments/assets/2b8867d5-7458-4c60-9848-a7b82616aff0)
![bot_replymenu](https://github.com/user-attachments/assets/75de2a1e-dbd8-45f3-bd25-96278a9a29f0)
![bot_replyoptions](https://github.com/user-attachments/assets/c2a133e8-9189-4df8-818d-8429370c47cb)
![bot_chatsettings](https://github.com/user-attachments/assets/905182c7-2dc7-4163-9477-4bef5072bb5a)

4. AI Testing (still buggy, needs optimization!):
![AI-Example1](https://github.com/user-attachments/assets/3fede739-9754-4e80-a239-0401dae3740a)
![AI-Example2](https://github.com/user-attachments/assets/872a9085-1406-4ad9-8b39-5cac21b6cbc6)

*Release Version*
* Second release

*Project Author*:
* GitHub: https://github.com/MaksimVitalyevich


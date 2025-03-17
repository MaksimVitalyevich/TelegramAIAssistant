using Assistant;
using TelegrammBot;

namespace Commands
{
    /// <summary>
    /// Менеджер команд бота. Обрабатывает все типы команд
    /// </summary>
    public class CommandManager
    {
        private readonly Dictionary<string, TextCommand> textCommands = [];
        private readonly Dictionary<string, CallbackCommand> callbackCommands = [];
        private IDefaultTextCommand? defaultTextCommand;
        private IDefaultCallbackCommand? defaultCallbackCommand;
        
        public CommandManager(TelegramBotClient client, AI_AssistantLogic ai)
        {
            RegisterCommands(client, ai);
        }

        private void RegisterCommands(TelegramBotClient client, AI_AssistantLogic ai)
        {
            // Команды использующиеся по умолчанию
            defaultTextCommand = new DefaultTextReplyCommand(client, ai);
            defaultCallbackCommand = new DefaultCallbackCommand(client);

            // Текстовые команды, Навигации по меню:
            AddTextCommand(new OnStart(client));
            AddTextCommand(new OnHelp(client));
            AddTextCommand(new OnShowAiModel(client));
            AddTextCommand(new OnProfileShow(client));
            AddTextCommand(new OnDateTimePrint(client));

            // Остальное (вызовы меню ответов и т. п.):
            AddTextCommand(new OnInlineMenu(client));
            AddTextCommand(new OnQuickReply(client));
            AddTextCommand(new OnMessageReply(client));

            // Команды запросов (callback), Вызовы меню/подменю:
            AddCallbackCommand(new OnWebUrlOpen(client));
            AddCallbackCommand(new OnAssistantMenuShow(client));
            AddCallbackCommand(new OnChatSettings(client));
            AddCallbackCommand(new OnRPSMenuShow(client));
            AddCallbackCommand(new OnProfileChange(client));

            // Определенные запросы-действия:
            AddCallbackCommand(new OnBackToMain(client));
            AddCallbackCommand(new OnAssistantOn(client));
            AddCallbackCommand(new OnAssistantOff(client));
            AddCallbackCommand(new OnNewChat(client));
            AddCallbackCommand(new OnSelectChat(client));
            AddCallbackCommand(new OnSwitchChat(client));
            AddCallbackCommand(new OnLoadChat(client));
            AddCallbackCommand(new OnDeleteChat(client));
            AddCallbackCommand(new OnRPSGameStart(client));
            AddCallbackCommand(new OnProfileNameSet(client));
            AddCallbackCommand(new OnProfileTopicSet(client));
            AddCallbackCommand(new OnProfileStyleSet(client));
        }

        private void AddTextCommand(TextCommand command) => textCommands[command.CommandName] = command;
        private void AddCallbackCommand(CallbackCommand command) => callbackCommands[command.CommandName] = command;

        public IDefaultTextCommand GetDefaultCommand() => defaultTextCommand;
        public IDefaultCallbackCommand GetDefaultCallbackCommand() => defaultCallbackCommand;
        public TextCommand? GetTextCommand(string commandName) => textCommands.TryGetValue(commandName, out var command) ? command : null;
        public CallbackCommand? GetCallbackCommand(string commandName) => callbackCommands.TryGetValue(commandName, out var command) ? command : null;
    }
}

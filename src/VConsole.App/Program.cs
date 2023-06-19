using VConsole;
using VConsole.App.Commands;

Parser.Default
    .RegisterCommand<Clone>()
    .RegisterCommand<Commit>()
    .RegisterCommand<Push>()
    .ParseArguments(args);

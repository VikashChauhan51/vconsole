using VConsole;
using VConsole.App.Commands;

await Parser.Default
    .RegisterCommand<Clone>()
    .RegisterCommand<Commit>()
    .RegisterCommand<Push>()
    .ParseArguments(args);

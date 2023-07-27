using System;
using System.Collections.Generic;
using poetools.Console.Commands;

namespace poetools.Console
{
    public class CommandRegistry : IDisposable
    {
        private readonly List<ICommand> _commands = new List<ICommand>();
        private readonly Dictionary<string, ICommand> _commandLookup = new Dictionary<string, ICommand>();
        private readonly DictionaryTree _autoCompleter = new DictionaryTree();

        public ICommand DefaultCommand { get; set; } = new ErrorCommand();
        public IEnumerable<ICommand> Commands => _commands;

        public event Action<ICommand> CommandAdded;
        public event Action<ICommand> CommandRemoved;

        public void Register(params ICommand[] commandList)
        {
            foreach (var command in commandList)
            {
                _commands.Add(command);
                _commandLookup.Add(command.Name, command);
                _autoCompleter.Insert(command.Name);

                AddAutoCompletions(command.AutoCompletions);
                CommandAdded?.Invoke(command);
            }
        }

        public void RemoveAutoCompletions(IEnumerable<string> autoCompletions)
        {
            foreach (var autoCompletion in autoCompletions)
                _autoCompleter.Remove(autoCompletion);
        }

        public void AddAutoCompletions(IEnumerable<string> autoCompletions)
        {
            foreach (var autoCompletion in autoCompletions)
                _autoCompleter.Insert(autoCompletion);
        }

        public void Unregister(params ICommand[] commandList)
        {
            foreach (var command in commandList)
            {
                _commands.Remove(command);
                _commandLookup.Remove(command.Name);
                _autoCompleter.Remove(command.Name);

                RemoveAutoCompletions(command.AutoCompletions);
                CommandRemoved?.Invoke(command);
            }
        }

        public void FindCommands(string commandName, List<string> results)
        {
            results.Clear();

            if (commandName.Length == 0)
                return;

            _autoCompleter.GetWithPrefix(commandName, results);

            for (int i = 0; i < results.Count; i++)
                results[i] = commandName + results[i];
        }

        public ICommand FindCommand(string commandName)
        {
            if (!_commandLookup.TryGetValue(commandName, out var result))
                result = DefaultCommand;

            return result;
        }

        public void Dispose()
        {
            DefaultCommand?.Dispose();

            // foreach (var command in _commands)
            //     command.Dispose();
        }
    }
}

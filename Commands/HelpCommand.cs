using System.Collections.Generic;
using UnityEngine;

namespace poetools.Console.Commands
{
    [CreateAssetMenu(menuName = RuntimeConsoleNaming.AssetMenuName + "/Commands/Help")]
    public class HelpCommand : Command
    {
        public override string Name => "help";
        public override string Help => "Print information on how to use other commands!";

        public override IEnumerable<string> AutoCompletions => _autoCompletions;

        private List<string> _autoCompletions = new List<string>();
        private CommandRegistry _commandRegistry;
        private RuntimeConsole _console;

        public override void Initialize(RuntimeConsole console)
        {
            _console = console;
            console.RegistrationFinished += HandleRegistrationFinished;
        }

        private void HandleRegistrationFinished()
        {
            _console.RegistrationFinished -= HandleRegistrationFinished;

            _commandRegistry = _console.CommandRegistry;
            _commandRegistry.CommandAdded += HandleCommandAdded;
            _commandRegistry.CommandRemoved += HandleCommandRemoved;

            _autoCompletions.Clear();

            foreach (var command in _commandRegistry.Commands)
                _autoCompletions.Add($"help {command.Name}");

            _commandRegistry.AddAutoCompletions(_autoCompletions);
        }

        public override void Dispose()
        {
            if (_commandRegistry != null)
            {
                _commandRegistry.Dispose();
                _commandRegistry.CommandAdded -= HandleCommandAdded;
                _commandRegistry.CommandRemoved -= HandleCommandRemoved;
            }
        }

        public override void Execute(string[] args, RuntimeConsole console)
        {
            if (args.Length != 1)
                return;

            var target = args[0];
            var command = _commandRegistry.FindCommand(target);
            console.Log("help", $"\n{command.Help}");
        }

        private void HandleCommandAdded(ICommand command)
        {
            _commandRegistry.RemoveAutoCompletions(_autoCompletions);
            _autoCompletions.Add(command.Name);
            _commandRegistry.AddAutoCompletions(_autoCompletions);
        }

        private void HandleCommandRemoved(ICommand command)
        {
            _commandRegistry.RemoveAutoCompletions(_autoCompletions);
            _autoCompletions.Remove(command.Name);
            _commandRegistry.AddAutoCompletions(_autoCompletions);
        }
    }
}

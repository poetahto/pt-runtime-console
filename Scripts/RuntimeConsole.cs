using System;
using System.Collections.Generic;
using System.Linq;
using poetools.Console.Commands;
using UnityEngine;

namespace poetools.Console
{
    // todo: draggable and close-box view
    // todo: command options and auto-complete for them

    public class RuntimeConsole : MonoBehaviour
    {
        [SerializeField]
        private LogPrefix logPrefix;

        [SerializeField]
        private UserPrefix userPrefix;

        [SerializeField]
        private int maxCommandHistory = 30;

        [SerializeField]
        private RuntimeConsoleView consoleViewPrefab;

        [SerializeField]
        private Command[] autoRegisterCommands;

        private List<ICommand> _commandInstances;
        private List<string> _suggestions;
        private int _autoCompleteIndex;
        private string _oldValue;

        public static event Action<CreateEvent> OnCreate;

        public CommandRegistry CommandRegistry { get; private set; }
        public RuntimeConsoleView View { get; private set; }
        private IInputHistory InputHistory { get; set; }
        public bool IsOpen => View.IsVisible();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            OnCreate = null;
        }

        private void Awake()
        {
            CommandRegistry = new CommandRegistry();
            InputHistory = new InputHistory(maxCommandHistory);
            _commandInstances = new List<ICommand>();
            _suggestions = new List<string>();

            // Initialize the view.
            View = Instantiate(consoleViewPrefab, transform);
            View.name = consoleViewPrefab.name;
            View.SetVisible(false);

            // Register default commands.
            foreach (Command command in autoRegisterCommands)
            {
                Command instance = Instantiate(command);
                instance.Initialize(this);
                CommandRegistry.Register(instance);
                _commandInstances.Add(instance);
            }

            OnCreate?.Invoke(new CreateEvent{Console = this});
        }

        private void OnDestroy()
        {
            foreach (ICommand command in _commandInstances)
                command.Dispose();

            CommandRegistry.Dispose();
        }

        private void OnEnable()
        {
            View.OnVisibilityChanged += HandleVisibilityChange;
            View.inputFieldDisplay.onSubmit.AddListener(HandleSubmit);
            View.inputFieldDisplay.onValueChanged.AddListener(HandleInputChange);
        }

        private void OnDisable()
        {
            View.OnVisibilityChanged -= HandleVisibilityChange;
            View.inputFieldDisplay.onSubmit.RemoveListener(HandleSubmit);
            View.inputFieldDisplay.onValueChanged.RemoveListener(HandleInputChange);
        }

        // === Public API ===
        public void Log(string category, string message)
        {
            string header = logPrefix.GenerateMessage(category);
            View.Text += $"\n{header}{message}";
        }

        public void ToggleVisibility()
        {
            View.SetVisible(!View.IsVisible());
        }

        public void CycleAutoComplete()
        {
            _autoCompleteIndex++;
            UpdateAutoCompleteText();
        }

        public void MoveHistoryBackward()
        {
            if (InputHistory.TryMoveBackwards(out string prevCommand))
                View.inputFieldDisplay.text = prevCommand;
        }

        public void MoveHistoryForward()
        {
            if (InputHistory.TryMoveForwards(out string nextCommand))
                View.inputFieldDisplay.text = nextCommand;
        }

        // === Event Handlers ===
        private void HandleSubmit(string input)
        {
            string[] splitInput = input.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            if (input.Length > 0 && splitInput.Length > 0)
            {
                string[] args = ArgumentTools.Parse(splitInput);
                ICommand command = CommandRegistry.FindCommand(splitInput[0]);
                View.Text += '\n' + userPrefix.GenerateMessage(input);

                command.Execute(args, this);
                InputHistory.AddEntry(input);
            }

            ResetInputField();
        }

        private void HandleVisibilityChange(bool wasVisible, bool isVisible)
        {
            if (isVisible)
                ResetInputField();
        }

        private void HandleInputChange(string value)
        {
            string[] splitInput = value.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            if (splitInput.Length == 0)
            {
                // case: we have no input.
                View.autoCompleteDisplay.text = string.Empty;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                int newSpaceCount = value.Count(c => c == ' ');
                int oldSpaceCount = _oldValue.Count(c => c == ' ');

                // If we entered a space, apply the auto-complete.
                if (_suggestions.Any() && newSpaceCount > oldSpaceCount)
                {
                    _autoCompleteIndex = 0;
                    string autoCompleteText = View.autoCompleteDisplay.text;
                    View.inputFieldDisplay.text = autoCompleteText + " ";
                    View.inputFieldDisplay.caretPosition = View.inputFieldDisplay.text.Length;
                }
            }

            CommandRegistry.FindCommands(View.inputFieldDisplay.text, _suggestions);
            UpdateAutoCompleteText();
            _oldValue = value;
        }

        // === Helpers ===
        private void UpdateAutoCompleteText()
        {
            int index = (int) Mathf.Repeat(_autoCompleteIndex, _suggestions.Count);
            string autoCompleteText = _suggestions.Count > 0 ? _suggestions[index] : "";
            View.autoCompleteDisplay.text = autoCompleteText;
        }

        private void ResetInputField()
        {
            View.inputFieldDisplay.text = string.Empty;
            View.inputFieldDisplay.ActivateInputField();
            InputHistory.Clear();
        }

        // === Structures ===
        public struct CreateEvent
        {
            public RuntimeConsole Console;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using poetools.Console.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace poetools.Console
{
    // NOT DOABLE: move init method into base class
    // DONE: allow customization of command prefix format
    // todo: draggable and close-box view
    // todo: command options and auto-complete for them
    // todo: clean up
    // todo: do singleton / static pass on codebase to ensure quick-play-mode compatibility

    // INVARIANTS - autoCompleteIndex should always be in range of autoCompleteCommands
    // INVARIANTS - all reference-type fields must not be null.

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

        private List<string> _suggestions = new List<string>();
        private int _autoCompleteIndex;
        private string _oldValue;

        public static event Action<CreateEvent> OnCreate;
        public event Action RegistrationFinished;

        public CommandRegistry CommandRegistry { get; private set; }
        public RuntimeConsoleView View { get; private set; }
        private IInputHistory InputHistory { get; set; }

        private void Awake()
        {
            CommandRegistry = new CommandRegistry();
            InputHistory = new InputHistory(maxCommandHistory);

            // Initialize the view.
            View = Instantiate(consoleViewPrefab, transform);
            View.name = consoleViewPrefab.name;
            View.SetVisible(false);

            // Register default commands.
            foreach (Command command in autoRegisterCommands)
            {
                command.Initialize(this);
                CommandRegistry.Register(command);
            }

            OnCreate?.Invoke(new CreateEvent{Console = this});
            RegistrationFinished?.Invoke();
        }

        private void OnDestroy()
        {
            CommandRegistry.Dispose();
            OnCreate = null;
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
                var newSpaceCount = value.Count(c => c == ' ');
                var oldSpaceCount = _oldValue.Count(c => c == ' ');

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

        private void UpdateAutoCompleteText()
        {
            var index = (int) Mathf.Repeat(_autoCompleteIndex, _suggestions.Count);
            string autoCompleteText = _suggestions.Count > 0 ? _suggestions[index] : "";
            View.autoCompleteDisplay.text = autoCompleteText;
        }

        private void CycleAutoCompleteOption(int direction)
        {
            _autoCompleteIndex += direction;
            UpdateAutoCompleteText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && InputHistory.TryMoveBackwards(out string prevCommand))
                View.inputFieldDisplay.text = prevCommand;

            if (Input.GetKeyDown(KeyCode.DownArrow) && InputHistory.TryMoveForwards(out string nextCommand))
                View.inputFieldDisplay.text = nextCommand;

            if (Input.GetKeyDown(KeyCode.Tab))
                CycleAutoCompleteOption(1);
        }

        public void Log(string category, string message)
        {
            string header = logPrefix.GenerateMessage(category);
            View.Text += $"\n{header}{message}";
        }

        private void ResetInputField()
        {
            View.inputFieldDisplay.text = string.Empty;
            View.inputFieldDisplay.ActivateInputField();
            InputHistory.Clear();
        }

        public struct CreateEvent
        {
            public RuntimeConsole Console;
        }
    }
}

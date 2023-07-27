using UnityEngine;

namespace poetools.Console
{
    /// <summary>
    /// Old-input system controller for the console.
    /// </summary>
    public class InputConsoleController : MonoBehaviour
    {
        public KeyCode visibilityToggleKey = KeyCode.BackQuote;
        public KeyCode cycleAutoCompleteKey = KeyCode.UpArrow;
        public KeyCode historyBackwardKey = KeyCode.DownArrow;
        public KeyCode historyForwardKey = KeyCode.Tab;

        private void Update()
        {
            bool toggleVisibility = Input.GetKeyDown(visibilityToggleKey);
            bool cycleAutoComplete = Input.GetKeyDown(cycleAutoCompleteKey);
            bool historyBackward = Input.GetKeyDown(historyBackwardKey);
            bool historyForward = Input.GetKeyDown(historyForwardKey);

            foreach (var console in FindObjectsOfType<RuntimeConsole>())
            {
                if (toggleVisibility)
                    console.ToggleVisibility();

                if (console.IsOpen)
                {
                    if (cycleAutoComplete)
                        console.CycleAutoComplete();

                    if (historyBackward)
                        console.MoveHistoryBackward();

                    if (historyForward)
                        console.MoveHistoryForward();
                }
            }
        }
    }
}

using UnityEngine;

namespace poetools.Console
{
    public class InputConsoleController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                foreach (var runtimeConsole in FindObjectsOfType<RuntimeConsole>())
                    runtimeConsole.View.SetVisible(!runtimeConsole.View.IsVisible());
            }
        }
    }
}

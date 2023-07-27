using UnityEngine;

namespace poetools.Console.Commands
{
    [CreateAssetMenu(menuName = RuntimeConsoleNaming.AssetMenuName + "/Commands/Clear")]
    public class ClearCommand : Command
    {
        public override string Name => "clear";
        public override string Help => "Removes all messages from the console.";

        public override void Execute(string[] args, RuntimeConsole console)
        {
            console.View.Text = string.Empty;
            console.Log(Name, "Cleared Console.");
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace poetools.Console.Commands
{
    [CreateAssetMenu(menuName = RuntimeConsoleNaming.AssetMenuName + "/Commands/Screen")]
    public class ScreenCommand : Command
    {
        public override string Name => "screen";

        public override IEnumerable<string> AutoCompletions => new[]
        {
            "screen max", "screen exclusive", "screen windowed",
            "screen resolution", "screen refresh", "screen msaa", "screen fov",
            "screen vsync", "screen vsync true", "screen vsync false",
        };

        public override void Execute(string[] args, RuntimeConsole console)
        {
            if (args.Length >= 1)
            {
                switch (args[0])
                {
                    case "max":
                        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                        break;
                    case "exclusive":
                        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                        break;
                    case "windowed":
                        Screen.fullScreenMode = FullScreenMode.Windowed;
                        break;
                    case "resolution" when args.Length >= 3 && int.TryParse(args[1], out var width) && int.TryParse(args[2], out var height):
                        Screen.SetResolution(width, height, Screen.fullScreenMode);
                        break;
                    case "refresh" when args.Length >= 2 && int.TryParse(args[1], out var rate):
                        Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, rate);
                        break;
                    case "vsync" when args.Length >= 2 && bool.TryParse(args[1], out var vsync):
                        QualitySettings.vSyncCount = vsync ? 1 : 0;
                        break;
                    case "fov" when args.Length >= 2 && int.TryParse(args[1], out var fov):
                        Camera.main.fieldOfView = fov;
                        break;
                }
            }
        }
    }
}

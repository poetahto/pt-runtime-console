using System.Collections.Generic;
using poetools.Console.Commands;
using UnityEngine;

namespace poetools.Console
{
    [CreateAssetMenu(menuName = RuntimeConsoleNaming.AssetMenuName + "/Settings")]
    public class RuntimeConsoleSettings : ScriptableObject
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

        // Public API

        public LogPrefix LogPrefix => logPrefix;
        public UserPrefix UserPrefix => userPrefix;
        public int MaxCommandHistory => maxCommandHistory;
        public RuntimeConsoleView ConsoleViewPrefab => consoleViewPrefab;
        public IEnumerable<Command> AutoRegisterCommands => autoRegisterCommands;
    }
}

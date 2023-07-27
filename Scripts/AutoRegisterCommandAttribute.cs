using System;
using poetools.Console.Commands;
using UnityEngine;

namespace poetools.Console
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterCommandAttribute : Attribute {}

    public static class AutoCommandRegister
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            RuntimeConsole.OnCreate += AutoRegister;
        }

        private static void AutoRegister(RuntimeConsole.CreateEvent eventData)
        {
            var commandRegistry = eventData.Console.CommandRegistry;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (typeof(ICommand).IsAssignableFrom(type) && type.GetCustomAttributes(typeof(AutoRegisterCommandAttribute), true).Length > 0)
                    {
                        var instance = Activator.CreateInstance(type) as ICommand;
                        commandRegistry.Register(instance);
                    }
                }
            }
        }
    }
}

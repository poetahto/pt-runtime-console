using System.Collections.Generic;

namespace poetools.Console
{
    public static class ArgumentTools
    {
        public static string Combine(IEnumerable<string> args)
        {
            string result = "";

            foreach (string arg in args)
                result += arg + ' ';

            result = result.TrimEnd(' ');
            return result;
        }

        public static string Combine(IList<string> args, int start)
        {
            return Combine(args, start, args.Count);
        }

        public static string Combine(IList<string> args, int start, int end)
        {
            string result = "";

            for (int i = start; i < end; i++)
                result += args[i] + ' ';

            result = result.TrimEnd(' ');
            return result;
        }

        public static string[] Parse(string[] input)
        {
            string[] args = new string[input.Length - 1];

            for (int i = 0; i < args.Length; i++)
                args[i] = input[i + 1];

            return args;
        }
    }
}

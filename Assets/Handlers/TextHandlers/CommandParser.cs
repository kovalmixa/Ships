using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.Handlers.TextHandlers
{
    public enum CommandElementType { Literal, Argument }

    public readonly struct CommandArgs
    {
        private readonly string[] _args;

        public int Count => _args.Length;

        public CommandArgs(string[] args)
        {
            _args = args ?? System.Array.Empty<string>();
        }

        public string GetString(int index, string defaultValue = "")
        {
            return index >= 0 && index < _args.Length ? _args[index] : defaultValue;
        }

        public int GetInt(int index, int defaultValue = 0)
        {
            if (index >= 0 && index < _args.Length && int.TryParse(_args[index], out int result))
                return result;
            return defaultValue;
        }

        public float GetFloat(int index, float defaultValue = 0f)
        {
            if (index >= 0 && index < _args.Length && float.TryParse(_args[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;
            return defaultValue;
        }

        public string GetJoined(int startIndex = 0, string separator = " ")
        {
            if (startIndex >= _args.Length) return string.Empty;
            return string.Join(separator, _args.Skip(startIndex));
        }
    }

    public static class CommandParser
    {
        private static Dictionary<string, Action<CommandArgs>> _commands = new()
        {
            {
                "print",
                args => Debug.Log(args.GetJoined(startIndex: 0))
            },
        };

        private static Dictionary<Func<string, bool>, CommandElementType> _rules = new()
        {
            { text => !string.IsNullOrEmpty(text) && !text.StartsWith("-"), CommandElementType.Literal },
            { text => true, CommandElementType.Argument }
        };

        public static void ParseCommandTree(string text)
        {
            var tokens = Tokenizer<CommandElementType>.GetTokens(text, _rules);
            //ExecuteCommand(IEnumerable<Token<CommandElement>> tokens);
        }

        //private static void ExecuteCommand(IEnumerable<Token<CommandElement>> tokens, int index = 0)
        //{
            
        //}

    }
}

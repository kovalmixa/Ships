using System;
using System.Collections.Generic;

namespace Assets.Handlers.TextHandlers
{
    public readonly struct Token<T>
    {
        public T Type { get; }
        public string Value { get; }

        public Token(T type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public static class Tokenizer<T>
    {
        public static IEnumerable<Token<T>> GetTokens(string text, Dictionary<Func<string, bool>, T> rules)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;

            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                foreach (var rule in rules)
                {
                    if (rule.Key.Invoke(word))
                    {
                        yield return new Token<T>(rule.Value, word);
                        break;
                    }
                }
            }
        }
    }
}
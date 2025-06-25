using ModelLanguageCompiler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelLanguageCompiler.ViewModel
{
    public class Lexer
    {
        private static readonly string[] keywords = { "readln", "writeln", "if", "else", "while", "for", "to", "step", "next", "begin", "end" };
        private static readonly string[] types = { "%", "!", "$" };
        private static readonly string[] logicalConstants = { "true", "false" };
        private static readonly string operatorsPattern = @"(!=|==|<=|>=|\|\||&&|:=|[+\-*/<>=])";
        private static readonly string delimitersPattern = @"[{}();,]";

        public List<Token> Tokenize(string code)
        {
            var tokens = new List<Token>();
            var lines = code.Split('\n');
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                int column = 0;
                var matches = Regex.Matches(line, $@"\s*(\w+|{operatorsPattern}|{delimitersPattern}|\S)");
                foreach (Match match in matches)
                {
                    string value = match.Groups[1].Value;
                    column = match.Index + 1;
                    if (string.IsNullOrWhiteSpace(value)) continue;

                    Token token = new Token { Value = value, Line = lineIndex + 1, Column = column };

                    if (Array.Exists(keywords, k => k == value) || Array.Exists(types, t => t == value))
                        token.Type = TokenType.Keyword;
                    else if (Array.Exists(logicalConstants, c => c == value))
                        token.Type = TokenType.LogicalConstant;
                    else if (Regex.IsMatch(value, @"^\d+(\.\d+)?$"))
                        token.Type = TokenType.Number;
                    else if (Regex.IsMatch(value, operatorsPattern))
                        token.Type = TokenType.Operator;
                    else if (Regex.IsMatch(value, delimitersPattern))
                        token.Type = TokenType.Delimiter;
                    else if (Regex.IsMatch(value, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
                        token.Type = TokenType.Identifier;
                    else
                        token.Type = TokenType.Error;

                    tokens.Add(token);
                }
            }
            return tokens;
        }
    }
}

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
        private static readonly Regex hexNumberRegex = new Regex(@"^[0-9A-Fa-f]+[hH]$");

        public List<Token> Tokenize(string code)
        {
            var tokens = new List<Token>();
            var lines = code.Split('\n');
            int currentLine = 1;
            int globalPos = 0;

            while (globalPos < code.Length)
            {
                if (char.IsWhiteSpace(code[globalPos]))
                {
                    if (code[globalPos] == '\n') currentLine++;
                    globalPos++;
                    continue;
                }

                if (globalPos + 1 < code.Length && code[globalPos] == '/' && code[globalPos + 1] == '*')
                {
                    int commentStart = globalPos;
                    int commentLine = currentLine;
                    int commentColumn = GetColumnNumber(code, commentStart, currentLine);

                    globalPos += 2;

                    while (globalPos < code.Length && !(code[globalPos] == '*' &&
                           globalPos + 1 < code.Length && code[globalPos + 1] == '/'))
                    {
                        if (code[globalPos] == '\n') currentLine++;
                        globalPos++;
                    }

                    if (globalPos >= code.Length)
                    {
                        tokens.Add(new Token
                        {
                            Type = TokenType.Error,
                            Value = "Незакрытый комментарий",
                            Line = commentLine,
                            Column = commentColumn
                        });
                        break;
                    }

                    string commentContent = code.Substring(commentStart, globalPos - commentStart + 2);
                    tokens.Add(new Token
                    {
                        Type = TokenType.Comment,
                        Value = commentContent,
                        Line = commentLine,
                        Column = commentColumn
                    });

                    globalPos += 2;
                    continue;
                }

                var remainingCode = code.Substring(globalPos);
                var tokenMatch = Regex.Match(remainingCode, $@"^(\w+|{operatorsPattern}|{delimitersPattern}|\S)");

                if (tokenMatch.Success)
                {
                    string value = tokenMatch.Groups[1].Value;
                    int column = GetColumnNumber(code, globalPos, currentLine);

                    Token token = new Token
                    {
                        Value = value,
                        Line = currentLine,
                        Column = column
                    };

                    if (Array.Exists(keywords, k => k == value) || Array.Exists(types, t => t == value))
                        token.Type = TokenType.Keyword;
                    else if (Array.Exists(logicalConstants, c => c == value))
                        token.Type = TokenType.LogicalConstant;
                    else if (Regex.IsMatch(value, @"^\d+(\.\d+)?$"))
                        token.Type = TokenType.Number;
                    else if (hexNumberRegex.IsMatch(value))
                        token.Type = TokenType.HexNumber;
                    else if (Regex.IsMatch(value, operatorsPattern))
                        token.Type = TokenType.Operator;
                    else if (Regex.IsMatch(value, delimitersPattern))
                        token.Type = TokenType.Delimiter;
                    else if (Regex.IsMatch(value, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
                        token.Type = TokenType.Identifier;
                    else
                        token.Type = TokenType.Error;

                    tokens.Add(token);
                    globalPos += value.Length;

                    int newlines = value.Count(c => c == '\n');
                    if (newlines > 0)
                    {
                        currentLine += newlines;
                    }
                }
                else
                {
                    globalPos++;
                }
            }

            return tokens;
        }

        private int GetColumnNumber(string code, int position, int currentLine)
        {
            int lineStart = code.LastIndexOf('\n', position);
            return lineStart == -1 ? position + 1 : position - lineStart;
        }
    }
}

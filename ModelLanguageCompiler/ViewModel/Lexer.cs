using ModelLanguageCompiler.Model;
using System.Text.RegularExpressions;

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
        private static readonly Regex binaryNumberRegex = new Regex(@"^[01]+[Bb]$");
        private static readonly Regex octNumberRegex = new Regex(@"^[0-7]+[Oo]$");
        private static readonly Regex decNumberRegex = new Regex(@"^[0-10]+[Dd]$");

        public static List<Token> Tokenize(string code, out List<string> numbers, out List<string> ids)
        {
            numbers = new List<string>();
            ids = new List<string>();
            var tokens = new List<Token>();
            var lines = code.Split('\n');
            int globalPos = 0;

            while (globalPos < code.Length)
            {
                if (char.IsWhiteSpace(code[globalPos]))
                {
                    globalPos++;
                    continue;
                }

                if (globalPos + 1 < code.Length && code[globalPos] == '/' && code[globalPos + 1] == '*')
                {
                    globalPos += 2;

                    while (globalPos < code.Length
                            && !(code[globalPos] == '*'
                            && globalPos + 1 < code.Length
                            && code[globalPos + 1] == '/'))
                    {
                        globalPos++;
                    }

                    if (globalPos >= code.Length)
                    {
                        break;
                    }

                    globalPos += 2;
                    continue;
                }

                var remainingCode = code.Substring(globalPos);
                var tokenMatch = Regex.Match(remainingCode, $@"^(\w+|{operatorsPattern}|{delimitersPattern}|\S)");

                if (tokenMatch.Success)
                {
                    string value = tokenMatch.Groups[1].Value;
                    Token token = new Token
                    {
                        Value = value
                    };

                    if (Array.Exists(Lexer.keywords, k => k == value) || Array.Exists(Lexer.types, t => t == value))
                    {
                        token.Type = TokenType.Keyword;
                        if (GetKeywords().Contains(value))
                        {
                            token.Index = KeywordIndex;
                            token.TableIndex = GetKeywords().IndexOf(value) + 1;
                        }
                        else
                        {
                            token.Index = DelimitersIndex;
                            token.TableIndex = GetDelimiters().IndexOf(value) + 1;
                        }
                    }
                    else if (Array.Exists(logicalConstants, c => c == value))
                    {
                        token.Type = TokenType.LogicalConstant;
                        token.Index = KeywordIndex;
                        token.TableIndex = GetKeywords().IndexOf(value) + 1;
                    }
                    else if (Regex.IsMatch(value, @"^\d+(\.\d+)?$"))
                    {
                        token.Type = TokenType.Number;
                        token.Index = NumberIndex;
                        if (!numbers.Contains(value))
                        {
                            numbers.Add(value);
                        }
                        token.TableIndex = numbers.IndexOf(value) + 1;
                    }
                    else if (hexNumberRegex.IsMatch(value))
                    {
                        token.Type = TokenType.Number;
                        token.Index = NumberIndex;
                        if (!numbers.Contains(value))
                        {
                            numbers.Add(value);
                        }
                        token.TableIndex = numbers.IndexOf(value) + 1;
                    }
                    else if (binaryNumberRegex.IsMatch(value))
                    {
                        token.Type = TokenType.Number;
                        token.Index = NumberIndex;
                        if (!numbers.Contains(value))
                        {
                            numbers.Add(value);
                        }
                        token.TableIndex = numbers.IndexOf(value) + 1;
                    }
                    else if (octNumberRegex.IsMatch(value))
                    {
                        token.Type = TokenType.Number;
                        token.Index = NumberIndex;
                        if (!numbers.Contains(value))
                        {
                            numbers.Add(value);
                        }
                        token.TableIndex = numbers.IndexOf(value) + 1;
                    }
                    else if (decNumberRegex.IsMatch(value))
                    {
                        token.Type = TokenType.Number;
                        token.Index = NumberIndex;
                        if (!numbers.Contains(value))
                        {
                            numbers.Add(value);
                        }
                        token.TableIndex = numbers.IndexOf(value) + 1;
                    }
                    else if (Regex.IsMatch(value, operatorsPattern))
                    {
                        token.Type = TokenType.Operator;
                        token.Index = DelimitersIndex;
                        token.TableIndex = GetDelimiters().IndexOf(value) + 1;
                    }
                    else if (Regex.IsMatch(value, delimitersPattern))
                    {
                        token.Type = TokenType.Delimiter;
                        token.Index = DelimitersIndex;
                        token.TableIndex = GetDelimiters().IndexOf(value) + 1;
                    }
                    else if (Regex.IsMatch(value, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
                    {
                        token.Type = TokenType.Identifier;
                        token.Index = IdsIndex;
                        if (!ids.Contains(value))
                        {
                            ids.Add(value);
                        }
                        token.TableIndex = ids.IndexOf(value) + 1;
                    }
                    else
                    {
                        token.Type = TokenType.Error;
                    }

                    tokens.Add(token);
                    globalPos += value.Length;
                }
                else
                {
                    globalPos++;
                }
            }

            return tokens;
        }

        public static List<string> GetKeywords()
        {
            var result = new List<string>(keywords);
            result.AddRange(logicalConstants);
            return result;
        }

        public static List<string> GetDelimiters()
        {
            var delimiters = new List<string> { "{", "}", "(", ")", ";", "," };
            var operators = new List<string> { "!=", "==", "<=", ">=", "||", "&&", ":=", "+", "-", "*", "/", "<", ">", "=" };

            var result = new List<string>(delimiters);
            result.AddRange(operators);
            result.AddRange(Lexer.types);
            return result;
        }

        private static int KeywordIndex => 1;
        private static int DelimitersIndex => 2;
        private static int NumberIndex => 3;
        private static int IdsIndex => 4;

    }
}

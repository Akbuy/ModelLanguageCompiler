using ModelLanguageCompiler.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ModelLanguageCompiler.ViewModel
{
    public class Parser(List<Token> tokens)
    {
        private readonly List<Token> _tokens = tokens;
        private int _pos = 0;

        private Token Current => GetNextNonCommentToken();

        private Token GetNextNonCommentToken()
        {
            int pos = _pos;
            while (pos < _tokens.Count && _tokens[pos].Type == TokenType.Comment)
            {
                pos++;
            }
            return pos < _tokens.Count ? _tokens[pos] : _tokens[^1];
        }
        private void Match(string expected)
        {
            var current = GetNextNonCommentToken();
            if (current.Value == expected)
            {
                _pos++;
                while (_pos < _tokens.Count && _tokens[_pos].Type == TokenType.Comment)
                {
                    _pos++;
                }
            }
            else
            {
                throw new Exception($"Ожидалось '{expected}', найдено '{current.Value}' на {current.Line} строке, {current.Column} позиция");
            }
        }

        public void ParseProgram()
        {
            Match("{");
            while (Current.Value != "}")
            {
                if (IsType(Current.Value))
                    ParseDeclaration();
                else
                    ParseStatement();
            }
            Match("}");
        }

        private void ParseDeclaration()
        {
            if (!IsType(Current.Value))
                throw new Exception("Ожидался тип");
            _pos++;

            MatchIdentifier();
            while (Current.Value == ",")
            {
                _pos++;
                MatchIdentifier();
            }
            Match(";");
        }

        private void ParseStatement()
        {
            if (Current.Type == TokenType.Identifier && Peek().Value == ":=")
                if (Peek(2).Value == "to")
                {
                    ParseFor();
                }
                else
                {
                    ParseAssignment();
                }
            else if (Current.Value == "if")
                ParseIf();
            else if (Current.Value == "while")
                ParseWhile();
            else if (Current.Value == "for")
                ParseFor();
            else if (Current.Value == "writeln")
                ParseWrite();
            else if (Current.Value == "readln")
                ParseRead();
            else if (Current.Value == "begin")
                ParseCompound();
            else
                throw new Exception($"Неизвестный оператор: {Current.Value}");
        }

        private void ParseAssignment()
        {
            MatchIdentifier();
            Match(":=");
            ParseExpression();
            Match(";");
        }

        private void ParseIf()
        {
            Match("if");
            Match("(");
            ParseExpression();
            Match(")");
            ParseStatement();
            if (Current.Value == "else")
            {
                _pos++;
                ParseStatement();
            }
        }

        private void ParseWhile()
        {
            Match("while");
            Match("(");
            ParseExpression();
            Match(")");
            ParseStatement();
        }

        private void ParseFor()
        {
            Match("for");
            ParseAssignment();
            Match("to");
            ParseExpression();
            if (Current.Value == "step")
            {
                _pos++;
                ParseExpression();
            }
            ParseStatement();
            Match("next");
        }

        private void ParseCompound()
        {
            Match("begin");
            ParseStatement();
            while (Current.Value == ";")
            {
                _pos++;
                ParseStatement();
            }
            Match("end");
        }

        private void ParseWrite()
        {
            Match("writeln");
            ParseExpression();
            while (Current.Value == ",")
            {
                _pos++;
                ParseExpression();
            }
            Match(";");
        }

        private void ParseRead()
        {
            Match("readln");
            MatchIdentifier();
            while (Current.Value == ",")
            {
                _pos++;
                MatchIdentifier();
            }
            Match(";");
        }

        private void ParseExpression()
        {
            ParseOperand();
            while (IsRelOp(Current.Value))
            {
                _pos++;
                ParseOperand();
            }
        }

        private void ParseOperand()
        {
            ParseTerm();
            while (IsAddOp(Current.Value))
            {
                _pos++;
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();
            while (IsMulOp(Current.Value))
            {
                _pos++;
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (Current.Type == TokenType.Identifier ||
                Current.Type == TokenType.Number ||
                Current.Type == TokenType.HexNumber ||
                Current.Type == TokenType.LogicalConstant)
            {
                _pos++;
            }
            else if (Current.Value == "(")
            {
                _pos++;
                ParseExpression();
                Match(")");
            }
            else if (Current.Value == "!")
            {
                _pos++;
                ParseFactor();
            }
            else
                throw new Exception("Неверный множитель: " + Current.Value);
        }

        private Token Peek(int offset = 1) => _pos + offset < _tokens.Count ? _tokens[_pos + offset] : _tokens[^1];

        private void MatchIdentifier()
        {
            if (Current.Type == TokenType.Identifier)
                _pos++;
            else
                throw new Exception("Ожидался идентификатор");
        }

        private static bool IsRelOp(string val) => val is "!=" or "==" or "<" or "<=" or ">" or ">=";
        private static bool IsAddOp(string val) => val is "+" or "-" or "||";
        private static bool IsMulOp(string val) => val is "*" or "/" or "&&";
        private static bool IsType(string val) => val is "%" or "!" or "$";
    }
}
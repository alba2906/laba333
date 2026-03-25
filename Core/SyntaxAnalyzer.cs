using System.Collections.Generic;
using System.Linq;

namespace Laba1.Core
{
    public sealed class SyntaxAnalyzer
    {
        private List<Token> _tokens = new();
        private int _position;
        private SyntaxAnalysisResult _result = new();

        public SyntaxAnalysisResult Analyze(IEnumerable<Token> tokens)
        {
            _tokens = tokens
                .Where(t => t.Type != TokenType.Whitespace)
                .ToList();

            _position = 0;
            _result = new SyntaxAnalysisResult();

            if (_tokens.Count == 0)
            {
                _result.Errors.Add(new SyntaxError
                {
                    InvalidFragment = string.Empty,
                    Line = 1,
                    Column = 1,
                    Message = "Пустой ввод. Ожидалось объявление словаря."
                });
                return _result;
            }

            ParseDictionaryDeclaration();

            return _result;
        }

        private void ParseDictionaryDeclaration()
        {
            // 1. Dictionary
            if (!ExpectWord("Dictionary", "Ожидалось слово 'Dictionary'."))
            {
                SkipToEnd();
                return;
            }

            // 2. <
            if (!ExpectSymbol(TokenType.LessThan, "Ожидался символ '<'."))
            {
                SkipToEnd();
                return;
            }

            // 3. int
            if (!ExpectWord("int", "Ожидалось ключевое слово 'int'."))
            {
                SkipToEnd();
                return;
            }

            // 4. ,
            if (!ExpectSymbol(TokenType.Comma, "Ожидался символ ','."))
            {
                SkipToEnd();
                return;
            }

            // 5. string
            if (!ExpectWord("string", "Ожидалось ключевое слово 'string'."))
            {
                SkipToEnd();
                return;
            }

            // 6. >
            if (!ExpectSymbol(TokenType.GreaterThan, "Ожидался символ '>'."))
            {
                SkipToEnd();
                return;
            }

            // 7. Идентификатор - КРИТИЧЕСКАЯ ОШИБКА
            if (!ExpectIdentifier())
            {
                // Пропускаем до ';' или конца и выходим
                SkipToSemicolonOrEnd();
                return;
            }

            // 8. =
            if (!ExpectSymbol(TokenType.Assign, "Ожидался символ '='."))
            {
                SkipToEnd();
                return;
            }

            // 9. new
            if (!ExpectWord("new", "Ожидалось ключевое слово 'new'."))
            {
                SkipToEnd();
                return;
            }

            // 10. Dictionary
            if (!ExpectWord("Dictionary", "Ожидалось слово 'Dictionary'."))
            {
                SkipToEnd();
                return;
            }

            // 11. <
            if (!ExpectSymbol(TokenType.LessThan, "Ожидался символ '<'."))
            {
                SkipToEnd();
                return;
            }

            // 12. int
            if (!ExpectWord("int", "Ожидалось ключевое слово 'int'."))
            {
                SkipToEnd();
                return;
            }

            // 13. ,
            if (!ExpectSymbol(TokenType.Comma, "Ожидался символ ','."))
            {
                SkipToEnd();
                return;
            }

            // 14. string
            if (!ExpectWord("string", "Ожидалось ключевое слово 'string'."))
            {
                SkipToEnd();
                return;
            }

            // 15. >
            if (!ExpectSymbol(TokenType.GreaterThan, "Ожидался символ '>'."))
            {
                SkipToEnd();
                return;
            }

            // 16. {
            if (!ExpectSymbol(TokenType.OpenBrace, "Ожидался символ '{' в начале инициализации."))
            {
                SkipToEnd();
                return;
            }

            // 17. Элементы словаря
            ParseDictionaryElementList();

            // 18. }
            ExpectSymbol(TokenType.CloseBrace, "Ожидался символ '}' в конце инициализации.");

            // 19. ; - необязательный
            if (!IsAtEnd() && Current().Type == TokenType.Semicolon)
            {
                _position++;
            }
        }

        /// <summary>
        /// Пропустить всё до конца
        /// </summary>
        private void SkipToEnd()
        {
            _position = _tokens.Count;
        }

        /// <summary>
        /// Пропустить до ';' или конца
        /// </summary>
        private void SkipToSemicolonOrEnd()
        {
            while (!IsAtEnd() && Current().Type != TokenType.Semicolon)
            {
                _position++;
            }

            // Пропускаем ';' если он есть
            if (!IsAtEnd() && Current().Type == TokenType.Semicolon)
            {
                _position++;
            }
        }

        private bool ExpectWord(string expectedWord, string errorMessage)
        {
            if (IsAtEnd())
            {
                AddError(CurrentOrLast(), errorMessage);
                return false;
            }

            Token current = Current();

            if (current.Lexeme == expectedWord)
            {
                _position++;
                return true;
            }

            AddError(current, errorMessage);
            return false;
        }

        private bool ExpectSymbol(TokenType expectedType, string errorMessage)
        {
            if (IsAtEnd())
            {
                AddError(CurrentOrLast(), errorMessage);
                return false;
            }

            if (Current().Type == expectedType)
            {
                _position++;
                return true;
            }

            AddError(Current(), errorMessage);
            return false;
        }

        private bool ExpectIdentifier()
        {
            if (IsAtEnd())
            {
                AddError(CurrentOrLast(), "Ожидался идентификатор словаря.");
                return false;
            }

            Token current = Current();

            // Идентификатор: тип Identifier и НЕ "Dictionary"
            if (current.Type == TokenType.Identifier && current.Lexeme != "Dictionary")
            {
                _position++;
                return true;
            }

            AddError(current, "Ожидался идентификатор словаря.");
            return false;
        }

        private void ParseDictionaryElementList()
        {
            if (IsAtEnd() || Current().Type != TokenType.OpenBrace)
            {
                return;
            }

            ParseDictionaryElement();

            while (!IsAtEnd() && Current().Type == TokenType.Comma)
            {
                _position++;

                if (IsAtEnd())
                {
                    AddError(CurrentOrLast(), "Незавершённый список элементов после ','.");
                    break;
                }

                if (Current().Type == TokenType.CloseBrace)
                {
                    AddError(Current(), "Лишняя запятая перед '}'.");
                    break;
                }

                ParseDictionaryElement();
            }
        }

        private void ParseDictionaryElement()
        {
            if (!ExpectSymbol(TokenType.OpenBrace, "Ожидался символ '{' в элементе словаря."))
                return;

            if (!ExpectNumber())
                return;

            if (!ExpectSymbol(TokenType.Comma, "Ожидался символ ',' между ключом и значением."))
                return;

            if (!ExpectString())
                return;

            ExpectSymbol(TokenType.CloseBrace, "Ожидался символ '}' в элементе словаря.");
        }

        private bool ExpectNumber()
        {
            if (IsAtEnd())
            {
                AddError(CurrentOrLast(), "Ожидалось целое число.");
                return false;
            }

            if (Current().Type == TokenType.UnsignedInteger)
            {
                _position++;
                return true;
            }

            AddError(Current(), "Ожидалось целое число.");
            return false;
        }

        private bool ExpectString()
        {
            if (IsAtEnd())
            {
                AddError(CurrentOrLast(), "Ожидалась строка в двойных кавычках.");
                return false;
            }

            if (Current().Type == TokenType.StringLiteral)
            {
                _position++;
                return true;
            }

            AddError(Current(), "Ожидалась строка в двойных кавычках.");
            return false;
        }

        private void AddError(Token token, string message)
        {
            _result.Errors.Add(new SyntaxError
            {
                InvalidFragment = token?.Lexeme ?? string.Empty,
                Line = token?.Line ?? 1,
                Column = token?.StartColumn ?? 1,
                Message = message
            });
        }

        private Token Current()
        {
            return _tokens[_position];
        }

        private Token CurrentOrLast()
        {
            if (_tokens.Count == 0)
            {
                return new Token
                {
                    Lexeme = string.Empty,
                    Line = 1,
                    StartColumn = 1
                };
            }

            if (_position >= _tokens.Count)
                return _tokens[_tokens.Count - 1];

            return _tokens[_position];
        }

        private bool IsAtEnd()
        {
            return _position >= _tokens.Count;
        }
    }
}
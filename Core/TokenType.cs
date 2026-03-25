namespace Laba1.Core
{
    public enum TokenType
    {
        KeywordNew,        // 1
        KeywordInt,        // 2
        KeywordString,     // 3
        Identifier,        // 4
        Whitespace,        // 5
        Assign,            // 6
        StringLiteral,     // 7
        UnsignedInteger,   // 8
        LessThan,          // 9
        GreaterThan,       // 10
        Comma,             // 11
        OpenBrace,         // 12
        CloseBrace,        // 13
        Semicolon,         // 14
        Error              // 99
    }
}

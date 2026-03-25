namespace Laba1.Core
{
    public sealed class ResultRow
    {
        public string Code { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string Lexeme { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        
        public string Description { get; set; } = string.Empty;

        public int Line { get; set; }
        public int Column { get; set; }
        public bool IsError { get; set; }
    }
}
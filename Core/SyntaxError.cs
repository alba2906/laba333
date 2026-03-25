namespace Laba1.Core
{
    public sealed class SyntaxError
    {
        public string InvalidFragment { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }

        public string Location => $"строка {Line}, {Column}-{Column}";
    }
}
using System.Collections.Generic;

namespace Laba1.Core
{
    public sealed class SyntaxAnalysisResult
    {
        public List<SyntaxError> Errors { get; } = new();
        public bool HasErrors => Errors.Count > 0;
    }
}
using System.Data.Common;
using System.Windows.Shapes;

namespace ModelLanguageCompiler.Model
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
        public int TableIndex { get; set; }
        public override string ToString() => $"({Index}:{TableIndex})";
    }
}

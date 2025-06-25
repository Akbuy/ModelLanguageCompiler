using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLanguageCompiler.Model
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public override string ToString() => $"[{Type}] '{Value}' at ({Line}:{Column})";
    }
}

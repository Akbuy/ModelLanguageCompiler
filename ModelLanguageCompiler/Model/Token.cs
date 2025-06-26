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

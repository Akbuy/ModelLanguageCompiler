using ModelLanguageCompiler.View;
using ModelLanguageCompiler.ViewModel;
using System.Windows;
using System.Windows.Documents;

namespace ModelLanguageCompiler
{
    public partial class MainWindow : Window
    {
        private List<string> _numbers = new List<string>();
        private List<string> _ids = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Tables_Click(object sender, RoutedEventArgs e)
        {
            TablesWindow tablesWindow = new TablesWindow(Lexer.GetKeywords(), Lexer.GetDelimiters(), _numbers, _ids);
            tablesWindow.ShowDialog();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CodeInputBox.Document.Blocks.Clear();
            ParseOutputTextBox.Clear();
            TokensList.Items.Clear();
            _numbers.Clear();
            _ids.Clear();
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(CodeInputBox.Document.ContentStart, CodeInputBox.Document.ContentEnd);
            string code = range.Text;

            var tokens = Lexer.Tokenize(code, out _numbers, out _ids);
            TokensList.Items.Clear();
            foreach (var token in tokens)
            {
                TokensList.Items.Add(token.ToString());
            }

            try
            {
                Parser parser = new Parser(tokens);
                parser.ParseProgram();
                ParseOutputTextBox.Text = "Синтаксический анализ пройден успешно.";
            }
            catch (Exception ex)
            {
                ParseOutputTextBox.Text = $"Ошибка синтаксического анализа: {ex.Message}";
            }
        }
    }
}

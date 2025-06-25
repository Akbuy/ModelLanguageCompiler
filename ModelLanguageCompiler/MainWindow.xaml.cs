using ModelLanguageCompiler.ViewModel;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ModelLanguageCompiler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(CodeInputBox.Document.ContentStart, CodeInputBox.Document.ContentEnd);
            string code = range.Text;

            Lexer lexer = new Lexer();
            var tokens = lexer.Tokenize(code);
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

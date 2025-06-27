using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModelLanguageCompiler.View
{
    public partial class TablesWindow : Window
    {
        public TablesWindow(List<string> keywords, List<string> delimiters, List<string> numbers, List<string> ids)
        {
            InitializeComponent();

            ListWords.ItemsSource = keywords;
            ListLimiters.ItemsSource = delimiters;
            ListNumbers.ItemsSource = numbers;
            ListIds.ItemsSource = ids;
        }
    }
}

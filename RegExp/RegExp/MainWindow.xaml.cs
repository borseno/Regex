using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegExp
{
    public partial class MainWindow : Window
    {
        private string regularExpression;
        private string inputText;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RegExpValidate(object sender, TextCompositionEventArgs e)
        {

        }

        private void ChangeValue(object sender, TextChangedEventArgs e)
        {

        }
    }
}

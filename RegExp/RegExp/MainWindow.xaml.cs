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
using System.Text.RegularExpressions;

namespace RegExp
{
    public partial class MainWindow : Window
    {
        private string RegExpValue
        {
            get { return InputRegExp.Text; }
            set { InputRegExp.Text = value; }
        }
        private string Text
        {
            get { return InputString.Text; }
            set { InputString.Text = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {     
            UpdateValues();
        }

        private void UpdateValues()
        {
            
        }
    }
}

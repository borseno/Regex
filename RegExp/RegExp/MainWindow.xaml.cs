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
            get { return new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text; }
            set {
                InputString.Document.Blocks.Clear();
                InputString.Document.Blocks.Add(new Paragraph(new Run(value)));
            }
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
            Regex regex = null;
            try
            {
                regex = new Regex(RegExpValue);
            }
            catch (ArgumentException e)
            {
                // TODO:
                // tell the user that regex pattern is wrong
                return;
            }

            if (regex.IsMatch(Text))
            {
                foreach (var i in regex.Matches(Text))
                {
                    // TODO:
                    // highlight the matched substrings
                }
            }
        }
    }
}

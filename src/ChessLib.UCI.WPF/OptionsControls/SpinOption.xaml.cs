using ChessLib.UCI.Commands.FromEngine.Options;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessLib.UCI.WPF.OptionsControls
{
    /// <summary>
    /// Interaction logic for SpinOption.xaml
    /// </summary>
    public partial class SpinOption : UserControl
    {
        public SpinOption()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var context = (UCISpinOption)DataContext;
            if (double.TryParse(e.Text, out double num))
            {
                e.Handled = true;
                if (context.Max.HasValue)
                {
                    e.Handled = num <= context.Max;
                }
                if (context.Min.HasValue)
                {
                    e.Handled = num >= context.Min;
                }
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}

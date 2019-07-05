using ChessLib.UCI.Commands.FromEngine.Options;
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

namespace ChessLib.UCI.WPF.OptionsControls
{
    /// <summary>
    /// Interaction logic for ButtonOption.xaml
    /// </summary>
    public partial class ButtonOption : UserControl
    {
        public ButtonOption()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var context = DataContext as UCIButtonOption;
            context.Value(context.Name);
        }
    }


}

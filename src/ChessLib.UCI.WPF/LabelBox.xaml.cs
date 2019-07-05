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

namespace ChessLib.UCI.WPF
{
    /// <summary>
    /// Interaction logic for LabelBox.xaml
    /// </summary>
    public partial class LabelBox : TextBlock
    {
        public LabelBox()
        {
            InitializeComponent();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelBox), new FrameworkPropertyMetadata(typeof(LabelBox)));
        }
    }
}

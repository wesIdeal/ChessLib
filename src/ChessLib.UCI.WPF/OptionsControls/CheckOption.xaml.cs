﻿using ChessLib.UCI.Commands.FromEngine.Options;
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
    public partial class OptionBase : UserControl
    {
        public OptionBase()
        {

        }
    }
    /// <summary>
    /// Interaction logic for CheckOption.xaml
    /// </summary>
    public partial class CheckOption : UserControl
    {
        public CheckOption()
        {
            InitializeComponent();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckOption), new FrameworkPropertyMetadata(typeof(CheckOption)));

        }

        
    }


}

﻿using System;
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
    /// Interaction logic for ComboOption.xaml
    /// </summary>
    public partial class ComboOption : UserControl
    {
        public ComboOption()
        {
            InitializeComponent();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboOption), new FrameworkPropertyMetadata(typeof(ComboOption)));

        }
    }
}

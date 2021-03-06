﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Xaml;
using StorageBuffer.Domain;

namespace StorageBuffer
{
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow(string message)
        {
            InitializeComponent();
            lMessage.Content = message;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void BtnDeny_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}

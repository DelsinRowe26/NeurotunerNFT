using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxAct.xaml
    /// </summary>
    public partial class MessageBoxAct : Window
    {
        public MessageBoxAct()
        {
            InitializeComponent();
        }

        private void btnSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.neurotuners.ru/");
            Close();
        }
    }
}

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
using System.Windows.Shapes;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для WinSkipTwo.xaml
    /// </summary>
    public partial class WinSkipTwo : Window
    {
        public static int SkipIndex = 0;

        public WinSkipTwo()
        {
            InitializeComponent();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            SkipIndex = -1;
            Close();
        }

        private void btnDo_Click(object sender, RoutedEventArgs e)
        {
            SkipIndex = 1;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(SkipIndex == 0)
            {
                e.Cancel = true;
            }
        }
    }
}

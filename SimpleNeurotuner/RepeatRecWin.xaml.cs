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
    /// Логика взаимодействия для RepeatRecWin.xaml
    /// </summary>
    public partial class RepeatRecWin : Window
    {
        public static int RepRecInd = 0;
        public static int TimeInd = 0;
        public RepeatRecWin()
        {
            InitializeComponent();
        }

        private void btnRec_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTime.SelectedIndex == 0)
            {
                TimeInd = 100;
                RepRecInd = 1;
                Close();
            }
            else if(cmbTime.SelectedIndex == 1)
            {
                TimeInd = 200;
                RepRecInd = 1;
                Close();
            }
            else if (cmbTime.SelectedIndex == 2)
            {
                TimeInd = 300;
                RepRecInd = 1;
                Close();
            }
            else
            {
                MessageBox.Show("Вы не выбрали количество секунд.");
            }
        }

        private void WinRecRep_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cmbTime.SelectedIndex == -1)
            {
                e.Cancel = true;
            }
            else
            {
                if (cmbTime.SelectedIndex == 0)
                {
                    TimeInd = 100;
                    RepRecInd = 1;
                }
                else if (cmbTime.SelectedIndex == 1)
                {
                    TimeInd = 200;
                    RepRecInd = 1;
                }
                else if (cmbTime.SelectedIndex == 2)
                {
                    TimeInd = 300;
                    RepRecInd = 1;
                }
            }
        }
    }
}

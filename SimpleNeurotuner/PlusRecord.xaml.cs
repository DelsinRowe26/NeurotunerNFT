using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для PlusRecord.xaml
    /// </summary>
    public partial class PlusRecord : Window
    {
        private int clickRec = 0;
        public PlusRecord()
        {
            InitializeComponent();
        }

        private void WinRec_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string index = FileLanguage.ReadToEnd();
                FileLanguage.Close();
                if (index == "0")
                {
                    btnYes.Content = "Да";
                    btnNo.Content = "Нет";
                    tbMakeRec.Text = "Хотите сделать еще одну запись?";
                }
                else
                {
                    btnYes.Content = "Yes";
                    btnNo.Content = "No";
                    tbMakeRec.Text = "Want to make another record?";
                }
            }
            catch (Exception ex)
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string index = FileLanguage.ReadToEnd();
                FileLanguage.Close();
                if (index == "0")
                {
                    string msg = "Ошибка в WinRec_Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in WinRec_Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            clickRec = 1;
            this.Close();
        }

        private void WinRec_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (clickRec == 1)
            {
                CreateWindow create = new CreateWindow();
                create.Show();
            }
            else
            {
                CreateWindow.RecIndex = 1;
            }
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            clickRec = 0;
            this.Close();
        }
    }
}

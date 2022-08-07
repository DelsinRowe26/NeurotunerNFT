using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TEST_API;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для ActivationForm.xaml
    /// </summary>
    public partial class ActivationForm : Window
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path2;
        string langindex;
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void btnActivation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (check.act(tbMail.Text, tbKey.Text, "reself"))
                {
                    case '0':
                    case '1':
                        if (langindex == "0")
                        {
                            MessageBox.Show("активация успешна!\nПерезапустите программу");
                        }
                        else
                        {
                            MessageBox.Show("activation successful!\nRestart the program");
                        }
                        File.WriteAllText(path, tbKey.Text + "\n" + tbMail.Text);
                        //Application.Exit();
                        break;
                    case '2':
                        if (langindex == "0")
                        {
                            MessageBox.Show("ключ устарел или неверный email");
                        }
                        else
                        {
                            MessageBox.Show("key is outdated or invalid email");
                        }
                        break;
                    case '3':
                        if (langindex == "0")
                        {
                            MessageBox.Show("неверный ключ");
                        }
                        else
                        {
                            MessageBox.Show("invalid key");
                        }

                        break;
                    default:
                        if (langindex == "0")
                        {
                            MessageBox.Show("Что-то пошло не так...");
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong...");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в ActClick: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in ActClick: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                langindex = FileLanguage.ReadToEnd();
                FileLanguage.Close();
                path2 = path + @"\NeurotunerNFT\Data";
                if (langindex == "0")
                {
                    btnActivation.Content = "Активировать";
                    Title = "Форма Активации";
                }
                else
                {
                    btnActivation.Content = "Activate";
                    Title = "ActivationForm";
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в ActLoaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in ActLoaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }
    }
}

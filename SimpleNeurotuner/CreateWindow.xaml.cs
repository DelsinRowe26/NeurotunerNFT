using System;
using System.Diagnostics;
using System.IO;
using System.Windows;


namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public string FileName;
        //string langindex;
        public string cutFileName;
        private FileInfo fileCreate = new FileInfo("Data_Create.tmp");
        private FileInfo fileCutCreate = new FileInfo("Data_cutCreate.tmp");
        static StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
        string langindex = FileLanguage.ReadToEnd();

        public CreateWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //string index = FileLanguage.ReadToEnd();
                //langindex = FileLanguage.ReadLine();
                if (langindex == "0")
                {
                    lbRecordTitle.Content = "Название записи";
                    Title = "Создание файла";
                    btnCreate.Content = "Создать";
                }
                else
                {
                    lbRecordTitle.Content = "Record title";
                    Title = "Create window";
                    btnCreate.Content = "Create";
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в CreateWindow_Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in CreateWindow_Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileName = tbRecordTitle.Text + ".wav";
                cutFileName = "cut" + tbRecordTitle.Text + ".wav";
                
                File.WriteAllText(fileCreate.FullName, FileName);
                File.WriteAllText(fileCutCreate.FullName, cutFileName);
                if (langindex == "0")
                {
                    if (File.Exists(@"Record\" + FileName))
                    {
                        string msg = "Файл с таким именем существует,\nпереименуйте файл.";
                        LogClass.LogWrite(msg);
                        MessageBox.Show(msg);
                    }
                    else
                    {
                        LogClass.LogWrite("Файл создан.");
                        this.Close();
                    }
                }
                else
                {
                    if (File.Exists(@"Record\" + FileName))
                    {
                        string msg = "A file with the same name exists,\nrename the file.";
                        LogClass.LogWrite(msg);
                        MessageBox.Show(msg);
                    }
                    else
                    {
                        LogClass.LogWrite("The file has been created.");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в CreateWindow_btnCreate_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in CreateWindow_btnCreate_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //await File.WriteAllTextAsync(fileCreate.FullName, FileName);
        }
    }
}

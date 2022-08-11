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
        public static int RecIndex = 0;

        public CreateWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                /*FileInfo file = new FileInfo("DataRec.tmp");
                File.WriteAllText(file.FullName, "0");
                file.Refresh();*/
                /*FileStream fileStream = new FileStream("DataRec.tmp", FileMode.Truncate);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLineAsync("0");
                streamWriter.Close();
                fileStream.Close();*/
                RecIndex = 0;
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string langindex = FileLanguage.ReadToEnd();
                FileLanguage.Close();
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
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string langindex = FileLanguage.ReadToEnd();
                FileLanguage.Close();
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
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string langindex = FileLanguage.ReadToEnd();
                FileLanguage.Close();
                FileName = tbRecordTitle.Text + ".wav";
                cutFileName = "cut" + tbRecordTitle.Text + ".wav";
                
                File.WriteAllText(fileCreate.FullName, FileName);
                File.WriteAllText(fileCutCreate.FullName, cutFileName);
                fileCreate.Refresh();
                fileCutCreate.Refresh();
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
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                string langindex = FileLanguage.ReadToEnd();
                FileLanguage.Close();
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

        private void tbRecordTitle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                btnCreate_Click(sender, e);
            }
        }
    }
}

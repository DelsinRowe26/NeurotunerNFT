using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для SaveDeleteWindow.xaml
    /// </summary>
    public partial class SaveDeleteWindow : Window
    {
        int clickSD;
        string index;
        public SaveDeleteWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                index = FileLanguage.ReadToEnd();
                if (index == "0")
                {
                    Title = "Окно сохранения/удаления";
                    lbSaveDelete.Content = "Сохранить вашу запись или удалить и перезаписать?";
                    btnSave.Content = "Сохранить";
                    btnDelete.Content = "Удалить";
                    LogClass.LogWrite("Открыто окно сохранения.");
                }
                else
                {
                    Title = "Save/Delete Window";
                    lbSaveDelete.Content = "Keep your recording or delete and overwrite?";
                    btnSave.Content = "Save";
                    btnDelete.Content = "Delete";
                    LogClass.LogWrite("The save window is open.");
                }
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clickSD = 1;
                //CreateWindow.RecIndex = 1;
                /*FileStream fileStream = new FileStream("DataRec.tmp", FileMode.Truncate);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLineAsync("1");
                streamWriter.Close();
                fileStream.Close();*/
                /*StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                string FileCut = FileCutRecord.ReadToEnd();
                string myfile = FileRecord.ReadToEnd();
                File.Move(myfile, @"Record\" + myfile);
                File.Delete(FileCut);*/
                this.Close();
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в btnSave_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnSave_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clickSD = 2;
                /*StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                string myfile = FileRecord.ReadToEnd();
                string cutmyfile = FileCutRecord.ReadToEnd();
                File.Delete(myfile);
                File.Delete(cutmyfile);*/
                this.Close();
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в btnDelete_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnDelete_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (clickSD == 1)
                {
                    //CreateWindow.RecIndex = 1;
                    PlusRecord record = new PlusRecord();
                    record.Show();
                    StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                    index = FileLanguage.ReadToEnd();
                    StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                    StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                    string FileCut = FileCutRecord.ReadToEnd();
                    string myfile = FileRecord.ReadToEnd();
                    File.Move(myfile, @"Record\" + myfile);
                    File.Delete(FileCut);
                    if (index == "0")
                    {
                        LogClass.LogWrite("Запись сохранена.");
                    }
                    else
                    {
                        LogClass.LogWrite("The entry has been saved.");
                    }
                }
                else if(clickSD == 2)
                {
                    StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                    index = FileLanguage.ReadToEnd();
                    StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                    StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                    string myfile = FileRecord.ReadToEnd();
                    string cutmyfile = FileCutRecord.ReadToEnd();
                    File.Delete(myfile);
                    File.Delete(cutmyfile);
                    if (index == "0")
                    {
                        LogClass.LogWrite("Запись удалена.");
                    }
                    else
                    {
                        LogClass.LogWrite("The entry has been deleted.");
                    }
                }
                else
                {
                    StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                    index = FileLanguage.ReadToEnd();
                    if (index == "0")
                    {
                        string msg = "Если выскочило данное окошко,\nто вы нажали на крестик,\n и файл с записью удалится.";
                        LogClass.LogWrite(msg);
                        MessageBox.Show(msg);
                    }
                    else
                    {
                        string msg = "If this window popped up,\nthen you clicked on the cross,\n and the file with the record will be deleted.";
                        LogClass.LogWrite(msg);
                        MessageBox.Show(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в Closing: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Closing: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }
    }
}

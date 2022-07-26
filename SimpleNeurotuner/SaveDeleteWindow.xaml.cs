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
                }
                else
                {
                    Title = "Save/Delete Window";
                    lbSaveDelete.Content = "Keep your recording or delete and overwrite?";
                    btnSave.Content = "Save";
                    btnDelete.Content = "Delete";
                }
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в Loaded: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Loaded: \r\n" + ex.Message;
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
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnSave_Click: \r\n" + ex.Message;
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
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnDelete_Click: \r\n" + ex.Message;
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
                    StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                    StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                    string FileCut = FileCutRecord.ReadToEnd();
                    string myfile = FileRecord.ReadToEnd();
                    File.Move(myfile, @"Record\" + myfile);
                    File.Delete(FileCut);
                }
                else if(clickSD == 2)
                {
                    StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                    StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                    string myfile = FileRecord.ReadToEnd();
                    string cutmyfile = FileCutRecord.ReadToEnd();
                    File.Delete(myfile);
                    File.Delete(cutmyfile);
                }
                else
                {
                    StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                    index = FileLanguage.ReadToEnd();
                    if (index == "0")
                    {
                        string msg = "Если выскочило данное окошко,\nто вы нажали на крестик,\n и файл с записью удалится.";
                        MessageBox.Show(msg);
                    }
                    else
                    {
                        string msg = "If this window popped up,\nthen you clicked on the cross,\n and the file with the record will be deleted.";
                        MessageBox.Show(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в Closing: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Closing: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }
    }
}

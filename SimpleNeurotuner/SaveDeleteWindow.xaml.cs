using System.IO;
using System.Windows;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для SaveDeleteWindow.xaml
    /// </summary>
    public partial class SaveDeleteWindow : Window
    {
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
            catch
            {

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
            StreamReader FileRecord = new StreamReader("Data_Create.tmp");
            string FileCut = FileCutRecord.ReadToEnd();
            string myfile = FileRecord.ReadToEnd();
            File.Move(myfile, @"Record\" + myfile);
            File.Delete(FileCut);
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            StreamReader FileRecord = new StreamReader("Data_Create.tmp");
            StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
            string myfile = FileRecord.ReadToEnd();
            string cutmyfile = FileCutRecord.ReadToEnd();
            File.Delete(myfile);
            File.Delete(cutmyfile);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*StreamReader FileRecord = new StreamReader("Data_Create.tmp");
            StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
            string myfile = FileRecord.ReadToEnd();
            string cutmyfile = FileCutRecord.ReadToEnd();*/
        }
    }
}

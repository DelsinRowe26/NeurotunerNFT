using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //public string index;
        

        public Window1()
        {
            InitializeComponent();
        }

        private void Help_Loaded(object sender, RoutedEventArgs e)
        {
            StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
            string index = FileLanguage.ReadToEnd();
            try
            {
                if (index == "0")
                {
                    Title = "Помощь";
                    tbHelp.Text = "Добро пожаловать в Нейротюнер.\n\nИнструкция пользователя:\n1.  Подключите (проводную) аудио-гарнитуру к компьютеру.\n2.  Запишите свой голос (нажать кнопку «рекорд» - подсвечивается).\nКак только начнется запись – держите звук «А» 3 секунды.\nНа экране появится визуальный отпечаток голоса.\n3.  Приступайте к самонастройке. Наденьте гарнитуру.\nНажмите основную кнопку (фиолетовая, слева на панели, подсвечивается).\n4.  100 секунд настройки. Извлекайте продолжительный звук «Ааааа»,\nвнимательно слушая свой голос в наушниках (подержали 10 секунд, затем вдох и снова).\nРот необходимо открыть широко. Старайтесь максимально настроиться на голос.\nЛучше с закрытыми глазами. Чем лучше резонирует ощущение своего голоса и голос в наушниках – тем выше эффект!\n5.  Когда таймер остановился - нажмите снова кнопку «рекорд» (подсвечивается) и запишите свой голос еще раз.\nСравните с изначальным.\nСлева будет исходный, справа – получившийся.\nЭффект держится от 30 минут до нескольких часов.\n\nДостигаемый эффект:\n- синхронизация работы полушарий головного мозга (активизация мышления)\n- постановка голоса (приятный тембр, уверенный)\n- соединение слуха и голоса (легче координировать свою речь)\n- повышение альфа-ритма (естественная расслабленность, снятие тревоги, зажимов)\n- повышение настроения и полезное состояние для эффективных переговоров\n\nПри регулярном использовании – происходит полезный накопительный эффект (триггер состояния и звучания голоса) и качество нейротюнинга – растет!\n\nТехнология совершенно безопасна для слуха и голоса, основана на природных механизмах.\n\nЖелаем приятного использования!";
                }
                else
                {
                    Title = "Help";
                    tbHelp.Text = "Welcome to Neurotuner NFT.\n\nInstruction:\n1. If the microphone and speakers (headphones) \nare not selected by default, then select them yourself.\n2. Select an entry.\n3. Press the start button, and enjoy.You can also use the slider\nto adjust the volume of your voice in the microphone.\nThere was a desire to stop, press the stop button.\n4. If you want to make your own recording, then switch the\nmode from listening to recording mode.\n5. You will have a window in which you can name your entry.\n6. After you name your entry and press the create button,\nyou will go to the main window in which an additional\nbutton will appear.\n7. To make a recording, first start making a sound, and then\npress the start button, then the recording will begin, after\nthe end of the recording, a window will pop up about\nthe completion of the recording.\n8. Then you click on the listen button and listen to your recording.\nAfter you have listened to the recording, press\nthe stop button. A window pops up in which you choose\nto keep your entry, or delete and overwrite.\n\nVersion: 1.1";
                }
            }
            catch (Exception ex)
            {
                if (index == "0")
                {
                    string msg = "Ошибка в Help_Loaded: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Help_Loaded: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void tbHelp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CSCore;
using CSCore.SoundIn;//Вход звука
using CSCore.SoundOut;//Выход звука
using CSCore.CoreAudioAPI;
using CSCore.Streams;

using CSCore.Codecs;
using CSCore.Codecs.WAV;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Diagnostics;
using System.IO;

//using Microsoft.DirectX.DirectSound;
//using Buffer = Microsoft.DirectX.DirectSound.Buffer;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

using System.Windows.Threading;
using CSCore.DSP;
using System.Windows.Shapes;
using System.Globalization;
using Intersoft.Crosslight;
using CSCore.Streams.Effects;

namespace SimpleNeurotuner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint pdwVolume);

        [DllImport("BiblZvuk.dll", CallingConvention = CallingConvention.Cdecl)]
        //unsafe
          static extern int vizualzvuk(string filename, string secfile, int[] Rdat,int ParV);
        
        private FileInfo fileInfo = new FileInfo("window.tmp");
        private FileInfo fileInfo1 = new FileInfo("Data_Load.tmp");
        private FileInfo FileLanguage = new FileInfo("Data_Language.tmp");
        private FileInfo fileinfo = new FileInfo("DataTemp.tmp");
        private SimpleMixer mMixer;
        private int SampleRate;//44100;
        //private Equalizer equalizer;
        private WasapiOut mSoundOut;
        private WasapiCapture mSoundIn;
        private SampleDSP mDsp;
        private SampleDSPRecord mDspRec;
        private SampleDSPTurbo mDspTurbo;
        string[] file1 = File.ReadAllLines("window.tmp");

        string folder = "Record";
        private IWaveSource mSource;
        private MMDeviceCollection mOutputDevices;
        private MMDeviceCollection mInputDevices;
        public double Magn;
        string myfile;
        string cutmyfile;
        public int index = 1;
        string langindex;
        double[] magnRec;
        string FileName, cutFileName;
        //DispatcherTimer timer1 = new DispatcherTimer();
        private System.Windows.Point startPoint;
        //public Image ImgSpectr;
        private System.Windows.Shapes.Rectangle rectangle;
        float Vol = 1;
        //private PitchShifter _pitchShifter;

        private ISampleSource mMp3;
        private string file, filename, RecordName;
        private string record;
        private string[] allfile;
        private int click, audioclick = 0;

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        BackgroundWorker worker;
        
        public MainWindow()
        {
            InitializeComponent();
            //ShowCurrentVolume();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (worker.CancellationPending == true)
                    {
                        //e.Cancel = true;
                        (sender as BackgroundWorker).ReportProgress(100);
                        break;
                        //return;
                    }
                    (sender as BackgroundWorker).ReportProgress(i);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                PBNFT.Value = e.ProgressPercentage;
                if(PBNFT.Value == 100)
                {
                    PBNFT.Visibility = Visibility.Hidden;
                    lbPBNFT.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void select_an_entry()
        {
            try
            {
                if (langindex == "0")
                {
                    string msg = "Выберите запись.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Select a record.";
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в select_an_entry: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in select_an_entry: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void NFT_download()
        {
            if (langindex == "0")
            {
                string msg = "Подождите пока загрузится NFT картинка.";
                MessageBox.Show(msg);
            }
            else
            {
                string msg = "Wait for the NFT image to load.";
                MessageBox.Show(msg);
            }
        }

        private void Mixer()
        {
            try
            {
                mMixer = new SimpleMixer(1, SampleRate) //стерео, 44,1 КГц
                {
                    FillWithZeros = true,
                    DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                };
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void SimpleNeurotuner_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (file1.Length == 0)
                {
                    //File.Create("DataTemp.dat");
                    WelcomeWindow window = new WelcomeWindow();
                    window.Show();
                    File.AppendAllText(fileInfo.FullName, "1");
                }
                //Находит устройства для захвата звука и заполнияет комбобокс
                MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
                mInputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                MMDevice activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                SampleRate = activeDevice.DeviceFormat.SampleRate;
                
                foreach (MMDevice device in mInputDevices)
                {
                    cmbInput.Items.Add(device.FriendlyName);
                    if (device.DeviceID == activeDevice.DeviceID) cmbInput.SelectedIndex = cmbInput.Items.Count - 1;
                }

                //Находит устройства для вывода звука и заполняет комбобокс
                activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                mOutputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
                foreach (MMDevice device in mOutputDevices)
                {
                    cmbOutput.Items.Add(device.FriendlyName);
                    if (device.DeviceID == activeDevice.DeviceID) cmbOutput.SelectedIndex = cmbOutput.Items.Count - 1;
                }

                cmbRecord.Items.Add("Select a record");
                cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                Filling();
                string[] filename = File.ReadAllLines(fileInfo1.FullName);
                if (filename.Length == 1)
                {
                    Languages();
                }
                if (!File.Exists("log.tmp"))
                {
                    File.Create("log.tmp").Close();
                }
                else 
                {
                    if (File.ReadAllLines("log.tmp").Length > 1000)
                    {
                        File.WriteAllText("log.tmp", " ");
                    }
                }
                if (!Directory.Exists("Image"))
                {
                    Directory.CreateDirectory("Image");
                }
                //SampleRate = mSoundIn.WaveFormat.SampleRate;
                TembroClass tembro = new TembroClass();
                tembro.Tembro(48000);
                var wih = new WindowInteropHelper(this);
                var hWnd = wih.Handle;
                /*uint volume;
                waveOutGetVolume(IntPtr.Zero, out volume);
                int vol = (int)((volume >> 16) & 0xffff);*/
                //lbVolValue.Content = volume;
                
                //lbVolGain.Content = 0;
            }
            catch (Exception ex)
            {
                if (langindex == "0")
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

        /*private void ShowCurrentVolume()
        {
            uint volume;
            waveOutGetVolume(IntPtr.Zero, out volume);
            int left = (int)(volume & 0xFFFF);
            int right = (int)((volume >> 16) & 0xFFFF);
        }*/

        private void Filling()
        {
            try
            {
                allfile = Directory.GetFiles(folder);
                foreach (string filename in allfile)
                {
                    //record = filename.Replace(@"Record\", "");
                    record = filename.Remove(0, 7);
                    cmbRecord.Items.Add(record);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Filling: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Filling: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void btnStart_Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbModes.SelectedIndex == 0)
                {
                    Recording();
                    btnStart_Open.IsEnabled = false;
                    if (langindex == "0")
                    {
                        LogClass.LogWrite("Начало записи голоса.");
                    }
                    else
                    {
                        LogClass.LogWrite("Start voice recording.");
                    }
                }
                else
                {
                    if ((filename != "Record\\Select a record") && (filename != "Record\\Выберите запись"))
                    {
                        if (PBNFT.Value == 100)
                        {
                            click = 1;
                            PitchShifter.Kamp = -1;
                            await Task.Run(() => Sound(file));
                            StartFullDuplex();
                            if (langindex == "0")
                            {
                                LogClass.LogWrite("Начало прослушивания записи.");
                            }
                            else
                            {
                                LogClass.LogWrite("Start listening to the recording.");
                            }

                            btnStart_Open.IsEnabled = false;
                            cmbRecord.IsEnabled = false;
                            btnPlayer.IsEnabled = false;
                            btnTurbo.IsEnabled = false;
                        }
                        else
                        {
                            NFT_download();
                        }
                    }
                    else
                    {
                        select_an_entry();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в btnStart: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnStart: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Stop()
        {
            try
            {
                if (mMixer != null)
                {
                    mMixer.Dispose();
                    mMp3.ToWaveSource(32).Loop().ToSampleSource().Dispose();
                    mMixer = null;
                }
                if (mSoundOut != null)
                {
                    mSoundOut.Stop();
                    mSoundOut.Dispose();
                    mSoundOut = null;
                }
                if (mSoundIn != null)
                {
                    mSoundIn.Stop();
                    mSoundIn.Dispose();
                    mSoundIn = null;
                }
                if (mSource != null)
                {
                    mSource.Dispose();
                    mSource = null;
                }
                if (mMp3 != null)
                {
                    if (mDspRec != null)
                    {
                        mDspRec.Dispose();
                    }
                    mMp3.Dispose();
                    mMp3 = null;
                }
            }
            catch (Exception ex)
            {
                /*if (langindex == "0")
                {
                    string msg = "Ошибка в Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }*/
            }
        }

        private void StopOut()
        {
            try
            {
                mSoundOut.Stop();
                /*if (mSoundOut != null)
                {
                    mSoundOut.Stop();
                    //mSoundOut.Dispose();
                    //mSoundOut = null;
                }*/
            }
            catch (Exception ex)
            {
                /*if (langindex == "0")
                {
                    string msg = "Ошибка в Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }*/
            }
        }

        private async void StartFullDuplex()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };               

                //Init DSP для смещения высоты тона
                mDsp = new SampleDSP(source.ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/.ToMono());

                //SetPitchShiftValue();
                mSoundIn.Start();

                //Инициальный микшер
                //Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                //await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private async void StartFullDuplexTurbo()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                //Init DSP для смещения высоты тона
                mDspTurbo = new SampleDSPTurbo(source.ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/.ToMono());
                SetPitchShiftValue();

                //SetPitchShiftValue();
                mSoundIn.Start();

                //Инициальный микшер
                Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDspTurbo.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.

                await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplexTurbo: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplexTurbo: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private void SoundOut()
        {
            try
            {
                mSoundOut = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);
                
                //mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(32));

                //mSoundOut.Initialize(mSource);
                mSoundOut.Play();

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void SetPitchShiftValue()
        {
            mDspTurbo.PitchShift = (float)Math.Pow(2.0F, 0 / 13.0F);
        }

        private async void Sound(string file)
        {
            try
            {
                //Stop();
                if (click != 0)
                {
                    Mixer();
                    mMp3 = CodecFactory.Instance.GetCodec(filename).ToMono().ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/;
                    mDspRec = new SampleDSPRecord(mMp3.ToWaveSource(32).ToSampleSource());
                    mMixer.AddSource(mDspRec.ChangeSampleRate(mMixer.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                    await Task.Run(() => SoundOut());
                    
                }
                else
                {
                    Stop();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stop();
                btnStart_Open.IsEnabled = true;
                btnPlayer.IsEnabled = true;
                btnTurbo.IsEnabled = true;
                cmbRecord.IsEnabled = true;
                btnRecord.IsEnabled = true;
                click = 0;
                audioclick = 0;
                if (langindex == "0")
                {
                    LogClass.LogWrite("Остановка записи.");
                }
                else
                {
                    LogClass.LogWrite("Stop recording.");
                }
                if (cmbModes.SelectedIndex == 0)
                {
                    if (audioclick == 0)
                    {
                        /*int[] Rdat = new int[5000];
                        int Ndt;
                        Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 2);*/
                        SaveDeleteWindow saveDelete = new SaveDeleteWindow();
                        saveDelete.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в btnStop: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnStop: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void SimpleNeurotuner_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Stop();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SimpleNeurotuner_Closing: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SimpleNeurotuner_Closing: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnStart_Open_MouseMove(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            btnStart_Open.Style = style;
        }

        private void btnTurbo_MouseMove(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            btnTurbo.Style = style;
        }

        private void btnPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            btnPlayer.Style = style;
        }

        private void btnStart_Open_MouseLeave(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            btnStart_Open.Style = style;
        }

        private void btnTurbo_MouseLeave(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            btnTurbo.Style = style;
        }

        private void btnPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            btnPlayer.Style = style;
        }

        private void btnStop_MouseMove(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            btnStop.Style = style;
        }

        private void btnStop_MouseLeave(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            btnStop.Style = style;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window1 window1 = new Window1();
                window1.Show();
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в MenuItem_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in MenuItem_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnRecord_MouseMove(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            btnRecord.Style = style;
        }

        private void btnRecord_MouseLeave(object sender, MouseEventArgs e)
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new SolidColorBrush(Colors.Blue) });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            btnRecord.Style = style;
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //audioclick = 1;
                //mDsp.PitchShift = 0;
                btnRecord.IsEnabled = false;
                click = 1;
                if(langindex == "0")
                {
                    LogClass.LogWrite("Прослушивание собственной записи.");
                }
                else
                {
                    LogClass.LogWrite("Listening to your own recording.");
                }
                Audition();
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в btnRecord_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in btnRecord_Click: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Recording()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                myfile = FileRecord.ReadToEnd();
                cutmyfile = FileCutRecord.ReadToEnd();
                using (mSoundIn = new WasapiCapture())
                {
                    mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                    mSoundIn.Initialize();
                    lbRecordPB.Visibility = Visibility.Visible; 
                    mSoundIn.Start();
                    using (WaveWriter record = new WaveWriter(cutmyfile, mSoundIn.WaveFormat))
                    {
                        mSoundIn.DataAvailable += (s, data) => record.Write(data.Data, data.Offset, data.ByteCount);
                        for(int i = 0; i < 100; i++)
                        {
                            pbRecord.Value++;
                            await Task.Delay(50);
                        }
                        //Thread.Sleep(5000);
                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                    }
                    Thread.Sleep(100);
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    //File.Move(myfile, @"Record\" + myfile);
                    //CutRecord cutRecord = new CutRecord();
                    //cutRecord.CutFromWave(cutmyfile, myfile, start, end);

                }
                if (langindex == "0")
                {
                    string msg = "Запись и обработка завершена.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Recording and processing completed.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Recording: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Recording: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        /*private void Recordind2()
        {
            //float[] buffer = new float[4096];
            mSoundIn = new WasapiCapture();
            mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
            mSoundIn.Initialize();

            var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

            mDsp = new SampleDSP(source.ToSampleSource().ToMono());
            
            mSoundIn.Start();

            //Инициальный микшер
            Mixer();

            //Добавляем наш источник звука в микшер
            mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

            SoundOut();
        }*/

        private void Languages()
        {
            try
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                File.WriteAllText("Data_Load.tmp", "1");
                File.WriteAllText("DataTemp.tmp", "0");
                langindex = FileLanguage.ReadToEnd();
                if (langindex == "0")
                {
                    index = 0;
                    cmbRecord.Items.Clear();
                    cmbRecord.Items.Add("Выберите запись");
                    cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                    Filling();
                    cmbModes.Items.Clear();
                    cmbModes.Items.Add("Записи");
                    cmbModes.Items.Add("Прослушивание");
                    cmbModes.SelectedIndex = cmbModes.Items.Count - 1;
                    Title = "Нейротюнер NFT";
                    btnStart_Open.Content = "Старт";
                    btnTurbo.Content = "Турбо";
                    btnPlayer.Content = "Плеер";
                    btnStop.Content = "Стоп";
                    Help.Header = "Помощь";
                    TabNFT.Header = "gNeuro NFT";
                    TabSettings.Header = "Настройки";
                    lbVersion.Content = "Версия: 1.1";
                    btnRecord.Content = "Прослушать";
                    cmbInput.ToolTip = "Микрофон";
                    cmbOutput.ToolTip = "Наушники";
                    cmbRecord.ToolTip = "Записи";
                    cmbModes.ToolTip = "Режимы";
                    lbPBNFT.Content = "Идёт загрузка NFT...";
                    lbRecordPB.Content = "Идёт запись...";
                }
                else if (langindex != "0")
                {
                    index = 0;
                    cmbRecord.Items.Clear();
                    cmbRecord.Items.Add("Select a record");
                    cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                    Filling();
                    cmbModes.Items.Clear();
                    cmbModes.Items.Add("Record");
                    cmbModes.Items.Add("Audition");
                    cmbModes.SelectedIndex = cmbModes.Items.Count - 1;
                    Title = "Neurotuner NFT";
                    btnStart_Open.Content = "Start";
                    btnTurbo.Content = "Turbo";
                    btnPlayer.Content = "Player";
                    btnStop.Content = "Stop";
                    Help.Header = "Help";
                    TabNFT.Header = "gNeuro NFT";
                    TabSettings.Header = "Settings";
                    lbVersion.Content = "Version: 1.1";
                    btnRecord.Content = "Audition";
                    cmbInput.ToolTip = "Microphone";
                    cmbOutput.ToolTip = "Speaker";
                    cmbRecord.ToolTip = "Record";
                    cmbModes.ToolTip = "Modes";
                    lbPBNFT.Content = "NFT loading in progress...";
                    lbRecordPB.Content = "Recording in progress...";
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Languages: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Languages: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        public static void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        public static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.Width, (int)visual.Height, 99, 98, PixelFormats.Pbgra32);
            Size visualSize = new Size(visual.Width, visual.Height);
            visual.Measure(visualSize);
            visual.Arrange(new Rect(visualSize));
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            bitmap.Render(visual);
            encoder.Frames.Add(frame);

            using(var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        private void SimpleNeurotuner_Activated(object sender, EventArgs e)
        {
            try
            {
                string[] text = File.ReadAllLines("Data_Load.tmp");
                string[] text1 = File.ReadAllLines(fileinfo.FullName);
                //string[] filename = File.ReadAllLines(fileInfo1.FullName);
                if (file1.Length != 1)
                {
                    if (text.Length == 0 && text1.Length == 1)
                    {
                        Languages();
                    }
                    if (langindex == "0")
                    {
                        cmbRecord.Items.Clear();
                        cmbRecord.Items.Add("Выберите запись");
                        cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                        Filling();
                    }
                    else
                    {
                        cmbRecord.Items.Clear();
                        cmbRecord.Items.Add("Select a record");
                        cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                        Filling();
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SimpleNeurotuner_Activated: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SimpleNeurotuner_Activated: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void slPitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            SetPitchShiftValue();
            lbPitchValue.Content = slPitch.Value.ToString("f1");
        }

        private void btnPlayer_Click(object sender, RoutedEventArgs e)
        {
            if ((filename != "Record\\Select a record") && (filename != "Record\\Выберите запись"))
            {
                if (PBNFT.Value == 100)
                {
                    AuditionTurbo();
                    btnStart_Open.IsEnabled = false;
                    btnTurbo.IsEnabled = false;
                    btnPlayer.IsEnabled = false;
                }
                else
                {
                    NFT_download();
                }
            }
            else
            {
                select_an_entry();
            }
        }

        private void btnTurbo_Click(object sender, RoutedEventArgs e)
        {
            StartFullDuplexTurbo();
            btnStart_Open.IsEnabled = false;
            btnPlayer.IsEnabled = false;
            btnTurbo.IsEnabled = false;
        }

        private void cmbModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbModes.SelectedIndex == 0)
                {
                    CreateWindow window = new CreateWindow();
                    window.Show();

                    btnStart_Open.IsEnabled = true;
                    cmbRecord.IsEnabled = false;
                    btnPlayer.Visibility = Visibility.Hidden;
                    btnTurbo.Visibility = Visibility.Hidden;
                    btnRecord.Visibility = Visibility.Visible;
                    pbRecord.Visibility = Visibility.Visible;
                    pbRecord.Value = 0;
                    if(langindex == "0")
                    {
                        LogClass.LogWrite("Включен режим записи.");
                    }
                    else
                    {
                        LogClass.LogWrite("Recording mode is on.");
                    }
                }
                else if (cmbModes.SelectedIndex != 0)
                {
                    cmbRecord.IsEnabled = true;
                    btnRecord.Visibility = Visibility.Hidden;
                    pbRecord.Visibility = Visibility.Hidden;
                    btnPlayer.Visibility = Visibility.Visible;
                    btnTurbo.Visibility = Visibility.Visible;
                    if (langindex == "0")
                    {
                        cmbRecord.Items.Clear();
                        cmbRecord.Items.Add("Выберите запись");
                        cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                        Filling();
                        LogClass.LogWrite("Включен режим прослушивания.");
                    }
                    else
                    {
                        cmbRecord.Items.Clear();
                        cmbRecord.Items.Add("Select a record");
                        cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                        Filling();
                        LogClass.LogWrite("Listening mode is on.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в cmbModes_SelectionChanged: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in cmbModes_SelectionChanged: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnDecVol_Click(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        private void btnIncVol_Click(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_UP);
            
        }

        private void btnIncInactive_MouseMove(object sender, MouseEventArgs e)
        {
            
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new System.Windows.Media.FontFamily("Verdana") });
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new ImageBrush() });
            style.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = new SolidColorBrush(Colors.White) });
            ControlTemplate control = new ControlTemplate();
            
            //control.Resources.Add(new Setter { Property = Control.BackgroundProperty, Value = new ImageBrush(new ImageSource(@"E:\programs\СНейро\SNero\SimpleNeurotuner\bin\Debug\Neurotuners\button\button - soundup - hover.png")) });
        }

        private void btnIncVol_MouseMove(object sender, MouseEventArgs e)
        {
            BitmapImage bmp = new BitmapImage();
            Uri uri = new Uri(@"E:\programs\СНейро\SNero\SimpleNeurotuner\bin\Debug\Neurotuners\button\button-soundup-hover.png");
            bmp.UriSource = uri;
            Image background = new Image();
            background.Source = bmp;
            //btnIncVol.Template.Resources.Add(new Setter { Property = Control.BackgroundProperty, Value = new ImageBrush(bmp)}, new Setter { Property = Control.BackgroundProperty, Value = new ImageBrush(bmp) });
        }

        private void btnDecVol_MouseMove(object sender, MouseEventArgs e)
        {
            BitmapImage bmp = new BitmapImage();
            Uri uri = new Uri(@"E:\programs\СНейро\SNero\SimpleNeurotuner\bin\Debug\Neurotuners\button\button - sounddown - hover.png");
            bmp.UriSource = uri;
            Image background = new Image();
            background.Source = bmp;
            btnDecVol.Content = background;
        }

        private async void Audition()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                myfile = FileRecord.ReadToEnd();
                //Stop();
                Mixer();
                mMp3 = CodecFactory.Instance.GetCodec(/*@"Record\" + */myfile).ToMono().ToSampleSource();
                mMixer.AddSource(mMp3.ChangeSampleRate(mMixer.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                await Task.Run(() => SoundOut());
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Audition: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Audition: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void AuditionTurbo()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                myfile = FileRecord.ReadToEnd();
                //Stop();
                Mixer();
                mMp3 = CodecFactory.Instance.GetCodec(filename).ToMono().ToSampleSource();
                mMixer.AddSource(mMp3.ChangeSampleRate(mMixer.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                await Task.Run(() => SoundOut());
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Audition: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Audition: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void cmbRecord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbRecord.SelectedItem != null)
                {
                    //unsafe
                    {
                        filename = @"Record\" + cmbRecord.SelectedItem.ToString();
                        
                        if ((filename != "Record\\Select a record") && (filename != "Record\\Выберите запись"))
                        {
                            RecordName = cmbRecord.SelectedItem.ToString();
                            int indexOfChar = RecordName.IndexOf(".");
                            RecordName = RecordName.Substring(0, indexOfChar);
                            if (langindex == "0")
                            {
                                LogClass.LogWrite("Выбрана запись " + filename);
                            }
                            else
                            {
                                LogClass.LogWrite("Record selected " + filename);
                            }
                            PBNFT.Visibility = Visibility.Visible;
                            lbPBNFT.Visibility = Visibility.Visible;
                            int[] Rdat = new int[250000];
                            int Ndt;
                            int Ww, Hw, k, ik, dWw, dHw;
                            worker.RunWorkerAsync();
                            Ndt = await Task.Run(() =>
                            {
                                return vizualzvuk(filename, filename, Rdat, 0);
                            });
                            Hw = (int)Math.Sqrt(Ndt);
                            Ww = (int)((double)(Ndt) / (double)(Hw) + 0.5);
                            dWw = (int)((Image1.Width - (double)Ww) / 2.0) - 5;
                            if (dWw < 0)
                                dWw = 0;
                            dHw = (int)((Image1.Height - (double)Hw) / 2.0) - 5;
                            if (dHw < 0)
                                dHw = 0;
                            WriteableBitmap wb = new WriteableBitmap((int)Image1.Width, (int)Image1.Height, Ww, Hw, PixelFormats.Bgra32, null);

                            // Define the update square (which is as big as the entire image).
                            Int32Rect rect = new Int32Rect(0, 0, (int)Image1.Width, (int)Image1.Height);

                            byte[] pixels = new byte[(int)Image1.Width * (int)Image1.Height * wb.Format.BitsPerPixel / 8];
                            //Random rand = new Random();
                            k = 0;
                            ik = 0;
                            int Wwt = 2, Hwt = 2, it0 = Ww / 2, jt0 = Hw / 2, it = 0, jt = 0;
                            int R = 0, G = 0, B = 0, A = 0;
                            int pixelOffset, poffp = 0, kt = 0;
                            while (k < Ndt)
                            {
                                if (ik % 4 == 0)
                                {
                                    R = Rdat[3 * k];
                                    G = Rdat[3 * k + 1];
                                    B = Rdat[3 * k + 2];
                                    A = 255;
                                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                                    pixels[pixelOffset] = (byte)B;
                                    pixels[pixelOffset + 1] = (byte)G;
                                    pixels[pixelOffset + 2] = (byte)R;
                                    pixels[pixelOffset + 3] = (byte)A;
                                    jt++;
                                    if (jt == Hwt)
                                    {
                                        ik++;
                                    }
                                }
                                else if (ik % 4 == 1)
                                {
                                    R = Rdat[3 * k];
                                    G = Rdat[3 * k + 1];
                                    B = Rdat[3 * k + 2];
                                    A = 255;
                                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                                    pixels[pixelOffset] = (byte)B;
                                    pixels[pixelOffset + 1] = (byte)G;
                                    pixels[pixelOffset + 2] = (byte)R;
                                    pixels[pixelOffset + 3] = (byte)A;
                                    it++;
                                    if (it == Wwt)
                                    {
                                        ik++;
                                    }
                                }
                                else if (ik % 4 == 2)
                                {
                                    R = Rdat[3 * k];
                                    G = Rdat[3 * k + 1];
                                    B = Rdat[3 * k + 2];
                                    A = 255;
                                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                                    pixels[pixelOffset] = (byte)B;
                                    pixels[pixelOffset + 1] = (byte)G;
                                    pixels[pixelOffset + 2] = (byte)R;
                                    pixels[pixelOffset + 3] = (byte)A;
                                    jt--;
                                    if (jt == -1)
                                    {
                                        ik++;
                                        //jt0--;
                                    }
                                }
                                else
                                {
                                    R = Rdat[3 * k];
                                    G = Rdat[3 * k + 1];
                                    B = Rdat[3 * k + 2];
                                    A = 255;
                                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                                    pixels[pixelOffset] = (byte)B;
                                    pixels[pixelOffset + 1] = (byte)G;
                                    pixels[pixelOffset + 2] = (byte)R;
                                    pixels[pixelOffset + 3] = (byte)A;
                                    it--;
                                    if (it == -1)
                                    {
                                        it = 0;
                                        jt = 0;
                                        ik++;
                                        it0--;
                                        jt0--;
                                        Hwt += 2;
                                        Wwt += 2;
                                    }
                                }
                                int stride = ((int)Image1.Width * wb.Format.BitsPerPixel) / 8;
                                wb.WritePixels(rect, pixels, stride, 0);
                                k++;
                            }
                            // Show the bitmap in an Image element.
                            Image1.Source = wb;
                            Image1.UpdateLayout();
                            if (!File.Exists(@"Image\" + RecordName + ".bmp"))
                            {
                                SaveToBmp(GridName, @"Image\" + RecordName + ".bmp");
                                if (langindex == "0")
                                {
                                    string msg = "NFT картинка сохранена.";
                                    LogClass.LogWrite(msg);
                                    MessageBox.Show(msg);
                                }
                                else
                                {
                                    string msg = "NFT picture saved.";
                                    LogClass.LogWrite(msg);
                                    MessageBox.Show(msg);
                                }
                            }
                            worker.CancelAsync();
                            
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в cmbRecord_SelectionChanged: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in cmbRecord_SelectionChanged: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }        
    }
}

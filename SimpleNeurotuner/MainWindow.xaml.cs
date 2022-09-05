using System;
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
using System.Windows.Media.Animation;

using System.Windows.Threading;
using CSCore.DSP;
using System.Windows.Shapes;
using System.Globalization;
using Intersoft.Crosslight;
using CSCore.Streams.Effects;
using TEST_API;

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

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("BiblZvuk.dll", CallingConvention = CallingConvention.Cdecl)]
        //unsafe
        public static extern int vizualzvuk(string filename, string secfile, int[] Rdat,int ParV);


        private FileInfo fileInfo = new FileInfo("window.tmp");
        private FileInfo fileInfo1 = new FileInfo("Data_Load.tmp");
        private FileInfo FileLanguage = new FileInfo("Data_Language.tmp");
        private FileInfo fileinfo = new FileInfo("DataTemp.tmp");
        private SimpleMixer mMixer, mMixerRight;
        private int SampleRate;//44100;
        //private Equalizer equalizer;
        private WasapiOut mSoundOut, mSoundOut1;
        private WasapiCapture mSoundIn, mSoundIn1;
        private SampleDSP mDsp;
        private SampleDSPRecord mDspRec;
        private SampleDSPTurbo mDspTurbo, mDspTurbo1;
        private SampleDSPPitch mDspPitch, mDspDef;
        string[] file1 = File.ReadAllLines("window.tmp");

        string folder = "Record";
        private IWaveSource mSource;
        private MMDeviceCollection mOutputDevices;
        private MMDeviceCollection mInputDevices, mInputDevices1;
        public double Magn;
        string myfile;
        string cutmyfile;
        public int index = 1;
        string langindex, fileDeleteRec1, fileDeleteCutRec1, fileDeleteRec2, fileDeleteCutRec2;
        double[] magnRec;
        string FileName, cutFileName;
        //DispatcherTimer timer1 = new DispatcherTimer();
        private System.Windows.Point startPoint;
        //public Image ImgSpectr;
        private System.Windows.Shapes.Rectangle rectangle;
        float Vol = 1;
        //private PitchShifter _pitchShifter;

        private ISampleSource mMp3;
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2;

        private float reverbVal;
        private float pitchVal;
        private string file, filename, RecordName;
        private string record;
        private string[] allfile;
        private int click, audioclick = 0, clickRec = 0;

        private static int limit = 50;
        private int ImgBtnStartClick = 0, ImgBtnRecordClick = 0, NFTRecordClick = 0, ImgBtnListenClick = 0, ImgBtnTurboClick = 0, ModeIndex, BtnSetClick = 0, NFTShadow = 0;

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
        }//hbvjhgvdv

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
                if(PBNFT.Value == 25)
                {
                    string uri = @"Neurotuners\progressbar\Group 13.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if(PBNFT.Value == 50)
                {
                    string uri = @"Neurotuners\progressbar\Group 12.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 75)
                {
                    string uri = @"Neurotuners\progressbar\Group 11.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 100)
                {
                    PBNFT.Visibility = Visibility.Hidden;
                    lbPBNFT.Visibility = Visibility.Hidden;
                    string uri = @"Neurotuners\element\progressbar-backgrnd.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    /*if (!File.Exists(@"Image\" + RecordName + ".bmp"))
                    {
                        SaveToBmp(Image1, @"Image\" + RecordName + ".bmp");
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
                    }*/
                    //imgPBNFTBack.Visibility = Visibility.Hidden;
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
                    //Right = true,
                    //Left = true,
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

        private void MixerRight()
        {
            try
            {
                mMixerRight = new SimpleMixer(1, SampleRate) //стерео, 44,1 КГц
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

                /*mInputDevices1 = deviceEnum.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                foreach (MMDevice device in mInputDevices1)
                {
                    cmbInput1.Items.Add(device.FriendlyName);
                    if (device.DeviceID == activeDevice.DeviceID) cmbInput1.SelectedIndex = cmbInput1.Items.Count - 1;
                }*/


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
                if (!Directory.Exists("Image"))
                {
                    Directory.CreateDirectory("Image");
                }
                if (!Directory.Exists("Record"))
                {
                    Directory.CreateDirectory("Record");
                }
                if (!Directory.Exists(path + "NeurotunerNFT"))
                {
                    Directory.CreateDirectory(path + @"\NeurotunerNFT");
                    path2 = path + @"\NeurotunerNFT\Data";

                }
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
                
                //SampleRate = mSoundIn.WaveFormat.SampleRate;
                CreateWindow.RecIndex = 0;
                
                //int volume, vol;
                var wih = new WindowInteropHelper(this);
                var hWnd = wih.Handle;
                

                //SimpleAudioVolume audioVolume = new SimpleAudioVolume(hWnd);
                //float volume;
                //audioVolume.GetMasterVolumeNative(out volume);
                //int vol = (int)volume;

                /*if (check.strt(path2) > limit)
                {
                    this.IsEnabled = false;
                    ActivationForm activation = new ActivationForm();
                    activation.Show();
                }*/
                //GetWaveVolume(hWnd,out volume);
                //vol = volume;
                //ShowCurrentVolume();
                ModeIndex = 1;
                Modes();
                if (langindex == "0")
                {
                    string msg = "Подключите проводную аудио-гарнитуру к компьютеру.\nЕсли на данный момент гарнитура не подключена,\nто подключите проводную гарнитуру, и перезапустите программу для того, чтобы звук подавался в наушники.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Connect a wired audio headset to your computer.\nIf a headset is not currently connected,\nthen connect a wired headset and restart the program so that the sound is played through the headphones.";
                    MessageBox.Show(msg);
                }
                btnRecordShadow.Opacity = 1;
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

        public static void Rec()
        {
            FileStream fileStream = new FileStream("DataRec.tmp", FileMode.Append);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLineAsync("0");
            streamWriter.Close();
            fileStream.Close();
        }

        private void ShowCurrentVolume()
        {
            
            uint volume;
            waveOutGetVolume(IntPtr.Zero, out volume);
            int right = (int)((volume >> 16) & 0xFFFF);
            int left = (int)(volume & 0xFFFF);
            slLeft.Value = left;
            slRight.Value = right;
            
        }

        private void SetVolume()
        {
            uint volume = (uint)(slLeft.Value + ((int)slRight.Value << 16));
            waveOutSetVolume(IntPtr.Zero, volume);
        }
        
        private void Autobalance()
        {
            int volume = (int)(slLeft.Value + slRight.Value) / 2;
            slLeft.Value = volume;
            slRight.Value = volume;
            SetVolume();
        }

        private void Filling()
        {
            try
            {
                allfile = Directory.GetFiles(folder, "*.wav");
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
                if ((filename != "Record\\Select a record") && (filename != "Record\\Выберите запись"))
                {
                    if (PBNFT.Value == 100)
                    {
                        click = 1;
                        PitchShifter.Kamp = -1;
                        ImgBtnStartClick = 1;
                        string uri = @"Neurotuners\button\button-play-active.png";
                        ImgStart.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                        await Task.Run(() => Sound(file));
                        StartFullDuplex();
                        //SFD();
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
                        slReverb.IsEnabled = false;
                        btnStop.IsEnabled = true;
                        btnModeRecord.IsEnabled = false;
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

        private void StopImg()
        {
            ImgBtnStartClick = 0;
            ImgBtnListenClick = 0;
            ImgBtnTurboClick = 0;
            string uri = @"Neurotuners\button\button-play-inactive.png";
            Dispatcher.Invoke(() => ImgStart.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource);
            string uri1 = @"Neurotuners\button\button-listen-inactive.png";
            Dispatcher.Invoke(() => ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource);
            string uri2 = @"Neurotuners\button\button-turbo-inactive.png";
            Dispatcher.Invoke(() => ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource);
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

        private void SoundIn()
        {
            mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
            Dispatcher.Invoke(() => mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex]);
            mSoundIn.Initialize();
            mSoundIn.Start();
        }

        private async void SFD()
        {
            await Task.Run(() => SoundIn());
            var source = new SoundInSource(mSoundIn) { FillWithZeros = true };
            mDspTurbo = new SampleDSPTurbo(source.ToSampleSource().ToMono());
            //SetPitchShiftValue();
            Mixer();

            //Добавляем наш источник звука в микшер
            mMixer.AddSource(mDspTurbo.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

            //Запускает устройство воспроизведения звука с задержкой 1 мс.

            await Task.Run(() => SoundOut());
        }

        private async void StartFullDuplex()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                //await Task.Run(() => SoundIn());
                SoundIn();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };               

                //Init DSP для смещения высоты тона
                mDsp = new SampleDSP(source.ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/.ToMono());

                //SetPitchShiftValue();

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
                //await Task.Run(() => SoundIn());
                SoundIn();

                    var source = new SoundInSource(mSoundIn) { FillWithZeros = true };
                    var xsource = source.ToSampleSource();

                    var reverb = new DmoWavesReverbEffect(xsource.ToWaveSource());
                    reverb.ReverbTime = reverbVal;
                    reverb.HighFrequencyRTRatio = ((float)1) / 1000;
                    xsource = reverb.ToSampleSource();

                    //Init DSP для смещения высоты тона
                    mDspTurbo = new SampleDSPTurbo(xsource.ToMono());
                    SetPitchShiftValue();

                //SetPitchShiftValue();
                

                //Инициальный микшер
                Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDspTurbo.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.

                await Task.Run(() => SoundOut());
                //SoundOut();

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

        private void SoundOut1()
        {
            try
            {
                mSoundOut1 = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);

                //mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut1.Initialize(mMixer.ToWaveSource(32));

                //mSoundOut.Initialize(mSource);
                mSoundOut1.Play();
                mSoundOut1.Volume = 5;

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundOut1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SoundOut1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private ChannelMask SoundOut()
        {
            try
            {

                mSoundOut = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);
                Dispatcher.Invoke(() => mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex]);
                //mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];

                

                mSoundOut.Initialize(mMixer.ToWaveSource(32).ToMono());
                

                mSoundOut.Play();
                mSoundOut.Volume = 10;
                return ChannelMask.SpeakerFrontLeft;
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                    return ChannelMask.SpeakerFrontLeft;
                }
                else
                {
                    string msg = "Error in SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                    return ChannelMask.SpeakerFrontLeft;
                }
            }
        }

        private void SetPitchShiftValue()
        {
            mDspTurbo.PitchShift = (float)Math.Pow(2.0F, pitchVal / 13.0F);
        }

        private async void Sound(string file)
        {
            try
            {
                //Stop();
                if (click != 0)
                {
                    Mixer();
                    mMp3 = CodecFactory.Instance.GetCodec(filename).ToMono().ToSampleSource();
                    mDspRec = new SampleDSPRecord(mMp3.ToWaveSource(32).ToSampleSource());
                    //SampleRate = mDspRec.WaveFormat.SampleRate;
                    mMixer.AddSource(mDspRec.ChangeSampleRate(mDspRec.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
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

        private void StopMinus1()
        {
            string uri = @"Neurotuners\button\button-stop-active.png";
            ImgStopBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            btnStart_Open.IsEnabled = true;
            btnPlayer.IsEnabled = true;
            btnTurbo.IsEnabled = true;
            cmbRecord.IsEnabled = true;
            btnStop.IsEnabled = false;
            //btnRecord.IsEnabled = true;
            slPitchShift.Value = 0;
            lbValuePitch.Content = 0;
            slReverb.IsEnabled = true;
            slPitchShift.IsEnabled = false;
            btnModeRecord.IsEnabled = true;
            btnRecording.IsEnabled = false;
        }

        private void Stop0()
        {
            btnRecording.IsEnabled = true;
            btnStop.IsEnabled = false;
            btnModeAudio.IsEnabled = true;
            btnStopEffect.Opacity = 0;
            SaveDeleteWindow saveDelete = new SaveDeleteWindow();
            saveDelete.Show();
        }

        private void Stop1()
        {
            audioclick = 0;
            if (audioclick == 0)
            {
                Stop();
                StopImg();
                Dispatcher.Invoke(() => btnTurboShadow.Opacity = 0);
                Dispatcher.Invoke(() => btnRecordShadow.Opacity = 1);
                Dispatcher.Invoke(() => btnRecording.IsEnabled = true);
                Dispatcher.Invoke(() => btnStop.IsEnabled = false);
                Dispatcher.Invoke(() => btnTurbo.IsEnabled = false);
            }
            
        }

        private void Stop12()
        {
            audioclick = 0;
            if (audioclick == 0)
            {
                Stop();
                StopImg();
                Dispatcher.Invoke(() => btnTurboShadow.Opacity = 1);
                Dispatcher.Invoke(() => btnRecordShadow.Opacity = 0);
                Dispatcher.Invoke(() => btnRecording.IsEnabled = false);
                Dispatcher.Invoke(() => btnStop.IsEnabled = false);
                Dispatcher.Invoke(() => btnTurbo.IsEnabled = true);
            }

        }

        private void Stop11()
        {
            audioclick = 0;
            if (audioclick == 0)
            {
                Stop();
                StopImg();
                Dispatcher.Invoke(() => btnTurboShadow.Opacity = 1);
                Dispatcher.Invoke(() => btnRecordShadow.Opacity = 0);
                Dispatcher.Invoke(() => btnRecording.IsEnabled = false);
                Dispatcher.Invoke(() => btnStop.IsEnabled = false);
                Dispatcher.Invoke(() => btnTurbo.IsEnabled = true);
            }

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModeIndex == -1)
                {
                    Stop();
                    StopImg();
                    StopMinus1();
                }
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
                if (ModeIndex == 0)
                {
                    if (audioclick == 0)
                    {
                        /*int[] Rdat = new int[5000];
                        int Ndt;
                        Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 2);*/
                        Stop();
                        StopImg();
                        Stop0();
                    }
                }
                if (ModeIndex == 1)
                {
                    if (audioclick == 0)
                    {
                        /*int[] Rdat = new int[5000];
                        int Ndt;
                        Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 2);*/
                        
                        Stop12();
                        //btnModeAudio.IsEnabled = true;
                        //btnStopEffect.Opacity = 0;
                        //SaveDeleteWindow saveDelete = new SaveDeleteWindow();
                        //saveDelete.Show();
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
                if (File.Exists(fileDeleteRec1))
                {
                    File.Delete(fileDeleteRec1);
                }
                if (File.Exists(fileDeleteCutRec1))
                {
                    File.Delete(fileDeleteCutRec1);
                }
                if (File.Exists(fileDeleteRec2))
                {
                    File.Delete(fileDeleteRec2);
                }
                if (File.Exists(fileDeleteCutRec2))
                {
                    File.Delete(fileDeleteCutRec2);
                }
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
            string uri = @"Neurotuners\button\button-play-hover.png";
            ImgStart.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnTurbo_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-turbo-hover.png";
            ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-listen-hover.png";
            ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnStart_Open_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ImgBtnStartClick == 1)
            {
                string uri = @"Neurotuners\button\button-play-active.png";
                ImgStart.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\button-play-inactive.png";
                ImgStart.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnTurbo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ImgBtnTurboClick == 1)
            {
                string uri = @"Neurotuners\button\button-turbo-active.png";
                ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\button-turbo-inactive.png";
                ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            if(ImgBtnListenClick == 1)
            {
                string uri = @"Neurotuners\button\button-listen-active.png";
                ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\button-listen-inactive.png";
                ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnStop_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-stop-hover.png";
            ImgStopBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnStop_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-stop-inactive.png";
            ImgStopBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
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

        private async void Recording()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                myfile = FileRecord.ReadToEnd();
               cutmyfile = FileCutRecord.ReadToEnd();
                //NFTRecordClick = 1;
                //myfile = "MyRecord1.wav";
                //cutmyfile = "cutMyRecord1.wav";
                FileRecord.Close();
                FileCutRecord.Close();
                /*if (File.Exists(myfile))
                {
                    File.Delete(myfile);
                }
                if (File.Exists(cutmyfile))
                {
                    File.Delete(cutmyfile);
                }*/
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
                            await Task.Delay(40);
                            if(pbRecord.Value == 25)
                            {
                                string uri1 = @"Neurotuners\progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"Neurotuners\progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"Neurotuners\progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"Neurotuners\progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);
                        
                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        
                    }
                    Thread.Sleep(100);
                    string uri = @"Neurotuners\element\progressbar-backgrnd1.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    //NFT_drawing1(myfile);
                    //File.Move(myfile, @"Record\" + myfile);
                    //CutRecord cutRecord = new CutRecord();
                    //cutRecord.CutFromWave(cutmyfile, myfile, start, end);

                }
                if (langindex == "0")
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    btnPlayer.IsEnabled = true;
                    btnRecording.IsEnabled = false;
                    string msg = "Запись и обработка завершена.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    btnPlayerEffect.Opacity = 1;
                }
                else
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    btnPlayer.IsEnabled = true;
                    btnRecording.IsEnabled = false;
                    string msg = "Recording and processing completed.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    btnPlayerEffect.Opacity = 1;
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

        private async void Recording1()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                //myfile = FileRecord.ReadToEnd();
                //cutmyfile = FileCutRecord.ReadToEnd();
                NFTRecordClick = 1;
                myfile = "MyRecord1.wav";
                cutmyfile = "cutMyRecord1.wav";
                fileDeleteRec1 = myfile;
                fileDeleteCutRec1 = cutmyfile;
                FileRecord.Close();
                FileCutRecord.Close();
                if (File.Exists(myfile))
                {
                    File.Delete(myfile);
                }
                if (File.Exists(cutmyfile))
                {
                    File.Delete(cutmyfile);
                }
                using (mSoundIn = new WasapiCapture())
                {
                    mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                    mSoundIn.Initialize();
                    lbRecordPB.Visibility = Visibility.Visible;
                    mSoundIn.Start();
                    using (WaveWriter record = new WaveWriter(cutmyfile, mSoundIn.WaveFormat))
                    {
                        mSoundIn.DataAvailable += (s, data) => record.Write(data.Data, data.Offset, data.ByteCount);
                        for (int i = 0; i < 100; i++)
                        {
                            pbRecord.Value++;
                            await Task.Delay(40);
                            if (pbRecord.Value == 25)
                            {
                                string uri1 = @"Neurotuners\progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"Neurotuners\progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"Neurotuners\progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"Neurotuners\progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    Thread.Sleep(100);
                    string uri = @"Neurotuners\element\progressbar-backgrnd1.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing1(myfile);
                    //File.Move(myfile, @"Record\" + myfile);
                    //CutRecord cutRecord = new CutRecord();
                    //cutRecord.CutFromWave(cutmyfile, myfile, start, end);

                }
                if (langindex == "0")
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    //btnPlayer.IsEnabled = true;
                    btnRecordShadow.Opacity = 0;
                    btnRecording.IsEnabled = false;
                    btnTurbo.IsEnabled = true;
                    btnAudition1.IsEnabled = false;
                    btnAudition2.IsEnabled = false;
                    btnTurboShadow.Opacity = 1;
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
                }
                else
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    //btnPlayer.IsEnabled = true;
                    btnRecordShadow.Opacity = 0;
                    btnRecording.IsEnabled = false;
                    btnTurbo.IsEnabled = true;
                    btnAudition1.IsEnabled = false;
                    btnAudition2.IsEnabled = false;
                    btnTurboShadow.Opacity = 1;
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
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

        private async void Recording2()
        {
            try
            {
                StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                //myfile = FileRecord.ReadToEnd();
                //cutmyfile = FileCutRecord.ReadToEnd();
                NFTRecordClick++;
                myfile = "MyRecord2.wav";
                cutmyfile = "cutMyRecord2.wav";
                fileDeleteRec2 = myfile;
                fileDeleteCutRec2 = cutmyfile;
                FileRecord.Close();
                FileCutRecord.Close();
                if (File.Exists(myfile))
                {
                    File.Delete(myfile);
                }
                if (File.Exists(cutmyfile))
                {
                    File.Delete(cutmyfile);
                }
                using (mSoundIn = new WasapiCapture())
                {
                    mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                    mSoundIn.Initialize();
                    lbRecordPB.Visibility = Visibility.Visible;
                    mSoundIn.Start();
                    using (WaveWriter record = new WaveWriter(cutmyfile, mSoundIn.WaveFormat))
                    {
                        mSoundIn.DataAvailable += (s, data) => record.Write(data.Data, data.Offset, data.ByteCount);
                        for (int i = 0; i < 100; i++)
                        {
                            pbRecord.Value++;
                            await Task.Delay(35);
                            if (pbRecord.Value == 25)
                            {
                                string uri1 = @"Neurotuners\progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"Neurotuners\progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"Neurotuners\progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"Neurotuners\progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    Thread.Sleep(100);
                    string uri = @"Neurotuners\element\progressbar-backgrnd1.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing2(myfile);
                    //File.Move(myfile, @"Record\" + myfile);
                    //CutRecord cutRecord = new CutRecord();
                    //cutRecord.CutFromWave(cutmyfile, myfile, start, end);

                }
                if (langindex == "0")
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    btnAudition1.IsEnabled = true;
                    btnAudition2.IsEnabled = true;
                    //btnPlayer.IsEnabled = true;
                    btnRecording.IsEnabled = false;
                    btnRecordShadow.Opacity = 0;
                    btnTurbo.IsEnabled = true;
                    btnTurboShadow.Opacity = 1;
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса. Сейчас вы можете нажав на картинку прослушать свою запись. Либо начать новую сессию нажав на кнопку записи.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
                }
                else
                {
                    ImgBtnRecordClick = 0;
                    string uri = @"Neurotuners\button\button-record-inactive.png";
                    ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    btnAudition1.IsEnabled = true;
                    btnAudition2.IsEnabled = true;
                    //btnPlayer.IsEnabled = true;
                    btnRecording.IsEnabled = false;
                    btnRecordShadow.Opacity = 0;
                    btnTurbo.IsEnabled = true;
                    btnTurboShadow.Opacity = 1;
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear. Now you can click on the picture to listen to your recording. Or start a new session by clicking on the record button.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
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
                    btnRecording.ToolTip = "Запись";
                    cmbModes.Items.Clear();
                    cmbModes.Items.Add("Записи");
                    cmbModes.Items.Add("Прослушивание");
                    rbMan.Content = "Мужчина";
                    rbWoman.Content = "Женщина";
                    cmbModes.SelectedIndex = cmbModes.Items.Count - 1;
                    Title = "Нейротюнер";
                    btnStart_Open.Content = "Старт";
                    btnStart_Open.ToolTip = "Старт";
                    btnTurbo.Content = "Турбо";
                    btnTurbo.ToolTip = "Турбо";
                    btnPlayer.Content = "Плеер";
                    btnPlayer.ToolTip = "Плеер";
                    btnStop.Content = "Стоп";
                    btnStop.ToolTip = "Стоп";
                    btnSettings.ToolTip = "Настройки";
                    btnModeAudio.ToolTip = "Режим прослушивания";
                    btnModeRecord.ToolTip = "Режим записи";
                    btnIncVol.ToolTip = "Громкость +";
                    btnDecVol.ToolTip = "Громкость -";
                    Help.Header = "Помощь";
                    TabNFT.Header = "gNeuro NFT";
                    TabSettings.Header = "Настройки";
                    lbVersion.Content = "Версия: 1.1";
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
                    rbMan.Content = "Man";
                    rbWoman.Content = "Woman";
                    Title = "Neurotuner";
                    btnStart_Open.Content = "Start";
                    btnTurbo.Content = "Turbo";
                    btnTurbo.ToolTip = "Turbo";
                    btnSettings.ToolTip = "Settings";
                    btnModeAudio.ToolTip = "Mode Audition";
                    btnModeRecord.ToolTip = "Mode Record";
                    btnIncVol.ToolTip = "Volume +";
                    btnDecVol.ToolTip = "Volume -";
                    btnPlayer.Content = "Player";
                    btnPlayer.ToolTip = "Player";
                    btnStop.Content = "Stop";
                    btnStop.ToolTip = "Stop";
                    Help.Header = "Help";
                    btnRecording.ToolTip = "Record";
                    btnStart_Open.ToolTip = "Start";
                    TabNFT.Header = "gNeuro NFT";
                    TabSettings.Header = "Settings";
                    lbVersion.Content = "Version: 1.1";
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
                /*StreamReader reader = new StreamReader("DataRec.tmp");
                string readIndex = reader.ReadToEnd();*/
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
                if (CreateWindow.RecIndex == 1)
                {
                    string uri = @"Neurotuners\button\record-mode-inactive.png";
                    ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    //reader.Close();
                    CreateWindow.RecIndex = 0;
                    ModeIndex = -1;
                    Modes();
                }
                if(YesNoWin.btnOKInd == 1)
                {
                    RecAct();
                }
                
                if(RepeatRecWin.RepRecInd == 1)
                {
                    TurboAct();
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

        private void RecAct()
        {
            if (NFTRecordClick == 0)
            {
                /*if (langindex == "0")
                {
                    string msg = "Держите звук 'А' 3 секунды.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Hold the sound 'A' for 3 seconds.";
                    MessageBox.Show(msg);
                }*/
                ImgBtnRecordClick = 1;
                string uri = @"Neurotuners\button\button-record-active.png";
                ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                pbRecord.Visibility = Visibility.Visible;
                Recording1();
                btnRecording.IsEnabled = false;
                btnStart_Open.IsEnabled = false;
                btnModeAudio.IsEnabled = false;
                YesNoWin.btnOKInd = 0;
                if (langindex == "0")
                {
                    LogClass.LogWrite("Начало первой записи голоса.");
                }
                else
                {
                    LogClass.LogWrite("The beginning of the first voice recording.");
                }
            }
            else if (NFTRecordClick != 0)
            {
                /*if (langindex == "0")
                {
                    string msg = "Держите звук 'А' 3 секунды.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Hold the sound 'A' for 3 seconds.";
                    MessageBox.Show(msg);
                }*/
                ImgBtnRecordClick++;
                string uri = @"Neurotuners\button\button-record-active.png";
                ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                pbRecord.Visibility = Visibility.Visible;
                Recording2();
                YesNoWin.btnOKInd = 0;
                //btnRecording.IsEnabled = false;
                //btnStart_Open.IsEnabled = false;
                //btnModeAudio.IsEnabled = false;
                if (langindex == "0")
                {
                    LogClass.LogWrite("Начало второй записи голоса.");
                }
                else
                {
                    LogClass.LogWrite("The beginning of the second voice recording.");
                }
            }
        }

        private async void TurboAct()
        {
            /*if (langindex == "0")
                        {
                            string msg = "Приступим к самонастройке. Наденьте гарнитуру.\n100 секунд настройки. Извлекайте продолжительный и периодический звук «Ааааа»\nвнимательно слушая свой голос в наушниках.\nСтарайтесь максимально настроиться на него. Лучше с закрытыми глазами.\nЧем четче резонирует ощущение своего голоса и голос в наушниках – тем выше эффект!";
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            string msg = "Let's start with self-tuning. Put on your headset.\n100 seconds settings. Make a long and intermittent “Ahhh”\nsound by listening carefully to your voice with headphones.\nTry to tune in to it as much as possible. Better with closed eyes.\nThe more clearly the feeling of your voice and the voice in the headphones resonates, the higher the effect!";
                            MessageBox.Show(msg);
                        }*/

            StartFullDuplexTurbo();

            //PitchTimerMan();
            ImgBtnTurboClick = 1;
            string uri = @"Neurotuners\button\button-turbo-active.png";
            ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            //Thread.Sleep(2000);
            //btnStart_Open.IsEnabled = false;
            //btnPlayer.IsEnabled = false;
            rbMan.IsEnabled = false;
            rbWoman.IsEnabled = false;
            btnTurbo.IsEnabled = false;
            RepeatRecWin.RepRecInd = 0;
            //slPitchShift.IsEnabled = true;
            //slReverb.IsEnabled = false;
            //btnStop.IsEnabled = true;
            //btnModeRecord.IsEnabled = false;
            //cmbRecord.IsEnabled = false;
            lbTimer.Visibility = Visibility.Visible;
            if (reverbVal == 150)
            {
                await Task.Run(() => PitchTimerMan());
            }
            else if (reverbVal == 400)
            {
                await Task.Run(() => PitchTimerWoman());
            }
        }

        private void slPitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            //SetPitchShiftValue();
            //lbPitchValue.Content = slPitch.Value.ToString("f1");
        }

        private void btnPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (ModeIndex == -1)
            {
                if ((filename != "Record\\Select a record") && (filename != "Record\\Выберите запись"))
                {
                    if (PBNFT.Value == 100)
                    {

                        AuditionTurbo();
                        ImgBtnListenClick = 1;
                        string uri = @"Neurotuners\button\button-listen-active.png";
                        ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                        btnStart_Open.IsEnabled = false;
                        btnTurbo.IsEnabled = false;
                        btnPlayer.IsEnabled = false;
                        btnStop.IsEnabled = true;
                        slReverb.IsEnabled = false;
                        cmbRecord.IsEnabled = false;
                        btnModeRecord.IsEnabled = false;
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
            else if(ModeIndex == 0)
            {
                Audition();
                ImgBtnListenClick = 1;
                string uri = @"Neurotuners\button\button-listen-active.png";
                ImgListenBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                btnPlayer.IsEnabled = false;
                btnStop.IsEnabled = true;
                btnPlayerEffect.Opacity = 0;
                btnStopEffect.Opacity = 1;
            }
        }

        private async void btnTurbo_Click(object sender, RoutedEventArgs e)
        {
            if (ModeIndex == -1)
            {
                StartFullDuplexTurbo();
                ImgBtnTurboClick = 1;
                string uri = @"Neurotuners\button\button-turbo-active.png";
                ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                //Thread.Sleep(2000);
                btnStart_Open.IsEnabled = false;
                btnPlayer.IsEnabled = false;
                btnTurbo.IsEnabled = false;
                slPitchShift.IsEnabled = true;
                slReverb.IsEnabled = false;
                btnStop.IsEnabled = true;
                btnModeRecord.IsEnabled = false;
                cmbRecord.IsEnabled = false;
            }
            else if(ModeIndex == 1)
            {

                if (rbMan.IsChecked == true || rbWoman.IsChecked == true)
                {
                    if (RepeatRecWin.RepRecInd == 1)
                    {
                        /*if (langindex == "0")
                        {
                            string msg = "Приступим к самонастройке. Наденьте гарнитуру.\n100 секунд настройки. Извлекайте продолжительный и периодический звук «Ааааа»\nвнимательно слушая свой голос в наушниках.\nСтарайтесь максимально настроиться на него. Лучше с закрытыми глазами.\nЧем четче резонирует ощущение своего голоса и голос в наушниках – тем выше эффект!";
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            string msg = "Let's start with self-tuning. Put on your headset.\n100 seconds settings. Make a long and intermittent “Ahhh”\nsound by listening carefully to your voice with headphones.\nTry to tune in to it as much as possible. Better with closed eyes.\nThe more clearly the feeling of your voice and the voice in the headphones resonates, the higher the effect!";
                            MessageBox.Show(msg);
                        }*/

                        StartFullDuplexTurbo();

                        //PitchTimerMan();
                        ImgBtnTurboClick = 1;
                        string uri = @"Neurotuners\button\button-turbo-active.png";
                        ImgTurboBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                        //Thread.Sleep(2000);
                        //btnStart_Open.IsEnabled = false;
                        //btnPlayer.IsEnabled = false;
                        btnTurbo.IsEnabled = false;
                        //slPitchShift.IsEnabled = true;
                        //slReverb.IsEnabled = false;
                        //btnStop.IsEnabled = true;
                        //btnModeRecord.IsEnabled = false;
                        //cmbRecord.IsEnabled = false;
                        lbTimer.Visibility = Visibility.Visible;
                        if (reverbVal == 150)
                        {
                            await Task.Run(() => PitchTimerMan());
                        }
                        else if (reverbVal == 400)
                        {
                            await Task.Run(() => PitchTimerWoman());
                        }
                    }
                    else
                    {
                        RepeatRecWin recWin = new RepeatRecWin();
                        recWin.Show();
                    }
                }
                else
                {
                    if (langindex == "0")
                    {
                        string msg = "Выберите пол.";
                        MessageBox.Show(msg);
                        LogClass.LogWrite("Режим нейротюнинга.");
                    }
                    else
                    {
                        string msg = "Choose a gender.";
                        MessageBox.Show(msg);
                        LogClass.LogWrite("Neuro tuning mode.");
                    }
                }
            }
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

        private void Modes()
        {
            try
            {
                if (ModeIndex == 0)
                {
                    CreateWindow window = new CreateWindow();
                    window.Show();

                    btnStart_Open.IsEnabled = false;
                    //cmbRecord.IsEnabled = false;
                    //imgShadowNFT.Visibility = Visibility.Hidden;
                    slPitchShift.Visibility = Visibility.Hidden;
                    lbValuePitch.Visibility = Visibility.Hidden;
                    btnModeRecord.IsEnabled = false;
                    btnStop.IsEnabled = false;
                    btnModeAudio.IsEnabled = true;
                    btnRecording.IsEnabled = true;
                    slReverb.Visibility = Visibility.Hidden;
                    lbReverb.Visibility = Visibility.Hidden;
                    cmbRecord.IsEnabled = false;
                    btnPlayer.IsEnabled = false;
                    btnTurbo.IsEnabled = false;
                    pbRecord.Visibility = Visibility.Visible;
                    pbRecord.Value = 0;
                    if (langindex == "0")
                    {
                        LogClass.LogWrite("Включен режим записи.");
                    }
                    else
                    {
                        LogClass.LogWrite("Recording mode is on.");
                    }
                }
                else if (ModeIndex == -1)
                {
                    StreamReader sr = new StreamReader("DataRec.tmp");
                    string read = sr.ReadToEnd();
                    sr.Close();
                    cmbRecord.IsEnabled = true;
                    string uri = @"Neurotuners\button\listen-mode-active.png";
                    ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    btnModeAudio.IsEnabled = false;
                    btnModeRecord.IsEnabled = true;
                    btnRecording.IsEnabled = false;
                    slPitchShift.Visibility = Visibility.Visible;
                    lbValuePitch.Visibility = Visibility.Visible;
                    slReverb.Visibility = Visibility.Visible;
                    lbReverb.Visibility = Visibility.Visible;
                    btnStop.IsEnabled = false;
                    cmbRecord.IsEnabled = true;
                    btnStart_Open.IsEnabled = true;
                    btnPlayer.IsEnabled = true;
                    btnTurbo.IsEnabled = true;
                    //btnRecord.Visibility = Visibility.Hidden;
                    pbRecord.Visibility = Visibility.Hidden;
                    /*btnPlayer.Visibility = Visibility.Visible;
                    btnTurbo.Visibility = Visibility.Visible;*/
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
                else if (ModeIndex == 1)
                {
                    btnModeAudio.Visibility = Visibility.Hidden;
                    btnModeRecord.Visibility = Visibility.Hidden;
                    imgBack.Visibility = Visibility.Hidden;
                    cmbRecord.Visibility = Visibility.Hidden;
                    imgLibRec.Visibility = Visibility.Hidden;
                    //pbRecord.Visibility = Visibility.Visible;
                    pbRecord.Value = 0;
                    btnRecording.IsEnabled = true;
                    btnStart_Open.Visibility = Visibility.Hidden;
                    btnStop.IsEnabled = false;
                    btnPlayer.Visibility = Visibility.Hidden;
                    btnTurbo.IsEnabled = false;
                    btnAudition1.IsEnabled = false;
                    btnAudition2.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Modes: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Modes: \r\n" + ex.Message;
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
            /*pbVolumeRight.Value -= 1310;
            pbVolumeLeft.Value -= 1310;
            SetVolume();*/
            string uri = @"Neurotuners\button\button-sounddown-active.png";
            imgBTNinacDown.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnIncVol_Click(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_UP);
            /*pbVolumeRight.Value += 1310;
            pbVolumeLeft.Value += 1310;
            SetVolume();*/
            string uri = @"Neurotuners\button\button-soundup-active.png";
            imgBTNinaUp.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnRecording_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-record-hover.png";
            ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnRecording_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ImgBtnRecordClick == 1)
            {
                string uri = @"Neurotuners\button\button-record-active.png";
                ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\button-record-inactive.png";
                ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnModeAudio_MouseMove(object sender, MouseEventArgs e)
        {
            if (ModeIndex != -1)
            {
                string uri = @"Neurotuners\button\listen-mode-hover.png";
                ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void chBAuto_Checked(object sender, RoutedEventArgs e)
        {
            Autobalance();
        }

        private void slLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(chBAuto.IsChecked == true)
            {
                //slRight.Value = slLeft.Value;

            }
            //SetVolume();
        }

        private void slRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if(chBAuto.IsChecked == true)
            //{
                //slLeft.Value = slRight.Value;
            //}
            //SetVolume();
        }

        private void slReverb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbReverb.Content = slReverb.Value.ToString("f1");
        }

        private void rbMan_Checked(object sender, RoutedEventArgs e)
        {
            TembroClass tembro = new TembroClass();
            string pathFile = @"Pattern\Wide_voiceMan.tmp";
            tembro.Tembro(SampleRate, pathFile);
            pitchVal = 0;
            reverbVal = 150;
            
        }

        private void PitchTimerMan()
        {
            int timer = RepeatRecWin.TimeInd;
            int i = 0;
            while (timer > 0)
            {
                Dispatcher.Invoke(() => lbTimer.Content = timer.ToString());
                if (i < 50) 
                {
                    pitchVal -= 0.05f;
                    SetPitchShiftValue();
                    i++;
                }
                Thread.Sleep(1000);
                timer--;
            }

            Dispatcher.Invoke(() => lbTimer.Content = timer.ToString());
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Hidden);
            Dispatcher.Invoke(() => rbWoman.IsEnabled = true);
            Dispatcher.Invoke(() => rbMan.IsEnabled = true);
            pitchVal = 0;
            Stop1();
        }

        private void PitchTimerWoman()
        {
            int timer = RepeatRecWin.TimeInd;
            int i = 0;
            while (timer > 0)
            {
                Dispatcher.Invoke(() => lbTimer.Content = timer.ToString());
                if (i < 50)
                {
                    pitchVal -= 0.04f;
                    SetPitchShiftValue();
                    i++;
                }
                Thread.Sleep(1000);
                timer--;
            }
            Dispatcher.Invoke(() => lbTimer.Content = timer.ToString());
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Hidden);
            Dispatcher.Invoke(() => rbWoman.IsEnabled = true);
            Dispatcher.Invoke(() => rbMan.IsEnabled = true);
            pitchVal = 0;
            Stop1();
        }

        private void rbWoman_Checked(object sender, RoutedEventArgs e)
        {
            TembroClass tembro = new TembroClass();
            string pathFile = @"Pattern\Wide_voiceWoman.tmp";
            tembro.Tembro(SampleRate, pathFile);
            pitchVal = 0;
            reverbVal = 400;
        }

        private void btnModeAudio_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ModeIndex != -1)
            {
                string uri = @"Neurotuners\button\listen-mode-inactive.png";
                ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\listen-mode-active.png";
                ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void slPitchShift_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetPitchShiftValue();
            lbValuePitch.Content = slPitchShift.Value.ToString("f1");
        }

        private void btnModeRecord_MouseMove(object sender, MouseEventArgs e)
        {
            if (ModeIndex != 0)
            {
                string uri = @"Neurotuners\button\record-mode-hover.png";
                ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnAudition1_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            Audition1();
            btnStop.IsEnabled = true;
        }

        private void btnAudition2_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            Audition2();
            btnStop.IsEnabled = true;
        }

        private void btnModeRecord_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ModeIndex != 0)
            {
                string uri = @"Neurotuners\button\record-mode-inactive.png";
                ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"Neurotuners\button\record-mode-active.png";
                ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnModeRecord_Click(object sender, RoutedEventArgs e)
        {
            ModeIndex = 0;
            string uri = @"Neurotuners\button\record-mode-active.png";
            ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            string uri1 = @"Neurotuners\button\listen-mode-inactive.png";
            ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
            Modes();
        }

        private void btnModeAudio_Click(object sender, RoutedEventArgs e)
        {
            ModeIndex = -1;
            string uri = @"Neurotuners\button\listen-mode-active.png";
            ImgBtnModeAudio.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            string uri1 = @"Neurotuners\button\record-mode-inactive.png";
            ImgBtnModeRecord.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
            Modes();
        }

        private void btnSettings_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-settings-hover.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-settings-inactive.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            BtnSetClick++;
            string uri = @"Neurotuners\button\button-settings-active.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            if (BtnSetClick == 1)
            {
                tabNFTSet.SelectedItem = TabSettings;
                btnAudition1.Visibility = Visibility.Hidden;
                //imgShadowNFT.Visibility = Visibility.Hidden;
            }
            else
            {
                if (NFTShadow == 1)
                {
                    //imgShadowNFT.Visibility = Visibility.Visible;
                }
                btnAudition1.Visibility = Visibility.Visible;
                tabNFTSet.SelectedItem = TabNFT;
                BtnSetClick = 0;
            }
        }

        private void btnRecording_Click(object sender, RoutedEventArgs e)
        {
            if (ModeIndex == 0)
            {
                ImgBtnRecordClick = 1;
                string uri = @"Neurotuners\button\button-record-active.png";
                ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                Recording();
                btnRecording.IsEnabled = false;
                btnStart_Open.IsEnabled = false;
                btnModeAudio.IsEnabled = false;
                if (langindex == "0")
                {
                    LogClass.LogWrite("Начало записи голоса.");
                }
                else
                {
                    LogClass.LogWrite("Start voice recording.");
                }
            }
            else if (ModeIndex == 1)
            {
                if (YesNoWin.btnOKInd == 1)
                {
                    if (NFTRecordClick == 0)
                    {
                        /*if (langindex == "0")
                        {
                            string msg = "Держите звук 'А' 3 секунды.";
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            string msg = "Hold the sound 'A' for 3 seconds.";
                            MessageBox.Show(msg);
                        }*/
                        ImgBtnRecordClick = 1;
                        string uri = @"Neurotuners\button\button-record-active.png";
                        ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                        Recording1();
                        btnRecording.IsEnabled = false;
                        btnStart_Open.IsEnabled = false;
                        btnModeAudio.IsEnabled = false;
                        YesNoWin.btnOKInd = 0;
                        if (langindex == "0")
                        {
                            LogClass.LogWrite("Начало первой записи голоса.");
                        }
                        else
                        {
                            LogClass.LogWrite("The beginning of the first voice recording.");
                        }
                    }
                    else if (NFTRecordClick != 0)
                    {
                        /*if (langindex == "0")
                        {
                            string msg = "Держите звук 'А' 3 секунды.";
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            string msg = "Hold the sound 'A' for 3 seconds.";
                            MessageBox.Show(msg);
                        }*/
                        ImgBtnRecordClick = 1;
                        string uri = @"Neurotuners\button\button-record-active.png";
                        ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                        Recording2();
                        YesNoWin.btnOKInd = 0;
                        //btnRecording.IsEnabled = false;
                        //btnStart_Open.IsEnabled = false;
                        //btnModeAudio.IsEnabled = false;
                        if (langindex == "0")
                        {
                            LogClass.LogWrite("Начало второй записи голоса.");
                        }
                        else
                        {
                            LogClass.LogWrite("The beginning of the second voice recording.");
                        }
                    }
                }
                else
                {
                    YesNoWin win = new YesNoWin();
                    win.ShowDialog();
                }
            }
        }

        private void btnIncVol_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-soundup-hover.png";
            imgBTNinaUp.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }//hdchdhddhdhhd

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-sounddown-inactive.png";
            imgBTNinacDown.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnIncVol_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-soundup-inactive.png";
            imgBTNinaUp.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnDecVol_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"Neurotuners\button\button-sounddown-hover.png";
            imgBTNinacDown.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
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
                //mDspPitch = new SampleDSPPitch(mMp3.ToWaveSource(32).ToSampleSource());
                mDspDef = new SampleDSPPitch(mMp3.ToWaveSource(32).ToSampleSource());
                //mDspPitch.PitchShift = (float)Math.Pow(2.0F, 3 / 13.0F);
                //mMixer.AddSource(mDspPitch.ChangeSampleRate(mDspPitch.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                mMixer.AddSource(mDspDef.ChangeSampleRate(mDspDef.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
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

        private async void Audition1()
        {
            try
            {
                //StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                //myfile = FileRecord.ReadToEnd();
                myfile = "MyRecord1.wav";
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
                    string msg = "Ошибка в Audition1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Audition1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Audition2()
        {
            try
            {
                //StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                //myfile = FileRecord.ReadToEnd();
                myfile = "MyRecord2.wav";
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
                    string msg = "Ошибка в Audition2: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Audition2: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void NFT_drawing1(string filename)
        {
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
            NFTShadow = 1;
            //imgShadowNFT.Visibility = Visibility.Visible;

            worker.CancelAsync();
        }

        private async void NFT_drawing2(string filename)
        {
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
            dWw = (int)((Image2.Width - (double)Ww) / 2.0) - 5;
            if (dWw < 0)
                dWw = 0;
            dHw = (int)((Image2.Height - (double)Hw) / 2.0) - 5;
            if (dHw < 0)
                dHw = 0;
            WriteableBitmap wb = new WriteableBitmap((int)Image2.Width, (int)Image2.Height, Ww, Hw, PixelFormats.Bgra32, null);

            // Define the update square (which is as big as the entire image).
            Int32Rect rect = new Int32Rect(0, 0, (int)Image2.Width, (int)Image2.Height);

            byte[] pixels = new byte[(int)Image2.Width * (int)Image2.Height * wb.Format.BitsPerPixel / 8];
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
                int stride = ((int)Image2.Width * wb.Format.BitsPerPixel) / 8;
                wb.WritePixels(rect, pixels, stride, 0);
                k++;
            }
            // Show the bitmap in an Image element.
            Image2.Source = wb;
            Image2.UpdateLayout();
            NFTShadow = 1;
            //imgShadowNFT.Visibility = Visibility.Visible;

            worker.CancelAsync();
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
                            //imgPBNFTBack.Visibility = Visibility.Visible;
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
                            NFTShadow = 1;
                            imgShadowNFT.Visibility = Visibility.Visible;
                            
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

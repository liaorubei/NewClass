using System;
using System.IO;
using System.Speech.Synthesis;
using System.Text;
using System.Windows.Forms;

namespace WindowsTTS
{
    public partial class FormTTS : Form
    {
        private SpeechSynthesizer synthesizer;//TTS合成器
        private String prevPath = null;//上一次打开的默认路径
        public FormTTS()
        {
            InitializeComponent();

            synthesizer = new SpeechSynthesizer();

            var installedVoices = synthesizer.GetInstalledVoices();


            //遍历当前电脑已经安装的阅读者的声音，在”控制面板“的”语音识别“项可以查看已经安装的语言
            //Microsoft Huihui Desktop
            //Microsoft Zira Desktop

            foreach (var item in installedVoices)
            {
                Console.WriteLine(item.VoiceInfo.Name);
            }

            synthesizer.SelectVoice(installedVoices[0].VoiceInfo.Name);//设置阅读者的声音，不过好像不起作用

            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);//设置语音的年龄和性别，好像不起作用
            synthesizer.SetOutputToDefaultAudioDevice();

            //各种事件回调
            synthesizer.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(OnSpeakStarted);
            synthesizer.StateChanged += new EventHandler<StateChangedEventArgs>(OnStateChanged);
            synthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(OnSpeakCompleted);
            synthesizer.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(OnSpeakProgress);

            prevPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //初始化教程
            this.tb_content.AppendText("使用说明\r\n");
            this.tb_content.AppendText("\"打开\"可以选择文本文件,从而实现内容导入功能\r\n");
            this.tb_content.AppendText("\"开始\"包含暂停功能,可以开始/暂停阅读文本框内的内容\r\n");
            this.tb_content.AppendText("\"停止\"停止或取消正在阅读的任务\r\n");
            this.tb_content.AppendText("\"另存\"可以把当前文本框内的内容转为语音文件并保存起来\r\n");
        }

        private void OnSpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            Console.WriteLine("OnSpeakStarted");

            PrintState();
        }

        private void OnSpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            Console.WriteLine("OnSpeakProgress");

            this.pb_tts.Value = e.CharacterPosition;//实时更新进度条进度，注（实际最大值与转语音个数有可能不相符，因为实际上标点符号是不计算在内的）
        }

        private void OnSpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Console.WriteLine("OnSpeakCompleted");

            this.pb_tts.Value = this.pb_tts.Maximum;      //跑完进度条,不管是播放完成,还是导出完成,还是直接取消播放导致的,都让进度条走完
            synthesizer.SetOutputToDefaultAudioDevice();  //因为在保存的时候更改了输出对象,所以在这里要改回输出对象为扬声器,恢复到默认设置
        }

        private void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            //根据播放状态的改变，更改 “暂停/播放" 按钮的文字
            switch (e.State)
            {
                case SynthesizerState.Paused:
                    bt_play_pause.Text = "播放";
                    break;
                case SynthesizerState.Ready:
                    bt_play_pause.Text = "播放";
                    break;
                case SynthesizerState.Speaking:
                    bt_play_pause.Text = "暂停";
                    break;
            }
        }

        private void bt_play_pause_Click(object sender, EventArgs e)
        {
            //根据当前的播放状态，更改合成器的暂停/播放/继续
            switch (synthesizer.State)
            {
                case SynthesizerState.Paused:
                    synthesizer.Resume();
                    break;
                case SynthesizerState.Ready:
                    //进度条最大值，模拟最大值，因为在这里包括了标点符号，与TTS时标点符号不时行计数的
                    this.pb_tts.Maximum = this.tb_content.Text.Trim().Length;
                    //要求用异步来执行,而不是同步方法(SpeechSynthesizer.Speak())不然的话,主线程会被卡住,直到阅读完成
                    synthesizer.SpeakAsync(this.tb_content.Text);
                    break;
                case SynthesizerState.Speaking:
                    synthesizer.Pause();
                    break;
                default:
                    break;
            }

        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            synthesizer.SpeakAsyncCancelAll();//取消播放，直接中止播放
            synthesizer.Resume();//取消播放后会触发结束事件,如果是在"暂停"状态下取消播放会导致[合成器被暂停时,不能更改语音输出设备。]错误,所以这里更改合成器的状态
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            //文件对话框相关设置
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "保存";
            dialog.Filter = "*.wav|*.wav|*.mp3|*.mp3";
            dialog.InitialDirectory = prevPath;
            DialogResult result = dialog.ShowDialog();

            switch (result)
            {
                //如果是确定按钮，则保存文件到指定地方，并且保存过后要把输出设置到扬声器，即在结束事件中更改输出目标，不然没有声音输出了
                case DialogResult.OK:
                    try
                    {
                        if (synthesizer.State == SynthesizerState.Speaking || synthesizer.State == SynthesizerState.Paused)
                        {
                            MessageBox.Show("当有播放任务时(或暂停),当前操作无法继续\r\n你可以停止当前任务后再进行音频保存操作", "提示");
                            return;
                        }
                        PrintState();
                        pb_tts.Maximum = tb_content.Text.Length;//进度条最大值  
                        synthesizer.SetOutputToWaveFile(dialog.FileName);//声音输出到文件系统
                        synthesizer.SpeakAsync(tb_content.Text);
                        prevPath = dialog.FileName;//注意保存目前的文件,便于下次打开或保存时还能打开同一位置(目录)
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                    break;
            }
        }

        /// <summary>
        /// 打印当前合成器的状态,用以观察调试
        /// </summary>
        private void PrintState()
        {
            Console.WriteLine("state=" + synthesizer.State);
        }

        private void bt_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "打开";
            dialog.Filter = "*.txt|*.txt|*.*|*.*";
            dialog.InitialDirectory = prevPath;
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !String.IsNullOrEmpty(dialog.FileName))
            {
                prevPath = dialog.FileName;//注意保存目前的文件,便于下次打开或保存时还能打开同一位置(目录)           

                tb_content.Clear();//清屏文本框
                synthesizer.SpeakAsyncCancelAll();//如果有正在播放的TTS,直接取消全部正在播放的进程，因为要加载新的文本，旧的文本就要清空了  

                String line = null;
                StreamReader reader = new StreamReader(dialog.FileName, Encoding.Default);
                while ((line = reader.ReadLine()) != null)
                {
                    tb_content.AppendText(line);
                    tb_content.AppendText("\r\n");
                }
                reader.Close();//关闭文件流
            }
        }
    }
}

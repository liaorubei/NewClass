using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace WindowsASR
{
    public partial class Form_ASR : Form
    {
        private SpeechRecognitionEngine engine;


        public Form_ASR()
        {
            InitializeComponent();

            // Create an in-process speech recognizer for the en-US locale.
            var installRecognizers = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (var item in installRecognizers)
            {
                Console.WriteLine("RecognizerInfo.Culture={0},RecognizerInfo.Name={1}", item.Culture, item.Name);
            }
            //  engine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            engine = new SpeechRecognitionEngine();


            //创建一个语法生成器
            GrammarBuilder builder = new GrammarBuilder();
            builder.Append(new Choices(new String[] { "选择", "打开" }));
            builder.Append(new Choices(new String[] { "百度地图", "手机语音", "语音识别", "手机百度" }));

            // 生成并加载语法规则
            engine.LoadGrammar(new Grammar(builder));

            // 语音识别事件回调
            engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
            engine.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(OnAudioStateChanged);
            engine.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(OnRecognizeCompleted);


            // 设置默认输入设备为麦克风
            engine.SetInputToDefaultAudioDevice();
        }

        private void OnAudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            Console.WriteLine("OnAudioStateChanged--" + e.AudioState);
        }

        private void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Console.WriteLine("OnRecognizeCompleted--" + e.Result.Text);
        }


        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("OnSpeechRecognized--" + e.Result.Text);
        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            //开始异步语音识别
            engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            engine.RecognizeAsyncCancel();
        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            engine.RecognizeAsyncStop();
        }


    }
}

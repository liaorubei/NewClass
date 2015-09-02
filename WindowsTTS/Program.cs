using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsTTS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormTTS());
            }
            catch (Exception ex)
            {
                StreamWriter writer = new StreamWriter("log.txt");
                writer.WriteLine(ex.StackTrace);
                writer.Flush();
                writer.Close();
            }
        }
    }
}

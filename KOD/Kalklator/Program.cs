using System;
using System.IO;
using System.Windows.Forms;

namespace Kalklator
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string loadedExpression = null;

            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".kalk"))
            {
                loadedExpression = File.ReadAllText(args[0]);
            }

            Application.Run(new Form1(loadedExpression));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowVision
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Preload the ONNX model on a background thread so it's ready when needed
            Task.Run(() =>
            {
                try
                {
                    // Accessing the Instance property triggers the model loading
                    var parser = FlowVision.lib.Classes.SimpleOmniParser.Instance;
                }
                catch
                {
                    // Ignore startup errors - they will be caught/logged when the user actually tries to use it
                }
            });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

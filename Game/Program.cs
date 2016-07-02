using System;
using System.Windows.Forms;
using OurGame.Engine.Serialization;
using System.IO;

namespace OurGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //try
            //{
                using (var game = OurGame.Game)
                    game.Run();
            //}
            //catch(Exception e)
            //{

            //    string message = e.Message + "\n" + e.StackTrace;
            //    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            
        }
    }
#endif
}

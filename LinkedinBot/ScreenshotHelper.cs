using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LinkedinBot
{
    public static class ScreenshotHelper
    {
        private static DirectoryInfo directoryForScreenshots = new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), "Screenshots"));

        static ScreenshotHelper()
        {
            if (Directory.Exists(directoryForScreenshots.FullName)) return;

            Directory.CreateDirectory(directoryForScreenshots.FullName);
        }

        public static void Make(int id, string postfix)
        {
            Graphics graph = null;

            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            graph = Graphics.FromImage(bmp);

            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            bmp.Save(Path.Combine(directoryForScreenshots.ToString(), 
                string.Format("{0}_{1}_{2}.png", GetCorretDateTime(), id, postfix)));
        }

        private static string GetCorretDateTime()
        {
            return DateTime.Now.ToString("yy_MM_dd_HH_m_s");
        }
    }
}

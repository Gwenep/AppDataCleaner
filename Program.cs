using System;
using System.Threading;
using System.Windows.Forms;

namespace AppDataCleaner
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.ThreadException += (_, e) => HandleUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    AppLogger.Error("未处理的线程异常", ex);
            };

            Application.Run(new Form1());
        }

        private static void HandleUnhandledException(Exception ex)
        {
            AppLogger.Error("未处理的界面异常", ex);

            MessageBox.Show(
                $"程序遇到一个未处理的错误，详细信息已写入日志。{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "出错了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

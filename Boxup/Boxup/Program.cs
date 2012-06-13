using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
namespace Boxup
{
    static class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        /// <summary>
        /// 发生未捕获的异常时，更新为下线状态。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("出现如下错误：\r\n{0}\r\n{1}\r\n如果您经常遇到此问题，请与软件开发商联系。", e.Exception.Message, e.Exception), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //MessageBox.Show(Resources.Application_ThreadException_Pre + e.Exception.Message + Resources.Application_ThreadException_Tail, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Error(e.Exception.Message, e.Exception);
            //}
            throw e.Exception;
        }
    }
}

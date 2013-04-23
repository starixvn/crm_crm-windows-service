// -----------------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Web;

    /**
     * public class Log Extensions
     * @Author: bobby.nguyen
     * @Date: 11012011
     **/
    public static class LogExtensions
    {
        private static BackgroundWorker _worker = null;
        private static Queue<Exception> _queueEx = new Queue<Exception>();
        /**
         * Function: Exception Handler
         */
        public static void ExceptionHandler(Exception ex)
        {
            _queueEx.Enqueue(ex);
            using (_worker = new BackgroundWorker())
            {
                _worker.WorkerSupportsCancellation = true;
                _worker.DoWork += new DoWorkEventHandler(WorkerRunning);
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
                _worker.RunWorkerAsync();
            }
        }
        /**
         * Function: BackgroundWorker running
         */
        private static void WorkerRunning(object sender, DoWorkEventArgs e)
        {
            if (_queueEx.Count > 0)
            {
                var ex = _queueEx.Dequeue();
                Fanex.Logging.Logger.Log.Error(ex.Message, ex);

            }
            else
            {
                _worker.CancelAsync();
            }
        }
        /**
         * Function: BackgroundWorker commpleted
         */
        private static void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO
        }


        public static void WriteLog(string sContent)
        {
            var outPath = string.Format(@"{0}\Logs\Error", HttpContext.Current.Server.MapPath("~/"));

            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            outPath += string.Format(@"\{0:yyyy-MM-dd}.txt", DateTime.Now.Date);

            var fs = new FileStream(outPath, FileMode.Append, FileAccess.Write);
            var sw = new StreamWriter(fs);

            sw.WriteLine(DateTime.Now + ": " + sContent);

            sw.Close();
            fs.Close();
        }
    }
}

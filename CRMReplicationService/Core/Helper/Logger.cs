// -----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Core.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Logger
    {
        public static void WriteLog(string message)
        {
            message += "<br/><hr/>";
            string path = Application.StartupPath + "/Logs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += string.Format("/{0}_log.htm", DateTime.Now.ToString("MM-dd-yyyy"));

            // Specify file, instructions, and privelegdes
            var file = new FileStream(path, FileMode.Append, FileAccess.Write);

            // Create a new stream to write to the file
            var sw = new StreamWriter(file);

            // Write a string to the file
            sw.WriteLine(string.Format("{0}--{1}", DateTime.Now, message));

            // Close StreamWriter
            sw.Close();

            // Close file
            file.Close();
        }
    }
}

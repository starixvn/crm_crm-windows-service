// -----------------------------------------------------------------------
// <copyright file="SendMailExtension.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Mail;
    using System.Text;
    using Fanex.CRM.CRMReplication.Core.Configurations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SendMailExtension
    {
        #region private variables
        private static BackgroundWorker _worker = null;
        private static Queue<MailSend> _queueMail = new Queue<MailSend>();
        private static bool _sendMailStatus = true;
        #endregion
        /**
         * Accounting - Send mail & notification
         **/
        public static void AccSendMail(MailSection.HostName hostName, string to, string cc, string mailTitle, string subject, string body)
        {
            AccSendMail(hostName, to, cc, mailTitle, subject, body, null);

        }
        public static bool AccSendMail(MailSection.HostName hostName, string to, string cc, string mailTitle, string subject, string body, MailAttachment mailAttachment)
        {
            AccSendMail(hostName, to, cc, string.Empty, mailTitle, subject, body, mailAttachment, null);
            return _sendMailStatus;
        }
        public static bool AccSendMail(MailSection.HostName hostName, string to, string cc, string bcc, string mailTitle, string subject, string body, MailAttachment mailAttachment)
        {
            AccSendMail(hostName, to, cc, bcc, mailTitle, subject, body, mailAttachment, null);
            return _sendMailStatus;
        }
        public static void AccSendMail(MailSection.HostName hostName, string to, string cc, string bcc, string mailTitle, string subject, string body, MailAttachment mailAttachment, AlternateView altView)
        {
            var mailSend = new MailSend()
            {
                HostName = hostName,
                To = to,
                Cc = cc,
                Bcc = bcc,
                Title = mailTitle,
                Subject = subject,
                Body = body,
                Attachment = mailAttachment,
                AltView = altView
            };

            _queueMail.Enqueue(mailSend);

            using (_worker = new BackgroundWorker())
            {
                _worker.WorkerSupportsCancellation = true;
                _worker.DoWork += new DoWorkEventHandler(WorkerRunning);
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
                _worker.RunWorkerAsync();
            }

        }
        /**
         * Function: Backgroundworker running
         */
        private static void WorkerRunning(object sender, DoWorkEventArgs e)
        {
            if (_queueMail.Count > 0)
            {
                var queueMail = _queueMail.Dequeue();
                e.Result = SendMail(queueMail);
            }
            else
            {
                _worker.CancelAsync();
            }
        }
        /**
         * Function: Backgroundworker commpleted
         */
        private static void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _sendMailStatus = (bool)e.Result;
        }

        public static bool SendMail(MailSend mailSend)
        {
            try
            {
                var mailSetting = MailSection.Instance[MailSection.Setting.Server][mailSend.HostName.ToString()];

                var msg = new MailMessage();

                #region to

                if (!string.IsNullOrEmpty(mailSend.To))
                {
                    var mailArr = mailSend.To.Split(';');
                    foreach (var toMail in mailArr)
                    {
                        msg.To.Add(toMail);
                    }

                }

                #endregion to

                msg.From = new MailAddress(mailSetting.Username, mailSend.Title, Encoding.UTF8);

                #region cc

                if (!string.IsNullOrEmpty(mailSend.Cc))
                {
                    var mailArr = mailSend.Cc.Split(';');
                    foreach (var ccMail in mailArr)
                    {
                        msg.CC.Add(ccMail);
                    }
                }

                #endregion

                #region bcc
                if (!string.IsNullOrEmpty(mailSend.Bcc))
                {
                    var mailArr = mailSend.Bcc.Split(';');
                    foreach (var bccMail in mailArr)
                    {
                        msg.Bcc.Add(bccMail);
                    }
                }

                #endregion
                msg.Subject = mailSend.Subject;
                msg.SubjectEncoding = Encoding.UTF8;

                msg.IsBodyHtml = true;
                msg.Body = mailSend.Body;
                msg.BodyEncoding = Encoding.UTF8;

                var attachFile = mailSend.Attachment;
                if (attachFile != null)
                {
                    attachFile.Content.Seek(0, SeekOrigin.Begin);
                    var attachment = new Attachment(attachFile.Content, attachFile.FileName, attachFile.ContentType);
                    msg.Attachments.Add(attachment);
                }

                if (mailSend.AltView != null)
                {
                    msg.AlternateViews.Add(mailSend.AltView);
                }
                msg.Priority = MailPriority.High;

                var client = new SmtpClient
                {
                    Credentials =
                        new System.Net.NetworkCredential(mailSetting.Username, mailSetting.Password),
                    Port = mailSetting.Port,
                    Host = mailSetting.Host,
                    EnableSsl = true
                };

                client.Send(msg);
            }
            catch (Exception ex)
            {
                Helper.Logger.WriteLog(ex.Message);
                Helper.Logger.WriteLog(ex.StackTrace);
                return false;
            }
            return true;
        }

        public static bool SendMail(MailSend mailSend, List<MailAttachment> mailAttachment)
        {
            try
            {
                //var mailSetting = MailSection.Instance[MailSection.Setting.Server][mailSend.HostName.ToString()];
                var mailSetting = MailSection.Instance[MailSection.Setting.Server][MailSection.Instance.Active];

                var msg = new MailMessage();

                #region to

                if (!string.IsNullOrEmpty(mailSend.To))
                {
                    var mailArr = mailSend.To.Split(';');
                    foreach (var toMail in mailArr)
                    {
                        msg.To.Add(toMail);
                    }

                }

                #endregion to

                msg.From = new MailAddress(mailSetting.Username, mailSend.Title, Encoding.UTF8);

                #region cc

                if (!string.IsNullOrEmpty(mailSend.Cc))
                {
                    var mailArr = mailSend.Cc.Split(';');
                    foreach (var ccMail in mailArr)
                    {
                        msg.CC.Add(ccMail);
                    }
                }

                #endregion

                #region bcc
                if (!string.IsNullOrEmpty(mailSend.Bcc))
                {
                    var mailArr = mailSend.Bcc.Split(';');
                    foreach (var bccMail in mailArr)
                    {
                        msg.Bcc.Add(bccMail);
                    }
                }

                #endregion
                msg.Subject = mailSend.Subject;
                msg.SubjectEncoding = Encoding.UTF8;

                msg.IsBodyHtml = true;
                msg.Body = mailSend.Body;
                msg.BodyEncoding = Encoding.UTF8;

                //var attachFile = mailSend.Attachment;
                if (mailAttachment != null && mailAttachment.Count > 0)
                {
                    //attachFile.Content.Seek(0, SeekOrigin.Begin);
                    //var attachment = new Attachment(attachFile.Content, attachFile.FileName, attachFile.ContentType);
                    //msg.Attachments.Add(attachment);
                    foreach (var file in mailAttachment)
                    {
                        file.Content.Seek(0, SeekOrigin.Begin);
                        var attachment = new Attachment(file.Content, file.FileName, file.ContentType);
                        msg.Attachments.Add(attachment);
                    }
                }

                if (mailSend.AltView != null)
                {
                    msg.AlternateViews.Add(mailSend.AltView);
                }
                msg.Priority = MailPriority.High;

                var client = new SmtpClient
                {
                    Port = mailSetting.Port,
                    Host = mailSetting.Host
                };
                if (mailSetting.Auth)
                {
                    client.Credentials = new System.Net.NetworkCredential(mailSetting.Username, mailSetting.Password);
                    client.EnableSsl = true;
                }

                client.Send(msg);
            }
            catch (Exception ex)
            {
                Helper.Logger.WriteLog(ex.Message);
                Helper.Logger.WriteLog(ex.StackTrace);
                return false;
            }
            return true;
        }



    }
    public class MailSend
    {
        public MailSection.HostName HostName { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailAttachment Attachment { get; set; }
        public AlternateView AltView { get; set; }

    }
    public class MailAttachment
    {
        public string FileName { get; set; }
        public MemoryStream Content { get; set; }
        public string ContentType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CULROC_Official.Models.ValueObject;

namespace CULROC_Official.Models.FunctionObject
{
    public class EmailFO
    {
        //modify from https://dotblogs.com.tw/shadow/archive/2011/05/23/25887.aspx

        /// <summary>
        /// MailMessage 
        /// </summary>
        private MailMessage _mailMessage;

        /// <summary>
        /// 取得錯誤訊息(當Email發送失敗時)
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// 完整的錯誤訊息物件
        /// </summary>
        public SmtpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Password
        /// </summary>
        private const string Password = "test2276";

        /// <summary>
        /// ServerMailAddress
        /// </summary>
        private const string ServerMailAddress = "test@jesda.com.tw";

        /// <summary>
        /// SmtpServer use google smtp server
        /// </summary>
        private const string SmtpServer = "smtp.gmail.com";
        
        /// <summary>
        /// SmtpServer port use google smtp server
        /// </summary>
        private const int SmtpServerPort = 587;

        private SendCompletedEventHandler SendCompeleted = null;

        public EmailFO(EmailVO emailVO)
        {
            _mailMessage = new MailMessage();
            this.SetRecipient(emailVO.Address);
            _mailMessage.Subject = emailVO.Subject;
            _mailMessage.Body = emailVO.Context;
        }

        public EmailFO(EmailVO emailVO, SendCompletedEventHandler sendCompeleted) : this(emailVO)
        {
            SendCompeleted = sendCompeleted;
        }



        /// <summary>
        /// 設定收件者
        /// </summary>
        /// <param name="recipients">收件者信箱</param>
        /// <returns>self</returns>
        public void SetRecipient(List<string> recipients)
        {
            if (recipients.Count > 0)
            {
                foreach (string recipient in recipients)
                    _mailMessage.To.Add(recipient);
            }
        }


        /// <summary>
        /// 設定夾帶檔案
        /// </summary>
        /// <param name="fileStreams">(夾帶檔案名稱,夾帶檔案串流)</param>
        /// <returns>self</returns>
        public void SetFile(IDictionary<string, Stream> fileStreams)
        {
            //附件處理
            if (fileStreams != null && fileStreams.Count > 0) //寄信時有夾帶附檔
            {
                foreach (var fileStream in fileStreams)
                {
                    Attachment attFile = new Attachment(fileStream.Value, fileStream.Key);
                    _mailMessage.Attachments.Add(attFile);
                }
            }
        }




        /// <summary>
        /// 寄出信件
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Send()
        {
            try
            {
                _mailMessage.From = new MailAddress(ServerMailAddress);

                _mailMessage.IsBodyHtml = false;

                //公司、客戶的smtp_server
                SmtpClient client = new SmtpClient(SmtpServer, SmtpServerPort);
                client.Credentials = new NetworkCredential(ServerMailAddress,Password); //寄信帳密
                    
                client.EnableSsl = true;
                client.SendCompleted += SendCompeleted;
                //client.Send(_mailMessage); //寄出一封信
                client.Send(_mailMessage);



                ////釋放每個附件，才不會Lock住
                //if (_mailMessage.Attachments.Count > 0)
                //{
                //    foreach (var attachment in _mailMessage.Attachments)
                //    {
                //        attachment.Dispose();
                //    }
                //}
                //_mailMessage.Dispose();

            }
            catch (SmtpException exp)
            {
                ErrorMsg = exp.Message;
                StatusCode =  exp.StatusCode;
                return false;
            }
            StatusCode = SmtpStatusCode.Ok;
            return true; //成功

        }
    }
}
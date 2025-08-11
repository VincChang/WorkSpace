using System;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace CSNetLib;

/// <summary>
/// 寄送 email
/// </summary>
public class ofSmtp
{
    public ofSmtp() { }
    /// <summary>
    /// 寄送 email
    /// </summary>
    /// <param name="from">寄件者 email</param>
    /// <param name="tos">收件者 email, 可為多個, 例:abc@ddd.com;cde@yahoo.com</param>
    /// <param name="ccs">副本收件者 email, 可為多個, 例:abc@ddd.com;cde@yahoo.com</param>
    /// <param name="bccs">密件副本收件者 email, 可為多個, 例:abc@ddd.com;cde@yahoo.com</param>
    /// <param name="subject">主旨</param>
    /// <param name="body">內文</param>
    /// <param name="attaches">附檔, 可為多個, 例:c:\document\certificat.pdf;d:\profile\joblist.txt</param>
    /// <param name="userCredential">true:直接使用 user 之認證資訊</param>
    /// <param name="relayserver">relay server ip or hostname</param>
    public static void SendMail(string from,
                                string tos,
                                string ccs,
                                string bccs,
                                string subject,
                                string body,
                                string attaches,
                                bool userCredential,
                                string relayserver)
    {
        using (SmtpClient client = new SmtpClient())
        {
            client.UseDefaultCredentials = userCredential;
            client.Host = relayserver;
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(from.Trim());
                message.SubjectEncoding = Encoding.UTF8;
                message.Subject = subject;
                SetBody(body, message);
                SetAddresses(message.To, tos);
                SetAddresses(message.CC, ccs);
                SetAddresses(message.Bcc, bccs);
                SetAttachments(message.Attachments, attaches);
                client.Send(message);
            }
        }
    }
    private static void SetBody(string value, MailMessage message)
    {
        message.BodyEncoding = Encoding.UTF8;
        string bodyValue = string.Empty;
        if (value.ToLower().Trim().StartsWith("<!doctype html>") || value.ToLower().Contains("<html>"))
        {
            message.IsBodyHtml = true;
            bodyValue = value;
        }
        else
        {
            message.IsBodyHtml = true;
            bodyValue = string.Format("<!DOCTYPE html>\n<html>\n<head>\n</head>\n<body>\n{0}\n</body>\n</html>", value);
        }
        message.Body = bodyValue;
    }
    private static void SetAddresses(MailAddressCollection mac, string text)
    {
        if (string.IsNullOrEmpty(text) || text.GetType() == typeof(DBNull))
            return;
        foreach (string address in text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            mac.Add(new MailAddress(address));
    }
    private static void SetAttachments(AttachmentCollection ac, string text)
    {
        if (string.IsNullOrEmpty(text) || text.GetType() == typeof(DBNull))
            return;
        foreach (string attach in text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            ac.Add(FileAttachment(attach));
    }
    private static Attachment FileAttachment(string FileName)
    {
        // this solution was founded at 
        // https://connect.microsoft.com/VisualStudio/feedback/details/676569/max-13-characters-in-attachment-when-using-chinese-characters
        FileInfo fi = new FileInfo(FileName);
        Attachment attach = new Attachment(FileName);
        attach.NameEncoding = Encoding.UTF8;
        attach.Name = UTF8Encoding(fi.Name);
        return attach;
    }
    public static string UTF8Encoding(string value)
    {
        return string.Format(" =?utf-8?B?{0}?=", Base64Encode(value));
    }
    private static string Base64Encode(string data)
    {
        try
        {
            byte[] encDataByte = Encoding.UTF8.GetBytes(data);
            string encodedData = Convert.ToBase64String(encDataByte);
            return encodedData;
        }
        catch (Exception e)
        {
            throw new Exception("Error in base64Encode" + e.Message);
        }

    }
}
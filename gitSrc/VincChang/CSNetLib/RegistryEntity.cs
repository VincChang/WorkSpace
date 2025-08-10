using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSNetLib
{
    public class RegistryEntity
    {
        public const string ServerMssql = "mssql";
        public const string ServerSftp = "sftp";
        public const string ServerOracle = "oracle";
        public const string NonSpecific = "nonspecific";
        private const string KeyManagerName = @".DEFAULT\Software\Yuanta\KeyManager";
        private string MyFullKeyName { get { return $"{KeyManagerName}\\{Func}\\{Server.Replace('\\', '$')}\\{User.Replace('\\', '$')}"; } }
        public RegistryKey RegKey { get; set; }
        public string Func { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public DateTime ChangedTime { get; set; }
        public string Working { get; set; }
        public string Password { get; set; }
        public string Service { get; set; }
        public string PasswordEncrypted { get; set; }
        /// <summary>
        /// 取得以 KeyDataManager 程式維護所加密的密碼存放在  RegistryKey 裡
        /// </summary>
        /// <param name="kind">請以 ServerMssql / ServerSftp / ServerOracle 決定 server function</param>
        /// <param name="server">server 位置, 若包含 '\' 時則會轉換為 '$' </param>
        /// <param name="user">帳號, 若包含 '\' 時則會轉換為 '$' </param>
        public RegistryEntity(string kind, string server, string user)
        {
            Func = kind;
            Server = server;
            User = user;
            RegKey = Registry.Users.OpenSubKey(MyFullKeyName, RegistryKeyPermissionCheck.ReadSubTree);
            if (RegKey != null)
            {
                var obj = RegKey.GetValue("ChangedTime");
                if (obj != null)
                    ChangedTime = DateTime.Parse(obj.ToString());
                Working = RegKey.GetValue("Working")?.ToString();
                Service = RegKey.GetValue("Service")?.ToString();
                PasswordEncrypted = RegKey.GetValue("Password")?.ToString();
                Password = ofCrypto.DecryptStringAES(PasswordEncrypted, Working);
            }
            else
                throw new Exception($"registry key {MyFullKeyName} 資料不存在, 請以 KeyDataManager 維護!");
        }
    }
}
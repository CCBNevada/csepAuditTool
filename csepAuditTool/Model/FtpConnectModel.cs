using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace csepAuditTool.Model
{
    internal class FtpConnectModel
    {
        public string Sftp_Host { get; set; }
        public string Sftp_User { get; set; }
        public string Sftp_Pass { get; set; }
        public string Sftp_Port { get; set; }
        public int Sftp_PortInt { get { return int.Parse(Sftp_Port); } }
        public string Sftp_RemoteDirectory { get; set; }
        public string Sftp_IncomingFileRoot { get; set; }
        public string Sftp_RemoteFilenameDownloadToday { get { return Sftp_IncomingFileRoot + DateTime.Now.ToString("yyyyMMdd"); } }
        public string Sftp_RemoteFullPath { get { return Sftp_RemoteDirectory.TrimEnd('/') + "/" + Sftp_RemoteFilenameDownloadToday; } }
        public string Sftp_LocalDirectory { get; set; }
        public string Sftp_LocalFilenameRoot { get; set; }
        public string Sftp_LocalFilename { get { return Sftp_LocalFilenameRoot + DateTime.Now.ToString("yyyyMMdd"); } }
        public string Sftp_LocalFullPath { get { return Sftp_LocalDirectory.TrimEnd('\\') + "\\" + Sftp_LocalFilename; } }
        public string Sftp_OutgoingFileRoot { get; set; }
        public string Sftp_OutgoingFilename { get { return Sftp_OutgoingFileRoot + DateTime.Now.ToString("yyyyMMdd"); } }
        public string Sftp_LocalUploadFullPath { get { return Sftp_LocalDirectory.TrimEnd('\\') + "\\" + Sftp_OutgoingFilename; } }
        //public string Sftp_RemoteFilenameUploadToday { get { return Sftp_OutgoingFileRoot + DateTime.Now.ToString("yyyyMMdd"); } }
        //public string Sftp_RemoteFileLocalFullPath { get { return Sftp_LocalFilenameRoot; } }
        public string Sftp_RemoteFileUploadFullPath { get { return Sftp_RemoteDirectory.TrimEnd('/') + '/' + Sftp_OutgoingFilename; } }
        //      <add key = "sftp_host" value="ccbnvdev.sftp.wpengine.com"/>
        //<add key = "sftp_user" value="ccbnvdev-sdotson"/>
        //<add key = "sftp_pass" value="23fPB0HVOemhkXAvzB0"/>
        //<add key = "sftp_port" value="2222"/>
        //<add key = "sftp_remoteDirectory" value="nvkids/dotx/locate"/>
        //<add key = "sftp_localDirectory" value="C:\CSEP_Working_Temp"/> <!-- do not forget the closing slash -->
        //<add key = "sftp_localFilenameRoot" value="csepFileReq_"/>
        //<add key = "sftp_incomingFileRoot" value="req"/>
        //<add key = "sftp_outgoingFileRoot" value="rsp"/>
        private SftpClient? _localClient { get; set; }

        public FtpConnectModel()
        {
            try
            {
                Sftp_Host = ConfigurationManager.AppSettings["sftp_host"] ?? string.Empty;
                Sftp_User = ConfigurationManager.AppSettings["sftp_user"] ?? string.Empty;
                Sftp_Pass = ConfigurationManager.AppSettings["sftp_pass"] ?? string.Empty;
                Sftp_Port = ConfigurationManager.AppSettings["sftp_port"] ?? string.Empty;
                Sftp_RemoteDirectory = ConfigurationManager.AppSettings["sftp_remoteDirectory"] ?? string.Empty;
                Sftp_IncomingFileRoot = ConfigurationManager.AppSettings["sftp_incomingFileRoot"] ?? string.Empty;
                Sftp_LocalDirectory = ConfigurationManager.AppSettings["sftp_localDirectory"] ?? string.Empty;
                Sftp_LocalFilenameRoot = ConfigurationManager.AppSettings["sftp_localFilenameRoot"] ?? string.Empty;
                Sftp_OutgoingFileRoot = ConfigurationManager.AppSettings["sftp_outgoingFileRoot"] ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool CheckValues()
        {
            if (Sftp_Host == string.Empty ||
                Sftp_User == string.Empty ||
                Sftp_Pass == string.Empty ||
                Sftp_Port == string.Empty ||
                Sftp_RemoteDirectory == string.Empty ||
                Sftp_IncomingFileRoot == string.Empty ||
                Sftp_LocalDirectory == string.Empty ||
                Sftp_LocalFilenameRoot == string.Empty ||
                Sftp_OutgoingFileRoot == string.Empty)
            {
                return false;
            }
            if (!int.TryParse(Sftp_Port, out int testInt)) return false;
            return true;
        }

        public bool LocalDirectoryExistsCheck()
        {
            var directoryExists = System.IO.Directory.Exists(Sftp_LocalDirectory);
            if (!directoryExists) System.IO.Directory.CreateDirectory(Sftp_LocalDirectory);
            return System.IO.Directory.Exists(Sftp_LocalDirectory);
        }

        public void BuildSftpClientConnection()
        {
            _localClient = new SftpClient(Sftp_Host, Sftp_PortInt, Sftp_User, Sftp_Pass);
        }

        public bool CheckConnection()
        {
            try
            {
                if (_localClient == null) BuildSftpClientConnection();
                if(_localClient != null && !_localClient.IsConnected) _localClient.Connect();
            }
            catch (Exception ex)
            {
                if (_localClient != null) _localClient.Disconnect();
                return false;
            }
            if (_localClient == null) return false;
            return _localClient.IsConnected;
        }

        public bool RemoteDirectoryExists()
        {
            if (!CheckConnection()) return false;
            return _localClient != null && _localClient.Exists(Sftp_RemoteDirectory);
        }

        public bool RemoteFileExists()
        {
            if (!CheckConnection()) return false;
            return _localClient != null && _localClient.Exists(Sftp_RemoteFullPath);
        }

        public bool DownloadFile()
        {
            if (!CheckConnection()) return false;
            if(_localClient == null) return false;
            using (Stream myStream = File.OpenWrite(Sftp_LocalFullPath))
            {
                _localClient.DownloadFile(Sftp_RemoteFullPath, myStream);
            }
            _localClient.Disconnect();
            return File.Exists(Sftp_LocalFullPath);
        }

        public string BuildOutgoingContentString(List<string> OutgoingResults)
        {
            var thisFileContent = "";
            for (var i = 0; i < OutgoingResults.Count; i++)
            {
                thisFileContent += OutgoingResults[i].ToString() + "\r\n";
            }
            return thisFileContent;
        }

        public bool SaveUploadFile(List<string> outgoingMatches)
        { 
            if (File.Exists(Sftp_LocalUploadFullPath)) File.Delete(Sftp_LocalUploadFullPath);
            var outgoingFileContent = BuildOutgoingContentString(outgoingMatches);
            using (var localFileStream = new FileStream(Sftp_LocalUploadFullPath, FileMode.CreateNew))
            {
                using (var localWriter = new BinaryWriter(localFileStream))
                {
                    localWriter.Write(Encoding.UTF8.GetBytes(outgoingFileContent));
                }
            }
            return File.Exists(Sftp_LocalUploadFullPath);
        }

        public bool UploadFile()
        {
            if (!CheckConnection()) return false;
            if(_localClient == null) return false;
            using (FileStream fs = new FileStream(Sftp_LocalUploadFullPath, FileMode.Open))
            {
                _localClient.BufferSize = 1024;
                _localClient.UploadFile(fs, Sftp_RemoteFileUploadFullPath);
            }
            var remoteFileExists = _localClient.Exists(Sftp_RemoteFileUploadFullPath);
            _localClient.Disconnect();
            return remoteFileExists;
        }
    }
}

//https://www.c-sharpcorner.com/blogs/sftp-file-upload-with-c-sharp-application
//Public static void Main(string[] args)
//{
//    Console.WriteLine("Create client Object");
//    using (SftpClient sftpClient = new SftpClient(getSftpConnection("host", "userName", 22, "filePath")))
//    {
//        Console.WriteLine("Connect to server");
//        sftpClient.Connect();
//        Console.WriteLine("Creating FileStream object to stream a file");
//        using (FileStream fs = new FileStream("filePath", FileMode.Open))
//        {
//            sftpClient.BufferSize = 1024;
//            sftpClient.UploadFile(fs, Path.GetFileName("filePath"));
//        }
//        sftpClient.Dispose();
//    }
//}

//public static ConnectionInfo getSftpConnection(string host, string username, int port, string publicKeyPath)
//{
//    return new ConnectionInfo(host, port, username, privateKeyObject(username, publicKeyPath));
//}

//private static AuthenticationMethod[] privateKeyObject(string username, string publicKeyPath)
//{
//    PrivateKeyFile privateKeyFile = new PrivateKeyFile(publicKeyPath);
//    PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod =
//       new PrivateKeyAuthenticationMethod(username, privateKeyFile);
//    return new AuthenticationMethod[]
//     {
//        privateKeyAuthenticationMethod
//     };
//}
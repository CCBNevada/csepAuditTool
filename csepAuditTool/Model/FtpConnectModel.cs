using Renci.SshNet;
using SimpleLogger;
using System.Configuration;
using System.Text;

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
        public string Sftp_RemoteFileUploadFullPath { get { return Sftp_RemoteDirectory.TrimEnd('/') + '/' + Sftp_OutgoingFilename; } }
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
                SimpleLog.Info("FtpConnect has required parameter values from app.config. (new FtpConnectModel())");
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
            if (!int.TryParse(Sftp_Port, out int sftpInt)) return false;
            SimpleLog.Info("FtpConnect required properties have parseable values. (FtpConnectModel.CheckValues())");
            return true;
        }

        public bool LocalDirectoryExistsCheck()
        {
            var directoryExists = Directory.Exists(Sftp_LocalDirectory);
            if (!directoryExists) Directory.CreateDirectory(Sftp_LocalDirectory);
            var localDirExists = Directory.Exists(Sftp_LocalDirectory);
            if (!localDirExists) return false;
            SimpleLog.Info("Local Directory found to exist after creation or locate. (FtpConnectModel.LocalDirectoryExistsCheck())");
            return true;
        }

        public void BuildSftpClientConnection()
        {
            _localClient = new SftpClient(Sftp_Host, Sftp_PortInt, Sftp_User, Sftp_Pass);
            SimpleLog.Info("SftpClient connection built. (FtpConnectModel.BuildSftpClientConnection())");
        }

        public bool CheckConnection()
        {
            var updatedOrOpened = false;
            try
            {
                if (_localClient == null)
                {
                    updatedOrOpened = true;
                    BuildSftpClientConnection();
                }
                if (_localClient != null && !_localClient.IsConnected)
                {
                    updatedOrOpened = true;
                    _localClient.Connect();
                }
            }
            catch (Exception ex)
            {
                if (_localClient != null) _localClient.Disconnect();
                return false;
            }
            if (_localClient == null) return false;
            var clientConnected = _localClient.IsConnected;
            if (!clientConnected) return false;
            if (updatedOrOpened) SimpleLog.Info("Sftp created and opened, SftpClient.IsConnected=true. (FtpConnectModel.CheckConnection())");
            return true;
        }

        public bool RemoteDirectoryExists()
        {
            if (!CheckConnection()) return false;
            var remoteDirectoryExists = _localClient != null && _localClient.Exists(Sftp_RemoteDirectory);
            if (!remoteDirectoryExists) return false;
            SimpleLog.Info("Remote Directory found to exist. (FtpConnectModel.RemoteDirectoryExists())");
            return true;
        }

        public bool RemoteFileExists()
        {
            if (!CheckConnection()) return false;
            var remoteFileExists = _localClient != null && _localClient.Exists(Sftp_RemoteFullPath);
            if (!remoteFileExists) return false;
            SimpleLog.Info("Remote File found to exist. (FtpConnectModel.RemoteFileExists())");
            return true;
        }

        public bool DownloadFile()
        {
            if (!CheckConnection()) return false;
            if (_localClient == null) return false;
            using (Stream myStream = File.OpenWrite(Sftp_LocalFullPath))
            {
                _localClient.DownloadFile(Sftp_RemoteFullPath, myStream);
            }
            _localClient.Disconnect();
            var downloadedFileExists = File.Exists(Sftp_LocalFullPath);
            if (!downloadedFileExists) return false;
            SimpleLog.Info("File Downloaded successfully and exists locally. (FtpConnectModel.DownloadFile())");
            return true;
        }

        public string BuildOutgoingContentString(List<string> OutgoingResults)
        {
            var thisFileContent = "";
            for (var i = 0; i < OutgoingResults.Count; i++)
            {
                thisFileContent += OutgoingResults[i].ToString() + "\r\n";
            }
            SimpleLog.Info("Outgoing Result String built. (FtpConnectModel.BuildOutgoingContentString()");
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
            var uploadedFile = File.Exists(Sftp_LocalUploadFullPath);
            if (!uploadedFile) return false;
            SimpleLog.Info("Upload file created and saved to local disk. (FtpConnectModel.SaveUploadFile())");
            return true;
        }

        public bool UploadFile()
        {
            if (!CheckConnection()) return false;
            if (_localClient == null) return false;
            using (FileStream fs = new FileStream(Sftp_LocalUploadFullPath, FileMode.Open))
            {
                _localClient.BufferSize = 1024;
                _localClient.UploadFile(fs, Sftp_RemoteFileUploadFullPath);
            }
            var remoteFileExists = _localClient.Exists(Sftp_RemoteFileUploadFullPath);
            _localClient.Disconnect();
            if (!remoteFileExists) return false;
            SimpleLog.Info("Upload file uploaded to Sftp server. (FtpConnectModel.UploadFile())");
            return true;
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
using csepAuditTool.Model;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Data;

namespace csepAuditTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello, World!");
                Console.WriteLine("Main Entry.");

                var ftpConn = new FtpConnectModel();

                if (!ftpConn.CheckValues()) throw new Exception("At least one required config value is missing. Update App.config to continue.");

                if (!ftpConn.LocalDirectoryExistsCheck()) throw new Exception("Unable to read from or create local directory. Update App.config to continue.");

                ftpConn.BuildSftpClientConnection();

                if (!ftpConn.CheckConnection()) throw new Exception("Unable to connect to SFTP with existing Configuration. Update App.config to continue.");

                if (!ftpConn.RemoteDirectoryExists()) throw new Exception("Remote directory does not exist. Update App.config to continue.");

                if (!ftpConn.RemoteFileExists()) throw new Exception("Remote file on server at provided path missing. Update App.config to continue.");

                if (!ftpConn.DownloadFile()) throw new Exception("No file downloaded, unknown error occurred. Update App.config to continue.");

                var incomingValues = new IncomingRowsCollectionModel(ftpConn);

                var outgoingResultMatches = new OutgoingRowsCollectionModel(incomingValues);

                var uploadLocateRequest = new UploadLocateRequestModel(outgoingResultMatches, ftpConn);
                if(uploadLocateRequest.FileUploaded)
                {
                    var myTest = uploadLocateRequest.FileUploaded;
                }

                Console.WriteLine("Main Exit.");

                Environment.Exit(0);
                
            } catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}



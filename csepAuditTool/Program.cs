using csepAuditTool.Model;
using SimpleLogger;


namespace csepAuditTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var appStartDateTime = DateTime.Now;
                SimpleLog.SetLogFile(logDir: "..\\Logs", prefix: "csepAuditTool_Log_", writeText: false);
                var thisAppStartMessage = String.Format("@@@@@@@@@@@@@@@@@@@@ APPLICATION ENTRY @@@@@@@@@@@@@@@@@@@@ {0}", appStartDateTime.ToString("MM/dd/yyyy h:mm:ss tt"));
                Console.WriteLine(thisAppStartMessage);
                SimpleLog.Info(thisAppStartMessage);

                if (!ProtectConfigurationSectionModel.ProtectConfigurationSection())
                    throw new Exception("UNABLE TO ENCRYPT APP.CONFIG FILE, DEBUG SOURCE. (ProtectConfigurationSectionModel.ProtectConfigurationSection())");

                var ftpConn = new FtpConnectModel();

                if (!ftpConn.CheckValues())
                    throw new Exception("AT LEAST ONE REQUIRED CONFIG VALUE IS MISSING. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.CheckValues())");

                if (!ftpConn.LocalDirectoryExistsCheck())
                    throw new Exception("UNABLE TO READ FROM OR CREATE LOCAL DIRECTORY. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.LocalDirectoryExistsCheck())");

                ftpConn.BuildSftpClientConnection();

                if (!ftpConn.CheckConnection())
                    throw new Exception("UNABLE TO CONNECT TO SFTP WITH EXISTING CONFIGURATION. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.CheckConnection())");

                if (!ftpConn.RemoteDirectoryExists())
                    throw new Exception("REMOTE DIRECTORY DOES NOT EXIST. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.RemoteDirectoryExists())");

                if (!ftpConn.RemoteFileExists())
                    throw new Exception("REMOTE FILE ON SERVER AT PROVIDED PATH MISSING. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.RemoteFileExists())");

                if (!ftpConn.DownloadFile())
                    throw new Exception("NO FILE DOWNLOADED, UNKNOWN ERROR OCCURRED. UPDATE APP.CONFIG TO CONTINUE. (FtpConnectModel.DownloadFile())");

                var incomingValues = new IncomingRowsCollectionModel(ftpConn);

                var outgoingResultMatches = new OutgoingRowsCollectionModel(incomingValues);
                if (outgoingResultMatches.OutgoingRowsCollection.Count == 0) SimpleLog.Info("No Matches Found, Remaining Processes Skipped. (OutgoingRowsCollectionModel())");
                else
                {
                    var uploadLocateRequest = new UploadLocateRequestModel(outgoingResultMatches, ftpConn);

                    if (uploadLocateRequest.NoStringListMatchesCreated) throw new Exception("UNKNOWN ERROR OCCURRED CREATING STRING LIST FROM EXISTING OBJECT. (UploadLocateRequestModel())");
                    if (!uploadLocateRequest.FileUploaded)
                        throw new Exception("LOCATOR RESPONSE FILE NOT UPLOADED, UNKNOWN ERROR. CHECK AND UPDATE APP.CONFIG TO CONTINUE. (UploadLocateRequestModel.BuildUploadContents())");
                }

                var appEndDateTime = DateTime.Now;

                var thisAppSuccessMessage = "-----@@@@@@@@@@@@@@@@@@@@ ALL PROCESSES COMPLETED SUCCESSFULLY @@@@@@@@@@@@@@@@@@@@-----";
                Console.WriteLine(thisAppSuccessMessage);
                SimpleLog.Info(thisAppSuccessMessage);

                var appTime = appEndDateTime - appStartDateTime;
                var thisAppTimeMessage = String.Format("@@@@@@@@@@@@@@@@@@@@ Application Total Run Time: {0} @@@@@@@@@@@@@@@@@@@@", appTime);
                Console.WriteLine(thisAppTimeMessage);
                SimpleLog.Info(thisAppTimeMessage);

                var thisAppEndMessage = String.Format("@@@@@@@@@@@@@@@@@@@@ APPLICATION EXIT @@@@@@@@@@@@@@@@@@@@ {0}", appEndDateTime.ToString("MM/dd/yyyy h:mm:ss tt"));
                Console.WriteLine(thisAppEndMessage);
                SimpleLog.Info(thisAppEndMessage);


            }
            catch (Exception ex)
            {
                SimpleLog.Error("EXEPTION HANDLED AND THROWN.");
                SimpleLog.Log(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}



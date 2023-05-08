using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace SharedLib
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ILogger<CommandExecutor> _logger;
        public CommandExecutor(ILogger<CommandExecutor> logger) 
        {
            _logger = logger;
        }
        public void SshExecutePasswordAuth(string hostAddress, string username, string password, string command)
        {
            try
            {
                using (SshClient sshClient = new SshClient(new PasswordConnectionInfo(hostAddress, username, password)))
                {
                    sshClient.Connect();
                    var sshStream = sshClient.CreateShellStream("", 80, 40, 640, 400, 1024);
                    if (sshClient.IsConnected)
                    {
                        sshStream.WriteLine(command);
                        if (sshStream.DataAvailable)
                        {
                            string line;
                            while ((line = sshStream.ReadLine(TimeSpan.FromSeconds(2))) != null)
                            {
                                //_logger.LogInformation(line);
                                Console.WriteLine(line);
                            }
                        }
                        sshStream.Close();
                        sshClient.Disconnect();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SshExecutePasswordAuthWithoutShell(string hostAddress, string username, string password, string command)
        {
            try
            {
                using (SshClient sshClient = new SshClient(new PasswordConnectionInfo(hostAddress, username, password)))
                {
                    sshClient.Connect();
                    if (sshClient.IsConnected)
                    {
                        _logger.LogInformation("SSH command output:\n{0}", sshClient.CreateCommand(command).Execute());
                        sshClient.Disconnect();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SshExecutePubKeyAuth(string hostAddress, string username, string keyPath, string command, string keyPassword = "")
        {
            try
            {
                if (!File.Exists(keyPath))
                {
                    _logger.LogError("Key file not found");
                }
                else
                {
                    PrivateKeyFile keyFile;
                    if (!string.IsNullOrEmpty(keyPassword))
                    {
                        keyFile = new PrivateKeyFile(keyPassword);
                    }
                    else
                    {
                        keyFile = new PrivateKeyFile(keyPath);
                    }

                    using (SshClient sshClient = new SshClient(new PrivateKeyConnectionInfo(hostAddress, username, keyFile)))
                    {
                        sshClient.Connect();
                        var sshStream = sshClient.CreateShellStream("", 80, 40, 640, 400, 1024);
                        if (sshClient.IsConnected)
                        {
                            sshStream.WriteLine(command);
                            if (sshStream.DataAvailable)
                            {
                                string line;
                                while ((line = sshStream.ReadLine(TimeSpan.FromSeconds(2))) != null)
                                {
                                    //_logger.LogInformation(line);
                                    Console.WriteLine(line);
                                }
                            }
                            sshStream.Close();
                            sshClient.Disconnect();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SshExecutePubKeyAuthWithoutShell(string hostAddress, string username, string keyPath, string command, string keyPassword = "")
        {
            try
            {
                if (!File.Exists(keyPath))
                {
                    _logger.LogError("Key file not found");
                }
                else
                {
                    PrivateKeyFile keyFile;
                    if (!string.IsNullOrEmpty(keyPassword))
                    {
                        keyFile = new PrivateKeyFile(keyPassword);
                    }
                    else
                    {
                        keyFile = new PrivateKeyFile(keyPath);
                    }

                    using (SshClient sshClient = new SshClient(new PrivateKeyConnectionInfo(hostAddress, username, keyFile)))
                    {
                        sshClient.Connect();
                        if (sshClient.IsConnected)
                        {
                            _logger.LogInformation("SSH command output:\n{0}", sshClient.CreateCommand(command).Execute());
                            sshClient.Disconnect();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public interface ICommandExecutor
    {
        public void SshExecutePasswordAuth(string hostAddress, string username, string password, string command);
        public void SshExecutePasswordAuthWithoutShell(string hostAddress, string username, string password, string command);
        public void SshExecutePubKeyAuth(string hostAddress, string username, string keyPath, string command, string keyPassword = "");
        public void SshExecutePubKeyAuthWithoutShell(string hostAddress, string username, string keyPath, string command, string keyPassword = "");
    }
}

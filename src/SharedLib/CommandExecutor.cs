using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Security.Cryptography;

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
                        _logger.LogInformation("Executing command:\n{0}", command);
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
                        _logger.LogInformation("Executing command:\n{0}", command);
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
        public void SshExecutePubKeyAuth(string hostAddress, string username, string keyPath, string command, bool autoContinue, string keyPassword = "")
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
                    var sshTask = Task.Run(() => {

                        using (SshClient sshClient = new SshClient(new PrivateKeyConnectionInfo(hostAddress, username, keyFile)))
                        {
                            sshClient.Connect();
                            var sshStream = sshClient.CreateShellStream("", 80, 40, 640, 400, 1024);
                            if (sshClient.IsConnected)
                            {
                                sshStream.WriteLine(command);

                                //var read = sshStream.ReadAsync(1024)
                                bool commandStillRunning = true;
                                while (sshStream.DataAvailable || commandStillRunning)
                                {
                                    string line;
                                    while ((line = sshStream.ReadLine(TimeSpan.FromMinutes(1))) != null)
                                    {
                                        //_logger.LogInformation(line);
                                        Console.WriteLine(line);
                                    }
                                    if (commandStillRunning)
                                    {
                                        Console.WriteLine("\nCommand still running? (y/n)");
                                        string? keyPress = Console.ReadLine();
                                        if (keyPress != null)
                                        {
                                            switch (keyPress)
                                            {
                                                case "y":
                                                    continue;
                                                case "Y":
                                                    continue;
                                                default:
                                                    commandStillRunning = false;
                                                    break;
                                            }
                                        }
                                    }
                                }
                                sshStream.Close();
                            }
                            sshClient.Disconnect();
                        }

                    });
                    sshTask.Wait();
                    if (!autoContinue)
                    {
                        Console.WriteLine("\nContinue? (y/n)");
                        string? keyPress = Console.ReadLine();

                        if (keyPress != null)
                        {
                            switch (keyPress)
                            {
                                case "y":
                                    break;
                                case "Y":
                                    break;
                                default:
                                    Environment.Exit(1);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SshExecutePubKeyAuthWithoutShell(string hostAddress, string username, string keyPath, string command, bool autoContinue, string keyPassword = "")
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
                            _logger.LogInformation("Executing command:\n{0}", command);

                            var sshCommand = sshClient.CreateCommand(command);

                            var async = sshCommand.BeginExecute();

                            while (!async.IsCompleted)
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(10));
                            }
                            _logger.LogInformation("SSH command output:\n{0}",sshCommand.EndExecute(async));
                            sshClient.Disconnect();
                            if (!autoContinue)
                            {
                                Console.WriteLine("\nContinue? (y/n)");
                                string? keyPress = Console.ReadLine();

                                if (keyPress != null)
                                {
                                    switch (keyPress)
                                    {
                                        case "y":
                                            break;
                                        case "Y":
                                            break;
                                        default:
                                            Environment.Exit(1);
                                            break;
                                    }
                                }
                            }
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
        public void SshExecutePubKeyAuth(string hostAddress, string username, string keyPath, string command, bool autoContinue, string keyPassword = "");
        public void SshExecutePubKeyAuthWithoutShell(string hostAddress, string username, string keyPath, string command, bool autoContinue, string keyPassword = "");
    }
}

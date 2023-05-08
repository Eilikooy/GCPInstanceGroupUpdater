using CommandLine;

namespace SharedLib.Models
{
    public class CommandLineArguments
    {
        [Option('f', "friendlyname", Required = true, HelpText = "Entrys DatabaseName friendly name")]
        public string FriendlyName { get; set; } = string.Empty;

        [Option('u', "sshusername", Required = true, HelpText = "Username for SSH login")]
        public string SshUsername { get; set; } = string.Empty;

        [Option('p', "sshpassword", Required = false, SetName = "sshcredential", HelpText = "Username for SSH login")]
        public string SSHPassword { get; set; } = string.Empty;

        [Option('k', "sshkeyfile", Required = true, SetName = "sshcredential", HelpText = "SSH key file for SSH login")]
        public string SshKeyFile { get; set; } = string.Empty;

        [Option('c', "autoContinue", Required = false, HelpText = "Auto continue")]
        public bool AutoContinue { get; set; } = false;
    }
}

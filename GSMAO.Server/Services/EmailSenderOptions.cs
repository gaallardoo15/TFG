using System.Text.RegularExpressions;

namespace GSMAO.Server.Services
{
    public class EmailSenderOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private string _smtpConfig { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public string smtpConfig
        {
            get => _smtpConfig;
            set
            {
                _smtpConfig = value;

                // smtpConfig is in username:password@localhost:1025 format; extract the part
                var smtpConfigPartsRegEx = new Regex(@"(.*)\:(.*)\|(.+)\:(.+)");
                var smtpConfigPartsMatch = smtpConfigPartsRegEx.Match(value);

                Username = smtpConfigPartsMatch.Groups[1].Value;
                Password = smtpConfigPartsMatch.Groups[2].Value;
                Host = smtpConfigPartsMatch.Groups[3].Value;
                Port = Convert.ToInt32(smtpConfigPartsMatch.Groups[4].Value);
            }
        }

        public required string EmailFromName { get; set; }
        public required string EmailFromAddress { get; set; }
        public bool EnableSSL { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Host { get; set; }
        public int Port { get; protected set; }
    }
}

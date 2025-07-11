using Microsoft.Extensions.Options;

namespace Grc.Middleware.Api.Utils {

    public class ServiceLogger : IServiceLogger {
        private readonly IEnvironmentProvider _environement;
        private readonly LoggingOptions _options;
        private readonly string _fileName;
        private readonly string _folderName;
        public string Id { set; get; }
        public string Channel { set; get; }

        public ServiceLogger(string name) {
            _fileName = name;
            _folderName = "grc_middleware";
        }

        public ServiceLogger(IEnvironmentProvider environement, IOptions<LoggingOptions> loggingOptions, string name) {
            _environement = environement;
            _options = loggingOptions.Value;
            _fileName = string.IsNullOrWhiteSpace(name) ? _options.DefaultLogName : name;
            _folderName = _options.LogFolder;
        }

        public void LogActivity(string message, string type = "MSG") {
            string filepath;
            EventWaitHandle waitHandle = null;

            // ... processing time
            var date = DateTime.Now;
            var shortDate2 = date.ToString("yyyy-MM-dd");
            try {
                waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, $"grc_{shortDate2}");
                waitHandle.WaitOne();

                char[] delim = { ' ' };
                var tZone = TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time");
                var words = tZone.StandardName.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                string abbrev = string.Empty;
                foreach (string chaStr in words) {
                    abbrev += chaStr[0];
                }

                string filename = $"grc_{_fileName}_{shortDate2}.txt";
                filepath = _environement.IsLive ? $@"E:\Toran\Activity_Logs\{_folderName}": $@"C:\Toran\Activity_Logs\{_folderName}";

                //..create directory if not found
                if (!Directory.Exists(filepath)) {
                    Directory.CreateDirectory(filepath);
                }

                //..file path
                filepath = $@"{filepath}\{filename}";
                if (!File.Exists(filepath)) {
                    File.Create(filepath).Close();
                }

                var timeIn = $"{date:yyyy.MM.dd HH:mm:ss} {abbrev}";
                string messageToLog = $"[{timeIn}]: [{type}]\t\t {(Channel != null ? $"CHANNEL: {Channel}\t" : "")} {(Id != null ? Id + "\t" : "")} {message}";
                using var file = new StreamWriter(filepath, true);
                file.WriteLine(messageToLog);


            } catch (Exception ex) {

                try {
                    string filename = $"grc_{_fileName}_{shortDate2}.txt";
                    filepath = _environement.IsLive ? $@"E:\Toran\Activity_Logs\{_folderName}": $@"C:\Toran\Activity_Logs\{_folderName}";
                    if (!Directory.Exists(filepath)) {
                        Directory.CreateDirectory(filepath);
                    }

                    using var file = new StreamWriter(filepath, true);
                    file.WriteLine($"LOG ERROR :: An error occurred while creating log file.");
                    file.WriteLine($"{ex.Message}");

                } catch (Exception ex1) {

                    try {
                        string filename = $"grc_{_fileName}_{shortDate2}.txt";
                        filepath = _environement.IsLive ? $@"E:\Toran\Activity_Logs\{_folderName}": $@"C:\Toran\Activity_Logs\{_folderName}";
                        if (!Directory.Exists(filepath)) {
                            Directory.CreateDirectory(filepath);
                        }

                        using var file = new StreamWriter(filepath, true);
                        file.WriteLine($"LOG ERROR :: An error occurred while creating log file.");
                        file.WriteLine($"{ex1.Message}");
                    } catch (Exception) {
                        return;
                    }

                }
            } finally {
                waitHandle?.Set();
            }
        }
    }

}

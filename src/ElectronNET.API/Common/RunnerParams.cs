namespace ElectronNET.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;

    public sealed class RunnerParams
    {
        private string fileName;
        private string arguments;
        private string directory;
        private string userName;
        private string verb;
        private ProcessWindowStyle windowStyle;

        /// <summary>
        ///     Default constructor.  At least the <see cref='ProcessStartInfo.FileName'/>
        ///     property must be set before starting the process.
        /// </summary>
        public RunnerParams()
        {
        }

        /// <summary>
        ///     Specifies the name of the application or document that is to be started.
        /// </summary>
        public RunnerParams(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        ///     Specifies the name of the application that is to be started, as well as a set
        ///     of command line arguments to pass to the application.
        /// </summary>
        public RunnerParams(string fileName, string arguments)
        {
            this.fileName = fileName;
            this.arguments = arguments;
        }

        /// <summary>
        ///     Specifies the set of command line arguments to use when starting the application.
        /// </summary>
        public string Arguments
        {
            get
            {
                return this.arguments ?? string.Empty;
            }

            set
            {
                this.arguments = value;
            }
        }

        public bool CreateNoWindow { get; set; }

        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        public bool RedirectStandardInput { get; set; }

        public bool RedirectStandardOutput { get; set; }

        public bool RedirectStandardError { get; set; }

        public Encoding StandardInputEncoding { get; set; }

        public Encoding StandardErrorEncoding { get; set; }

        public Encoding StandardOutputEncoding { get; set; }

        /// <summary>
        ///    <para>
        ///       Returns or sets the application, document, or URL that is to be launched.
        ///    </para>
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName ?? string.Empty;
            }

            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        ///     Returns or sets the initial directory for the process that is started.
        ///     Specify "" to if the default is desired.
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                return this.directory ?? string.Empty;
            }

            set
            {
                this.directory = value;
            }
        }

        public bool ErrorDialog { get; set; }

        public IntPtr ErrorDialogParentHandle { get; set; }

        public string UserName
        {
            get
            {
                return this.userName ?? string.Empty;
            }

            set
            {
                this.userName = value;
            }
        }

        [DefaultValue("")]
        public string Verb
        {
            get
            {
                return this.verb ?? string.Empty;
            }

            set
            {
                this.verb = value;
            }
        }

        [DefaultValue(ProcessWindowStyle.Normal)]
        public ProcessWindowStyle WindowStyle
        {
            get
            {
                return this.windowStyle;
            }

            set
            {
                if (!Enum.IsDefined(typeof(ProcessWindowStyle), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ProcessWindowStyle));
                }

                this.windowStyle = value;
            }
        }

        public bool UseShellExecute { get; set; }
    }
}
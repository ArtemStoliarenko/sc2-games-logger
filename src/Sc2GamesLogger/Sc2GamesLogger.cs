using Sc2GamesLogger.Core;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Sc2GamesLogger
{
    public partial class Sc2GamesLogger : Form
    {
        private const string peridodConfigKey = "UpdatePeriod";

        private const string timeoutConfigKey = "Timeout";

        private FileLogger fileLogger;

        public Sc2GamesLogger()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ChangeButtonsState(true);

                string path = sfd.FileName;
                int period = int.Parse(ConfigurationManager.AppSettings[peridodConfigKey]);
                int timeout = int.Parse(ConfigurationManager.AppSettings[timeoutConfigKey]);

                fileLogger?.Dispose();
                fileLogger = new FileLogger(path, period, timeout);
                fileLogger.Start();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            fileLogger.Stop();
            ChangeButtonsState(false);
        }

        private void ChangeButtonsState(bool loggingEnabled)
        {
            btnStart.Enabled = !loggingEnabled;
            btnStop.Enabled = loggingEnabled;
        }
    }
}

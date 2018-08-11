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

        private const string timerFormat = @"hh\:mm\:ss";

        private const string defaultTimerValue = "00:00:00";

        private static readonly TimeSpan timerIncrement = TimeSpan.FromSeconds(1);

        private FileLogger fileLogger;

        private TimeSpan timerTime;

        private bool LoggingEnabled => btnStop.Enabled;

        public Sc2GamesLogger()
        {
            InitializeComponent();
            this.ActiveControl = null;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ChangeState(true);

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
            ChangeState(false);
        }

        private void ChangeState(bool loggingEnabled)
        {
            btnStart.Enabled = !loggingEnabled;
            btnStop.Enabled = loggingEnabled;
            textboxTimer.Enabled = loggingEnabled;

            if (loggingEnabled)
            {
                timerTime = TimeSpan.Zero;
                tbTimer.Text = defaultTimerValue;
                this.ActiveControl = null;
            }
        }

        private void timerText_Tick(object sender, EventArgs e)
        {
            timerTime += timerIncrement;
            tbTimer.Text = timerTime.ToString(timerFormat);
        }
    }
}

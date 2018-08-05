using Sc2GamesLogger.Core;
using System;
using System.Windows.Forms;

namespace Sc2GamesLogger
{
    public partial class Sc2GamesLogger : Form
    {
        private readonly FileLogger fileLogger = new FileLogger("test.txt", 20, 10);

        public Sc2GamesLogger()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            fileLogger.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            fileLogger.Stop();
        }
    }
}

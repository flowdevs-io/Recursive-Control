using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision
{
    public partial class OmniParserForm : Form
    {
        private OmniParserConfig _config;

        public OmniParserForm()
        {
            InitializeComponent();
            _config = OmniParserConfig.LoadConfig();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string url = omniParserServerURL.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            // Save the URL to the config file
            _config.ServerURL = url;
            _config.SaveConfig();

            // Optionally, you can close the form after saving
            this.Close();
        }

        private void OmniParserForm_Load(object sender, EventArgs e)
        {
            // Load the URL from the config file
            omniParserServerURL.Text = _config.ServerURL;
        }
    }
}

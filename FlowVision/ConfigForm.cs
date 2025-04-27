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
    public partial class ConfigForm: Form
    {
        private string currentModel;
        public ConfigForm(string model)
        {
            currentModel = model;
            InitializeComponent();

            //load config
            APIConfig config = APIConfig.LoadConfig(currentModel);

            deploymentNameTextBox.Text = config.DeploymentName;
            endpointURLTextBox.Text = config.EndpointURL;
            apiKeyTextBox.Text = config.APIKey;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //save config
            APIConfig config = new APIConfig
            {
                DeploymentName = deploymentNameTextBox.Text,
                EndpointURL = endpointURLTextBox.Text,
                APIKey = apiKeyTextBox.Text
            };

            config.SaveConfig(currentModel);

            // Close the form
            this.Close();
        }

    }
}

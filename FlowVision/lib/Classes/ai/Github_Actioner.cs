using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using FlowVision.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FlowVision.lib.Classes
{
	public class Github_Actioner
	{
		private readonly IChatCompletionService _chatService;
		private readonly ChatHistory _history;
		private readonly Kernel _kernel;
		private readonly RichTextBox _outputBox;
		private readonly PromptExecutionSettings _settings;

		private const string CONFIG_SECTION = "actioner";

		public Github_Actioner(RichTextBox outputTextBox)
		{

		}

	}
}

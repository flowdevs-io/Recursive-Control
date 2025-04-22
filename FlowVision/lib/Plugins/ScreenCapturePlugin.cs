using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    /* 
         * This class is preserved for backward compatibility.
         * It delegates to the new split classes: WindowSelectionPlugin and ScreenCaptureOmniParserPlugin.
         * Consider updating references to use the new plugins directly.
         */
    internal class ScreenCapturePlugin
    {
        private readonly WindowSelectionPlugin _windowSelector;
        private readonly ScreenCaptureOmniParserPlugin _screenCaptureOmniParser;

        public ScreenCapturePlugin()
        {
            _windowSelector = new WindowSelectionPlugin();
            _screenCaptureOmniParser = new ScreenCaptureOmniParserPlugin();
        }

        [KernelFunction, Description("Used to capture the Screen and return Parsed Content")]
        public async Task<List<ParsedContent>> CaptureScreen(string handleString)
        {
            return await _screenCaptureOmniParser.CaptureScreen(handleString);
        }

        //capture the whole screen
        [KernelFunction, Description("Used to capture the whole screen")]
        public async Task<List<ParsedContent>> CapturewholeScreen()
        {
            return await _screenCaptureOmniParser.CaptureWholeScreen();
        }

        [KernelFunction, Description("Used to set current handle as foreground")]
        public async Task<bool> ForegroundSelect(string handleString)
        {
            return await _windowSelector.ForegroundSelect(handleString);
        }

        [KernelFunction, Description("Returns a list of available window handles, titles, and process names.")]
        public string ListWindowHandles()
        {
            return _windowSelector.ListWindowHandles();
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    /// <summary>
    /// Screen capture plugin - now delegates to WindowSelectionPlugin only.
    /// For web automation, use PlaywrightPlugin instead.
    /// </summary>
    internal class ScreenCapturePlugin
    {
        private readonly WindowSelectionPlugin _windowSelector;

        public ScreenCapturePlugin()
        {
            _windowSelector = new WindowSelectionPlugin();
        }

        [Description("Used to set current handle as foreground")]
        public async Task<bool> ForegroundSelect(string handleString)
        {
            return await _windowSelector.ForegroundSelect(handleString);
        }

        [Description("Returns a list of available window handles, titles, and process names.")]
        public string ListWindowHandles()
        {
            return _windowSelector.ListWindowHandles();
        }
    }
}

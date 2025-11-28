using System;
using System.ComponentModel;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class ClipboardPlugin
    {
        [Description("Sets the system clipboard text content")]
        public void SetClipboardText(string text)
        {
            PluginLogger.LogPluginUsage("ClipboardPlugin", "SetClipboardText", text);
            
            if (Application.OpenForms.Count > 0)
            {
                Application.OpenForms[0].Invoke(new Action(() => {
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch (Exception ex)
                    {
                        PluginLogger.LogError("ClipboardPlugin", "SetClipboardText", ex.Message);
                    }
                }));
            }
        }

        [Description("Gets the current text content from the system clipboard")]
        public string GetClipboardText()
        {
            PluginLogger.LogPluginUsage("ClipboardPlugin", "GetClipboardText");
            string clipboardText = "";
            
            if (Application.OpenForms.Count > 0)
            {
                clipboardText = (string)Application.OpenForms[0].Invoke(new Func<string>(() => {
                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            return Clipboard.GetText();
                        }
                        else
                        {
                            return "[Clipboard is empty or contains non-text data]";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Error reading clipboard: {ex.Message}";
                    }
                }));
            }
            return clipboardText;
        }
    }
}

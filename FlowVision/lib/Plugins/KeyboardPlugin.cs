﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    internal class KeyboardPlugin
    {
        // Constants for keybd_event
        private const byte VK_LWIN = 0x5B;
        private const byte VK_R = 0x52; // 'R' key
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Import keybd_event from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [KernelFunction, Description("Used to interact with the keyboard")]
        public async Task<bool> SendKey(string keyCombo)
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "SendKey", keyCombo);
            try
            {
                SendKeys.SendWait(keyCombo);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [KernelFunction, Description("Enter Key")]
        public async Task<bool> EnterKey()
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "EnterKey", "ENTER");
            try
            {
                SendKeys.SendWait("{ENTER}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [KernelFunction, Description("Ctrl + Letter")]
        public async Task<bool> CtrlKey(string letter)
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "CtrlKey", letter);
            try
            {
                SendKeys.SendWait($"^({letter})");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*
         *
         *[KernelFunction, Description("Alt + F4")]
        public async Task<bool> AltFFour()
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("KeyboardPlugin", "AltFFour", "Alt + F4");
            AppendLog("Sending Key Command: Alt + F4");
            try
            {
                SendKeys.SendWait("%{F4}");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Error Sending Key: {ex.Message}");
                return false;
            }
        }
         */

        /*
        [KernelFunction, Description("Opens the Run dialog (Windows+R)")]
        public async Task<bool> OpenRunDialog()
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("KeyboardPlugin", "OpenRunDialog", "Windows+R");
            AppendLog("Opening Run dialog (Windows+R)");
            try
            {
                // Simulate Windows key down
                keybd_event(VK_LWIN, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                // Simulate 'R' key down
                keybd_event(VK_R, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                // Simulate 'R' key up
                keybd_event(VK_R, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                // Simulate Windows key up
                keybd_event(VK_LWIN, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);

                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Error opening Run dialog: {ex.Message}");
                return false;
            }
        }
        */
    }
}

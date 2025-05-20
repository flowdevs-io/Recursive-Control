using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowVision.lib.Classes;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace FlowVision.Tests
{
    [TestClass]
    public class MultiAgentActionerTests
    {
        private static string CallExtract(string plan)
        {
            var actioner = new MultiAgentActioner(null);
            var method = typeof(MultiAgentActioner).GetMethod("ExtractActionableStep", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method.Invoke(actioner, new object[] { plan });
        }

        [TestMethod]
        public void ExtractActionableStep_ReturnsFirstActionableLine()
        {
            string plan = "1. Use WindowSelectionPlugin to list windows\n2. Use ScreenCapturePlugin to capture";
            string result = CallExtract(plan);
            Assert.AreEqual("Use WindowSelectionPlugin to list windows", result);
        }

        [TestMethod]
        public void ExtractActionableStep_SingleLineActionable()
        {
            string plan = "Use MousePlugin to click start button";
            string result = CallExtract(plan);
            Assert.AreEqual(plan, result);
        }

        [TestMethod]
        public void ExtractActionableStep_ReturnsNullForNonActionable()
        {
            string plan = "Hello there";
            string result = CallExtract(plan);
            Assert.IsNull(result);
        }

        private static string ConfigPath(string name)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision", "Config", $"{name}.json");
        }

        [TestMethod]
        public void PlannerPrompt_ContainsTools_WhenDynamicPromptsEnabled()
        {
            var config = new ToolConfig();
            config.DynamicToolPrompts = true;
            config.EnableCMDPlugin = true; // ensure at least one tool
            config.SaveConfig("toolsconfig");

            var actioner = new MultiAgentActioner(null);
            actioner.SetChatHistory(new List<LocalChatMessage>());

            var field = typeof(MultiAgentActioner).GetField("plannerHistory", BindingFlags.NonPublic | BindingFlags.Instance);
            var history = field.GetValue(actioner);
            var enumerator = ((System.Collections.IEnumerable)history).GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            string content = (string)first.GetType().GetProperty("Content").GetValue(first);

            Assert.IsTrue(content.Contains("You have access to the following tools"));
        }

        [TestMethod]
        public void PlannerPrompt_OmitsTools_WhenDynamicPromptsDisabled()
        {
            var config = new ToolConfig();
            config.DynamicToolPrompts = false;
            config.EnableCMDPlugin = true;
            config.SaveConfig("toolsconfig");

            var actioner = new MultiAgentActioner(null);
            actioner.SetChatHistory(new List<LocalChatMessage>());

            var field = typeof(MultiAgentActioner).GetField("plannerHistory", BindingFlags.NonPublic | BindingFlags.Instance);
            var history = field.GetValue(actioner);
            var enumerator = ((System.Collections.IEnumerable)history).GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            string content = (string)first.GetType().GetProperty("Content").GetValue(first);

            Assert.IsFalse(content.Contains("You have access to the following tools"));
        }
    }
}

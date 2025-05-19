using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowVision.lib.Classes;
using System.Reflection;

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
    }
}

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Markdig;

namespace FlowVision.lib.Classes
{
    public static class MarkdownHelper
    {
        private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public static void ApplyMarkdownFormatting(RichTextBox richTextBox, string markdown)
        {
            // Convert markdown to plain text (we'll handle formatting ourselves)
            string plainText = markdown;

            richTextBox.Text = plainText;
            richTextBox.SelectAll();
            richTextBox.SelectionFont = new Font("Segoe UI", 10F);
            richTextBox.SelectionColor = Color.Black;
            richTextBox.SelectionLength = 0; // Deselect

            // Process headers
            ApplyHeadersFormatting(richTextBox, plainText);

            // Process bold
            ApplyBoldFormatting(richTextBox, plainText);

            // Process italics
            ApplyItalicsFormatting(richTextBox, plainText);

            // Process inline code
            ApplyInlineCodeFormatting(richTextBox, plainText);

            // Process code blocks
            ApplyCodeBlockFormatting(richTextBox, plainText);

            // Process bullet points
            ApplyBulletPointsFormatting(richTextBox, plainText);
        }

        private static void ApplyHeadersFormatting(RichTextBox richTextBox, string text)
        {
            // Headers: # Header 1, ## Header 2, etc.
            var headerRegex = new Regex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline);
            var matches = headerRegex.Matches(text);

            foreach (Match match in matches)
            {
                int level = match.Groups[1].Length; // Number of # characters
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    // Select the header text without the # characters
                    richTextBox.Select(startIndex + level + 1, match.Groups[2].Length);
                    float fontSize = 16 - level; // Decrease size for deeper headers
                    richTextBox.SelectionFont = new Font("Segoe UI", fontSize, FontStyle.Bold);
                }
            }
        }

        private static void ApplyBoldFormatting(RichTextBox richTextBox, string text)
        {
            // Bold: **bold text** or __bold text__
            var boldRegex = new Regex(@"(\*\*|__)(.*?)\1");
            var matches = boldRegex.Matches(text);

            foreach (Match match in matches)
            {
                // Find the text in the RichTextBox
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    string boldText = match.Groups[2].Value;
                    // Select the text including the ** or __
                    richTextBox.Select(startIndex, match.Length);
                    // Replace with just the text (no ** or __)
                    richTextBox.SelectedText = boldText;
                    // Re-select the text and make it bold
                    richTextBox.Select(startIndex, boldText.Length);
                    richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, FontStyle.Bold);
                }
            }
        }

        private static void ApplyItalicsFormatting(RichTextBox richTextBox, string text)
        {
            // Italics: *italic text* or _italic text_
            var italicsRegex = new Regex(@"(\*|_)(.*?)\1");
            var matches = italicsRegex.Matches(text);

            foreach (Match match in matches)
            {
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    string italicText = match.Groups[2].Value;
                    richTextBox.Select(startIndex, match.Length);
                    richTextBox.SelectedText = italicText;
                    richTextBox.Select(startIndex, italicText.Length);
                    richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, FontStyle.Italic);
                }
            }
        }

        private static void ApplyInlineCodeFormatting(RichTextBox richTextBox, string text)
        {
            // Inline code: `code`
            var codeRegex = new Regex(@"`([^`]+)`");
            var matches = codeRegex.Matches(text);

            foreach (Match match in matches)
            {
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    string codeText = match.Groups[1].Value;
                    richTextBox.Select(startIndex, match.Length);
                    richTextBox.SelectedText = codeText;
                    richTextBox.Select(startIndex, codeText.Length);
                    richTextBox.SelectionFont = new Font("Consolas", richTextBox.SelectionFont.Size);
                    richTextBox.SelectionBackColor = Color.LightGray;
                }
            }
        }

        private static void ApplyCodeBlockFormatting(RichTextBox richTextBox, string text)
        {
            // Code blocks: ```code``` or ~~~code~~~
            var codeBlockRegex = new Regex(@"(```|~~~)(.*?)\1", RegexOptions.Singleline);
            var matches = codeBlockRegex.Matches(text);
            foreach (Match match in matches)
            {
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    string codeBlockText = match.Groups[2].Value;
                    richTextBox.Select(startIndex, match.Length);
                    richTextBox.SelectedText = codeBlockText;
                    richTextBox.Select(startIndex, codeBlockText.Length);
                    richTextBox.SelectionFont = new Font("Consolas", richTextBox.SelectionFont.Size);
                    richTextBox.SelectionBackColor = Color.LightGray;
                }
            }
        }

        private static void ApplyBulletPointsFormatting(RichTextBox richTextBox, string text)
        {
            // Bullet points: - item or * item
            var bulletRegex = new Regex(@"^(\*|-)\s+(.+)$", RegexOptions.Multiline);
            var matches = bulletRegex.Matches(text);
            foreach (Match match in matches)
            {
                int startIndex = richTextBox.Text.IndexOf(match.Value);
                if (startIndex >= 0)
                {
                    // Select the bullet point text
                    richTextBox.Select(startIndex, match.Length);
                    richTextBox.SelectionIndent = 20; // Indent for bullet points
                }
            }
        }

        internal static bool ContainsMarkdown(string message)
        {
            // Check for common Markdown patterns
            return Regex.IsMatch(message, @"(\*\*|__|`|~~|\*|-|\#)", RegexOptions.Multiline);
        }
    }
}
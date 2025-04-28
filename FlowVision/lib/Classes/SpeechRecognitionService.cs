using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowVision.lib.Classes
{
    public class SpeechRecognitionService : IDisposable
    {
        private SpeechRecognitionEngine recognizer;
        private bool isListening = false;
        
        public event EventHandler<string> SpeechRecognized;
        public event EventHandler<string> CommandRecognized; // New event for voice commands

        public SpeechRecognitionService()
        {
            try
            {
                var toolConfig = ToolConfig.LoadConfig("toolsconfig");
                
                // Get the installed recognizers
                var recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers()
                    .FirstOrDefault(r => r.Culture.Name == toolConfig.SpeechRecognitionLanguage) ?? 
                    SpeechRecognitionEngine.InstalledRecognizers().First();
                
                // Create a new recognition engine
                recognizer = new SpeechRecognitionEngine(recognizerInfo);
                
                // Configure the recognizer with grammar - using dictation grammar for free speech
                recognizer.LoadGrammar(new DictationGrammar());
                
                // Add command grammar for send message functionality
                if (!string.IsNullOrEmpty(toolConfig.VoiceCommandPhrase))
                {
                    AddCommandGrammar(toolConfig.VoiceCommandPhrase);
                }
                
                // Set up event handlers
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                recognizer.RecognizeCompleted += Recognizer_RecognizeCompleted;
                
                // Set input device to default audio device
                recognizer.SetInputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                // Rethrow to be handled by the caller
                throw new Exception("Failed to initialize speech recognition: " + ex.Message, ex);
            }
        }

        private void AddCommandGrammar(string commandPhrase)
        {
            try
            {
                // Create a Choices object with the command phrase
                Choices commands = new Choices();
                commands.Add(commandPhrase);

                // Build a grammar from the choices
                GrammarBuilder grammarBuilder = new GrammarBuilder();
                grammarBuilder.Append(commands);
                Grammar grammar = new Grammar(grammarBuilder);
                grammar.Name = "CommandGrammar";

                // Add the grammar to the recognizer
                recognizer.LoadGrammar(grammar);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add command grammar: " + ex.Message, ex);
            }
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Make sure confidence is at a reasonable level
            if (e.Result.Confidence >= 0.6)
            {
                // Check if this is a command phrase
                var toolConfig = ToolConfig.LoadConfig("toolsconfig");
                if (!string.IsNullOrEmpty(toolConfig.VoiceCommandPhrase) && 
                    string.Equals(e.Result.Text, toolConfig.VoiceCommandPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    // It's a command, trigger command event
                    CommandRecognized?.Invoke(this, e.Result.Text);
                }
                else
                {
                    // Regular speech, trigger normal event
                    SpeechRecognized?.Invoke(this, e.Result.Text);
                }
            }
        }

        private void Recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            // Handle recognition completion
            if (e.Error != null)
            {
                // Notify of error
                MessageBox.Show($"Speech recognition error: {e.Error.Message}", "Recognition Error");
            }
            
            isListening = false;
        }

        public void StartListening()
        {
            if (recognizer != null && !isListening)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                isListening = true;
            }
        }

        public void StopListening()
        {
            if (recognizer != null && isListening)
            {
                recognizer.RecognizeAsyncStop();
                isListening = false;
            }
        }

        public void UpdateCommandPhrase(string phrase)
        {
            if (recognizer != null && !string.IsNullOrEmpty(phrase))
            {
                // Remove old command grammar if it exists
                var existingGrammar = recognizer.Grammars.FirstOrDefault(g => g.Name == "CommandGrammar");
                if (existingGrammar != null)
                {
                    recognizer.UnloadGrammar(existingGrammar);
                }

                // Add new command grammar
                AddCommandGrammar(phrase);
            }
        }

        public void Dispose()
        {
            if (recognizer != null)
            {
                StopListening();
                recognizer.Dispose();
                recognizer = null;
            }
        }
    }
}

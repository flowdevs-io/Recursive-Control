using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using FlowVision.lib.Classes;
using Microsoft.Playwright;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Threading;
using System.Text.Json;

namespace FlowVision.lib.Plugins
{
    /// <summary>
    /// Playwright plugin for browser automation within FlowVision.
    /// </summary>
    internal class PlaywrightPlugin : IAsyncDisposable
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private bool _initialized = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private string _currentSessionId = "default";
        private bool _useSession = true;
        
        /// <summary>
        /// Initializes the Playwright environment if not already done.
        /// </summary>
        private async Task InitializePlaywrightAsync()
        {
            if (_initialized) return;
            
            await _semaphore.WaitAsync();
            
            try
            {
                if (!_initialized)
                {
                    PluginLogger.LogPluginUsage("PlaywrightPlugin", "Initialize");
                    PluginLogger.NotifyTaskStart("Playwright initialization", "Setting up browser automation environment");
                    
                    _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                    _initialized = true;
                    
                    PluginLogger.NotifyTaskComplete("Playwright initialization");
                }
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Playwright initialization", false);
                throw new Exception($"Failed to initialize Playwright: {ex.Message}", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Sets the active session ID for browser operations.
        /// </summary>
        [KernelFunction, Description("Sets the active session ID for browser operations")]
        public string SetSessionId(
            [Description("Session ID to use")] string sessionId = "default")
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "SetSessionId", $"SessionId: {sessionId}");
            
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = "default";
            }
            
            _currentSessionId = sessionId;
            return $"Session ID set to: {sessionId}";
        }
        
        /// <summary>
        /// Enables or disables session persistence.
        /// </summary>
        [KernelFunction, Description("Enables or disables session persistence")]
        public string EnableSessionPersistence(
            [Description("Enable session persistence (true/false)")] string enable = "true")
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "EnableSessionPersistence", $"Enable: {enable}");
            
            bool enableSession = string.IsNullOrEmpty(enable) ? true : bool.Parse(enable);
            _useSession = enableSession;
            
            return $"Session persistence set to: {enableSession}";
        }
        
        /// <summary>
        /// Launches a browser with the specified options.
        /// </summary>
        [KernelFunction, Description("Launches a new browser instance")]
        public async Task<string> LaunchBrowser(
            [Description("Browser type (chromium, firefox, webkit)")] string browserType = "chromium",
            [Description("Launch as headless browser (true/false)")] string headless = "true")
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "LaunchBrowser", $"Type: {browserType}, Headless: {headless}");
            PluginLogger.NotifyTaskStart("Browser launch", $"Starting {browserType} browser");
            
            await InitializePlaywrightAsync();
            
            try
            {
                // Close existing browser if one is open
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    await _browser.DisposeAsync();
                    _browser = null;
                }
                
                // Parse headless parameter
                bool isHeadless = string.IsNullOrEmpty(headless) ? true : bool.Parse(headless);
                
                // Launch browser based on type
                switch (browserType.ToLower())
                {
                    case "firefox":
                        _browser = await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = isHeadless });
                        break;
                    case "webkit":
                        _browser = await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = isHeadless });
                        break;
                    default:
                        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = isHeadless });
                        break;
                }
                
                // Check if we should restore a session
                if (_useSession)
                {
                    var storageState = PlaywrightSessionManager.GetStorageState(_currentSessionId);
                    if (storageState != null)
                    {
                        PluginLogger.LogPluginUsage("PlaywrightPlugin", "RestoreSession", $"Restoring session: {_currentSessionId}");
                        
                        var storageFile = await SaveStorageStateToTempFile(storageState);
                        
                        // Create context with storage state
                        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                        {
                            StorageStatePath = storageFile
                        });
                        
                        // Clean up temp file
                        try { File.Delete(storageFile); } catch { /* Ignore errors */ }
                    }
                    else
                    {
                        // Create new context
                        _context = await _browser.NewContextAsync();
                    }
                }
                else
                {
                    // Create new context without session
                    _context = await _browser.NewContextAsync();
                }
                
                // Create a new page
                _page = await _context.NewPageAsync();
                
                PluginLogger.NotifyTaskComplete("Browser launch");
                return $"Successfully launched {browserType} browser";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Browser launch", false);
                return $"Error launching browser: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Saves the current browser session for future use.
        /// </summary>
        [KernelFunction, Description("Saves the current browser session for future use")]
        public async Task<string> SaveSession()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "SaveSession", $"SessionId: {_currentSessionId}");
            
            try
            {
                if (_context == null)
                {
                    return "Error: Browser context not available. Launch browser first.";
                }
                
                PluginLogger.NotifyTaskStart("Session save", $"Saving session: {_currentSessionId}");
                
                // Get storage state from current context
                var storageState = await _context.StorageStateAsync();
                
                // Save session state
                PlaywrightSessionManager.SaveStorageState(_currentSessionId, storageState);
                
                PluginLogger.NotifyTaskComplete("Session save");
                return $"Session saved successfully with ID: {_currentSessionId}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Session save", false);
                return $"Error saving session: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Navigates to the specified URL.
        /// </summary>
        [KernelFunction, Description("Navigates to a specified URL")]
        public async Task<string> NavigateTo(
            [Description("URL to navigate to")] string url)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "NavigateTo", $"URL: {url}");
            
            try
            {
                if (_page == null)
                {
                    return "Error: Browser not launched. Call LaunchBrowser first.";
                }
                
                PluginLogger.NotifyTaskStart("Navigation", $"Navigating to {url}");
                
                // Ensure URL has protocol
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                
                // Navigate to the URL and wait for network to be idle
                var response = await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
                
                // Save session state after navigation if enabled
                if (_useSession)
                {
                    await SaveSession();
                }
                
                PluginLogger.NotifyTaskComplete("Navigation");
                return $"Successfully navigated to {url}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Navigation", false);
                return $"Error navigating to {url}: {ex.Message}";
            }
        }

        // Helper method to save storage state to a temporary file
        private async Task<string> SaveStorageStateToTempFile(string storageState)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            File.WriteAllText(tempFile, storageState);
            return tempFile;
        }
        
        /// <summary>
        /// Takes a screenshot of the current page.
        /// </summary>
        [KernelFunction, Description("Takes a screenshot of the current page")]
        public async Task<string> TakeScreenshot(
            [Description("Path where the screenshot should be saved")] string path = null,
            [Description("CSS selector to screenshot specific element")] string selector = null)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "TakeScreenshot");
            
            try
            {
                if (_page == null)
                {
                    return "Error: Browser not launched. Call LaunchBrowser first.";
                }
                
                PluginLogger.NotifyTaskStart("Screenshot", "Capturing screenshot");
                
                // Generate a default path if not provided
                if (string.IsNullOrEmpty(path))
                {
                    string screenshotDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "FlowVision", "Screenshots");
                    
                    Directory.CreateDirectory(screenshotDir);
                    path = Path.Combine(screenshotDir, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                }
                
                // Take screenshot of specific element or full page
                if (!string.IsNullOrEmpty(selector))
                {
                    var element = await _page.QuerySelectorAsync(selector);
                    if (element == null)
                    {
                        PluginLogger.NotifyTaskComplete("Screenshot", false);
                        return $"Error: Could not find element with selector: {selector}";
                    }
                    
                    await element.ScreenshotAsync(new ElementHandleScreenshotOptions
                    {
                        Path = path
                    });
                }
                else
                {
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = path,
                        FullPage = true
                    });
                }
                
                PluginLogger.NotifyTaskComplete("Screenshot");
                return $"Screenshot saved to: {path}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Screenshot", false);
                return $"Error taking screenshot: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Clicks on an element identified by a selector and automatically saves session state.
        /// </summary>
        [KernelFunction, Description("Clicks on an element identified by CSS selector")]
        public async Task<string> ClickElement(
            [Description("CSS selector for the element to click")] string selector)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "ClickElement", $"Selector: {selector}");
            
            try
            {
                if (_page == null)
                {
                    return "Error: Browser not launched. Call LaunchBrowser first.";
                }
                
                if (string.IsNullOrEmpty(selector))
                {
                    return "Error: Selector cannot be empty";
                }
                
                PluginLogger.NotifyTaskStart("Click interaction", $"Clicking on element: {selector}");
                
                await _page.ClickAsync(selector);
                
                // Save session state after clicking if enabled
                if (_useSession)
                {
                    await SaveSession();
                }
                
                PluginLogger.NotifyTaskComplete("Click interaction");
                return $"Successfully clicked on element: {selector}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Click interaction", false);
                return $"Error clicking element: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Types text into an input field identified by a selector and automatically saves session state.
        /// </summary>
        [KernelFunction, Description("Types text into an input field identified by CSS selector")]
        public async Task<string> TypeText(
            [Description("CSS selector for the input field")] string selector,
            [Description("Text to type into the field")] string text)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "TypeText", $"Selector: {selector}, Text length: {text?.Length ?? 0}");
            
            try
            {
                if (_page == null)
                {
                    return "Error: Browser not launched. Call LaunchBrowser first.";
                }
                
                if (string.IsNullOrEmpty(selector))
                {
                    return "Error: Selector cannot be empty";
                }
                
                PluginLogger.NotifyTaskStart("Text input", $"Typing into field: {selector}");
                
                // Clear the field first
                await _page.FillAsync(selector, "");
                
                // Type the text
                await _page.TypeAsync(selector, text);
                
                // Save session state after typing if enabled
                if (_useSession)
                {
                    await SaveSession();
                }
                
                PluginLogger.NotifyTaskComplete("Text input");
                return $"Successfully typed text into: {selector}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Text input", false);
                return $"Error typing text: {ex.Message}";
            }
        }

        /// <summary>
        /// Lists all available saved sessions.
        /// </summary>
        [KernelFunction, Description("Lists all available saved browser sessions")]
        public string ListSessions()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "ListSessions");
            
            try
            {
                var sessions = PlaywrightSessionManager.GetAllSessions();
                
                if (sessions.Count == 0)
                {
                    return "No saved sessions found.";
                }
                
                return $"Available sessions ({sessions.Count}):\n" + string.Join("\n", sessions);
            }
            catch (Exception ex)
            {
                return $"Error listing sessions: {ex.Message}";
            }
        }

        /// <summary>
        /// Deletes a saved session.
        /// </summary>
        [KernelFunction, Description("Deletes a saved browser session")]
        public string DeleteSession(
            [Description("ID of the session to delete")] string sessionId = null)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "DeleteSession", $"SessionId: {sessionId ?? _currentSessionId}");
            
            try
            {
                string id = string.IsNullOrEmpty(sessionId) ? _currentSessionId : sessionId;
                
                if (PlaywrightSessionManager.DeleteSession(id))
                {
                    return $"Session '{id}' deleted successfully.";
                }
                else
                {
                    return $"Session '{id}' not found.";
                }
            }
            catch (Exception ex)
            {
                return $"Error deleting session: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Closes the browser and releases all resources.
        /// </summary>
        [KernelFunction, Description("Closes the browser and releases resources")]
        public async Task<string> CloseBrowser()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "CloseBrowser");
            
            try
            {
                if (_browser == null)
                {
                    return "Browser was not running";
                }
                
                PluginLogger.NotifyTaskStart("Browser cleanup", "Closing browser and resources");
                
                // Save session state before closing if enabled
                if (_useSession && _context != null)
                {
                    await SaveSession();
                }
                
                await _browser.CloseAsync();
                _browser = null;
                _context = null;
                _page = null;
                
                PluginLogger.NotifyTaskComplete("Browser cleanup");
                return "Browser successfully closed";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Browser cleanup", false);
                return $"Error closing browser: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Disposes of all resources used by the plugin.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
                _browser = null;
            }
            
            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }
            
            _page = null;
            _context = null;
            _initialized = false;
            _semaphore.Dispose();
        }
    }
}

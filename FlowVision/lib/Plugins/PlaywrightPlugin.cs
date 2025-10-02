using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using FlowVision.lib.Classes;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading;
using System.Text.Json;

namespace FlowVision.lib.Plugins
{
    /// <summary>
    /// Playwright plugin for browser automation within FlowVision.
    /// </summary>
    internal class PlaywrightPlugin 
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
        /// Gets whether a browser instance is currently active.
        /// </summary>
        [Description("Checks if a browser is already running")]
        public string IsBrowserActive()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "IsBrowserActive");
            
            if (_browser != null)
            {
                return $"Yes, a browser is currently active. You can continue using the existing browser without launching a new one.";
            }
            else
            {
                return "No browser is currently active. You need to call LaunchBrowser first.";
            }
        }

        /// <summary>
        /// Gets information about the current Playwright status.
        /// </summary>
        [Description("Gets detailed information about the current browser status")]
        public string GetBrowserStatus()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "GetBrowserStatus");
            
            var status = new Dictionary<string, string>
            {
                { "PlaywrightInitialized", _initialized ? "Yes" : "No" },
                { "BrowserActive", _browser != null ? "Yes" : "No" },
                { "CurrentSessionId", _currentSessionId },
                { "SessionPersistenceEnabled", _useSession ? "Yes" : "No" }
            };
            
            return JsonSerializer.Serialize(status, new JsonSerializerOptions { WriteIndented = true });
        }

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
        [Description("Sets the active session ID for browser operations")]
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
        [Description("Enables or disables session persistence")]
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
        [Description("Launches a new browser instance or uses existing browser if already running")]
        public async Task<string> LaunchBrowser(
            [Description("Browser type (chromium, firefox, webkit)")] string browserType = "chromium",
            [Description("Launch as headless browser (true/false)")] string headless = "true",
            [Description("Force launch a new browser even if one exists (true/false)")] string forceNew = "false")
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "LaunchBrowser", $"Type: {browserType}, Headless: {headless}, ForceNew: {forceNew}");
            
            // Check if we should force a new browser launch
            bool shouldForceNew = !string.IsNullOrEmpty(forceNew) && bool.Parse(forceNew);
            
            // Check if a browser is already active
            if (_browser != null && !shouldForceNew)
            {
                return $"Using existing {browserType} browser that is already running. To force a new browser launch, set forceNew to true.";
            }
            
            PluginLogger.NotifyTaskStart("Browser launch", $"Starting {browserType} browser");
            
            await InitializePlaywrightAsync();
            
            try
            {
                // Close existing browser if one is open and we're forcing a new one
                if (_browser != null)
                {
                    if (_page != null)
                    {
                        try { await _page.CloseAsync(); } catch { /* ignore */ }
                        _page = null;
                    }

                    if (_context != null)
                    {
                        try { await _context.CloseAsync(); } catch { /* ignore */ }
                        _context = null;
                    }

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
        [Description("Saves the current browser session for future use")]
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
        [Description("Navigates to a specified URL")]
        public async Task<string> NavigateTo(
            [Description("URL to navigate to")] string url,
            [Description("Navigation wait strategy: load, domcontentloaded, networkidle")]
            string waitUntil = "load")
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
                
                // Determine wait strategy
                WaitUntilState waitState = WaitUntilState.Load;
                if (!string.IsNullOrEmpty(waitUntil))
                {
                    switch (waitUntil.ToLower())
                    {
                        case "domcontentloaded":
                            waitState = WaitUntilState.DOMContentLoaded;
                            break;
                        case "networkidle":
                            waitState = WaitUntilState.NetworkIdle;
                            break;
                        case "commit":
                            waitState = WaitUntilState.Commit;
                            break;
                        default:
                            waitState = WaitUntilState.Load;
                            break;
                    }
                }

                // Navigate to the URL using the chosen strategy with a timeout
                var response = await _page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = waitState,
                    Timeout = 30000
                });
                
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
        [Description("Takes a screenshot of the current page")]
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
        [Description("Clicks on an element identified by CSS selector")]
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

                // Wait for the element to be visible and enabled before clicking
                var element = await _page.QuerySelectorAsync(selector);
                if (element == null)
                {
                    PluginLogger.NotifyTaskComplete("Click interaction", false);
                    return $"Error: Could not find element with selector: {selector}";
                }

                // Wait for element to be visible and enabled (interactable)
                try
                {
                    await _page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                    {
                        State = WaitForSelectorState.Visible,
                        Timeout = 5000
                    });
                }
                catch (TimeoutException)
                {
                    PluginLogger.NotifyTaskComplete("Click interaction", false);
                    return $"Error: Element with selector '{selector}' did not become visible in time.";
                }

                try
                {
                    await _page.ClickAsync(selector, new PageClickOptions
                    {
                        Timeout = 5000
                    });
                }
                catch (Exception clickEx)
                {
                    PluginLogger.NotifyTaskComplete("Click interaction", false);
                    return $"Error: Failed to click element '{selector}': {clickEx.Message}";
                }
                
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
        [Description("Types text into an input field identified by CSS selector")]
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
                
                // Try multiple common selectors for Bing if a generic search selector is provided
                string[] selectorsToTry;
                if (selector == "input[name='q']" && await _page.QuerySelectorAsync(selector) == null)
                {
                    // For Bing specifically, try multiple selector options
                    string currentUrl = _page.Url;
                    if (currentUrl.Contains("bing.com"))
                    {
                        selectorsToTry = new[] { 
                            selector,
                            "#sb_form_q", 
                            "input[type='search']",
                            "[aria-label='Enter your search term']",
                            "#SearchBox" 
                        };
                        PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", "Detected Bing, will try multiple selectors");
                    }
                    else
                    {
                        selectorsToTry = new[] { selector };
                    }
                }
                else
                {
                    selectorsToTry = new[] { selector };
                }

                // Variables for retry logic
                const int maxRetries = 3;
                int currentRetry = 0;
                Exception lastException = null;
                
                while (currentRetry < maxRetries)
                {
                    try
                    {
                        foreach (var currentSelector in selectorsToTry)
                        {
                            try
                            {
                                // Log which selector we're trying
                                if (selectorsToTry.Length > 1)
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Trying selector: {currentSelector}");
                                }
                                
                                // Check if element exists
                                var element = await _page.QuerySelectorAsync(currentSelector);
                                if (element == null)
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Element not found with selector: {currentSelector}");
                                    continue; // Try next selector
                                }

                                // Check if element is visible
                                bool isVisible = await element.IsVisibleAsync();
                                if (!isVisible)
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Element not visible with selector: {currentSelector}");
                                    continue; // Try next selector
                                }

                                // Check if element is enabled (not disabled)
                                bool isDisabled = await element.EvaluateAsync<bool>("el => el.disabled");
                                if (isDisabled)
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Element is disabled with selector: {currentSelector}");
                                    continue; // Try next selector
                                }

                                // Try to scroll element into view for better interaction
                                await element.ScrollIntoViewIfNeededAsync();
                                
                                // Get element dimensions to verify it has size
                                var boundingBox = await element.BoundingBoxAsync();
                                if (boundingBox == null || boundingBox.Width <= 0 || boundingBox.Height <= 0)
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Element has no dimensions with selector: {currentSelector}");
                                    continue;
                                }

                                // First try to focus the element
                                await element.FocusAsync();
                                
                                // Wait for the element to be actually focused
                                await Task.Delay(100);
                                
                                // Clear the field using keyboard shortcuts first
                                await _page.Keyboard.DownAsync("Control");
                                await _page.Keyboard.PressAsync("a");
                                await _page.Keyboard.UpAsync("Control");
                                await _page.Keyboard.PressAsync("Delete");
                                
                                // Then try direct filling (works better in most cases)
                                await element.FillAsync("");
                                
                                // Type the text with a slight delay to simulate human typing
                                await element.TypeAsync(text, new ElementHandleTypeOptions { Delay = 5 });
                                
                                // Check if the text was actually entered
                                string actualValue = await element.EvaluateAsync<string>("el => el.value || el.textContent");
                                if (string.IsNullOrEmpty(actualValue))
                                {
                                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Element accepted focus but no text was entered with selector: {currentSelector}");
                                    
                                    // Try one last approach - direct JavaScript input
                                    await _page.EvaluateAsync($"document.querySelector('{currentSelector}').value = '{text.Replace("'", "\\'")}'");
                                    
                                    // Verify once more
                                    actualValue = await element.EvaluateAsync<string>("el => el.value || el.textContent");
                                    if (string.IsNullOrEmpty(actualValue))
                                    {
                                        continue; // Try next selector
                                    }
                                }
                                
                                // Save session state after typing if enabled
                                if (_useSession)
                                {
                                    await SaveSession();
                                }
                                
                                PluginLogger.NotifyTaskComplete("Text input");
                                return $"Successfully typed text into field with selector: {currentSelector}";
                            }
                            catch (Exception selectorEx)
                            {
                                // Log exception for this selector but continue trying others
                                PluginLogger.LogError("PlaywrightPlugin", "TypeText", $"Error with selector '{currentSelector}': {selectorEx.Message}");
                                lastException = selectorEx;
                                
                                // Continue to the next selector
                            }
                        }
                        
                        // If we've tried all selectors and none worked, throw an exception to trigger a retry
                        throw new Exception("No selectors were successful");
                    }
                    catch (Exception retryEx)
                    {
                        currentRetry++;
                        lastException = retryEx;
                        
                        if (currentRetry < maxRetries)
                        {
                            // Log that we're retrying
                            PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Retry attempt {currentRetry}/{maxRetries}");
                            
                            // Wait with increasing backoff before retrying
                            await Task.Delay(500 * currentRetry);
                            
                            // Take a screenshot to help diagnose the issue
                            string screenshotDir = Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "FlowVision", "Diagnostics");
                            
                            Directory.CreateDirectory(screenshotDir);
                            string screenshotPath = Path.Combine(screenshotDir, $"typetext_retry_{currentRetry}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                            
                            try
                            {
                                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                                PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Diagnostic screenshot saved to {screenshotPath}");
                            }
                            catch
                            {
                                // Ignore screenshot errors
                            }
                        }
                    }
                }
                
                // If we get here, all retries have failed
                string errorDetails = lastException != null ? $": {lastException.Message}" : "";
                PluginLogger.NotifyTaskComplete("Text input", false);
                
                // Try to get the HTML of the page for debugging
                string pageHtml = "";
                try
                {
                    pageHtml = await _page.ContentAsync();
                    // Save the HTML for debugging
                    string htmlDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "FlowVision", "Diagnostics");
                    
                    Directory.CreateDirectory(htmlDir);
                    string htmlPath = Path.Combine(htmlDir, $"page_debug_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                    File.WriteAllText(htmlPath, pageHtml);
                    PluginLogger.LogInfo("PlaywrightPlugin", "TypeText", $"Page HTML saved to {htmlPath}");
                }
                catch
                {
                    // Ignore HTML capture errors
                }
                
                return $"Error typing text after {maxRetries} attempts{errorDetails}. The element may be in an iframe or protected by the website.";
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
        [Description("Lists all available saved browser sessions")]
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
        [Description("Deletes a saved browser session")]
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
        /// Closes the current browser instance and disposes all related resources.
        /// </summary>
        [Description("Closes the active browser and releases resources")]
        public async Task<string> CloseBrowser()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "CloseBrowser");

            try
            {
                if (_page != null)
                {
                    try { await _page.CloseAsync(); } catch { /* ignore */ }
                    _page = null;
                }

                if (_context != null)
                {
                    try { await _context.CloseAsync(); } catch { /* ignore */ }
                    _context = null;
                }

                if (_browser != null)
                {
                    try { await _browser.CloseAsync(); } catch { /* ignore */ }
                    await _browser.DisposeAsync();
                    _browser = null;
                }

                _initialized = false;

                return "Browser closed successfully.";
            }
            catch (Exception ex)
            {
                return $"Error closing browser: {ex.Message}";
            }
        }
        /// Gets the full HTML content of the current page.
        /// </summary>
        [Description("Gets the current page HTML content")]
        public async Task<string> GetPageContent()
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "GetPageContent");

            if (_page == null)
            {
                return "Error: Browser not launched. Call LaunchBrowser first.";
            }

            return await _page.ContentAsync();
        }

        /// <summary>
        /// Retrieves text content from an element identified by a CSS selector.
        /// </summary>
        [Description("Gets the text content of an element by CSS selector")]
        public async Task<string> GetElementText(
            [Description("CSS selector of the element")] string selector)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "GetElementText", $"Selector: {selector}");

            if (_page == null)
            {
                return "Error: Browser not launched. Call LaunchBrowser first.";
            }

            if (string.IsNullOrEmpty(selector))
            {
                return "Error: Selector cannot be empty";
            }

            try
            {
                var element = await _page.QuerySelectorAsync(selector);
                if (element == null)
                {
                    return $"Error: Could not find element with selector: {selector}";
                }

                string text = await element.TextContentAsync();
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error getting element text: {ex.Message}";
            }
        }

        /// <summary>
        /// Executes custom JavaScript in the current page and returns the result.
        /// </summary>
        [Description("Executes a JavaScript snippet in the current page")]
        public async Task<string> ExecuteScript(
            [Description("JavaScript code to run")] string script)
        {
            PluginLogger.LogPluginUsage("PlaywrightPlugin", "ExecuteScript");

            if (_page == null)
            {
                return "Error: Browser not launched. Call LaunchBrowser first.";
            }

            if (string.IsNullOrWhiteSpace(script))
            {
                return "Error: Script cannot be empty.";
            }

            try
            {
                string result = await _page.EvaluateAsync<string>(script);
                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error executing script: {ex.Message}";
            }
        }
    }
}

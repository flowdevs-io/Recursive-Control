# FlowVision

**Vision:** Making computing so easy, anyone can do it.

FlowVision aims to revolutionize how users interact with their computers by leveraging the power of Large Language Models (LLMs) and a suite of integrated tools. It provides a conversational AI interface that can understand natural language requests and translate them into actions on the user's machine, bridging the gap between human intent and computer execution.

## Core Features

*   **Conversational AI Interface:** Interact with your computer using natural language through a chat window ([`Form1`](../../t:/Human-Use/FlowVision/Form1.cs)). The AI assistant interprets your requests and utilizes available tools to fulfill them.
*   **LLM Integration:** Powered by advanced LLMs like Azure OpenAI ([`Actioner`](../../t:/Human-Use/FlowVision/lib/Classes/ai/Actioner.cs)) or potentially others like GitHub Models ([`Github_Actioner`](../../t:/Human-Use/FlowVision/lib/Classes/ai/Github_Actioner.cs)), enabling sophisticated understanding and task execution.
*   **Extensible Tool System:** Integrates with Microsoft Semantic Kernel to manage and invoke various plugins (tools) that extend the AI's capabilities beyond simple chat.

## Available Tools (Plugins)

FlowVision comes equipped with a powerful set of tools (plugins) that the AI can use to interact with your system. These tools are the key to making complex tasks simple. You can enable or disable these tools via the Tools configuration menu (`File -> Tools`).

*   **CMD Plugin ([`CMDPlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/CMDPlugin.cs)):** Allows the AI to execute commands directly in the Windows Command Prompt. Useful for running scripts, managing files, or performing system tasks.
*   **PowerShell Plugin ([`PowershellPlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/PowershellPlugin.cs)):** Enables the execution of PowerShell scripts, offering more advanced system administration and automation capabilities compared to CMD.
*   **Screen Capture & OmniParser Plugin ([`ScreenCaptureOmniParserPlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/ScreenCaptureOmniParserPlugin.cs)):** This is a cornerstone of FlowVision's "vision" capabilities.
    *   It captures screenshots of the entire screen or specific application windows.
    *   It sends the captured image to an **OmniParser** service ([`OmniParserClient`](../../t:/Human-Use/FlowVision/lib/Classes/OmniParserClient.cs)). OmniParser analyzes the image, identifies UI elements (buttons, text fields, etc.), and returns structured data about the content and layout.
    *   This allows the AI to "see" and understand the visual interface of applications, enabling interaction with GUI elements even without direct API access. Configure the OmniParser URL via the Vision menu (`Vision -> OmniParser`).
*   **Keyboard Plugin ([`KeyboardPlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/KeyboardPlugin.cs)):** Simulates keyboard input, allowing the AI to type text, press keys, and use keyboard shortcuts in applications.
*   **Mouse Plugin ([`MousePlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/MousePlugin.cs)):** Simulates mouse movements and clicks (left, right, double-click) at specific screen coordinates or relative positions. Essential for interacting with graphical interfaces based on visual understanding from OmniParser.
*   **Window Selection Plugin ([`WindowSelectionPlugin`](../../t:/Human-Use/FlowVision/lib/Plugins/WindowSelectionPlugin.cs)):** Manages application windows. It can list open windows, find specific windows by title, bring windows to the foreground, and provide window handles necessary for targeted screen captures or interactions.

## Configuration

FlowVision requires some initial setup for optimal functionality:

1.  **LLM Configuration:** Set up your LLM provider (e.g., Azure OpenAI) credentials via the LLM menu (`LLM -> Setup -> Azure OpenAI` or `Github`). Enter the required endpoint, API key, and deployment/model name ([`ConfigForm`](../../t:/Human-Use/FlowVision/ConfigForm.cs)).
2.  **Tool Configuration:** Access the tool settings via `File -> Tools` ([`ToolConfigForm`](../../t:/Human-Use/FlowVision/ToolConfigForm.cs)). Here you can:
    *   Enable/disable specific plugins.
    *   Configure AI behavior (e.g., `Temperature`, `Auto-Invoke Functions`).
    *   Toggle chat history retention.
    *   Customize the AI's system prompt.
    *   Enable/disable plugin usage logging.
3.  **OmniParser Configuration:** Configure the URL for your OmniParser service instance via `Vision -> OmniParser` ([`OmniParserForm`](Human-Use/FlowVision/OmniParserForm.cs)). This is crucial for enabling the screen analysis features.

## Getting Started

1.  Clone the repository.
2.  Open the solution (`FlowVision.sln`) in Visual Studio.
3.  Build the solution. This will restore necessary NuGet packages ([`packages.config`](../../t:/Human-Use/FlowVision/packages.config)).
4.  Run the application (`FlowVision.exe`).
5.  Configure the LLM, Tools, and OmniParser settings through the application menus as described above.
6.  Start chatting with the AI!

## Dependencies

FlowVision relies on several key libraries:

*   **Microsoft Semantic Kernel:** For AI orchestration, planning, and plugin management.
*   **Azure OpenAI / OpenAI SDKs:** For communicating with the LLM services.
*   **Newtonsoft.Json:** For JSON serialization/deserialization.
*   **Windows Forms:** For the user interface.

By combining a powerful LLM with a versatile set of tools, especially the vision capabilities provided by OmniParser, FlowVision takes a significant step towards making computer interaction intuitive and accessible for everyone.
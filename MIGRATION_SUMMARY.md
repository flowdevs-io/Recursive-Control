# Migration from Semantic Kernel to Microsoft Extensions.AI

## Overview
This project has been migrated from Microsoft Semantic Kernel to use the Microsoft Extensions.AI abstraction layer, which is part of the Microsoft Agent Framework ecosystem.

## Key Changes

### 1. Namespace Changes
- **Removed**: `Microsoft.SemanticKernel.*`
- **Added**: `Microsoft.Extensions.AI`
- **Added**: `Azure.AI.OpenAI` (for Azure client)
- **Added**: `OpenAI.Chat` (for chat types)

### 2. Type Replacements

| Old (Semantic Kernel) | New (Extensions.AI) |
|----------------------|---------------------|
| `IChatCompletionService` | `IChatClient` |
| `ChatHistory` | `List<ChatMessage>` |
| `Kernel` | Removed - direct client usage |
| `OpenAIPromptExecutionSettings` | `ChatOptions` |
| `ToolCallBehavior` | `ChatToolMode` |
| `[KernelFunction]` attribute | `[Description]` attribute (standard .NET) |

### 3. API Pattern Changes

#### Agent/Chat Client Creation
**Before (SK)**:
```csharp
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
var kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();
```

**After (Extensions.AI)**:
```csharp
var azureClient = new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
var chat = azureClient.AsChatClient(deploymentName);
```

#### Chat History Management
**Before (SK)**:
```csharp
history.AddSystemMessage("prompt");
history.AddUserMessage("user input");
history.AddAssistantMessage("response");
```

**After (Extensions.AI)**:
```csharp
history.Add(new ChatMessage(ChatRole.System, "prompt"));
history.Add(new ChatMessage(ChatRole.User, "user input"));
history.Add(new ChatMessage(ChatRole.Assistant, "response"));
```

#### Tool/Function Registration
**Before (SK)**:
```csharp
builder.Plugins.AddFromType<CMDPlugin>();
```

**After (Extensions.AI)**:
```csharp
var tools = new List<AIFunction>();
tools.AddRange(AIFunctionFactory.Create(new CMDPlugin()));
```

#### Chat Completion with Streaming
**Before (SK)**:
```csharp
var settings = new OpenAIPromptExecutionSettings
{
    Temperature = 0.7,
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
var stream = chat.GetStreamingChatMessageContentsAsync(history, settings, kernel);
await foreach (var message in stream)
{
    responseBuilder.Append(message.Content);
}
```

**After (Extensions.AI)**:
```csharp
var options = new ChatOptions
{
    Temperature = 0.7f,
    Tools = tools,
    ToolMode = ChatToolMode.Auto
};
await foreach (var update in chat.CompleteStreamingAsync(history, options))
{
    if (update.Text != null)
    {
        responseBuilder.Append(update.Text);
    }
}
```

#### Function Invocation (Auto-invoke tools)
**Before (SK)**:
Built into Kernel with `ToolCallBehavior.AutoInvokeKernelFunctions`

**After (Extensions.AI)**:
```csharp
var functionInvokingClient = new FunctionInvokingChatClient(chatClient);
```

### 4. Plugin Changes
- Removed `[KernelFunction]` attributes from all plugin methods
- Plugins now use standard `[Description]` attributes
- No need for special plugin wrapper classes
- Plugins are instantiated directly and passed to `AIFunctionFactory.Create()`

## Files Modified

### Core AI Classes
- `FlowVision/lib/Classes/ai/Actioner.cs` - Migrated to Extensions.AI
- `FlowVision/lib/Classes/ai/MultiAgentActioner.cs` - Migrated to Extensions.AI
- `FlowVision/lib/Classes/ai/Github_Actioner.cs` - Migrated to Extensions.AI
- `FlowVision/lib/Classes/ToolDescriptionGenerator.cs` - Removed SK dependency

### Plugins (all migrated)
- `FlowVision/lib/Plugins/CMDPlugin.cs`
- `FlowVision/lib/Plugins/KeyboardPlugin.cs`
- `FlowVision/lib/Plugins/MousePlugin.cs`
- `FlowVision/lib/Plugins/PlaywrightPlugin.cs`
- `FlowVision/lib/Plugins/PowershellPlugin.cs`
- `FlowVision/lib/Plugins/RemoteControlPlugin.cs`
- `FlowVision/lib/Plugins/ScreenCaptureOmniParserPlugin.cs`
- `FlowVision/lib/Plugins/ScreenCapturePlugin.cs`
- `FlowVision/lib/Plugins/WindowSelectionPlugin.cs`

### UI
- `FlowVision/Form1.cs` - Updated namespace imports

## Packages

### Removed Semantic Kernel Packages (Keeping for backward compatibility during transition)
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Abstractions
- Microsoft.SemanticKernel.Core
- Microsoft.SemanticKernel.Connectors.AzureOpenAI
- Microsoft.SemanticKernel.Connectors.OpenAI

### Core Packages (Already Present)
- Microsoft.Extensions.AI (9.4.0-preview.1.25207.5)
- Microsoft.Extensions.AI.Abstractions (9.4.0-preview.1.25207.5)
- Microsoft.Extensions.AI.AzureAIInference (9.4.0-preview.1.25207.5)

### Added Packages
- Microsoft.Extensions.AI.OpenAI (9.4.0-preview.1.25207.5) - For AsChatClient extension methods
- Microsoft.Agents.AI (1.0.0-preview.251001.3) - Optional, for advanced agent features
- Microsoft.Agents.AI.OpenAI (1.0.0-preview.251001.3) - Optional

### Supporting Packages (Already Present)
- Azure.AI.OpenAI (2.2.0-beta.4)
- OpenAI (2.2.0-beta.4)
- Azure.Core (1.45.0)

## Benefits of Migration

1. **Simplified API**: No more complex Kernel builders and service registration
2. **Better Abstractions**: IChatClient is provider-agnostic
3. **Standard .NET**: Uses standard attributes ([Description]) instead of custom ones
4. **Flexibility**: Easy to swap AI providers
5. **Future-Proof**: Microsoft Extensions.AI is the future direction for AI in .NET
6. **Lighter Dependencies**: Fewer packages and abstractions to maintain

## Testing Checklist

- [ ] Test basic single-agent actions with Actioner
- [ ] Test multi-agent workflow with MultiAgentActioner
- [ ] Verify all plugins work correctly
  - [ ] CMDPlugin
  - [ ] PowerShellPlugin
  - [ ] KeyboardPlugin
  - [ ] MousePlugin
  - [ ] ScreenCapturePlugin
  - [ ] WindowSelectionPlugin
  - [ ] PlaywrightPlugin
  - [ ] RemoteControlPlugin
- [ ] Test tool/function calling and auto-invocation
- [ ] Test streaming responses
- [ ] Test configuration loading and API connectivity
- [ ] Test error handling

## Notes

- The migration maintains backward compatibility where possible
- Semantic Kernel packages are kept in packages.config during transition
- The core functionality remains the same - only the underlying framework changed
- Performance should be similar or better due to simpler abstractions

## References

- [Microsoft Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-prompts)
- [Microsoft Agent Framework](https://github.com/microsoft/agent-framework)
- [Migration Guide](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/SemanticKernelMigration/README.md)

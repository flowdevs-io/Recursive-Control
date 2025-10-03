---
layout: default
title: Prompt Engineering Guide
---

# Prompt Engineering Guide for Recursive Control

Master the art of communicating with your AI agents to achieve precise, efficient automation.

## Why Prompt Engineering Matters
Recursive Control's multi-agent system (Hermes, Daedalus, and Talos) interprets natural language, but structured prompts yield better results:
- **Clarity reduces errors**: Ambiguous requests lead to wrong assumptions.
- **Structure aids planning**: Well-organized prompts help the Planner agent create better steps.
- **Context improves accuracy**: Provide relevant details for better execution.

## Core Principles
1. **Be Specific**: Include exact details like app names, file paths, or expected outcomes.
2. **Break It Down**: For complex tasks, suggest steps or use multi-stage prompts.
3. **Provide Context**: Mention current state, like open windows or recent actions.
4. **Use Verification**: Ask the agent to confirm steps or describe what it sees.
5. **Handle Errors Gracefully**: Include fallback instructions.

## Prompt Patterns
### Basic Command
**Template**: "Perform [action] in [location/app] with [details]."

**Example**: "Open Chrome and navigate to github.com/flowdevs-io/Recursive-Control."

### Multi-Step Workflow
**Template**: "Do the following steps: 1. [Step 1] 2. [Step 2] ... Verify each step."

**Example**: "Create a report: 1. Open Excel. 2. Add headers: Date, Task, Status. 3. Fill with today's data. 4. Save as 'daily-report.xlsx' in Documents."

### Vision-Assisted
**Template**: "Capture the screen, describe what you see, then [action based on description]."

**Example**: "Take a screenshot of the current window, identify the search bar, and type 'AI tools' into it."

### Conditional Logic
**Template**: "If [condition], do [action A]; else do [action B]."

**Example**: "If Chrome is open, navigate to YouTube; else open Chrome first then go to YouTube."

## Advanced Techniques
### Chain of Thought
Encourage reasoning: "Think step-by-step: First, check if the app is open. If not, open it. Then..."

### Role Playing
Assign roles: "As a efficient automation expert, optimize this workflow: [task]."

### Few-Shot Examples
Provide samples: "Like how you opened Notepad last time, now open Paint and draw a square."

## Common Pitfalls
- **Too Vague**: "Do something with files" → Agents might guess wrong.
- **Overly Complex**: Break long prompts into multiple interactions.
- **Assuming State**: Always verify: "Focus on the foreground window and describe it first."

## Tips for Power Users
- Use the UI's chat history to build context across prompts.
- Combine plugins explicitly: "Use Playwright to automate browser, then CMD to process downloads."
- Test incrementally: Start with simple tasks and build up.

Master these patterns to turn Recursive Control into your ultimate productivity copilot!

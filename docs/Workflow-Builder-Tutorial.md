---
layout: default
title: Workflow Builder Tutorial
---

# Workflow Builder Tutorial: Gamified Learning Adventures

Level up your Recursive Control skills by building interactive workflows! This tutorial turns learning into a game: complete "quests" by crafting prompt chains that automate real tasks. Each quest includes objectives, step-by-step guidance, and verification challenges.

Think of this as a simulator – test your prompts here before running them in the app. Earn "badges" by successfully completing each workflow (self-assessed via expected outcomes).

## Quest 1: GitHub Issue Automator (Beginner Level)
**Objective**: Automate creating a GitHub issue in your repo using browser automation and keyboard inputs.

**Badge**: Issue Master

**Step-by-Step Prompt Chain**:
1. **Launch the Browser**: "Open Chrome and navigate to github.com/login."
   - *Expected*: Browser opens to GitHub login page. (Verify: Describe the screen to confirm.)

2. **Login**: "Focus on the username field and type 'yourusername', then tab to password and type 'yourpassword', then press enter."
   - *Tip*: Use KeyboardPlugin for targeted input. (Challenge: Add verification – "If login fails, notify me.")

3. **Navigate to Repo**: "Go to github.com/yourusername/your-repo/issues."
   - *Expected*: Issues page loads.

4. **Create Issue**: "Click the 'New issue' button, type 'Bug: App crashes on load' in title, add description 'Steps to reproduce: 1. Open app. 2. Click button.', then submit."
   - *Challenge*: Use ScreenCapture to identify the button's bounding box before clicking.

**Full Chain Prompt** (Copy-Paste Ready):
```
Perform these steps to create a GitHub issue:
1. Open Chrome and go to github.com/login.
2. Log in with username 'yourusername' and password 'yourpassword'.
3. Navigate to github.com/yourusername/your-repo/issues.
4. Click 'New issue', fill title 'Bug: App crashes', description 'Steps: Open app, click button', and submit.
Verify each step with a screenshot description.
```

**Verification Quest**: Run this in Recursive Control. Did it create the issue? If not, refine the prompt (e.g., handle 2FA).

## Quest 2: Daily Report Generator (Intermediate Level)
**Objective**: Automate opening Excel, filling data, and saving a report.

**Badge**: Report Wizard

**Step-by-Step Prompt Chain**:
1. **Open App**: "Launch Excel and create a new spreadsheet."
2. **Add Headers**: "Type 'Date' in A1, 'Task' in B1, 'Status' in C1."
3. **Fill Data**: "In A2 type today's date, B2 'Implement feature X', C2 'Completed'."
4. **Save**: "Save as 'daily-report.xlsx' in Documents."

**Full Chain Prompt**:
```
Build a daily report in Excel:
1. Open Excel, new file.
2. Headers: A1=Date, B1=Task, C1=Status.
3. Data: A2=today's date, B2=Review code, C2=Done.
4. Save to Documents as daily-report.xlsx.
Confirm each cell after typing.
```

**Challenge**: Add conditional logic – "If file exists, append instead of overwrite."

## Quest 3: File Organizer Bot (Advanced Level)
**Objective**: Scan Downloads folder, organize files by type using CMD/PowerShell.

**Badge**: Organization Overlord

**Step-by-Step Prompt Chain**:
1. **Scan Folder**: "Use CMD to list files in Downloads."
2. **Create Folders**: "Make directories: Images, Documents, Others."
3. **Move Files**: "Move .jpg to Images, .pdf to Documents, others to Others."
4. **Verify**: "List contents of each new folder."

**Full Chain Prompt**:
```
Organize Downloads:
1. CMD: dir %USERPROFILE%\Downloads
2. Create folders: mkdir Images Documents Others
3. Move: move *.jpg Images, move *.pdf Documents, move *.* Others
4. Verify: dir Images, dir Documents, dir Others
Handle errors if folders exist.
```

**Epic Challenge**: Integrate vision – "Screenshot Downloads folder and describe file icons before moving."

## Level Up Tips
- **Gamification Hack**: Track your success rate. Aim for 100% on 5 quests to "unlock" custom workflow creation.
- **Combine Quests**: Chain them, e.g., "Generate report, then create GitHub issue about it."
- **Debug Mode**: If a step fails, add "Describe the screen and suggest fixes" to your prompts.

## Go Interactive: Jupyter Notebook Simulator
For hands-on practice, download our [Interactive Workflow Builder Notebook](tutorials/Workflow-Builder-Notebook.ipynb). It includes widgets to build and simulate prompt chains without running the full app!

**How to Use**:
1. Install Jupyter: `pip install notebook`
2. Download the .ipynb file.
3. Run `jupyter notebook` and open the file.
4. Interact with widgets to test prompts in real-time.

Completed all quests? Share your custom workflows on Discord!

Back to [Getting Started](Getting-Started.html)

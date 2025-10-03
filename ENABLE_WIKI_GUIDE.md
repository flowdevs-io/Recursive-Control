# How to Enable GitHub Wiki & Documentation

## 🚫 Issue: "I don't see a wiki page option in GitHub"

GitHub Wiki needs to be **enabled** in repository settings. Here are **3 solutions**:

---

## ✅ Solution 1: Enable GitHub Wiki (Recommended)

### Step-by-Step:

1. **Go to Your Repository**
   ```
   https://github.com/flowdevs-io/Recursive-Control
   ```

2. **Click "Settings" Tab**
   - Top navigation bar
   - Requires admin/owner permissions

3. **Scroll to "Features" Section**
   - About halfway down the settings page
   - Look for checkboxes

4. **Enable Wiki**
   - Find "Wikis" checkbox
   - ✅ Check the box
   - Wait for save (automatic)

5. **Wiki Tab Appears!**
   - Refresh page
   - "Wiki" tab now visible in main navigation
   - Click to create first page

### Publish Wiki Content:

**Method A: Web Interface**
```
1. Click "Wiki" tab
2. Click "Create the first page"
3. Title: "Home"
4. Content: Copy from wiki/Home.md
5. Click "Save Page"
6. Repeat for other pages
```

**Method B: Git Clone**
```bash
# Clone wiki repository
git clone https://github.com/flowdevs-io/Recursive-Control.wiki.git

# Copy all wiki files
cp wiki/*.md Recursive-Control.wiki/

# Rename Home to match GitHub convention
cd Recursive-Control.wiki
mv Home.md Home.md  # Already correct

# Commit and push
git add .
git commit -m "Complete documentation wiki"
git push origin master
```

---

## ✅ Solution 2: Use GitHub Pages (Alternative)

**I've already set this up for you!** The `docs/` folder is ready.

### Step-by-Step:

1. **Go to Repository Settings**
   ```
   Settings → Pages (left sidebar)
   ```

2. **Configure GitHub Pages**
   - **Source**: Deploy from a branch
   - **Branch**: `main` (or `master`)
   - **Folder**: `/docs`
   - Click **Save**

3. **Wait 2-3 Minutes**
   - GitHub builds your site
   - Check Actions tab for build status

4. **Access Your Documentation**
   ```
   https://flowdevs-io.github.io/Recursive-Control/
   ```

### What's Already Set Up:

```
docs/
├── _config.yml              # Jekyll configuration ✅
├── index.md                 # Home page (from wiki/Home.md) ✅
├── Installation.md          # Setup guide ✅
├── Getting-Started.md       # Tutorial ✅
├── Multi-Agent-Architecture.md  # Technical deep dive ✅
├── FAQ.md                   # Questions & answers ✅
├── Troubleshooting.md       # Problem solving ✅
├── API-Reference.md         # Developer docs ✅
├── Blog-Post-v2.0.md        # v2.0 announcement ✅
├── System-Prompts-Reference.md  # Prompts ✅
├── UI-Features.md           # UI improvements ✅
└── UI-Redesign.md           # UI redesign ✅
```

**Theme**: Cayman (beautiful, modern)
**Features**: 
- Automatic navigation
- Syntax highlighting
- Mobile responsive
- SEO optimized

---

## ✅ Solution 3: Use README + Docs Folder (Simplest)

Keep everything in the main repository with links.

### Update Main README.md:

Add this section:

```markdown
## 📚 Documentation

- [Installation Guide](docs/Installation.md)
- [Getting Started](docs/Getting-Started.md)
- [Multi-Agent Architecture](docs/Multi-Agent-Architecture.md)
- [API Reference](docs/API-Reference.md)
- [FAQ](docs/FAQ.md)
- [Troubleshooting](docs/Troubleshooting.md)

### Reference
- [Version 2.0 Blog Post](docs/Blog-Post-v2.0.md)
- [System Prompts Reference](docs/System-Prompts-Reference.md)
- [UI Features](docs/UI-Features.md)
- [UI Redesign](docs/UI-Redesign.md)
```

**Pros:**
- ✅ No setup needed
- ✅ Works immediately
- ✅ Visible to all users
- ✅ Easy to maintain

**Cons:**
- ❌ No wiki-style interface
- ❌ No automatic navigation
- ❌ Less discoverable

---

## 🎯 Comparison: Which to Use?

### GitHub Wiki
**Best for:** Traditional wiki experience
**Pros:**
- Separate git repository
- Wiki-style navigation
- Easy for non-devs to edit
- Standard GitHub feature
**Cons:**
- Requires enabling
- Separate from main repo
- Less visibility in searches

### GitHub Pages
**Best for:** Professional documentation site ⭐
**Pros:**
- Beautiful themed website
- Custom domain support
- Full control over design
- Great SEO
- Already set up!
**Cons:**
- Slightly more complex
- Requires Pages setup (5 minutes)

### Docs Folder in Repo
**Best for:** Quick and simple
**Pros:**
- Immediate availability
- No setup
- Single repository
- Version controlled with code
**Cons:**
- Basic markdown rendering
- No navigation sidebar
- Manual links needed

---

## 🚀 My Recommendation

**Use GitHub Pages** (Solution 2) because:

1. ✅ I've already set it up for you
2. ✅ Professional appearance
3. ✅ Automatic navigation
4. ✅ Beautiful Cayman theme
5. ✅ Mobile-friendly
6. ✅ Takes 2 minutes to enable

**Steps:**
```
1. Go to: Settings → Pages
2. Source: "Deploy from a branch"
3. Branch: main, Folder: /docs
4. Click Save
5. Wait 2-3 minutes
6. Visit: https://flowdevs-io.github.io/Recursive-Control/
```

**Done!** 🎉

---

## 🔧 Enabling GitHub Wiki (Detailed)

### If You Don't See Settings:

**Problem:** Not repository owner/admin

**Solutions:**
- Ask repository owner to enable
- Fork repository (you'll have settings)
- Use GitHub Pages or docs folder instead

### If Wiki Option is Disabled:

**Problem:** Organization policy or repository type

**Solutions:**
1. Check organization settings
2. Contact org admin
3. Use GitHub Pages instead

### If Wiki Enable Checkbox Missing:

**Problem:** Older GitHub interface or private repo restrictions

**Solutions:**
1. Update repository visibility settings
2. Enable via GitHub API
3. Use GitHub Pages as alternative

---

## 📞 Quick Help

### Enable Wiki Not Working?
```bash
# Enable via GitHub CLI (if you have gh installed)
gh repo edit --enable-wiki

# Or via API
curl -X PATCH \
  -H "Authorization: token YOUR_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  https://api.github.com/repos/flowdevs-io/Recursive-Control \
  -d '{"has_wiki":true}'
```

### GitHub Pages Not Building?
1. Check Actions tab for errors
2. Verify `docs/` folder exists
3. Check `_config.yml` is valid YAML
4. Make sure branch is correct (main/master)

### Links Not Working?
- Wiki links: Use page names without .md
- GitHub Pages: Use full paths with .md (or remove for clean URLs)
- Docs folder: Use relative paths with .md

---

## ✅ What You Have Now

**Ready to Use:**
```
✅ docs/ folder with all documentation
✅ _config.yml configured for GitHub Pages
✅ 12 markdown files ready
✅ 78,000 words of content
✅ Beautiful theme selected
✅ Navigation configured
```

**To Publish:**
```
1. Enable GitHub Pages (2 minutes)
   OR
2. Enable Wiki and copy files (5 minutes)
   OR
3. Use docs/ folder directly (immediate)
```

---

## 🎉 Next Steps

**Choose Your Method:**

**Option A: GitHub Pages (Recommended)**
```bash
# Already done! Just enable in Settings → Pages
# Result: https://flowdevs-io.github.io/Recursive-Control/
```

**Option B: GitHub Wiki**
```bash
# Enable in Settings → Features → Wikis ✓
# Clone wiki and copy files
git clone https://github.com/flowdevs-io/Recursive-Control.wiki.git
cp wiki/*.md Recursive-Control.wiki/
cd Recursive-Control.wiki && git add . && git commit -m "Docs" && git push
```

**Option C: Docs Folder**
```bash
# Already done! Just update main README.md
# Add links to docs/*.md files
# Commit and push
```

---

## 💡 Pro Tip

**Use GitHub Pages for best results!**

It gives you:
- Professional documentation site
- Automatic navigation
- Beautiful theme
- Mobile-friendly
- SEO optimized
- Free hosting

And I've already set it all up for you! Just enable it in settings. 🚀

---

<p align="center">
  <strong>Questions?</strong><br>
  <a href="https://discord.gg/mQWsWeHsVU">Join Discord</a> | 
  <a href="https://github.com/flowdevs-io/Recursive-Control/issues">Report Issue</a>
</p>

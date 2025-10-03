# UI Documentation Link Updated

## Changes Made

Updated the documentation link in the FlowVision UI to point to the new GitHub Pages documentation site.

### File Modified
- **`FlowVision/Form1.cs`** - Updated `documentationToolStripMenuItem_Click` method

### Changes
**Before:**
- URL: `https://github.com/flowdevs-io/Recursive-Control/wiki`
- Fallback message: `Visit: https://github.com/flowdevs-io/Recursive-Control/wiki`

**After:**
- URL: `https://flowdevs-io.github.io/Recursive-Control`
- Fallback message: `Visit: https://flowdevs-io.github.io/Recursive-Control`

## Location in UI

Users can access documentation via:
- **Menu**: Help → Documentation
- This opens the comprehensive GitHub Pages documentation site

## What Users Will See

When clicking the documentation menu item, users will now be taken to:
https://flowdevs-io.github.io/Recursive-Control

This provides:
- Complete installation guide
- Getting started tutorial
- API reference for developers
- Multi-agent architecture explanation
- Troubleshooting guide
- FAQ section

## Branch & Pull Request

- **Branch**: `update-docs-link`
- **Base**: `master`
- **Files Changed**: 1 file, 2 lines modified

## Next Steps

1. Create pull request: https://github.com/flowdevs-io/Recursive-Control/pull/new/update-docs-link
2. Review and merge the PR
3. Users will get the new documentation link in the next build

## Note

The "About" dialog (Help → About) still shows the main repository link (`github.com/flowdevs-io/Recursive-Control`) which is correct - that points to the project home, while the documentation link now specifically points to the documentation site.

---

**Status**: ✅ Changes committed and pushed to `update-docs-link` branch
**Ready for**: Pull request and merge

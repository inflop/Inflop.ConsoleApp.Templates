Save the most recently presented implementation plan to a Markdown file with progress tracking functionality.

**Instructions:**

1. Analyze the last presented plan from the conversation (usually in ExitPlanMode or similar format)

2. Generate a file name based on the plan's topic:
   - Format: `[TOPIC]_IMPLEMENTATION_PLAN.md`
   - Examples: `TEMPLATE_RENAMING_IMPLEMENTATION_PLAN.md`, `DATABASE_MIGRATION_PLAN.md`, `API_REFACTORING_PLAN.md`
   - If user provided an argument ($ARGUMENTS), use it as the file name (add .md extension if not provided)
   - Name should be in UPPERCASE with underscores instead of spaces

3. Create plan structure with the following sections:

## Required file structure:

```markdown
# [Implementation plan title]

**Created:** [YYYY-MM-DD with today's date]
**Status:** = Planning in progress
**Version:** 1.0

---

## <¯ Goal

[Describe implementation goal in 1-3 sentences]

---

## =Ë [Main section name - e.g., "Scope of changes"]

###  STEP 1: [First step name]

#### [ ] 1.1 [Task 1 - detailed title]
**Description/Action:**
[Detailed description of what needs to be done]

**Files to modify:** (if applicable)
- `path/to/file1.ext`
- `path/to/file2.ext`

**Changes:** (if applicable)
```
Old code ’ New code
```

**Status:** ó Awaiting implementation

**Issues encountered:**
_No issues - not started yet_

---

#### [ ] 1.2 [Task 2]
[Similar structure]

---

###  STEP 2: [Second step name]
[Similar structure with sub-items]

---

## =à Execution order

1.  **STEP 1** - [Brief step 1 description]
2.  **STEP 2** - [Brief step 2 description]
3.  **STEP 3** - [Brief step 3 description]
[etc.]

---

## ñ Estimated time

| Step | Time | Status |
|------|------|--------|
| STEP 1: [Name] | X min | ó Not started |
| STEP 2: [Name] | Y min | ó Not started |
| **TOTAL** | **Z minutes** | **ó 0% complete** |

---

##  Success criteria

After implementation completion:

- [ ] Criterion 1 (e.g., "All tests pass")
- [ ] Criterion 2 (e.g., "Documentation updated")
- [ ] Criterion 3 (e.g., "Code compiles without warnings")
[List of all criteria - minimum 3-5 items]

---

## =Ý Change history

| Date | Person | Change | Status |
|------|--------|--------|--------|
| [YYYY-MM-DD] | Claude | Plan created |  Completed |
| | | | |

---

## =¨ Issues encountered and solutions

### Issue #1: (Issue title - fill when occurs)
**Date:** YYYY-MM-DD
**Description:**
_Detailed issue description_

**Solution:**
_How it was resolved or status: L Unresolved /  Resolved_

---

## =Ì Additional notes

[Any additional information, documentation links, technical notes, etc.]
```

4. When saving the plan:
   - Copy the ENTIRE structure of the presented plan (all steps, tasks, details)
   - Keep all checkboxes as `[ ]` (unchecked)
   - Set all statuses to `ó Awaiting implementation` or appropriate initial status
   - Insert today's date in YYYY-MM-DD format
   - Add all details from original plan (commands, file paths, code replacements)

5. After saving:
   - Inform user about the name and location of created file
   - Remind that you will update this file continuously during implementation:
     * Checking off checkboxes `[ ]` ’ `[x]` after completing tasks
     * Changing statuses from `ó Awaiting` to ` Completed` or `L Error`
     * Adding issues to =¨ section when they occur
     * Updating % completion in time table

## Important guidelines:

- **DO NOT shorten the plan** - copy ALL details from the original plan
- All checkboxes should be initially `[ ]` (unchecked)
- All statuses should be `ó Awaiting implementation` at start
- Created date = today (format: YYYY-MM-DD)
- Use emoji for better readability:  L ó = <¯ =Ë =à ñ =¨ =Ì
- Generate unique and descriptive file name (UPPERCASE with underscores)
- If plan contains bash commands, code, or other technical details - **PRESERVE THEM ALL**

## Usage examples:

```
User: /save-plan
Claude:  Plan saved to: FEATURE_X_IMPLEMENTATION_PLAN.md

User: /save-plan DATABASE_MIGRATION
Claude:  Plan saved to: DATABASE_MIGRATION.md
```

## Update example during implementation:

When user works on the plan, automatically update the file:
- After completing task 1.1: change `[ ] 1.1` ’ `[x] 1.1` and status to ` Completed`
- If error occurs: add to =¨ section with problem description
- After each step: update % completion in the table

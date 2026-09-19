# Workflow Audit Logging Skill

## Purpose

Create a complete Jupyter Notebook audit record for a workspace-modifying workflow when the user explicitly requests workflow logging or asks for this skill to be used.

This skill is **opt-in**. Do not create workflow audit notebooks automatically for ordinary workspace modifications.

## Activation

Use this skill only when the user explicitly requests it, including requests such as:

* “Use the workflow logging skill.”
* “Audit this workflow.”
* “Create a workflow log for this task.”
* “Record this work in `/Workflows/`.”
* “Run this task with workflow auditing enabled.”

Do not infer activation merely because a task creates, edits, moves, renames, or deletes files.

Once activated for a request, apply the skill to the entire workflow associated with that request unless the user explicitly limits its scope.

## Workflow Record

For an activated workflow, create a new Jupyter Notebook (`.ipynb`) under `/Workflows/` with a concise, descriptive name for the workflow.

Treat the notebook as part of the requested work and complete it before declaring the audited task finished.

The notebook must provide a complete, chronological audit trail of the work performed. Record, as applicable:

* The user's request.
* The initial plan.
* Changes or deviations from the plan.
* Files inspected.
* Files created, modified, moved, renamed, or deleted.
* Commands actually executed.
* Observed command output.
* Edits made.
* Validation or tests performed.
* Test and validation results.
* Errors or unexpected behavior.
* Failed attempts.
* Intermediate results.
* Material decisions that affected the work.
* Relevant TODOs, notes, temporary text, or planning artifacts created during the workflow.
* The final state of the workspace.
* The final response provided to the user.

Include concise summaries of reasoning or decisions where they materially affected the work, but do not fabricate reasoning, results, commands, outputs, or evidence that were not actually observed.

## Notebook Organization

Use Jupyter Notebook formatting and organizational features to make the record clear and readable.

Appropriate elements include:

* Markdown headings and subheadings.
* Tables.
* Lists.
* Blockquotes.
* Code blocks.
* Horizontal sections.
* Clearly labeled command output.
* Source snippets.
* Diffs.
* Test results.
* Error output.

A useful organizational pattern is:

1. **Request**
2. **Initial Plan**
3. **Pre-change State**
4. **Work Log**
5. **Changes**
6. **Validation**
7. **Final State**
8. **User Response**

These section names are guidance rather than a rigid schema. Omit sections that are irrelevant, rename them when a clearer heading would help, and add additional sections where useful.

Preserve a clear chronological progression so another reader can reconstruct what the agent did, why material decisions were made, and what results were actually observed.

## Evidence Requirements

The workflow notebook is an audit record, not the execution environment for the task.

Do not rerun commands merely to reproduce them inside the notebook.

Record the commands actually executed and the output actually observed during the workflow.

Clearly distinguish:

* Actions actually performed.
* Results actually observed.
* Proposed actions.
* Inferred information.
* Unverified assumptions.

Do not fabricate notebook outputs, test results, terminal output, file states, diffs, or other evidence.

Do not omit failed attempts, errors, intermediate results, or deviations from the original plan.

## Sensitive Information

Do not include secrets, credentials, authentication tokens, private keys, or other sensitive information in the workflow notebook.

If such information appears during the workflow, redact or omit it while preserving enough context for the audit record to remain understandable.

## Audit Scope

Only the workflow for which the user explicitly activated this skill is audited.

Subsequent workflows are not automatically audited unless the user activates the skill again.

If the user explicitly states that workflow logging should remain enabled for a larger sequence of tasks, follow that requested scope until the user ends it.

## Non-Recursive Logging

Do not recursively audit the auditing process itself.

Creating, updating, inspecting, formatting, or validating the workflow notebook required by this skill does not constitute a separate workflow and must not trigger another workflow record.

Actions performed solely to maintain the current workflow notebook must never cause recursive logging, nested workflow records, or an audit loop.

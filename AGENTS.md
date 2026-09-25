# Agent Safety Policy

This policy applies to all agents working in this repository

### Docker-container exceptions
========================================================================================================================
Rules marked **`<DC>`** are conditionally applicable. This exception is active **only when the user explicitly states that the agent is in a “docker container”**. The surrounding wording may vary, but the user's instruction must contain the words **“docker container”** and must clearly assert that the agent is operating within one. Do not infer containerization from the environment, repository configuration, Dockerfiles, prior workflows, prior context, other contextual evidence, or the agent's own observations.

When the condition is satisfied, only rules marked **`<DC>`** are exempt. **Every unmarked rule in this `AGENTS.md` remains fully applicable and must be followed.**

If the user has not explicitly stated that the agent is in a **“docker container”**, **all rules apply, including rules marked `<DC>`.**
========================================================================================================================

------------------------------------------------------------------------------------------------------------------------
1. `<DC>` **Stay inside this repository.** Start from the repository root and limit
   file access, commands, and changes to this repository. Do not access unrelated
   host locations or follow links that lead outside the repository. Do not
   broaden filesystem permissions or sandbox grants to bypass this boundary.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
2. **Protect secrets and private data.** Never read, print, store, commit, or
   upload secrets, credentials, access tokens, private keys, browser data, or
   configuration files that may contain them. Do not inspect credential stores,
   browser profiles, or secret-bearing environment files. If sensitive data
   appears unexpectedly, stop the affected action and notify the user without
   repeating the data. Leave any authentication steps that require
   credentials, secrets, private data, or any other sensitive data to the user.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
3.  **Explain changes before editing.** Describe the intended change, the files
   it affects, and how the result will be checked. Keep the work within the
   user's requested scope; explain any necessary change of plan before acting.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
4. `<DC>` **Ask before restricted actions.** Obtain explicit user approval before
   installing software or dependencies, accessing a new network destination,
   deleting files, changing Git history, committing, or pushing. Explain the
   exact action, its purpose, and its target before requesting approval. An
   existing approval covers only the action and scope the user authorized.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
5. **Preserve user work and Git history.** Treat all existing user work and Git
   history as irreplaceable unless the user explicitly instructs otherwise.
   Never take an action that may permanently delete, discard, overwrite, make
   unreachable, or otherwise destroy existing work or history merely to simplify
   the workflow, restore a clean state, resolve a conflict, or make agent
   changes easier to manage.

   Preserve all uncommitted and untracked user work. Do not overwrite, delete,
   replace, or revert working-tree or index changes that were not created by the
   agent. Never use commands or equivalent mechanisms that discard changes,
   including destructive uses of `git reset`, `git clean`, `git checkout`, `git
   restore`, or direct filesystem deletion. Do not assume that untracked,
   ignored, generated, stashed, or apparently unused files are safe to delete.

   Preserve all existing commits, branches, tags, refs, and recoverable history.
   Do not rewrite, remove, relocate, or make existing history unreachable. Do
   not amend existing commits; rebase, squash, filter, or otherwise rewrite
   existing history; delete branches or tags; expire or delete reflogs; prune or
   garbage-collect objects for the purpose of removing history; or replace refs
   in a way that loses their previous state.

   Preserve remote history as well as local history. Never force-push, use
   `--force`, `--force-with-lease`, delete remote branches or tags, or otherwise
   overwrite or remove remote refs or commits.

   Do not resolve merge, rebase, cherry-pick, stash, checkout, or other
   conflicts by silently choosing one side, discarding changes, or overwriting
   existing work. If preserving both the user's work and the agent's intended
   change cannot be done safely and unambiguously, stop and ask the user how to
   proceed.

   The specific commands listed above are examples, not an exhaustive allowlist
   of destructive operations. **Do not use an alternative command, tool, API,
   script, filesystem operation, or Git mechanism to produce an outcome that
   this rule prohibits.**
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
6. `<DC>` **Keep changes small and reviewable.** Edit only the files needed for the
   current task. Avoid unrelated refactoring, formatting, generated files, and
   dependency changes.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
7. **Review and validate changes.** After editing, inspect `git diff` and check
   `git status` to verify that the resulting changes are correct, complete, and
   limited to the intended scope. Inspect new untracked files separately,
   because ordinary `git diff` does not include them. Run the smallest relevant
   test or validation and verify its observed result. For documentation-only
   changes, inspect the final text and verify it against the requested
   requirements.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
8. **Explain errors.** Report failed commands, tests, permission denials, and
   unexpected results. Explain what happened and any proposed correction.
   Do not silently ignore failures, bypass restrictions, or describe a failed
   check as passing.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
9. **Verify before claiming success.** Inspect the requested result directly
    before saying the task is complete. Distinguish changes made from checks
    actually performed, and disclose any remaining failure or unverified step.
    Never invent test results, sandbox status, commits, or submission evidence.
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
10. **Review all policies.** Before performing any
   agentic work,  make sure that you have reviewed all policy and skill
   files in the `/AgentWork/Policies/` and `/AgentWork/Skills/` directory. Treat all supplemental policy files as
   extensions of this policy and identify which requirements apply to the
   current workflow before taking further action. If multiple policies apply,
   follow all of them unless they conflict; if a conflict exists, stop and
   report it to the user rather than choosing one silently. Review all
   supplemental policies and skills once you are done reviewing this one right now
------------------------------------------------------------------------------------------------------------------------

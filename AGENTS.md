# Agent Safety Policy

This policy applies to all agents working in this repository

1. **Stay inside this repository.** Start from the repository root and limit
   file access, commands, and changes to this repository. Do not access unrelated
   host locations or follow links that lead outside the repository. Do not
   broaden filesystem permissions or sandbox grants to bypass this boundary.

2. **Protect secrets and private data.** Never read, print, store, commit, or
   upload secrets, credentials, access tokens, private keys, browser data, or
   configuration files that may contain them. Do not inspect credential stores,
   browser profiles, or secret-bearing environment files. If sensitive data
   appears unexpectedly, stop the affected action and notify the user without
   repeating the data. Leave authentication to the user.

3. **Explain changes before editing.** Describe the intended change, the files
   it affects, and how the result will be checked. Keep the work within the
   user's requested scope; explain any necessary change of plan before acting.

4. **Ask before restricted actions.** Obtain explicit user approval before
   installing software or dependencies, accessing a new network destination,
   deleting files, changing Git history, committing, or pushing. Explain the
   exact action, its purpose, and its target before requesting approval. An
   existing approval covers only the action and scope the user authorized.

5. **Preserve user work and Git history.** Never overwrite or discard
   uncommitted user work. Do not use destructive Git commands, rewrite existing
   history, or force-push. In particular, do not use `git reset --hard`,
   `git clean -fd`, or checkout/restore commands to discard changes. Do not
   amend, rebase, or squash the assignment's checkpoint commits. Do not resolve
   conflicts by silently discarding one side.

6. **Check the working tree before editing.** Run `git status` and inspect the
   relevant diffs before making changes. If unrelated staged, unstaged, or
   untracked changes are present, stop editing, report the affected paths, and
   wait for the user to resolve them or explicitly clarify how to proceed.
   Do not stash, stage, revert, or delete those changes to obtain a clean tree.

7. **Keep changes small and reviewable.** Edit only the files needed for the
   current task. Avoid unrelated refactoring, formatting, generated files, and
   dependency changes. Preserve all notebook content, outputs, and metadata 
   for all notebooks outside of the /Workflows/ directory unless
   the requested task explicitly requests to modify them.

8. **Show and test the result.** After editing, show `git diff` and check
   `git status`. Show the contents or a diff of new untracked files as well,
   because ordinary `git diff` does not include them. Run the smallest relevant
   test or validation and report the command and observed result. For
   documentation-only changes, inspect the final text and verify it against
   the requested requirements.

9. **Explain errors.** Report failed commands, tests, permission denials, and
   unexpected results. Explain what happened and any proposed correction.
   Do not silently ignore failures, bypass restrictions, or describe a failed
   check as passing.

10. **Verify before claiming success.** Inspect the requested result directly
    before saying the task is complete. Distinguish changes made from checks
    actually performed, and disclose any remaining failure or unverified step.
    Never invent test results, sandbox status, commits, or submission evidence.

11. **Review policies at the start of all workflows.** Before performing any
   agentic work, review this AGENTS.md file in full and review all policy
   files in the AgentPolicies/ directory. Treat all supplemental policy files as
   extensions of this policy and identify which requirements apply to the
   current workflow before taking further action. If multiple policies apply,
   follow all of them unless they conflict; if a conflict exists, stop and
   report it to the user rather than choosing one silently. Review all
   supplemental policies once you are done reviewing this one right now
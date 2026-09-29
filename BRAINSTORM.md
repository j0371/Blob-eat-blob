# Blob-Eat-Blob Brainstorm

Planning notes for the Shark ability and blob state work on `feat/lunge-suppression`. This is a plan, not a spec. Nothing here is committed to yet, and the game working comes before any of it being pretty.

Current values, all defined in `HelperScripts/GameConfig.cs`:

| Constant | Value |
| --- | --- |
| `SuppressionSeconds` | 3 |
| `SuppressedSpeedFactor` | 1/6 |
| `LungeSpeedMultiplier` | 1.5 |

---

## 1. State management

**Problem.** State changes are spread across several places. `Blob` has `EnterNormalState`, `EnterSuppressedState`, and `SetBlobState`. `Shark` has one `Enter...` method per lunge and deflect state, and each of those also does side effects such as launching the blob, locking movement, and changing the blob's state. Timer callbacks, the eat callback, and the input checks all jump between states from different directions. It works, but it is hard to see from one place what causes what.

**Ideas.**

- **One blob state event on every transition.** Add a single event on `Blob`, something like `StateChanged`, that fires whenever `BlobState` changes. Anything that cares (Shark, HUD, effects, sounds) subscribes to that instead of adding its own callback. This would replace the growing pile of one-off events and direct calls. It should fire from one place only, the point where `BlobState` is assigned.
- **Inline `OnLungeEnded` into the Lunging timer callback.** `OnLungeEnded` is only used as the Lunging timeout callback. It is two lines: refresh if the lunge hit something, otherwise enter recovery. Passing it as a lambda where the Lunging timer is created removes one named method and puts the whole lunge ending in the place you would look for it.
- **Move side effects out of state methods and into input/action handlers.** State methods should mostly set the state and start the timer. Launching the blob, locking movement, and setting Protected belong to the action that caused the transition (releasing the attack button, pressing deflect). This keeps the `Enter...` methods safe to call from anywhere, for example when eating refreshes an ability, without accidentally launching the blob.
- **Rename `EnterLungeState` to `SetLungeState`.** It only sets the state field and manages the timer, and it is easy to confuse with `EnterLungingState`. The new name matches what it does. Purely cosmetic, so do it only when already touching those lines.

**Open questions.**

- If side effects move out, where does the launch code live? A small `Lunge()` method called from the charging case is probably enough.
- Does the single event replace `EnemyEaten`, or stay alongside it? Eating is an action, not a state, so probably it stays.

**Scope warning.** This is the biggest item here and the easiest to over-build. Do the rename and the inline first, because they are tiny. Leave the single state event until the game is stable.

---

## 1a. Master blob state event (FUTURE REFACTOR, NOT IMPLEMENTED)

> **Status: planned only.** Nothing in this section exists in the code yet. No `.cs` file has been changed for it. Do not start it until the game is stable and turned in, unless something blocks the game.

**Idea.** One event on `Blob` that fires on **every** state transition, not just suppression, and carries the new state. Something like `public event Action<BlobStates> StateChanged;`. A single master state manager subscribes to that one event and handles every cross-cutting reaction in one place: cooldowns, HUD updates, ability locks, speed changes, and so on.

**What it replaces.**

- The virtual `EnterNormalState` and `EnterSuppressedState` pattern on `Blob`, and the overrides in `Player` and `NonPlayer`. Speed changes would move into the manager instead of living in per-class overrides.
- Ad-hoc callbacks such as `OnLungeEnded` and `OnSuppressionEnded`, and the one-off direct calls between `Shark` and `Blob`.
- The `SetBlobState` switch that routes some states through Enter methods and sets the rest directly. Every state would go through the same path.

**Shape of it.**

- State methods only do two things: flip the state and fire the event. They do not launch the blob, lock movement, change speed, or start other timers themselves.
- The manager is the only subscriber. It reads the new state and reacts. For example, on Suppressed it applies the slow speed. On Normal it restores speed. On Protected it can suppress incoming affliction.
- Input and action handlers (release attack, press deflect) still own the actions that cause a transition, such as the lunge launch. This lines up with the "move side effects out of state methods" idea in section 1.

**Why it could be worth it.**

- One place to look when asking "what happens when a blob becomes Suppressed".
- Adding a new state or a new reaction (Cloaked, Shrouded, a Squid ink effect, a sound, a HUD flash) means adding a case in the manager instead of touching several classes.
- A clean separation: state changes are facts, and reactions are the manager's job.

**Risks and open questions.**

- **Ordering.** Anything that changes state inside the manager's handler fires the event again. Decide up front whether transitions are queued or whether re-entry is allowed, and guard against infinite loops.
- **Timers.** Decide who owns the suppression and lunge timers. The most consistent answer is the manager, started when it sees the relevant state. That is a bigger change than it sounds.
- **Which blobs get a manager.** Player, NonPlayer, and Shark blobs all use states. A manager per blob is simplest, and a shared one needs to know which blob a state belongs to.
- **Event args.** The new state alone may not be enough. The old state, and who caused the change, could be useful for things like the afflicter in `Afflicted`.
- **Cost.** This touches `Blob`, `Player`, `NonPlayer`, and `Shark` at once. It is the largest item in this document, and it is the easiest to over-build.

**Recommended order if it ever happens.**

1. Finish and playtest the current version first.
2. Add the event and fire it from the single place `BlobState` is assigned, with no subscribers yet.
3. Add the manager and move one reaction into it, such as speed on Suppressed and Normal. Check that the game plays the same.
4. Move the rest one reaction at a time, deleting the old callback each time.
5. Remove the virtual Enter methods last.

---
## 2. Affliction event

**Current behavior.** `Blob` has `public event Action<Blob> Afflicted`. In `OnEnemyCollide`, when the other blob's `AfflictingState` is Suppressing, this blob raises `Afflicted` with that enemy. Each blob subscribes to its own event in `_Ready`, and the virtual `OnAfflicted` decides what happens: return if Protected, otherwise call `Suppress()`.

**Idea.** Consider renaming `Afflicted` to `Afflicting`. Reason: the event describes an afflicting blob acting on this one. The counter-argument is that `Afflicted` reads naturally as "I was afflicted", and the subscriber is the victim. Pick one and move on.

**Principle.** The target decides the reaction. The afflicter only announces contact and never reaches into another blob's state. That keeps room for other reactions later, such as a shielded blob ignoring it, or a Squid ink effect having a different reaction than a Shark lunge.

**Watch-outs.**

- Both blobs' detection areas can fire on the same contact. Make sure a blob only raises the event when the *other* blob is the afflicter, which the current check already does.
- The event fires before the eat check in `OnEnemyCollide`. Suppression then applies before the eat condition is evaluated. That is fine today, but keep the order in mind if the eat rule changes.
- `Protected` is checked by the target in `OnAfflicted`. Do not duplicate that check on the afflicter's side.

---

## 3. Eat refresh

**Rule.** Eating gives the Shark a fresh start.

- Eating resets all cooldowns. That means the lunge cooldown and the deflect cooldown.
- Reset `lungeHitEnemy` at the start of every lunge, in `EnterLungingState`, so a hit from a previous lunge never carries over.
- Eating mid-lunge skips recovery and suppression. If the lunge ate something, when it ends the Shark goes straight back to attack ready instead of into recovery.

**How it works now.**

- `Blob.EatEnemy` grows the blob, frees the enemy, clears suppression, and raises `EnemyEaten`.
- `Shark.OnBlobAteEnemy` handles the event. While Lunging it sets `lungeHitEnemy`. In LungeRecovery or OnCooldown it refreshes the lunge ability right away. If the deflect is on cooldown, it resets it to ready.
- When the Lunging timer ends, `lungeHitEnemy` decides between refresh and recovery.

**Open questions.**

- Should eating during an active Deflecting window end the deflect early, or just leave it running? Current behavior leaves it running.
- Should eating during LungeCharging do anything? Currently no.

---

## 4. Suppression

**Goal.** Suppression is a slowdown, not a freeze.

- **Remove `suppressionId`.** No counter and no id check on the timer.
- **Cancel and restart the timer.** Keep a reference to the current suppression timer. When `Suppress()` is called again, disconnect the old timer's `Timeout` and create a new one, so the full duration restarts. When the timer fires, if the blob is still Suppressed, call `EnterNormalState`. Otherwise do nothing. A Godot `SceneTreeTimer` cannot be stopped directly, so disconnecting the signal is how to cancel it.
- **Speed factor is 1/6, not zero.** A suppressed blob moves at one sixth of normal speed. It can still move and it can still be eaten.
- **Virtual entry methods.** `EnterSuppressedState` and `EnterNormalState` are virtual on `Blob`. `Player` and `NonPlayer` override them to set speed, using `SuppressedSpeedFactor` and the base speed. This is how each blob type decides what suppression means for it.
- `ClearSuppression()` cancels the timer and returns the blob to Normal. It is called when the blob eats something.

**Watch-outs.**

- `SetBlobState` routes Normal and Suppressed through the virtual methods so speed always updates. Any new state that changes speed should do the same.
- A deflect or lunge ending can set the blob back to Normal while a suppression is pending. Check that the interactions feel right in play before changing anything.

---

## 5. Timers

**Lunge countdowns use `CreateTimer`.**

- Lunging, LungeRecovery, and OnCooldown each create a one-shot timer through `GetTree().CreateTimer(seconds)` and connect `Timeout` to the next transition.
- Entering any new lunge state drops the previous timer by clearing the reference, and a callback from an old timer does nothing when it sees a newer timer in the field.
- The HUD cooldown bar reads the remaining time from the timer instead of a hand-decremented variable.

**Deflect stays per-frame.** The Deflecting and OnCooldown countdowns are left as hand-decremented timers in `ManageDefendState`. Deflect is short and simple, and it is not worth changing until the lunge version has been playtested.

**Charging stays per-frame.** The charge counts upward each physics frame. The charge time is needed at release to compute the lunge duration, and the HUD bar reads it. A `CreateTimer` cannot count up, so this one stays.

**Also per-frame.** The LungeRecovery slide, which decays the blob's velocity toward zero each frame.

---

## 6. Principles

- Simple, readable code. Functional beats pretty.
- No over-engineering. No new systems or refactors unless something blocks the game.
- Underscore prefix only on read-only, const, and assigned-once fields. New mutable fields and properties have no underscore. Existing names are left alone.
- One feature at a time, and finish it before starting the next.
- Explain before editing.
- Sizes, timings, and names live in `GameConfig`.
- No commits, pushes, or history changes unless explicitly asked.

---

## 7. Known gaps

- **Player suppression needs a playtest.** The acceleration and deceleration movement interacts with the reduced speed. Confirm that a suppressed player feels slow, and does not slide strangely or get stuck.
- **Non-player refresh limits.** Only blobs with a Shark child get the lunge and deflect refresh. Check what non-player blobs do on eating, and whether they need any of this at all.
- **Lunge runs its full duration after a hit.** When a lunge eats something, it keeps going until the timer ends and only then skips recovery. Decide whether it should end early.
- **`SuppressionSeconds = 3`.** Placeholder. Tune in play.
- **`SuppressedSpeedFactor = 1/6`.** Placeholder. Tune in play.
- **`LungeSpeedMultiplier = 1.5`.** Set by hand after testing. Revisit only if the lunge feels wrong.
- **Deflect ending mid-lunge.** `EnterDeflectCooldown` sets the blob to Normal, which can wipe Protected or Suppressed if it lands during a lunge or recovery.
- **Right-click during a charge.** LungeCharging is not part of the deflect lockout, so a deflect can start mid-charge.
- **Spawner-created blobs don't move.** `SetIsPreSpawning(false)` disables physics processing, so the flag looks inverted. This is likely why the population dies off.
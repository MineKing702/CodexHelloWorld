# AwesomeGame2 Architecture Rules

## Single Source of Truth
- `GameSave` is the single source of truth for all mutable gameplay state.
- All mutable gameplay state must live inside `GameSave`.
- All gameplay managers operate on `GameSave`.
- Managers may read and mutate `GameSave`, but must not own mutable gameplay state in instance fields, static fields, singletons, viewmodels, console classes, or Unity-facing classes.

## ViewModel Policy
- ViewModels are read-only projections derived from `GameSave`.
- ViewModels must not store gameplay logic or mutable state.

## Client Policy
- Console and future Unity clients must not own gameplay state.
- Clients should issue commands and render viewmodels from shared-library manager outputs.

## Save/Load Quality Gate
- Save/load JSON round-trip behavior must be covered by automated tests.

## Future Systems Rule
- Future gameplay systems must add all persistent mutable state to `GameSave`.

# ADR 0001: Layered MVVM desktop app with Avalonia

- **Status:** Accepted
- **Date:** 2026-11-04, recorded afterward from the code

## Context

ClassPulse is a single-user desktop app for teachers, and it stores its data locally. The original reasoning for the technology choice was not recorded.

## Decision

- Build the UI with Avalonia and MVVM, using CommunityToolkit.Mvvm source generators.
- Keep the layers in the order Views, ViewModels, Services, Data.
- Let view models depend on service interfaces.
- Keep domain validation in entities, with private setters.
- Map view models to views by naming convention with `ViewLocator`.

## Consequences

- View models and services can be tested without a UI, using stubs and test databases.
- Class and namespace names are part of the navigation contract.
- Reflection in `ViewLocator` is not safe for trimming.
- Existing exceptions to the layering are listed as TD-02 in [tech-debt.md](../tech-debt.md).

# Multi-targeting the Z* libraries (.NET 8 / 9 / 10)

**Date:** 2026-09-18
**Status:** Approved
**Scope:** ZDatabase, ZSecurity, ZWebAPI

## Goal

Ship the Z* libraries against the current .NET LTS (.NET 10 / EF Core 10) without
dropping consumers still on .NET 8 or .NET 9. Add unit test projects to ZSecurity and
ZWebAPI, which currently have none, at >= 90% line coverage.

## Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Target frameworks | `net8.0;net9.0;net10.0` | Widest reach. Verified to build and test clean on all three. |
| EF Core floor per TFM | 8.0.11 / 9.0.0 / 10.0.0 | A floor is a *minimum*. NuGet resolves the lowest match, so the floor is what consumers actually get. |
| Version layout | Root `Directory.Build.props` | One TFM-to-version map per repo; library and tests cannot drift apart. |
| CI test coverage | Run the suite on every TFM | Catches EF Core behavioural differences, not just compile breaks. |
| Release type | `feat` -> minor | Purely additive; no existing consumer breaks. |

### Why 8.0.11 and not 8.0.0

EF Core 8.0.0 pulls `Microsoft.Extensions.Caching.Memory` 8.0.0, which carries a high
severity advisory (NU1903, GHSA-qj66-m88j-hmgj). Because NuGet resolves a floor to the
lowest satisfying version, declaring 8.0.0 would hand that advisory to every net8.0
consumer. 8.0.11 is the verified point at which the warning clears.

## Architecture

Each repo gets the same three-part shape.

### 1. Root `Directory.Build.props`

```xml
<Project>
  <PropertyGroup>
    <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
  </PropertyGroup>
  <PropertyGroup>
    <EFCoreVersion Condition="'$(TargetFramework)' == 'net8.0'">8.0.11</EFCoreVersion>
    <EFCoreVersion Condition="'$(TargetFramework)' == 'net9.0'">9.0.0</EFCoreVersion>
    <EFCoreVersion Condition="'$(TargetFramework)' == 'net10.0'">10.0.0</EFCoreVersion>
  </PropertyGroup>
</Project>
```

MSBuild passes `TargetFramework` as a global property during the inner (per-TFM) build,
so the conditions evaluate correctly during both restore and build. Verified, not assumed.

Repos that consume sibling Z* packages add a matching map for those (see Release train).

### 2. Project files

Drop `<TargetFramework>`; replace hardcoded package versions with `$(EFCoreVersion)` and
the sibling-package equivalents. `<Version>` stays in the library `.csproj` because
`release.config.cjs` lists it as a semantic-release git asset.

### 3. CI workflow

`DOTNET_VERSION: 9.0.x` becomes a multi-line list so the test runner has all three
runtimes available:

```yaml
env:
  DOTNET_VERSION: |
    8.0.x
    9.0.x
    10.0.x
```

Build, test, pack and publish commands are unchanged. `dotnet test` discovers and runs
every TFM on its own.

## Release train

The three libraries form a dependency chain:

```
ZDatabase  <-  ZSecurity  <-  ZWebAPI
```

Published today, `ZDatabase 1.5.1` and `ZSecurity 1.3.0` ship **net9.0 assets only**. A
net8.0 leg in a downstream repo therefore cannot restore them (NU1202). Multi-targeting
must land in dependency order:

1. **ZDatabase** -> publish (first version carrying net8.0/net10.0 assets)
2. **ZSecurity** -> bump its ZDatabase floor to that version, multi-target, publish
3. **ZWebAPI** -> bump its ZDatabase and ZSecurity floors, multi-target, publish

Downstream CI stays red between steps until the upstream package is on NuGet.org. This is
inherent to the chain, not a defect in the change.

**Local verification strategy:** rather than hand-waving the downstream legs, each
upstream library is packed locally and served from a scratch folder feed via a
`NuGet.config` that is *not committed*. That proves ZSecurity and ZWebAPI genuinely build
and test on all three TFMs before anything is published.

## Test projects

ZDatabase already has `ZDatabase.UnitTests` (100 tests). ZSecurity and ZWebAPI get new
projects built to the identical methodology.

### Conventions (extracted from ZDatabase.UnitTests)

- **Naming:** `MemberUnderTest_Pass_Scenario` / `MemberUnderTest_Fail_Scenario`
- **Body:** explicit `// Arrange`, `// Act`, `// Assert` blocks
- **Assertions:** FluentAssertions 7.0.0 (`.Should()`), never raw `Assert.*`
- **Substitutes:** NSubstitute (`Substitute.For<T>()`)
- **Docs:** XML doc comment on every test class and every test method
- **Layout:** test folders mirror source folders; `Fakes/` for concrete stand-ins,
  `Factories/` for builders; both `internal`, test classes `public`
- **`GlobalUsings.cs`:** global usings for FluentAssertions, NSubstitute and Xunit, plus
  `[assembly: ExcludeFromCodeCoverage]`
- **Namespaces:** block-scoped, matching the folder path

### Package set

`xunit` 2.9.2, `xunit.runner.visualstudio` 3.0.0, `Microsoft.NET.Test.Sdk` 17.12.0,
`FluentAssertions` 7.0.0, `NSubstitute` 5.3.0, `coverlet.collector` 6.0.2. FluentAssertions
is held at 7.0.0 deliberately: 8.x moved to a commercial licence.

### Coverage

Target **>= 90% line coverage** per repo, measured with coverlet. Edge cases are in scope,
not just happy paths: null and empty inputs, boundary values, exception paths, and the
guard clauses that distinguish these libraries' behaviour.

Code genuinely not worth testing (DI registration one-liners, plain DTOs) is marked
`[ExcludeFromCodeCoverage]`, consistent with ZDatabase's existing use of that attribute,
so the percentage reflects real logic rather than being inflated or unfairly depressed.

## Known API differences across EF Core majors

Only one surfaced across the whole surface area:

- `IReadOnlyEntityType.GetQueryFilter()` is obsolete in EF Core 10 (superseded by named
  query filters and `GetDeclaredQueryFilters()`). It appears in ZDatabase *test* code
  only. Handled with an `#if NET10_0_OR_GREATER` helper so every TFM compiles warning-free.
  The library's own `HasQueryFilter(...)` call is not obsolete and is untouched.

## Verification

Each repo must reach this bar before being called done:

1. `dotnet build -c Release` -> 0 errors, 0 warnings, all three TFMs
2. `dotnet test -c Release` -> all tests pass on all three TFMs
3. `dotnet pack` -> package contains `lib/net8.0`, `lib/net9.0`, `lib/net10.0` and correct
   per-TFM dependency groups
4. Coverage >= 90%

## Risks

| Risk | Mitigation |
|---|---|
| Downstream CI red until upstream publishes | Inherent to the chain; release in dependency order. Flagged in each PR. |
| .NET 8 EOL 2026-11-10, .NET 9 EOL'd 2026-05 | Documented in each README so consumers know what they are standing on. Dropping a TFM later is a two-line edit in `Directory.Build.props`. |
| Third-party packages lacking net8.0 assets | Checked up front: AutoMapper 14.0.0, ClosedXML 0.104.2 and QuestPDF 2024.12.3 all support net8.0. |
| New tests locking in current behaviour, bugs included | Tests are written against documented/intended behaviour. Anything that looks like a genuine bug is reported rather than silently enshrined. |

## Out of scope

- Upgrading third-party packages (AutoMapper, ClosedXML, QuestPDF) beyond what
  multi-targeting requires
- Refactoring library logic
- Changing the Trusted Publishing release pipeline beyond the SDK version list

---

## Outcome (2026-09-19)

Delivered across all three repos. Every figure below was observed, not estimated.

| Repo | Branch | Tests | Line coverage | Build |
| --- | --- | --- | --- | --- |
| ZDatabase | `claude/dotnet-efcore-upgrade-compat-514ccd` | 143 x 3 TFMs | **100.0%** (439/439) | 0 warnings |
| ZSecurity | `features/dotnet-multitargeting` | 69 x 3 TFMs | **100.0%** (71/71) | 0 warnings |
| ZWebAPI | `features/unit-tests` | 258 x 3 TFMs | **99.6%** (563/565) | 1 pre-existing warning |

ZSecurity and ZWebAPI had no test projects before this work.

Each repo was verified end to end: build on all three TFMs, suite green on all three, `dotnet pack`
producing `lib/net8.0` + `lib/net9.0` + `lib/net10.0` with correct per-TFM dependency groups.
ZSecurity and ZWebAPI were built against locally packed upstream packages served from a scratch
folder feed, via a `NuGet.config` that was deliberately not committed.

### Deviation from plan

Three third-party packages were checked for net8.0 support up front and all had it, so no
dependency bumps were needed. One library change was made beyond the plan: ZDatabase's
`EntityEntryExtensions.IsEnumerableTypeSubclassOf` was deleted. It was `internal` with no callers,
and `typeof(IEnumerable<>).IsAssignableFrom(t)` is `false` for every closed type, so its guard
always tripped and it could only ever return `false`. Removing provably unreachable code was
preferred to writing a test blessing a broken no-op.

### Tooling note

The coverlet collector (`--collect:"XPlat Code Coverage"`) rewrites the library assembly in place,
and Windows Application Control intermittently blocks the modified unsigned binary (`0x800711C7`),
which surfaces as a silent 0% report rather than an error. Coverage figures above were taken with
`dotnet-coverage`, which instruments through the CLR profiler instead.

### Findings raised, not fixed

Deliberately left for separate changes, since none belong in a multi-targeting PR:

1. **ZSecurity `ActionTypes.OnlyAdmins` does not restrict to admins** (`SecurityHandler.cs:90`).
   `actionsToCheck` is seeded with `actionName` before the attribute is read, so an admin-only
   method also accepts the plain action permission. Security-relevant.
2. **ZWebAPI unsigned integer filters use the signed readers**
   (`SummaryParametersExtensions.cs:161/168/175`): `UInt16`/`UInt32`/`UInt64` fall back to
   `GetInt16()`/`GetInt32()`/`GetInt64()`, so large values overflow and the boxed value has the
   wrong CLR type.
3. **ZWebAPI enums sent as JSON strings do not work** (`SummaryParametersExtensions.cs:187`).
   `Type.GetTypeCode` returns the underlying code for an enum, so the `IsEnum` branch is
   unreachable and `"Active"` lands in `case TypeCode.Int32` and throws.
4. **AutoMapper 14.0.0 carries a high severity advisory** (NU1903, GHSA-rvv3-g6hj-g44x, DoS via
   uncontrolled recursion). Patched only in 15.1.1+ / 16.1.1+, which crosses AutoMapper's v15
   commercial licensing change — a business decision, not a technical one.
5. **ZWebAPI's release workflow is still the old single-job style**, unlike the Trusted Publishing
   pipeline ZDatabase and ZSecurity use, and has no pull-request validation trigger.

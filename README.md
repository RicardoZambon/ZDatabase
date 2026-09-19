# ZDatabase
ZDatabase is a library that brings additional functionalities for Entity Framework

## Supported frameworks

ZDatabase ships assets for .NET 8, .NET 9 and .NET 10, each against the matching EF Core major:

| Target framework | EF Core (minimum) | Notes |
| --- | --- | --- |
| `net10.0` | 10.0.0 | LTS, supported until November 2028 |
| `net9.0` | 9.0.0 | STS, out of support since May 2026 |
| `net8.0` | 8.0.11 | LTS, out of support after November 2026 |

The EF Core versions above are floors, not pins: referencing a newer patch or minor of EF Core
within the same major works as expected.

The `net8.0` floor is 8.0.11 rather than 8.0.0 because EF Core 8.0.0 depends on a
`Microsoft.Extensions.Caching.Memory` carrying a high severity advisory
([GHSA-qj66-m88j-hmgj](https://github.com/advisories/GHSA-qj66-m88j-hmgj)).

The `net9.0` and `net8.0` targets exist for compatibility with applications that have not yet
moved to .NET 10. Both of those runtimes are at or near end of support, so new projects should
target `net10.0`.

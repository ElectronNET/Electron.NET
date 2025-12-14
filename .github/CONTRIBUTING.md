# Contributing

## Project Scope

The Electron.NET project ultimately tries to provide a framework for developing cross-platform client applications on the basis of .NET and Electron. Anything that is related to this goal will be considered. The project aims to be as close to Electron with .NET as a basis as possible. If your contribution does not reflect that goal, the chances of accepting it are limited.

## Code License

This is an open source project falling under the [MIT License](../LICENSE). By using, distributing, or contributing to this project, you accept and agree that all code within the Electron.NET project and its libraries are licensed under MIT license.

## Becoming a Contributor

Usually appointing someone as a contributor follows this process:

1. An individual contributes actively via discussions (reporting bugs, giving feedback to existing or opening new issues) and / or pull requests
2. The individual is either directly asked, invited or asks for contributor rights on the project
3. The individual uses the contribution rights to sustain or increase the active contributions

Every contributor might have to sign the contributor's license agreement (CLA) to establish a legal trust between the project and its contributors.

## Working on Electron.NET

### Issue Discussion

Discussion of issues should be placed transparently in the issue tracker here on GitHub.

* [General issues, bugs, new features](https://github.com/ElectronNET/Electron.NET/issues)
* [General discussions, help, exchange of ideas](https://github.com/ElectronNET/Electron.NET/discussions)

### Modifying the code

Electron.NET and its libraries uses features from the latest versions of C# (e.g., C# 10). You will therefore need a C# compiler that is up for the job.

1. Fork and clone the repo.
2. First try to build the ElectronNET.Core library and see if you get the tests running.
3. You will be required to resolve some dependencies via NuGet.

The build system of Electron.NET uses NUKE.

### Code Conventions

Most parts in the Electron.NET project are fairly straight forward. Among these are:

* Always use statement blocks for control statements, e.g., in a for-loop, if-condition, ...
* You may use a simple (throw) statement in case of enforcing contracts on argument
* Be explicit about modifiers (some files follow an older convention of the code base, but we settled on the explicit style)

### Development Workflow

1. If no issue already exists for the work you'll be doing, create one to document the problem(s) being solved and self-assign.
2. Otherwise please let us know that you are working on the problem. Regular status updates (e.g. "still in progress", "no time anymore", "practically done", "pull request issued") are highly welcome.
3. Create a new branch—please don't work in the `main` branch directly. It is reserved for releases. We recommend naming the branch to match the issue being addressed (`feature/#777` or `issue-777`).
4. Add failing tests for the change you want to make. Tests are crucial and should be taken from W3C (or other specification).
5. Fix stuff. Always go from edge case to edge case.
6. All tests should pass now. Also your new implementation should not break existing tests.
7. Update the documentation to reflect any changes. (or document such changes in the original issue)
8. Push to your fork or push your issue-specific branch to the main repository, then submit a pull request against `develop`.

Just to illustrate the git workflow for Electron.NET a little bit more we've added the following graphs.

Initially, Electron.NET starts at the `main` branch. This branch should contain the latest stable (or released) version.

Here we now created a new branch called `develop`. This is the development branch.

Now active work is supposed to be done. Therefore a new branch should be created. Let's create one:

```sh
git checkout -b feature/#777
```

There may be many of these feature branches. Most of them are also pushed to the server for discussion or synchronization.

```sh
git push -u origin feature/#777
```

Now feature branches may be closed when they are done. Here we simply merge with the feature branch(es). For instance the following command takes the `feature/#777` branch from the server and merges it with the `develop` branch.

```sh
git checkout develop
git pull
git pull origin feature/#777
git push
```

Finally, we may have all the features that are needed to release a new version of Electron.NET. Here we tag the release. For instance for the 1.0 release we use `v1.0`.

```sh
git checkout main
git merge develop
git tag v1.0
```

(The last part is automatically performed by our CI system. Don't tag manually.)

### Versioning

The rules of [semver](http://semver.org/) don't necessarily apply here, but we will try to stay quite close to them.

Prior to version 1.0.0 we use the following scheme:

1. MINOR versions for reaching a feature milestone potentially combined with dramatic API changes
2. PATCH versions for refinements (e.g. performance improvements, bug fixes)

After releasing version 1.0.0 the scheme changes to become:

1. MAJOR versions at maintainers' discretion following significant changes to the codebase (e.g., API changes)
2. MINOR versions for backwards-compatible enhancements (e.g., performance improvements)
3. PATCH versions for backwards-compatible bug fixes (e.g., spec compliance bugs, support issues)

#### Code style

Regarding code style like indentation and whitespace, **follow the conventions you see used in the source already.** In general most of the [C# coding guidelines from Microsoft](https://msdn.microsoft.com/en-us/library/ff926074.aspx) are followed. This project prefers type inference with `var` to explicitly stating (redundant) information.

It is also important to keep a certain `async`-flow and to always use `ConfigureAwait(false)` in conjunction with an `await` expression.

## Backwards Compatibility

We always try to remain backwards compatible beyond the currently supported versions of .NET.

For instance, in December 2025 there have been activity to remove .NET 6 support from the codebase. We rejected this. Key points:

1. We have absolutely no need to drop `.net6` support. It doesn't hurt us in any way.
2. Many are still using `.net6`, including Electron.NET (non-Core) users. It doesn't make sense to force them to update two things at the same time (.NET + Electron.NET).
3. We MUST NOT and NEVER update `Microsoft.Build.Utilities.Core`. This will make Electron.NET stop working on older Visual Studio and MSBuild versions. There's are also no reasons to update it in the first place.

It's important to note that the Microsoft label of "Out of support" on .NET has almost no practical meaning. We've rarely (if ever) seen any bugs fixed in the same .NET version which mattered. The bugs that all new .NET versions have are much worse than mature .NET versions which are declared as "out of support". Keep in mind that the LTS matters most for active development / ongoing supported projects. If, e.g., a TV has been released a decade ago it most likely won't be patched. Still, you might want to deploy applications to it, which then naturally would involve being based on "out of support" versions of the framework.

TL;DR: Unless there is a technical reason (e.g., a crucial new API not being available) we should not drop "out of support" .NET versions. At the time of writing (December 2025) the minimum supported .NET version remains at `.net6`.

## Timeline

**All of this information is related to ElectronNET.Core pre-v1!**

We pretty much release whenever we have something new (i.e., do fixes such as a 0.1.1, or add new features, such as a 0.2.0) quite quickly.

We will go for a 1.0.0 release of this as early as ~mid of January 2026 (unless we find some critical things or want to extend the beta phase for ElectronNET.Core). This should be sufficient time to get some user input and have enough experience to call it stable.

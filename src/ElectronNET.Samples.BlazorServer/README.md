# Electron.NET Sample: Blazor Server

This sample shows how to structure a Blazor Server app with Electron.NET, and how the different parts come together:

- **Client-side interactivity** — regular Blazor components and JS interop
- **Electron control** — controlling the window from components, and reacting to window events
- **Native capabilities** — dialogs, notifications, clipboard, shell

## Structure

```
Program.cs                  ASP.NET Core + Blazor + Electron.NET setup
Desktop/                    Everything Electron-specific
├── ElectronStartup.cs      creates the main window and the application menu
├── WindowService.cs        Electron control (BrowserWindow API) + window state
├── NativeService.cs        native capabilities (dialogs, notifications, clipboard, shell)
└── AppEvents.cs            events from Electron to the Blazor UI (menu clicks, window events)
Components/
├── App.razor               root document, global InteractiveServer render mode
├── Layout/MainLayout.razor navigation; handles navigation requests from the menu
└── Pages/
    ├── Interactivity.razor plain Blazor + JS interop (no Electron involved)
    ├── WindowControl.razor uses WindowService, listens to AppEvents
    └── Native.razor        uses NativeService
wwwroot/                    CSS and the JS interop module
```

## Key points

### 1. Keep Electron out of the components

Components never call `Electron.*` directly. They inject `WindowService` / `NativeService` instead. This keeps the Electron-specific code in one place and makes it obvious which parts of the UI need Electron. Both services expose `IsAvailable`, so the app still works (with reduced functionality) when opened in a normal browser. That can be useful for UI development.

### 2. Calls from Blazor to Electron are just .NET calls

With Blazor Server, the component code runs in the same .NET process as the Electron.NET API. There is no extra IPC layer to build: a click handler can call `Electron.Dialog.ShowOpenDialogAsync(...)` (through the service) and then read the file with `File.ReadAllTextAsync`.

### 3. Events from Electron to Blazor go through a singleton

Electron callbacks, such as menu clicks, window events or notification clicks, run on an Electron.NET thread outside of any Blazor circuit. To get them into the UI:

1. A singleton (`AppEvents`) raises a .NET event.
2. Components subscribe in `OnInitialized` and unsubscribe in `Dispose`.
3. Handlers switch to the component's renderer with `InvokeAsync(...)` before touching state, calling `StateHasChanged` or navigating.

`MainLayout.razor` uses this to turn application menu clicks into Blazor navigation. `WindowControl.razor` uses it to show live window state.

### 4. Blazor-specific window options

```csharp
var options = new BrowserWindowOptions
{
    Show = false,             // show only after the first render, see OnReadyToShow
    IsRunningBlazor = true,   // hides Node.js globals that would break blazor.web.js
};
```

### 5. Render mode

A desktop app has no reason to mix render modes, so `App.razor` makes the whole app interactive (`InteractiveServer`). Only the error page opts out with `[ExcludeFromInteractiveRouting]`.

### 6. Edit menu

When you replace the default application menu, include the `editMenu` role. Otherwise copy and paste shortcuts don't work in text inputs on macOS.

## Run

```sh
dotnet run
```

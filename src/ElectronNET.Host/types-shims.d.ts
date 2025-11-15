// Simplified ambient module declarations to silence unresolved external module TS errors
// and missing extended tsconfig references inside node_modules.
// This is internal only.
declare module 'electron-updater' {
  export const autoUpdater: any;
}

declare module '@ljharb/tsconfig';

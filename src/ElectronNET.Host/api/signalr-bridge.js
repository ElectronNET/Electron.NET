/**
 * SignalR connection module for Electron.NET
 *
 * This module provides a Socket.IO-compatible interface for SignalR communication.
 * Key features:
 * - Mimics Socket.IO's on() and emit() methods for compatibility with existing API modules
 * - Handles event registration and propagation between Electron and .NET
 * - Event args are always passed as arrays to match C# ElectronEvent(string, object[]) signature
 * - Spreads args when calling handlers to match Socket.IO behavior
 * - Supports automatic reconnection with configurable logging level
 */
const signalR = require("@microsoft/signalr");
const { app } = require("electron");
const { logger } = require("../logger");

// Flag to track if we've already initiated shutdown due to EPIPE
let isShuttingDownFromEPIPE = false;

// Handle EPIPE errors at the process stdout/stderr level
// When the pipe breaks (e.g., .NET process terminates), quit Electron gracefully
const handlePipeError = (err) => {
  if (err.code === "EPIPE" || err.code === "ERR_STREAM_WRITE_AFTER_END") {
    // Pipe is broken - the .NET process has terminated
    if (!isShuttingDownFromEPIPE) {
      isShuttingDownFromEPIPE = true;
      // Give a brief moment for any pending operations, then quit
      setImmediate(() => {
        if (app && app.quit) {
          app.quit();
        }
      });
    }
    return;
  }
  // Re-throw other errors
  throw err;
};

// Suppress EPIPE errors at the process stdout/stderr level
if (process.stdout && !process.stdout.listenerCount("error")) {
  process.stdout.on("error", handlePipeError);
}

if (process.stderr && !process.stderr.listenerCount("error")) {
  process.stderr.on("error", handlePipeError);
}

// Custom logger for SignalR that uses environment-aware logging
class SafeLogger {
  constructor(minLevel) {
    this.minLevel = minLevel || signalR.LogLevel.Warning;
  }

  log(logLevel, message) {
    // Skip if below minimum level
    if (logLevel < this.minLevel) {
      return;
    }

    switch (logLevel) {
      case signalR.LogLevel.Critical:
      case signalR.LogLevel.Error:
        logger.error(`[SignalR] ${message}`);
        break;
      case signalR.LogLevel.Warning:
        logger.warn(`[SignalR] ${message}`);
        break;
      case signalR.LogLevel.Information:
        logger.info(`[SignalR] ${message}`);
        break;
      case signalR.LogLevel.Debug:
      case signalR.LogLevel.Trace:
        logger.debug(`[SignalR] ${message}`);
        break;
    }
  }
}

class SignalRBridge {
  constructor(hubUrl, authToken) {
    this.hubUrl = hubUrl;
    this.authToken = authToken;
    this.connection = null;
    this.isConnected = false;
    this.eventHandlers = new Map(); // For socket.io-style .on() handlers
    this.callIdCounter = 0;
  }

  async connect() {
    // Append authentication token to the SignalR connection URL
    const connectionUrl = this.authToken
      ? `${this.hubUrl}?token=${this.authToken}`
      : this.hubUrl;

    // Determine SignalR log level based on environment
    // Warning level suppresses verbose packet-level logging
    const { getLogLevel, LogLevel: AppLogLevel } = require("../logger");
    let signalRLogLevel;

    if (getLogLevel() <= AppLogLevel.DEBUG) {
      // Debug mode: show Info level (connection events without packet details)
      signalRLogLevel = signalR.LogLevel.Information;
    } else {
      // Development/Production: only warnings and errors
      signalRLogLevel = signalR.LogLevel.Warning;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(connectionUrl)
      .withAutomaticReconnect()
      .configureLogging(new SafeLogger(signalRLogLevel))
      .build();

    // Handle reconnection
    this.connection.onreconnecting((error) => {
      logger.error(`[SignalRBridge] Connection lost. Reconnecting...`, error);
      this.isConnected = false;
    });

    this.connection.onreconnected((connectionId) => {
      this.isConnected = true;
    });

    this.connection.onclose((error) => {
      if (error) {
        logger.error(`[SignalRBridge] Connection closed:`, error);
      }
      this.isConnected = false;
    });

    // Set up handlers for messages from .NET
    this.setupMessageHandlers();

    try {
      await this.connection.start();
      this.isConnected = true;

      // Register with the hub
      await this.connection.invoke("RegisterElectronClient");

      return true;
    } catch (err) {
      // Check if this is an authentication error
      if (err.message && err.message.includes("401")) {
        logger.error(
          `[SignalRBridge] Authentication failed: The authentication token is invalid or missing.`,
        );
        logger.error(
          `[SignalRBridge] Please ensure the --authtoken parameter is correctly passed to Electron.`,
        );
      } else {
        logger.error(`[SignalRBridge] Connection failed:`, err);
      }
      this.isConnected = false;
      return false;
    }
  }

  setupMessageHandlers() {
    // Handle generic events from .NET - this is where .NET's Emit() calls arrive
    this.connection.on("event", (eventName, args) => {
      // args is an array from .NET - spread it when calling handlers
      const argsArray = Array.isArray(args) ? args : [args];

      // Check if we have handlers registered for this event
      if (this.eventHandlers.has(eventName)) {
        const handlers = this.eventHandlers.get(eventName);
        handlers.forEach((handler) => {
          try {
            handler(...argsArray);
          } catch (err) {
            logger.error(
              `[SignalRBridge] Error in event handler for ${eventName}:`,
              err,
            );
          }
        });
      }
    });
  }

  // Socket.io compatibility: register event handler
  on(eventName, callback) {
    if (!this.eventHandlers.has(eventName)) {
      this.eventHandlers.set(eventName, []);
    }
    this.eventHandlers.get(eventName).push(callback);
  }

  // Socket.io compatibility: emit event (send to .NET)
  async emit(eventName, ...args) {
    if (!this.isConnected) {
      logger.warn(`[SignalRBridge] Cannot emit ${eventName} - not connected`);
      return;
    }

    try {
      // Always pass args as an array to match C# method signature
      await this.connection.invoke("ElectronEvent", eventName, args);
    } catch (err) {
      logger.error(`[SignalRBridge] Error emitting ${eventName}:`, err);
      throw err;
    }
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.isConnected = false;
    }
  }
}

module.exports = { SignalRBridge };

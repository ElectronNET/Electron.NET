// SignalR connection module for Electron.NET
const signalR = require('@microsoft/signalr');

class SignalRBridge {
    constructor(hubUrl) {
        this.hubUrl = hubUrl;
        this.connection = null;
        this.isConnected = false;
        this.pendingCalls = new Map(); // For tracking API calls
        this.callIdCounter = 0;
    }

    async connect() {
        console.log(`[SignalRBridge] Connecting to ${this.hubUrl}`);

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Handle reconnection
        this.connection.onreconnecting((error) => {
            console.log(`[SignalRBridge] Connection lost. Reconnecting...`, error);
            this.isConnected = false;
        });

        this.connection.onreconnected((connectionId) => {
            console.log(`[SignalRBridge] Reconnected with ID: ${connectionId}`);
            this.isConnected = true;
        });

        this.connection.onclose((error) => {
            console.log(`[SignalRBridge] Connection closed`, error);
            this.isConnected = false;
        });

        // Set up handlers for messages from .NET
        this.setupMessageHandlers();

        try {
            await this.connection.start();
            this.isConnected = true;
            console.log(`[SignalRBridge] Connected successfully`);
            
            // Register with the hub
            await this.connection.invoke('RegisterElectronClient');
            console.log(`[SignalRBridge] Registered as Electron client`);
            
            return true;
        } catch (err) {
            console.error(`[SignalRBridge] Connection failed:`, err);
            this.isConnected = false;
            return false;
        }
    }

    setupMessageHandlers() {
        // Handle API calls from .NET
        this.connection.on('electronApiCall', (method, data) => {
            console.log(`[SignalRBridge] Received API call: ${method}`);
            this.handleApiCall(method, data);
        });
    }

    async handleApiCall(method, data) {
        // This will be implemented to route to the actual Electron API
        // For now, just log it
        console.log(`[SignalRBridge] Handling API call: ${method} with data:`, data);
        
        // TODO: Route to actual Electron API handlers
        // This will be connected to the existing API modules (browserWindows, dialog, etc.)
    }

    async invokeMethod(methodName, ...args) {
        if (!this.isConnected) {
            throw new Error('SignalR connection is not established');
        }

        try {
            const result = await this.connection.invoke(methodName, ...args);
            return result;
        } catch (err) {
            console.error(`[SignalRBridge] Error invoking ${methodName}:`, err);
            throw err;
        }
    }

    async sendElectronEvent(eventName, eventData) {
        if (!this.isConnected) {
            console.warn(`[SignalRBridge] Cannot send event - not connected`);
            return;
        }

        try {
            await this.connection.invoke('ElectronEvent', eventName, JSON.stringify(eventData));
        } catch (err) {
            console.error(`[SignalRBridge] Error sending event:`, err);
        }
    }

    async disconnect() {
        if (this.connection) {
            await this.connection.stop();
            this.isConnected = false;
            console.log(`[SignalRBridge] Disconnected`);
        }
    }

    // Socket.io compatibility method - for easier transition
    on(eventName, callback) {
        if (this.connection) {
            this.connection.on(eventName, callback);
        }
    }

    // Socket.io compatibility method
    emit(eventName, ...args) {
        // Map to SignalR invoke
        return this.invokeMethod(eventName, ...args);
    }
}

module.exports = { SignalRBridge };

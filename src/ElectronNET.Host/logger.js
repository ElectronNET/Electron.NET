/**
 * Environment-aware logging utility for Electron.NET
 * 
 * Provides structured logging with log levels that respect the environment.
 * Preserves console.time/timeEnd for performance measurements.
 */

// Log levels
const LogLevel = {
    DEBUG: 0,
    INFO: 1,
    WARN: 2,
    ERROR: 3,
    SILENT: 4
};

/**
 * Detect the current environment based on various indicators
 * @returns {'development'|'production'|'debug'} The detected environment
 */
function detectEnvironment() {
    // Check for unpacked/development mode flags
    const args = process.argv.join(' ').toLowerCase();
    if (args.includes('--unpackeddotnet') || 
        args.includes('--unpackedelectron') ||
        args.includes('--unpackeddotnetsignalr')) {
        return 'development';
    }
    
    // Check NODE_ENV
    const nodeEnv = process.env.NODE_ENV?.toLowerCase();
    if (nodeEnv === 'development' || nodeEnv === 'dev') {
        return 'development';
    }
    
    // Check for debugger
    if (process.execArgv.some(arg => arg.includes('inspect') || arg.includes('debug'))) {
        return 'debug';
    }
    
    // Default to production for packaged apps
    return 'production';
}

// Determine current environment and default log level
const environment = detectEnvironment();
const defaultLogLevels = {
    'debug': LogLevel.DEBUG,
    'development': LogLevel.INFO,
    'production': LogLevel.WARN
};

let currentLogLevel = defaultLogLevels[environment];

/**
 * Set the current log level
 * @param {number} level - LogLevel enum value
 */
function setLogLevel(level) {
    currentLogLevel = level;
}

/**
 * Get the current log level
 * @returns {number} Current LogLevel
 */
function getLogLevel() {
    return currentLogLevel;
}

/**
 * Get the current environment
 * @returns {string} Current environment name
 */
function getEnvironment() {
    return environment;
}

/**
 * Check if a log level should be output
 * @param {number} level - LogLevel to check
 * @returns {boolean} True if the level should be logged
 */
function shouldLog(level) {
    return level >= currentLogLevel;
}

/**
 * Safe wrapper for console methods that catches EPIPE errors
 */
const safeConsole = {
    log: (...args) => {
        try {
            console.log(...args);
        } catch (e) {
            // Ignore EPIPE errors when console is detached
        }
    },
    warn: (...args) => {
        try {
            console.warn(...args);
        } catch (e) {
            // Ignore EPIPE errors when console is detached
        }
    },
    error: (...args) => {
        try {
            console.error(...args);
        } catch (e) {
            // Ignore EPIPE errors when console is detached
        }
    },
    // Preserve timing functions as-is
    time: (label) => {
        try {
            console.time(label);
        } catch (e) {
            // Ignore EPIPE errors
        }
    },
    timeEnd: (label) => {
        try {
            console.timeEnd(label);
        } catch (e) {
            // Ignore EPIPE errors
        }
    }
};

/**
 * Logger with environment-aware log levels
 */
const logger = {
    /**
     * Log a debug message (only in debug mode)
     */
    debug: (...args) => {
        if (shouldLog(LogLevel.DEBUG)) {
            safeConsole.log('[DEBUG]', ...args);
        }
    },
    
    /**
     * Log an info message
     */
    info: (...args) => {
        if (shouldLog(LogLevel.INFO)) {
            safeConsole.log('[INFO]', ...args);
        }
    },
    
    /**
     * Log a warning message
     */
    warn: (...args) => {
        if (shouldLog(LogLevel.WARN)) {
            safeConsole.warn('[WARN]', ...args);
        }
    },
    
    /**
     * Log an error message
     */
    error: (...args) => {
        if (shouldLog(LogLevel.ERROR)) {
            safeConsole.error('[ERROR]', ...args);
        }
    },
    
    /**
     * Start a timing measurement (always logged)
     */
    time: (label) => {
        safeConsole.time(label);
    },
    
    /**
     * End a timing measurement (always logged)
     */
    timeEnd: (label) => {
        safeConsole.timeEnd(label);
    }
};

module.exports = {
    logger,
    safeConsole,
    LogLevel,
    setLogLevel,
    getLogLevel,
    getEnvironment,
    shouldLog
};

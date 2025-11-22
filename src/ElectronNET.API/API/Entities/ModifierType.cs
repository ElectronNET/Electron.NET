namespace ElectronNET.API.Entities;

/// <summary>
/// Specifies the possible modifier keys for a keyboard input (maps to InputEvent.modifiers).
/// </summary>
/// <remarks>Up-to-date with Electron API 39.2</remarks>
public enum ModifierType
{
    /// <summary>
    /// The Shift key.
    /// </summary>
    shift,

    /// <summary>
    /// The Control key.
    /// </summary>
    control,

    /// <summary>
    /// The Control key (alias for control).
    /// </summary>
    ctrl,

    /// <summary>
    /// The Alt key.
    /// </summary>
    alt,

    /// <summary>
    /// The Meta key.
    /// </summary>
    meta,

    /// <summary>
    /// The Command key.
    /// </summary>
    command,

    /// <summary>
    /// The Command key (alias for command).
    /// </summary>
    cmd,

    /// <summary>
    /// Indicates whether the keypad modifier key is pressed.
    /// </summary>
    isKeypad,

    /// <summary>
    /// Indicates whether the key is an auto-repeated key.
    /// </summary>
    isAutoRepeat,

    /// <summary>
    /// Indicates whether the left mouse button is pressed.
    /// </summary>
    leftButtonDown,

    /// <summary>
    /// Indicates whether the middle mouse button is pressed.
    /// </summary>
    middleButtonDown,

    /// <summary>
    /// Indicates whether the right mouse button is pressed.
    /// </summary>
    rightButtonDown,

    /// <summary>
    /// The Caps Lock key.
    /// </summary>
    capsLock,

    /// <summary>
    /// The Num Lock key.
    /// </summary>
    numlock,

    /// <summary>
    /// The Left key.
    /// </summary>
    left,

    /// <summary>
    /// The Right key.
    /// </summary>
    right
}
using System.Collections.Generic;

namespace ElectronNET.API.Entities
{
    using ElectronNET.Converter;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Input event payload as used by webContents 'input-event' and 'before-input-event'.
    /// Fields map to KeyboardEvent properties where noted, and type/modifiers follow Electron's InputEvent structure.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class InputEvent
    {
        /// <summary>
        /// Equivalent to KeyboardEvent.key.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Equivalent to KeyboardEvent.code.
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// Equivalent to KeyboardEvent.repeat.
        /// </summary>
        public bool IsAutoRepeat { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.isComposing.
        /// </summary>
        public bool IsComposing { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.shiftKey.
        /// </summary>
        public bool Shift { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.controlKey.
        /// </summary>
        public bool Control { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.altKey.
        /// </summary>
        public bool Alt { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.metaKey.
        /// </summary>
        public bool Meta { get; set; } = false;

        /// <summary>
        /// Equivalent to KeyboardEvent.location.
        /// </summary>
        public int Location { get; set; } = 0;

        /// <summary>
        /// An array of modifiers of the event, can be `shift`, `control`, `ctrl`, `alt`,
        /// `meta`, `command`, `cmd`, `iskeypad`, `isautorepeat`, `leftbuttondown`,
        /// `middlebuttondown`, `rightbuttondown`, `capslock`, `numlock`, `left`, `right`.
        /// </summary>
        [JsonConverter(typeof(ModifierTypeListConverter))]
        public List<ModifierType> Modifiers { get; set; }

        /// <summary>
        /// For MouseInputEvent: The x-coordinate of the event (Integer).
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// For MouseInputEvent: The y-coordinate of the event (Integer).
        /// </summary>
        public int? Y { get; set; }

        /// <summary>
        /// For MouseInputEvent: The button pressed, can be 'left', 'middle', or 'right' (optional).
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// For MouseInputEvent: Global x in screen coordinates (Integer, optional).
        /// </summary>
        public int? GlobalX { get; set; }

        /// <summary>
        /// For MouseInputEvent: Global y in screen coordinates (Integer, optional).
        /// </summary>
        public int? GlobalY { get; set; }

        /// <summary>
        /// For MouseInputEvent: Movement delta on x-axis since last event (Integer, optional).
        /// </summary>
        public int? MovementX { get; set; }

        /// <summary>
        /// For MouseInputEvent: Movement delta on y-axis since last event (Integer, optional).
        /// </summary>
        public int? MovementY { get; set; }

        /// <summary>
        /// For MouseInputEvent: Click count (Integer, optional).
        /// </summary>
        public int? ClickCount { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Horizontal scroll delta (Integer, optional).
        /// </summary>
        public int? DeltaX { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Vertical scroll delta (Integer, optional).
        /// </summary>
        public int? DeltaY { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Horizontal wheel ticks (Integer, optional).
        /// </summary>
        public int? WheelTicksX { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Vertical wheel ticks (Integer, optional).
        /// </summary>
        public int? WheelTicksY { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Horizontal acceleration ratio (Integer, optional).
        /// </summary>
        public int? AccelerationRatioX { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: Vertical acceleration ratio (Integer, optional).
        /// </summary>
        public int? AccelerationRatioY { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: True if wheel deltas are precise (optional).
        /// </summary>
        public bool? HasPreciseScrollingDeltas { get; set; }

        /// <summary>
        /// For MouseWheelInputEvent: True if the target can scroll (optional).
        /// </summary>
        public bool? CanScroll { get; set; }

        /// <summary>
        /// Can be `undefined`, `mouseDown`, `mouseUp`, `mouseMove`, `mouseEnter`,
        /// `mouseLeave`, `contextMenu`, `mouseWheel`, `rawKeyDown`, `keyDown`, `keyUp`, `char`,
        /// `gestureScrollBegin`, `gestureScrollEnd`, `gestureScrollUpdate`,
        /// `gestureFlingStart`, `gestureFlingCancel`, `gesturePinchBegin`,
        /// `gesturePinchEnd`, `gesturePinchUpdate`, `gestureTapDown`, `gestureShowPress`,
        /// `gestureTap`, `gestureTapCancel`, `gestureShortPress`, `gestureLongPress`,
        /// `gestureLongTap`, `gestureTwoFingerTap`, `gestureTapUnconfirmed`,
        /// `gestureDoubleTap`, `touchStart`, `touchMove`, `touchEnd`, `touchCancel`,
        /// `touchScrollStarted`, `pointerDown`, `pointerUp`, `pointerMove`,
        /// `pointerRawUpdate`, `pointerCancel` or `pointerCausedUaAction`.
        /// </summary>
        public InputEventType Type { get; set; }
    }
}
using System.Collections.Generic;

namespace ElectronNET.API.Entities
{
    using ElectronNET.Converter;
    using System.Text.Json.Serialization;

    /// <summary>
    /// 
    /// </summary>
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
        /// `meta`, `command`, `cmd`, `isKeypad`, `isAutoRepeat`, `leftButtonDown`,
        /// `middleButtonDown`, `rightButtonDown`, `capsLock`, `numLock`, `left`, `right`
        /// </summary>
        [JsonConverter(typeof(ModifierTypeListConverter))]
        public List<ModifierType> Modifiers { get; set; }

        /// <summary>
        /// Can be `undefined`, `mouseDown`, `mouseUp`, `mouseMove`, `mouseEnter`,
        /// `mouseLeave`, `contextMenu`, `mouseWheel`, `rawKeyDown`, `keyDown`, `keyUp`,
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



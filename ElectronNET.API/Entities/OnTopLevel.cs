using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    public enum OnTopLevel
    {
        normal,
        floating,
        [Description("torn-off-menu")]
        tornOffMenu,
        [Description("modal-panel")]
        modalPanel,
        [Description("main-menu")]
        mainMenu,
        status,
        [Description("pop-up-menu")]
        popUpMenu,
        [Description("screen-saver")]
        screenSaver
    }
}
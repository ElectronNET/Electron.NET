namespace ElectronNET.API.Entities
{
    public class CookieRemovedResponse
    {
        public Cookie cookie {get;set;}

        public CookieChangedCause cause { get; set; }
        public bool removed { get; set; }
    }
}
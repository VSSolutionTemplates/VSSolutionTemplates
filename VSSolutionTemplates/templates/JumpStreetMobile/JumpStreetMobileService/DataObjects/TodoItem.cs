using Microsoft.Azure.Mobile.Server;

namespace JumpStreetMobileService.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Name { get; set; }

        public bool Done { get; set; }
    }
}
using System.Collections.Generic;
namespace Dajeej.Models
{
    public class PublicDevice
    {
       
        public int PublicDeviceId { get; set; }
        public int CountryId { get; set; }
        public string DeviceId { get; set; }
        public bool IsAndroiodDevice { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<PublicNotificationDevice> PublicNotificationDevice { get; set; }
    }
}


namespace Dajeej.Models
{
    public class PublicNotificationDevice
    {

        public int PublicNotificationDeviceId { get; set; }
        public int PublicNotificationId { get; set; }
        public int? PublicDeviceId { get; set; }
        public bool? IsRead { get; set; }


        public virtual PublicNotification PublicNotification { get; set; }
        public virtual PublicDevice PublicDevice { get; set; }



    }
}

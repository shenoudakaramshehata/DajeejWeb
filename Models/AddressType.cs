using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace Dajeej.Models
{
    public class AddressType
    {
        [Key]
        public int AddressTypeId { get; set; }
        public string AddressType1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
    }
}
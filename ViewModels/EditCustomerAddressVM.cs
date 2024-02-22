namespace Dajeej.ViewModels
{
    public class EditCustomerAddressVM
    {
        public int CustomerAddressId { get; set; }
        public int CustomerId { get; set; }
        public string AddressNickname { get; set; }
        public string Area { get; set; }
        public int AddressTypeId { get; set; }
        public string Street { get; set; }
        public string Block { get; set; }
        public string Avenue { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Office { get; set; }
        public string AdditionalDirection { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}

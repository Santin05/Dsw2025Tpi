namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public string? eMail { get; set; }
        public string? name { get; set; }
        public string? phoneNumber { get; set; }

        public Customer(string email, string name, string phoneNumber, Guid id) : base(id)
        {
            this.eMail = email;
            this.name = name;
            this.phoneNumber = phoneNumber;
        }

        public Customer(string email, string name, string phoneNumber) : base()
        {
            this.eMail = email;
            this.name = name;
            this.phoneNumber = phoneNumber;
        }

        public Customer() : base() { }
    }
}

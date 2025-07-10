namespace EasyPay.Domain.Identity
{
    public class AuthItem:EntityBase<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

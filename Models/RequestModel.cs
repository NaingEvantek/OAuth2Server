

namespace OAuthServer.Models
{
    public class ParaCustomerProfile
    {
        public Query query { get; set; }
        public PageForCustProfile page { get; set; }
    }
    public class Query
    {
        public ContactFilter contactFilter { get; set; }
    }

    public class PageForCustProfile
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }

    public class ContactFilter
    {
        public string contactTypeCode { get; set; }
        public string contactPhoneNumber { get; set; }

        public string cardNumber { get; set; }
    }
}

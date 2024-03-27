using System.Collections.ObjectModel;

namespace Rocky_Utility
{
    public static class WC
    {
        public const string ImagePath = @"\images\product\";
        public const string ImagePathPosts = @"\images\post\";

        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";

        public const string UserCookie = "UserCookie";

        public const string AdminRole = "Admin";
        public const string CastomerRole = "Customer";

        public const string CategoryName = "Category";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusApproved,StatusCancelled,StatusInProcess,StatusPending,StatusRefunded,StatusShipped
            });
    }

}

    


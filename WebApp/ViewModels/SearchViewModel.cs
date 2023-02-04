using WebApp.AccountManager;

namespace WebApp.ViewModels
{
    public class SearchViewModel
    {
        public string UserId { get; set; }
        public List<UserWithFriendExt> UserList { get; set; }
    }
}

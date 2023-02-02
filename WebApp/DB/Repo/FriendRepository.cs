using Microsoft.EntityFrameworkCore;
using WebApp.AccountManager;
using WebApp.DB.Model;

namespace WebApp.DB.Repo
{
    public class FriendRepository : Repository<Friend>
    {
        public FriendRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void AddFriend(User user, User friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == user.Id && x.CurrentFriendId == friend.Id);

            if (friends == null)
            {
                var item = new Friend()
                {
                    UserId = user.Id,
                    CurrentFriendId = friend.Id,
                    User = user,
                    CurrentFriend = friend
                };

                Create(item);
            }
        }

        public List<User> GetFriendsByUser(User user)
        {
            return Set.Include(x => x.CurrentFriend).AsEnumerable().Where(x => x.User.Id == user.Id).Select(x => x.CurrentFriend).ToList();
        }

        public void DeleteFriend(User user, User friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == user.Id && x.CurrentFriendId == friend.Id);
        
            if (friends != null)
            {
                Delete(friends);
            }
        }
    }
}

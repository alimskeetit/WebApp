using WebApp.AccountManager;
using WebApp.DB.Model;

namespace WebApp.ViewModels
{
    public class ChatViewModel
    {
        public User You { get; set; }

        public User ToWhom { get; set; }

        public List<Message> History { get; set; }

        public Message NewMessage { get; set; }

        public ChatViewModel()
        {
            NewMessage = new Message();
        }

    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.AccountManager;
using WebApp.DB.Model;
using WebApp.DB.Repo;
using WebApp.DB.UnitOfWork;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AccountManagerController: Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View("../Index");
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                //маппим из LoginViewModel в User
                var user = _mapper.Map<User>(model);

                //входим в аккаунт по Email и паролю
                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);
                
                //Если пользователь найден, то 
                if (result.Succeeded)
                {
                    //переходим на "Моя Страница"
                    return RedirectToAction("MyPage");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View("../Index");
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
           //Удаляем аутентификационные Cookie
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        [Route("MyPage")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var model = new UserViewModel(await _userManager.GetUserAsync(User));
            model.Friends = await GetAllFriends();
            return View("MyPage", model);
        }

        //переход в редактирование
        [Route("Edit")]
        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            var result = _userManager.GetUserAsync(User);

            //маппим из User в UserEditViewModel
            var x = _mapper.Map<UserEditViewModel>(result.Result);
            return View("Edit", x);
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var list = _userManager.Users.AsEnumerable().Where(user => user.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            var withfriend = await GetAllFriends();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == currentUser.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {   
                UserId = currentUser.Id,
                UserList = data
            };

            return model;
        }

        private async Task<List<User>> GetAllFriends()
        {
            var result = await _userManager.GetUserAsync(User);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendRepository;
            return repository.GetFriendsByUser(result);
        }

        private async Task<List<User>> GetAllFriends(User user)
        {
            var repo = _unitOfWork.GetRepository<Friend>() as FriendRepository;

            return repo.GetFriendsByUser(user);
        }

        public async Task<IActionResult> AddFriend(string id)
        {
            var repo = _unitOfWork.GetRepository<Friend>() as FriendRepository;

            //добаляем User друга по id
            repo.AddFriend(await _userManager.GetUserAsync(User), await _userManager.FindByIdAsync(id));

            return RedirectToAction("MyPage", "AccountManager");
        }

        public async Task<IActionResult> DeleteFriend(string id)
        {
            var repo = _unitOfWork.GetRepository<Friend>() as FriendRepository;
            
            //удаляем у User друга по id
            repo.DeleteFriend(await _userManager.GetUserAsync(User), await _userManager.FindByIdAsync(id));

            return RedirectToAction("MyPage", "AccountManager");
        }

        [Route("UserList")]
        [HttpGet]
        public IActionResult UserList(SearchViewModel model)
        {
            return View("UserList", model);
        }

        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserList(string search)
        {
            var model = await CreateSearch(search);
            return UserList(model);
        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            //Получаем текущего пользователя из БД
            var user = await _userManager.GetUserAsync(User);

            //Получаем друга из БД
            var friend = await _userManager.FindByIdAsync(id);

            //Репозиторий для работы с БД
            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var messages = repo.GetMessages(user, friend);

            var model = new ChatViewModel()
            {
                You = user,
                ToWhom = friend,
                History = messages.OrderBy(x => x.Id).ToList()
            };

            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            //Получаем текущего пользователя из БД
            var user = await _userManager.GetUserAsync(User);

            //Получаем друга из БД
            var friend = await _userManager.FindByIdAsync(id);

            //Репозиторий для работы с БД
            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = user,
                Recipient = friend,
                Text = chat.NewMessage.Text
            };

            repo.Create(item);

            var messages = repo.GetMessages(user, friend);

            var model = new ChatViewModel()
            {
                You = user,
                ToWhom = friend,
                History = messages.OrderBy(x => x.Id).ToList()
            };

            return View("Chat", model);
        }
    }
}

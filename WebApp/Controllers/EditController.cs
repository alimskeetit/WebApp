using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.AccountManager;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
	public class EditController: Controller
	{
        private readonly UserManager<User> _userManager;
        private IMapper _mapper;
        public EditController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

		private void UpdateUser(User user, UserEditViewModel usereditvm)
		{
            user.Image = usereditvm.Image;
            user.LastName = usereditvm.LastName;
            user.FirstName = usereditvm.FirstName;
            user.Email = usereditvm.Email;
            user.BirthDate = usereditvm.BirthDate;
            user.UserName = usereditvm.Email;
            user.Status = usereditvm.Status;
            user.About = usereditvm.About;
        }

        [Authorize]
		[Route("Update")]
		[HttpPost]
		public async Task<IActionResult> Update(UserEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.UserId);

                UpdateUser(user, model);

                var result = await _userManager.UpdateAsync(user);
                return RedirectToAction("Edit", "AccountManager");
			}
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
		}
	}
}

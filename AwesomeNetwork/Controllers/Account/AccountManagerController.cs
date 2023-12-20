using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeNetwork.Data;
using AwesomeNetwork.Data.Repository;
using AwesomeNetwork.Data.UoW;
using AwesomeNetwork.Extentions;
using AwesomeNetwork.Models.Users;
using AwesomeNetwork.ViewModels;
using AwesomeNetwork.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AwesomeNetwork.Controllers.Account
{
       public class AccountManagerController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private IWebHostEnvironment _appEnvironment;
        private IUnitOfWork _unitOfWork;
        private ApplicationDbContext context;
        public AccountManagerController(UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment appEnvironment,
            ApplicationDbContext appcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _appEnvironment = appEnvironment;
            context = appcontext;
        }


        public string LastFriends()
        {
            
            return "sad";
        }

        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var usergen = new GenetateUsers();
            var userlist = usergen.Populate(35);

            foreach(var user in userlist)
            {
                var result = await _userManager.CreateAsync(user, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }
        
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var model = new UserViewModel(result);
            model.Friends = (await GetAllFriend(model.User, false));
            model.Posts = _unitOfWork.GetPosts(model.User.Id).TakeLast(3).ToList();
            return View("Mypage", model);
        }
        [Authorize]
        [Route("UserPage")]
        [HttpGet]
        public async Task<IActionResult> UserPage(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id); 
            var model = new UserViewModel(user);
            model.Friends = (await GetAllFriend(model.User, false));
            model.Posts = _unitOfWork.GetPosts(model.User.Id).TakeLast(3).ToList();
            return View("UserPage", model);
        }

        public async Task<IActionResult> UserPosts(string? Id)
        {
            var result = !Id.IsNullOrEmpty()?
                await _userManager.FindByIdAsync(Id):
                await _userManager.GetUserAsync(User);

            var model = new UserViewModel(result);
            model.Posts = _unitOfWork.GetPosts(model.User.Id).ToList();
            return View("UserPosts", model);
        }

        public async Task<IActionResult>UserFriends(string? Id)
        {
            var result = !Id.IsNullOrEmpty() ?
                await _userManager.FindByIdAsync(Id) :
                await _userManager.GetUserAsync(User);

            var model = new UserViewModel(result);

            model.Friends = (await GetAllFriend(model.User));
            model.Posts = _unitOfWork.GetPosts(model.User.Id);
            return View("UserFriends", model);
        }

        private async Task<List<User>> GetAllFriend(User user,bool all=true)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            if(all)
            {
                return repository.GetFriendsByUser(user);
            }
            return repository.GetFriendsByUser(user).Take(3).ToList();
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var user = User;

            var result = _userManager.GetUserAsync(user);

            var editmodel = _mapper.Map<UserEditViewModel>(result.Result);

            return View("Edit", editmodel);
        }


        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model,IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                string path = "/Images/Default/user.png";
                if (Image != null)
                {
                    model.Image = Image.Name;
                    path = "/Images/" + model.UserId + Image.FileName;
                    if (model.Image != null)
                    {
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileStream);
                        }
                        model.Image = path;
                    }
                }
                var user = await _userManager.FindByIdAsync(model.UserId);
                user.Convert(model);
                user.Image = path;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "некоректні дані");
                return View("Edit", model);
            }
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var user = _mapper.Map<User>(model);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                   return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    ModelState.AddModelError("", "Невірний логін та (або) пароль");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(string search,string? cat="")
        {
            var model = await CreateSearch(search,cat);
            return View("UserList", model);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.AddFriend(result, friend);


            return RedirectToAction("MyPage", "AccountManager");
        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }


        private async Task<SearchViewModel> CreateSearch(string search,string? cat="")
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var list = _userManager.Users.Where(u=>u.UserRole.Contains(cat)).AsEnumerable().ToList();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                //list = _userManager.Users.AsEnumerable().ToList();
            }

            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }


        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);


            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            repository.Create(item);

            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(string PostTitle,string PostDescription,string PostType)
        {
            var currentuser = User;
            var result = await _userManager.GetUserAsync(currentuser);
            result.Posts.Add(
            new Post()
            {
                Title = PostTitle,
                Description = PostDescription,
                PostType = PostType,
                User = result,
            });
            context.SaveChanges();
            return RedirectToAction("MyPage", "AccountManager");
        }

        public async Task<IActionResult> Messages(string userId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string Text,int PostId)
        {
            var currentuser = User;
            var result = await _userManager.GetUserAsync(currentuser);
            var post = context.Posts.Where(p => p.Id == PostId).FirstOrDefault();
            result.Comments.Add(
            new Comment()
            {
                Text = Text,
                Post = post,
                User = result,
            });
            context.SaveChanges();
            return RedirectToAction("Details", "Posts",new {id = post.Id });
        }
    }
}

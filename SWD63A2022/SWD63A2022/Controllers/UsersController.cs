using Common;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63A2022.Controllers
{
    public class UsersController : Controller
    {
        private readonly FireStoreDataAccess fireStore;
        public UsersController(FireStoreDataAccess _fireStore)
        {
            fireStore = _fireStore;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
           Common.User myUser = await fireStore.GetUser(User.Claims.ElementAt(4).Value);
            if (myUser == null)
                return View(new Common.User() { Email = User.Claims.ElementAt(4).Value });
            else return View(myUser);
        }

        [Authorize]
        public async Task<IActionResult> Register(User user)
        {
           user.Email = User.Claims.ElementAt(4).Value;
           await fireStore.AddUser(user);
            
           return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Send()
        { return View();
        }

        [Authorize][HttpPost]
        public async Task<IActionResult> Send(Message msg)
        {
            msg.Id = Guid.NewGuid().ToString();
           // msg.DateSent = Google.Cloud.Firestore.Timestamp.FromDateTime(DateTime.UtcNow);
           await fireStore.AddMessage(User.Claims.ElementAt(4).Value, msg);
            return RedirectToAction("List");
        }
        [Authorize]
        public async Task<IActionResult> List()
        {
            var messages = await fireStore.ListMessages(User.Claims.ElementAt(4).Value);
            return View(messages);
        }
    }
}

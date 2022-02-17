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
    }
}

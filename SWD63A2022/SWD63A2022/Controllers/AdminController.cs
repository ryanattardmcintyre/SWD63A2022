using Common;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63A2022.Controllers
{
    public class AdminController : Controller
    {
        private CacheDataAccess _cache;
        public AdminController(CacheDataAccess cache)
        {
            _cache = cache;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem m)
        {
            _cache.AddMenuItem(m);
            return View();
        }
    }
}

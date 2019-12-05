using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginAndReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace LoginAndReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext DbContext;
        public HomeController(MyContext context){
            DbContext=context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("")]
        public IActionResult Registration(User newUser){
            if(ModelState.IsValid){
                if(DbContext.Users.Any(user => user.Email==newUser.Email)){
                    ModelState.AddModelError("Email","Not today");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                DbContext.Add(newUser);
                DbContext.SaveChanges();
                HttpContext.Session.SetInt32("userId", newUser.UserId);
                return RedirectToAction("success");
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("login");
        }

        [HttpPost("user/login")]
        public IActionResult LoginAction(Login userlog){
            if(ModelState.IsValid){
                var userInDb = DbContext.Users.FirstOrDefault(user => user.Email==userlog.LoginEmail);
                if(userInDb==null){
                    ModelState.AddModelError("LoginEmail","Not today");
                    return View("Index");
                }
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(userlog, userInDb.Password, userlog.LoginPassword);
                if(result==0){
                    ModelState.AddModelError("LoginEmail","Not today");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return RedirectToAction("success");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success(){
            if(HttpContext.Session.GetInt32("userId")==null){
                return View("login");
            }
            return View("success");
        }

        [HttpGet("logout")]
        public IActionResult logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }





        // public IActionResult Privacy()
        // {
        //     return View();
        // }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}

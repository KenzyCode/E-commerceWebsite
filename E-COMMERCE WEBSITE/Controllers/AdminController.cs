using E_COMMERCE_WEBSITE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_COMMERCE_WEBSITE.Controllers
{
    
   
    public class AdminController : Controller
    {
        // GET: Admin
        static string username = "user101";
        static string password = "userpassword";
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(AdminModel login)
        {           
            if (username != login.username)
            {
                ViewBag.Error = "Username not found";
                return View();
            }
            else if (password != login.Password && login.Counter < 5)
            {
                ViewBag.Error = "Wrong Password";
                login.Counter++;
                return View(login);
            }
            else if(password != login.Password && login.Counter >= 5)
            {
                return View("Error");
            }
            else
            {
                return View("Dashbord");
            }  
            
           
        }
        [HttpGet]
        public ActionResult Dashbord()
        {
            return View();
        }
        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}
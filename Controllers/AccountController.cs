using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WorthIt.Models;

namespace WorthIt.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                using (var entities = new IdentityModel())
                {
                    var userStore = new UserStore<User>(entities);
                    var manager = new UserManager<User>(userStore);

                    var user = new User()
                    {
                        UserName = model.EmailAddress,
                        Email = model.EmailAddress,
                        EmailConfirmed = true
                    };

                    IdentityResult result = manager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        FormsAuthentication.SetAuthCookie(model.EmailAddress, true);
                        return RedirectToAction("Index", "Home");

                        //add user 
                        User brainTreeNewUser = manager.FindByName(model.EmailAddress);

                        //create customer
                        string merchantId = ConfigurationManager.AppSettings["Braintree.MerchantID"];
                        string publicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"];
                        string privateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                        string environment = ConfigurationManager.AppSettings["Braintree.Environment"];

                        Braintree.BraintreeGateway worthItGateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);
                        
                        var braintreeCustomer = new Braintree.CustomerRequest()
                        {
                            CustomerId = brainTreeNewUser.Id,
                            Email = brainTreeNewUser.Email
                        };

                        var customerRegister = await worthItGateway.Customer.CreateAsync(braintreeCustomer);

                    }
                    else
                    {
                        ModelState.AddModelError("EmailAddress", "Unable to register with this Email address");
                    }
                }
            }
            
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var entities = new IdentityModel())
                {
                    try
                    {
                        var userStore = new UserStore<User>(entities);
                        var manager = new UserManager<User>(userStore);

                        var user = manager.FindByEmail(model.EmailAddress);
                        if (manager.CheckPassword(user, model.Password))
                        {
                            FormsAuthentication.SetAuthCookie(model.EmailAddress, true);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("EmailAddress", "Could not sign in with this name and/or password");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;
using SmartyStreets.USStreetApi;

namespace WorthIt.Controllers
{
    public class CheckoutController : Controller
    {
        WorthITDatabaseEntities entities = new WorthITDatabaseEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        //getValidateAddress
        [HttpPost]
        public ActionResult ValidateAddress(string street1, string street2, string city, string state, int? zipcode, string country)
        {
            string authId = ConfigurationManager.AppSettings["SmartyStreets.AuthID"];
            string authToken = ConfigurationManager.AppSettings["SmartyStreets.AuthToken"];
            ClientBuilder builder = new ClientBuilder(authId, authToken);
            Client client = builder.Build();
            Lookup lookup = new Lookup()
            {
                City = city,
                State = state,
                Street = street1,
                Street2 = street2,
                ZipCode = zipcode.ToString()
            };

            var addShippingAddress = new Address()
            {
                UserId = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name).Id,
                ShippingAddress1 = street1,
                ShippingAddress2 = street2,
                City = city,
                State = state,
                PostalCode = zipcode.ToString(),
                Country = country,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            entities.Addresses.Add(addShippingAddress);
            entities.SaveChanges();

            client.Send(lookup);
            return Json(lookup.Result);
        }

        [HttpPost]
        public ActionResult deleteAddress(int id)
        {
            var address = entities.Addresses.Single(x => x.Id == id);
            entities.Addresses.Remove(address);
            entities.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        //main
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index (Checkout model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                var lastOrder = entities.Orders.OrderByDescending(x => x.Created).FirstOrDefault(x => x.AspNetUser.UserName == name);

                if (User.Identity.IsAuthenticated)
                {
                    AspNetUser currentUser = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name);
                    var cartItems = currentUser.Carts.Where(x => x.UserId == currentUser.Id);

                    foreach (var cartItem in cartItems)
                    {
                        entities.Orders.Add(new Order { UserId = cartItem.UserId, OrderNumber = Guid.NewGuid(), Created = DateTime.UtcNow, Modified = DateTime.UtcNow, PurchaserEmail = User.Identity.Name, ShippingAddress = Convert.ToString(model.ShippingAddress1 + model.ShippingAddress2) });
                        foreach (var cartProductItem in cartItem.CartProducts)
                        {
                            entities.OrderProducts.Add(new OrderProduct { ProductId = cartProductItem.ProductId, Quantity = cartProductItem.Quantity ?? 0, Created = DateTime.UtcNow, Modified = DateTime.UtcNow });
                        }
                    }

                    var removeCartproductItems = entities.CartProducts.Where(x => x.Cart.AspNetUser.UserName == User.Identity.Name);

                    foreach (var item in removeCartproductItems)
                    {
                        entities.CartProducts.Remove(item);
                    }

                    lastOrder.Completed = DateTime.UtcNow;
                    entities.SaveChanges();
                }
                else
                {
                    return RedirectToAction("index", "home");
                }

                if (lastOrder == null || lastOrder.OrderProducts.Sum(x => x.Quantity) == 0)
                {
                    return RedirectToAction("index", "cart");
                }
                
                var worthItShippingAddress = new Address()
                {
                    ShippingAddress1 = model.ShippingAddress1,
                    ShippingAddress2 = model.ShippingAddress2,
                    City = model.ShippingCity,
                    State = model.ShippingState,
                    PostalCode = model.ZipCode.ToString(),
                    Country = model.ShippingCountry
                };

                string merchantId = ConfigurationManager.AppSettings["Braintree.MerchantID"];
                string publicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"];
                string privateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                string environment = ConfigurationManager.AppSettings["Braintree.Environment"];

                Braintree.BraintreeGateway worthItGateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

                var cardRequest = new Braintree.TransactionCreditCardRequest
                {
                    CardholderName = model.CreditCardName,
                    CVV = model.CreditCardVerificationValue.ToString(),
                    ExpirationMonth = model.CreditCardExpirationMonth.ToString(),
                    ExpirationYear = model.CreditCardExpirationYear.ToString(),
                    Number = model.CreditCardNumber
                };

                var transactionInfo = new Braintree.TransactionRequest
                {
                    Amount = entities.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0.1m,
                    CreditCard = cardRequest
                };


                //if user is logged in, associate this transaction with their account
                if (User.Identity.IsAuthenticated)
                {
                    var searchUser = new Braintree.CustomerSearchRequest();
                     searchUser.Email.Is(User.Identity.Name);
                    var braintreeCustomers = worthItGateway.Customer.Search(searchUser);
                    if (braintreeCustomers != null &&  braintreeCustomers.Ids.Any())
                        transactionInfo.CustomerId = braintreeCustomers.FirstItem.Id;
                }

                Braintree.Result<Braintree.Transaction> result = await worthItGateway.Transaction.SaleAsync(transactionInfo);
                if (!result.IsSuccess())
                {
                    ModelState.AddModelError("cardRequest", "Could not authorize payment");
                    return View(model);
                }

                string sendGridApiKey = ConfigurationManager.AppSettings["SendGrid.Apikey"];

                var worthItClient = new SendGrid.SendGridClient(sendGridApiKey);
                var mailMesseage = new SendGrid.Helpers.Mail.SendGridMessage
                {
                    Subject = string.Format("Receipt for order {0}", lastOrder.Id),
                    From = new SendGrid.Helpers.Mail.EmailAddress("admin@worthItApp.com", "Worth It Administrator"),
                };

                mailMesseage.SetTemplateId("82560f81-ae71-4bfe-badb-82c6f3b6c9be");
                mailMesseage.AddTo(new SendGrid.Helpers.Mail.EmailAddress(model.Email));

                var mailContent = new SendGrid.Helpers.Mail.Content("text/plain", "Thank you for placing order");

                mailMesseage.AddContent(mailContent.Type, mailContent.Value);
                mailMesseage.AddSubstitution("%orderNum%", lastOrder.Id.ToString());

                SendGrid.Response worthItResponse = await worthItClient.SendEmailAsync(mailMesseage);

                return RedirectToAction("Index", "Receipt");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult States(int? id)
        {
            var placeState = entities.States.Where(x => x.CountryId == id);
            if (id.HasValue)
            {
                return Json(placeState.Select(x => new { ID = x.Id, Text = x.StateName, Value = x.StateCode }).ToArray());
            }
            return Json(new State[0]);
        }

        [HttpPost]
        public ActionResult Country()
        {
            var c = entities.Countries.Count();
            return Json(entities.Countries.Select(x => new { Text = x.CountryName, Value = x.Id }).ToArray());
        }
    }
}
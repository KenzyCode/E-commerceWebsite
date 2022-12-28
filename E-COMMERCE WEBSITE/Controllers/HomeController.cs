using System; 
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_COMMERCE_WEBSITE.Models;


namespace E_COMMERCE_WEBSITE.Controllers
{
    public class HomeController  : Controller
    {
        //Cteating an object of our database of class ecommerceDatabaseEntities
        //we assigned the new - db to it
       static ecommerceWebsiteDatabaseEntities db = new ecommerceWebsiteDatabaseEntities();
        public static List<ProductTable> GetCat()
        {
            List<ProductTable> genProduct = db.ProductTables.ToList();
            List<ProductTable> category = new List<ProductTable>();
            //get the fist product
            while (genProduct.Count > 0)
            {
                ProductTable product = genProduct.FirstOrDefault();
                category.Add(product);
                var removeProd = genProduct.Where(x => x.Categories.ToLower() == product.Categories.ToLower()).ToList();
                if (removeProd.Count != 0)
                {
                    foreach(var subProd in removeProd)
                    {
                        genProduct.Remove(subProd);
                    }
                }
            }
            return category;
        }
        public static List<ProductTable> GetBrand()
        {
            List<ProductTable> genProduct = db.ProductTables.ToList();
            List<ProductTable> brand = new List<ProductTable>();           
            while (genProduct.Count != 0)
            {
                ProductTable product = genProduct.FirstOrDefault();
                brand.Add(product);
                var removeProd = genProduct.Where(x => x.Brand == product.Brand).ToList();
                if (removeProd.Count != 0)
                {
                    foreach (var subProd in removeProd)
                    {
                        genProduct.Remove(subProd);
                    }
                }
            }
            return brand;
        }
        // GET: Home
        [HttpGet]
        public ActionResult Index(UserTable user)
        {
            if (GetCat().Count != 0)
            {
                ViewBag.Category = GetCat();
                foreach(var cat in GetCat())
                {
                    cat.UserId = user.UserId;
                }
            }
            if (GetBrand().Count != 0)
            {
                ViewBag.Brand = GetBrand();
                foreach(var cat in GetBrand())
                {
                    cat.UserId = user.UserId;
                }
            }
            if (user.UserId != 0)
            {
                ViewBag.User = user;
            }
            //at liberty to pull out list of product to be 
            //displayed in the view
            //create a list of our product table
            var products = db.ProductTables.ToList();
            //to add user details to each product
            if (user != null)
            {
                foreach (var product in products)
                {
                    product.UserId = user.UserId;
                }  
            }         

           
            ViewBag.Products = products;
            return View();
        }
        [HttpGet]
        public ActionResult Category(ProductTable categories)
         {
             //pull a list of product of same category
             var products = db.ProductTables.Where(x => x.Categories.ToLower() == categories.Categories.ToLower()).ToList();
                 // to ensure we don't loss our user details
                 if (categories.UserId != 0)
             {
                 var user = db.UserTables.Find(categories.UserId);
                 ViewBag.User = user;
                 foreach (var product in products)
                 {
                     product.UserId = user.UserId;
                 }
             }
             ViewBag.Products = products;
             return View();
         }
        [HttpGet]
        public ActionResult ProductDetail(ProductTable product)
        {
            //the productTable product is used to collect the product detail
            //coming from the homepage or any other page.
            //This product is parsed into the productDetail view
            //which is called up by the model class
            return View(product);
        }
        [HttpGet]
        public ActionResult AddProductLogin(ProductTable product)
        {
            //This will help us keep track of the product to add to cart 
            UserTable user = new UserTable();
            user.ProductId = product.ProductId;
            return RedirectToAction("Login", user);
        }
        [HttpGet]
        public ActionResult AddProductToCart(ProductTable product)
        {
            var user = db.UserTables.Find(product.UserId);
            ViewBag.User = user;
            //this action is to add product to the cart 
            RequestPurchaseTable addProduct = new RequestPurchaseTable
            {
                ProductId = product.ProductId,
                ProductQty = 1,
                ProductAmt = product.ProductAmt,
                UserId = user.UserId,
                DeliveryStatus = "cart"

            };
            //recall to check if product has already been requested in db

            //add product details to request table 
            db.RequestPurchaseTables.Add(addProduct);
            //save to db
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index", user  );
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Login(UserTable user)
        {
            return View(user);
        }
        [HttpPost] 
        public ActionResult Login(UserTable user, string a) 
        {
            //query our database for login details
            var checkUser = db.UserTables.Where(x => x.Email.ToLower() == user.Email).FirstOrDefault();
            if (checkUser is null)
            {
                ViewBag.Error = "Email not found";
                return View(user);
            }
            else if (checkUser.Password != user.Password)
            {
                ViewBag.Error = "Incorrect Password";
                return View(user);
            }
            else if (user.ProductId != null)
            {
                //this action is to add product to the cart 
                var product = db.ProductTables.Find(user.ProductId);
                RequestPurchaseTable addProduct = new RequestPurchaseTable
                {
                    ProductId = product.ProductId,
                    CartProductQty = 1,
                    ProductAmt = product.ProductAmt,
                    UserId = checkUser.UserId, 
                    DeliveryStatus = "cart"

                };
                //recall to check if product has already been requested in db

                //add product details to request table 
                db.RequestPurchaseTables.Add(addProduct);
                //save to db 
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index", checkUser);
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", checkUser);
            }

        }
        [HttpGet]
        public ActionResult Cart(UserTable user)
        {
            //identify the user and pull out items in his cart
            if (user.UserId is 0)
            {
                return View("Login", user);
            }
            else
            {
                ViewBag.User = user;
                //check the requsect table for the user chart items
                var usersCart = db.RequestPurchaseTables.Where(a => a.UserId == user.UserId && a.DeliveryStatus == "cart").ToList();

                    if (usersCart.Count != 0)
                {
                    for (int i = 0; i < usersCart.Count; i++)
                    {
                        var productFullDetail = db.ProductTables.Find(usersCart[i].ProductId);
                         
                    }
                }
                foreach (var item in usersCart)
                {
                    var productFullDetail = db.ProductTables.Find(item.ProductId);
                    item.ProductAmt = productFullDetail.ProductAmt;
                    item.Brand = productFullDetail.Brand;
                    item.ImagePaths = productFullDetail.ImagePaths;
                    item.ProductName = productFullDetail.ProductName;
                    item.ProductQty = productFullDetail.ProductQty;

                }
                ViewBag.Cart = usersCart;

            }
            return View();
        }
      /*  public ActionResult AddQty(RequestPurchaseTable product)
        {
            if (product.CartProductQty is null)
            {
                product.CartProductQty = 1;
            }
            //keeping track of the user
            var user = db.UserTables.Find(product.UserId);
            //increase the quantity 
            if (product.CartProductQty <product.ProductQty)
            {
                product.CartProductQty++;
            }
            //update data in database
            //dbObject.Entry(objectItem2Update).state = Entitystate.Modified;
            db.Entry(product).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //if productQty succesfully adds, we redirect to the cart action
                return RedirectToAction("Cart", user);
            }
            catch(Exception)
            {
                return RedirectToAction("Cart", user);
            }
        }

        public ActionResult SubQty(RequestPurchaseTable product)
        {
            if (product.CartProductQty is null)
            {
                product.CartProductQty = 1;
            }
            //keeping track of the user
            var user = db.UserTables.Find(product.UserId);
            //increase the quantity 
            if (product.CartProductQty >= 2)
            {
                product.CartProductQty--;
            }
            //update data in database
            //dbObject.Entry(objectItem2Update).state = Entitystate.Modified;
            db.Entry(product).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //if productQty succesfully adds, we redirect to the cart action
                return RedirectToAction("Cart", user);
            }
            catch
            {
                return RedirectToAction("Cart", user);
            }
        }*/
        [HttpGet]
        public ActionResult RemoveItem(RequestPurchaseTable product)
        {
            var user = db.UserTables.Find(product.UserId);
            //we are to remove the selected product from the db
            //the Remove() linq is used to remove a item form an object list
            var findProduct = db.RequestPurchaseTables.Find(product.RequestId);
            db.RequestPurchaseTables.Remove(findProduct);
            try
            {
                db.SaveChanges();
                return RedirectToAction("cart", user);
            }
            catch
            {
                return RedirectToAction("cart", user);
            }
        }
        //the action is used to search for product
        [HttpPost]
        public ActionResult Search(IndexModel searchItem)
        {
            var user = db.UserTables.Find(searchItem.UserId);
            ViewBag.User = user;
            //to enture all ViewBags in the index page are intact
            var products = db.ProductTables.ToList();
            //to add user details to each product
            if (user != null)
            {
                foreach (var product in products)
                {
                    product.UserId = user.UserId;   

                }
            }
            ViewBag.Products = products;
            //---------------

            var results = db.ProductTables.Where(x => x.Categories.Contains(searchItem.search) || x.Brand.Contains(searchItem.search) || x.ProductName.Contains(searchItem.search) || x.Model.Contains(searchItem.search)).ToList();
            
                if (results.Count != 0)
            {
                ViewBag.Result = results;
                //the userId of the user is added to each result 
                //to ensure it is not lost
                if (user != null)
                {
                    foreach(var result in results)
                    {
                        result.UserId = user.UserId;
                    }
                }
            }
            else
            {
                ViewBag.NoResult = "No Result Found";
            }
            return View("Index");

        }
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
       [HttpPost]
        public ActionResult Signup(UserTable signUp)
        {
            //confirm email is not in the db
            var userCheck = db.UserTables.Where(x => x.Email.ToLower() == signUp.Email.ToLower()).FirstOrDefault();

            if (userCheck != null)
            {
                ViewBag.Error = "Email already exists";
                return View(signUp);
            }
            else if (signUp.Password != signUp.ConfirmPassword)
            {
                ViewBag.Error = "Password doen't match";
                return View(signUp);
            }
            else
            {
                //add user to your db
                db.UserTables.Add(signUp);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("index", signUp);
                }
                catch
                {
                    ViewBag.Error = "Please reenter details";
                    return View(signUp);
                }
            }
           
        }


    }
}
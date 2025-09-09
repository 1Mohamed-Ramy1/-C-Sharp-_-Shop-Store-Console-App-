using App.Models;
using Newtonsoft.Json;
using App.Services;
using App.Pages;
using App.Routes;

var router = new Router();

router.Register("home", () => new HomePage());
router.Register("login", () => new LoginPage());
router.Register("admin",()=>new AdminPage());
router.Register("shop",()=>new ShopPage());
router.Register("signup",()=>new SignupPage());

router.Start("home");
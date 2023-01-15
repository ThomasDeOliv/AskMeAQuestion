using AskMeAQuestion.Models;
using AskMeAQuestion.Services.Interface;
using AskMeAQuestion.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AskMeAQuestion.Controllers
{
    /// <summary>
    /// Controleur de la partie visiteur de l'application
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        /// <summary>
        /// Service ILogger
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Service Login
        /// </summary>
        private readonly ILoginService _loginService;

        /// <summary>
        /// Service inscription
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Service HttpContextAccessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Ìnstance du service ILogger</param>
        /// <param name="contextAccessor">Ìnstance du service IHttpContextAccessor</param>
        /// <param name="loginService">Ìnstance du service ILoginService</param>
        /// <param name="subscriptionService">Ìnstance du service ISubscriptionService</param>
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor, ILoginService loginService, ISubscriptionService subscriptionService)
        {
            _logger = logger;
            _loginService = loginService;
            _subscriptionService = subscriptionService;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Donne la valeur du Claim de l'identifiant de l'utilisateur si ce dernier existe. Sinon renvoie une valeur nulle.
        /// </summary>
        private string _cookieClaim => _contextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type == "sessionId") != null ?
                    _contextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type == "sessionId").Value : null;

        /// <summary>
        /// Action affichant un message de bienvenue et deux liens pour se loguer et s'inscrire
        /// </summary>
        /// <returns>Retourne la vue, ou redirige vers l'espace personnel si des cookies du site valides sont présents</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (_cookieClaim != null && await _loginService.CheckCookieAsync(_cookieClaim))
            {
                return RedirectToAction(nameof(LoggedController.PersonnalSpace), "Logged");
            }
            return View();
        }

        /// <summary>
        /// Action de déconnexion
        /// </summary>
        /// <returns>Redirige vers l'Index</returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _loginService.DisconnectAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary> 
        /// Action retournant une vue de login à l'utilisateur
        /// </summary>
        /// <param name="returnUrl">URL de retour</param>
        /// <param name="messageToUser">Message adressé à l'utilisateur en cas d'erreur de login</param>
        /// <returns>Redirection vers l'espace personnel si cookie valide présent, affichage de la vue sinon</returns>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl, string messageToUser)
        {
            if (_cookieClaim != null && await _loginService.CheckCookieAsync(_cookieClaim))
            {
                return RedirectToAction(nameof(LoggedController.PersonnalSpace), "Logged");
            }
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            if (messageToUser != null)
            {
                model.MessageToUser = messageToUser;
            }
            return View(model);
        }

        /// <summary>
        /// Action retournant une vue d'inscription à l'utilisateur
        /// </summary>
        /// <param name="messageToUser">Message adressé à l'utilisateur en cas d'erreur de saisie</param>
        /// <returns>Retourne la page d'inscription, redirige vers l'espace personnel si cookie du site présent</returns>
        [HttpGet]
        public async Task<IActionResult> Subscription(string messageToUser)
        {
            if (_cookieClaim != null && await _loginService.CheckCookieAsync(_cookieClaim))
            {
                return RedirectToAction(nameof(LoggedController.PersonnalSpace), "Logged");
            }
            SubscriptionViewModel model = new SubscriptionViewModel();
            if (messageToUser is not null)
            {
                model.MessageToUser = messageToUser;
            }
            return View(model);
        }

        /// <summary>
        /// Action retournant une VM au server contenant des infos à vérifier pour se connecter
        /// </summary>
        /// <param name="model">VM contenant les informations de login</param>
        /// <returns>Redirige vers l'espace personnel si cookie présent ou informations soumises validées, redirige vers cette page avec message d'erreur sinon</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isConnected = await _loginService.VerificationLoginAsync(model.UserLogin, model.UserPassword, model.UserOpenConnection);
                if (isConnected)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return Redirect("/Home/Login?messageToUser=ErrorConnection");
                }
            }
            return BadRequest();
        }

        /// <summary>
        /// Action retournant une VM contenant des informations d'inscriptions pour un nouvel utilisteur
        /// </summary>
        /// <param name="model">VM enrichie des infos données par l'utilisateur</param>
        /// <returns>Retour sur la page d'accueil, ou celle d'inscription avec une erreur</returns>
        [HttpPost]
        public async Task<IActionResult> Subscription(SubscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool subscriptionState = await _subscriptionService.SubscriptionAsync(model);
                if (subscriptionState)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Redirect("/Home/Subscription?messageToUser=ErrorSubscription");
                }
            }
            return BadRequest();
        }

        /// <summary>
        /// Action de renvoie de la page d'erreur
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

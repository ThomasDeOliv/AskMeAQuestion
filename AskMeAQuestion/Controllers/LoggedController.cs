using AskMeAQuestion.Services.Interface;
using AskMeAQuestion.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AskMeAQuestion.Controllers
{
    /// <summary>
    /// Controleur de la partie utilisateur/administrateur de sondage de l'application
    /// </summary>
    [Authorize]
    public class LoggedController : Controller
    {
        #region Propriétés

        /// <summary>
        /// Identifiant de session de l'utilisateur, accessible dans le cookie
        /// </summary>
        private string _userSession
        {
            get
            {
                if (_contextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type == "sessionId") != null)
                {
                    return _contextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type == "sessionId").Value;
                }
                return null;
            }
        }

        #endregion

        #region Services

        /// <summary>
        /// Service ILogger
        /// </summary>
        private readonly ILogger<LoggedController> _logger;

        /// <summary>
        /// Service de sondages
        /// </summary>
        private readonly IPollService _pollService;

        /// <summary>
        /// Service de vote
        /// </summary>
        private readonly IVoteService _voteService;

        /// <summary>
        /// HttpContextAccessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur de classe, chargeant différents service lors de son démarrage
        /// </summary>
        /// <param name="logger">Instance du service ILogger</param>
        /// <param name="contextAccessor">Instance du service IHttpContextAccessor</param>
        /// <param name="pollService">Instance du service IPollService</param>
        /// <param name="voteService">Instance du service IVoteService</param>
        public LoggedController(ILogger<LoggedController> logger, IHttpContextAccessor contextAccessor, IPollService pollService, IVoteService voteService)
        {
            _logger = logger;
            _pollService = pollService;
            _contextAccessor = contextAccessor;
            _voteService = voteService;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Action permettant d'obtenir la page d'espace personnel
        /// </summary>
        /// <param name="message">Potentiel message déclenchant une notification</param>
        /// <returns>La page de l'espace personnel d'un utilisateur, ou celle de connexion si identifiant de session dans le cookie non valide</returns>
        [HttpGet]
        public async Task<IActionResult> PersonnalSpace(string message)
        {
            // On charge le modèle en fonction de cet ID
            PersonnalSpaceViewModel personnalSpaceViewModel = await _pollService.LoadPersonnalSpaceAsync(_userSession);

            // Si l'utilisateur a été trouvé et que la VM a été hydratée
            if (personnalSpaceViewModel != null)
            {
                // Si un script s'est déclenché
                if (message != null)
                {
                    personnalSpaceViewModel.MessageToUser = message;
                }

                // Retourner la vue à l'utilisateur
                return View(personnalSpaceViewModel);
            }
            // Si les cookies sont périmés ou non valide, on retourne à la page de Login
            return RedirectToAction(nameof(HomeController.Login), "Home");
        }

        /// <summary>
        /// Action de consultation des résultats d'un sondage
        /// </summary>
        /// <param name="resultKey">Clé de consultation des résultats d'un sondage</param>
        /// <returns>Page de consultation des résultats ou retour à l'espace personnel avec un message d'erreur</returns>
        [HttpGet]
        public async Task<IActionResult> PollResults(string resultKey)
        {
            // On vérifie que la clé des résultats soit attribuée à un sondage, et si tel est le cas on retourne une vue avec un sondage
            PollResultsViewModel pollResultsViewModel = await _voteService.CheckResultKeyAsync(resultKey, _userSession);

            if (pollResultsViewModel != null)
            {
                return View(pollResultsViewModel);
            }
            return BadRequest();
        }

        /// <summary>
        /// Áction de consultation de la page des liens du sondages
        /// </summary>
        /// <param name="pollId">Identifiant du sondage</param>
        /// <returns>Vue permettant de consulter les liens de désactivation, résultats et participation + code de participation à un sondage si administrateur, page d'erreur sinon</returns>
        [HttpGet]
        public async Task<IActionResult> PollGetLinks(int pollId)
        {
            // On instancie notre VM en lui passant l'ID du sondage, l'id de session de l'utilisateur et le nom du domaine
            PollGetLinksViewModel pollGetLinksViewModel = await _pollService.GetAdminLinksOfPollAsync(pollId, _userSession, _contextAccessor.HttpContext.Request.Host.Value);

            // Sinon on retourne notre vue hydratée à l'utilisateur
            if (pollGetLinksViewModel != null)
            {
                return View(pollGetLinksViewModel);
            }
            // Si cette dernière est nulle, on retourne une BadRequest
            return BadRequest();
        }

        /// <summary>
        /// Action permettant de charger la page de création de sondage
        /// </summary>
        /// <returns>Page de création de sondage vide</returns>
        [HttpGet]
        public async Task<IActionResult> PollCreation()
        {
            PollCreationViewModel pollCreationViewModel = await _pollService.CreateNewPollAsync(_userSession);

            if (pollCreationViewModel != null)
            {
                return View(pollCreationViewModel);
            }
            return BadRequest();
        }

        /// <summary>
        /// Action d'enregistrement d'un nouveau sondage, de ses réponses associées et inscription de l'administrateur du sondage comme participant
        /// </summary>
        /// <param name="pollCreationViewModel">VM hydratée par l'utilisateur</param>
        /// <returns>Redirection vers l'espace personnel ou vers page d'erreur si données transmises au server incohérantes</returns>
        [HttpPost]
        public async Task<IActionResult> PollCreation(PollCreationViewModel pollCreationViewModel)
        {
            // On s'assure que toutes les conditions soient remplies pour valider la création du sondage
            if (pollCreationViewModel.PollAnswers.Count() < 2 || string.IsNullOrWhiteSpace(pollCreationViewModel.PollDescription) || string.IsNullOrWhiteSpace(pollCreationViewModel.PollTitle))
            {
                // Si les conditions de création ne sont pas valables
                return BadRequest();

            }
            // Enregistrement et récupération de l'ID du sondage
            var registred = await _pollService.RegisterNewPollAsync(pollCreationViewModel, _userSession);

            if (registred != 0)
            {
                // Retour à la page d'espace de l'utilisateur
                return Redirect($"PollGetLinks?pollId={registred}");
            }
            return BadRequest();
        }

        /// <summary>
        /// Action permettant de charger la page d'ajout de participants à un sondage uniquement pour l'administrateur du sondage
        /// </summary>
        /// <param name="pollId">Identifiant d'un sondage</param>
        /// <returns>Vue hydratée avec quelques données d'un sondage à l'administrateur, Erreur si autre utilisateur</returns>
        [HttpGet]
        public async Task<IActionResult> PollAddParticipant(int pollId)
        {
            // On récupère la vue hydratée avec les données adhéquates tout en s'assurant que ce soit l'administrateur qui le demande
            PollAddParticipantViewModel pollAddParticipantViewModel = await _pollService.GetPollByIdForAddParticipantsAsync(pollId, _userSession);

            if (pollAddParticipantViewModel != null)
            {
                return View(pollAddParticipantViewModel);
            }
            return BadRequest();
        }

        /// <summary>
        /// Action d'enregistrement de participants à un sondage en BDD
        /// </summary>
        /// <param name="pollAddParticipantViewModel">VM de la page d'ajout de participants</param>
        /// <returns>Redirection vers l'espace personnel ou vers la page d'erreur si données incohérantes</returns>
        [HttpPost]
        public async Task<IActionResult> PollAddParticipant(PollAddParticipantViewModel pollAddParticipantViewModel)
        {
            // On enregistre la liste en base de données
            bool stateOfRegistration = await _pollService.RegisterInvitationAsync(pollAddParticipantViewModel);

            // Si aucun doublons n'a été saisi
            if (stateOfRegistration)
            {
                // On retourne la vue de l'espace personnel
                return Redirect("PersonnalSpace?message=participantsAdded");
            }
            // Si un doublon a été saisi
            return BadRequest();
        }

        /// <summary>
        /// Action permettant le chargement de la page de vote d'un sondage
        /// </summary>
        /// <param name="code">Code de participation</param>
        /// <returns>Retourne vue hydratée permettant de voter à un sondage précis pour les participants, renvoie la vue d'espace personnel pour les autres utilisateurs</returns>
        [HttpGet]
        public async Task<IActionResult> PollParticipation(string code)
        {
            // On vérifie que l'utilisateur soit autorisé à participer au sondage
            PollParticipationViewModel model = await _pollService.AllowUserToParticipateAsync(code, _userSession);

            // Si aucun sondage n'existe ou que ce dernier a été vérouillé
            if (model != null)
            {
                return View(model);
            }
            return Redirect("PersonnalSpace?message=noVote");
        }

        /// <summary>
        /// Action d'enregistrement de votes en BDD
        /// </summary>
        /// <param name="pollParticipationViewModel">VM de la page de vote</param>
        /// <returns>Retourne à l'espace personnel ou page d'erreur si données incohérantes.</returns>
        [HttpPost]
        public async Task<IActionResult> PollParticipation(PollParticipationViewModel pollParticipationViewModel)
        {
            bool registred = await _voteService.RegistrationOfVotes(pollParticipationViewModel);

            if (registred)
            {
                return Redirect($"PollResults?resultKey={pollParticipationViewModel.PollResultKey}");
            }
            return BadRequest();
        }

        /// <summary>
        /// Action permettant d'obtenir la page de fermeture d'un sondage
        /// </summary>
        /// <param name="closingKey">Clé unique de fermeture d'un sondage</param>
        /// <returns>Retourne la vue de fermeture d'un sondage</returns>
        [HttpGet]
        public async Task<IActionResult> PollClosing(string closingKey)
        {
            PollClosingViewModel pollClosingViewModel = await _pollService.PollClosingAsync(closingKey, _userSession);
            if (pollClosingViewModel != null)
            {
                return View(pollClosingViewModel);
            }
            return BadRequest();
        }

        /// <summary>
        /// Action de fermeture d'un sondage
        /// </summary>
        /// <param name="pollClosingViewModel">VM de la page de fermeture</param>
        /// <returns>Retour sur la page d'espace personnel</returns>
        [HttpPost]
        public async Task<IActionResult> PollClosing(PollClosingViewModel pollClosingViewModel)
        {
            await _pollService.ClosePollAsync(pollClosingViewModel);
            return Redirect("PersonnalSpace?message=closedPoll");
        }

        #endregion
    }
}
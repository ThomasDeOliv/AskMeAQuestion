using AskMeAQuestion.Services.ViewModel;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services.Interface
{
    public interface IPollService
    {
        /// <summary>
        /// Fonction de chargement de l'espace personnel d'un utilisateur
        /// </summary>
        /// <param name="userSession">Numéro de session de l'utilisateur dans le cookie</param>
        /// <returns>Vue modèle permettant le chargement du contenu de l'espace personnel de l'utilisateur</returns>
        Task<PersonnalSpaceViewModel> LoadPersonnalSpaceAsync(string userSession);

        /// <summary>
        /// Fonction de chargement d'une vue de création d'un sondage
        /// </summary>
        /// <param name="userSession">Numéro de session de l'utilisateur dans le cookie</param>
        /// <returns>Retourne VM de la page de création de sondage</returns>
        Task<PollCreationViewModel> CreateNewPollAsync(string userSession);

        /// <summary>
        /// Fonction permettant l'enregistrement d'un nouveau sondage
        /// </summary>
        /// <param name="model">Vue-Modèle de la vue de création enrichie avec les informations necessaires à la création d'un sondage</param>
        /// <param name="userSession">Identifiant de session de l'utilisateur</param>
        /// <returns>True si l'enregistrement du sondage, des questions et de l'administrateur s'est bien déroulé</returns>
        Task<int> RegisterNewPollAsync(PollCreationViewModel model, string userSession);

        /// <summary>
        /// Fonction chargeant une vue permettant l'ajout de participant à un sondage spécifique
        /// </summary>
        /// <param name="pollId">Id du sondage en base de données</param>
        /// <param name="userSession">Numéro de session de l'utilisateur dans le cookie</param>
        /// <returns>Vue modèle permettant le chargement des caractéristiques d'un sondage et la récupération de la liste des nouveaux participants</returns>
        Task<PollAddParticipantViewModel> GetPollByIdForAddParticipantsAsync(int pollId, string userSession);

        /// <summary>
        /// Fonction permettant d'enregistrer une liste d'invitations à un sondage
        /// </summary>
        /// <param name="pollAddParticipantViewModel">Vue model de l'ajout de participants</param>
        /// <returns>Booléen : true si la liste ajoutée est valide, faux sinon</returns>
        Task<bool> RegisterInvitationAsync(PollAddParticipantViewModel pollAddParticipantViewModel);

        /// <summary>
        /// Fonction permettant le chargement de la page de vote d'un sondage pour une personne inscrite 
        /// </summary>
        /// <param name="code">Code de participation à un sondage</param>
        /// <param name="userSession">Numéro de session de l'utilisateur dans le cookie</param>
        /// <returns>VM de la vue de vote à un sondage si tout s'est passé comme prévu, null sinon</returns>
        Task<PollParticipationViewModel> AllowUserToParticipateAsync(string code, string userSession);

        /// <summary>
        /// Fonction permettant d'obtenir les liens d'accès, de participation et de cloture d'un sondage sous condition d'etre l'administrateur du sondage
        /// </summary>
        /// <param name="pollId">Id du sondage</param>
        /// <param name="userSession">Numéro de session de l'utilisateur dans le cookie</param>
        /// <param name="host">Hôte de l'application</param>
        /// <returns>Vue modèle permettant le chargement d'une vue affichant le code de participation ainsi que les liens de résultats, participation directe et cloture d'un sondage</returns>
        Task<PollGetLinksViewModel> GetAdminLinksOfPollAsync(int pollId, string userSession, string host);

        /// <summary>
        /// Fonction vérifiant si un sondage a déjà été clôturé
        /// </summary>
        /// <param name="closingKey">Code de fermeture d'un sondage</param>
        /// <param name="userSession">Session de l'utilisateur</param>
        /// <returns>Booléen : true si fermé, false sinon</returns>
        Task<PollClosingViewModel> PollClosingAsync(string closingKey, string userSession);

        /// <summary>
        /// Procédure de cloture d'un sondage
        /// </summary>
        /// <param name="pollClosingViewModel">Vue-modèle de la page de fermeture d'un sondage</param>
        /// <returns>Aucun</returns>
        Task ClosePollAsync(PollClosingViewModel pollClosingViewModel);
    }
}

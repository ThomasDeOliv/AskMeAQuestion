using AskMeAQuestion.Services.ViewModel;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services.Interface
{
    public interface IVoteService
    {
        /// <summary>
        /// Fonction d'enregistrement d'un vote ou de modification si déjà existant
        /// </summary>
        /// <param name="pollParticipationViewModel">Vue-Modèle de la page de participation enrichie des réponses données par l'utilisateur</param>
        /// <returns>Booléen : true si enregistrement ok, false sinon</returns>
        Task<bool> RegistrationOfVotes(PollParticipationViewModel pollParticipationViewModel);

        /// <summary>
        /// Fonction permettant l'affichage des résultats d'un vote
        /// </summary>
        /// <param name="resultKey">Clé des résultats d'un sondage</param>
        /// <param name="userSession">Session de l'utilisateur</param>
        /// <returns>Retourne vue-modèle enrichie des informations requises à l'affichage d'un résultat</returns>
        Task<PollResultsViewModel> CheckResultKeyAsync(string resultKey, string userSession);
    }
}

using AskMeAQuestion.Services.ViewModel;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services.Interface
{
    public interface ISubscriptionService
    {

        /// <summary>
        /// Fonction procédant à l'inscrition d'une personne
        /// </summary>
        /// <param name="model">Vue-Modèle de la page d'inscription enrichie des informations necessaires à l'inscription</param>
        /// <returns>Booléen : true si énregistrée, faux sinon</returns>
        Task<bool> SubscriptionAsync(SubscriptionViewModel model);
    }
}

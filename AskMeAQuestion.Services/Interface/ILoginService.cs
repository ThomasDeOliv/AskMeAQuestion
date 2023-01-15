using System.Threading.Tasks;

namespace AskMeAQuestion.Services.Interface
{
    public interface ILoginService
    {
        /// <summary>
        /// Fonction permettant de vérifier si une personne qui se connecte est inscrite. Lui attribue également un cookie avec un ID de connexion unique si tel est le cas.
        /// </summary>
        /// <param name="login">Login saisi par le visiteur</param>
        /// <param name="password">Mot de passe saisi par le visiteur</param>
        /// <param name="rememberMe">Valeur booléenne représentant le fait que l'utilisateur ait coché la case "Se souvenir de moi" ou non</param>
        /// <returns>Booléen : true si l'utilisateur existe, faux sinon</returns>
        Task<bool> VerificationLoginAsync(string login, string password, bool rememberMe);

        /// <summary>
        /// Fonction vérifiant, qu'en cas de présence d'un cookie, ce dernier appartienne à l'Id de session en cours d'un utilisateur
        /// </summary>
        /// <param name="userSession">Ide de session de l'utilisateur stocké dans son cookie</param>
        /// <returns>Booléen : true si l'Id appartient à un utilisateur, false sinon</returns>
        Task<bool> CheckCookieAsync(string userSession);

        /// <summary>
        /// Procédure de déconnexion d'un utilisateur
        /// </summary>
        /// <returns>Aucun</returns>
        Task DisconnectAsync();
    }
}

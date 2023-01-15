using System;
using System.Security.Cryptography;
using System.Text;

namespace AskMeAQuestion.Services.Helper
{
    public class Security
    {
        /// <summary>
        /// Fonction de hashage d'un mot de passe
        /// </summary>
        /// <param name="stringToHash">Mot de passe en clair</param>
        /// <returns>Mot de passe hashé au format SHA256</returns>
        public string EncryptAsync(string stringToHash)
        {
            byte[] byteTabPassword = Encoding.UTF8.GetBytes(stringToHash);
            byte[] byteTabCrypted = SHA256.HashData(byteTabPassword);
            string crypted = Convert.ToBase64String(byteTabCrypted);
            return crypted;
        }
    }
}

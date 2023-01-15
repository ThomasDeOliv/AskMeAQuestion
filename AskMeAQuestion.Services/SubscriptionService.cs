using AskMeAQuestion.Data;
using AskMeAQuestion.Data.Model;
using AskMeAQuestion.Services.Helper;
using AskMeAQuestion.Services.Interface;
using AskMeAQuestion.Services.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _ctx;
        private Security _security { get; set; }
        public SubscriptionService(AppDbContext ctx)
        {
            _ctx = ctx;
            _security = new Security();
        }

        #region Fonctions

        public async Task<bool> SubscriptionAsync(SubscriptionViewModel model)
        {
            // Vérification du Nom donné par l'utilisateur
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                // Instanciation d'une liste de nom d'animaux
                List<string> Animaux = new List<string>()
                {
                    "Ours",
                    "Bélier",
                    "Guépard",
                    "Panda",
                    "Cheval",
                    "Dinde",
                    "Hippocampe",
                    "Léopard",
                    "Autruche",
                    "Faisan",
                    "Pie",
                    "Paon",
                    "Lapin",
                    "Furet",
                    "Dragon"
                };

                // Instanciation d'un generateur de nombre aleatoire
                Random rnd = new Random();

                // Saisie aléatoire d'un des noms de la liste et concatenation avec un numero aleatoire
                model.Name = Animaux[rnd.Next(Animaux.Count() - 1)] + rnd.Next(0, 100).ToString();
            }

            // Instanciation d'un nouvel utilisateur
            User newUser = new User()
            {
                UserLogin = model.Login.Trim(),
                UserMail = model.Mail.Trim(),
                UserName = model.Name.Trim(),
                UserPassword = _security.EncryptAsync(model.Password)
            };

            // Vérifier qu'il n'y ai aucun autre utilisateur avec le meme login ou la meme adresse mail dans la BDD
            var similarUser = _ctx.Users.Where(u => u.UserMail.Equals(newUser.UserMail) || u.UserLogin.Equals(newUser.UserLogin)).ToList();

            if (!similarUser.Any())
            {
                // Enregistrement dans la liste des utilisateurs puis actualisation de la base de données si aucun utilisateur similaire
                await _ctx.Users.AddAsync(newUser);
                await _ctx.SaveChangesAsync();
            }

            return !similarUser.Any();
        }

        #endregion
    }
}

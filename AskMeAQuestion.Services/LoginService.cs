using AskMeAQuestion.Data;
using AskMeAQuestion.Services.Helper;
using AskMeAQuestion.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly AppDbContext _ctx;
        private Security _security { get; set; }

        public LoginService(IHttpContextAccessor contextAccessor, AppDbContext ctx)
        {
            _contextAccessor = contextAccessor;
            _ctx = ctx;
            _security = new Security();
        }

        #region Fonctions

        public async Task<bool> VerificationLoginAsync(string login, string password, bool rememberMe)
        {
            // Mot de passe à tester, saisit par un visiteur et immédiatement crypté
            string passwordHashed = _security.EncryptAsync(password);

            // Vérifier l'existance de l'utilisateur et du mot de passe
            var searchedUser = await _ctx.Users.FirstOrDefaultAsync(u => u.UserLogin.Equals(login.Trim()) && u.UserPassword.Equals(passwordHashed));

            // Conditions si résultat existant ou non
            if (searchedUser == null)
            {
                // return false si inexistant
                await _contextAccessor.HttpContext.ForbidAsync();
                return false;
            }
            else
            {
                // Création d'un nouvel identifiant de session
                var sessionId = Guid.NewGuid().ToString();

                // Enregistrement de l'Id de session en base de donnée
                searchedUser.UserSession = sessionId;
                _ctx.Users.Update(searchedUser);
                await _ctx.SaveChangesAsync();

                // Enregistrer ces infos sous forme d'une liste de Claims
                var claims = new List<Claim>()
                {
                    new Claim("sessionId", sessionId)
                    // Le mail est crypté pour ne pas avoir de potentiels problèmes avec le RGPD
                };

                // Générer avec ces Claims un objet ClaimsIdentity
                var claimIdentity = new ClaimsIdentity(claims, "AskMeQuestionCookie");

                // Utiliser ce ClaimsIdentity pour faire un ClaimsPrincipal
                var principal = new ClaimsPrincipal(claimIdentity);

                // Connecter avec le ClaimsPrincipal - la deuxieme propriete permet de definir si le cookie est persistant ou non
                await _contextAccessor.HttpContext.SignInAsync(principal, new AuthenticationProperties()
                {
                    IsPersistent = rememberMe
                });

                // renvoyer true
                return true;
            }
        }

        public async Task<bool> CheckCookieAsync(string userSession)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task DisconnectAsync()
        {
            await _contextAccessor.HttpContext.SignOutAsync();
        }

        #endregion
    }
}

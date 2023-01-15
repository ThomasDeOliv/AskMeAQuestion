using AskMeAQuestion.Data;
using AskMeAQuestion.Data.Model;
using AskMeAQuestion.Services.Interface;
using AskMeAQuestion.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services
{
    public class PollService : IPollService
    {
        private readonly AppDbContext _ctx;

        public PollService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        #region Fonctions

        public async Task<PersonnalSpaceViewModel> LoadPersonnalSpaceAsync(string userSession)
        {
            // On recherche notre utilisateur dans la base de données
            var requestedUser = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            if (requestedUser != null)
            {
                // Instanciation d'un modèle
                PersonnalSpaceViewModel personnalSpaceViewModel = new PersonnalSpaceViewModel();

                // On effectue alors la recherche des sondages pour lesquels l'utilisateur est inscrit par son mail et auquel il a déjà voté et pour lesquels il n'est pas administrateur
                var participatedPolls = await _ctx.Polls
                    .Include(p => p.Participants)
                    .Include(p => p.User)
                    .Where(p => p.UserId != requestedUser.UserId && p.Participants.Any(p => p.ParticipantMail == requestedUser.UserMail && p.ParticipantAlreadyVoted == true))
                    .Select(p => new PollViewModel()
                    {
                        PollTitle = p.PollTitle,
                        PollDescription = p.PollDescription,
                        PollCreationDate = p.PollCreationDate,
                        PollClosingDate = p.PollClosingDate,
                        PollAccessKey = p.PollParticipationKey,
                        PollResultKey = p.PollResultKey,
                        PollAlreadyVoted = true
                    })
                    .OrderByDescending(p => p.PollCreationDate)
                    .ToListAsync();

                // Chargement de la liste des sondages crées par l'utilisateur 
                var ownPolls = await _ctx.Polls
                    .Include(p => p.Participants)
                    .Where(p => p.UserId.Equals(requestedUser.UserId))
                    .Select(p => new PollViewModel()
                    {
                        PollId = p.PollId,
                        PollTitle = p.PollTitle,
                        PollDescription = p.PollDescription,
                        PollMultipleAnswers = p.PollMultipleAnswers,
                        PollResultKey = p.PollResultKey,
                        PollAccessKey = p.PollParticipationKey,
                        PollCreationDate = p.PollCreationDate,
                        PollClosingDate = p.PollClosingDate,
                        PollDeleteKey = p.PollClosingKey,
                        PollAlreadyVoted = p.Participants.FirstOrDefault(p => p.ParticipantMail == requestedUser.UserMail).ParticipantAlreadyVoted ? true : false,
                    })
                    .OrderByDescending(p => p.PollCreationDate)
                    .ToListAsync();

                // Ajout des données au modèle
                personnalSpaceViewModel.ParticipatedPolls = participatedPolls;
                personnalSpaceViewModel.OwnPolls = ownPolls;
                personnalSpaceViewModel.UserName = requestedUser.UserName;

                return personnalSpaceViewModel;
            }
            return null;
        }

        public async Task<PollCreationViewModel> CreateNewPollAsync(string userSession)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            if (user is null)
            {
                return null;
            }

            PollCreationViewModel pollCreationViewModel = new PollCreationViewModel();

            pollCreationViewModel.UserName = user.UserName;

            return pollCreationViewModel;
        }

        public async Task<int> RegisterNewPollAsync(PollCreationViewModel model, string sessionId)
        {
            // Recherche de l'utilisateur
            User user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == sessionId);

            if (user is null)
            {
                return 0;
            }

            // Si le modèle est fiable, on instancie un nouveau sondage
            Poll poll = new Poll()
            {
                UserId = user.UserId,
                PollTitle = model.PollTitle,
                PollDescription = model.PollDescription,
                PollMultipleAnswers = model.PollMultipleAnswer,
                PollCreationDate = DateTime.Now,
                PollResultKey = Guid.NewGuid().ToString().ToUpper().Replace('-', '0'),
                PollParticipationKey = Guid.NewGuid().ToString().ToUpper().Replace('-', '0'),
                PollClosingKey = Guid.NewGuid().ToString().ToUpper().Replace('-', '0')
            };

            // On enregistre le sondage en base de donnée
            await _ctx.Polls.AddAsync(poll);
            await _ctx.SaveChangesAsync();

            // On instancie une nouvelle liste de réponses. L'Id est récupéré une fois l'enregistrement effectué du sondage
            List<Response> responses = new List<Response>();
            foreach (var item in model.PollAnswers)
            {
                Response response = new Response()
                {
                    ResponseDescription = item,
                    PollId = poll.PollId
                };
                responses.Add(response);
            }

            // On enregistre les réponses
            await _ctx.Responses.AddRangeAsync(responses);
            await _ctx.SaveChangesAsync();

            // On procède également à l'inscription automatique de l'administrateur comme participant
            await _ctx.Participants.AddAsync(new Participant()
            {
                ParticipantAlreadyVoted = false,
                ParticipantMail = user.UserMail,
                PollId = poll.PollId
            });

            // Enregistrement de la participation
            await _ctx.SaveChangesAsync();

            return poll.PollId;
        }

        public async Task<PollAddParticipantViewModel> GetPollByIdForAddParticipantsAsync(int pollId, string userSession)
        {
            // Recherche de l'utilisateur
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            // On recherche un sondage dont on connait l'Id et dont l'utilisateur est administrateur
            var requestedPoll = await _ctx.Polls
                .Include(p => p.Participants)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PollId == pollId && p.User.UserSession == userSession);

            if (requestedPoll != null && requestedPoll.PollClosingDate == null)
            {
                PollAddParticipantViewModel requestedModel = new PollAddParticipantViewModel()
                {
                    UserName = user.UserName,
                    PollId = requestedPoll.PollId,
                    PollDescription = requestedPoll.PollDescription,
                    PollTitle = requestedPoll.PollTitle,
                    PollParticipants = requestedPoll.Participants.Select(p => new PollParticipantViewModel()
                    {
                        ParticipantAlreadyVoted = p.ParticipantAlreadyVoted,
                        ParticipantMail = p.ParticipantMail
                    }).ToList()
                };
                return requestedModel;
            }
            return null;
        }

        public async Task<bool> RegisterInvitationAsync(PollAddParticipantViewModel pollAddParticipantViewModel)
        {
            // On recherche la liste des mails des participants déjà inscrits
            var currentParticipants = await _ctx.Participants
                .Where(p => p.PollId == pollAddParticipantViewModel.PollId)
                .Select(p => p.ParticipantMail)
                .ToListAsync();

            // Flag permettant de signifier qu'il existe des doublons entrés par l'utilisateur ou qu'une mauvaise donnée a été passée
            bool flag = true;

            // Traitement des données et récupération dans une task
            List<Participant> participants = new List<Participant>();

            // Déclaration d'une expression régulière vérifiant le format des mails, pour s'assurer que l'utilisateur n'ait pas entré de données non adaptées
            Regex mailMatch = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            // On vérifie ensuite que l'un des mails saisis n'existe pas en base de données et que le format mail est bien respecté
            foreach (var item in pollAddParticipantViewModel.PollNewParticipants)
            {
                if (mailMatch.IsMatch(item.Trim().ToLower()))
                {
                    if (!currentParticipants.Contains(item.Trim().ToLower()))
                    {
                        participants.Add(new Participant()
                        {
                            ParticipantMail = item.Trim().ToLower(),
                            ParticipantAlreadyVoted = false,
                            PollId = pollAddParticipantViewModel.PollId
                        });
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                else
                {
                    flag = false;
                    break;
                }
            }

            // Finalement on vérifie que l'ensemble des éléments saisis par l'utilisateur soient bien distincts si le test précédent s'est bien passé
            if (flag)
            {
                List<Participant> distinctParticipant = participants.Distinct().ToList();

                // Si tel n'est pas le cas on retourne faux
                if (!distinctParticipant.Except(participants).Any())
                {
                    // Enregistrement en BDD si toutes les opérations de vérification sont passées sans problème
                    await _ctx.Participants.AddRangeAsync(distinctParticipant);
                    await _ctx.SaveChangesAsync();
                }
                else
                {
                    flag = false;
                }
            }

            // Retour de la valeur du flag
            return flag;
        }

        public async Task<PollParticipationViewModel> AllowUserToParticipateAsync(string code, string sessionId)
        {
            // On recherche notre utilisateur
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == sessionId);

            // On récupère le sondage avec ses réponses associés et on s'assure aussi que l'utilisateur ait le droit d'y participer
            var poll = await _ctx.Polls
                .Include(p => p.Responses)
                .Include(p => p.Participants)
                .Where(p => p.Participants.Any(p => p.ParticipantMail == user.UserMail))
                .FirstOrDefaultAsync(p => p.PollParticipationKey == code);

            // On recherche le nombre de participants
            var nmbParticipants = await _ctx.Participants.Where(p => p.PollId == poll.PollId).CountAsync();

            // On retourne la valeur null si aucun sondage répondant aux précédents critères n'existe ou que ce dernier a été cloturé
            if (poll is null || poll.PollClosingDate != null)
            {
                return null;
            }

            // On vérifie si un vote pour ce sondage est déjà associé à cet utilisateur
            var relatedVotes = await _ctx.Votes
                .Include(v => v.Response)
                .Where(v => v.Response.PollId == poll.PollId & v.UserId == user.UserId)
                .ToListAsync();

            PollParticipationViewModel pollParticipationViewModel;

            // Si il n'y a pas de votes
            if (relatedVotes == null)
            {
                // On instancie notre vm
                pollParticipationViewModel = new PollParticipationViewModel()
                {
                    UserName = user.UserName,
                    PollParticipantsNumber = nmbParticipants,
                    PollResultKey = poll.PollResultKey,
                    PollTitle = poll.PollTitle,
                    PollDescription = poll.PollDescription,
                    PollId = poll.PollId,
                    PollMultipleAnswers = poll.PollMultipleAnswers,
                    UserId = user.UserId,
                    Responses = poll.Responses.Select(r => new ResponseViewModel()
                    {
                        ResponseDescription = r.ResponseDescription,
                        ResponseId = r.ResponseId,
                        ResponseIsSelected = false
                    }).ToList()
                };
            }
            else
            {
                // On instancie notre vm
                pollParticipationViewModel = new PollParticipationViewModel()
                {
                    UserName = user.UserName,
                    PollParticipantsNumber = nmbParticipants,
                    PollResultKey = poll.PollResultKey,
                    PollTitle = poll.PollTitle,
                    PollDescription = poll.PollDescription,
                    PollId = poll.PollId,
                    PollMultipleAnswers = poll.PollMultipleAnswers,
                    UserId = user.UserId,
                    Responses = poll.Responses.Select(r => new ResponseViewModel()
                    {
                        ResponseId = r.ResponseId,
                        ResponseDescription = r.ResponseDescription,
                        ResponseIsSelected = relatedVotes.FirstOrDefault(v => v.ResponseId == r.ResponseId) != null ? true : false
                    }).ToList()
                };
            }
            return pollParticipationViewModel;
        }

        public async Task<PollGetLinksViewModel> GetAdminLinksOfPollAsync(int pollId, string userSession, string host)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            var poll = await _ctx.Polls.FirstOrDefaultAsync(p => p.PollId == pollId && p.UserId == user.UserId);

            if (poll != null)
            {
                PollGetLinksViewModel pollGetLinksViewModel = new PollGetLinksViewModel()
                {
                    UserName = user.UserName,
                    PollClosingDate = poll.PollClosingDate,
                    PollTitle = poll.PollTitle,
                    PollParticipationKey = poll.PollParticipationKey,
                    PollClosingKey = poll.PollClosingKey,
                    PollResultKey = poll.PollResultKey,
                    PollParticipationLink = "https://" + host + "/Logged/PollParticipation?code=" + poll.PollParticipationKey,
                    PollClosingLink = "https://" + host + "/Logged/PollClosing?closingKey=" + poll.PollClosingKey,
                    PollResultLink = "https://" + host + "/Logged/PollResults?resultKey=" + poll.PollResultKey
                };
                return pollGetLinksViewModel;
            }
            return null;
        }

        public async Task<PollClosingViewModel> PollClosingAsync(string closingKey, string userSession)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            var poll = await _ctx.Polls.FirstOrDefaultAsync(p => p.PollClosingKey == closingKey);

            if (poll == null || poll.UserId != user.UserId)
            {
                return null;
            }

            PollClosingViewModel pollClosingViewModel = new PollClosingViewModel()
            {
                PollClosingKey = closingKey,
                UserSession = userSession,
                PollAlreadyClosed = poll.PollClosingDate != null ? true : false
            };

            return pollClosingViewModel;
        }

        public async Task ClosePollAsync(PollClosingViewModel pollClosingViewModel)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == pollClosingViewModel.UserSession);

            var poll = await _ctx.Polls.FirstOrDefaultAsync(p => p.PollClosingKey == pollClosingViewModel.PollClosingKey && p.UserId == user.UserId);

            if (poll != null)
            {
                if (poll.PollClosingDate == null)
                {
                    // Mise à jour de la date de fermeture du sondage
                    poll.PollClosingDate = DateTime.Now;

                    // Sauvegarde en BDD
                    _ctx.Polls.Update(poll);
                    await _ctx.SaveChangesAsync();
                }
            }
        }

        #endregion
    }
}

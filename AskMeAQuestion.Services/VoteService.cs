using AskMeAQuestion.Data;
using AskMeAQuestion.Data.Model;
using AskMeAQuestion.Services.Interface;
using AskMeAQuestion.Services.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMeAQuestion.Services
{

    public class VoteService : IVoteService
    {
        private readonly AppDbContext _ctx;

        public VoteService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        #region Fonctions

        public async Task<bool> RegistrationOfVotes(PollParticipationViewModel pollParticipationViewModel)
        {
            // On cherche tout d'abord l'utilisateur qui effectue le vote
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserId == pollParticipationViewModel.UserId);

            // On recherche le participant du sondage actuel associé à notre utilisateur
            var participant = await _ctx.Participants.FirstOrDefaultAsync(p => p.ParticipantMail == user.UserMail && p.PollId == pollParticipationViewModel.PollId);

            // On s'assure que le modèle de données soit cohérant : choix unique et une seule réponse OU choix multiple et autant de réponses que'l'on veut
            if ((pollParticipationViewModel.PollMultipleAnswers && pollParticipationViewModel.PollVotedResponses.Count() >= 0) || (!pollParticipationViewModel.PollMultipleAnswers && pollParticipationViewModel.PollVotedResponses.Count() == 1))
            {
                // Si le participant avait déjà voté
                if (participant.ParticipantAlreadyVoted)
                {
                    // On supprime tous les votes précédents liés au sondage et à notre utilisateur dans la table des votes
                    List<Vote> previousVotes = await _ctx.Votes.Where(v => v.UserId == user.UserId && v.Response.PollId == pollParticipationViewModel.PollId).ToListAsync();

                    // Suppression des anciens votes de la BDD
                    _ctx.Votes.RemoveRange(previousVotes);
                    await _ctx.SaveChangesAsync();
                }

                // Création d'une liste d'objets Vote qui correspondent au nouveaux votes
                List<Vote> newVotes = new List<Vote>();
                foreach (var vote in pollParticipationViewModel.PollVotedResponses)
                {
                    newVotes.Add(new Vote()
                    {
                        UserId = pollParticipationViewModel.UserId,
                        ResponseId = vote
                    });
                }

                // Enregistrement des nouveaux votes
                await _ctx.Votes.AddRangeAsync(newVotes);
                await _ctx.SaveChangesAsync();

                // Si il s'agit de la première participation, on change le status de vote du participant
                if (!participant.ParticipantAlreadyVoted)
                {
                    // Enregistrement de la participation de l'utilisateur
                    participant.ParticipantAlreadyVoted = true;
                    _ctx.Participants.Update(participant);
                    await _ctx.SaveChangesAsync();
                }

                return true;
            }
            return false;
        }

        public async Task<PollResultsViewModel> CheckResultKeyAsync(string resultKey, string userSession)
        {
            // Recherche de l'utilisateur
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserSession == userSession);

            // On recherche le sondage dont on connait la clé d'accès aux résultats
            Poll poll = await _ctx.Polls
                .Include(p => p.Participants)
                .Include(p => p.Responses)
                .ThenInclude(r => r.Votes)
                .FirstOrDefaultAsync(p => p.PollResultKey == resultKey);

            // On recherche tous les votes effectués relatifs à ce sondage
            List<Vote> votes = await _ctx.Votes
                .Include(v => v.Response)
                .ThenInclude(r => r.Poll)
                .Where(v => v.Response.Poll.PollId == poll.PollId)
                .ToListAsync();

            // Instanciation de la VM
            PollResultsViewModel pollResultsViewModel = new PollResultsViewModel()
            {
                UserName = user.UserName,
                PollDescription = poll.PollDescription,
                PollCreationDate = poll.PollCreationDate,
                PollClosingDate = poll.PollClosingDate,
                PollMultipleAnswers = poll.PollMultipleAnswers,
                PollTitle = poll.PollTitle,
                PollNumberOfGuests = poll.Participants.Count(),
                PollNumberOfVotes = votes.Count(),
                PollResponseRate = Math.Round(100d * votes.GroupBy(v => v.UserId).Count() / poll.Participants.Count(), 2),
                PollNumberOfVotesPerUser = votes.GroupBy(v => v.UserId).Count(),
                PollVotes = poll.Responses.GroupBy(r => r.ResponseId)
                                .Select(g => new VoteViewModel()
                                {
                                    VoteResponseId = g.Key,
                                    VoteResponseDescription = g.FirstOrDefault(r => r.ResponseId == g.Key).ResponseDescription,
                                    VoteNumber = votes.Count(v => v.ResponseId == g.Key),
                                    VotePrctOfVotes = votes.Count() != 0 ? Math.Round(100d * votes.Count(v => v.ResponseId == g.Key) / votes.Count(), 2) : 0
                                })
                                .OrderByDescending(v => v.VotePrctOfVotes)
                                .ToList()
            };

            return pollResultsViewModel;
        }

        #endregion
    }
}

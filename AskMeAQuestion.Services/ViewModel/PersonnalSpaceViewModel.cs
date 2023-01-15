using System;
using System.Collections.Generic;

namespace AskMeAQuestion.Services.ViewModel
{
    /// <summary>
    /// Classe permettant l'hydratation de la vue de l'espace personnel lors du chargement de cette dernière
    /// </summary>
    public class PersonnalSpaceViewModel
    {
        public string UserName { get; set; }

        public string MessageToUser { get; set; }

        public List<PollViewModel> OwnPolls { get; set; }

            = new List<PollViewModel>();
        public List<PollViewModel> ParticipatedPolls { get; set; }
            = new List<PollViewModel>();
    }

    /// <summary>
    /// Classe permettant de décrire un sondage
    /// </summary>
    public class PollViewModel
    {
        public int PollId { get; set; }

        public string PollTitle { get; set; }

        public string PollDescription { get; set; }

        public bool PollAlreadyVoted { get; set; }

        public bool PollMultipleAnswers { get; set; }

        public string PollResultKey { get; set; }

        public string PollAccessKey { get; set; }

        public string PollDeleteKey { get; set; }

        public DateTime PollCreationDate { get; set; }

        public DateTime? PollClosingDate { get; set; }
    }
}

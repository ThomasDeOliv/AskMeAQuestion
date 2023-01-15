using System.Collections.Generic;

namespace AskMeAQuestion.Services.ViewModel
{
    public class PollAddParticipantViewModel
    {
        public int PollId { get; set; }

        public string PollTitle { get; set; }

        public string PollDescription { get; set; }

        public List<string> PollNewParticipants { get; set; } = new List<string>();

        public List<PollParticipantViewModel> PollParticipants { get; set; } = new List<PollParticipantViewModel>();

        public string UserName { get; set; }
    }

    public struct PollParticipantViewModel
    {
        public string ParticipantMail { get; set; }
        public bool ParticipantAlreadyVoted { get; set; }
    }
}

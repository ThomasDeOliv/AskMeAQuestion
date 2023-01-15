using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Services.ViewModel
{
    public class PollCreationViewModel
    {
        [Required(ErrorMessage = "Merci de saisir un titre")]
        [MaxLength(120, ErrorMessage = "Le champs titre ne peut pas contenir plus de 120 caractères")]
        public string PollTitle { get; set; }

        [Required(ErrorMessage = "Merci de saisir une description")]
        [MaxLength(255, ErrorMessage = "Le champs description ne peut pas contenir plus de 255 caractères")]
        public string PollDescription { get; set; }

        public bool PollMultipleAnswer { get; set; }

        public List<string> PollAnswers { get; set; } = new List<string>();

        public string UserName { get; set; }
    }
}

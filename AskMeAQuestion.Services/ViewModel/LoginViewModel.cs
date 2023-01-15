using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Services.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Identifiant")]
        [Required(ErrorMessage = "Un identifiant est requis")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Veuillez ne saisir que des caractères alphanumériques")]
        [MaxLength(50)]
        public string UserLogin { get; set; }

        [Display(Name = "Mot de passe")]
        [Required(ErrorMessage = "Un mot de passe est requis")]
        [MaxLength(255)]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Display(Name = "Garder la session ouverte")]
        public bool UserOpenConnection { get; set; }

        public string ReturnUrl { get; set; }

        public string MessageToUser { get; set; }
    }
}

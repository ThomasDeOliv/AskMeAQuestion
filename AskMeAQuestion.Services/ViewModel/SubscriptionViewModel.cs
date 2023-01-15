using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.Services.ViewModel
{
    public class SubscriptionViewModel
    {
        [MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Veuillez ne saisir que des caractères alphanumériques")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Veuillez saisir un identifiant")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Veuillez ne saisir que des caractères alphanumériques")]
        [MaxLength(50)]
        public string Login { get; set; }

        // Expression régulière trouvée ici : https://waytolearnx.com/2019/09/expression-reguliere-pour-valider-une-adresse-mail-en-csharp.html
        [Required(ErrorMessage = "Veuillez saisir adresse mail")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Veuillez saisir une adresse valide")]
        [MaxLength(320)]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Veuillez saisir un mot de passe")]
        [StringLength(255, ErrorMessage = "Le mot de passe doit être de 8 caractères au minimum et 50 au maximum", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Le mot de passe doit contenir un minimum de 8 caractères dont au moins une majuscule et un chiffre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Veuillez confirmer votre mot de passe")]
        [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne sont pas identiques")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string MessageToUser { get; set; }
    }
}

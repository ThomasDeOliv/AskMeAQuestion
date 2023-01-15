// -------------------------------------------------------------------- Implémentations de jetons --------------------------------------------------------------------

// Conteneur des messages d'erreur
var ErrorContainer = document.getElementById('errorProposition');

// Tableau vide contenant les éléments de la liste
var list = [];

// Compteur de réponses
var listIndex = 0;

// Fonction d'ajout d'un jeton 
function AddToListAnswers() {

    // Booléen de test d'une nouvelle proposition
    var flag = true;

    // Saisie utilisateur
    var userEntry = String(document.getElementById('bottomUserEntry').value).trim().replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;");

    if (userEntry === "") {
        // Afficher message d'erreur
        ErrorContainer.innerHTML = "Merci de saisir une proposition de réponse";

        // Reinitialisation
        document.getElementById('bottomUserEntry').value = "";
    }
    else if (userEntry.length > 255) {
        // Afficher message d'erreur
        ErrorContainer.innerHTML = "Merci de ne pas saisir plus de 255 caractères par propositions";
    }
    else {

        if (list.includes(userEntry)) {

            flag = false;
        }

        if (flag == true) {

            // Réinitialiser message d'erreur
            ErrorContainer.innerHTML = "";

            if (list.includes(userEntry)) {
                ErrorContainer.innerHTML = "Cette proposition existe déjà";
            }
            else {
                // Ajout de l'élément au tableau
                list.push(userEntry);

                // Affichage du nouvel élément selon le type de retour
                lowerContainer.innerHTML += "<div id=\"newEntry[" + listIndex + "]\" class=\"pollListItem\"><p class=\"proposition2\">" + list[listIndex] + "</p><input id=\"hiddenProposition[" + listIndex + "]\" name=\"PollAnswers\" class=\"proposition\" value=\"" + list[listIndex] + "\" hidden/><button class=\"btn btn - primary deletePropositionButton\" onclick=\"deleteEntry(" + listIndex + ")\">Supprimer</button></div>";

                // Incrémentation de l'index
                listIndex++;

                // Reinitialisation
                document.getElementById('bottomUserEntry').value = "";
            }
        }
        else {

            // Reinitialisation
            document.getElementById('bottomUserEntry').value = "";

            ErrorContainer.innerHTML = "Merci de ne pas saisir deux fois la même réponse";
        }
    }
}

// Fonction de suppression d'un élément
function deleteEntry(index) {
    // On recherche quel bouton supprimé a été appelé 
    const elementToRemove = document.getElementById("hiddenProposition[" + index + "]").value;

    // On définit un tableau local
    let restartPropositions = [];

    // On le retire de la liste
    for (let i = 0; i < list.length; i++) {
        if (!(list[i] === elementToRemove)) {
            restartPropositions.push(list[i]);
        }
    }

    // On réinitialise le tableau
    list = [];
    list = restartPropositions;

    // On réinitialise le décompte d'éléments
    listIndex = list.length;

    // On retire les éléments du conteneur
    for (let j = 0; j < list.length + 1; j++) {
        let toRetire = document.getElementById('newEntry[' + j + ']');
        toRetire.remove();
    }

    // On remet l'ensemble des réponses non supprimées
    for (let k = 0; k < list.length; k++) {
        lowerContainer.innerHTML += "<div id=\"newEntry[" + k + "]\" class=\"pollListItem\"><p class=\"proposition2\">" + list[k] + "</p><input id=\"hiddenProposition[" + k + "]\" name=\"PollAnswers\" class=\"proposition\" value=\"" + list[k] + "\" hidden/><button class=\"btn btn - primary deletePropositionButton\" onclick=\"deleteEntry(" + k + ")\">Supprimer</button></div>";
    }
}

// Fonction de vérification du nombre d'éléments avant le submit
function SubmitForm()
{
    // Si le nombre total de réponses est inférieur à deux alors on bloque le submit par défaut
    if (list.length < 2)
    {
        // On affiche un message d'erreur
        ErrorContainer.innerHTML = "Veuillez saisir au moins deux réponses";

        // On retourne la valeur FAUX
        return false;
    }
    else
    {
        // Sinon on retourne VRAI
        return true;
    }
}

// -------------------------------------------------------------------- Formulaire de participation à un sondage --------------------------------------------------------------------

function CheckForParticipation() {
    var code = prompt("Saisissez le code de participation : ");
    if (code == "" || code == null) {
        return false;        
    }
    else {
        document.getElementById('insertCode').innerHTML += "<input type=\"text\" name=\"code\" id=\"codeContainer\" value=\"" + code + "\" hidden/>";
        return true;
    }
}

// -------------------------------------------------------------------- Confirmation de cloture de sondage --------------------------------------------------------------------

function ConfirmPollClosing() {
    var result = confirm('Souhaitez-vous fermer ce sondage ?');
    if (!result) {
        event.preventDefault();
    }
}

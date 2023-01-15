// -------------------------------------------------------------------- Implémentations de jetons --------------------------------------------------------------------

// Conteneur des messages d'erreur
var ErrorContainer = document.getElementById('errorProposition');

// Tableaux vides qui doivent contenant les adresses mails déjà saisies et celles entrées par l'utilisateur
var existingPropositions = [];
var newPropositions = [];

// Compteur d'adresses ajoutées
var listIndex = 0;

// Expression régulière de la forme d'un mail
const mailRegex = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

// Fonction se déclenchant au chargement de la page d'ajout de participants
function OnLoadAddParticipants(){

    // Nombre d'adresses saisies déjà enregistrées
    let maxValue = parseInt(document.getElementById('maxValuesPropositions').value);

    // Chargement de l'ensemble des propositions déjà saisies dans la liste
    for (let i = 0; i < maxValue; i++) {

        existingPropositions.push(document.getElementById('proposition[' + i + ']').value);
    }
}

// Fonction d'ajout d'un jeton 
function AddToListMails() {

    // Booléen de test d'une nouvelle proposition
    var flag = true;

    // Saisie utilisateur
    var userEntry = String(document.getElementById('bottomUserEntry').value).trim().replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;").toLowerCase();

    if (userEntry === "" ) {
        // Afficher message d'erreur
        ErrorContainer.innerHTML = "Merci de saisir une adresse mail";

        // Reinitialisation
        document.getElementById('bottomUserEntry').value = "";
    }
    else if (userEntry.length > 320) {
        // Afficher message d'erreur
        ErrorContainer.innerHTML = "Merci de ne pas saisir plus de 320 caractères";
    }
    else if (!mailRegex.test(userEntry)) {
        // Afficher message d'erreur
        ErrorContainer.innerHTML = "Merci de saisir un format d'adresse mail valide"
    }
    else {

        if (existingPropositions.includes(userEntry) || newPropositions.includes(userEntry)){

            flag = false;
        }

        if (flag == true) {

            // Réinitialiser message d'erreur
            ErrorContainer.innerHTML = "";

            // Ajout de l'élément au tableau
            newPropositions.push(userEntry);

            // Affichage du nouvel élément selon le type de retour
            lowerContainer.innerHTML += "<div id=\"newEntry[" + listIndex + "]\" class=\"pollListItem\"><p class=\"proposition2\">" + newPropositions[listIndex] + "</p><input id=\"hiddenProposition[" + listIndex + "]\" name=\"PollNewParticipants\" class=\"proposition\" value=\"" + newPropositions[listIndex] + "\" hidden/><input type=\"button\" class=\"btn btn - primary deletePropositionButton\" onclick=\"deleteEntry(" + listIndex + ")\" value=\"Supprimer\"/></div>";

            // Incrémentation de l'index
            listIndex++;

            // Reinitialisation
            document.getElementById('bottomUserEntry').value = "";
        }
        else {

            // Reinitialisation
            document.getElementById('bottomUserEntry').value = "";

            ErrorContainer.innerHTML = "Merci de ne pas saisir deux fois la même adresse";
        }
    }
}

// Fonction de suppression d'un élément
function deleteEntry(index)
{
    // On recherche quel bouton supprimé a été appelé 
    const elementToRemove = document.getElementById("hiddenProposition[" + index + "]").value;

    // On définit un tableau local
    let restartPropositions = [];

    // On le retire de la liste
    for (let i = 0; i < newPropositions.length; i++) {
        if (!(newPropositions[i] === elementToRemove)) {
            restartPropositions.push(newPropositions[i]);
        }
    }

    // On réinitialise le tableau
    newPropositions = [];
    newPropositions = restartPropositions;

    // On réinitialise le décompte d'éléments
    listIndex = newPropositions.length;

    // On retire les éléments du conteneur
    for (let j = 0; j < newPropositions.length + 1; j++) {
        let toRetire = document.getElementById('newEntry[' + j + ']');
        toRetire.remove();
    }

    // On remet l'ensemble des réponses non supprimées
    for (let k = 0; k < newPropositions.length; k++) {
        lowerContainer.innerHTML += "<div id=\"newEntry[" + k + "]\" class=\"pollListItem\"><p class=\"proposition2\">" + newPropositions[k] + "</p><input id=\"hiddenProposition[" + k + "]\" name=\"PollNewParticipants\" class=\"proposition\" value=\"" + newPropositions[k] + "\" hidden/><input type=\"button\" class=\"btn btn - primary deletePropositionButton\" onclick=\"deleteEntry(" + k + ")\" value=\"Supprimer\"/></div>";
    }
}

// Fonction de vérification du nombre d'éléments avant le submit
function SubmitForm()
{
    // Si le nombre total de réponses est nul alors on bloque le submit par défaut
    if (newPropositions.length === 0)
    {
        // On affiche un message d'erreur
        ErrorContainer.innerHTML = "Veuillez saisir au moins une nouvelle adresse";

        // On retourne la valeur FAUX
        return false;
    }
    else
    {
        // Sinon on retourne VRAI
        return true;
    }
}
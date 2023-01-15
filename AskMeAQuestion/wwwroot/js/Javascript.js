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

// -------------------------------------------------------------------- Fermeture d'un sondage --------------------------------------------------------------------

function SubmitClosing() {

    var returnType = Boolean(document.getElementById('alreadyClosed').checked);

    if (returnType) {
        var result = alert('Ce sondage a déjà été clôturé');
    }
    else {
        var result = confirm('Souhaitez-vous fermer ce sondage ?');
        if (!result) {
            event.preventDefault();
            history.go(-1);
        }
    }
}
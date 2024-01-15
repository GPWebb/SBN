FormHandler.PostLoad = function (form, response) {
    setPartTypeVisibility();

    $("#Type_Static").change(function () {
        setPartTypeVisibility();
    });
    $("#Type_Dynamic").change(function () {
        setPartTypeVisibility();
    });
}

var setPartTypeVisibility = function () {
    if ($("#Type_Static").prop("checked") == true) {
        document.querySelector("#PartXML").parentElement.parentElement.parentElement.setAttribute("style", "");
        document.querySelector("#PartXML_ActionID").parentElement.parentElement.parentElement.setAttribute("style", "display:none");
    }
    else {
        document.querySelector("#PartXML").parentElement.parentElement.parentElement.setAttribute("style", "display:none");
        document.querySelector("#PartXML_ActionID").parentElement.parentElement.parentElement.setAttribute("style", "");
    }
}
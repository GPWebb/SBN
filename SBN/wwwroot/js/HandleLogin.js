"use strict";

FormHandler.HandleResponse = function (data, formId) {
    var response = data.responseXML ?? data;

    var resultType = this.ReadNodeByName(response, "ResultType");

    switch (resultType) {
        case "Success":
            window.location.href = this.ReadNodeByName(response, "URL");
            break;

        default:
            RenderMessage("An unexpected error occurred logging in", "Error", formId, null);
            break;
    }
}

FormHandler.HandleError = function (data, formId) {
    var response = data.responseXML ?? data;

    if (response === null) {
        RenderMessage("An unexpected error ocurred logging in", "Error", formId, null);
    }

    try {
        var resultType = this.ReadNodeByName(response, "ResultType");

        switch (resultType) {
            case "InvalidUsername":
                RenderMessage("Your username could not be recognised", "Error", formId, null);
                break;

            case "InvalidPassword":
                RenderMessage("Your password was incorrect", "Error", formId, null);
                break;

            case "PasswordOutdated":
                RenderMessage("The supplied password was changed on " + this.ReadNodeByName(response, "PasswordExpiredDate"), "Error", formId, null);
                break;

            case "PasswordMustBeChanged":
                window.location.href = this.ReadNodeValue(response, "URL", false);
                break;

            case "AccountLocked":
                RenderMessage("Your account has been locked", "Error", formId, null);
                break;

            case "InvalidSessionToken":
            case "UnknownError":
            default:
                RenderMessage("Sorry, an error occurred logging in", "Error", formId, null);
                break;
        }
    }
    catch (error) {
        RenderMessage("An unexpected error ocurred logging in", "Error", formId, error);
    }
}

var msg = getParameterByName("msg");
if (!(msg === "" || msg == null)) {
    RenderMessage(getParameterByName("msg"), "Warning", "PageContent", null);
}
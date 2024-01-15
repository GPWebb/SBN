"use strict";

var deleteFailure = function (response, base) {
    RenderMessage("An error occurred deleting this record", "Error", $("#" + base).parent, errorThrown);
}

var deleteSuccess = function(response, redir) {
    window.location.href = redir + "?msg=Record+Deleted&mtyp=Success";
}

var deleteView = function (url, base, redir) {
    $.ajax({
        url: url,
        method: "DELETE",
        accepts: "text/xml",
    }).done(function (response, textStatus, jqXHR) {
            deleteSuccess(response, redir);
    }).fail(function (response, textStatus, errorThrown) {
            deleteFailure(response, base, errorThrown);
        }
    ); 
}
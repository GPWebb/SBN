"use strict";

var getElementByXPath = function(xmlDocument, xpath) {
    return new XPathEvaluator()
        .createExpression(xpath)
        .evaluate(xmlDocument, XPathResult.FIRST_ORDERED_NODE_TYPE)
        .singleNodeValue
}

//#region "Build HTML"
var CreateNode = function (definition) {
    var node = document.createElement(definition.name);

    if (definition.hasOwnProperty("attributes")) {
        for (var i = 0; i < definition.attributes.length; i++) {
            var attrDef = definition.attributes[i];

            var attribute = document.createAttribute(attrDef.name);
            attribute.nodeValue = attrDef.value;

            node.setAttributeNode(attribute);
        }
    }

    if (definition.hasOwnProperty("text")) {
        node.appendChild(document.createTextNode(definition.text));
    }

    return node;
}

var CreateDocumentChildren = function (definition, parent) {
    if (definition.hasOwnProperty("children")) {
        for (var c = 0; c < definition.children.length; c++) {
            var childNodeSet = CreateNodeSet(definition.children[c]);
            parent.appendChild(childNodeSet);
        }
    }

    return parent;
}

var CreateDocument = function (definition) {
    try {
        var root = document.implementation.createDocument(null, definition.name);

        return CreateDocumentChildren(definition, root);
    }
    catch (error) {
        throw error;
    }
}

var CreateNodeSet = function (definition) {
    var nodeSet = CreateNode(definition);

    return CreateDocumentChildren(definition, nodeSet);
}
//#endregion "Build HTML"

var RenderMessage = function (message, messageType, base, error) {
    try {
        var messageClass;
        switch (messageType) {
            case "Error":
                messageClass = "alert-danger";
                break;

            case "Warning":
                messageClass = "alert-warning";
                break;

            case "Message":
                messageClass = "alert-success";
                break;
        }

        var div = {
            name: "div",
            attributes: [
                { name: "class", value: "alert alert-dismissible fade show " + messageClass },
                { name: "role", value: "alert" }
            ],
            text: message
        };

        var btn = {
            name: "button",
            attributes: [
                { name: "type", value: "button" },
                { name: "class", value: "btn-close" },
                { name: "data-bs-dismiss", value: "alert" },
                { name: "aria-label", value: "Close" }
            ]
        };

        var messageBarDiv = CreateNode(div);

        var closeButton = CreateNode(btn);

        messageBarDiv.appendChild(closeButton);

        if (base.substring(0, 1) == "#") base = base.substring(1);

        var node = document.getElementById(base);
        var parent = node.parentNode;
        parent.insertBefore(messageBarDiv, node);

        if (messageType == "Error" || messageType == "Warning") {
            LogError(error, message);
        }
    }
    catch (error) {
        LogError(error, message);
        window.alert(message);
    }
}

var LogError = function (error, message) {
    try {
        var errorData = {
            name: "RequestLog",
            children: [
                { name: "RequestLogCategory", text: "Error" },
                { name: "Message", text: error + "\r\n" + message },
                { name: "LogURL", text: window.location },
                { name: "ReferrerURL", text: document.referrer }
            ]
        };

        var errorDoc = CreateDocument(errorData);

        //var errorDoc = document.implementation.createDocument(null, "RequestLog", null);

        //var errorCat = errorDoc.createElement("RequestLogCategory");
        //errorCat.value = "Error";
        //errorDoc.appendChild(errorCat);

        //var errorMessage = errorDoc.createElement("Message");
        //errroMessage.value = XMLEscape(error + "\r\n" + message);
        //errorDoc.appendChild(errorMessage);

        //var logURL = errorDoc.createElement("LogURL");
        //logURL.value = XMLEscape(window.location);
        //errorDoc.appendChild(logURL);

        //var referrerURL = errorDoc.createELement("ReferrerURL");
        //referrerURL.value = document.referrer;
        //errorDoc.appendChild(referrerURL)

        $.ajax({
            url: "/app/RequestLog/Log",
            method: "post",
            accepts: "text/xml",
            data: errorDoc
        });
    }
    catch (error) {

    }
}

//#region "Dialogs"
var RenderDialog = function (dialogAlias, confirmAction) {
    renderDialogInternal(dialogAlias);

    $("#modalConfirm")
        .attr("onclick", confirmAction + ";$('#modalDialog').modal('hide');");

    $("#modalDialog").modal("show");
}

var RenderDialogMany = function (dialogAlias, path, link, successMessage, errorMessage) {
    renderDialogInternal(dialogAlias);

    $("#modalConfirm")
        .attr("onclick", "executeDialogActions('" + path + "', '" + link.replaceAll("'", "\\'") + "', '" + successMessage + "', '" + errorMessage + "');$('#modalDialog').modal('hide');");

    $("#modalDialog").modal("show");
}

var executeDialogActions = function (path, link, successMessage, errorMessage) {
    var parent = document.getElementById(path + "_wrapper").parentNode.parentNode.id;
    try {
        var dt = $("#" + path).DataTable();
        var selectedRows = dt.rows({ selected: true }).data();

        for (var i = 0; i < selectedRows.length; i++) {
            var cmd = processLink(link, selectedRows[i]);
            eval(cmd);
        }
    }
    catch (error) {
        RenderMessage(errorMessage, "Error", parent, error);
    }
    finally {
        $('#modalDialog').modal('hide');
        RenderMessage(successMessage, "Message", parent, null);
    }
}

var renderDialogInternal = function (dialogAlias) {
    var dialog = dialogs.filter(function (item) { return item.alias == dialogAlias; })[0];

    $("#modalTitle")
        .text(dialog.title);
    if (dialog.bodyHTML != null) {
        $("#modalBody")
            .innerHTML(dialog.bodyHTML);
    }
    else {
        $("#modalBody")
            .text(dialog.body);
    }
    $("#modalDismiss")
        .text(dialog.dismiss);
    $("#modalConfirm")
        .text(dialog.confirm);
}

var openDialog = function (id) {
    var modal = new bootstrap.Modal(document.getElementById(id));
    modal.show();
}

var closeDialog = function (id) {
    //var modal = new bootstrap.Modal(document.getElementById(id));
    //modal.hide();

    var modal = document.getElementById(id);

    modal.setAttribute("class", "modal fade");
    modal.setAttribute("style", "display: none;");
    modal.setAttribute("aria-hidden", "true");

    modal.removeAttribute("aria-modal");
    modal.removeAttribute("role");

    var body = document.querySelector("body.modal-open");
    body.removeAttribute("class");

    var modalBackdrop = document.querySelector("div.modal-backdrop");
    body.removeChild(modalBackdrop);    
}
//#endregion

//http://stackoverflow.com/questions/133925/javascript-post-request-like-a-form-submit
var post = function(path, params, method = 'post') {
    var form = document.createElement('form');
    form.method = method;
    form.action = path;

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement('input');
            hiddenField.type = 'hidden';
            hiddenField.name = key;
            hiddenField.value = params[key];

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}

var getParameterByName = function(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

//#region "Logout"
var logoutSuccess = function(response) {
    window.location.href = '/';
    return false;
};

var logoutFailure = function(response, errorThrown) {
    RenderMessage("Sorry, an error occurred. Please try again later", "Error", "PageContent", errorThrown);
};

var logout = function() {
    $.ajax({
        url: "/api/logout",
        method: "post",
        accepts: "text/xml"
    }).done(function (response, textStatus, jqXHR) {
            logoutSuccess(response);
    }).fail(function (response, textStatus, errorThrown) {
            logoutFailure(response, errorThrown);
        }
    );
};
//#endregion "Logout

String.prototype.replaceAll = function (search, replace) {
    return this.replace(new RegExp(search, 'g'), replace)
}

var disableElement = function (elements) {
    for (var element of elements) {
        element.setAttribute("disabled", "disabled");
        element.setAttribute("aria-disabled", "true");
    }
}

var enableElement = function (elements) {
    for (var element of elements) {
        element.removeAttribute("disabled");
        element.removeAttribute("aria-disabled", "true");
    }
}

var XMLEscape = function(input) {
    return document.createElement('div')
        .appendChild(document.createTextNode(input))
        .parentNode
        .innerHTML;
}

var TransformTemplate = function (templateResponse, transformResponse) {
    var xsltProcessor = new XSLTProcessor();
    xsltProcessor.importStylesheet(transformResponse);

    var resultDocument = xsltProcessor.transformToFragment(templateResponse, document);

    return resultDocument;
}

var renderDate = function (date) {
    document.write(new Date(Date.parse(date)).toLocaleDateString());
}

var setVisibleByTag = function(element, tag) {
    var container = document.querySelectorAll("#" + element);

    for (var node of container.entries()) {
        if (tag === "All") {
            node.setAttribute("style", "");
        }
        else {
            node.setAttribute("style",
                node.dataset.tag === tag
                    ? ""
                    : "display:none");
        }
    }   
}

var filterCarouselItems = function(filter, carousel, hidden) {
    if (filter == 'all') {
        $(hidden).each(function () {
            var owl = $(".owl-carousel");
            elem = $(this).parent().html();

            owl.owlCarousel('add', elem).owlCarousel('update');
            $(this).parent().remove();
        });
    } else {
        $(hidden + '.' + filter).each(function () {
            var owl = $(".owl-carousel");
            elem = $(this).parent().html();

            owl.owlCarousel('add', elem).owlCarousel('update');
            $(this).parent().remove();
        });

        $(carousel + ' :not(.' + filter + ')').each(function () {
            var owl = $(".owl-carousel");
            targetPos = $(this).parent().index();
            elem = $(this).parent();

            $(elem).clone().appendTo($(hidden));
            owl.owlCarousel('remove', targetPos).owlCarousel('update');;
        });
    }
}
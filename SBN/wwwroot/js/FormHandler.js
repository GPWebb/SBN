"use strict";

var formHandlerOutstandingSelects = 0;
var formHandlerSelectAttempts = 100;
var selectWaitBetweenAttempts = 50;

var formHandlerOutstandingAssets = 0;
var formHandlerAssetAttempts = 100;
var assetWaitBetweenAttempts = 100;
var assetHandlerStarted = false;

var errorState = false;

var FormHandler = {
    //#region "Handle XML"
    XMLFromHTML5: function (html) {
        var singletons = [
            "area",
            "base",
            "br",
            "col",
            "command",
            "embed",
            "hr",
            "img",
            "input",
            "keygen",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr"
        ];

        for (var i = 0; i < singletons.length; i++) {
            var start = 0;
            do {
                start = html.indexOf("<" + singletons[i], start);
                if (start >= 0) {
                    var end = html.indexOf(">", start);
                    html = html.substring(0, end) + "/" + html.substring(end);
                    start = end;
                }
            } while (start >= 0)
        }

        return html;
    },

    PopulateDataSource: function (dataSource, fields) {
        while (dataSource.includes("{")) {
            var parameter = dataSource.substring(dataSource.indexOf("{") + 1, dataSource.indexOf("}", dataSource.indexOf("{")));
            var field = fields.filter(function (item) { return item.name === parameter; })[0];

            if (!field) throw ("Requested field '" + parameter + "' not found in input values when merging data");

            dataSource = dataSource.replace("{" + parameter + "}", field.value);
        }

        return dataSource;
    },

    BuildFormXML: function (root, fields) {
        try {
            var doc = document.implementation.createDocument("", "", null);
            var root = doc.createElement(root);

            for (var i = 0; i < fields.length; i++) {
                try {
                    if (fields[i].name !== "" && !fields[i].ignore) {
                        var field = doc.createElement(fields[i].name);

                        field.innerHTML = this.XMLFromHTML5(fields[i].value.toString());

                        var keys = Object.keys(fields[i]);
                        for (var k = 0; k < keys.length; k++) {
                            switch (keys[k].toLowerCase()) {
                                case "name":
                                case "value":
                                case "raw":
                                    break;

                                case "datasource":
                                    field.setAttribute("DataSource", this.PopulateDataSource(fields[i].DataSource, fields));
                                    break;

                                default:
                                    field.setAttribute(keys[k], fields[i][keys[k]]);
                            }
                        }

                        root.appendChild(field);
                    }
                }
                catch (err) {
                    throw ("Error adding field " + fields[i].name + ": " + err);
                }
            }

            doc.appendChild(root);

            var serialized = new XMLSerializer().serializeToString(doc.documentElement);

            return serialized;
        } catch (error) {
            throw "Error compiling form input to submit. Your changes have not been saved";
        }
    },

    ReadNodeByName: function (data, nodePath, handleMissing) {
        try {
            var node = data.getElementsByTagName(nodePath)[0];

            return node.nodeValue ?? node.innerHTML;
        } catch (error) {
            if (handleMissing) {
                return null
            } else {
                throw ("Could not find value of node " + node);
            }
        }
    },

    ReadNodeValues: function (data, nodePath, handleMissing, isRaw) {
        try {
            var result = data.evaluate(nodePath, data, null, XPathResult.ANY_TYPE, null);
            var nodes = [];
            var res;
            while (res = result.iterateNext()) {
                if (isRaw) {
                    var node = "";
                    for (var n = 0; n < res.childNodes.length; n++) {
                        node += res.childNodes[n].nodeValue ?? res.childNodes[n].outerHTML;
                    }
                    nodes.push(node);
                }
                else {
                    nodes.push(res.childNodes[0].nodeValue ?? res.childNodes[0].outerHTML);
                }
            }

            return nodes;
        } catch (error) {
            if (handleMissing) {
                return null
            } else {
                throw ("Could not find value of node " + nodePath);
            }
        }
    },

    ReadNodeValue: function (data, nodePath, handleMissing, isRaw) {
        var values = this.ReadNodeValues(data, nodePath, handleMissing, isRaw);
        return (values != null)
            ? values[0]
            : null;
    },
    //#endregion

    //#region "Handle response"
    UpdateFromResponse: function (form, requestUrl) {
        var originalUrl = $(form).attr("data-action");

        if (requestUrl !== "" && requestUrl != originalUrl) {
            $(form).attr("data-action", requestUrl);

            this.SetFormVariables(form, requestUrl, originalUrl);
        }
    },

    SetFormVariables: function (form, requestUrl, originalUrl) {
        var urlPattern = $(form).attr("data-action-pattern");

        var originalParts = originalUrl.split("/");
        var requestParts = requestUrl.split("/");
        var patternParts = urlPattern.split("/");

        if (originalParts.length != requestParts.length || originalParts.length != patternParts.length) {
            throw ("Resource URL lengths do not match");
        }

        for (var i = 0; i < originalParts.length; i++) {
            if (patternParts[i].startsWith("{") && patternParts[i].endsWith("}")) {
                if (originalParts[i] != requestParts[i]) {
                    var partElementID = "#" + patternParts[i].substring(1, patternParts[i].length - 1);
                    $(partElementID).val(requestParts[i]);
                }
            }
        }
    },

    HandleResponse: function (data, xhr, formId) {
        if (data == undefined) {
            RenderMessage("No data returned in response to your request", "Error", formId, null);
        }
        else {
            try {
                var form = "#" + formId;

                if (data.evaluate($(form).data("root"), data, null, XPathResult.ANY_TYPE, null) != null) {

                    var existed = ($("#" + $(form).data("record") + "ID").val() != 0);

                    this.PopulateForm(form, data);

                    var requestMessage = xhr.getResponseHeader("X-Message") ?? (existed ? "Record updated" : "Record added");
                    var requestMessageType = xhr.getResponseHeader("X-Message-Type") ?? "Message";

                    this.UpdateFromResponse(form, xhr.getResponseHeader("X-Resource-URL"));

                    RenderMessage(requestMessage, requestMessageType, formId, null);
                } else {
                    var url = this.ReadNodeValue(data, "Url", false, false);
                    var message = this.ReadNodeValue(data, "Message", false, false);
                    var messageType = this.ReadNodeValue(data, "MessageType", false, false);
                    //var statusCode = this.ReadNodePathValue(data, "StatusCode");

                    if (url !== "") {
                        window.location.href = url;
                    }

                    if (message !== "") {
                        RenderMessage(message, messageType, formId, null);
                    }
                }
            } catch (error) {
                RenderMessage("Sorry, an error occurred. Your requested action may or may not have been performed.", "Error", formId, error);
            }
        }
    },

    HandleError: function (data, formId, error) {
        RenderMessage("Sorry, an error occurred", "Error", formId, error);
    },
    //#endregion

    CallAPI: function (data, action, method, formId) {
        $.ajax({
            url: action,
            method: method,
            dataType: "xml",
            contentType: "text/xml",
            accepts: "text/xml",
            data: data,
        }).done(function (response, textStatus, jqXHR) {
            FormHandler.HandleResponse(response, jqXHR, formId);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            FormHandler.HandleError(jqXHR, formId, errorThrown);
        });
    },

    //#region "Prepare data for API"
    SetValue: function (formData, checkBox) {
        for (var i = 0; i < formData.length; i++) {
            if (formData[i].name === checkBox.id) {
                formData[i].value = checkBox.checked;
                return;
            }
        }
        formData.push({ name: checkBox.id, value: checkBox.checked });
    },

    ReadFormData: function (form) {
        var formData = $(form).serializeArray();

        for (var i = 0; i < formData.length; i++) {
            var formElement = document.getElementById(formData[i].name);

            if (formElement.nodeName.toLowerCase() == "select") {
                formData[i].text = formElement.options[formElement.selectedIndex].text;
            }

            if (formElement != null) {
                var formElementData = formElement.dataset;
                for (var d in formElementData) {
                    switch (d) {
                        case "dataSource":
                            formData[i].DataSource = formElementData[d];
                            break;

                        case "field":
                            formData[i].ignore = true;

                        case "encodeRaw":
                            break;

                        default:
                            if (!(d.startsWith("validation") || ["root", "emptyMessage", "startingValue"].includes(d))) {
                                formData[i][d] = formElementData[d];
                            }
                            break;
                    }
                }

                formData[i].raw = (formElement.hasAttribute("data-encode-raw"));
            }
        }

        var checkBoxes = $(form).find(":checkbox");

        for (var i = 0; i < checkBoxes.length; i++) {
            this.SetValue(formData, checkBoxes[i]);
        }

        return formData;
    },
    //#endregion

    //#region "Assets"
    RemoveAssetFile: function (id) {
        var removeLink = document.getElementById(id);
        var container = removeLink.parentNode.parentNode.parentNode;
        var field = container.getAttribute("id").replace("_Preview", "");

        document.getElementById(field).val = "";

        var img = container.querySelector("img");
        img.setAttribute("style", "display:none");
        img.setAttribute("src", "");

        container.setAttribute("style", "display:none");
        document.getElementById(field + "_Files").setAttribute("style", "");
    },

    PreviewAsset: function (element) {
        var assetIDField = document.getElementById(element);
        if (assetIDField.value != "") {
            var assetId = assetIDField.value;

            document.getElementById(element + "_Files").setAttribute("style", "display:none");
            var container = document.getElementById(element + "_Preview");
            container.setAttribute("style", "");

            var img = container.querySelector("img");
            img.setAttribute("style", "");
            img.setAttribute("src", "/asset/ID/" + assetId + "/th");
        }
    },

    HandleAssets: function (form) {
        assetHandlerStarted = true;

        var formID = "#" + form.attr("id");

        var assets = $(formID + " :input[type=file]");

        for (var i = 0; i < assets.length; i++) {
            var assetID = this.UploadAsset(assets[i], formID);

            if (assetID != null) {
                $(assets[i] + "_AssetID").val(assetID);
            }
        }
    },

    UploadAsset: function (asset, formId) {
        var formData = new FormData();

        var file = asset.files[0];

        if (file == null) {
            formHandlerOutstandingAssets--;
        }
        else {
            formData.append("FileUpload", file);

            formData.append("AssetType", asset.dataset.assetType);
            formData.append("AssetPath", asset.dataset.assetPath);
            if (asset.dataset.namePath != "") {
                var name = $("#" + asset.dataset.namePath).val();
                formData.append("AssetName", name);
            }
            if (asset.dataset.descriptionPath != "") {
                var description = $("#" + asset.dataset.descriptionPath).val();
                formData.append("Description", description);
            }

            $.ajax({
                type: 'PUT',
                url: '/asset/',
                data: formData,
                dataType: 'xml',
                contentType: false,
                processData: false
            }).done(function (assetCreatedResponse, textStatus, jqXHR) {
                FormHandler.HandleAssetResponse(asset, assetCreatedResponse, jqXHR);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                formHandlerOutstandingAssets--;
                errorState = true;
                FormHandler.HandleError(jqXHR, formId.substring(1));
            });
        }
    },

    HandleAssetResponse: function (asset, assetCreatedResponse, jqXHR) {
        var assetFileControlID = asset.attributes["id"].value;
        var assetIDControl = assetFileControlID.substring(0, assetFileControlID.length - 6);

        var assetID = assetCreatedResponse.querySelector("AssetID").innerHTML;
        document.getElementById(assetIDControl).value = assetID;

        formHandlerOutstandingAssets--;
    },
    //#endregion

    //#region "Save"
    Handle: function (form) {
        errorState = false;
        this.SetOutstandingAssets("#" + form.attr("id"));
        assetHandlerStarted = false;
        return this.HandleInner(form);
    },

    HandleInner: function (form) {
        try {
            if (formHandlerOutstandingAssets > 0) {
                if (!assetHandlerStarted) {
                    this.HandleAssets(form);
                }

                setTimeout(function () { FormHandler.HandleInner(form) }, assetWaitBetweenAttempts);
            }
            else if (!errorState) {
                var formData = this.ReadFormData(form);
                var data = this.BuildFormXML($(form).data("record"), formData);

                this.CallAPI(data, $(form).attr("data-action"), $(form).attr("method"), $(form).attr("id"));
            }
        }
        catch (error) {
            RenderMessage("Sorry, an error occurred. Your requested action may or may not have been performed.", "Error", form.attr("id"), error);
        }
        return false;
    },
    //#endregion

    //#region "Selects"
    CreateSelectOption: function (value, text) {
        var option = document.createElement("option");
        option.value = value;
        option.text = text;

        return option;
    },

    PopulateSelect: function (id, data) {
        var select = document.getElementById(id);

        select.add(this.CreateSelectOption("", select.getAttribute("data-empty-message")));

        var items = data.evaluate(select.dataset.root, data, null, XPathResult.ANY_TYPE, null)

        var item = items.iterateNext();
        while (item) {
            select.add(this.CreateSelectOption(item.childNodes[0].innerHTML, item.childNodes[1].innerHTML));

            if (item.childNodes.length > 2) {
                for (var i = 2; i < item.childNodes.length; i++) {
                    var node = item.childNodes[i];
                    var rawName = node.nodeName;
                    var rawData = node.innerHTML;

                    select.lastChild.setAttribute(rawName.replace("__", "data-"), rawData);
                }
            }
            item = items.iterateNext();
        }

        if (select.dataset.startingValue) {
            select.value = select.dataset.startingValue;
        }

        formHandlerOutstandingSelects--;
    },

    PopulateSelectCall: function (id, sourceURL, form) {
        $.ajax({
            url: sourceURL,
            method: "GET",
            accepts: "text/xml"
        }).done(function (response) {
                FormHandler.PopulateSelect(id, response);
        }).fail(function (response) {
                FormHandler.HandleError(response, form.substring(1));
        });
    },

    PopulateSelects: function (form) {
        var selects = $(form).find("select");

        formHandlerOutstandingSelects = selects.length;

        for (var i = 0; i < selects.length; i++) {
            var id = selects[i].getAttribute("id");
            var sourceURL = selects[i].dataset.sourceUrl;

            if (sourceURL == "") throw ("No source specified for select, cannot proceed");

            this.PopulateSelectCall(id, sourceURL, form);
        }
    },
    //#endregion

    //#region "Lock/unlock"
    LockForm: function (form) {
        var inputs = this.FormControls(form);

        for (var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if (input.hasAttribute("disabled")) {
                input.setAttribute("data-disabled", "true");
            }
            else {
                input.setAttribute("disabled", "disabled");
            }
        }
    },

    UnlockForm: function (form) {
        var inputs = this.FormControls(form);

        for (var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if (!input.hasAttribute("data-disabled")) {
                input.removeAttribute("disabled");
            }
        }
    },
    //#endregion

    //#region "Load"
    PrepareForm: function (form) {
        try {
            this.LockForm(form);

            this.PopulateSelects(form);
            this.SetOutstandingAssets(form);
        }
        catch (error) {
            RenderMessage("Sorry, an error occurred populating the form", "Error", form, error);
            errorState = true;
        }
    },

    SetOutstandingAssets: function (form) {
        formHandlerOutstandingAssets = $(form + " :input[type=file]").length;
    },

    FormControls: function (form) {
        var inputs = $(form).find(":input");
        var buttons = $(form).find(":button");

        inputs.append(buttons);

        return inputs;
    },
    //#endregion

    //#region "Populate form"
    PopulateForm: function (form, response) {
        try {
            if (formHandlerOutstandingSelects > 0) {
                if (formHandlerSelectAttempts-- > 0) {
                    setTimeout(function () { FormHandler.PopulateForm(form, response) }, selectWaitBetweenAttempts);
                }
                else {
                    RenderMessage("Sorry, an error occurred populating the form", "Error", form.substring(1), "Form selects population failed after allowed attempts");
                }
            }
            else {
                var inputs = $(form).find(":input:not([type=button]):not([type=submit]):not([type=reset])");

                var base = $(form).data("root");

                for (var i = 0; i < inputs.length; i++) {
                    var fieldName = inputs[i].getAttribute("name");
                    var fieldID = inputs[i].getAttribute("id");
                    var dataSource = inputs[i].getAttribute("data-source") ?? base + "/" + fieldName;
                    var type = (inputs[i].getAttribute("type") ?? inputs[i].tagName).toLowerCase();
                    var value = this.ReadNodeValue(response, dataSource, true, inputs[i].getAttribute("data-encode-raw") ?? false);

                    if (typeof value != "undefined") {
                        //if (raw) value = XMLEscape(value);

                        switch (type) {
                            case "text":
                            case "number":
                            case "select":
                                $("#" + fieldID).val(value);
                                break;

                            case "textarea":
                                if ($("#" + fieldID).data("editor") == "summernote")
                                    $("#" + fieldID).summernote('code', value);
                                else
                                    $("#" + fieldID).val(value);
                                break;

                            case "checkbox":
                                $("#" + fieldID).prop("checked", value);
                                break;

                            case "hidden":
                                $("#" + fieldID).val(value);
                                if ($("#" + fieldID + "[data-editor]").length) {
                                    editor.setValue(vkbeautify.xml(value));
                                }

                                if (document.getElementById(fieldID + "_Files") != null) {
                                    this.PreviewAsset(fieldID);
                                }

                                if (document.getElementById(fieldID).className == "datetimepart") {
                                    var dateTimeParts = document.getElementById(fieldID).value.split("T");
                                    document.getElementById(fieldID + "_Date").value = dateTimeParts[0];
                                    document.getElementById(fieldID + "_Time").value = dateTimeParts[1];
                                }
                                break;

                            case "radio":
                                $("#" + fieldID).prop("checked", ($("#" + fieldID).val() == value));
                                break;

                            //default:
                            //    throw (type + " not handled");
                        }
                    }
                }

                this.PostLoad(form, response);

                this.UnlockForm(form);
            }
        }
        catch (error) {
            RenderMessage("Sorry, an error occurred populating the form", "Error", form.substring(1), error);
        }
    },

    Populate: function (form) {
        if (!errorState) {
            $.ajax({
                url: $(form).attr("data-action"),
                method: "GET",
                dataType: "xml",
                contentType: "text/xml",
                accepts: "text/xml"
            }).done(function (response, textStatus, jqXHR) {
                FormHandler.PopulateForm(form, response);
            }).fail(function (jqXHR, formId, errorThrown) {
                FormHandler.HandleError(response, $(form).attr("id"), errorThrown);
            });
        }
    },
    //#endregion

    PostLoad: function (form, response) {

    }
};
"use strict";

var AssetGridHandler = {
    //#region "Load"
    PickAsset: function (assetType, assetPath, fieldID) {
        try {
            $.ajax({
                url: "/api/assets/Asset/Type/" + assetType + "?AssetPath=" + encodeURIComponent(assetPath),
                method: "GET",
                dataType: "xml",
                contentType: "application/xml",
                accepts: "application/xml"
            }).done(function (response, textStatus, jqXHR) {
                AssetGridHandler.HandleAssetLoadResponse(response, jqXHR, fieldID);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                AssetGridHandler.HandleAssetLoadError(jqXHR, fieldID, errorThrown);
            });
        }
        catch (error) {
            try {
                closeDialog(fieldID + '_Modal');

                RenderMessage("Sorry, an error occurred", "Error", fieldID, error); 
            }
            catch (e) {

            }
        }
    },

    HandleAssetLoadResponse: function (template, jqXHR, fieldID) {
        $.ajax({
            url: "/api/site/Transform/AssetImageGrid",
            method: "GET",
            dataType: "xml",
            contentType: "application/xml",
            accepts: "application/xml"
        }).done(function (response, textStatus, jqXHR) {
            AssetGridHandler.HandleTemplateLoadResponse(template, response, jqXHR, fieldID);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            AssetGridHandler.HandleTemplateLoadError(jqXHR, fieldID, errorThrown);
        });
    },

    HandleTemplateLoadResponse: function (templateResponse, transformResponse, jqXHR, fieldID) {
        var parser = new DOMParser();
        var transformElement = getElementByXPath(transformResponse, "/Transforms/Transform/TransformXsl").textContent;
        var transform = parser.parseFromString(transformElement, "application/xml");

        var transformedTemplate = TransformTemplate(templateResponse, transform);

        var gridRoot = document.getElementById(fieldID + '_AssetImageGrid');

        gridRoot.replaceChildren(transformedTemplate);

        this.DisplayDialog(fieldID);
    },

    DisplayDialog: function (fieldID) {
        openDialog(fieldID + '_Modal');

        document.getElementById(fieldID + '_Loading').setAttribute("style", "display:none");
        document.getElementById(fieldID + '_Body').removeAttribute("style");
    },

    HandleAssetLoadError: function (jqXHR, fieldID, errorThrown) {
        var statusCode = jqXHR.status;

        if (statusCode == 404) {
            this.DisplayDialog(fieldID);
        }
        else {
            var messageText;

            switch (statusCode) {
                case 422:
                    messageText = getElementByXPath(jqXHR.responseXML, "/Response/Message").innerHTML;
                    break;

                default:
                    messageText = "Sorry, an error occurred";
                    break;
            }

            closeDialog(fieldID + '_Modal');

            var form = document.getElementById(fieldID).form;

            RenderMessage(messageText, "Error", form.getAttribute("id"), errorThrown);
        }
    },

    HandleTemplateLoadError: function (jqXHR, fieldID, errorThrown) {
        var statusCode = jqXHR.status;
        var messageText;

        switch (statusCode) {
            case 404:
            case 422:
                messageText = getElementByXPath(jqXHR.responseXML, "/Response/Message").innerHTML;
                break;

            default:
                messageText = "Sorry, an error occurred";
                break;
        }

        closeDialog(fieldID + '_Modal');

        var form = document.getElementById(fieldID).form;

        RenderMessage(messageText, "Error", form.getAttribute("id"), errorThrown);
    },
    //#endregion "Load"

    //#region "Upload"
    UploadAsset: function (assetFieldID, dialogID) {
        var asset = document.getElementById(assetFieldID);
        var file = asset.files[0];

        var formData = new FormData();

        if (file != null) {
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
                AssetGridHandler.HandleAssetResponse(asset, assetCreatedResponse, jqXHR, dialogID, assetFieldID);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                AssetGridHandler.HandleError(jqXHR, dialogID, "Sorry, an error occurred. Your file may or may not have been added");
            });
        }
    },

    HandleError: function (jqXHR, dialogID, message) {
        RenderMessage(message, "Error", dialogID);
    },

    HandleAssetResponse: function (asset, assetCreatedResponse, jqXHR, dialogID, assetFieldID) {
        var fieldID = asset.getAttribute("id").replace("_Files", "");

        RenderMessage("File uploaded", "Message", dialogID, null);
        this.ClearUploadForm(assetFieldID);

        this.PickAsset(asset.dataset.assetType, asset.dataset.assetPath, fieldID);
    },

    ClearUploadForm: function (assetFieldID) {
        var fieldID = asset.getAttribute("id").replace("_Files", "");

        document.getElementById(assetFieldID).value = "";
        document.getElementById(fieldID + "_ModalAssetName").value = "";
        document.getElementById(fieldID + "_ModalAssetDescription").value = "";
    },
    //#endregion "Upload"

    //#region "Select"
    SelectAsset: function (item) {
        if (item.tagName == "IMG") {
            item = item.parentElement;
        }

        var alreadySelected = (item.dataset.selected == "true");

        var assets = document.querySelectorAll("div.AssetGridThumbnail");
        for (var i = 0; i < assets.length; i++) {
            var style = assets[i].getAttribute("style");
            if (style!=null && style.includes("background-color")) {
                assets[i].removeAttribute("style");
            }
            assets[i].removeAttribute("data-selected");
        }

        if (!alreadySelected) {
            item.setAttribute("style", "background-color: #0d6efd");
            item.setAttribute("data-selected", "true");
        }
    },

    SetAsset: function (fieldID) {
        var asset = document.querySelector('[data-selected]');

        var assetID = asset.getAttribute("id");
        var assetThumbnailSrc = asset.getElementsByTagName("img")[0].getAttribute("src");

        assetID = assetID.substr(5);
        document.getElementById(fieldID).value = assetID;

        closeDialog(fieldID + '_Modal');

        document.getElementById(fieldID + "_Preview").removeAttribute("style");
        document.getElementById(fieldID + "_Files").setAttribute("style", "display:none");
        document.getElementById(fieldID + "_Thumbnail").setAttribute("src", assetThumbnailSrc)
        document.getElementById(fieldID + "_Thumbnail").setAttribute("style", "");

    },
    //#endregion "Select"

    //#region "Search"
    SearchAsset: function (dialog) {
        var baseName = dialog.replace("_Modal", "");
        var dialog = document.querySelector("#" + dialog);
        var images = dialog.querySelectorAll("div.AssetGridThumbnail");
        var search = dialog.querySelector("#" + baseName + "_AssetSearch").value;

        for (var i = 0; i < images.length; i++) {
            if (search == "") {
                var style = images[i].getAttribute("style");
                if (style.includes("display:none")) {
                    images[i].setAttribute("style", style.replace("display:none", ""));
                }
            }
            else {
                var image = images[i].getElementsByTagName("img")[0]
                var text = image.getAttribute("alt") + "|" + image.getAttribute("title").toLowerCase();
                if (!text.toLowerCase().includes(search)) {
                    images[i].setAttribute("style", "display:none");

                    var selected = images[i].getAttribute("data-selected");
                    if (selected != null && selected.value == "true") {
                        this.SelectAsset(images[i]);
                    }
                }
            }
        }
    }
    //#endregion
};
"use strict";

var confirmActions = [];

var processLink = function (link, row) {
	while (link.includes("[%")) {
		var ref = link.substring(link.indexOf("[%") + 2, link.indexOf("]", link.indexOf("[%")));

		link = link.replace("[%" + ref + "]", row[ref]);
	}

	return link;
}

//#region Deletion
var deleteSuccess = function (response, id, idCol, grid) {
	var path = "#" + grid;

	var dt = $(path).DataTable();

	dt.rows(function (idx, data, node) {
		return eval("data." + idCol + "==" + id);
	})
		.remove()
		.draw();

	//RenderMessage("Record deleted", "Success", $(path).parent);
}

var deleteGrid = function (url, id, idCol, grid) {
	$.ajax({
		url: url + id,
		method: "DELETE",
		accepts: "text/xml"
	}).done(function (response, textStatus, jqXHR) {
			deleteSuccess(response, id, idCol, grid);
	}).fail(function (response) {
			throw ("Delete failed");
	});
}
//#endregion Deletion


//#region Buttons
var addButtons = function (tableId, buttons, buttonRoot) {
	buttonRoot.appendChild(CreateNode({ name: "div", attributes: [{ name: "id", value: tableId + "_Buttons" }] }));

	buttonRoot = document.querySelector('#' + tableId + '_Buttons');

	for (var button of buttons) {
		buttonRoot.appendChild(CreateNode(button));
		buttonRoot.appendChild(document.createTextNode(" "));
	}
};

var processSelections = function (table, buttonRoot) {
	var selected = table.querySelectorAll("tr.selected").length ?? 0;

	switch (selected) {
		case 0:
			enableElement(buttonRoot.querySelectorAll("button[data-select='None']"));
			disableElement(buttonRoot.querySelectorAll("button[data-select='Single']"));
			disableElement(buttonRoot.querySelectorAll("button[data-select='Multiple']"));
			break;

		case 1:
			disableElement(buttonRoot.querySelectorAll("button[data-select='None']"));
			enableElement(buttonRoot.querySelectorAll("button[data-select='Single']"));
			enableElement(buttonRoot.querySelectorAll("button[data-select='Multiple']"));
			break;

		default:
			disableElement(buttonRoot.querySelectorAll("button[data-select='None']"));
			disableElement(buttonRoot.querySelectorAll("button[data-select='Single']"));
			enableElement(buttonRoot.querySelectorAll("button[data-select='Multiple']"));
			break;
	}
};

var handleButton = function (button, table) {
	var dt = $("#" + table).DataTable();
	var selectedRows = dt.rows({ selected: true }).data();

	if (button.hasAttribute("data-link")) {
		var link = button.getAttribute("data-link");

		var target = processLink(link, selectedRows[0]);

		window.location.href = target;
	}

	if (button.hasAttribute("data-dialog")) {
		var dialog = button.getAttribute("data-dialog");

		confirmActions = [];

		for (var i = 0; i < selectedRows.length; i++) {
			var confirmAction = processLink(button.getAttribute("data-confirm"), selectedRows[i]);
			confirmActions.push(confirmAction);
        }

		RenderDialogMany(dialog, table, button.getAttribute("data-confirm"), button.getAttribute("data-success"), button.getAttribute("data-error"));
    }
}
//#endregion Buttons
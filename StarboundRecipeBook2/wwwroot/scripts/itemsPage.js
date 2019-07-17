
// This one is needed outside of the init scope
// Can hide in in an IIFE, no point though

const searchByCookieName = "searchBy"

window.addEventListener('DOMContentLoaded', () => {
	const searchByButtonIDs = ["searchByDisplayedName", "searchByInternalName"]
	const filterCheckboxIDs = ["filterGeneric", "filterObjects", "filterActiveItems", "filterConsumables", "partialNameMatch"]

	for (let i = 0; i < filterCheckboxIDs.length; i++) {
		let cookieValue = getCookie(filterCheckboxIDs[i])
		let element = document.getElementById(filterCheckboxIDs[i])

		if (cookieValue === "") {
			// Set selection to 'true' by default if no cookie was selected, and create the cookie
			element.checked = true
			document.cookie = filterCheckboxIDs[i] + "=true"
		}
		else {
			element.checked = cookieValue === "true"
		}
	}

	var searchBy = getCookie(searchByCookieName)
	if (searchBy === "") {
		document.cookie = searchByButtonIDs[0]
		searchBy = searchByButtonIDs[0]
	}

	let element = document.getElementById(searchBy)
	element.checked = true
});

function getCookie(cname) {
	var name = cname + "=";
	var decodedCookie = decodeURIComponent(document.cookie);
	var ca = decodedCookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function filterCheckboxTick(checkbox) {
	let filter = checkbox.getAttribute('id')
	document.cookie = filter + "=" + checkbox.checked
}

function searchByButtonClick(radioButton) {
	let filter = radioButton.getAttribute('id')
	document.cookie = searchByCookieName + "=" + filter
}


const searchByCookieName = "searchBy"
const partialNameMatchCookieName = "partialNameMatch"

const filterKeyword = "filter-"
const filterTypeOptions = {
    "ActiveItem": "Active Items",
    "Armor": "Armors",
    "Augment": "Augments",
    "Codex": "Codexes",
    "Consumable": "Consumeables",
    "CurrencyItem": "Currencies",
    "Flashlight": "Flashlights",
    "Generic": "Generic",
    "Instrument": "Instruments",
    "Liquid": "Liquids",
    "Material": "Materials",
    "Object" : "Objects",
    "Tool" : "Tools",
}

window.addEventListener('DOMContentLoaded', () => {
    const searchByButtonIDs = ["searchByDisplayedName", "searchByInternalName"]
    const checkboxContainer = document.getElementById("filterCheckboxContainer")

    Object.keys(filterTypeOptions).forEach(function (key) {
        let checkbox = document.createElement("input")
        checkboxContainer.appendChild(checkbox)

        checkbox.setAttribute("type", "checkbox")
        checkbox.setAttribute("id", filterKeyword + key)
        checkbox.onclick = function () { filterCheckboxTick(checkbox) }

        checkboxContainer.appendChild(document.createTextNode(filterTypeOptions[key]));
        checkboxContainer.appendChild(document.createElement("br"))

        let cookieValue = getCookie(filterKeyword + key)

        // Set selection to 'true' by default if no cookie was selected, and create the cookie
        if (cookieValue === "") {
            checkbox.checked = true
            document.cookie = filterKeyword + key + "=true"
        }
        else {
            checkbox.checked = cookieValue === "true"
        }
    })

    var searchBy = getCookie(searchByCookieName)
    if (searchBy === "") {
        document.cookie = searchByButtonIDs[0]
        searchBy = searchByButtonIDs[0]
    }

    let searchByElement = document.getElementById(searchBy)
    searchByElement.checked = true

    let partialNameMatch = getCookie(partialNameMatchCookieName)
    let partialNameMatchElement = document.getElementById("partialNameMatch")

    if (partialNameMatch === "") {
        partialNameMatch = "true"
        partialNameMatchElement.checked = true
    } else {
        partialNameMatchElement.checked = partialNameMatch === "true"
    }
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

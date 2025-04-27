var currentUrl = window.location.href;
var searchStrings = ["/Manager/Create", "/employee/create"];
var containsSubstring = searchStrings.some(searchString => currentUrl.includes(searchString));
if (!containsSubstring) {
    localStorage.removeItem("CurrentIdTab")
}
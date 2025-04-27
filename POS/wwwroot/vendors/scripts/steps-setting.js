$(document).ready(function () {
    //Enable Tooltips
    var tooltipTriggerList = [].slice.call(
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
    );
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    //Advance Tabs
    $(".next").click(function () {
        var nextTabLinkEl = $(".nav-tabs .active")
            .closest("li")
            .next("li")
            .find("a")[0];

        const nextTab = new bootstrap.Tab(nextTabLinkEl);
        nextTab.show();
        //localStorage.setItem("CurrentIdTab", nextTabLinkEl.id);
        //validate = validateInputs(nextTabLinkEl.id);
        //if (validate) {
        //} else {
        //    ErrorNotify();
        //}
    });

    $(".previous").click(function () {
        const prevTabLinkEl = $(".nav-tabs .active")
            .closest("li")
            .prev("li")
            .find("a")[0];

        const prevTab = new bootstrap.Tab(prevTabLinkEl);
        prevTab.show();
    //    localStorage.setItem("CurrentIdTab", prevTabLinkEl.id);
    });
});

//<script>
//    window.addEventListener('load', function () {
//                var currentIdTab = localStorage.getItem("CurrentIdTab");
//    if (currentIdTab != null && currentIdTab != undefined) {
//                    const tabElement = document.getElementById(currentIdTab);
//    if (tabElement != null && tabElement != undefined) {
//                        var tab = new bootstrap.Tab(tabElement)
//    tab.show();
//                    } else {
//        localStorage.removeItem("CurrentIdTab");
//                    }
//                }
//            });
//</script>
//<script src="vendors/scripts/manage-locastorage.js"></script>

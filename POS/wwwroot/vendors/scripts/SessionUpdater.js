SessionUpdater = (function () {
    var clientMovedSinceLastTimeout = false;
    var keepSessionAliveUrl = null;
    var timeout = 10 * 1000 * 60; // 10 minutes 

    async function setupSessionUpdater() {
        // store local value
        keepSessionAliveUrl = "/Home/KeepSessionAlive";
        // setup handlers
        listenForChanges();
        // start timeout - it'll run after n minutes
        await checkToKeepSessionAlive();
    }

    function listenForChanges() {
        $("body").one("mousemove keydown", function () {
            clientMovedSinceLastTimeout = true;
        });
    }


    // fires every n minutes - if there's been movement ping server and restart timer
    async function checkToKeepSessionAlive() {
        setTimeout(async function () { await keepSessionAlive(); }, timeout);
    }

    async function keepSessionAlive() {
        // if we've had any movement since last run, ping the server
        if (clientMovedSinceLastTimeout && keepSessionAliveUrl != null) {
            //$.ajax({
            //    type: "POST",
            //    url: keepSessionAliveUrl,
            //    success: function (data) {
            //        // reset movement flag
            //        clientMovedSinceLastTimeout = false;
            //        // start listening for changes again
            //        listenForChanges();
            //        // restart timeout to check again in n minutes
            //        checkToKeepSessionAlive();


            //    },
            //    error: function (data) {
            //        console.log("Error posting to " & keepSessionAliveUrl);
            //    }
            //});
            const response = await fetch(keepSessionAliveUrl, {
                method: "POST"

            });
            if (response.ok) {
                clientMovedSinceLastTimeout = false;
                // start listening for changes again
                listenForChanges();
                // restart timeout to check again in n minutes
                checkToKeepSessionAlive();
            }
            else {
                console.log("Error posting to " & keepSessionAliveUrl);
            }
        }
    }

    // export setup method
    return {
        Setup: setupSessionUpdater
    };

})();
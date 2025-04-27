"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("http://localhost:5030/notificationhub").build();

connection.on("NOTIFICATIONS", function (message) {
    var messageJson = JSON.parse(message);
    document.getElementsByClassName("notification-list").innerHTML += `
        <li>
            <strong>${messageJson.Header}</strong><br>
            ${messageJson.Message}
        </li>`;

    toastr.info(messageJson.Message, messageJson.Header);
});

connection.start().catch(err => console.error(err));

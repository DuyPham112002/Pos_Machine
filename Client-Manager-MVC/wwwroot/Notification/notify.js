"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("http://api.lamboro.vn/notificationhub").build();
connection.on("NOTIFICATIONS", function (message) {
    var messageJson = JSON.parse(message);
    toastSuccess(`<span style="font-weight: bold; color: red;">${messageJson.Header}</span>: ${messageJson.Message}`);
});

connection.start().catch(err => console.error(err));

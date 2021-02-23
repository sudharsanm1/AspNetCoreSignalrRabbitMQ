"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub")
.configureLogging(signalR.LogLevel.Information)
.build();


//Disable send button until connection is established
// document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    console.log("SignalR Connected.");
    // document.getElementById("sendButton").disabled = false;

    connection.invoke('GetConnectionId').then(function (connectionId) {
        document.getElementById("userInput").value = connectionId;     
        document.getElementById("userInput").disabled = true;
    });

}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    // var message = document.getElementById("messageInput").value;
    // connection.invoke("SendMessage", user, message).catch(function (err) {
    //     return console.error(err.toString());
    // });
    event.preventDefault();
});
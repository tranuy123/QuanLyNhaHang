var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("Connection started");
}).catch(function (err) {
    return console.error(err.toString());
});
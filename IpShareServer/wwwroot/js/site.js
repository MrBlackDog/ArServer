var connectionForm = document.getElementById("connectionForm");
var connectionUrl = document.getElementById("connectionUrl");
var connectButton = document.getElementById("connectButton");
var stateLabel = document.getElementById("stateLabel");
var sendMessage = document.getElementById("sendMessage");
var sendButton = document.getElementById("sendButton");
var sendForm = document.getElementById("sendForm");
var commsLog = document.getElementById("commsLog");
var closeButton = document.getElementById("closeButton");
var socket;

var scheme = document.location.protocol == "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";

connectionUrl.value = scheme + "://" + document.location.hostname + port + "/ws";
var clientGuid;

function updateState() {
    function disable() {
        sendMessage.disabled = true;
        sendButton.disabled = true;
        closeButton.disabled = true;
    }
    function enable() {
        sendMessage.disabled = false;
        sendButton.disabled = false;
        closeButton.disabled = false;
    }

    connectionUrl.disabled = true;
    connectButton.disabled = true;

    if (!socket) {
        disable();
    } else {
        switch (socket.readyState) {
            case WebSocket.CLOSED:
                stateLabel.innerHTML = "Closed";
                disable();
                connectionUrl.disabled = false;
                connectButton.disabled = false;
                break;
            case WebSocket.CLOSING:
                stateLabel.innerHTML = "Closing...";
                disable();
                break;
            case WebSocket.CONNECTING:
                stateLabel.innerHTML = "Connecting...";
                disable();
                break;
            case WebSocket.OPEN:
                stateLabel.innerHTML = "Open";
                enable();
                break;
            default:
                stateLabel.innerHTML = "Unknown WebSocket State: " + htmlEscape(socket.readyState);
                disable();
                break;
        }
    }
}

closeButton.onclick = function () {
    if (!socket || socket.readyState != WebSocket.OPEN) {
        alert("socket not connected");
    }
    socket.close(1000, "Closing from client");
}

sendButton.onclick = function () {
    if (!socket || socket.readyState != WebSocket.OPEN) {
        alert("socket not connected");
    }
    var data = $("#sendMessage").val()
    socket.send(data);
    //commsLog.innerHTML += '<tr>' +
    //    '<td class="commslog-client">Client</td>' +
    //    '<td class="commslog-server">Server</td>' +
    //    '<td class="commslog-data">' + htmlEscape(data) + '</td></tr>';
}

connectButton.onclick = function () {
    stateLabel.innerHTML = "Connecting";
    socket = new WebSocket(connectionUrl.value);
    socket.onopen = function (event) {
        console.log("Open")
        updateState();
        commsLog.innerHTML += '<tr>' +
            '<td colspan="3" class="commslog-data">Connection opened</td>' +
            '</tr>';
    };
    socket.onclose = function (event) {
        updateState();
        commsLog.innerHTML += '<tr>' +
            '<td colspan="3" class="commslog-data">Connection closed. Code: ' + htmlEscape(event.code) + '. Reason: ' + htmlEscape(event.reason) + '</td>' +
            '</tr>';
    };
    socket.onerror = function (event) {
        console.error("Hi: " + event);
    };
    socket.onmessage = function (event) {
        if (event.data.startsWith('Your id')) {
            var strings = event.data.split(':');
            clientGuid = strings[1];
        }
        commsLog.innerHTML += '<tr>' +
            '<td class="commslog-server">Server</td>' +
            '<td class="commslog-client">Client</td>' +
            '<td class="commslog-data">' + htmlEscape(event.data) + '</td></tr>';
    };
};

function htmlEscape(str) {
    return str
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

function str2ab(str) {
    var buf = new ArrayBuffer(str.length * 2); // 2 bytes for each char
    var bufView = new Uint16Array(buf);
    for (var i = 0, strLen = str.length; i < strLen; i++) {
        bufView[i] = str.charCodeAt(i);
    }
    return buf;
}
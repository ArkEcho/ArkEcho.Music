var lh = "localhost";
var lhIp = "127.0.0.1";
var net = "192.168.0.59"; // Adresse im Netzwerk
var webSocket;

function connect()
{
	var address = document.getElementById("address");
	var port = document.getElementById("port");
	var addressVal = address.value;
	var portVal = port.value;
	var fullConnection = "ws://" + addressVal + ":" + portVal;
	webSocket = new WebSocket(fullConnection);
	webSocket.onopen = function(e){ onConnected(e); };
	webSocket.onclose = function(e){ onClose(e); };
	webSocket.onmessage = function(e){ onMessage(e); };
	webSocket.onerror = function(e){ onError(e); };
}

function onConnected(e){
	alert("Connected..");
}

function onClose(e){
	alert("Disconnected...");
}

function onMessage(e){
	//var json = JSON.parse(e.data);
	//var out = "Typ: " + json.Typ + " Message: " + json.Message;
	alert('Server:' + e.data);
}

function onError(e){
	alert("Error...");
}

function send() {
    var type = document.getElementById("type");
    var valType = type.value;

    var msg = document.getElementById("msg");
    var valMsg = msg.value;

    var json = '{ "Type": '+valType+', "Message": "'+valMsg+'" }';
    webSocket.send(json);
}

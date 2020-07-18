#include "WebServer.h"

#include <QWebSocket>
#include <QNetWorkInterface>

WebServer::WebServer(const QString &name, QObject *parent)
	: QWebSocketServer(name, QWebSocketServer::NonSecureMode,parent)
{
	connect(this, SIGNAL(newConnection()), this, SLOT(newWSConnection()));
}

void WebServer::newWSConnection()
{
	QWebSocket *socket = nextPendingConnection();

	connect(socket, SIGNAL(textMessageReceived(QString)), this, SLOT(onTextMessageReceived(QString)));
	connect(socket, SIGNAL(disconnected()), this, SLOT(socketDisconnected()));

	webSocketList_.append(socket);

    emit wsConnected();
}

void WebServer::onTextMessageReceived(const QString &message)
{
    ServerMessage data;

    data.dateTime_ = QDateTime::currentDateTime();
    data.message_ = message;
    data.socket_ =  qobject_cast<QWebSocket*>(sender());

	emit newTextMessageReceived(data); // Auslösen des Signals des WebServer
}

void WebServer::socketDisconnected()
{
	QWebSocket* client = qobject_cast<QWebSocket*>(sender());

	if (!client) return;
	webSocketList_.removeAll(client); // Jedes vorkommen des Pointers in der Liste löschen, 
                                      // nicht den Inhalt der Liste :P
	client->deleteLater();            // Den Sender erst nach Ende des Slots löschen, nicht beim ausführen

    emit wsDisconnected();
}

WebServer::~WebServer()
{
    // Beim löschen der Liste werden alle Sockets disconnected
	qDeleteAll(webSocketList_); // Gesamte Liste von Pointern löschen, 
								// das selbe wie durchiterieren und durch delete Statements löschen
}

QList<QWebSocket*> WebServer::getWebSocketList()
{
	return webSocketList_;
}

QString WebServer::getWebServerNetworkAdress()
{
    QString networkAddress = "";
    foreach(const QHostAddress &address, QNetworkInterface::allAddresses())
    {
        if (address.protocol() == QAbstractSocket::IPv4Protocol && address != QHostAddress(QHostAddress::LocalHost))
        {
            networkAddress = address.toString();
            if (networkAddress.startsWith("192.168") || networkAddress.startsWith("141.7")) return networkAddress;
        }
    }
    return networkAddress;
}

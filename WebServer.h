#ifndef WEBSERVER_H
#define WEBSERVER_H

#include <QWebSocketServer>
#include <QList>
#include <QWebSocket>

struct ServerMessage
{
    QDateTime dateTime_;
    QString message_;
    QWebSocket* socket_;

    QString getTime() { return dateTime_.time().toString("hh:mm:ss.zzz"); }
    QString getDate() { return dateTime_.date().toString("dd.MM.yyyy"); }
    QString getAddress() 
    { 
        if (!socket_) return ""; 
        else return socket_->peerAddress().toString(); 
    }
};

class WebServer : public QWebSocketServer
{
	Q_OBJECT

public:
    // Verbindet Signal und Slot für neue Verbindung
	WebServer(const QString &name, QObject *parent = 0);
	~WebServer();

	QList<QWebSocket*> getWebSocketList();

    QString getWebServerNetworkAdress();

signals:
    // Bei neuer WebSocket Connection ausgelöst
    void wsConnected();

    // Bei Trennung eines WebSocket Connection ausgelöst
    void wsDisconnected();

    // Werden bei neuer Nachricht ausgelöst und übergeben Nachricht + Sender als Pointer
	void newTextMessageReceived(ServerMessage);

private slots:
    // Ausgelöst bei neuer Verbindung eines WebSocket;verbindet mit restlichen Slots;fügt der Liste hinzu
	void newWSConnection();

    // Angeschlossen an Text/Binary Nachricht Signal der WebSockets;füllt Struct mit Sender WebSocket Pointer
	void onTextMessageReceived(const QString &message);

    // Bei Ende der Verbindung wird der Socket aus der Liste gelöscht
	void socketDisconnected();

private:
	QList<QWebSocket*> webSocketList_;
};

#endif // WEBSERVER_H

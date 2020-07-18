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
    // Verbindet Signal und Slot f�r neue Verbindung
	WebServer(const QString &name, QObject *parent = 0);
	~WebServer();

	QList<QWebSocket*> getWebSocketList();

    QString getWebServerNetworkAdress();

signals:
    // Bei neuer WebSocket Connection ausgel�st
    void wsConnected();

    // Bei Trennung eines WebSocket Connection ausgel�st
    void wsDisconnected();

    // Werden bei neuer Nachricht ausgel�st und �bergeben Nachricht + Sender als Pointer
	void newTextMessageReceived(ServerMessage);

private slots:
    // Ausgel�st bei neuer Verbindung eines WebSocket;verbindet mit restlichen Slots;f�gt der Liste hinzu
	void newWSConnection();

    // Angeschlossen an Text/Binary Nachricht Signal der WebSockets;f�llt Struct mit Sender WebSocket Pointer
	void onTextMessageReceived(const QString &message);

    // Bei Ende der Verbindung wird der Socket aus der Liste gel�scht
	void socketDisconnected();

private:
	QList<QWebSocket*> webSocketList_;
};

#endif // WEBSERVER_H

#ifndef SERVERDIALOGMODEL_H
#define SERVERDIALOGMODEL_H

#include <QObject>
#include "WebServer.h"

class QWebSocket;

enum UPDATEVIEWENUM
{
    NEWCONNECTION = 0,
    CLOSEDCONNECTION,
    NEWMESSAGERECEIVED
};

class ServerDialogModel : public QObject
{
    Q_OBJECT

public:
    ServerDialogModel(QObject *parent = 0);
    ~ServerDialogModel();
    
    QList<QWebSocket*> getWebServerSocketList();
    QList<ServerMessage> getServerMessageList();
    QString getWebServerNetworkAdress();

signals:
    // Löst entsprechenden Slot im View(ServerDialog) aus, übergabe bestimmt was gemacht wird
    void updateView(int);

private slots:
    // Vom WebServer bei neuer Connection ausgelöst; Aktualisieren von twConnection
    void onWSConnected();

    // Vom WebServer bei getrennter Connection ausgelöst; Aktualisieren von twConnection
    void onWSDisconnected();

    // Wird vom WebServer bei empfangener Text-Nachricht ausgelöst; Nachricht wird mit WebSocket des Senders übergeben
    // Hier passiert das Nachrichten Handling und die entsprechende Antwort
    void onTextMessageReceived(const ServerMessage &data);

private:
    QList<ServerMessage> messageList_;
    WebServer *webServer_;
};

#endif // SERVERDIALOGMODEL_H

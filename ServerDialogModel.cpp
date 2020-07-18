#include "ServerDialogModel.h"

#include <QWebSocket>
#include <QDebug>

const QString SERVERNAME = "FelicGameServer";

ServerDialogModel::ServerDialogModel(QObject *parent)
    :QObject(parent)
    ,webServer_(0)
{

    webServer_ = new WebServer(SERVERNAME, this);
    if (webServer_->listen(QHostAddress::Any, 1000)) // Port festlegen
    {
        connect(webServer_, SIGNAL(newTextMessageReceived(ServerMessage)), this, SLOT(onTextMessageReceived(ServerMessage)));
        connect(webServer_, SIGNAL(wsConnected()), this, SLOT(onWSConnected()));
        connect(webServer_, SIGNAL(wsDisconnected()), this, SLOT(onWSDisconnected()));
    }
}

ServerDialogModel::~ServerDialogModel()
{
    webServer_->close();

    delete webServer_;
}

QList<QWebSocket*> ServerDialogModel::getWebServerSocketList()
{
    QList<QWebSocket*> list;
    if (!webServer_) return list;
    return webServer_->getWebSocketList();
}

QList<ServerMessage> ServerDialogModel::getServerMessageList()
{
    return messageList_;
}

QString ServerDialogModel::getWebServerNetworkAdress()
{
    return webServer_->getWebServerNetworkAdress();
}

void ServerDialogModel::onTextMessageReceived(const ServerMessage &data)
{
    messageList_.append(data);
    updateView(NEWMESSAGERECEIVED);
}

void ServerDialogModel::onWSConnected()
{
    emit updateView(NEWCONNECTION);
}

void ServerDialogModel::onWSDisconnected()
{
    emit updateView(CLOSEDCONNECTION);
}
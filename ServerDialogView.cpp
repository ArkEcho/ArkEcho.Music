#include "ServerDialogView.h"
#include "ServerDialogModel.h"

#include <QTableWidget>
#include <QLabel>
#include <QWebSocket>

const QString DIALOGTITLE = "Easy WebSocketServer";

enum TableWidgetConnectionBenutzerTabs
{
    VERBINDUNGEN = 0
};

enum TableWidgetMessageRegistrationTabs
{
    NACHRICHTEN = 0
};

enum TableConnectionColumns
{
    CON_PEER_ADRESSE = 0,
    CON_MAX_COUNT
};

enum TableMessageColumns
{
    MSG_DATE = 0,
    MSG_TIME,
    MSG_PEER_ADRESSE,
    MSG_NACHRICHT,
    MSG_MAX_COUNT
};

ServerDialogView::ServerDialogView(QWidget *parent)
	:QDialog(parent)
    ,ui_(0)
    ,model_(0)
{
	ui_ = new Ui::ServerDialog;
	ui_->setupUi(this);
    
    // UI initialisieren
    initUI();

    model_ = new ServerDialogModel();
    connect(model_, SIGNAL(updateView(int)), this, SLOT(onUpdateView(int)));

    // Die Server Adresse annzeigen, erst nach erstellen des Model
    setLblAdress();
}

ServerDialogView::~ServerDialogView()
{
    delete model_;
    delete ui_;
}

void ServerDialogView::onUpdateView(const int &uve)
{
    switch (uve)
    {
    case NEWCONNECTION:
        setTWConnection();
        break;
    case CLOSEDCONNECTION:
        setTWConnection();
        break;
    case NEWMESSAGERECEIVED:
        setTWMessage();
        break;
    default:
        break;
    }
    qApp->processEvents();
}

void ServerDialogView::initUI()
{
    // Window Setup
    this->setWindowTitle(DIALOGTITLE);
    this->setWindowFlags(windowFlags() | Qt::WindowMinimizeButtonHint);

    int tabwBenutzerConnectionWidth = 250;
    ui_->twConnection->setEditTriggers(QAbstractItemView::NoEditTriggers);
    ui_->twConnection->setSelectionBehavior(QAbstractItemView::SelectRows);
    ui_->twConnection->verticalHeader()->setVisible(false);
    ui_->twConnection->setMinimumWidth(tabwBenutzerConnectionWidth);
    ui_->twConnection->setColumnCount(CON_MAX_COUNT);
    ui_->twConnection->setColumnWidth(CON_PEER_ADRESSE, tabwBenutzerConnectionWidth);
    ui_->twConnection->setHorizontalHeaderLabels(QString("Peer Adresse").split(";"));

    int tabwMessageRegistrationWidth = 500;
    ui_->twMessage->setEditTriggers(QAbstractItemView::NoEditTriggers);
    ui_->twMessage->setSelectionBehavior(QAbstractItemView::SelectRows);
    ui_->twMessage->verticalHeader()->setVisible(false);
    ui_->twMessage->setMinimumWidth(tabwMessageRegistrationWidth);
    ui_->twMessage->setColumnCount(MSG_MAX_COUNT);
    ui_->twMessage->setColumnWidth(MSG_DATE, (tabwMessageRegistrationWidth / 6) * 1);
    ui_->twMessage->setColumnWidth(MSG_TIME, (tabwMessageRegistrationWidth / 6) * 1);
    ui_->twMessage->setColumnWidth(MSG_PEER_ADRESSE, (tabwMessageRegistrationWidth / 6) * 1);
    ui_->twMessage->setColumnWidth(MSG_NACHRICHT, (tabwMessageRegistrationWidth / 6) * 3);
    ui_->twMessage->setHorizontalHeaderLabels(QString("Date;Time;Peer Adress;Message").split(";"));
}

void ServerDialogView::setTWMessage()
{
    if (!model_ || !ui_) return;
    QList<ServerMessage> list = model_->getServerMessageList();
    ui_->twMessage->setRowCount(list.size());

    QListIterator<ServerMessage> it(list);
    ServerMessage sm;
    int row = 0;
    while (it.hasNext())
    {
        sm = it.next();
        ui_->twMessage->setItem(row, MSG_DATE, new QTableWidgetItem(sm.getDate()));
        ui_->twMessage->setItem(row, MSG_TIME, new QTableWidgetItem(sm.getTime()));
        ui_->twMessage->setItem(row, MSG_PEER_ADRESSE, new QTableWidgetItem(sm.getAddress()));
        ui_->twMessage->setItem(row, MSG_NACHRICHT, new QTableWidgetItem(sm.message_));
        ++row;
    }
    ui_->twMessage->scrollToBottom();
}

void ServerDialogView::setTWConnection()
{
    if (!model_ || !ui_) return;
    QList<QWebSocket*> list = model_->getWebServerSocketList();
    int listSize = list.size();
    ui_->twConnection->setRowCount(listSize);

    for (int i = 0; i < listSize; ++i)
    {
        if (!list.at(i)) continue;
        ui_->twConnection->setItem(i, CON_PEER_ADRESSE, new QTableWidgetItem(list.at(i)->peerAddress().toString()));
    }
}

void ServerDialogView::setLblAdress()
{
    QString text = "Network Address: " + model_->getWebServerNetworkAdress();
    ui_->lblAdress->setText(text);
}

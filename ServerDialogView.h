#ifndef SERVERDIALOGVIEW_H
#define SERVERDIALOGVIEW_H

#include "ui_ServerDialog.h"

#include <QDialog>

class ServerDialogModel;

class ServerDialogView : public QDialog
{
	Q_OBJECT

public:
	ServerDialogView(QWidget *parent = 0);
	~ServerDialogView();

private slots:
    // Aktualisiert das UI je nach übergabe, wird vom Model ausgelöst
    void onUpdateView(const int &uve);

private:
    ServerDialogModel *model_;
	Ui::ServerDialog *ui_;

    // Initialisiert das UI und ruft die anderen init auf
    void initUI();

    // Setzt das TW Message anhand der Message Liste
    void setTWMessage();

    // Setzt das TW Connection anhand der WebSocket Liste des WebServers
    void setTWConnection();

    // Setzt das Lbl Logo mit der Server Adresse und Port
    void setLblAdress();

};

#endif // SERVERDIALOGVIEW_H

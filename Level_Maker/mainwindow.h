#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QDial>
#include <vector>
#include <QGridLayout>
#include <QHBoxLayout>
#include <QSpinBox>
#include <QPushButton>

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();


public slots:
    void afterLinesNumChange(int);
    void colorChange(int);
    void save();
    void load();

private:
    Ui::MainWindow *ui;
    std::vector<std::vector<QDial*>> board;
    QVBoxLayout *listsOfBubbles;
    QSpinBox *linesNum;
    QSpinBox *bubbleNum;
    QPushButton *mySave;
    QPushButton *myLoad;
    void newLine(bool);
    void deleteLine();
    QString getFinalString();
    int prevLine = 0;
    QString filePath;
};

#endif // MAINWINDOW_H

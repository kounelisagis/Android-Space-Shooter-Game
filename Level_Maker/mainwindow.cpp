#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QSizePolicy>
#include <QScrollArea>
#include <QGroupBox>
#include <QFileDialog>
#include <QTextStream>
#include <QMessageBox>
#include <QLabel>
#include <QDebug>

#define maxLine 10


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    setWindowTitle (tr("Level Maker"));
    setFixedSize(750,900);


    listsOfBubbles = new QVBoxLayout();
    listsOfBubbles->setSizeConstraint(QLayout::SetFixedSize);

    QWidget* scrollAreaContent = new QWidget;
    scrollAreaContent->setLayout (listsOfBubbles);

    QScrollArea* scrollArea = new QScrollArea;
    scrollArea->setHorizontalScrollBarPolicy (Qt::ScrollBarAlwaysOff);
    scrollArea->setVerticalScrollBarPolicy (Qt::ScrollBarAlwaysOn);
    scrollArea->setWidgetResizable (true);
    scrollArea->setWidget (scrollAreaContent);


    QVBoxLayout* groupBoxLayout = new QVBoxLayout;
    groupBoxLayout->addWidget(scrollArea);

    QGroupBox *groupBox = new QGroupBox;
    groupBox->setLayout( groupBoxLayout);


    linesNum = new QSpinBox();
    QObject::connect(linesNum,  SIGNAL(valueChanged(int)), this, SLOT(afterLinesNumChange(int)));
    linesNum->setValue(0);
    linesNum->setMinimum(0);

    bubbleNum = new QSpinBox();
    bubbleNum->setValue(1);
    bubbleNum->setMinimum(1);


    mySave = new QPushButton("SAVE");
    QObject::connect(mySave,  SIGNAL(released()), this, SLOT(save()));

    myLoad = new QPushButton("LOAD");
    QObject::connect(myLoad,  SIGNAL(released()), this, SLOT(load()));

    QHBoxLayout *mainLayout = new QHBoxLayout;
    mainLayout->addWidget(groupBox);

    QVBoxLayout* details = new QVBoxLayout;

    QHBoxLayout* bubbleBox = new QHBoxLayout;
    QLabel *label1 = new QLabel("bubbles:");
    bubbleBox->addWidget(label1);
    bubbleBox->addWidget(bubbleNum);

    QHBoxLayout* linesBox = new QHBoxLayout;
    QLabel *label2 = new QLabel("lines:");
    linesBox->addWidget(label2);
    linesBox->addWidget(linesNum);

    details->addLayout(bubbleBox);
    details->addLayout(linesBox);

    details->addWidget(myLoad);
    details->addWidget(mySave);
    details->setSizeConstraint(QLayout::SetMinimumSize);

    mainLayout->addLayout(details);

    QWidget* widget = new QWidget(this);
    widget->setLayout(mainLayout);

    setCentralWidget(widget);

}


void MainWindow::load()
{
    filePath = QFileDialog::getOpenFileName(this, tr("Load Level"), QDir::homePath()+"/Desktop", tr("Text (*.txt)"));

    QFile file(filePath);

    if(!file.open(QIODevice::ReadOnly)) {
        QMessageBox::information(nullptr, "error", file.errorString());
        return;
    }

    QTextStream in(&file);
    QString text = in.readAll();
    //QString path = file.fileName();
    //QStringList parts = path.split("/");
    //nameToSave = parts.at(parts.size()-1);
    file.close();

    //qDebug() << fileName;

    QStringList list = text.split('\n');

    bubbleNum->setValue(list[0].toInt());

    linesNum->setValue(0);
    linesNum->setValue(list.count()-1);


    for(int i=1;i<list.size();i++)
    {
        QStringList temp = list[i].split(' ');

        if(i%2==0)
            temp.removeAt(0);

        for(int j=0;j<temp.size();j++)
        {
            int x = temp[j].toInt();
            int value = x + 3 <= 6 ? x + 3 : x - 4;

            board[i-1][j]->setValue(value);
        }

    }

}

void MainWindow::save()
{
    //QString destination = "/Desktop/";// + nameToSave;

    QString fileName = QFileDialog::getSaveFileName(this, tr("Save Level"), filePath, tr("Text (*.txt)"));

    QFile file(fileName);

    if (file.open(QIODevice::ReadWrite | QIODevice::Truncate | QIODevice::Text)) {
        QTextStream stream(&file);
        stream << getFinalString();
        file.close();
    }

}

QString MainWindow::getFinalString()
{
    QString finalString = QString::number(bubbleNum->value()) + '\n';

    for(unsigned int i=0;i<board.size();i++)
    {
        if(i%2==1)
            finalString += ' ';

        for(unsigned int j=0;j<board[i].size();j++)
        {
            int value = board[i][j]->value() - 3 >= 0 ? board[i][j]->value() - 3 : board[i][j]->value() + 4;
            finalString += QString::number(value);

            if(j+1!=board[i].size())
                finalString += ' ';
        }
        if(i+1!=board.size())
            finalString += '\n';

    }

    return finalString;
}

void MainWindow::afterLinesNumChange(int i)
{
    while(i>prevLine)
    {
        bool fullLine = prevLine%2==0 ? true : false;
        newLine(fullLine);
        prevLine++;
    }
    while(i<prevLine)
    {
        deleteLine();
        prevLine--;
    }
}

void MainWindow::newLine(bool fullLine)
{
    QHBoxLayout *temp = new QHBoxLayout;

    std::vector<QDial*> line;

    if(!fullLine)
        temp->addStretch(25);

    int times = fullLine ? maxLine : maxLine-1;

    for(int j=0;j<times;j++)
    {
        QDial* myDial = new QDial();

        //---PROPERTIES---

        myDial->setMinimum(0);
        myDial->setMaximum(6);
        myDial->setFixedSize(50, 50);
        myDial->setValue(3);

        //----------------

        line.push_back(myDial);

        temp->addWidget(myDial);

        QObject::connect(myDial,  SIGNAL(valueChanged(int)), this, SLOT(colorChange(int)));

    }

    if(!fullLine)
        temp->addStretch(25);


    board.push_back(line);

    listsOfBubbles->addLayout(temp);

}

void MainWindow::deleteLine()
{
    std::vector<QDial*> last_line = board.back();
    board.pop_back();

    for (QDial* myBubble : last_line)
        delete myBubble;

    QLayoutItem *temp = listsOfBubbles->takeAt(listsOfBubbles->count()-1);

    delete temp;
}


void MainWindow::colorChange(int i){

    QDial* dial = qobject_cast<QDial *>(QObject::sender());

    if(i==3)
        dial->setStyleSheet("background-color:white;");
    else if(i==4)
        dial->setStyleSheet("background-color:red;");
    else if(i==5)
        dial->setStyleSheet("background-color:green;");
    else if(i==6)
        dial->setStyleSheet("background-color:blue;");
    else if(i==0)
        dial->setStyleSheet("background-color:yellow;");
    else if(i==1)
        dial->setStyleSheet("background-color:magenta;");
    else if(i==2)
        dial->setStyleSheet("background-color:cyan;");
}

MainWindow::~MainWindow()
{
    delete ui;
}

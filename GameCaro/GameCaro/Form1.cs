using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager socket;
        #endregion
        public Form1()
        {
            InitializeComponent();

            ChessBoard = new ChessBoardManager(panelBoard, txbPlayerName, pctbMark);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            procCoolDown.Step = Cons.COOL_DOWN_STEP;
            procCoolDown.Maximum = Cons.COOL_DOWN_TIME;
            procCoolDown.Value = 0;

            tmCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

            socket = new SocketManager();

            //ChessBoard.DrawChessBoard();

            //tmCoolDown.Start();

            NewGame();
        }
        #region Method
        void EndGame()
        {
            tmCoolDown.Stop();
            panelBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
           // MessageBox.Show("Ket thuc");
        }

        void NewGame()
        {
            procCoolDown.Value = 0;
            tmCoolDown.Stop();
            undoToolStripMenuItem.Enabled = true;
            ChessBoard.DrawChessBoard();       
        }

        void Undo()
        {
            ChessBoard.Undo();
            procCoolDown.Value = 0;
        }

        void Quit()
        {
            
                Application.Exit();
        }

        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmCoolDown.Start();
           panelBoard.Enabled = false;        //***********
            procCoolDown.Value = 0;

            socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "", e.ClickedPoint));

            undoToolStripMenuItem.Enabled = false;

            Listen();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
            socket.Send(new SocketData((int)SocketCommand.END_GAME, "", new Point()));
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            procCoolDown.PerformStep();

            if(procCoolDown.Value >= procCoolDown.Maximum )
            {
                EndGame();
                socket.Send(new SocketData((int)SocketCommand.TIME_OUT, "", new Point()));
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
            socket.Send(new SocketData((int)SocketCommand.NEW_GAME, "", new Point()));
            panelBoard.Enabled = true;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Ban co chac khong?", "Thong Bao", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                //Application.Exit();
            e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                }catch  { }
            }
        }

        private void btnConnect_LAN_Click(object sender, EventArgs e)
        {
            socket.IP = txbIP_Address.Text;

            if (!socket.ConnectServer())
            {
                socket.isServer = true;
                panelBoard.Enabled = true;
                socket.CreateServer();
            }
            else
            {
                socket.isServer = false;
                panelBoard.Enabled = false;
                Listen();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP_Address.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(txbIP_Address.Text))
            {
                txbIP_Address.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        void Listen()
        {
           
                Thread listenThread = new Thread(() =>
                {
                    try
                    {
                        SocketData data = (SocketData)socket.Receive();
                        ProcessData(data);
                    }
                    catch(Exception e) {  } 
                });
                listenThread.IsBackground = true;
                listenThread.Start();
            
        }

        void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        panelBoard.Enabled = false; 
                    }));
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        procCoolDown.Value = 0;
                        panelBoard.Enabled = true;
                        tmCoolDown.Start();
                        ChessBoard.OtherPlayerMarked(data.Point);

                        undoToolStripMenuItem.Enabled = true;
                    }));
                    break;
                case (int)SocketCommand.UNDO:
                    Undo();
                    procCoolDown.Value = 0;
                   
                    break;
                case (int)SocketCommand.END_GAME:
                    MessageBox.Show("Đã kết thúc game  ");
                    break;
                case (int)SocketCommand.TIME_OUT:
                    MessageBox.Show("HẾT GIỜ  ");
                    break;
                case (int)SocketCommand.QUIT:
                    tmCoolDown.Stop();
                    MessageBox.Show("Người chơi đã thoát ");
                    break;
                default:
                    break;
            }

            Listen();
        }
        
            
        
        #endregion

        
    }
}

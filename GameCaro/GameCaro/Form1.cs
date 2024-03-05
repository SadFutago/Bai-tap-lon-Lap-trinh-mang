using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
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

            ChessBoard.DrawChessBoard();

            //tmCoolDown.Start();

            NewGame();
        }

        void EndGame()
        {
            tmCoolDown.Stop();
            panelBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            MessageBox.Show("Ket thuc");
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
        }

        void Quit()
        {
            
                Application.Exit();
        }

        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            procCoolDown.Value = 0;
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            procCoolDown.PerformStep();

            if(procCoolDown.Value >= procCoolDown.Maximum )
            {
                
                EndGame();                
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
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
        }
    }
}

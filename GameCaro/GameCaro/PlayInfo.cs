﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro
{
    public class PlayInfo
    {
        private Point point;

        public Point Point
        {
            get { return point; }
            set { point = value; }
        }

        private int currentPlayer;
        public int CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        public PlayInfo(Point point, int currentPLayer)
        {
            this.Point = point;
            this.CurrentPlayer = currentPLayer;
        }

       
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace BoringPlatformer
{
    public partial class Form1 : Form
    {
        //player
        Rectangle player = new Rectangle(290, 380, 20, 20);
        const int playerXSpeed = 7;

        //platforms
        List<Rectangle> platform = new List<Rectangle>();

        //end door
        Rectangle doorRectangle = new Rectangle(615, 140, 30, 30);
        Rectangle doorEllipse = new Rectangle(615, 125, 30, 30);

        //player control variables
        bool wDown = false;
        bool aDown = false;
        bool sDown = false;
        bool dDown = false;
        bool spaceDown = false;

        //variable to track when the player is in the air
        bool inAir = false;

        //jumping stuff
        //made up numbers: List<int> jumpYSpeed = new List<int>(new int[] { -15, -10, -6, -3, -1, 0, 0, 0, 1, 3, 6, 10, 15 });
        //made up numbers 2.0: List<int> jumpYSpeed = new List<int>(new int[] { -30, -25, -20, -16, -12, -9, -6, -4, -2, -1, 0, 1, 2, 4, 6, 9, 12, 16, 20, 25, 30 });
        //parabola: List<int> jumpYSpeed = new List<int>(new int[] {-36, -25, -16, -9, -4, -1, 0, 1, 4, 9, 16, 25, 36, 49, 64 });
        List<int> jumpYSpeed = new List<int>(new int[] { -25, -18, -13, -9, -5, -2, -1, 0, 1, 2, 5, 9, 13, 18, 25, 32, 39, 45, 50, 54, 57, 59, 60, 60, 60, 60, 60, 60, 60, 60, 60 });

        int jumpCounter = 0;
        int score = 0;
        int gamesWon = 0;

        //brush
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //gamestate variable
        string gameState = "waiting";

        public Form1()
        {
            InitializeComponent();

            scoreLabel.Visible = false;
        }

        public void GameInitialize()
        {
            titleLabel.Visible = false;
            subtitleLabel.Visible = false;
            scoreLabel.Visible = true;
            scoreLabel.Text = "0";

            gameTimer.Enabled = true;
            gameState = "running";
            score = 0;
            jumpCounter = 0;
            inAir = false;

            player.Location = new Point(40, 340);

            platform.Add(new Rectangle (0, 360, 180, 20));
            platform.Add(new Rectangle (260, 360, 180, 20));
            platform.Add(new Rectangle (500, 300, 150, 20));
            platform.Add(new Rectangle (340, 230, 80, 20));
            platform.Add(new Rectangle (500, 170, 260, 20));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Space:
                    spaceDown = true;
                    break;
                case Keys.Enter:
                    if (gameState == "waiting" || gameState == "over, died" || gameState == "over, won")
                    {
                        SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);
                        confirmBeep.Play();
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over, died" || gameState == "over, won")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
                case Keys.Space:
                    spaceDown = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move the player
            if (aDown == true && player.X > 0)
            {
                player.X -= playerXSpeed;
            }
            
            if (dDown == true && player.X <= this.Width - player.Width)
            {
                player.X += playerXSpeed;
            }

            if (spaceDown == true && inAir == false)
            {
                player.Y += jumpYSpeed[0];
                jumpCounter = 1;
                inAir = true;
            }
            else if (inAir == true)
            {
                player.Y += jumpYSpeed[jumpCounter];
                jumpCounter++;
                //troubleshootLabel.Text = $"jumpCounter = {jumpCounter}";

                for (int i = 0; i < platform.Count; i++)
                {
                    if (player.X + player.Width > platform[i].X && player.X < platform[i].X + platform[i].Width && player.Y <= platform[i].Y && player.Y + jumpYSpeed[jumpCounter] >= platform[i].Y)
                    {
                        player.Y = platform[i].Y - player.Height;
                        inAir = false;
                    }
                }
                if (player.Y > this.Height)
                {
                    gameState = "over, died";
                    gameTimer.Enabled = false;
                    platform.Clear();
                }
            }
            else
            {
                bool onPlatform = false;

                for (int i = 0; i < platform.Count; i++)
                {
                    //for if the player is above the platform
                    if (player.X + player.Width > platform[i].X && player.X < platform[i].X + platform[i].Width && player.Y == platform[i].Y - player.Height)
                    {
                        onPlatform = true;
                    }
                }
                
                if (!onPlatform)
                {
                    inAir = true;
                    jumpCounter = 8;
                }
            }

            switch (gamesWon)
            {
                case 1: //make the one platform a bit too far away to reach, add extra invisible rectangles
                    break;
                case 2: //make the platforms start to fall once the player hops onto them
                    break;
                case 3: //make the platforms move away from the player
                    break;
            }

            //check if player reached end door
            if (player.IntersectsWith(doorRectangle) || player.IntersectsWith(doorEllipse))
            {
                gameState = "over, won";
            }

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameState == "waiting")
            {
                titleLabel.Text = "BORING PLATFORMER";
                subtitleLabel.Text = "Press Enter to Start or Esc to Exit";
            }
            else if (gameState == "running")
            {
                //draw player
                e.Graphics.FillRectangle(whiteBrush, player);

                //draw platforms
                for (int i = 0; i < platform.Count; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, platform[i]);
                }

                //draw end door
                e.Graphics.FillRectangle(whiteBrush, doorRectangle);
                e.Graphics.FillEllipse(whiteBrush, doorEllipse);
            }
            else if (gameState == "over, died")
            {
                titleLabel.Text = "YOU DIED";
                subtitleLabel.Text = "Press Enter to Try Again or Esc to Exit";

                scoreLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
            else if (gameState == "over, won")
            {
                titleLabel.Text = "YOU WON!!!";
                subtitleLabel.Text = "Press Enter to Play Again or Esc to Exit";

                scoreLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;

                gamesWon++;
            }
        }
    }
}
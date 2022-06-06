using System;
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
        const int playerXSpeed = 6;
        int playerYSpeed = 0;

        //platforms
        List<Rectangle> platform = new List<Rectangle>();

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

            player.Location = new Point(290, 380);

            platform.Add(new Rectangle (0, 360, 180, 20));
            platform.Add(new Rectangle(260, 360, 180, 20));
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
                    if (gameState == "waiting" || gameState == "over")
                    {
                        SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);
                        confirmBeep.Play();
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over")
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
                    if (player.X + player.Width > platform[i].X && player.X < platform[i].X + platform[i].Width && player.Y >= platform[i].Y && player.Y + jumpYSpeed[jumpCounter] >= platform[i].Y)
                    {
                        player.Y = platform[i].Y - player.Height;
                        inAir = false;
                    }
                }
                if (player.Y > this.Height)
                {
                    gameState = "over";
                    gameTimer.Enabled = false;
                }
            }
            
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameState == "waiting")
            {
                titleLabel.Text = "BORING PLATFORMER";
                subtitleLabel.Text = "Press Enter to Start or Escape to Exit";
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
            }
            else if (gameState == "over")
            {
                titleLabel.Text = "titleLabel";
                subtitleLabel.Text = "subtitleLabel";

                scoreLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
        }
    }
}
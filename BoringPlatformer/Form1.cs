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
        const int playerXSpeed = 7;

        //platforms
        List<Rectangle> platform = new List<Rectangle>();

        //end door
        Rectangle doorRectangle = new Rectangle(615, 140, 30, 30);
        Rectangle doorEllipse = new Rectangle(615, 125, 30, 30);

        //player control variables
        bool aDown = false;
        bool dDown = false;
        bool spaceDown = false;

        //variable to track when the player is in the air
        bool inAir = false;

        //jumping stuff
        //made up numbers: List<int> jumpYSpeed = new List<int>(new int[] { -15, -10, -6, -3, -1, 0, 0, 0, 1, 3, 6, 10, 15 });
        //made up numbers 2.0: List<int> jumpYSpeed = new List<int>(new int[] { -30, -25, -20, -16, -12, -9, -6, -4, -2, -1, 0, 1, 2, 4, 6, 9, 12, 16, 20, 25, 30 });
        //parabola: List<int> jumpYSpeed = new List<int>(new int[] {-36, -25, -16, -9, -4, -1, 0, 1, 4, 9, 16, 25, 36, 49, 64 });
        //List<int> jumpYSpeed = new List<int>(new int[] { -25, -18, -13, -9, -5, -2, -1, 0, 1, 2, 5, 9, 13, 18, 25, 32, 39, 45, 50, 54, 57, 59, 60, 60, 60, 60, 60, 60, 60, 60, 60 });
        List<int> jumpYSpeed = new List<int>(new int[] { -25, -18, -13, -9, -5, -2, -1, 0, 1, 2, 5, 9, 13, 18, 25, 32, 37, 40, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41, 41 });

        int jumpCounter = 0;
        int level = 1;

        //brush
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //gamestate variable
        string gameState = "waiting";

        public Form1()
        {
            InitializeComponent();

            levelLabel.Visible = false;
        }

        public void GameInitialize()
        {
            titleLabel.Visible = false;
            subtitleLabel.Visible = false;
            levelLabel.Visible = true;
            levelLabel.Text = $"{level}";

            gameTimer.Enabled = true;
            gameState = "running";
            jumpCounter = 0;
            inAir = false;

            platform.Clear();

            //platforms in order that they need to be jumped, except level-specific platforms (the ones that aren't in the first playthrough) are at the end
            platform.Add(new Rectangle(0, 360, 180, 20));
            platform.Add(new Rectangle(260, 360, 180, 20));
            platform.Add(new Rectangle(500, 300, 150, 20));
            platform.Add(new Rectangle(340, 230, 80, 20));

            switch (level)
            {
                case 2:
                    platform.Add(new Rectangle(520, 150, 240, 20)); //platform that the door is on
                    platform.Add(new Rectangle(0, 180, 280, 20)); //invisble platform 1
                    platform.Add(new Rectangle(330, 115, 120, 20)); //invisible platform 2
                    break;
                default:
                    platform.Add(new Rectangle(500, 170, 260, 20));
                    break;
            }

            if (level == 3)
            {
                player.Location = new Point(620, 150);
                doorRectangle.Location = new Point(35, 330);
                doorEllipse.Location = new Point(35, 315);
            }
            else
            {
                player.Location = new Point(40, 340);
                doorRectangle.Location = new Point(615, platform[4].Y - 30);
                doorEllipse.Location = new Point(615, platform[4].Y - 45);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aDown = true;
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
                    else if (gameState == "over, complete")
                    {
                        SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);
                        confirmBeep.Play();
                        gameState = "waiting";
                        Refresh();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over, died" || gameState == "over, won" || gameState == "over, complete")
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
                case Keys.A:
                    aDown = false;
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

                SoundPlayer jumpBoing = new SoundPlayer(Properties.Resources.jumpBoing);
                jumpBoing.Play();
            }
            else if (inAir == true)
            {
                player.Y += jumpYSpeed[jumpCounter];
                jumpCounter++;

                //check if the player landed on a platform
                for (int i = 0; i < platform.Count; i++)
                {
                    if (player.X + player.Width > platform[i].X && player.X < platform[i].X + platform[i].Width && player.Y <= platform[i].Y && player.Y + jumpYSpeed[jumpCounter] >= platform[i].Y)
                    {
                        player.Y = platform[i].Y - player.Height;
                        inAir = false;
                        SoundPlayer blip = new SoundPlayer(Properties.Resources.blip);
                        blip.Play();
                    }
                }

                if (player.Y > this.Height)
                {
                    gameState = "over, died";
                    gameTimer.Enabled = false;
                    SoundPlayer fall = new SoundPlayer(Properties.Resources.fall);
                    fall.Play();
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


            /* list of ideas:
             * 
             * level 2: make the one platform a bit too far away to reach, add extra invisible rectangles (add score rectangle)
             * 
             * level 3: make the player start where the door normally is, and make the door stay where the player normally is
             * 
             * level 4: make the door teleport around
             * 
             * level 5: make the platforms fall once the player hops off of them
             * 
             * level 6: make all the platforms invisible
             * 
             * level 7: make the platforms move away from the player
             * 
             * level 8: don't paint the door, make the player move to another screen
             */


            if (level == 4)
            {
                Rectangle futurePlayer = new Rectangle(player.X, player.Y, player.Width, player.Height);

                //move the rectangle to the player's future position
                if (aDown == true && futurePlayer.X > 0)
                {
                    futurePlayer.X -= playerXSpeed;
                }

                if (dDown == true && futurePlayer.X <= this.Width - futurePlayer.Width)
                {
                    futurePlayer.X += playerXSpeed;
                }

                if (inAir == true)
                {
                    futurePlayer.Y += jumpYSpeed[jumpCounter];

                    for (int i = 0; i < platform.Count; i++)
                    {
                        if (futurePlayer.X + futurePlayer.Width > platform[i].X && futurePlayer.X < platform[i].X + platform[i].Width && futurePlayer.Y <= platform[i].Y && futurePlayer.Y + jumpYSpeed[jumpCounter] >= platform[i].Y)
                        {
                            futurePlayer.Y = platform[i].Y - futurePlayer.Height;
                        }
                    }
                }

                //check if, during the next tick, the player will get to the door
                if (futurePlayer.IntersectsWith(doorRectangle) || futurePlayer.IntersectsWith(doorEllipse))
                {
                    if (doorRectangle.X == 615)
                    {
                        doorRectangle.Location = new Point(35, 330);
                        doorEllipse.Location = new Point(35, 315);
                    }
                    else if (doorRectangle.X == 35)
                    {
                        doorRectangle.Location = new Point(340, platform[1].Y - 30);
                        doorEllipse.Location = new Point(340, platform[1].Y - 45);
                    }
                }
            }

            //check if player reached end door
            if (player.IntersectsWith(doorRectangle) || player.IntersectsWith(doorEllipse))
            {
                gameState = "over, won";
                gameTimer.Enabled = false;
                level++;
                SoundPlayer winBeep = new SoundPlayer(Properties.Resources.winBeep);
                winBeep.Play();

                if (level == 5)
                {
                    gameState = "over, complete";
                }
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
                if (level == 2)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        e.Graphics.FillRectangle(whiteBrush, platform[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < platform.Count; i++)
                    {
                        e.Graphics.FillRectangle(whiteBrush, platform[i]);
                    }
                }

                //draw end door
                e.Graphics.FillRectangle(whiteBrush, doorRectangle);
                e.Graphics.FillEllipse(whiteBrush, doorEllipse);
            }
            else if (gameState == "over, died")
            {
                titleLabel.Text = "YOU DIED";
                subtitleLabel.Text = "Press Enter to Try Again or Esc to Exit";

                levelLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
            else if (gameState == "over, won")
            {
                titleLabel.Text = "YOU WON!!!";
                subtitleLabel.Text = "Press Enter to Play Again or Esc to Exit";

                levelLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
            else if (gameState == "over, complete")
            {
                titleLabel.Text = "GAME COMPLETE";
                subtitleLabel.Text = "You've completed all the levels!\nPress Enter to Play Again or Esc to Exit";

                levelLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
        }
    }
}
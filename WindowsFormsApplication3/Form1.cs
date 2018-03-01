using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Connect Four";
            this.BackColor = Color.Yellow;
            this.Width = 600;
            this.Height = 600;

            int rowPos = 70;

            for (int row = 0; row < NUM_OF_ROWS; row++)
            {
                int colPos = 20;
                for (int col = 0; col < NUM_OF_COLS; col++)
                {
                    int tileNumber = col + (row * NUM_OF_COLS);

                    this.boardButtons[tileNumber] = new Button();
                    this.boardButtons[tileNumber].Name = "R" + row + "C" + col;
                    this.boardButtons[tileNumber].Location = new System.Drawing.Point(colPos, rowPos);
                    this.boardButtons[tileNumber].Font = new Font(boardButtons[tileNumber].Font.FontFamily, 16); // number is font size
                    this.boardButtons[tileNumber].Size = new System.Drawing.Size(50, 50);
                    this.boardButtons[tileNumber].TabIndex = tileNumber;
#if DEBUG
                    this.boardButtons[tileNumber].Text = Convert.ToString(tileNumber); //Shows numbers of each tile, not necessary for retail version
#endif    
                    this.boardButtons[tileNumber].UseVisualStyleBackColor = true;
                    this.boardButtons[tileNumber].BackColor = Color.LightGray;
                    this.boardButtons[tileNumber].Visible = true;
                    boardButtons[tileNumber].Click += (sender1, ex) => this.buttonHasBeenPressed(tileNumber);
                    this.Controls.Add(boardButtons[tileNumber]);

                    colPos = colPos + 55;
                }
                rowPos = rowPos + 55;
            }

            //Pre colored tiles for testing AI:
            //this.boardButtons[38].BackColor = Color.Green;
            //this.boardButtons[46].BackColor = Color.Red;
            //this.boardButtons[45].BackColor = Color.Red;
            //this.boardButtons[53].BackColor = Color.Red;
            ////this.boardButtons[29].BackColor = Color.Red;
            //this.boardButtons[55].BackColor = Color.Red;
            //this.boardButtons[62].BackColor = Color.Red;
            //this.boardButtons[52].BackColor = Color.Red;
            //this.boardButtons[60].BackColor = Color.Red;
            //this.boardButtons[59].BackColor = Color.Red;
            //this.boardButtons[8].BackColor = Color.Red;
            //this.boardButtons[24].BackColor = Color.Red;
            //this.boardButtons[40].BackColor = Color.Red;
            //this.boardButtons[34].BackColor = Color.Red;
            //this.boardButtons[35].BackColor = Color.Red;
            //this.boardButtons[36].BackColor = Color.Red;
            //this.boardButtons[51].BackColor = Color.Red;

            //this.boardButtons[61].BackColor = Color.Green;
            //this.boardButtons[54].BackColor = Color.Green;
            //this.boardButtons[44].BackColor = Color.Green;
            //this.boardButtons[37].BackColor = Color.Green;
            //this.boardButtons[38].BackColor = Color.Green;
            //this.boardButtons[61].BackColor = Color.Green;
            //this.boardButtons[0].BackColor = Color.Green;
            //this.boardButtons[16].BackColor = Color.Green;
            //this.boardButtons[32].BackColor = Color.Green;
            //this.boardButtons[48].BackColor = Color.Green;
            //this.boardButtons[56].BackColor = Color.Green;
            //this.boardButtons[57].BackColor = Color.Green;
            //this.boardButtons[58].BackColor = Color.Green;
            //this.boardButtons[50].BackColor = Color.Green;
            //this.boardButtons[42].BackColor = Color.Green;
            //this.boardButtons[43].BackColor = Color.Green;
            //this.boardButtons[44].BackColor = Color.Green;
            //this.boardButtons[26].BackColor = Color.Green;
            //this.boardButtons[27].BackColor = Color.Green;

            redWins.Name = "btnRedWins";
            redWins.Location = new System.Drawing.Point(150, 0);
            redWins.Font = new Font(redWins.Font.FontFamily, 15); // number is font size
            redWins.Size = new System.Drawing.Size(50, 50);
            redWins.TabIndex = numberPiecesLeft;                // that is one more than the lastTabIndex
            redWins.Text = "0";
            redWins.UseVisualStyleBackColor = true;
            redWins.BackColor = Color.Red;
            redWins.Visible = true;
            this.Controls.Add(redWins);

            greenWins.Name = "greenWins";
            greenWins.Location = new System.Drawing.Point(250, 0);
            greenWins.Font = new Font(redWins.Font.FontFamily, 15); // number is font size
            greenWins.Size = new System.Drawing.Size(50, 50);
            greenWins.TabIndex = numberPiecesLeft + 1;                // that is one more than the lastTabIndex
            greenWins.Text = "0";
            greenWins.UseVisualStyleBackColor = true;
            greenWins.BackColor = Color.Green;
            greenWins.Visible = true;
            this.Controls.Add(greenWins);
        }

        int greenWinsCount = 0;
        int redWinsCount = 0;

        Button redWins = new Button();
        Button greenWins = new Button();

        int numberPiecesLeft = NUM_OF_COLS * NUM_OF_ROWS;
        const int NUM_OF_COLS = 7, NUM_OF_ROWS = 6;

        Button[] boardButtons = new Button[NUM_OF_COLS * NUM_OF_ROWS]; // NUM_OF_COLS cols and 6 rows

        int LastMove = -1;           //global - auto asignment
        int LastSecondMove = -1;     //local - manuel asignment
        int LastThirdMove = -1;      //local - manuel asignment
        int LastForthMove = -1;      //local - manuel asignment
        int userLastMove = -1;       //global - auto asignment - its not tile number, its column number

        LinkedList<int> badMove = new LinkedList<int>();
        LinkedList<int> notGoodMove = new LinkedList<int>();

        private void newGame()
        {
            numberPiecesLeft = NUM_OF_COLS * NUM_OF_ROWS;
            for (int i = 0; i < numberPiecesLeft; i++)
            {
                boardButtons[i].BackColor = Color.LightGray;
                boardButtons[i].Text = "";
            }
            badMove.Clear();
            notGoodMove.Clear();
            redWins.Text = Convert.ToString(redWinsCount);
            greenWins.Text = Convert.ToString(greenWinsCount);

        }

        private void buttonHasBeenPressed(int tileNumberToUse)
        {
            if (placePieceRed(tileNumberToUse))     // if piece can be placed, place it
            {
                numberPiecesLeft = numberPiecesLeft - 1;
                if (numberPiecesLeft > 0 && DoesRedWin() == 0)  // this deals with computer's move
                {
                    placePieceGreen(AI());

                    if (DoesGreenWin() >= 1)
                    {
                        greenWinsCount++;
                        MessageBox.Show("The computer wins!!!");
                        newGame();
                    }
                }
                else if (DoesRedWin() >= 1)
                {
                    redWinsCount++;
                    MessageBox.Show("You're the winner!!!");
                    newGame();
                }
            }
            if (numberPiecesLeft == 0 && DoesGreenWin() == 0 && DoesRedWin() == 0)
            {
                MessageBox.Show("That game is a draw!!!");
                newGame();
            }
        }

        private bool placePieceGreen(int tileNumber)
        {
            bool placed = false;    // turns to true if a free lcation is found

            if (tileNumber >= 0 && tileNumber <= NUM_OF_COLS)
            {
                if (boardButtons[tileNumber].BackColor != Color.Red && boardButtons[tileNumber].BackColor != Color.Green)
                {
                    boardButtons[tileNumber].BackColor = Color.Green;
                    placed = true;
                }
                LastMove = tileNumber;
                tileNumber = tileNumber + NUM_OF_COLS;
                while (tileNumber < NUM_OF_COLS * NUM_OF_ROWS && boardButtons[tileNumber].BackColor != Color.Red && boardButtons[tileNumber].BackColor != Color.Green)
                {
                    boardButtons[tileNumber - NUM_OF_COLS].BackColor = Color.LightGray;
                    boardButtons[tileNumber].BackColor = Color.Green;
                    LastMove = tileNumber;
                    tileNumber = tileNumber + NUM_OF_COLS;
                }
            }
            return placed;
        }

        private bool placePieceRed(int tileNumber)
        {
            bool placed = false;    // turns to true if a free lcation is found

            if (tileNumber >= 0 && tileNumber <= NUM_OF_COLS * NUM_OF_ROWS)
            { 
                if (boardButtons[tileNumber].BackColor != Color.Red && boardButtons[tileNumber].BackColor != Color.Green)
                {
                    boardButtons[tileNumber].BackColor = Color.Red;
                    placed = true;
                }
                LastMove = tileNumber;
                tileNumber = tileNumber + NUM_OF_COLS;
                while (tileNumber < NUM_OF_COLS * NUM_OF_ROWS && boardButtons[tileNumber].BackColor != Color.Red && boardButtons[tileNumber].BackColor != Color.Green)
                {
                    boardButtons[tileNumber - NUM_OF_COLS].BackColor = Color.LightGray;
                    boardButtons[tileNumber].BackColor = Color.Red;
                    LastMove = tileNumber;
                    tileNumber = tileNumber + NUM_OF_COLS;
                }
            }
            return placed;
        }

        private int AI()
        {
            userLastMove = LastMove % NUM_OF_COLS;
            badMove.Clear();
            notGoodMove.Clear();
            numberPiecesLeft--;
            int returnValue = -1;

            returnValue = doesGreenWinIn1Move();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesRedWinIn1Move();
            if (returnValue != -1)
                return returnValue;

            doesRedWinAfterMoveOfGreen();
            doesGreenWinAfterMoveOfRed();
            doesRedWinAs2WaysAfterMoveOfGreenAnd3MovesOfItself();

            returnValue = doesRedWinIn2MovesAsUnpreventable();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesGreenWinAs2WaysAfter2MovesOfItself();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesRedWinsAs2WaysAfter2MovesOfItself();
            if (returnValue != -1)
                return returnValue;

            returnValue = lookForWinningPatternForRedUpto7ForwardMoves();
            if (returnValue != -1)
                return returnValue;

            returnValue = lookForWinningPatternForGreenUpto7ForwardMoves();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesGreenWinAfterMoveOfItself();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesRedWinAfterMoveOfItself();
            if (returnValue != -1)
                return returnValue;

            returnValue = doesGreenWinAfter2MovesOfItself();
            if (returnValue != -1)
                return returnValue;

            returnValue = betterThanRandom();
            if (returnValue != -1)
                return returnValue;

            returnValue = randomNotBadNotGood();
            if (returnValue != -1)
                return returnValue;

            returnValue = haveToDoBadMove();
            if (returnValue != -1)
                return returnValue;

            return -1;    // To understand if AI is working perfect and returns a value without throwing exception
        }

        private int DoesRedWin()
        {
            int ret = 0;

            if (winCheckColumnRed())
                ret++;

            if (winCheckRowRed())
                ret++;

            if (winCheckleftToRightDiagonalRed())
                ret++;

            if (winCheckrightToLeftDiagonalRed())
                ret++;

            return ret;
        }

        private bool winCheckColumnRed()
        {
            for (int row = 0; row < NUM_OF_ROWS - 3; row++)   // checking columns for four
            {
                for (int col = 0; col < NUM_OF_COLS; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Red)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckRowRed()
        {
            for (int row = 0; row < NUM_OF_ROWS; row++)     // checking rows for four
            {
                for (int col = 0; col < NUM_OF_COLS - 3; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Red)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckleftToRightDiagonalRed()
        {
            for (int row = 0; row < NUM_OF_ROWS - 3; row++)   // checking left to right diagnals for fours
            {
                for (int col = 0; col < NUM_OF_COLS - 3; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Red)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + 1 + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 2 + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 3 + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckrightToLeftDiagonalRed()
        {
            for (int row = 0; row<NUM_OF_ROWS - 3; row++)   // checking right to left diagnals right for fours
            {
                for (int col = 3; col<NUM_OF_COLS; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Red)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber - 1 + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber - 2 + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber - 3 + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
}
                }
            }
            return false;
        }
        private int DoesGreenWin()
        {
            int ret = 0;

            if (winCheckColumnGreen())
                ret++;

            if (winCheckRowGreen())
                ret++;

            if (winCheckleftToRightDiagonalGreen())
                ret++;

            if (winCheckrightToLeftDiagonalGreen())
                ret++;

            return ret;
        }

        private bool winCheckColumnGreen()
        {
            for (int row = 0; row < NUM_OF_ROWS - 3; row++)   // checking columns for four
            {
                for (int col = 0; col < NUM_OF_COLS; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Green)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckRowGreen()
        {
            for (int row = 0; row < NUM_OF_ROWS; row++)     // checking rows for four
            {
                for (int col = 0; col < NUM_OF_COLS - 3; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Green)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckleftToRightDiagonalGreen()
        {
            for (int row = 0; row < NUM_OF_ROWS - 3; row++)   // checking left to right diagnals for fours
            {
                for (int col = 0; col < NUM_OF_COLS - 3; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Green)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber + 1 + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 2 + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber + 3 + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool winCheckrightToLeftDiagonalGreen()
        {
            for (int row = 0; row < NUM_OF_ROWS - 3; row++)   // checking right to left diagnals right for fours
            {
                for (int col = 3; col < NUM_OF_COLS; col++)
                {
                    int tNumber = col + (row * NUM_OF_COLS);

                    if (boardButtons[tNumber].BackColor == Color.Green)
                    {
                        if (boardButtons[tNumber].BackColor == boardButtons[tNumber - 1 + NUM_OF_COLS * 1].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber - 2 + NUM_OF_COLS * 2].BackColor &&
                            boardButtons[tNumber].BackColor == boardButtons[tNumber - 3 + NUM_OF_COLS * 3].BackColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int doesGreenWinIn1Move()                         //checks if green wins in one move 
        {
            for (int i = 0; i < NUM_OF_COLS; i++)    
            {
                if (placePieceGreen(i))
                {                                                             //puts a green box when loop is executed each time to all available choices                                       
                    if (DoesGreenWin() >= 1)
                    {
                        boardButtons[LastMove].BackColor = Color.LightGray;
                        return i;
                    }                                                       //If there is no winner, first takes last move back then, returns i for next move
                    boardButtons[LastMove].BackColor = Color.LightGray;     //If there is no winner, takes last move back
                }
            }
            return -1;
        }

        private int doesRedWinIn1Move()                           //checks if red wins in one move
        {
            for (int i = 0; i < NUM_OF_COLS; i++)            
            {
                if (placePieceRed(i))                                      //puts a red box when loop is executed each time to all available choices
                {
                    if (DoesRedWin() >= 1)
                    {
                        boardButtons[LastMove].BackColor = Color.LightGray;
                        return i;
                    }                                                        //If there is no winner, first takes last move back then, returns i for next move
                    boardButtons[LastMove].BackColor = Color.LightGray;      //If there is no winner, takes last move back
                }
            }
            return -1;
        }

        private void doesRedWinAfterMoveOfGreen()                //2 moves forward - checks if red wins after one move of green
        {
            for (int i = 0; i < NUM_OF_COLS; i++)                
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    if (placePieceGreen(i))
                    {
                        LastSecondMove = LastMove;
                        if (placePieceRed(j))
                        {
                            if (DoesRedWin() >= 1)
                            {
                                badMove.AddLast(i);
                            }
                            boardButtons[LastMove].BackColor = Color.LightGray;
                        }
                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                    }
                }
            }
        }

        private int doesRedWinIn2MovesAsUnpreventable()                           //checks if red wins in two moves
        {
            for (int i = 0; i < NUM_OF_COLS-3; i++)
            {
                if (placePieceRed(i))                                      //puts a red box when loop is executed each time to all available choices
                {
                    LastSecondMove = LastMove;
                    if (placePieceRed(i+3))                                      //puts a red box when loop is executed each time to all available choices
                    {
                        if (DoesRedWin() >= 1 && !badMove.Contains(i))
                        {
                            boardButtons[LastMove].BackColor = Color.LightGray;
                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                            return i;
                        }                                                   //If there is no winner, first takes last move back then, returns i for next move
                        if (DoesRedWin() >= 1 && !badMove.Contains(i+3))
                        {
                            boardButtons[LastMove].BackColor = Color.LightGray;
                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                            return i+3;
                        }
                        boardButtons[LastMove].BackColor = Color.LightGray;      //If there is no winner, takes last move back
                    }
                    boardButtons[LastSecondMove].BackColor = Color.LightGray;
                }
            }
            return -1;
        }

        private void doesGreenWinAfterMoveOfRed()                // 2 moves forward - checks if green wins after one move of red
        {
            for (int i = 0; i < NUM_OF_COLS; i++)         
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    if (placePieceRed(i))
                    {
                        LastSecondMove = LastMove;
                        if (placePieceGreen(j))
                        {
                            if (DoesGreenWin() >= 1)
                            {
                                notGoodMove.AddLast(i);
                            }
                            boardButtons[LastMove].BackColor = Color.LightGray;
                        }
                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                    }
                }
            }
        }

        private void doesRedWinAs2WaysAfterMoveOfGreenAnd3MovesOfItself()  // !! 4 moves forward - checks if red wins as 2 ways
        {
            for (int i = 0; i < NUM_OF_COLS; i++)
            {             
                for (int k = 0; k < NUM_OF_COLS; k++)
                {
                    for (int l = 0; l < NUM_OF_COLS; l++)
                    {
                        if (!badMove.Contains(i) && !notGoodMove.Contains(i))
                        {
                            if (placePieceGreen(i))
                            {
                                LastForthMove = LastMove;
                                if (placePieceRed(i))
                                {
                                    LastThirdMove = LastMove;
                                    if (placePieceRed(k))
                                    {
                                        LastSecondMove = LastMove;
                                        if (placePieceRed(l))
                                        {
                                            if (DoesRedWin() >= 2)
                                            {
                                                boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                                if(DoesRedWin() >= 2)
                                                {
                                                    goto end;
                                                }
                                                boardButtons[LastMove].BackColor = Color.LightGray;
                                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                                boardButtons[LastForthMove].BackColor = Color.LightGray;
                                                notGoodMove.AddLast(i);
                                            }
                                            end:
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                        }
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                    }
                                    boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                }
                                boardButtons[LastForthMove].BackColor = Color.LightGray;
                            }
                        }
                    }                    
                }
            }
        }

        private int doesRedWinsAs2WaysAfter2MovesOfItself()     // 3 moves forward - checks if red wins as 2 ways after  its 2 moves 
        {
            for (int i = 0; i < NUM_OF_COLS; i++)
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    for (int k = 0; k < NUM_OF_COLS; k++)
                    {
                        if (placePieceRed(i))
                        {
                            LastThirdMove = LastMove;
                            if (placePieceRed(j))
                            {
                                LastSecondMove = LastMove;
                                if (placePieceRed(k))
                                {
                                    if (DoesRedWin() >= 2 && !badMove.Contains(i) && !notGoodMove.Contains(i) && i == j && i == k)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        return i;
                                    }
                                    if (DoesRedWin() >= 2 && i != j && j != k && i != k)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        Console.WriteLine(i + " " + " " + j + " " + k);
                                        if (((i < j && i > k) || (i < k && i > j)) && !badMove.Contains(i) && !notGoodMove.Contains(i))
                                            return i;
                                        if (((k < j && k > i) || (k > j && k < i)) && !badMove.Contains(k) && !notGoodMove.Contains(k))
                                            return k;
                                        if (((j > i && j < k) || (j < i && j > k)) && !badMove.Contains(j) && !notGoodMove.Contains(j))
                                            return j;
                                    }
                                    if (DoesRedWin() >= 2)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceGreen(i);
                                        LastThirdMove = LastMove;
                                        placePieceRed(j);
                                        LastSecondMove = LastMove;
                                        placePieceRed(k);
                                        if (DoesRedWin() <= 0 && !badMove.Contains(i) && !notGoodMove.Contains(i))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return i;
                                        }
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceGreen(j);
                                        LastThirdMove = LastMove;
                                        placePieceRed(i);
                                        LastSecondMove = LastMove;
                                        placePieceRed(k);
                                        if (DoesRedWin() <= 0 && !badMove.Contains(j) && !notGoodMove.Contains(j))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return j;
                                        }
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceGreen(k);
                                        LastThirdMove = LastMove;
                                        placePieceRed(j);
                                        LastSecondMove = LastMove;
                                        placePieceRed(i);
                                        if (DoesRedWin() <= 0 && !badMove.Contains(k) && !notGoodMove.Contains(k))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return k;
                                        }
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                    }                         
                                    boardButtons[LastMove].BackColor = Color.LightGray;
                                }
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                            }
                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                        }
                    }
                }
            }
            return -1;
        }

        private int lookForWinningPatternForRedUpto7ForwardMoves()
        {
            bool isColumnFull = false;
            List<int> columnMoves = new List<int>();

            for (int j = userLastMove - 3; j < userLastMove + 3; j++)
            {
                if (userLastMove != j)
                {
                    while (!isColumnFull)
                    {
                        if (placePieceRed(j))
                        {
                            columnMoves.Add(LastMove);
                        }
                        else
                        {
                            isColumnFull = true;
                        }
                    }
                    if (winCheckRowRed() || winCheckleftToRightDiagonalRed() || winCheckrightToLeftDiagonalRed())   // If triple blocks already exist, no need this
                    {
                        for (int k = 0; k < columnMoves.Count; k++)
                        {
                            boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                        }
                        columnMoves.Clear();
                        continue;
                    }
                    else
                    {
                        if (!badMove.Contains(userLastMove))
                        {
                            if (placePieceRed(userLastMove))
                            {
                                if (winCheckRowRed() || winCheckleftToRightDiagonalRed() || winCheckrightToLeftDiagonalRed())
                                {
                                    boardButtons[LastMove].BackColor = Color.LightGray;
                                    for (int k = 0; k < columnMoves.Count; k++)
                                    {
                                        boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                                    }
                                    columnMoves.Clear();
                                    return userLastMove;
                                }
                                boardButtons[LastMove].BackColor = Color.LightGray;
                            }
                        }
                    }
                    isColumnFull = false;
                    for (int k = 0; k < columnMoves.Count; k++)
                    {
                        boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                    }
                    columnMoves.Clear();
                }
            }
            return -1;
        }

        private int lookForWinningPatternForGreenUpto7ForwardMoves()
        {
            bool isColumnFull = false;
            List<int> columnMoves = new List<int>();

            for (int j = userLastMove - 3; j < userLastMove + 3; j++)
            {
                if (userLastMove != j)
                {
                    while (!isColumnFull)
                    {
                        if (placePieceGreen(j))
                        {
                            columnMoves.Add(LastMove);
                        }
                        else
                        {
                            isColumnFull = true;
                        }
                    }
                    if (winCheckRowGreen() || winCheckleftToRightDiagonalGreen() || winCheckrightToLeftDiagonalGreen())   // If triple blocks already exist, no need this
                    {
                        for (int k = 0; k < columnMoves.Count; k++)
                        {
                            boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                        }
                        columnMoves.Clear();
                        continue;
                    }
                    else
                    {
                        if (!badMove.Contains(userLastMove))
                        {
                            if (placePieceGreen(userLastMove))
                            {
                                if (winCheckRowGreen() || winCheckleftToRightDiagonalGreen() || winCheckrightToLeftDiagonalGreen())
                                {
                                    boardButtons[LastMove].BackColor = Color.LightGray;
                                    for (int k = 0; k < columnMoves.Count; k++)
                                    {
                                        boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                                    }
                                    columnMoves.Clear();
                                    return userLastMove;
                                }
                                boardButtons[LastMove].BackColor = Color.LightGray;
                            }
                        }
                    }   
                    isColumnFull = false;
                    for (int k = 0; k < columnMoves.Count; k++)
                    {
                        boardButtons[columnMoves[k]].BackColor = Color.LightGray;
                    }
                    columnMoves.Clear();
                }
            }
            return -1;
        }

        private int doesGreenWinAs2WaysAfter2MovesOfItself()     // 3 moves forward - checks if green wins as 2 ways after its 2 moves 
        {
            for (int i = 0; i < NUM_OF_COLS; i++)        
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    for (int k = 0; k < NUM_OF_COLS; k++)
                    {
                        if (placePieceGreen(i))
                        {
                            LastThirdMove = LastMove;
                            if (placePieceGreen(j))
                            {
                                LastSecondMove = LastMove;
                                if (placePieceGreen(k))
                                {
                                    if (DoesGreenWin() >= 2 && !badMove.Contains(i) && !notGoodMove.Contains(i) && i == j && i == k)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        return i;
                                    }
                                    if (DoesGreenWin() >= 2 && i != j && j != k && i != k)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        Console.WriteLine(i + " " + " " + j + " " + k);
                                        if (((i < j && i > k) || (i < k && i > j)) && !badMove.Contains(i) && !notGoodMove.Contains(i))
                                            return i;
                                        if (((k < j && k > i) || (k > j && k < i)) && !badMove.Contains(k) && !notGoodMove.Contains(k))
                                            return k;
                                        if (((j > i && j < k) || (j < i && j > k)) && !badMove.Contains(j) && !notGoodMove.Contains(j))
                                            return j;
                                    }
                                    if (DoesGreenWin() >= 2)
                                    {
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceRed(i);
                                        LastThirdMove = LastMove;
                                        placePieceGreen(j);
                                        LastSecondMove = LastMove;
                                        placePieceGreen(k);
                                        if (DoesGreenWin() <= 0 && !badMove.Contains(i) && !notGoodMove.Contains(i))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return i;
                                        }
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceRed(j);
                                        LastThirdMove = LastMove;
                                        placePieceGreen(i);
                                        LastSecondMove = LastMove;
                                        placePieceGreen(k);
                                        if (DoesGreenWin() <= 0 && !badMove.Contains(j) && !notGoodMove.Contains(j))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return j;
                                        }
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastMove].BackColor = Color.LightGray;

                                        placePieceRed(k);
                                        LastThirdMove = LastMove;
                                        placePieceGreen(j);
                                        LastSecondMove = LastMove;
                                        placePieceGreen(i);
                                        if (DoesGreenWin() <= 0 &&!badMove.Contains(k) && !notGoodMove.Contains(k))
                                        {
                                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                            boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                            boardButtons[LastMove].BackColor = Color.LightGray;
                                            return k;
                                        }
                                    }
                                    boardButtons[LastMove].BackColor = Color.LightGray;
                                }
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                            }
                            boardButtons[LastThirdMove].BackColor = Color.LightGray;
                        }
                    }
                }
            }
            return -1;
        }

        private int doesRedWinAfterMoveOfItself()               //checks if red wins after one move of itself
        {
            for (int i = 0; i < NUM_OF_COLS; i++)            
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    if (placePieceRed(i))
                    {
                        LastSecondMove = LastMove;
                        if (placePieceRed(j))
                        {
                            if (DoesRedWin()>=1 && !badMove.Contains(i) && !notGoodMove.Contains(i) && (i== userLastMove - 1 || i == userLastMove + 1 || i == userLastMove))
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return i;
                            }
                            if (DoesRedWin() >= 1 && !badMove.Contains(j) && !notGoodMove.Contains(j) && (j == userLastMove - 1 || j == userLastMove + 1 || j == userLastMove))
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return j;
                            }
                            boardButtons[LastMove].BackColor = Color.LightGray;
                        }
                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                    }
                }
            }
            for (int i = 0; i < NUM_OF_COLS; i++)
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    if (placePieceRed(i))
                    {
                        LastSecondMove = LastMove;
                        if (placePieceRed(j))
                        {
                            if (DoesRedWin() >= 1 && !badMove.Contains(i) && !notGoodMove.Contains(i) )
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return i;
                            }
                            if (DoesRedWin() >= 1 && !badMove.Contains(j) && !notGoodMove.Contains(j) )
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return j;
                            }
                            boardButtons[LastMove].BackColor = Color.LightGray;
                        }
                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                    }
                }
            }
            return -1;
        }

        private int doesGreenWinAfterMoveOfItself()              //checks if green wins after one move of itself
        {
            for (int i = 0; i < NUM_OF_COLS; i++)               
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    if (placePieceGreen(i))
                    {
                        LastSecondMove = LastMove;
                        if (placePieceGreen(j))
                        {
                            if (DoesGreenWin() >= 1 && !badMove.Contains(i) && !notGoodMove.Contains(i))
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return i;
                            }
                            if (DoesGreenWin() >= 1 && !badMove.Contains(j) && !notGoodMove.Contains(j))
                            {
                                boardButtons[LastMove].BackColor = Color.LightGray;
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                return j;
                            }
                            boardButtons[LastMove].BackColor = Color.LightGray;
                        }
                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                    }
                }
            }
            return -1;
        }

        private int doesGreenWinAfter2MovesOfItself()
        {
            for (int i = 0; i < NUM_OF_COLS; i++)
            {
                for (int j = 0; j < NUM_OF_COLS; j++)
                {
                    for (int k = 0; k < NUM_OF_COLS; k++)
                    {
                        if (placePieceGreen(i))
                        {
                            LastThirdMove = LastMove;
                            if (placePieceGreen(j))
                            {
                                LastSecondMove = LastMove;
                                if (placePieceGreen(k))
                                {
                                    if (DoesGreenWin() >= 1 && !badMove.Contains(k) && !notGoodMove.Contains(k))
                                    {
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        return k;
                                    }
                                    if (DoesGreenWin() >= 1 && !badMove.Contains(j) && !notGoodMove.Contains(j))
                                    {
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        return j;
                                    }
                                    if (DoesGreenWin() >= 1 && !badMove.Contains(i) && !notGoodMove.Contains(i))
                                    {
                                        boardButtons[LastMove].BackColor = Color.LightGray;
                                        boardButtons[LastSecondMove].BackColor = Color.LightGray;
                                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                                        Console.WriteLine(i + " " + " " + j + " " + k);
                                        return i;
                                    }
                                    boardButtons[LastMove].BackColor = Color.LightGray;
                                }
                                boardButtons[LastSecondMove].BackColor = Color.LightGray;
                            }
                        }
                        boardButtons[LastThirdMove].BackColor = Color.LightGray;
                    }
                }
            }
            return -1;
        }

        private int betterThanRandom()                          //rather then putting random, tries to put near to current choices of red
        {
            for (int i = 0; i < NUM_OF_COLS*NUM_OF_ROWS; i++)            
            {
                if (boardButtons[i].BackColor == Color.Red)
                {
                    if ((placePieceGreen(i % NUM_OF_COLS)))       //puts to top of Red piece
                    {
                        if (!badMove.Contains(i % NUM_OF_COLS) && !notGoodMove.Contains(i % NUM_OF_COLS))
                        {
                            boardButtons[LastMove].BackColor = Color.LightGray;
                            return i % NUM_OF_COLS;
                        }
                        boardButtons[LastMove].BackColor = Color.LightGray;
                    }
                    if (i != NUM_OF_COLS*NUM_OF_ROWS-1 && !badMove.Contains((i + 1) % NUM_OF_COLS) && !notGoodMove.Contains((i+1) % NUM_OF_COLS))      //puts to right side of red piece
                    {
                        if (placePieceGreen((i + 1) % NUM_OF_COLS))
                        {
                            boardButtons[LastMove].BackColor = Color.LightGray;
                            return (i + 1) % NUM_OF_COLS;
                        }
                    }
                    if (i != 0 && !badMove.Contains((i - 1) % NUM_OF_COLS) && !notGoodMove.Contains((i-1) % NUM_OF_COLS))       //puts to left side of red piece
                    {
                        if (placePieceGreen((i - 1) % NUM_OF_COLS))
                        {
                            boardButtons[LastMove].BackColor = Color.LightGray;
                            return (i - 1) % NUM_OF_COLS;
                        }
                    }
                }
            }
            return -1;
        }

        private int randomNotBadNotGood()                       //If cant put near a red, tries to put somewhere else
        {
            for (int i = 0; i < NUM_OF_COLS*NUM_OF_ROWS; i++)        
            {
                if (!badMove.Contains(i % NUM_OF_COLS) && boardButtons[i].BackColor == Color.LightGray)
                {
                    return i % NUM_OF_COLS;
                }
            }
            return -1;
        }

        private int haveToDoBadMove()                           //if there is no sensible move, it will have to do 'bad move'
        {
            for (int i = 0; i < NUM_OF_COLS*NUM_OF_ROWS; i++)            
            {
                if (boardButtons[i].BackColor == Color.LightGray)
                {
                    return i % NUM_OF_COLS;
                }
            }
            return -1;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MegaChallengeCasino
{
    public partial class Default : System.Web.UI.Page
    {

        // Images need to be created randomly so create a random class within the default class, but outside the methods,
        // so that it is accessible within any methods. The same instance of the random class will be used for each call below.
        //The Random class uses an algorithm that APPEARS AS THOUGH it creates a  random number each time, HOWEVER it really uses predictable algorithm
       // that will return the same results every time.To make it appear truly random, it is "seeded" with the current date and time as ticks.
       // However, when we create a new Random class each time and seed it with the SAME date and time ticks each time(by default) it will always 
       // return the SAME value!
        Random random = new Random();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Create an array for the 3 reels calling the spinReels method 3 times to select a random image for each of the image server controls.
                string[] reels = new string[] { spinReel(), spinReel(), spinReel() };

                // Set the imageUrl Property for each image server control. 
                displayImages(reels);

                // Create a view state object "PlayersMoney" to store the initial amount that can be added to or subtracted against as betted against.
                ViewState.Add("PlayersMoney", 100);
                displayPlayersMoney();
            }
        }

        protected void pullButton_Click(object sender, EventArgs e)
        {
            // When someone clicks the button...

            // Request that a bet is entered before proceeding
            if (string.IsNullOrEmpty(betTextBox.Text.Trim()))
            {
                resultLabel.Text = "Please place a bet!";
                return;
            } 

            // Use TryParse to determine if the user has entered a valid number into the bet textbox. If the value cannot be parsed into an integer, 
            // the string is not cast to an int and execution is returned.  
            int bet = 0;
            if (!int.TryParse(betTextBox.Text, out bet)) return ;

            // Pass the user's bet to the pullLever method ...
            int winnings = pullLever(bet);

            // Display the result of the spin
            displayResult(bet, winnings);

            // Adjust the players money after the bet and winnings
            adjustPlayersMoney(bet, winnings);

            // Display the players money after the adjustment
            displayPlayersMoney();

        }

        private int pullLever(int bet)
        {
            // Create an array for the 3 reels calling the spinReels method 3 times to select a random image for each of the image server controls.
            string[] reels = new string[] { spinReel(), spinReel(), spinReel() } ;

            // Set the imageUrl Property for each image server control. 
            displayImages(reels);

            // Evaluate the spin ...
            int multiplier = evaluateSpin(reels);

            // Calcuate the bet by the value returned from method "evaluateSpin" 
            return bet * multiplier;
        }

        private int evaluateSpin(string[] reels)
         {
            // If there is one or more bars, return 0. Passing the reels to isBar method which will equate to true when at least one bar exists.
            if (isBar(reels)) return 0;

            // If there are three 7's, return 100. Passing the reels to isJackpot method which will equate to true when three 7's exist.
            if (isJackpot(reels)) return 100;

            // If there are one or more cherries, return 2,3 or 4 respectively. Passing the reels to isWinner method, with an out parameter to determine
            // what the bet will be multiplied by, which will equate to true when at least one cherry exists. 
            int multiplier = 0;
            if (isWinner(reels, out multiplier)) return multiplier;

            return 0;

        }

        private bool isWinner(string[] reels, out int multiplier)
        {
            // Determine the multiplier for the bet by determining the number of cherries spun.
             multiplier = determineMultiplier(reels);
 
            // The method will return false when no cherries exist and the multiplier is zero, otherwise is will return false with the multiplier. 
            if (multiplier > 0) return true;
            return false;

        }

        private int determineMultiplier(string[] reels)
        {
            // Returns the multiplier for the bet, depending on how many cherries were spun.

            // Determine how many cherries ...
            int cherryCount = determineCherryCount(reels);

            // Then determine the multiplier
            if (cherryCount == 1) return 2;
            if (cherryCount == 2) return 3;
            if (cherryCount == 3) return 4;
            return 0;
        }

        private int determineCherryCount(string[] reels)
        {
            // Return the number of cherries spun. 
            int cherryCount = 0;

            if (reels[0] == "Cherry") cherryCount++;
            if (reels[1] == "Cherry") cherryCount++;
            if (reels[2] == "Cherry") cherryCount++;
            return cherryCount;
        }

        private bool isJackpot(string[] reels)
        {
            // Return true when three Seven's are spun, otherwise returns false
            if (reels[0] == "Seven" & reels[1] == "Seven" & reels[2] == "Seven")
                return true;
            else
                return false;
        }

        private bool isBar(string[] reels)
        {
            // Return true when one or bars are spun, otherwise returns false
            if (reels[0] == "Bar" || reels[1] == "Bar" || reels[2] == "Bar")
                return true;
            else
                return false;
        }

        private string spinReel()
        {
            // Create an array for the images of type string without the .png extension as that will be appended later.
            string[] images = new string[] { "Strawberry", "Bar", "Lemon", "Bell", "Clover", "Cherry", "Diamond", "Orange", "Seven", "HorseShoe", "Plum", "Watermelon" };

            // Get a random image string from the array of images, passing the maximum index value, which will be one less than the number of images as arrays are zero based.
           return images[random.Next(11)];
        }

        private void adjustPlayersMoney(int bet, int winnings)
        {
            // Place the current value in viewstate into an int variable
            // ViewState is an object so has to be cast as a string before it can be parsed as an integer
            int playersMoney = int.Parse(ViewState["PlayersMoney"].ToString());

            // Subtract the bet
            playersMoney -= bet;

            // Add the winnings
            playersMoney += winnings;

            // Place the adjusted amount into the veiwstate object. No need to cast when assigning a value to a viewstate object.
            ViewState["PlayersMoney"] = playersMoney;
         }

        private void displayImages(string[] reels)
        {
            // Set the image Url Property for images 1,2 and 3 by contacenating the images folder with the name of the random image plus the .png extension.
            Image1.ImageUrl = "/Images/" + reels[0] + ".png";
            Image2.ImageUrl = "/Images/" + reels[1] + ".png";
            Image3.ImageUrl = "/Images/" + reels[2] + ".png";
        }

        private void displayResult(int bet, int winnings)
        {
            if (winnings > 0)
                resultLabel.Text = String.Format("You bet {0:C} and won {1:C}!", bet, winnings);
            else
                resultLabel.Text = String.Format("Sorry, you lost  {0:C}. Better luck next time!", bet);
        }

        private void displayPlayersMoney()
        {
            moneyLabel.Text = string.Format("Player's Money: {0:C}", ViewState["PlayersMoney"]);
        }





    }
}
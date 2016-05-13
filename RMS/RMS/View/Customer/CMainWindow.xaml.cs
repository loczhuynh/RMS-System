using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Data;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using RMS.Server.BL;
using RMS.UI.Model;
using RMS.UI.View.Utility;
using RMS.UI.View.Employee;
using RMS.UI.Model;

// NEEDS: Twitter Client only works once and for one user
//        Game doesn't get comp coupon from database
namespace RMS.UI.View
{
    public partial class CMainWindow : Window
    {
        // Twitter Variables
        TwitterCredentials appCredentials;
        ITwitterCredentials userCredentials;
        string authorizationUrl;
        string pinCode;
        string userTweet;
        string couponCode;
        // Sidebar variables
        int _currentOrderId = -1;
        // Game variables
        Random RNG = new Random();

        public CMainWindow()
        {
            InitializeComponent();
            DataContext = this;
            appCredentials = new TwitterCredentials("SjR6gQCjwCk7yJbnXcNnqpMDV", "8ZVu4zHul08obms4FISnDEyX1Lz3hUFzdOLKlAxm1IpDV9P2Jb");
        }

        #region Side Bar
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (loginButton.Content.ToString() == "Login")
            {
                phoneBox.Visibility = Visibility.Hidden;
                phoneBox.IsEnabled = false;
                passwordBox.Visibility = Visibility.Hidden;
                passwordBox.IsEnabled = false;

                usernameLabel.Visibility = Visibility.Visible;
                usernameLabel.IsEnabled = true;

                loginButton.Content = "Logout";
            }
            else
            {
                usernameLabel.Visibility = Visibility.Hidden;
                usernameLabel.IsEnabled = false;

                phoneBox.Visibility = Visibility.Visible;
                phoneBox.IsEnabled = true;
                passwordBox.Visibility = Visibility.Visible;
                passwordBox.IsEnabled = true;

                loginButton.Content = "Login";
            }
        }      
 
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridCustomerOrder.SelectedItem == null)
                    return;

                CustomerItemOrder m = (CustomerItemOrder)gridCustomerOrder.SelectedItem;
                _lstCustomerItemOrder.Remove(m);
                RefreshCustomerOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Games
        #region Game Button Events
        private void GameSelect_Click(object sender, RoutedEventArgs e)
        {
            Button switchButton = sender as Button;

            // Hide all elements
            BejeweledButton.Visibility = Visibility.Hidden;
            BejeweledButton.IsEnabled = false;
            BloonsTDButton.Visibility = Visibility.Hidden;
            BloonsTDButton.IsEnabled = false;
            BloonsButton.Visibility = Visibility.Hidden;
            BloonsButton.IsEnabled = false;
            LinesButton.Visibility = Visibility.Hidden;
            LinesButton.IsEnabled = false;
            TicTacToeButton.Visibility = Visibility.Hidden;
            TicTacToeButton.IsEnabled = false;

            // Bring up the game elements
            if (switchButton.Name == "BejeweledButton")
            {
                WebGameBrowser.IsEnabled = true;
                WebGameBrowser.Visibility = Visibility.Visible;
                WebGameBrowser.Navigate("http://chat.kongregate.com/gamez/0020/0492/live/flappy-bird-7083.swf?kongregate_game_version=1392995857");
            }
            else if (switchButton.Name == "BloonsTDButton")
            {
                WebGameBrowser.IsEnabled = true;
                WebGameBrowser.Visibility = Visibility.Visible;
                WebGameBrowser.Navigate("http://chat.kongregate.com/gamez/0015/1730/live/Preloader.swf?kongregate_game_version=1444878243");
            }
            else if (switchButton.Name == "BloonsButton")
            {
                WebGameBrowser.IsEnabled = true;
                WebGameBrowser.Visibility = Visibility.Visible;
                WebGameBrowser.Navigate("http://chat.kongregate.com/gamez/0009/3202/live/ElectricBox2.swf?kongregate_game_version=1285951323");
            }
            else
            {
                TTTTL.Visibility = Visibility.Visible;
                TTTTM.Visibility = Visibility.Visible;
                TTTTR.Visibility = Visibility.Visible;
                TTTML.Visibility = Visibility.Visible;
                TTTMM.Visibility = Visibility.Visible;
                TTTMR.Visibility = Visibility.Visible;
                TTTBL.Visibility = Visibility.Visible;
                TTTBM.Visibility = Visibility.Visible;
                TTTBR.Visibility = Visibility.Visible;

                TTTTL.IsEnabled = true;
                TTTTM.IsEnabled = true;
                TTTTR.IsEnabled = true;
                TTTML.IsEnabled = true;
                TTTMM.IsEnabled = true;
                TTTMR.IsEnabled = true;
                TTTBL.IsEnabled = true;
                TTTBM.IsEnabled = true;
                TTTBR.IsEnabled = true;
            }

            // Enable the back button
            backButtonGame.IsEnabled = true;
        }

        private void TTTTL_Click(object sender, RoutedEventArgs e)
        {
            TTTTL.Content = "X";
            TTTTL.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTTM_Click(object sender, RoutedEventArgs e)
        {
            TTTTM.Content = "X";
            TTTTM.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTTR_Click(object sender, RoutedEventArgs e)
        {
            TTTTR.Content = "X";
            TTTTR.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTML_Click(object sender, RoutedEventArgs e)
        {
            TTTML.Content = "X";
            TTTML.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTMM_Click(object sender, RoutedEventArgs e)
        {
            TTTMM.Content = "X";
            TTTMM.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTMR_Click(object sender, RoutedEventArgs e)
        {
            TTTMR.Content = "X";
            TTTMR.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTBL_Click(object sender, RoutedEventArgs e)
        {
            TTTBL.Content = "X";
            TTTBL.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTBM_Click(object sender, RoutedEventArgs e)
        {
            TTTBM.Content = "X";
            TTTBM.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void TTTBR_Click(object sender, RoutedEventArgs e)
        {
            TTTBR.Content = "X";
            TTTBR.IsEnabled = false;
            if (!AllButtonsUsed())
                TTTComputerTurn();
            CheckGameOver();
        }

        private void ReplayGame_Click(object sender, RoutedEventArgs e)
        {
            // Clear game board
            TTTTL.Content = "";
            TTTTM.Content = "";
            TTTTR.Content = "";
            TTTML.Content = "";
            TTTMM.Content = "";
            TTTMR.Content = "";
            TTTBL.Content = "";
            TTTBM.Content = "";
            TTTBR.Content = "";

            // Hide End Game elements
            couponCodeLabel.Visibility = Visibility.Hidden;
            couponCodeLabel.IsEnabled = false;
            endGameReplay.Visibility = Visibility.Hidden;
            endGameReplay.IsEnabled = false;

            // Display board again
            TTTTL.Visibility = Visibility.Visible;
            TTTTM.Visibility = Visibility.Visible;
            TTTTR.Visibility = Visibility.Visible;
            TTTML.Visibility = Visibility.Visible;
            TTTMM.Visibility = Visibility.Visible;
            TTTMR.Visibility = Visibility.Visible;
            TTTBL.Visibility = Visibility.Visible;
            TTTBM.Visibility = Visibility.Visible;
            TTTBR.Visibility = Visibility.Visible;

            TTTTL.IsEnabled = true;
            TTTTM.IsEnabled = true;
            TTTTR.IsEnabled = true;
            TTTML.IsEnabled = true;
            TTTMM.IsEnabled = true;
            TTTMR.IsEnabled = true;
            TTTBL.IsEnabled = true;
            TTTBM.IsEnabled = true;
            TTTBR.IsEnabled = true;
        }

        private void BackButtonGame_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the browser is out of commission
            WebGameBrowser.Navigate("about:blank");
            WebGameBrowser.IsEnabled = false;
            WebGameBrowser.Visibility = Visibility.Hidden;

            // Make sure the TicTacToe buttons are out of commission
            TTTTL.Visibility = Visibility.Hidden;
            TTTTM.Visibility = Visibility.Hidden;
            TTTTR.Visibility = Visibility.Hidden;
            TTTML.Visibility = Visibility.Hidden;
            TTTMM.Visibility = Visibility.Hidden;
            TTTMR.Visibility = Visibility.Hidden;
            TTTBL.Visibility = Visibility.Hidden;
            TTTBM.Visibility = Visibility.Hidden;
            TTTBR.Visibility = Visibility.Hidden;

            TTTTL.IsEnabled = false;
            TTTTM.IsEnabled = false;
            TTTTR.IsEnabled = false;
            TTTML.IsEnabled = false;
            TTTMM.IsEnabled = false;
            TTTMR.IsEnabled = false;
            TTTBL.IsEnabled = false;
            TTTBM.IsEnabled = false;
            TTTBR.IsEnabled = false;

            // Bring up the other UI
            BejeweledButton.Visibility = Visibility.Visible;
            BejeweledButton.IsEnabled = true;
            BloonsTDButton.Visibility = Visibility.Visible;
            BloonsTDButton.IsEnabled = true;
            BloonsButton.Visibility = Visibility.Visible;
            BloonsButton.IsEnabled = true;
            LinesButton.Visibility = Visibility.Visible;
            LinesButton.IsEnabled = true;
            TicTacToeButton.Visibility = Visibility.Visible;
            TicTacToeButton.IsEnabled = true;

            // Disable the Back button
            backButtonGame.IsEnabled = false;
       }
        #endregion // Game Button Events

        #region Game Methods
        private void HideTTTButtons()
        {
            TTTTL.Visibility = Visibility.Hidden;
            TTTTM.Visibility = Visibility.Hidden;
            TTTTR.Visibility = Visibility.Hidden;
            TTTML.Visibility = Visibility.Hidden;
            TTTMM.Visibility = Visibility.Hidden;
            TTTMR.Visibility = Visibility.Hidden;
            TTTBL.Visibility = Visibility.Hidden;
            TTTBM.Visibility = Visibility.Hidden;
            TTTBR.Visibility = Visibility.Hidden;

            TTTTL.IsEnabled = false;
            TTTTM.IsEnabled = false;
            TTTTR.IsEnabled = false;
            TTTML.IsEnabled = false;
            TTTMM.IsEnabled = false;
            TTTMR.IsEnabled = false;
            TTTBL.IsEnabled = false;
            TTTBM.IsEnabled = false;
            TTTBR.IsEnabled = false;
        }

        // This method does crap right now. Needs an actual coupon code.
        private string GetCouponCode()
        {
            return "XXXXX";
        }

        private int IsGameOver()
        {
            // return 0 for not over, return 1 for X victory, return 2 for O victory
            if (TTTTL.Content.ToString() == "X" && TTTTM.Content.ToString() == "X"
                && TTTTR.Content.ToString() == "X") // top row
                return 1;
            if (TTTML.Content.ToString() == "X" && TTTMM.Content.ToString() == "X"
                && TTTMR.Content.ToString() == "X") // middle row
                return 1;
            if (TTTBL.Content.ToString() == "X" && TTTBM.Content.ToString() == "X"
                && TTTBR.Content.ToString() == "X") // bottom row
                return 1;
            if (TTTTL.Content.ToString() == "X" && TTTML.Content.ToString() == "X"
                && TTTBL.Content.ToString() == "X") // left column
                return 1;
            if (TTTTM.Content.ToString() == "X" && TTTMM.Content.ToString() == "X"
                && TTTBM.Content.ToString() == "X") // middle column
                return 1;
            if (TTTTL.Content.ToString() == "X" && TTTTM.Content.ToString() == "X"
                && TTTTR.Content.ToString() == "X") // right column
                return 1;
            if (TTTTL.Content.ToString() == "X" && TTTMM.Content.ToString() == "X"
                && TTTBR.Content.ToString() == "X") // top left to bottom right
                return 1;
            if (TTTBL.Content.ToString() == "X" && TTTMM.Content.ToString() == "X"
                && TTTTR.Content.ToString() == "X") // bottom left to top right
                return 1;

            if (TTTTL.Content.ToString() == "O" && TTTTM.Content.ToString() == "O"
                && TTTTR.Content.ToString() == "O") // top row
                return 2;
            if (TTTML.Content.ToString() == "O" && TTTMM.Content.ToString() == "O"
                && TTTMR.Content.ToString() == "O") // middle row
                return 2;
            if (TTTBL.Content.ToString() == "O" && TTTBM.Content.ToString() == "O"
                && TTTBR.Content.ToString() == "O") // bottom row
                return 2;
            if (TTTTL.Content.ToString() == "O" && TTTML.Content.ToString() == "O"
                && TTTBL.Content.ToString() == "O") // left column
                return 2;
            if (TTTTM.Content.ToString() == "O" && TTTMM.Content.ToString() == "O"
                && TTTBM.Content.ToString() == "O") // middle column
                return 2;
            if (TTTTL.Content.ToString() == "O" && TTTTM.Content.ToString() == "O"
                && TTTTR.Content.ToString() == "O") // right column
                return 2;
            if (TTTTL.Content.ToString() == "O" && TTTMM.Content.ToString() == "O"
                && TTTBR.Content.ToString() == "O") // top left to bottom right
                return 2;
            if (TTTBL.Content.ToString() == "O" && TTTMM.Content.ToString() == "O"
                && TTTTR.Content.ToString() == "O") // bottom left to top right
                return 2;

            if (TTTTL.Content.ToString() != "" && TTTTM.Content.ToString() != ""
                && TTTTR.Content.ToString() != "" && TTTML.Content.ToString() != ""
                && TTTMM.Content.ToString() != "" && TTTMR.Content.ToString() != ""
                && TTTBL.Content.ToString() != "" && TTTBM.Content.ToString() != ""
                && TTTBR.Content.ToString() != "")
                return 3;

            return 0;
        }
        
        private bool AllButtonsUsed()
        {
            if (TTTTL.Content.ToString() == "")
                return false;
            if (TTTTM.Content.ToString() == "")
                return false;
            if (TTTTR.Content.ToString() == "")
                return false;
            if (TTTML.Content.ToString() == "")
                return false;
            if (TTTMM.Content.ToString() == "")
                return false;
            if (TTTMR.Content.ToString() == "")
                return false;
            if (TTTBL.Content.ToString() == "")
                return false;
            if (TTTBM.Content.ToString() == "")
                return false;
            if (TTTBR.Content.ToString() == "")
                return false;
            return true;
        }

        private void TTTComputerTurn()
        {
            int randButton = (RNG.Next() % 9) + 1;

            switch(randButton){
                case 1:
                    if (TTTTL.Content.ToString() == "X" || TTTTL.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTTL.Content = "O";
                        TTTTL.IsEnabled = false;
                    }
                    break;
                case 2:
                    if (TTTTM.Content.ToString() == "X" || TTTTM.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTTM.Content = "O";
                        TTTTM.IsEnabled = false;
                    }
                    break;
                case 3:
                    if (TTTTR.Content.ToString() == "X" || TTTTR.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTTR.Content = "O";
                        TTTTR.IsEnabled = false;
                    }
                    break;
                case 4:
                    if (TTTML.Content.ToString() == "X" || TTTML.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTML.Content = "O";
                        TTTML.IsEnabled = false;
                    }
                    break;
                case 5:
                    if (TTTMM.Content.ToString() == "X" || TTTMM.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTMM.Content = "O";
                        TTTMM.IsEnabled = false;
                    }
                    break;
                case 6:
                    if (TTTMR.Content.ToString() == "X" || TTTMR.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTMR.Content = "O";
                        TTTMR.IsEnabled = false;
                    }
                    break;
                case 7:
                    if (TTTBL.Content.ToString() == "X" || TTTBL.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTBL.Content = "O";
                        TTTBL.IsEnabled = false;
                    }
                    break;
                case 8:
                    if (TTTBM.Content.ToString() == "X" || TTTBM.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTBM.Content = "O";
                        TTTBM.IsEnabled = false;
                    }
                    break;
                case 9:
                    if (TTTBR.Content.ToString() == "X" || TTTBR.Content.ToString() == "O")
                    {
                        TTTComputerTurn();
                    }
                    else
                    {
                        TTTBR.Content = "O";
                        TTTBR.IsEnabled = false;
                    }
                    break;

                default:
                    break;
            }

        }

        private void CheckGameOver()
        {
            int gameOver;

            gameOver = IsGameOver();
            switch (gameOver)
            {
                case 1:
                    // Player wins
                    couponCode = GetCouponCode();
                    couponCodeLabel.Content = "Congratulations! Your coupon code is:\n" + couponCode;
                    break;
                case 2:
                    // Computer wins
                    couponCodeLabel.Content = "Computer wins";
                    break;
                case 3:
                    couponCodeLabel.Content = "Draw!";
                    break;
                default:
                    break;
            }

            if(gameOver > 0)
            {
                couponCodeLabel.Visibility = Visibility.Visible;
                HideTTTButtons();
                endGameReplay.Visibility = Visibility.Visible;
                endGameReplay.IsEnabled = true;
            }
        }
        #endregion // Game Methods
        #endregion // Game

        #region Social Media
        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            authorizationUrl = CredentialsCreator.GetAuthorizationURL(appCredentials);
            // Hide and reset old elements
            twitterMessage.Visibility = Visibility.Hidden;
            twitterMessage.IsEnabled = false;
            twitterMessage.Text = "Write your tweet here";
            twitterButton.Visibility = Visibility.Hidden;
            twitterButton.IsEnabled = false;

            // Bring forward the correct elements
            twitterBrowser.Navigate(authorizationUrl);
            twitterBrowser.IsEnabled = true;
            twitterBrowser.Visibility = Visibility.Visible;
            twitterPinPassword.IsEnabled = true;
            twitterPinPassword.Visibility = Visibility.Visible;
            twitterPinLabel.Visibility = Visibility.Visible;
            twitterPinDone.IsEnabled = true;
            twitterPinDone.Visibility = Visibility.Visible;
        }

        private void TwitterPinDone_Click(object sender, RoutedEventArgs e)
        {
            // Set correct variables
            pinCode = twitterPinPassword.Password;
            userTweet = twitterMessage.Text;
            userCredentials = CredentialsCreator.GetCredentialsFromVerifierCode(pinCode, appCredentials);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);
            Tweet.PublishTweet(userTweet);

            // Hide unneeded elements
            twitterBrowser.IsEnabled = false;
            twitterBrowser.Visibility = Visibility.Hidden;
            twitterPinPassword.IsEnabled = false;
            twitterPinPassword.Visibility = Visibility.Hidden;
            twitterPinLabel.Visibility = Visibility.Hidden;
            twitterPinDone.IsEnabled = false;
            twitterPinDone.Visibility = Visibility.Hidden;
            twitterThanksLabel.Visibility = Visibility.Visible;

            backButtonSocialMedia.IsEnabled = true;
        }

        private void BackButtonSocialMedia_Click(object sender, RoutedEventArgs e)
        {
            // Bring up original elements
            twitterMessage.Visibility = Visibility.Visible;
            twitterMessage.IsEnabled = true;
            twitterButton.Visibility = Visibility.Visible;
            twitterButton.IsEnabled = true;

            // Put away old elements
            twitterBrowser.Navigate("about:blank");
            twitterBrowser.Visibility = Visibility.Hidden;
            twitterBrowser.IsEnabled = false;
            twitterPinLabel.Visibility = Visibility.Hidden;
            twitterPinLabel.IsEnabled = false;
            twitterPinPassword.Visibility = Visibility.Hidden;
            twitterPinPassword.IsEnabled = false;
            twitterPinDone.Visibility = Visibility.Hidden;
            twitterPinDone.IsEnabled = false;
            twitterThanksLabel.Visibility = Visibility.Hidden;
            twitterThanksLabel.IsEnabled = false;

            backButtonSocialMedia.IsEnabled = false;
        }
        #endregion // Social Media

        #region Food Menu

        #region Variables
        decimal _totalPrice = 0;
        decimal _totalTax = 0;
        decimal _subTotal = 0;
        List<CustomerItemOrder> _lstCustomerItemOrder = new List<CustomerItemOrder>();
        #endregion

        #region Properties
        public IList<MenuItemBL> EntreeMenuList
        {
            get
            {
                //1: for Entree category
                List<MenuItemBL> menu = ProgramStart.CustomerController.RetrieveAvailableMenuItemsInCategory(1);
                return menu;
            }
        }

        public IList<MenuItemBL> AppetizerMenuList
        {
            get
            {
                List<MenuItemBL> menu = ProgramStart.CustomerController.RetrieveAvailableMenuItemsInCategory(2); //2: for Entree category
                return menu;
            }
        }

        public IList<MenuItemBL> DrinksMenuList
        {
            get
            {
                List<MenuItemBL> menu = ProgramStart.CustomerController.RetrieveAvailableMenuItemsInCategory(3); //3: for Entree category
                return menu;
            }
        }

        public IList<MenuItemBL> DessertMenuList
        {
            get
            {
                List<MenuItemBL> menu = ProgramStart.CustomerController.RetrieveAvailableMenuItemsInCategory(4); //4: for Entree category
                return menu;
            }
        }
        #endregion // Properties

        private void EntreeButton_Click(object sender, RoutedEventArgs e)
        {
            entreeButton.Visibility = Visibility.Hidden;
            dessertButton.Visibility = Visibility.Hidden;
            appetizerButton.Visibility = Visibility.Hidden;
            drinksButton.Visibility = Visibility.Hidden;
        }

        private void FoodMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                MenuItemBL menuItem = button.DataContext as MenuItemBL;
                if (menuItem != null)
                {
                    if (button.Background == Brushes.YellowGreen) //has code number for button background color
                    {
                        //need to process order here
                        //ProgramStart.EmployeeController.DeactivateItem(menuItem);
                        button.Background = Brushes.Blue;
                    }
                    else
                    {
                        //ProgramStart.EmployeeController.ActivateItem(menuItem);
                        button.Background = Brushes.YellowGreen;
                    }
                    //create customer Order
                    CustomerItemOrder item = new CustomerItemOrder(menuItem.idMenuItem, menuItem.name, menuItem.price, "");
                    //the ItemOrderId will be the same as the last item in the list.
                    item.ItemOrderId = _lstCustomerItemOrder.Count;
                    _lstCustomerItemOrder.Add(item);
                    //refersh grid
                    RefreshCustomerOrder();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshCustomerOrder()
        {
            if (gridCustomerOrder.ItemsSource == null)
                gridCustomerOrder.ItemsSource = CustomerOrder;
            else
            {
                var temp = gridCustomerOrder.ItemsSource;
                gridCustomerOrder.ItemsSource = null;
                gridCustomerOrder.ItemsSource = temp;
            }
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            _totalTax = 0;
            _totalPrice = 0;
            _subTotal = 0;
            foreach (CustomerItemOrder m in _lstCustomerItemOrder)
            {
                _totalPrice = _totalPrice + m.Price;
            }
            _totalTax = _totalPrice * 0.0825M;
            _subTotal = _totalPrice + _totalTax;

            //update GUI.
            txtSubTotal.Text = string.Format("Sub Total: {0:C}", _subTotal);
            txtTotalPrice.Text = string.Format("Total Price: {0:C}", _totalPrice);
            txtTax.Text = string.Format("Total Tax: {0:C}", _totalTax);
        }

        public List<CustomerItemOrder> CustomerOrder
        {
            get
            {
                return _lstCustomerItemOrder;
            }
        }

        private void gridCustomerOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGrid item = (DataGrid)e.Source;
                CustomerItemOrder currentRow = (CustomerItemOrder)item.CurrentItem;

                if (currentRow != null)
                {
                    //display a dialog to ask use enter their special requirement.
                    CustomerOrderRequestWindow frm = new CustomerOrderRequestWindow(currentRow.SpecialRequest);
                    frm.ShowDialog();
                    //update special request.
                    CustomerItemOrder itemOrder = _lstCustomerItemOrder.Find(x => x.ItemOrderId == currentRow.ItemOrderId);
                    if (itemOrder != null)
                        itemOrder.SpecialRequest = frm.SpecialRequest;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //submit order to kitchen
        private void btnSubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //need to create an order, menu order, then change status of the order to send to kitchen
                OrderBL order = new OrderBL();
                order.date = DateTime.Now;
                

                //find available table for customer.
                List<TableBL> lstTableBL = ProgramStart.EmployeeController.GetAllAvailableTables();
                if (lstTableBL != null
                    && lstTableBL.Count > 0)
                {
                    order.idTable = lstTableBL[0].idTable;
                    order.idServer = lstTableBL[0].idServer;
                }
                else //not available, use default value
                {
                    order.idTable = 1;
                    order.idServer = 1;
                }
                order.idCustomer = -1; //server will create a guess customer for this order
                order.subTotal = _totalPrice;
                order.tax = _totalTax;

                _currentOrderId = -1;
                //submit order to server.
                _currentOrderId = ProgramStart.CustomerController.SubmitOrder(order);
                if (_currentOrderId > 0)
                {
                    //create order success
                    //create order menu item for this order.
                    foreach (CustomerItemOrder item in _lstCustomerItemOrder)
                    {
                        MenuOrderBL menu = new MenuOrderBL();
                        menu.idMenuItem = item.MenuItemId;
                        menu.idOrder = _currentOrderId;
                        menu.ItemName = item.ItemName;
                        menu.Price = item.Price;
                        menu.Request = item.SpecialRequest;

                        ProgramStart.CustomerController.SubmitMenuOrderItem(menu);
                    }

                    //order complete.
                    MessageBox.Show("We receive your Order. It will be available shortly.");
                    //referesh data source
                    _lstCustomerItemOrder.Clear();
                    RefreshCustomerOrder();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCheckOutOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentOrderId < 0)
                {
                    MessageBox.Show("Please submit your order in order to check out.");
                    return;
                }
                EOrderDetailPaymentWindow orderPayment = new EOrderDetailPaymentWindow(_currentOrderId);
                orderPayment.ShowDialog();
                //MessageBox.Show("Thank you and See you again!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion            

    }

    #region Custom Converters
    [ValueConversion(typeof(string), typeof(string))]
    public class CaloriesConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("Calories: {0}", value);
        }

        public object ConvertBack(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion // Custom Converters
}

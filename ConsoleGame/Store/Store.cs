﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame {
    public class Store {

        public string Name { get; set; }
        public List<Person> LogedInUsers { get; set; }
        public List<Person> SignUpUsers { get; set; }
        public Store() {
            LogedInUsers = new List<Person>();
            SignUpUsers = new List<Person>();
        }

        private string WellcomeMessage() {
            return $"Wellcome to { Name } store!\n";
        }

        public void WellcomeScreen() {
            var userChoice = new MyUserInputType { ExitAnswer = false };
            while (userChoice.ExitAnswer == false) {
                Console.Clear();
                userChoice = MyUserInput.ChooseFromMenu(WellcomeMessage(), "\nPlease enter your choice:", "x", "-", "LogIn", "SignUp", "-", "Exit");
                if (userChoice.ExitAnswer) {
                    Console.WriteLine("Thank You for your time!");
                } else {
                    switch (userChoice.IntegerValue) {
                        case 1:
                            EnterLoginCredentials();
                            break;
                        case 2:
                            SignUpNewUser();
                            break;
                    }
                }
            }
        }

        private void SignUpNewUser() {
            Console.Clear();
            Console.WriteLine(WellcomeMessage());
            var player = new Person();
            Console.WriteLine("*** SIGN UP ***\n");
            Console.WriteLine("Please enter your info!\n");
            Console.Write("Name:");
            player.Name = Console.ReadLine();
            player.Email = MyConsole.ReadLineEmail("e-mail:");
            Console.Write("Date of Birth:");
            player.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
            var userAnswer = MyUserInput.ChooseFromMenu("Gender info", "please enter option:", "x", "-", "Female", "Male", "-", "I don't want to answer!");
            if (userAnswer.ExitAnswer == false) {
                player.Gender = userAnswer.IntegerValue == 1
                    ? "Female"
                    : "Male";
            }
            var userPassword = "";
            var userPassword2 = "";
            do {
                Console.Write("password:");
                userPassword = MyConsole.ReadLineInputMask('*');
                Console.Write("confirm password:");
                userPassword2 = MyConsole.ReadLineInputMask('*');
                if (userPassword != userPassword2) {
                    Console.WriteLine("Your password don't match! Please try again.");
                }
            } while (userPassword != userPassword2);
            player.UserPassword = userPassword;
            SignUpUsers.Add(player);
            Console.WriteLine("\n*** Congratulation! *** \n\nPress any key to continue...");
            Console.ReadKey();
        }

        public void EnterLoginCredentials() {
            var player = new Person();
            var tryToLogIn = true;
            while (tryToLogIn) {
                Console.Clear();
                Console.WriteLine(WellcomeMessage()+"\nPlease login with your e-mail and password!");
                player.Email = MyConsole.ReadLineEmail("e-mail:");
                Console.Write("password:");
                var userPassword = MyConsole.ReadLineInputMask('*');
                if (IsUserAndPasswordCorrect(player, userPassword)) {
                    var newPlayer = SignInPlayerOrDefault(player, userPassword);
                    LogedInUsers.Add(newPlayer);
                    MainMenu.ShowMenu(this, newPlayer);
                    tryToLogIn = false;
                } else {
                    Console.WriteLine("You have entered invalid e-mail and password combination!");
                    tryToLogIn = MyMessages.TryAgainYesNo();
                }
            }
        }

        private Person SignInPlayerOrDefault(Person player, string userPassword) {
            var indexIfUserIsSignIn = SignUpUsers.IndexOf(SignUpUsers.Where(p => p.Email == player.Email).FirstOrDefault());
            if (indexIfUserIsSignIn >= 0) {
                return SignUpUsers[indexIfUserIsSignIn];
            } else {
                player.UserPassword = userPassword;
                return player;
            }
        }
        private bool IsUserAndPasswordCorrect(Person player, string userPassword) {
            var returnValue = true;
            var indexIfUserIsSignIn = SignUpUsers.IndexOf(SignUpUsers.Where(p => p.Email == player.Email).FirstOrDefault());
            if (indexIfUserIsSignIn >= 0) {
                returnValue = SignUpUsers[indexIfUserIsSignIn].UserPassword == userPassword;
            }
            return returnValue;
        }

        public static void Buy(Person player, Games game) {
            bool startOverAgain = true;
            while (startOverAgain) {
                MyUserInputType userAnswer = PaymentMethodAnswer(player, game);
                int userInputNumber = userAnswer.IntegerValue;
                if (userAnswer.ExitAnswer) {
                    startOverAgain = false;
                } else {
                    Payment(player, game, userInputNumber);
                    startOverAgain = false;
                }
            }
        }

        private static MyUserInputType PaymentMethodAnswer(Person player, Games game) {
            Console.Clear();
            Console.WriteLine("Wellcome to Different Academy games store!\n");
            Console.Write($"You want to buy * { game.GameName } * game and it will cost You { game.GamePrice } MKD!\n");
            return MyUserInput.ChooseFromMenu("Choose Payment method:\n", "\nSelect an option: ", "b", "-", "Cash", "Payment Card", "-", "Back to Main Menu");
        }

        private static void Payment(Person player, Games game, int paymentMethod) {
            bool isPaymentSuccessful = false;
            switch (paymentMethod) {
                case 1:
                    isPaymentSuccessful = CashProcessor.Paying(player, game);
                    break;
                case 2:
                    isPaymentSuccessful = CardProcessor.Paying(player, game);
                    break;
            }
            if(isPaymentSuccessful) {
                game.GameStatus = "Play";
                game.GameDateTimeOfBuying = DateTime.Now.ToString();
            }
        }
    }
}

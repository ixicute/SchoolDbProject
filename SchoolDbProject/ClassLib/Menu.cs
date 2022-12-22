using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDbProject.ClassLib
{
    internal class Menu
    {
        private int selectedIndex;
        //private string[] options;
        private List<string> optionss;
        private string prompt;

        public Menu(string prompt, List<string> optionss)
        {
            this.prompt = prompt;
            //this.options = options;
            this.optionss = optionss;
            selectedIndex = 0;
        }

        /// <summary>
        /// Configurations for the interactive menu as well as display of the set
        /// prompt and options from the object instance.
        /// </summary>
        private void DisplayOptions()
        {
            Console.WriteLine(prompt);

            for (int i = 0; i < optionss.Count; i++)
            {
                string currentOption = optionss[i];
                string prefix;

                if (i == selectedIndex)
                {
                    //The prefix defines the "shape" when item is highlighted.
                    prefix = "=> ";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Red;
                }

                else
                {
                    //This prefix defines the "shape" of items in general (not highlighted).
                    prefix = " ";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                //prints out the options as well as the prefix based on the if-statement logic.
                Console.WriteLine($"{prefix}<< {currentOption} >>");
            }

            Console.ResetColor();
        }

        /// <summary>
        /// presents a menu to select from and returns an int based on choice.
        /// Also controls the key-presses for the menu.
        /// </summary>
        public int PrintMenu()
        {
            //key press instance
            ConsoleKey keyPressed;

            do
            {
                Console.Clear();

                //Runs method that prints out options and prefix for the menu.
                DisplayOptions();

                //Reads input key
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex == -1)
                    {
                        selectedIndex = optionss.Count - 1;
                    }
                }

                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex == optionss.Count)
                    {
                        selectedIndex = 0;
                    }
                }

            } while (keyPressed != ConsoleKey.Enter);

            //Returns the selected index from the menu as int
            return selectedIndex;
        }
    }
}

using FishKing.GumRuntimes;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    static class TextInputHandler
    {

        /// <summary>
        /// Shows the TextField's pop-up with a keyboard and processes the input written by the user
        /// </summary>
        /// <param name="selectedTextField">The selected text field.</param>
        static public void ProcessTextInput(TextRuntime selectedTextField, bool isPassword = false)
        {
#if ANDROID
            var keyCallback = new AsyncCallback((result) =>
            {
                string data = GetKeyboardInput(result);
                //so data will have the string the user has inputted
            });

            string previousText = InputData.GetTextFromItem(textFieldName);
            string description = InputData.GetDescription(textFieldName);

            Guide.BeginShowKeyboardInput(Microsoft.Xna.Framework.PlayerIndex.One,
                title,
                description,
                previousText,
                keyCallback,
                null,
                isPassword);

#elif WINDOWS
            Console.WriteLine("Text Input Parse attempt: " + selectedTextField.Component.Name + " | Password mode: " + isPassword);
#endif
        }

        private static string GetKeyboardInput(IAsyncResult result)
        {
            string newData = Guide.EndShowKeyboardInput(result);
            return newData;
        }
    }
    }

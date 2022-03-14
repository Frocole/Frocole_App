using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AndroidIntegerFix : MonoBehaviour
{
    public InputField inputfield;

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID

        inputfield.characterValidation = InputField.CharacterValidation.None;


        #endif
    }

    List<char> validChars = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

   public void Reviewstring()
    {
        string a = inputfield.text;

        string filteredText = "";
        bool Positive = true;

        foreach (char item in a)
        {
            if (validChars.Contains(item)) filteredText += item;
            else if (item == '-' && a[0] == '-' && Positive)
            {
                filteredText += item;
                Positive = false;
            }
        }

        if (a != filteredText) inputfield.text = filteredText;
    }



}

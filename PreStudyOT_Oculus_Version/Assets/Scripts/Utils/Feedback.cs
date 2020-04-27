using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{
    public static Text InfoTextBottom;
    public static Text InfoTextSide;
    public static Text InfoTextButtons;
    static List<string> textsBottom = new List<string>();
    static List<string> textsSide = new List<string>();
    static List<string> textsButtons = new List<string>();
    static Text[] FeeddbackTexts = new Text[3];

    public string FeedbackInfo1;
    public string FeedbackInfo2;
    public string FeedbackInfo3;

    public void Awake()
    {
        Text[] textFields = GetComponentsInChildren<Text>();
        foreach (Text item in textFields)
        {
            if (item.name == "InfoTextBottom")
            {
                InfoTextBottom = item;
            }
            if (item.name == "InfoTextSide")
            {
                InfoTextSide = item;
            }       
            if (item.name == "InfoTextButtons")
            {
                InfoTextButtons = item;
            }
        }
        FeeddbackTexts[0] = InfoTextBottom;
        FeeddbackTexts[1] = InfoTextSide;
        FeeddbackTexts[2] = InfoTextButtons;
    }
    public void Start()
    {
    }
    private void Update()
    {
        FeedbackInfo2 = GameController.currentState.ToString();
        FeedbackInfo3 = GameController.currentCondition.ToString();
        if (GameController.currentTarget != null)
        {
            FeedbackInfo1 = GameController.currentTarget.ToString();

        }
        else
            FeedbackInfo1= "No Target";
    }
    public void UIVisibility(bool active)
    {
        InfoTextBottom.gameObject.SetActive(active);
        InfoTextButtons.gameObject.SetActive(active);
        InfoTextSide.gameObject.SetActive(active);
    }

    static void UpdateFeedback()
    {
        updateText(textsBottom, InfoTextBottom);
        updateText(textsSide, InfoTextSide);
        updateText(textsButtons, InfoTextButtons);
    }
    static void updateText(List<string> stringList, Text textfield)
    {
        while (stringList.Count> 9)
        {
            stringList.RemoveAt(0);
        }
        textfield.text = "";
        foreach (string item in stringList)
        {
            textfield.text += "\n" + item;
        }
    }    


    public static void AddTextToBottom(string text, bool clear)
    {
        if (clear)
        {
            textsBottom.Clear();
        }
        textsBottom.Add(text);
        UpdateFeedback();
    }   
    public static void AddTextToSide(string text, bool clear)
    {
        if (clear)
        {
            textsSide.Clear();
        }
        textsSide.Add(text);
        UpdateFeedback();
    }    
    public static void AddTextToButton(string text, bool clear)
    {
        if (clear)
        {
            textsButtons.Clear();
        }
        textsButtons.Add(text);
        UpdateFeedback();
    }
}

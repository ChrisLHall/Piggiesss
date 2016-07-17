using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreGUI : MonoBehaviour {
    public Text oneMinTxt;
    public Text threeMinTxt;
    public Text fiveMinTxt;
    public Text tenMinTxt;

    public void RefreshScores () {
        oneMinTxt.text = "1 Minute\n\n" + FormatScores(HighScoreUtil.LoadHighScores(1));
        threeMinTxt.text = "3 Minutes\n\n" + FormatScores(HighScoreUtil.LoadHighScores(3));
        fiveMinTxt.text = "5 Minutes\n\n" + FormatScores(HighScoreUtil.LoadHighScores(5));
        tenMinTxt.text = "10 Minutes\n\n" + FormatScores(HighScoreUtil.LoadHighScores(10));
    }

    string FormatScores (int[] scores) {
        string result = "";
        foreach (int score in scores) {
            result += score.ToString() + "\n";
        }
        result = result.Substring(0, result.Length - 1);
        return result;
    }
}

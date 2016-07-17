using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoreUtil {
    const int NUM_SCORES_TO_KEEP = 5;
    public static void ClipScoreArray (ref int[] scores, int length) {
        var newScores = new int[length];
        for (int i = 0; i < length; i++) {
            if (scores.Length > i) {
                newScores[i] = scores[i];
            } else {
                newScores[i] = 0;
            }
        }
        scores = newScores;
    }

    public static bool HighScoresExist (int gameDuration) {
        return PlayerPrefs.HasKey("scores" + gameDuration.ToString());
    }

    public static void ResetHighScores (int gameDuration) {
        var zeros = new int[NUM_SCORES_TO_KEEP];
        for (int i = 0; i < zeros.Length; i++) {
            zeros[i] = 0;
        }
        SortAndSaveScores(gameDuration, zeros);
    }

    static int[] SortAndSaveScores (int gameDuration, int[] scores) {
        System.Array.Sort<int>(scores,
                    new System.Comparison<int>(
                            (i1, i2) => i2.CompareTo(i1)
                    ));
        ClipScoreArray(ref scores, NUM_SCORES_TO_KEEP);
        string save = "";
        foreach (int score in scores) {
            save += score.ToString() + ",";
        }
        save = save.Substring(0, save.Length - 1);
        PlayerPrefs.SetString("scores" + gameDuration.ToString(), save);
        PlayerPrefs.Save();
        return scores;
    }

    public static int[] LoadHighScores (int gameDuration) {
        if (!HighScoresExist(gameDuration)) {
            ResetHighScores(gameDuration);
        }
        // Load the current high scores and compare with the current.
        string[] strScores = PlayerPrefs.GetString("scores" + gameDuration.ToString()).Split(',');
        int[] result = new int[strScores.Length];
        for (int i = 0; i < result.Length; i++) {
            result[i] = System.Int32.Parse(strScores[i]);
        }
        return result;
    }

    public static int[] AddHighScore (int current, int gameDuration) {
        if (!HighScoresExist(gameDuration)) {
            ResetHighScores(gameDuration);
        }
        int[] scores = LoadHighScores(gameDuration);
        int[] newScores = new int[scores.Length + 1];
        scores.CopyTo(newScores, 0);
        newScores[newScores.Length - 1] = current;
        return SortAndSaveScores(gameDuration, newScores);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class RequiredWordSystem : MonoBehaviour
{
    public List<string> requiredWords = new List<string>()
    {
        "bridge",
        "ladder",
        "box",
        "platform"
    };

    public bool ContainsRequiredWord(string prompt, out string foundWord)
    {
        prompt = prompt.ToLower();

        foreach (var word in requiredWords)
        {
            if (prompt.Contains(word))
            {
                foundWord = word;
                return true;
            }
        }

        foundWord = null;
        return false;
    }
}
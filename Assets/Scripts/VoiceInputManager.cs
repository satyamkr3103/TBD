using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;

public class VoiceInputManager : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;

    public Action<string> OnSpeechRecognized;

    void Start()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log("Voice Input: " + text);
            OnSpeechRecognized?.Invoke(text);
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("Speech Error: " + error);
        };
    }

    public void StartListening()
    {
        dictationRecognizer.Start();
    }

    public void StopListening()
    {
        dictationRecognizer.Stop();
    }
}
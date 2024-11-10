using System;
using UnityEngine;

public class ServerConsole : MonoBehaviour
{
    private ConsoleInput input;

    string strInput;

    //
    // Create console window, register callbacks
    //
    public void AwakeManual(Action<string> OnInputText)
    {
        //DontDestroyOnLoad( gameObject );
        
        input = new ConsoleInput();

        input.OnInputText += OnInputText;

        Application.logMessageReceivedThreaded += HandleLog;

        Debug.Log("Console Started");
    }


    //
    // Debug.Log* callback
    //
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Warning)
            System.Console.ForegroundColor = ConsoleColor.Yellow;
        else if (type == LogType.Error)
            System.Console.ForegroundColor = ConsoleColor.Red;
        else
            System.Console.ForegroundColor = ConsoleColor.White;

        // We're half way through typing something, so clear this line ..
        if (Console.CursorLeft != 0)
            input.ClearLine();

        //System.Console.WriteLine(stackTrace);
        //System.Console.WriteLine(message + "\n\n\n");

        // If we were typing something re-add it.
        input.redrawNextFrame = true;
    }

    //
    // Update the input every frame
    // This gets new key input and calls the OnInputText callback
    //
    void Update()
    {
        input.Update();
    }
}

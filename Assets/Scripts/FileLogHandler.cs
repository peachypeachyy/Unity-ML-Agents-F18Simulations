using System.IO;
using UnityEngine;

public class FileLogHandler : MonoBehaviour
{
    private StreamWriter writer;

    private void Awake()
    {
        // Set the log file path (e.g., "Assets/Logs/log.txt")
        string logFilePath = Path.Combine(Application.dataPath, "Logs/log.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Ensure directory exists
        writer = new StreamWriter(logFilePath, true);

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        writer.WriteLine($"{type}: {logString}");
        if (type == LogType.Exception || type == LogType.Error)
        {
            writer.WriteLine(stackTrace);
        }
        writer.Flush();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        writer.Close();
    }
}

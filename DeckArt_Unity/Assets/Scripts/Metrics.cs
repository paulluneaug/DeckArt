using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

public class Metrics
{
    private static Metrics _instance;
    private StreamWriter m_writer;

    private Dictionary<string, string> m_dictionary = new();

    private Metrics()
    {
    }

    public static Metrics GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Metrics();
        }
        
        return _instance;
    }

    public void WriteData(string title, string value)
    {
        string line = $"{value}\n";
        
        if (!m_dictionary.TryAdd(title, line))
            m_dictionary[title] += line;
    }

    public void WriteDataChunk(string title, string[] values)
    {
        string lines = "";
        
        for (int loop = 0; loop < values.Length; loop++)
        {
            lines += $"{values[loop]}\n";
        }
        lines += "\n";

        if (!m_dictionary.TryAdd(title, lines))
            m_dictionary[title] += lines;
    }

    public void FlushAll()
    {
        m_writer = File.CreateText("Assets/Resources/Metrics.txt");

        foreach (KeyValuePair<string, string> line in m_dictionary)
        {
            m_writer.WriteLine($"{line.Key}\n");
            m_writer.WriteLine(line.Value);
            m_writer.WriteLine(m_writer.NewLine);
        }
        m_writer.Flush();

        m_writer.Close();
    }

    public void ClearMetrics()
    {
        m_dictionary.Clear();
    }
}

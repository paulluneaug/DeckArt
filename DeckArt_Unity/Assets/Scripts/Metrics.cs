using System.IO;

public class Metrics
{
    private static Metrics _instance;
    private StreamWriter writer;

    private Metrics()
    {
        writer = new StreamWriter("Assets/Resources/Metrics.txt");
    }

    public static Metrics GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Metrics();
        }
        
        return _instance;
    }

    public void WriteShit(string shit)
    {
        writer.Write(shit);
        writer.Flush();
    }
}

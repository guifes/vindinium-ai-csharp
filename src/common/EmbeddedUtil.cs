using System.IO;
using System.Reflection;

public static class EmbeddedUtil
{
    public static string ReadTextFile(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(name))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
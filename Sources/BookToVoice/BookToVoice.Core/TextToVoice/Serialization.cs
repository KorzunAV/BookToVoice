using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BookToVoice.Core
{
    public class Serialization
    {
        public static void Save<T>(T obj, string fileName, FileMode f_m)
            where T : class
        {
            Stream fs = new FileStream(fileName, f_m);
            var fmt = new BinaryFormatter();
            fmt.Serialize(fs, obj);            
            fs.Close();
        }
    }
}

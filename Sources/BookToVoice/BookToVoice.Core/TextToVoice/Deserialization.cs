using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LibCsh.FileSys
{
    public class Deserialization
    {
        public static object LoadObj(string fileName)
        {      
            if (!File.Exists(fileName))
            {
                throw new FileLoadException("Deserialization -> I Can't load that.");
            }
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            object LoadConfig = bf.Deserialize(fs);            
            fs.Close();
            return LoadConfig;
        }
    }
}

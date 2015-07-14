
namespace Test
{
    using System;
    using System.IO;

    public class KeyGenerator
    {
        private readonly Random rand = new Random();

        public void GenerateKey(String s)
        {
            byte[] arr = new byte[32];
            rand.NextBytes(arr);

            FileStream fs = new FileStream(s + ".key", FileMode.Create, FileAccess.Write);
            fs.Write(arr, 0, arr.Length);
            fs.Close();
        }
    }
}

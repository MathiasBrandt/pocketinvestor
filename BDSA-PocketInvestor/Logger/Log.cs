namespace Logger
{
    using System;
    using System.IO;

    public class Log
    {
        private static Log instance = null;

        private static Log Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = new Log();
                    Enabled = true;
                }
                return instance;
            }
        }

        private TextWriter writer;

        private bool enabled;

        private Log()
        {
            writer = Console.Out;
            enabled = true;
        }

        public static bool Enabled
        {
            get
            {
                return Instance.enabled;
            }
            set
            {
                if (value)
                    Write("[Log started]");
                else
                    Write("[Log stopped]");
                Instance.enabled = value;
            }
        }

        public Stream Output
        {
            set
            {
                writer = new StreamWriter(value);
            }
        }

        public static void Write(Object message)
        {
            Instance.WriteObject(message);
        }

        private void WriteObject(Object message)
        {
            writer.WriteLine(DateTime.Now.ToShortTimeString() + " " + message.ToString());
        }
    }
}

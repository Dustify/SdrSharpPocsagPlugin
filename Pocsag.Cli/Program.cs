namespace Pocsag.Cli
{
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var file = new NAudio.Wave.WaveFileReader("SDRSharp_20210322_165253Z_153350000Hz_AF.wav");
                //var file = new NAudio.Wave.WaveFileReader("SDRSharp_20210407_174912Z_153350000Hz_AF.wav");

                var samples = new List<float>();

                while (true)
                {
                    var frame = file.ReadNextSampleFrame();

                    if (frame == null)
                    {
                        break;
                    }

                    samples.Add(frame[0]);
                }

                var pocsagManager =
                    new Manager(
                        file.WaveFormat.SampleRate,
                        ( PocsagMessage message) =>
                        {
                            Console.WriteLine(message.Payload);
                        });

                foreach (var sample in samples)
                {
                    pocsagManager.Process(sample);
                }

            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}

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
                var source = args[0];

                Console.WriteLine($"Source: {source}, press any key to process.");

                Console.ReadKey(true);

                var file = new NAudio.Wave.WaveFileReader(source);

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

                var decodes = 0;

                var pocsagManager =
                    new Manager(
                        file.WaveFormat.SampleRate,
                        (PocsagMessage message) =>
                        {
                            if (!message.IsValid)
                            {
                                return;
                            }

                            Console.Write($"{message.Bps} {message.ErrorsCorrected} ");
                            Console.WriteLine(message.Payload);

                            decodes++;
                        });

                pocsagManager.Pocsag512FilterDepth = 1;
                pocsagManager.Pocsag1200FilterDepth = 1;
                pocsagManager.Pocsag2400FilterDepth = 1;

                //foreach (var sample in samples)
                //{
                //    pocsagManager.Process(sample);
                //}

                pocsagManager.Process(samples.ToArray());

                Console.WriteLine($"Decodes: {decodes}");
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
                Console.WriteLine($"Exception: {exception.Message}");
            }

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}

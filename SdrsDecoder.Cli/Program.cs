namespace SdrsDecoder.Cli
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using NAudio.Wave;
    using SdrsDecoder.Acars;
    using SdrsDecoder.Ax25;
    using SdrsDecoder.Pocsag;
    using SdrsDecoder.Support;

    class Program
    {


        static void Main(string[] args)
        {
            try
            {
                //Ax25Debug();
                //Ax25NoDebug();
                Acars();
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
                Console.WriteLine($"Exception: {exception.Message}");
            }

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }

        static void Acars()
        {
            var source = "acars.wav";

            Console.WriteLine($"Source: {source}");

            var file = new WaveFileReader(source);

            var samples = new List<float>();

            while (true)
            {
                var frame = file.ReadNextSampleFrame();

                if (frame == null)
                {
                    break;
                }

                var s = frame[0];

                samples.Add(s);
            }

            var sr = file.WaveFormat.SampleRate;

            var chain = new AcarsChain(sr, (message) =>
            {
                if (message.HasErrors)
                {
                    return;
                }

                Console.WriteLine(message.Payload);
            });

            var position = 0;

            var sw = new Stopwatch();
            sw.Start();

            var debug = new List<float>();

            while (position < samples.Count)
            {
                var block = samples.Skip(position).Take(1000).ToArray();
                chain.Process(
                    block,
                    (s) => { debug.Add(s); }
                    );

                position += 1000;
            }

            sw.Stop();

            using (var writer = new WaveFileWriter("debug.wav", new WaveFormat(chain.Rv.dsr, 4)))
            {
                foreach (var ss in debug.ToArray())
                {
                    writer.WriteSample(ss);
                }
            }
        }

        static void Ax25NoDebug()
        {
            var source = "TNC_Test_Ver-1 track 1 AUDIO-2352.wav";
            //var source = "TNC_Test_Ver-1 track 2 AUDIO-2352.wav";

            Console.WriteLine($"Source: {source}");

            var file = new WaveFileReader(source);

            var samples = new List<float>();

            while (true)
            {
                var frame = file.ReadNextSampleFrame();

                if (frame == null)
                {
                    break;
                }

                var s = frame[0];

                samples.Add(s);
            }

            var sr = file.WaveFormat.SampleRate;

            var messageCount = 0f;
            var successCount = 0f;

            var chain = new Ax25Chain(
                file.WaveFormat.SampleRate,
                (message) =>
                {
                    messageCount += 1;

                    if (!message.HasErrors)
                    {
                        successCount += 1;
                        Console.WriteLine(message.Address + "/" + message.TypeText + ":" + message.Payload);
                    }
                });

            var position = 0;

            var sw = new Stopwatch();
            sw.Start();

            while (position < samples.Count)
            {
                var block = samples.Skip(position).Take(1000).ToArray();
                chain.Process(
                    block
                    );

                position += 1000;
            }

            sw.Stop();

            var realLength = (float)samples.Count / (float)sr;
            var procLength = sw.ElapsedMilliseconds / 1000f;

            Console.WriteLine($"Time factor: {procLength / realLength * 100f}% {sw.Elapsed}");
            Console.WriteLine($"{successCount} / {messageCount} = {successCount / messageCount * 100}%");
        }

        static void Ax25Debug()
        {
            //var source = "TNC_Test_Ver-1 track 1 AUDIO-2352.wav";
            //var source = "TNC_Test_Ver-1 track 2 AUDIO-2352.wav";
            var source = "aprs4-n3.wav";
            //var source = "ax25.wav";

            Console.WriteLine($"Source: {source}");

            var file = new WaveFileReader(source);

            var samples = new List<float>();

            while (true)
            {
                var frame = file.ReadNextSampleFrame();

                if (frame == null)
                {
                    break;
                }

                var s = frame[0];

                samples.Add(s);
            }

            var sr = file.WaveFormat.SampleRate;

            var messageCount = 0f;
            var successCount = 0f;

            var chain = new Ax25Chain(
                file.WaveFormat.SampleRate,
                (message) =>
                {
                    messageCount += 1;

                    if (!message.HasErrors)
                    {
                        successCount += 1;
                        Console.WriteLine(message.Address + "/" + message.TypeText + ":" + message.Payload);
                    }

                });

            var debug = new List<float>();
            void ws(float sample) { debug.Add(sample); }

            var position = 0;

            var sw = new Stopwatch();
            sw.Start();

            while (position < samples.Count)
            {
                var block = samples.Skip(position).Take(1000).ToArray();
                chain.Process(
                    block
                    , writeSample: ws
                    );

                position += 1000;
            }

            sw.Stop();

            var realLength = (float)samples.Count / (float)sr;

            var procLength = sw.ElapsedMilliseconds / 1000f;

            Console.WriteLine($"Time factor: {procLength / realLength * 100f}% {sw.Elapsed}");
            Console.WriteLine($"{successCount} / {messageCount} = {successCount / messageCount * 100}%");

            using (var writer = new WaveFileWriter("debug.wav", new WaveFormat(chain.Rv.dsr, 4)))
            {
                foreach (var ss in debug.ToArray())
                {
                    writer.WriteSample(ss);
                }
            }
        }
    }
}

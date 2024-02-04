namespace SdrsDecoder.Cli
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using NAudio.Wave;
    using SdrsDecoder.Ax25;
    using SdrsDecoder.Pocsag;
    using SdrsDecoder.Support;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var source = "TNC_Test_Ver-1 track 1 AUDIO-2352.wav";
                //var source = "TNC_Test_Ver-1 track 2 AUDIO-2352.wav";
                //var source = "aprs4-n3.wav";
                var source = "ax25.wav";

                var baud = 1200;

                if (args.Length > 0)
                {
                    source = args[0];
                }

                Console.WriteLine($"Source: {source}, press any key to process.");

                //Console.ReadKey(true);

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
                //var rv = ChainBase.GetResampleValues(baud, sr);

                //var interpolator = new Interpolator(rv.i);
                //var interpd = interpolator.Process(samples.ToArray());

                //using (var writer = new WaveFileWriter("interpd.wav", new WaveFormat(rv.isr, 1)))
                //{
                //    foreach (var s in interpd)
                //    {
                //        writer.WriteSample(s);
                //    }
                //}

                //var filter = new ChebyFilter(2200, 1, rv.isr);
                //var filterd = filter.Process(interpd);

                //using (var writer = new WaveFileWriter("filterd.wav", new WaveFormat(rv.isr, 1)))
                //{
                //    foreach (var s in filterd)
                //    {
                //        writer.WriteSample(s);
                //    }
                //}

                //var decim = new Decimator(rv.d);
                //var decimd = decim.Process(filterd);

                //using (var writer = new WaveFileWriter("decimd.wav", new WaveFormat(rv.dsr, 1)))
                //{
                //    foreach (var s in decimd)
                //    {
                //        writer.WriteSample(s);
                //    }
                //}

                //var iqDemod = new IqDemod(rv.dsr, baud, 1200, 2200);
                //var iqdemodd = iqDemod.Process(decimd);

                //using (var writer = new WaveFileWriter("iqdemodd.wav", new WaveFormat(rv.dsr, 1)))
                //{
                //    foreach (var s in iqdemodd)
                //    {
                //        writer.WriteSample(s);
                //    }
                //}

                var messageCount = 0f;
                var successCount = 0f;

                //var chain = new PocsagChain(
                //    baud,
                //    file.WaveFormat.SampleRate,
                //    (message) =>
                //    {
                //        messageCount += 1;

                //        if (!message.HasErrors)
                //        {
                //            successCount += 1;
                //        }

                //        Console.WriteLine(message.ErrorText + (message.HasErrors ? "" : " " + message.TypeText + " " + message.Payload));
                //    });

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

                        //Console.WriteLine(message.HasErrors);

                        //Console.WriteLine(message.Payload);
                    });

                //var debug = new List<float>();
                //void ws(float sample) { debug.Add(sample); }

                var position = 0;

                var sw = new Stopwatch();
                sw.Start();

                while (position < samples.Count)
                {
                    var block = samples.Skip(position).Take(1000).ToArray();
                    chain.Process(
                        block
                        //, writeSample: ws
                        );

                    position += 1000;
                }

                sw.Stop();

                var realLength = (float)samples.Count / (float)sr;

                var procLength = sw.ElapsedMilliseconds / 1000f;

                Console.WriteLine($"Time factor: {procLength / realLength * 100f}% {sw.Elapsed}");

                //chain.Process(samples.ToArray(), writeSample: ws);

                Console.WriteLine($"{successCount} / {messageCount} = {successCount / messageCount * 100}%");

                //using (var writer = new WaveFileWriter("debug.wav", new WaveFormat(chain.Rv.dsr, 4)))
                //{
                //    foreach (var ss in debug.ToArray())
                //    {
                //        writer.WriteSample(ss);
                //    }
                //}                
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

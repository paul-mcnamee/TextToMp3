using System.Speech.Synthesis; 
using System.Speech.AudioFormat;
using System.IO;

//reference Nuget Package NAudio.Lame
using NAudio.Wave;
using NAudio.Lame;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

class TextToMp3
{
    static void Main(string[] args)
    {
        using (SpeechSynthesizer synth = new SpeechSynthesizer())
        {
            if (synth != null)
            {
                //set some settings
                synth.Volume = (int)PromptVolume.Default;
                synth.Rate = (int)PromptRate.Fast;
                synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

                // Read the JSON file content
                string json = File.ReadAllText("entries.json");
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                
                if (dict != null)
                {
                    string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    Directory.CreateDirectory(exeDir + "\\output");
                    Directory.SetCurrentDirectory(exeDir + "\\output");

                    foreach ( var pair in dict )
                    {
                        Console.WriteLine(pair.Key + ":" + pair.Value);
                        //save to memory stream
                        MemoryStream ms = new MemoryStream();
                        synth.SetOutputToWaveStream(ms);

                        //do speaking
                        synth.Speak(pair.Value);

                        //now convert to mp3 using LameEncoder or shell out to audiograbber
                        ConvertWavStreamToMp3File(ref ms, pair.Key);
                    }
                }
            }
        }
    }

    public static void ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
    {
        //rewind to beginning of stream
        ms.Seek(0, SeekOrigin.Begin);

        using (var retMs = new MemoryStream())
        using (var rdr = new WaveFileReader(ms))
        using (var wtr = new LameMP3FileWriter(savetofilename, rdr.WaveFormat, LAMEPreset.VBR_90))
        {
            rdr.CopyTo(wtr);
        }
    }
}








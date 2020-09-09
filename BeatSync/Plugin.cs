using IPA;
using IPALogger = IPA.Logging.Logger;
using BS_Utils;
using System.IO.Ports;
using System.Diagnostics;
using System;
using BeatSaberMarkupLanguage;

namespace BeatSync
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }

        SerialPort arduinoCOM = new SerialPort("COM4", 115200);
        Stopwatch stopwatch = new Stopwatch();

        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            arduinoCOM.Open();
            BS_Utils.Utilities.BSEvents.noteWasCut += BSEvents_noteWasCut; 

        }

        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3)
        {
            var hex = new byte[] { 0 };
            TimeSpan ts = stopwatch.Elapsed;
            stopwatch.Reset();
            log.Debug("Time elapsed: " + ts.TotalMilliseconds);
            if (ts.TotalMilliseconds < 20)
            {
                hex = new byte[] { 5 };
                arduinoCOM.Write(hex, 0, hex.Length);
               
            }
            else
            {
                switch (arg1.noteType)
                {
                    case NoteType.NoteA:
                        hex = new byte[] { 1 };
                        arduinoCOM.Write(hex, 0, hex.Length);
                        break;

                    case NoteType.NoteB:
                        hex = new byte[] { 3 };
                        arduinoCOM.Write(hex, 0, hex.Length);
                        break;
                }
            }
            stopwatch.Start();
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

    }
}

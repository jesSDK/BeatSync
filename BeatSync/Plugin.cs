using IPA;
using IPALogger = IPA.Logging.Logger;
using BS_Utils;
using System.IO.Ports;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;
using BS_Utils.Utilities;
using System.Net.Sockets;
using WebSocketSharp;

namespace BeatSync
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }

        Stopwatch stopwatch = new Stopwatch();
        //NetworkStream clientStream;
        WebSocket ws = new WebSocket("ws://192.168.0.69:6969");
        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            log.Debug("Beatsync Started");
            log.Error("Beatsync Started - Error");
            BS_Utils.Utilities.BSEvents.noteWasCut += BSEvents_noteWasCut;
            TimeSpan waittime = new TimeSpan(0, 0, 0);
            ws.WaitTime = waittime;
            ws.Connect();
            ws.Send("GREEN");
            ws.OnClose += (sender, e) =>
            {
                log.Error("Websocket closed");
                ws.Connect();
            };
            //BSMLSettings.instance.AddSettingsMenu("BeatSync", "BeatSync.UI.BSMLsettings.bsml", SettingsViewController.instance);
        }


        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3)
        {
                if (ws.IsAlive)
                {
                var hex = new byte[] { 0 };
                TimeSpan ts = stopwatch.Elapsed;
                stopwatch.Reset();
                log.Debug("Time elapsed: " + ts.TotalMilliseconds);
                if (ts.TotalMilliseconds < 20)
                {
                    /*
                    hex = new byte[] { 5 };
                    //arduinoCOM.Write(hex, 0, hex.Length);
                    clientStream.Write(hex, 0, hex.Length);
                    */
                    ws.Send("PURPLE");
                }
                else
                {
                    switch (arg1.colorType)
                    {
                        case ColorType.ColorA:
                            log.Debug("A");
                            /*
                            hex = new byte[] { 1 };
                            //arduinoCOM.Write(hex, 0, hex.Length);
                            clientStream.Write(hex, 0, hex.Length);
                            */
                            ws.Send("RED");
                            break;

                        case ColorType.ColorB:
                            log.Debug("B");
                            /*
                            hex = new byte[] { 3 };
                            // arduinoCOM.Write(hex, 0, hex.Length);
                            clientStream.Write(hex, 0, hex.Length);
                            */
                            ws.Send("BLUE");
                            break;
                    }
                }
                stopwatch.Start();
            }
            
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

    }
}

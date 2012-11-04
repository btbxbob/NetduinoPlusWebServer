using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO;
using Nokia.LCDScreens;

namespace NetduinoPlusWebServer
{
    public class Program
    {
        const string WebFolder = "\\SD\\";
        static public Nokia_5110 LCD = new Nokia_5110(false, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D4);

        // Bitmap genenerated at https://stefan.co/stco-includes/createbitmap.php
        //fujixerox icon
        static public // Bitmap genenerated at https://stefan.co/stco-includes/createbitmap.php
byte[] Bitmap = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 64, 64, 64, 96,
96, 112, 112, 48, 56, 56, 56, 56, 60, 28, 12, 12, 4, 6, 6, 6, 6, 2, 2, 2, 2, 2,
2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 128, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 1, 3, 2, 6, 12, 24, 48, 96, 192, 128, 0, 0, 0, 0, 0, 128,
248, 252, 254, 254, 254, 254, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
255, 254, 254, 254, 254, 254, 254, 254, 254, 252, 252, 252, 252, 248, 248, 248,
248, 240, 240, 240, 240, 224, 224, 224, 192, 192, 192, 128, 128, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
135, 254, 248, 128, 0, 0, 1, 31, 127, 255, 255, 255, 255, 255, 255, 255, 255,
255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 127,
63, 31, 15, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 14, 63, 255, 127, 31, 1, 0, 0, 0, 0, 0, 1, 3, 7, 15, 31,
63, 63, 127, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
255, 255, 255, 255, 255, 255, 127, 127, 63, 63, 31, 15, 15, 7, 3, 1, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 128, 192, 224, 240, 240, 248, 252, 254, 255,
254, 252, 252, 248, 224, 192, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 7, 7, 7, 7, 7, 7, 3, 3, 3, 1, 1, 0,
0, 0, 0, 0, 0, 0, 0, 64, 64, 64, 64, 64, 96, 96, 96, 112, 112, 120, 120, 124,
60, 62, 62, 63, 63, 63, 31, 31, 31, 31, 15, 15, 15, 7, 7, 7, 3, 3, 1, 1, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static void Main()
        {
            LCD.ByteMap = Bitmap;
            LCD.Refresh();
            //LCD.DrawString("Start initial.");
            //LCD.Refresh();
            //start the LCD sheild
            //Nokia_5110 LCD = new Nokia_5110(false, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D4);
            LCD.BacklightBrightness = 100;
            //set ip static
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            networkInterface.EnableStaticIP("192.168.1.199", "255.255.255.0", "192.168.1.1");

            //the server:
            //Listener webServer = new Listener(RequestReceived);

            AnalogInput keys_port = new AnalogInput(Cpu.AnalogChannel.ANALOG_0);
            AnalogInput test_port = new AnalogInput(Cpu.AnalogChannel.ANALOG_2);

            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            //LCD.DrawString("Done init.");
            //LCD.Refresh();
            while (true)
            {
                // Blink LED to show we're still responsive

                led.Write(!led.Read());
                Thread.Sleep(1000);
                LCD.Clear();
                LCD.DrawString(0, 0, "========");
                LCD.DrawString(0, 1, "AD Port 0:");
                LCD.DrawString(0, 2, keys_port.ReadRaw().ToString());
                LCD.DrawString(0, 3, "AD Port 2:");
                LCD.DrawString(0, 4, test_port.ReadRaw().ToString());
                LCD.DrawString(0, 5, "========");
                LCD.Refresh();
            }
        }


        private static void RequestReceived(Request request)
        {
            // Use this for a really basic check that it's working
            //request.SendResponse("<html><body><p>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL +"</p></body></html>");

            // Send a file
            //LCD.DrawString("Request Received:"+request.Client.ToString());
            TrySendFile(request);
            LCD.DrawString(request.Client.ToString());
            LCD.Refresh();
        }

        /// <summary>
        /// Look for a file on the SD card and send it back if it exists
        /// </summary>
        /// <param name="request"></param>
        private static void TrySendFile(Request request)
        {
            // Replace / with \
            string filePath = WebFolder + request.URL.Replace('/', '\\');

            if (File.Exists(filePath))
                request.SendFile(filePath);
            else
            {
                request.Send404();
                LCD.DrawString("404:" + request.URL);
                LCD.Refresh();
            }
        }

    }
}

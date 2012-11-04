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
        static public Nokia_5110 LCD=new Nokia_5110(false, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D4);

        public static void Main()
        {
            LCD.DrawString("Start initial.");
            LCD.Refresh();
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
            LCD.DrawString("Done init.");
            LCD.Refresh();
            while (true)
            {
                // Blink LED to show we're still responsive
                
                led.Write(!led.Read());
                Thread.Sleep(1000);
                LCD.Clear();
                LCD.DrawString(0,0,"========");
                LCD.DrawString(0,1,"AD Port 0:");
                LCD.DrawString(0,2,keys_port.ReadRaw().ToString());
                LCD.DrawString(0,3,"AD Port 2:");
                LCD.DrawString(0,4,test_port.ReadRaw().ToString());
                LCD.DrawString(0,5,"========");
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

using System;
using System.Linq;
using HidApiAdapter;

namespace DevicesViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowDevices();

            Console.WriteLine("press any key to exit ...");
            Console.ReadKey();
        }

        private static void ShowDevices()
        {
            var deviceManager = HidDeviceManager.GetManager();
            
            //trying to find any device
            var devices = deviceManager.SearchDevices(0, 0);

            if(devices.Any())
            {
                foreach(var device in devices)
                {
                    device.Connect();
                    ShowDeviceInfo(device);
                    device.Disconnect();
                }
            }
            else
            {
                Console.WriteLine("no devices found");
            }
        }

        private static void ShowDeviceInfo(HidDevice device)
        {
            Console.WriteLine(
                $"device: {device.Path()}\n" +
                $"manufacturer: {device.Manufacturer()}\n" +
                $"product: {device.Product()}\n" +
                $"serial number: {device.SerialNumber()}\n");
        }
    }
}

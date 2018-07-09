# hidapi adapter

This project was created to port popular library, that hepls to interact with HID devices - hidapi (C language) https://github.com/signal11/hidapi

The main goal of project is to make library that adapted to C#

Here is example showing how to print inforamtion about connected HID devices

```
    //trying to find any device
    var devices = HidDeviceManager.GetManager().SearchDevices(0, 0);

    if(devices.Any())
    {
        foreach(var device in devices)
        {
            device.Connect();
            
            Console.WriteLine(
              $"device: {device.Path()}\n" +
              $"manufacturer: {device.Manufacturer()}\n" +
              $"product: {device.Product()}\n" +
              $"serial number: {device.SerialNumber()}\n");

            device.Disconnect();
        }

    }

```

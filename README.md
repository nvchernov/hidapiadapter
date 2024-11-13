# hidapi adapter

This project was created to port popular library, that helps to interact with HID devices - hidapi (C language) https://github.com/signal11/hidapi

The main goal of project is to make library that adapted to C#

Here is example showing how to print information about connected HID devices

```
    //trying to find devices with any vendor_id, product_id
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
## How to use?
To get devices use SearchDevices static method of HidDeviceManager class.

```
    HidDeviceManager.GetManager().SearchDevices(0, 0);
```

SearchDevices will return a set of founded devices (HidDevice)

To interact with devices use methods Read\Write. Here example
```
...
    m_ReadResult = m_Device.Read(m_Buffer, m_Buffer.Length);
    if (m_ReadResult == -1)
        throw new HardwareConnectionException("Cannot read data from device");
...
```

## Nuget

`Install-Package HidApiAdapter`

## Troubleshooting

### Properties of installed dlls

If you getting DllNotFoundException: ... DLL "hidapi32"

Change properties of installed dlls - hidapi32.dll hidapi64.dll like on screenshot

![image](https://user-images.githubusercontent.com/14893576/50458730-72e65780-0976-11e9-9d46-1d4874083586.png)

### No C++ Redistributable package installed

You may getting exception "DllNotFoundException: Unable to load DLL 'hidapi32': The specified module could not be found" because you have no C++ Redistributable package installed on your windows. To fix this just install it









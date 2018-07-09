using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HidApiAdapter
{
    public class HidDeviceManager
    {

        private static HidDeviceManager m_DeviceManager = null;

        private const int MAX_DEVICE_COUNT = 255;

        private HidDeviceManager() { }

        public static HidDeviceManager GetManager() =>
            m_DeviceManager ?? (m_DeviceManager = new HidDeviceManager());

        static HidDeviceManager()
        {
            HidApi.hid_init();
        }

        ~HidDeviceManager()
        {
            foreach (var deviceInfo in devicesInfo)
                HidApi.hid_free_enumeration(deviceInfo);

            HidApi.hid_exit();
        }

        private List<IntPtr> devicesInfo = new List<IntPtr>();

        public List<HidDevice> SearchDevices(int vid, int pid)
        {
            var devices = new List<HidDevice>();

            var devicesLinkedList = HidApi.hid_enumerate((ushort)vid, (ushort)pid);
            devicesInfo.Add(devicesLinkedList);

            if (devicesLinkedList == IntPtr.Zero)
                return null;

            hid_device_info deviceInfo = 
                (hid_device_info)Marshal.PtrToStructure(devicesLinkedList, typeof(hid_device_info));

            devices.Add(new HidDevice(deviceInfo, devicesLinkedList));

            while (deviceInfo.next != IntPtr.Zero && devices.Count < MAX_DEVICE_COUNT)
            {
                var ptr = deviceInfo.next;

                deviceInfo = (hid_device_info)Marshal.PtrToStructure(deviceInfo.next, typeof(hid_device_info));

                devices.Add(new HidDevice(deviceInfo, ptr));
            }

            if (devices.Count >= MAX_DEVICE_COUNT)
                throw new Exception($"device max number was reached while enumerating devices - {MAX_DEVICE_COUNT}");
            
            return devices;
        }


    }
}

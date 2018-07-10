using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HidApiAdapter
{
    /// <summary>
    /// HID devices manager, to get instance of <see cref="HidDeviceManager"/> use static method GetManager
    /// </summary>
    public class HidDeviceManager
    {

        private static HidDeviceManager m_DeviceManager = null;

        private const int MAX_DEVICE_COUNT = 255;

        private HidDeviceManager() { }

        /// <summary>
        /// Get current HID devices manager
        /// </summary>
        /// <returns>HID devices manager</returns>
        public static HidDeviceManager GetManager() =>
            m_DeviceManager ?? (m_DeviceManager = new HidDeviceManager());

        static HidDeviceManager()
        {
            HidApi.hid_init();
        }

        ~HidDeviceManager()
        {
            foreach (var deviceInfo in devicesEnumerations)
                HidApi.hid_free_enumeration(deviceInfo);

            HidApi.hid_exit();
        }

        /// <summary>
        /// Every device search result linked list of HidApi.hid_enumerate will store here to free unmanagment memory on destructor
        /// </summary>
        private List<IntPtr> devicesEnumerations = new List<IntPtr>();

        /// <summary>
        /// Try to find devices by Vendor Id and Product Id
        /// </summary>
        /// <param name="vid">Vendor Id. Set pid to 0 to find devices with any Product Id</param>
        /// <param name="pid">Product Id. Set vid and pid to 0 to find any devices</param>
        /// <returns></returns>
        public List<HidDevice> SearchDevices(int vid, int pid)
        {
            var devices = new List<HidDevice>();

            var devicesLinkedList = HidApi.hid_enumerate((ushort)vid, (ushort)pid);
            devicesEnumerations.Add(devicesLinkedList);

            if (devicesLinkedList == IntPtr.Zero)
                return null;

            hid_device_info deviceInfo = 
                Marshal.PtrToStructure<hid_device_info>(devicesLinkedList);

            devices.Add(new HidDevice(deviceInfo, devicesLinkedList));

            while (deviceInfo.next != IntPtr.Zero && devices.Count < MAX_DEVICE_COUNT)
            {
                var ptr = deviceInfo.next;

                deviceInfo = Marshal.PtrToStructure<hid_device_info>(deviceInfo.next);

                devices.Add(new HidDevice(deviceInfo, ptr));
            }

            if (devices.Count >= MAX_DEVICE_COUNT)
                throw new Exception($"device max number was reached while enumerating devices - {MAX_DEVICE_COUNT}");
            
            return devices;
        }


    }
}

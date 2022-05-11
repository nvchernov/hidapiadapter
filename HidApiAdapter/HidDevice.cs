using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HidApiAdapter
{
    public class HidDevice
    {
        private const int MARSHALED_STRING_MAX_LEN = 1024 / 2;

        private const int BUFFER_DEFAULT_SIZE = 1024;

        private readonly hid_device_info m_DeviceInfo;
        private IntPtr m_DevicePtr = IntPtr.Zero;

        private HidDevice() { }

        internal HidDevice(hid_device_info deviceInfo, IntPtr devicePtr)
        {
            m_DeviceInfo = deviceInfo;
            m_DevicePtr = devicePtr;
        }

        /// <summary>
        /// Can instance interact with HID device 
        /// </summary>
        public bool IsValid => m_DevicePtr != IntPtr.Zero;

        private bool m_IsConnected;
        /// <summary>
        /// Device connected successful
        /// </summary>
        public bool IsConnected => m_IsConnected;

        public int VendorId => m_DeviceInfo.vendor_id;
        public int ProductId => m_DeviceInfo.product_id;

        public int UsagePage => m_DeviceInfo.usage_page;
        public int Usage => m_DeviceInfo.usage;

        /// <summary>
        /// Platform-specific device path
        /// </summary>
        /// <returns></returns>
        public string Path() => 
            Marshal.PtrToStringAnsi(m_DeviceInfo.path);

        /// <summary>
        /// Connect to HID device
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (m_DevicePtr == IntPtr.Zero)
                return false;

            m_DevicePtr = HidApi.hid_open_path(m_DeviceInfo.path);

            if(m_DevicePtr != IntPtr.Zero)
                m_IsConnected = true;

            return true;
        }

        /// <summary>
        /// Disconnect from HID device
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (m_DevicePtr == IntPtr.Zero)
                return false;

            HidApi.hid_close(m_DevicePtr);

            m_IsConnected = false;
            return true;
        }

        private byte[] m_WriteBuffer = new byte[BUFFER_DEFAULT_SIZE];

        public int Write(byte[] bytes)
        {
            if (m_DevicePtr == IntPtr.Zero)
                return 0;

            if (bytes == null || bytes.Length == 0)
                return 0;

            if (m_WriteBuffer.Length <= bytes.Length)
                Array.Resize(ref m_WriteBuffer, bytes.Length + 2);

            //TODO fix this for other OSs
            //hidapi for windows has problem - first byte must be 0 also array length shuold be increased for 1
            Array.Copy(bytes, 0, m_WriteBuffer, 1, bytes.Length);

            return HidApi.hid_write(m_DevicePtr, m_WriteBuffer, Convert.ToUInt32(bytes.Length + 1));
        }

        public int SendFeatureReport(byte[] bytes)
        {
            return HidApi.hid_send_feature_report(m_DevicePtr, bytes, Convert.ToUInt32(bytes.Length));
        }

        public int Read(byte[] buff, int len)
        {
            if (m_DevicePtr == IntPtr.Zero)
                return 0;

            return HidApi.hid_read(m_DevicePtr, buff, Convert.ToUInt32(len));
        }

        #region device info

        StringBuilder m_DeviceInfoBuffer = new StringBuilder(BUFFER_DEFAULT_SIZE);

        /// <summary>
        /// Device serial number
        /// </summary>
        /// <returns></returns>
        public string SerialNumber()
        {
            m_DeviceInfoBuffer.Clear();
            HidApi.hid_get_serial_number_string(m_DevicePtr, m_DeviceInfoBuffer, MARSHALED_STRING_MAX_LEN);

            return m_DeviceInfoBuffer.ToString();
        }


        /// <summary>
        /// Device manufacturer
        /// </summary>
        /// <returns></returns>
        public string Manufacturer()
        {
            m_DeviceInfoBuffer.Clear();
            HidApi.hid_get_manufacturer_string(m_DevicePtr, m_DeviceInfoBuffer, MARSHALED_STRING_MAX_LEN);

            return m_DeviceInfoBuffer.ToString();
        }

        /// <summary>
        /// Device product
        /// </summary>
        /// <returns></returns>
        public string Product()
        {
            m_DeviceInfoBuffer.Clear();
            HidApi.hid_get_product_string(m_DevicePtr, m_DeviceInfoBuffer, MARSHALED_STRING_MAX_LEN);

            return m_DeviceInfoBuffer.ToString();
        }

        /// <summary>
        /// Get all available strings of HID device 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> DeviceStrings()
        {
            const int maxStringNum = 16;
            int counter = 0;

            m_DeviceInfoBuffer.Clear();
            while (HidApi.hid_get_indexed_string(m_DevicePtr, counter, m_DeviceInfoBuffer, MARSHALED_STRING_MAX_LEN) == 0 && counter++ < maxStringNum)
            {
                yield return m_DeviceInfoBuffer.ToString();
                m_DeviceInfoBuffer.Clear();
            }
        }

        /// <summary>
        /// Get a string from a HID device, based on its string index
        /// </summary>
        /// <param name="index">The index of the string to get</param>
        /// <returns></returns>
        public string DevicesString(int index)
        {
            m_DeviceInfoBuffer.Clear();

            var res = HidApi.hid_get_indexed_string(m_DevicePtr, index, m_DeviceInfoBuffer, MARSHALED_STRING_MAX_LEN);

            return res == 0 ? m_DeviceInfo.ToString() : null;
        }

        #endregion

        public override string ToString()
        {
            if (IsValid)
                return $"manufacturer: {Manufacturer()}, serial_number:{SerialNumber()}, product:{Product()}";
            else
                return "unknown device (not connected)";
        }

    }
}

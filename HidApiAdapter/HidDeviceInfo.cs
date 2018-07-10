using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HidApiAdapter
{
    /// <summary>
    /// hidapi info structure
    /// </summary>
    public struct hid_device_info
    {
        /// <summary>
        /// Platform-specific device path
        /// </summary>
        public IntPtr path;

        /// <summary>
        /// Device Vendor ID
        /// </summary>
        public ushort vendor_id;

        /// <summary>
        /// Device Product ID
        /// </summary>
        public ushort product_id;

        /// <summary>
        /// Serial Number
        /// </summary>
        public IntPtr serial_number;

        /// <summary>
        /// Device Release Number in binary-coded decimal, also known as Device Version Number
        /// </summary>
        public ushort release_number;

        /// <summary>
        /// Manufacturer String
        /// </summary>
        public IntPtr manufacturer_string;

        /// <summary>
        /// Product string
        /// </summary>
        public IntPtr product_string;

        /// <summary>
        /// Usage for this Device/Interface (Windows/Mac only).
        /// </summary>
        public ushort usage_page;

        /// <summary>
        /// Usage Page for this Device/Interface (Windows/Mac only).
        /// </summary>
        public ushort usage;

        /// <summary>
        /// The USB interface which this logical device
        /// represents. Valid on both Linux implementations
        /// in all cases, and valid on the Windows implementation
        /// only if the device contains more than one interface.
        /// </summary>
        public int interface_number;

        /// <summary>
        /// Pointer to the next device
        /// </summary>
        public IntPtr next;
    };
}

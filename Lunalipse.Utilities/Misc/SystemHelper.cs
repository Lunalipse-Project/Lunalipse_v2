using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class SystemHelper
    {
        public static string GetProcessorInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            stringBuilder.AppendLine("==========Processor Info========== ");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                stringBuilder.AppendFormat(">>> {0}", obj["DeviceID"]);
                stringBuilder.AppendLine("Name  -  " + obj["Name"]);
                stringBuilder.AppendLine("Manufacturer  -  " + obj["Manufacturer"]);
                stringBuilder.AppendLine("CurrentClockSpeed  -  " + obj["CurrentClockSpeed"]);
                stringBuilder.AppendLine("Caption  -  " + obj["Caption"]);
                stringBuilder.AppendLine("NumberOfCores  -  " + obj["NumberOfCores"]);
                stringBuilder.AppendLine("NumberOfEnabledCore  -  " + obj["NumberOfEnabledCore"]);
                stringBuilder.AppendLine("NumberOfLogicalProcessors  -  " + obj["NumberOfLogicalProcessors"]);
                stringBuilder.AppendLine("Architecture  -  " + obj["Architecture"]);
                stringBuilder.AppendLine("Family  -  " + obj["Family"]);
                stringBuilder.AppendLine("ProcessorType  -  " + obj["ProcessorType"]);
                stringBuilder.AppendLine("Characteristics  -  " + obj["Characteristics"]);
                stringBuilder.AppendLine("AddressWidth  -  " + obj["AddressWidth"]);
            }
            return stringBuilder.ToString();
        }

        public static string GetOSInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            int i = 0;
            stringBuilder.AppendLine("==========Operation System Info========== ");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                stringBuilder.AppendFormat(">>>OS {0}\n", i);
                stringBuilder.AppendFormat("Total Memory - {0}KB\n", obj["TotalVisibleMemorySize"]);
                stringBuilder.AppendLine("Caption  -  " + obj["Caption"]);
                stringBuilder.AppendLine("WindowsDirectory  -  " + obj["WindowsDirectory"]);
                stringBuilder.AppendLine("ProductType  -  " + obj["ProductType"]);
                stringBuilder.AppendLine("SerialNumber  -  " + obj["SerialNumber"]);
                stringBuilder.AppendLine("SystemDirectory  -  " + obj["SystemDirectory"]);
                stringBuilder.AppendLine("CountryCode  -  " + obj["CountryCode"]);
                stringBuilder.AppendLine("CurrentTimeZone  -  " + obj["CurrentTimeZone"]);
                stringBuilder.AppendLine("EncryptionLevel  -  " + obj["EncryptionLevel"]);
                stringBuilder.AppendLine("OSType  -  " + obj["OSType"]);
                stringBuilder.AppendLine("Version  -  " + obj["Version"]);
                i++;
            }
            return stringBuilder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common
{
    public class VersionHelper
    {
        private static volatile VersionHelper VHELPER_INSTANCE;
        private static readonly object VHELPER_LOCK = new object();

        public const string SOFTWARE_CODE = "Luna";
        

        public static VersionHelper Instance
        {
            get
            {
                if (VHELPER_INSTANCE == null)
                {
                    lock(VHELPER_LOCK)
                    {
                        if (VHELPER_INSTANCE == null)
                            return VHELPER_INSTANCE = new VersionHelper();
                    }
                }
                return VHELPER_INSTANCE;
            }
        }

        Version asmv;
        protected VersionHelper()
        {
            asmv = Assembly.GetEntryAssembly().GetName().Version;
            LinkerTime = GetLinkerTime(Assembly.GetEntryAssembly());
        }

        
        public string getFullVersion()
        {
            return asmv.ToString();
        }

        public int getBuildVersion()
        {
            return asmv.Build;
        }

        public int getMajorVersion()
        {
            return asmv.Major;
        }

        public int getRevisionVersion()
        {
            return asmv.Revision;
        }

        public int getMinorVersion()
        {
            return asmv.Minor;
        }

        public Version Version
        {
            get
            {
                return asmv;
            }
        }

        public DateTime LinkerTime { get; private set; }

        public string getGenerationTypedVersion(LunalipseGeneration lunalipseGeneration)
        {
            switch(lunalipseGeneration)
            {
                case LunalipseGeneration.Release:
                    return string.Format("{0}.{1}r{2}", asmv.Major, asmv.Minor, asmv.Build);
                case LunalipseGeneration.Build:
                    return string.Format("Build{0}", asmv.Revision);
                case LunalipseGeneration.Alpha:
                    return string.Format("Alpha {0}.{1}", asmv.Major, asmv.Revision);
                case LunalipseGeneration.Beta:
                    return string.Format("Beta {0}.{1}", asmv.Major, asmv.Revision);
                default:
                    return "";
            }
            
        }
        public DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }

    public enum LunalipseGeneration
    {
        Build,
        Alpha,
        Beta,
        Release
    }
}

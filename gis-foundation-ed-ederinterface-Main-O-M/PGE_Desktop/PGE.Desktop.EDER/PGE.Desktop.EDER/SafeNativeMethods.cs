using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Contains native methods imported from unmanaged dlls.
    /// NOTE: It is generally considered bad form to expose imported methods outside of this class.
    /// http://msdn.microsoft.com/en-us/library/ms182161(v=vs.100).aspx
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {

        internal const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        internal const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        internal const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        internal const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

        /// <summary>
        /// Retrieves the platform this process is running on.
        /// </summary>
        /// <returns>An instance of the Platform enum.</returns>
        public static Platform GetPlatform()
        {
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();

            if (System.Environment.OSVersion.Version.Major > 5
                || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor >= 1))
            {
                GetNativeSystemInfo(ref sysInfo);
            }
            else
            {
                GetSystemInfo(ref sysInfo);
            }

            switch (sysInfo.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_IA64:
                case PROCESSOR_ARCHITECTURE_AMD64:
                case PROCESSOR_ARCHITECTURE_INTEL:
                    return (Platform)sysInfo.wProcessorArchitecture;
                default:
                    return Platform.Unknown;
            }
        }

        /// <summary>
        /// Determines whether the specified process is running under WOW64.
        /// </summary>
        /// <param name="hProcess">A handle to the process. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <param name="wow64Process">
        /// A pointer to a value that is set to TRUE if the process is running under WOW64.
        /// If the process is running under 32-bit Windows, the value is set to FALSE.
        /// If the process is a 64-bit application running under 64-bit Windows, the value is also set to FALSE.</param>
        /// <returns>
        /// Returns true if the function succeeds; otherwise false.
        /// If the function fails, the return value is zero. To get extended error information, pinvoke GetLastError.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        /// <summary>
        /// Retrieves information about the current system to an application running under WOW64.
        /// If the function is called from a 64-bit application, it is equivalent to the GetSystemInfo function.
        /// 
        /// Equivalent to:
        /// pinvoke void WINAPI GetNativeSystemInfo(_Out_  LPSYSTEM_INFO lpSystemInfo);
        /// </summary>
        /// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that receives the information.</param>
        [DllImport("kernel32.dll")]
        internal static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        /// <summary>
        /// Retrieves information about the current system.
        /// To retrieve accurate information for an application running on WOW64, call the GetNativeSystemInfo function.
        /// 
        /// Equivalent to:
        /// void WINAPI GetSystemInfo(_Out_  LPSYSTEM_INFO lpSystemInfo);
        /// </summary>
        /// <param name="lpSystemInfo"></param>
        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        #region Nested types
        
        public enum Platform : ushort
        {
            /// <summary>
            /// Equivalent to
            /// #define PROCESSOR_ARCHITECTURE_INTEL 0
            /// </summary>
            X86 = 0,
            /// <summary>
            /// Equivalent to
            /// #define PROCESSOR_ARCHITECTURE_AMD64 9
            /// </summary>
            X64 = 9,
            /// <summary>
            /// Equivalent to
            /// #define PROCESSOR_ARCHITECTURE_IA64 6
            /// </summary>
            IA64 = 6,
            /// <summary>
            /// Equivalent to
            /// #define PROCESSOR_ARCHITECTURE_UNKNOWN 0xffff
            /// </summary>
            Unknown = 0xffff
        }

        /// <summary>
        /// Contains information about the current computer system. This includes the architecture and type of the processor,
        /// the number of processors in the system, the page size, and other such information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            /// <summary>
            /// The processor architecture of the installed operating system. This member can be one of the following values.
            /// </summary>
            public ushort wProcessorArchitecture;

            /// <summary>
            /// This member is reserved for future use.
            /// </summary>
            public ushort wReserved;

            /// <summary>
            /// The page size and the granularity of page protection and commitment. This is the page size used by the VirtualAlloc function.
            /// </summary>
            public uint dwPageSize;

            /// <summary>
            /// A pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs).
            /// </summary>
            public System.IntPtr lpMinimumApplicationAddress;

            /// <summary>
            /// A pointer to the highest memory address accessible to applications and DLLs.
            /// </summary>
            public System.IntPtr lpMaximumApplicationAddress;

            /// <summary>
            /// A mask representing the set of processors configured into the system. Bit 0 is processor 0; bit 31 is processor 31.
            /// </summary>
            public System.UIntPtr dwActiveProcessorMask;

            /// <summary>
            /// The number of logical processors in the current group. To retrieve this value, use the GetLogicalProcessorInformation function.
            /// </summary>
            public uint dwNumberOfProcessors;

            /// <summary>
            /// An obsolete member that is retained for compatibility. Use the wProcessorArchitecture, wProcessorLevel, and wProcessorRevision
            /// members to determine the type of processor.
            /// </summary>
            public uint dwProcessorType;

            /// <summary>
            /// The granularity for the starting address at which virtual memory can be allocated. For more information, see VirtualAlloc.
            /// </summary>
            public uint dwAllocationGranularity;

            /// <summary>
            /// The architecture-dependent processor level. It should be used only for display purposes. To determine the feature set of a
            /// processor, use the IsProcessorFeaturePresent function.
            /// </summary>
            public ushort wProcessorLevel;

            /// <summary>
            /// The architecture-dependent processor revision. The following table shows how the revision value is assembled for each type of
            /// processor architecture.
            /// </summary>
            public ushort wProcessorRevision;
        };

        #endregion
    }
}

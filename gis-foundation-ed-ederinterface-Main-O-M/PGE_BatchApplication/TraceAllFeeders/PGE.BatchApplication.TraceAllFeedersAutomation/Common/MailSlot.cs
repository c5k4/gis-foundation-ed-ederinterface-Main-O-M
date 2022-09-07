using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace PGE.BatchApplication.TraceAllFeeders.Common
{
    public class MailSlot
    {
        private static string MailSlotName = "";
        private SafeMailslotHandle mailHandle = null;

        public MailSlot(string mailSlotName)
        {
            MailSlotName = mailSlotName;
            MailSlot_SecuritAttribs sa = null;
            sa = CreateMailslotSecurity();

            // Create our mailslot.
            mailHandle = CreateMailslot(
                MailSlotName,               // Name of our mailslot
                0,                          // Defines the message does not have a max size
                -1,                         // Continues to wait for a message
                sa                          // Mailslot security attributes
                );

            if (mailHandle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }

        /*
        public static void CreateMailSlots()
        {
            MailSlot_SecuritAttribs sa = null;
            sa = CreateMailslotSecurity();

            // Create our mailslot.
            reconcileMailSlotHandle = CreateMailslot(
                Common.ReconcileMailSlot,               // Name of our mailslot
                0,                          // Defines the message does not have a max size
                -1,                         // Continues to wait for a message
                sa                          // Mailslot security attributes
                );

            if (reconcileMailSlotHandle.IsInvalid)
            {
                throw new Win32Exception();
            }

            // Create our mailslot.
            TraceAllMailSlotHandle = CreateMailslot(
                Common.TraceAllListMailSlot,               // Name of our mailslot
                0,                          // Defines the message does not have a max size
                -1,                         // Continues to wait for a message
                sa                          // Mailslot security attributes
                );

            if (TraceAllMailSlotHandle.IsInvalid)
            {
                throw new Win32Exception();
            }

            // Create our mailslot.
            finishedMailSlotHandle = CreateMailslot(
                Common.TraceAllFinishedMailSlot,               // Name of our mailslot
                0,                          // Defines the message does not have a max size
                -1,                         // Continues to wait for a message
                sa                          // Mailslot security attributes
                );

            if (finishedMailSlotHandle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }
         * */

        public static void WriteToMailSlot(string MailSlotName, string message)
        {
            SafeMailslotHandle mailSlotHandle = null;

            try
            {
                // Try to open the mailslot with the write access.
                mailSlotHandle = CreateFile(
                    MailSlotName,                           // The name of the mailslot
                    0x40000000,                             // Write access 
                    0x00000001,                             // Share mode
                    IntPtr.Zero,                            // Default security attributes
                    3,                                      // Opens existing mailslot
                    0,                                      // No other attributes set
                    IntPtr.Zero                             // No template file
                    );
                if (mailSlotHandle.IsInvalid)
                {
                    throw new Win32Exception();
                }

                Console.WriteLine("The mailslot ({0}) is opened.", MailSlotName);

                // Write messages to the mailslot.

                // Append '\0' at the end of each message considering the native C++ 
                // Mailslot server (CppMailslotServer).
                WriteMailslot(mailSlotHandle, message);

            }
            catch (Win32Exception ex)
            {
                Console.WriteLine("The client throws the error: {0}", ex.Message);
            }
            finally
            {
                if (mailSlotHandle != null)
                {
                    mailSlotHandle.Close();
                    mailSlotHandle = null;
                }
            }
        }

        private static void WriteMailslot(SafeMailslotHandle hMailslot, string message)
        {
            int cbMessageBytes = 0;         // Message size in bytes
            int cbBytesWritten = 0;         // Number of bytes written to the slot

            byte[] bMessage = Encoding.Unicode.GetBytes(message);
            cbMessageBytes = bMessage.Length;

            bool succeeded = WriteFile(
                hMailslot,                  // Handle to the mailslot
                bMessage,                   // Message to be written
                cbMessageBytes,             // Number of bytes to write
                out cbBytesWritten,         // Number of bytes written
                IntPtr.Zero                 // Not overlapped
                );
            if (!succeeded || cbMessageBytes != cbBytesWritten)
            {
                Console.WriteLine("WriteFile failed w/err 0x{0:X}",
                    Marshal.GetLastWin32Error());
            }
            else
            {
                Console.WriteLine("The message \"{0}\" is written to the slot",
                    message);
            }
        }

        public List<string> ReadMailslotMessages()
        {

            List<string> messages = new List<string>();
            int cbMessageBytes = 0;         // Size of the message in bytes
            int cbBytesRead = 0;            // Number of bytes read from the mailslot
            int cMessages = 0;              // Number of messages in the slot
            int nMessageId = 0;             // Message ID

            bool succeeded = false;

            // Check for the number of messages in the mailslot.
            succeeded = GetMailslotInfo(
                mailHandle,                  // Handle of the mailslot
                IntPtr.Zero,                // No maximum message size 
                out cbMessageBytes,         // Size of next message 
                out cMessages,              // Number of messages 
                IntPtr.Zero                 // No read time-out
                );
            if (!succeeded)
            {
                Console.WriteLine("GetMailslotInfo failed w/err 0x{0:X}",
                    Marshal.GetLastWin32Error());
                return messages;
            }

            //If message is -1 there is no messages
            if (cbMessageBytes == -1)
            {
                //No new messages
                return messages;
            }

            // Retrieve the messages one by one from the mailslot.
            while (cMessages != 0)
            {
                nMessageId++;

                // Declare a byte array to fetch the data
                byte[] bBuffer = new byte[cbMessageBytes];
                succeeded = ReadFile(
                    mailHandle,              // Handle of mailslot
                    bBuffer,                // Buffer to receive data
                    cbMessageBytes,         // Size of buffer in bytes
                    out cbBytesRead,        // Number of bytes read from mailslot
                    IntPtr.Zero             // Not overlapped I/O
                    );
                if (!succeeded)
                {
                    Console.WriteLine("ReadFile failed w/err 0x{0:X}",
                        Marshal.GetLastWin32Error());
                    break;
                }

                // Display the message. 
                messages.Add(Encoding.Unicode.GetString(bBuffer));

                // Get the current number of un-read messages in the slot. The number
                // may not equal the initial message number because new messages may 
                // arrive while we are reading the items in the slot.
                succeeded = GetMailslotInfo(
                    mailHandle,              // Handle of the mailslot
                    IntPtr.Zero,            // No maximum message size 
                    out cbMessageBytes,     // Size of next message 
                    out cMessages,          // Number of messages 
                    IntPtr.Zero             // No read time-out 
                    );
                if (!succeeded)
                {
                    Console.WriteLine("GetMailslotInfo failed w/err 0x{0:X}",
                        Marshal.GetLastWin32Error());
                    break;
                }
            }

            return messages;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(SafeMailslotHandle handle,
            byte[] bytes, int numBytesToWrite, out int numBytesWritten,
            IntPtr overlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeMailslotHandle CreateFile(string fileName,
            uint desiredAccess, uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            int flagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(SafeMailslotHandle handle,
            byte[] bytes, int numBytesToRead, out int numBytesRead,
            IntPtr overlapped);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMailslotInfo(SafeMailslotHandle hMailslot,
            IntPtr lpMaxMessageSize, out int lpNextSize, out int lpMessageCount,
            IntPtr lpReadTimeout);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeMailslotHandle CreateMailslot(string mailslotName,
            uint nMaxMessageSize, int lReadTimeout,
            MailSlot_SecuritAttribs securityAttributes);

        static MailSlot_SecuritAttribs CreateMailslotSecurity()
        {
            // Define the SDDL for the security descriptor.
            string sddl = "D:" +        // Discretionary ACL
                "(A;OICI;GRGW;;;AU)" +  // Allow read/write to authenticated users
                "(A;OICI;GA;;;BA)";     // Allow full control to administrators

            SafeLocalMemHandle pSecurityDescriptor = null;
            if (!ConvertStringSecurityDescriptorToSecurityDescriptor(
                sddl, 1, out pSecurityDescriptor, IntPtr.Zero))
            {
                throw new Win32Exception();
            }

            MailSlot_SecuritAttribs sa = new MailSlot_SecuritAttribs();
            sa.nLength = Marshal.SizeOf(sa);
            sa.lpSecurityDescriptor = pSecurityDescriptor;
            sa.bInheritHandle = false;
            return sa;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(
            string sddlSecurityDescriptor, int sddlRevision,
            out SafeLocalMemHandle pSecurityDescriptor,
            IntPtr securityDescriptorSize);

    }



    [StructLayout(LayoutKind.Sequential)]
    public class MailSlot_SecuritAttribs
    {
        public int nLength;
        public SafeLocalMemHandle lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [SuppressUnmanagedCodeSecurity,
    HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class SafeLocalMemHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeLocalMemHandle()
            : base(true)
        {
        }

        public SafeLocalMemHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(preexistingHandle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
        DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        protected override bool ReleaseHandle()
        {
            return (LocalFree(base.handle) == IntPtr.Zero);
        }
    }

    [SecurityCritical(SecurityCriticalScope.Everything),
    HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true),
    SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public class SafeMailslotHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeMailslotHandle()
            : base(true)
        {
        }

        public SafeMailslotHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(preexistingHandle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
        DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(base.handle);
        }
    }
}

using System;
using System.Runtime.InteropServices;

class DumpNetSession {

	// https://docs.microsoft.com/en-us/windows/desktop/api/lmshare/nf-lmshare-netsessionenum
	// https://msdn.microsoft.com/en-us/library/cc247273.aspx
	// https://pinvoke.net/default.aspx/netapi32/NetSessionEnum.html

	static void Main(string[] args) {

		var serverName = "";

		if (args.Length == 0) {
			Console.Write("Server name: ");
			serverName = Console.ReadLine();
		} else if (args.Length == 1) {
			serverName = args[0];
		} else {
			Console.Write("DumpSession.exe [server name]");
			Environment.Exit(1);
		}

		var entriesRead = 0;
		var totalEntries = 0;
		var level = 10;
		var ptrInfo = IntPtr.Zero;
		var resumeHandle = IntPtr.Zero;

		var result = NetSessionEnum(serverName, null, null, level, out ptrInfo, MAX_PREFERRED_LENGTH, out entriesRead, out totalEntries, ref resumeHandle);

		if (result != NET_API_STATUS.NERR_Success) {
			Console.WriteLine("ERROR(0x{0:X}, {0}): {1}", (int)result, result);
		}

		var entries = new SESSION_INFO_10[entriesRead];
		var iter = ptrInfo;

		for (var i = 0; i < entriesRead; i++) {
			entries[i] = (SESSION_INFO_10)Marshal.PtrToStructure(iter, typeof(SESSION_INFO_10));
			iter = (IntPtr)(iter.ToInt64() + Marshal.SizeOf(typeof(SESSION_INFO_10)));
		}

		NetApiBufferFree(ptrInfo);

		foreach (var entry in entries) {
			var cname = entry.sesi10_cname;
			var username = entry.sesi10_username;
			var time = entry.sesi10_time;
			var idle_time = entry.sesi10_time;

			Console.WriteLine("cname: " +cname +", username: " +username +", time: " +time +", idle_time: " +idle_time);
		}
	}

	[DllImport("NetAPI32.dll")]
	private static extern NET_API_STATUS NetSessionEnum(
		[MarshalAs(UnmanagedType.LPWStr)] string ServerName,
		[MarshalAs(UnmanagedType.LPWStr)] string UncClientName,
		[MarshalAs(UnmanagedType.LPWStr)] string UserName,
		int Level,
		out IntPtr bufptr,
		int prefmaxlen,
		out int entriesread,
		out int totalentries,
		ref IntPtr resume_handle);

	[DllImport("NetAPI32.dll")]
	private static extern int NetApiBufferFree(
		IntPtr Buff);

	[StructLayout(LayoutKind.Sequential)]
	public struct SESSION_INFO_10 {
		[MarshalAs(UnmanagedType.LPWStr)] public string sesi10_cname;
		[MarshalAs(UnmanagedType.LPWStr)] public string sesi10_username;
		public uint sesi10_time;
		public uint sesi10_idle_time;
	}

	public const int MAX_PREFERRED_LENGTH = -1;

	public enum NET_API_STATUS {
		NERR_Success = 0x0,
		ERROR_ACCESS_DENIED = 0x5,
		ERROR_NOT_ENOUGH_MEMORY = 0x8,
		ERROR_BAD_NETPATH = 0x35,
		ERROR_NETWORK_BUSY = 0x36,
		ERROR_INVALID_PARAMETER = 0x57,
		ERROR_INVALID_LEVEL = 0x7C,
		ERROR_MORE_DATA = 0xEA,
		NERR_UserNotFound = 0x8AD,
		NERR_ClientNameNotFound = 0x908,
		NERR_InvalidComputer = 0x92F
	}
}

using System.Runtime.InteropServices;

namespace WHAT.Utility;

public static class NativeFileDialog {
	[ComImport]
	[Guid("42F85136-DB7E-439C-85F1-E4075D135FC8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IFileOpenDialog {
		[PreserveSig]
		int Show([In] IntPtr hwndOwner);

		[PreserveSig]
		int SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

		[PreserveSig]
		int SetFileTypeIndex([In] uint iFileType);

		[PreserveSig]
		int GetFileTypeIndex(out uint piFileType);

		[PreserveSig]
		int Advise([In] IntPtr pfde, out uint pdwCookie);

		[PreserveSig]
		int Unadvise([In] uint dwCookie);

		[PreserveSig]
		int SetOptions([In] uint fos);

		[PreserveSig]
		int GetOptions(out uint pfos);

		[PreserveSig]
		int SetDefaultFolder([In] IShellItem psi);

		[PreserveSig]
		int SetFolder([In] IShellItem psi);

		[PreserveSig]
		int GetFolder(out IShellItem ppsi);

		[PreserveSig]
		int GetCurrentSelection(out IShellItem ppsi);

		[PreserveSig]
		int SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

		[PreserveSig]
		int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

		[PreserveSig]
		int SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

		[PreserveSig]
		int SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

		[PreserveSig]
		int SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

		[PreserveSig]
		int GetResult(out IShellItem ppsi);

		[PreserveSig]
		int AddPlace([In] IShellItem psi, int fdap);

		[PreserveSig]
		int SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

		[PreserveSig]
		int Close(int hr);

		[PreserveSig]
		int SetClientGuid([In] ref Guid guid);

		[PreserveSig]
		int ClearClientData();

		[PreserveSig]
		int SetFilter([In] IntPtr pFilter);

		[PreserveSig]
		int GetResults(out IShellItemArray ppenum);

		[PreserveSig]
		int GetSelectedItems(out IShellItemArray ppenum);
	}

	[ComImport]
	[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IShellItem {
		[PreserveSig]
		int BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);

		[PreserveSig]
		int GetParent(out IShellItem ppsi);

		[PreserveSig]
		int GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

		[PreserveSig]
		int GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

		[PreserveSig]
		int Compare([In] IShellItem psi, [In] uint hint, out int piOrder);
	}

	[ComImport]
	[Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IShellItemArray {
		[PreserveSig]
		int BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut);

		[PreserveSig]
		int GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);

		[PreserveSig]
		int GetPropertyDescriptionList(IntPtr keyType, ref Guid riid, out IntPtr ppv);

		[PreserveSig]
		int GetAttributes(int AttribFlags, uint sfgaoMask, out uint psfgaoAttribs);

		[PreserveSig]
		int GetCount(out uint pdwNumItems);

		[PreserveSig]
		int GetItemAt([In] uint dwIndex, out IShellItem ppsi);

		[PreserveSig]
		int EnumItems(out IntPtr ppenumShellItems);
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private struct COMDLG_FILTERSPEC {
		[MarshalAs(UnmanagedType.LPWStr)] public string pszName;
		[MarshalAs(UnmanagedType.LPWStr)] public string pszSpec;
	}

	private enum SIGDN : uint {
		FILESYSPATH = 0x80058000,
	}

	[ComImport]
	[Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
	private class FileOpenDialogCoClass { }

	private const int HRESULT_CANCELLED = unchecked((int)0x800704C7);
	private const int S_OK = 0;

	public static string? ShowAudioFilePicker(IntPtr ownerHwnd = 0) {
		IFileOpenDialog dialog = (IFileOpenDialog)new FileOpenDialogCoClass();

		try {
			COMDLG_FILTERSPEC[] filters = [
				new() { pszName = "WAV File", pszSpec = "*.wav" },
				new() { pszName = "MP3 File", pszSpec = "*.mp3" },
				new() { pszName = "OGG File", pszSpec = "*.ogg" },
				new() { pszName = "All Files", pszSpec = "*.*" }
			];

			SetFileTypesManual(dialog, filters);

			dialog.SetFileTypeIndex(1);
			dialog.SetTitle("Select an Audio File.");
			dialog.SetDefaultExtension("mp3");

			int hr = dialog.Show(ownerHwnd);
			if (hr == HRESULT_CANCELLED)
				return null;
			if (hr != S_OK)
				Marshal.ThrowExceptionForHR(hr);

			dialog.GetResult(out IShellItem item);
			item.GetDisplayName(SIGDN.FILESYSPATH, out string path);
			return path;
		} finally {
			Marshal.ReleaseComObject(dialog);
		}
	}

	private static void SetFileTypesManual(IFileOpenDialog dialog, COMDLG_FILTERSPEC[] filters) {
		int count = filters.Length;
		int structSize = Marshal.SizeOf<COMDLG_FILTERSPEC>();
		IntPtr pArray = Marshal.AllocCoTaskMem(structSize * count);

		IntPtr[] pinnedStrings = new IntPtr[count * 2];

		try {
			for (int i = 0; i < count; i++) {
				IntPtr pName = Marshal.StringToCoTaskMemUni(filters[i].pszName);
				IntPtr pSpec = Marshal.StringToCoTaskMemUni(filters[i].pszSpec);
				pinnedStrings[i * 2] = pName;
				pinnedStrings[i * 2 + 1] = pSpec;

				IntPtr slot = pArray + i * structSize;
				Marshal.WriteIntPtr(slot, 0, pName);
				Marshal.WriteIntPtr(slot, IntPtr.Size, pSpec);
			}

			SetFileTypesViaPointer(dialog, (uint)count, pArray);
		} finally {
			foreach (IntPtr ptr in pinnedStrings)
				if (ptr != IntPtr.Zero)
					Marshal.FreeCoTaskMem(ptr);

			Marshal.FreeCoTaskMem(pArray);
		}
	}

	private static unsafe void SetFileTypesViaPointer(IFileOpenDialog dialog, uint count, IntPtr pFilterArray) {
		IntPtr pUnk = Marshal.GetIUnknownForObject(dialog);

		try {
			Guid iid = new("42F85136-DB7E-439C-85F1-E4075D135FC8");
			Marshal.QueryInterface(pUnk, ref iid, out IntPtr pDialog);

			try {
				IntPtr* vtable = *(IntPtr**)pDialog;
				delegate* unmanaged[Stdcall]<IntPtr, uint, IntPtr, int> setFileTypes = (delegate* unmanaged[Stdcall]<IntPtr, uint, IntPtr, int>)vtable[4];

				int hr = setFileTypes(pDialog, count, pFilterArray);
				if (hr != S_OK)
					Marshal.ThrowExceptionForHR(hr);
			} finally {
				Marshal.Release(pDialog);
			}
		} finally {
			Marshal.Release(pUnk);
		}
	}
}
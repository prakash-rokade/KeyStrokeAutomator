using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PInvoke;

namespace KeyStrokeAutomator.WinApi
{
	public static class KeyboardEmulator
	{
		private static Dictionary<string, User32.VirtualKey> pairs = new Dictionary<string, User32.VirtualKey>()
		{
			{"{ENTER}", User32.VirtualKey.VK_RETURN },
			{"{ESC}", User32.VirtualKey.VK_ESCAPE },
			{"{TAB}", User32.VirtualKey.VK_TAB }
		};

		/// <summary>
		/// Sends text directly to the topmost window, as if it was entered by the user.
		/// </summary>
		/// <param name="text">The text to be sent to the active window.</param>
		public static void SendText(string text)
		{
			var inputs = new List<User32.INPUT>();
			foreach (var ch in text)
			{
				var down = Input.FromCharacter(ch, KeyDirection.Down);
				var up = Input.FromCharacter(ch, KeyDirection.Up);
				inputs.Add(down);
				inputs.Add(up);
			}
			SendInputs(inputs);
		}

		public static void SendTab()
		{
			var inputs = new List<User32.INPUT>
			{
				Input.FromKeyCode(User32.VirtualKey.VK_TAB, KeyDirection.Down),
				Input.FromKeyCode(User32.VirtualKey.VK_TAB, KeyDirection.Up),
			};
			SendInputs(inputs);
		}

		public static void SendEnter()
		{
			var inputs = new List<User32.INPUT>
			{
				Input.FromKeyCode(User32.VirtualKey.VK_RETURN, KeyDirection.Down),
				Input.FromKeyCode(User32.VirtualKey.VK_RETURN, KeyDirection.Up),
			};
			SendInputs(inputs);
		}

		public static void SendSpecialKey(User32.VirtualKey virtualKey)
		{
			var inputs = new List<User32.INPUT>
			{
				Input.FromKeyCode(virtualKey, KeyDirection.Down),
				Input.FromKeyCode(virtualKey, KeyDirection.Up),
			};
			SendInputs(inputs);
		}

		public static void SendInputs(List<User32.INPUT> inputs)
		{
			var size = Marshal.SizeOf(typeof(User32.INPUT));
			var success = User32.SendInput(inputs.Count, inputs.ToArray(), size);
			if (success != inputs.Count)
			{
				var exc = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				throw exc;
			}
		}

		public static bool TryGetVirtualKey(string key, out User32.VirtualKey value)
		{
			if (pairs.ContainsKey(key))
			{
				value = pairs[key];
				return true;
			}

			value = User32.VirtualKey.VK_NO_KEY;
			return false;
		}

		public static class Input
		{
			public static User32.INPUT FromKeyCode(User32.VirtualKey keyCode, KeyDirection direction)
			{
				User32.KEYEVENTF flags = 0;
				if (direction == KeyDirection.Up)
				{
					flags |= User32.KEYEVENTF.KEYEVENTF_KEYUP;
				}

				return new User32.INPUT
				{

					type = User32.InputType.INPUT_KEYBOARD,
					Inputs = new User32.INPUT.InputUnion
					{
						ki = new User32.KEYBDINPUT
						{
							wVk = keyCode,
							wScan = User32.ScanCode.NONAME,
							dwFlags = flags,
							time = 0,
							dwExtraInfo_IntPtr = IntPtr.Zero,
						}
					}
				};
			}

			public static User32.INPUT FromCharacter(char character, KeyDirection direction)
			{
				var flags = User32.KEYEVENTF.KEYEVENTF_UNICODE;

				// If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
				// we need to include the ExtendedKey flag in the Flags property.
				if ((character & 0xFF00) == 0xE000)
				{
					flags |= User32.KEYEVENTF.KEYEVENTF_EXTENDED_KEY;
				}
				if (direction == KeyDirection.Up)
				{
					flags |= User32.KEYEVENTF.KEYEVENTF_KEYUP;
				}

				return new User32.INPUT
				{

					type = User32.InputType.INPUT_KEYBOARD,
					Inputs = new User32.INPUT.InputUnion
					{
						ki = new User32.KEYBDINPUT
						{
							wVk = User32.VirtualKey.VK_NO_KEY,
							wScan = (User32.ScanCode) character,
							dwFlags = flags,
							time = 0,
							dwExtraInfo_IntPtr = IntPtr.Zero,
						}
					}
				};
			}
		}

		public enum KeyDirection
		{
			Up,
			Down,
		}
	}
}

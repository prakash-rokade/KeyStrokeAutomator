using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using KeyStrokeAutomator.WinApi;
using NHotkey;
using NHotkey.Wpf;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Markup;
using System.Text;
using System.Diagnostics;

namespace KeyStrokeAutomator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private bool _isAppProtected = false;

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private ObservableCollection<Entry> _entries = new ObservableCollection<Entry>();
		public ObservableCollection<Entry> Entries
		{
			get
			{
				return _entries;
			}

			set
			{
				if (value != _entries)
				{
					_entries = value;
					NotifyPropertyChanged();
				}
			}
		}

		private bool _hasVisibleEntries = true;
		public bool HasVisibleEntries
		{
			get
			{
				return _hasVisibleEntries;
			}

			set
			{
				if (value != _hasVisibleEntries)
				{
					_hasVisibleEntries = value;
					NotifyPropertyChanged();
				}
			}
		}

		public MainWindow()
        {
			DataContext = this;

			LoadEntries();
			InitializeComponent();
			RegisterHotkey();
			ProcessWatcher.StartWatch();

			HeaderRef.QueryInput.TextChanged += QueryInput_TextChanged;
			HeaderRef.QueryInput.PreviewKeyDown += QueryInput_PreviewKeyDown;
			Activated += HandleWindowActivated;
		}

		private void LoadEntries()
		{
			EntryCollection entryCollection = Utils.GetEntries();			

			//XmlSerializer employeeSerializer = new XmlSerializer(typeof(EntryCollection));
			//StringBuilder sbData =  new StringBuilder();
			//StringWriter swWriter = new StringWriter(sbData);
			//employeeSerializer.Serialize(swWriter, entryCollection);			
			//File.WriteAllText("data.xml", sbData.ToString());
			Entries = new ObservableCollection<Entry>(entryCollection.Entries);
			HasVisibleEntries = Entries.Count() > 0;
		}

		private void RegisterHotkey()
		{
			var toggleGesture = new KeyGesture(Key.Z, ModifierKeys.Windows | ModifierKeys.Shift);
			HotkeyManager.Current.AddOrReplace("Toggle", toggleGesture, HandleToggle);
		}

		private void HandleToggle(object sender, HotkeyEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				Visibility = Visibility.Hidden;
			}
			else
			{
				if (Entries.Count <= 0) return;

				CenterWindowOnScreen();
				Visibility = Visibility.Visible;
				Activate();
				HeaderRef.QueryInput.Focus();
			}
		}

		private void CenterWindowOnScreen()
		{
			if (NativeApi.GetCursorPos(out var p))
			{
				var currentScreen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(p.X, p.Y)).WorkingArea;

				var wih = new WindowInteropHelper(this);
				var hWnd = wih.Handle;
				_ = NativeApi.MoveWindow(hWnd, p.X, p.Y, (int)this.Width, (int)this.Height, false);
			}						
		}

		public void CloseWindowKeybinding(object sender, object e)
		{
			Visibility = Visibility.Hidden;
		}

		private void OnItemMouseDown(object sender, MouseButtonEventArgs e)
		{
			HandleSelectedItem(sender);
		}

		private void HandleSelectedItem(object sender)
		{
			var view = sender as System.Windows.Controls.ListView;

			if (view.SelectedItem is Entry selected && selected.IsVisible)
			{
				ProcessWatcher.BringMainWindowToFront();
				if(selected.EntryType == EntryType.SendInput)
				{
					ProcessSendInput(selected);
				}
				else
				{
					ProcessRunCommand(selected);
				}
				Visibility = Visibility.Hidden;
			}
		}
		private void ProcessSendInput(Entry entry)
		{
			foreach(var line in entry.Content)
			{
				if(KeyboardEmulator.TryGetVirtualKey(line, out var key))
				{
					KeyboardEmulator.SendSpecialKey(key);
				}
				else
				{
					KeyboardEmulator.SendText(line);
				}
			}
		}
		private void ProcessRunCommand(Entry entry)
		{
			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = entry.Content[0];
			info.Arguments = entry.Content[1];

			Process.Start(info);
		}
		private void QueryInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			int visibleCount = 0;

			foreach (var entry in Entries)
			{
				if (HeaderRef.QueryInput.Text.Trim() == "")
				{
					visibleCount++;
					entry.IsVisible = true;
					continue;
				}
				entry.IsVisible = entry.DisplayName.IndexOf(HeaderRef.QueryInput.Text, StringComparison.OrdinalIgnoreCase) >= 0;
				visibleCount = entry.IsVisible ? visibleCount + 1 : visibleCount;
			}

			HasVisibleEntries = visibleCount > 0;

			if (uiItems.SelectedItem == null && Entries.Count() > 0)
			{
				uiItems.SelectedIndex = 0;
				uiItems.ScrollIntoView(uiItems.SelectedItem);
			}
		}

		private void QueryInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (Entries.Count() == 0)
			{
				return;
			}

			Entry selected = uiItems.SelectedItem as Entry;
			IList<Entry> visibleEntries = Entries.Where(item => item.IsVisible == true).ToList();
			int selectedVisibleIndex = visibleEntries.IndexOf(selected);

			switch (e.Key)
			{
				case Key.Delete:
					if (Keyboard.IsKeyDown(Key.LeftCtrl) && selected != null)
					{
						//int lastIndex = selectedVisibleIndex;
						//db.Entries.Remove(selected);
						//db.SaveChangesAsync();
						//this.Entries.RemoveAt(uiItems.SelectedIndex);

						//visibleEntries = this.Entries.Where(item => item.IsVisible == true).ToList();

						//if (visibleEntries.Count() > 0)
						//{
						//	uiItems.SelectedItem = visibleEntries.ElementAt(Math.Min(lastIndex, visibleEntries.Count() - 1));
						//	uiItems.ScrollIntoView(uiItems.SelectedItem);
						//}
					}
					break;
				case Key.PageDown:
					uiItems.SelectedItem = visibleEntries.ElementAt(Math.Min(selectedVisibleIndex + 7, visibleEntries.Count() - 1));
					uiItems.ScrollIntoView(uiItems.SelectedItem);
					break;
				case Key.PageUp:
					uiItems.SelectedItem = visibleEntries.ElementAt(Math.Max(selectedVisibleIndex - 7, 0));
					uiItems.ScrollIntoView(uiItems.SelectedItem);
					break;
				case Key.Down:
					uiItems.SelectedItem = visibleEntries.ElementAt((selectedVisibleIndex + 1) % visibleEntries.Count());
					uiItems.ScrollIntoView(uiItems.SelectedItem);
					break;
				case Key.Up:
					int v = (selectedVisibleIndex - 1);
					uiItems.SelectedItem = visibleEntries.ElementAt(v < 0 ? visibleEntries.Count() - 1 : v); ;
					uiItems.ScrollIntoView(uiItems.SelectedItem);
					break;
				case Key.Enter:
					HandleSelectedItem(uiItems);
					break;
				default:
					break;
			}
		}

		private void HandleWindowActivated(object sender, EventArgs e)
		{
			if (Entries.Count() == 0)
			{
				return;
			}

			if (uiItems.SelectedItem == null)
			{
				uiItems.SelectedIndex = 0;
			}

			uiItems.ScrollIntoView(uiItems.SelectedItem);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ProtectApp();
		}
		private void ProtectApp()
		{
			if (_isAppProtected) return;

			var helper = new WindowInteropHelper(this);

			const uint WDA_MONITOR = 1;
			const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
			NativeApi.SetWindowDisplayAffinity(helper.Handle, WDA_EXCLUDEFROMCAPTURE);

			_isAppProtected = true;
		}
	}
}

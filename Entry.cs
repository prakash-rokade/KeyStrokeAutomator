using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Collections.Generic;

namespace KeyStrokeAutomator
{
	public enum EntryType
	{
		SendInput,
		RunCommand
	}

	public class Entry : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		private int _id;		
		public int Id
		{
			get => _id;
			set
			{
				_id = value;
				NotifyPropertyChanged();
			}
		}

		public EntryType EntryType { get; set; }

		public string[] Content { get;set; }


		private bool _isPinned = false;
		public bool IsPinned
		{
			get => _isPinned;

			set
			{
				if (value == _isPinned) return;

				_isPinned = value;
				NotifyPropertyChanged();
			}
		}

		private bool _isVisible = true;		
		public bool IsVisible
		{
			get => _isVisible;

			set
			{
				if (value == _isVisible) return;

				_isVisible = value;
				NotifyPropertyChanged();
			}
		}

		private string _displayName = String.Empty;
		public string DisplayName
		{
			get
			{
				return _displayName;
			}
			set
			{
				if (value == _displayName) return;

				_displayName = value; 
				NotifyPropertyChanged(); 
			}
		}
	}

	public class EntryCollection
	{
		public List<Entry> Entries { get; set; }
	}
}

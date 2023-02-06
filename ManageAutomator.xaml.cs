using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeyStrokeAutomator.WinApi;

namespace KeyStrokeAutomator
{
	/// <summary>
	/// Interaction logic for ManageAutomator.xaml
	/// </summary>
	public partial class ManageAutomator : Window
	{
		public ManageAutomator()
		{
			InitializeComponent();
			Loaded += ManageAutomator_Loaded;
		}

		private void ManageAutomator_Loaded(object sender, RoutedEventArgs e)
		{
			ProtectApp();
		}

		private void ProtectApp()
		{
			var helper = new WindowInteropHelper(this);			
			const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
			NativeApi.SetWindowDisplayAffinity(helper.Handle, WDA_EXCLUDEFROMCAPTURE);
		}
	}
}

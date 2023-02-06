using System.Windows;
using System.Windows.Input;

namespace KeyStrokeAutomator
{
	public class TrayViewModel
	{
		public ICommand ShowApplicationCommand => new DelegateCommand()
		{
			CommandAction = () =>
			{
				Application.Current.MainWindow.Show();
				Application.Current.MainWindow.Activate();
			}
		};

		public ICommand ExitApplicationCommand => new DelegateCommand()
		{
			CommandAction = () => Application.Current.Shutdown()
		};

		public ICommand SettingsCommand => new DelegateCommand()
		{
			CommandAction = () => 
			{  
				var manageAutomator = new ManageAutomator();
				manageAutomator.Show();
			}
		};
	}
}

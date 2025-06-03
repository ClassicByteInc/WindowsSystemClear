using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WindowsSystemClear.GUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void AddTargetItem(string path, string description)
		{
			// 替换 {UserProfile} 为当前用户的用户目录
			if (path.Contains("{UserProfile}"))
			{
				string userProfile = Environment.UserName;
				path = path.Replace("{UserProfile}", userProfile);
			}

			TargetListBox.Items.Add(new ListBoxItem
			{
				Content = new TextBlock
				{
					Text = $"{description} ({path})",
					ToolTip = path
				},
				Tag = path // Store the path in the Tag property for later use
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var fileName = System.IO.Path.GetTempFileName();
			File.WriteAllText(fileName, WindowsSystemClear.Resource.DefaultTarget);
			try
			{
				foreach (var item in TargetList.Load(fileName).Targets)
				{
					AddTargetItem(item.Key, item.Value);
				}
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show($"加载文件出错:{ex}", "错误", MessageBoxButton.YesNo, MessageBoxImage.Error);


			}
		}

		private void ChooseTargetFile(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog
			{
				Filter = "Target 文件 (*.target)|*.target|所有文件 (*.*)|*.*",
				Title = "选择 Target 文件"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string fileName = openFileDialog.FileName;
				TargetListBox.Items.Clear();
				try
				{
					foreach (var item in TargetList.Load(fileName).Targets)
					{
						AddTargetItem(item.Key, item.Value);
					}
				}
				catch (Exception ex)
				{
					System.Windows.MessageBox.Show($"加载文件出错:{ex}", "错误", MessageBoxButton.YesNo, MessageBoxImage.Error);

				}
			}
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			this.Hide();
			var newWindow = new NewTargetFileWindow();
			newWindow.ShowDialog();
			this.Show();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

		}
	}
}
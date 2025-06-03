using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Diagnostics;

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

		private TargetList? TargetList = null;

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
				TargetList = TargetList.Load(fileName);
				foreach (var item in TargetList.Targets)
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
				Filter = "Target 文件 (*.targets)|*.targets|所有文件 (*.*)|*.*",
				Title = "选择 Targets 文件"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string fileName = openFileDialog.FileName;
				TargetListBox.Items.Clear();
				try
				{
					TargetList = TargetList.Load(fileName);
					foreach (var item in TargetList.Targets)
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

		private void Log(string m)
		{
			Debug.WriteLine(m);
			Dispatcher.Invoke(() =>
			{
				if (logger != null)
				{
					logger.AppendText($"{DateTime.Now:HH:mm:ss} - {m}\n");
					logger.ScrollToEnd();
				}
				else
				{
					MessageBox.Show(m, "日志", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			});
		}

		private async void Delete(object sender, RoutedEventArgs e)
		{
			// 1. 保存原始状态
			string originalText = clearButton.Content.ToString();
			clearButton.Content = "正在清理...";
			clearButton.IsEnabled = false;

			// 2. 设置任务栏脉冲进度
			if (this.TaskbarItemInfo == null)
				this.TaskbarItemInfo = new TaskbarItemInfo();
			this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;

			// 3. 执行清理操作（不阻塞UI线程）
			await Task.Run(() =>
			{
				// 调用原有的删除逻辑
				Dispatcher.Invoke(() => Log("开始清理..."));
				DeleteCore();
				Dispatcher.Invoke(() => Log("清理完成。"));
			});

			// 4. 恢复按钮和任务栏状态
			clearButton.Content = originalText;
			clearButton.IsEnabled = true;
			this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
		}

		// 将原有的删除逻辑抽取到一个新方法
		private async void DeleteCore()
		{
			if (TargetList == null || TargetList.Targets == null)
			{
				Dispatcher.Invoke(() => Log("未加载目标列表，无法删除。"));
				return;
			}

			string targetsList = string.Join("\n", TargetList.Targets.Select(kv => $"{kv.Value} ({kv.Key})"));
			var result = Dispatcher.Invoke(() => MessageBox.Show(
				$"即将删除以下目标内容：\n\n{targetsList}\n\n是否确认删除？",
				"确认删除",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning));

			if (result != MessageBoxResult.Yes)
			{
				Dispatcher.Invoke(() => Log("用户取消了删除操作。"));
				return;
			}

			foreach (var path in TargetList.Targets.Keys)
			{
				var p = path.Replace("{UserProfile}",Environment.UserName);
				await Task.Run(() => { DeleteAll(p); });
			}
		}

		public void DeleteAll(string folderPath)
		{
			if (string.IsNullOrWhiteSpace(folderPath))
			{
				Log("文件夹路径为空或无效");
				return;
			}

			try
			{
				if (!Directory.Exists(folderPath))
				{
					Log($"指定的文件夹不存在: {folderPath}");
					return;
				}

				DeleteDirectoryContent(folderPath);
				Directory.Delete(folderPath);
				Console.WriteLine($"已成功删除文件夹 {folderPath} 及其所有内容");
			}
			catch (Exception ex)
			{
				Log($"删除文件夹 {folderPath} 时发生错误: {ex.Message}");
				throw;
			}
		}

		private void DeleteDirectoryContent(string folderPath)
		{
			try
			{
				// 删除所有文件
				string[] files = Directory.GetFiles(folderPath);
				foreach (string file in files)
				{
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						Log($"删除文件 {file} 时发生错误: {ex.Message}");
					}
				}

				// 递归删除所有子目录
				string[] subdirectories = Directory.GetDirectories(folderPath);
				foreach (string subdirectory in subdirectories)
				{
					try
					{
						DeleteAll(subdirectory);
					}
					catch (Exception ex)
					{
						Log($"删除子目录 {subdirectory} 时发生错误: {ex.Message}");
					}
				}
			}
			catch (Exception ex)
			{
				Log($"删除目录内容 {folderPath} 时发生错误: {ex.Message}");
				throw;
			}
		}
	}
}
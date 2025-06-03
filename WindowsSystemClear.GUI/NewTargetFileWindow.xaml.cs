using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WindowsSystemClear.GUI
{
	/// <summary>
	/// NewTargetFileWindow.xaml 的交互逻辑
	/// </summary>
	public partial class NewTargetFileWindow : Window
	{

		private static TargetList list = new();

		public NewTargetFileWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string userName = Environment.UserName;
			string selectedPath = targetPath.Text;
			if (!string.IsNullOrEmpty(userName))
			{
				selectedPath = selectedPath.Replace(userName, "{UserProfile}");
			}
			targetPath.Text = selectedPath;

			try
			{
				list.Targets.Add(selectedPath, descriptionBox.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("添加目标失败，请检查路径和描述是否正确。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			TargetListBox.Items.Add(new ListBoxItem
			{
				Content = new TextBlock
				{
					Text = $"{targetPath.Text}|({descriptionBox.Text})",
					ToolTip = descriptionBox.Text
				},
				Tag = descriptionBox.Text // Store the path in the Tag property for later use
			});
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (TargetListBox.SelectedItem is ListBoxItem selectedItem)
			{
				// 获取路径（假设Tag存储的是描述，路径在Content的Text中'|'前）
				string content = (selectedItem.Content as TextBlock)?.Text ?? "";
				int sepIndex = content.IndexOf('|');
				if (sepIndex > 0)
				{
					string path = content.Substring(0, sepIndex);
					// 从Targets中移除
					list.Targets.Remove(path);
				}
				// 从ListBox中移除
				TargetListBox.Items.Remove(selectedItem);
			}
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "Target List Files (*.targets)|*.targets",
				DefaultExt = ".targets",
				FileName = "targets"
			};

			if (dialog.ShowDialog() == true)
			{
				TargetList.Save(list, dialog.FileName);
				MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			var dlg = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				Title = "请选择目标文件夹"
			};

			if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			{
				string userName = Environment.UserName;
				string selectedPath = dlg.FileName;
				if (!string.IsNullOrEmpty(userName))
				{
					selectedPath = selectedPath.Replace(userName, "{UserProfile}");
				}
				targetPath.Text = selectedPath;
			}
			try
			{
				targetPath.Text = dlg.FileName;
			}
			catch (InvalidOperationException)
			{
			}

		}
	}
}

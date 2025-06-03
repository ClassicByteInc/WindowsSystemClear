namespace WindowsSystemClear
{
	[Serializable]
	public class TargetList
	{
		public Dictionary<String, String> Targets { get; set; } = [];

		public static TargetList Load(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				throw new ArgumentException("文件路径无效或文件不存在", nameof(path));

			using var stream = File.OpenRead(path);
			return System.Text.Json.JsonSerializer.Deserialize<TargetList>(stream)?? throw new InvalidOperationException("反序列化失败,请确定此文件为合法的 Targets 配置文件.");
		}

		public static void Save(TargetList targetList, string path)
		{
			if (targetList == null)
				throw new ArgumentNullException(nameof(targetList), "目标列表不能为空");
			using var stream = File.Create(path);
			System.Text.Json.JsonSerializer.Serialize(stream, targetList);
		}
	}


}
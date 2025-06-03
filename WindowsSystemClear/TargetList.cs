using System;

namespace WindowsSystemClear
{
	[Serializable]
	public class TargetList
	{
		public Dictionary<String,String> Targets { get; set; } = [];
	}
}
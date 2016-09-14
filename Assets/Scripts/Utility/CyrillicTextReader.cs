using UnityEngine;
using System.Collections;
using System.IO;  
using System.Text;
public sealed class CyrillicTextReader : StringReader
{
	public CyrillicTextReader(string text)
		: base(LoadCyrillicText(text))
	{
	}
	private static string LoadCyrillicText(string text)
	{
		return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
	}
}


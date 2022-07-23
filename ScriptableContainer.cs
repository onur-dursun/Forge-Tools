using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whaleforge.Tools
{
	[CreateAssetMenu]
	public class ScriptableContainer : ScriptableObject
	{
		public int multiplier;
		public string folderName;
		public string packageName;
		public Material mat;
	}
}
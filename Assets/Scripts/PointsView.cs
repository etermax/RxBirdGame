using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsView : MonoBehaviour 
{
	public Text label;

	public int Points { set { label.text = value.ToString("00"); } }
}

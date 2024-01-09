using UnityEngine;

// 很遗憾 Unity 经过这么多年仍然不支持在 UGUI 的 OnClick 中绑定自定义的 enum
// 所以只好给要绑定的按钮添加一些额外的属性标签
// 范例： Main Camera/MainMenu/backgroundGroup/imgBackground/btnOperation
public class NavTag : MonoBehaviour
{
	[InspectorName("Page to go / leave")]
	public NavPage page;
}
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	// 自定义的注解解析类，用于在面板上显示中文变量名
	[CustomPropertyDrawer(typeof(FieldNameAttribute))]
	public class FieldNameAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			FieldNameAttribute attr = this.attribute as FieldNameAttribute;
			if (attr.Name.Length > 0)
			{
				label.text = attr.Name;
			}
			EditorGUI.PropertyField(position, property, label);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    // 自定义的注解，用于在面板上显示中文变量名
    public class FieldNameAttribute : UnityEngine.PropertyAttribute
    {
        private string name = "";

        public string Name { get { return name; } }

        public FieldNameAttribute(string name)
        {
            this.name = name;
        }
    }
}

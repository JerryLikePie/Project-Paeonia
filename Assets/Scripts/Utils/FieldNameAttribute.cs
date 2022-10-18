using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    // �Զ����ע�⣬�������������ʾ���ı�����
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

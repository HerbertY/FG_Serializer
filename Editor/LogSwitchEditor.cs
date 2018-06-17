using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FGame.Editor
{
    public class CheckSwitchEditor : MonoBehaviour
    {
        private const string MENUPATH = "Fatoon/输出日志";

        [MenuItem(MENUPATH)]
        public static void SetMenuLogOut()
        {
            bool flag = Menu.GetChecked(MENUPATH);
            HelperTool.IsPrintLog = !flag;
            Menu.SetChecked(MENUPATH, HelperTool.IsPrintLog);
        }

        /// <summary>
        /// 保证菜单和数据一致性
        /// 特性中设置isValidateFunction 为 true,表示这个函数将在同名的itemName函数之前被调用
        /// </summary>
        [MenuItem(MENUPATH, true)]
        public static bool MenuLogOutCheck()
        {
            Menu.SetChecked(MENUPATH, HelperTool.IsPrintLog);
            return true;
        }
    }
}


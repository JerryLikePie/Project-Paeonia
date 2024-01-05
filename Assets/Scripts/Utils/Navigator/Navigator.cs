using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 页面标题（？）
[System.Serializable]
public enum NavPage
{
	Nav_Root_Main_Menu,	// 主菜单
	Nav_Operation		// 行动界面
}

// 用于保存结构化导航数据的结构体
// 用栈实现
public class NavData
{
	private Stack<NavPage> nav;

	public NavData()
	{
		nav = new Stack<NavPage>();
	}

	public void NavEnter(NavPage pageEntered)
	{
		nav.Push(pageEntered);
	}

	public void NavExit()
	{
		nav.Pop();
	}

	public NavPage GetCurrentPage()
	{
		return nav.Peek();
	}

	private static StringBuilder sb = new StringBuilder();
	public string GetFullNav()
	{
		sb.Clear();
		NavPage[] navs = nav.ToArray();
		for (int i = navs.Length - 1; i >= 0; --i)
		{
			sb.Append(navs[i].ToString());
			if (i != 0) sb.Append(" -> ");
		}
		return sb.ToString();
	}
		
}

/// <summary>
/// Navigator 用于获取当前页面在整个页树上的位置
/// 类似 MainMeun -> Operation -> ...
/// 
/// 当需要记录页面的切换时，调用 NavEnter/NavExit
/// NavEnter() 绑定在进入按钮上
/// NavExit() 绑定在退出按钮上
/// </summary>
public class Navigator : MonoBehaviour
{
	// 当前脚本所处的 Scene 的根 page
	public NavPage rootPage;

	private SceneMessager sceneMessager;

	private NavData nav;

	void Start()
	{
		sceneMessager = GameObject.Find("SceneMessager").GetComponent<SceneMessager>();
		ReloadNavigator();
	}

	public void ReloadNavigator()
	{
		nav = sceneMessager.LoadData<NavData>("Nav");

		// 如果没有获取到，说明是第一次加载游戏
		// 新建 nav 并保存
		if (nav == null)
		{
			nav = new NavData();
			nav.NavEnter(rootPage);
			sceneMessager.SaveData("Nav", nav);
		}
		// 否则看是否需要恢复到之前打开的界面，例如战役选择
		else
		{
			switch (nav.GetCurrentPage())
			{
				case NavPage.Nav_Root_Main_Menu:
					// 上一次打开的是主界面，不用管
					break;
				case NavPage.Nav_Operation:
					// 上一次打开的是战役选择

					break;
			}
		}
	}

	public void NavEnter(NavPage page)
	{
		nav.NavEnter(page);
		Debug.Log("current nav is: \n" + nav.GetFullNav());
	}

	// 无参方法，需要读取对象上的 NavTag
	public void NavEnter(NavTag tag)
	{
		Debug.Assert(tag != null);
		NavEnter(tag.page);
	}

	public void NavExit()
	{
		nav.NavExit();
		Debug.Log("current nav is: \n" + nav.GetFullNav());
	}
}
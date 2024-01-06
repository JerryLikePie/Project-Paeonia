using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// 页面标题（？）
[System.Serializable]
public enum NavPage
{
	Nav_Root_Main_Menu,		// 主菜单
	Nav_Battle,				// 主菜单-行动
	Nav_Battle_Operation1,	// 主菜单-行动-远征
	Nav_Battle_StoryCH0,	// 主菜单-行动-序章
	Nav_Battle_StoryCH1,	// 主菜单-行动-第一章
	Nav_Battle_Operation4	// 主菜单-行动-...
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

	public void Clear()
	{
		nav.Clear();
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

	// 进入某个页面对应的 button
	public USerializableDictionary<NavPage, Button> navButtons;

	private SceneMessager sceneMessager; 

	private NavData nav;

	void Start()
	{
		sceneMessager = GameObject.Find("SceneMessager").GetComponent<SceneMessager>();
		LoadAndRestore();
	}

	// 加载保存的 nav，并将界面恢复到 nav 所描述的样子
	public void LoadAndRestore()
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
		// 已有 nav，判断是否需要恢复
		else
		{
			NavPage currentPage = nav.GetCurrentPage();
			switch (currentPage)
			{
				case NavPage.Nav_Root_Main_Menu:
					break;
				case NavPage.Nav_Battle:
					// 目前不存在该情况
					break;
				case NavPage.Nav_Battle_Operation1:
				case NavPage.Nav_Battle_StoryCH0:
				case NavPage.Nav_Battle_StoryCH1:
				case NavPage.Nav_Battle_Operation4:
					nav.Clear();
					nav.NavEnter(rootPage);
					// 模拟“行动”按钮点击
					navButtons[NavPage.Nav_Battle].onClick.Invoke();
					// 模拟行动界面 行动选择按钮的点击
					navButtons[currentPage].onClick.Invoke();
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
	// see NavTag
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
/*
IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

By downloading, copying, installing or using the software you agree to this license.
If you do not agree to this license, do not download, install, copy or use the software.

    License Agreement For Kobayashi Maru Commander Open Source

Copyright (C) 2013 ~ 2017, Chih-Jen Teng(NDark) and Koguyue Entertainment, 
all rights reserved. Third party copyrights are property of their respective owners. 

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

  * Redistribution's of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer.

  * Redistribution's in binary form must reproduce the above copyright notice,
    this list of conditions and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

  * The name of Koguyue or all authors may not be used to endorse or promote products
    derived from this software without specific prior written permission.

This software is provided by the copyright holders and contributors "as is" and
any express or implied warranties, including, but not limited to, the implied
warranties of merchantability and fitness for a particular purpose are disclaimed.
In no event shall the Koguyue or all authors or contributors be liable for any direct,
indirect, incidental, special, exemplary, or consequential damages
(including, but not limited to, procurement of substitute goods or services;
loss of use, data, or profits; or business interruption) however caused
and on any theory of liability, whether in contract, strict liability,
or tort (including negligence or otherwise) arising in any way out of
the use of this software, even if advised of the possibility of such damage.  
*/
/*
@file VictoryEventManager.cs
@brief 勝敗條件管理器
@author NDark


# 掛在 GlobalSingleton 上
# 有兩個串列分別管理 勝利條件與 失敗條件
# CloseSystem() 當滿足條件時會自動切換勝利與失敗狀態 關閉必要的功能
## 關閉教學
## 關閉戰場特寫
## 關閉事件管理器
## 關閉關卡目標介面
### 關閉之前必須 檢查目前是否有勝利目標 沒有的話失敗不算輸
## 關閉能源調整
## 關閉小地圖繪圖
## 關閉攝影機跟隨
## GUIUpdate
## 關閉單位產生
## 關閉玩家控制
## 關閉玩家身上的所有聲音
## 關閉 主更新
## 關閉戰場選單
## 關閉倒數計時及經過時間
# 並且顯示對應的圖示
## 失敗時檢查如果不是真的失敗，就啟動結束的圖示 MessageCard_End 即可
## 真失敗就是失敗圖示
## 勝利就是勝利圖示


@date 20121125 by NDark 
. add class member m_VictoryConditions
. add class member m_LoseConditions
. add code for VictoryCondition_RemainingEnemyNum at Start()
. modify code to use m_VictoryConditions at CheckWin()
. remove code for VictoryCondition_RemainingEnemyNum at Start()
. modify code to use m_LoseConditions at CheckLose()
@date 20121204 by NDark . comment.
@date 20121207 by NDark . add code of close audio of main character at Win() and Lose() 
@date 20121213 by NDark 
. add class method IsWinOrLose()
. add class method CloseSystem()
@date 20121224 by NDark
. add enum element ShowBattleScore
. modify code at IsWinOrLose()
. add class method ShowBattleScore()
. add class member Debug_ToWin
. add class member Debug_ToLose
. add class method ToLose()
. add class method ToWin()
@date 20121225 by NDark . add BattleScoreManager at ShowBattleScore()
@date 20121226 by NDark 
. add code of CountDownTimeManager at CloseSystem()
. add code of ElapsedTimeManager at CloseSystem()
@date 20130123 by NDark . add code of TutorialEvent at CloseSystem().
@date 20130126 by NDark . add code of BattleEventCameraManager at CloseSystem().
@date 20130203 by NDark
. add code of ShowBattleScore() at VictoryState.VictoryState_Lose of Update()
. add code of GUI_EnergyManipulator at CloseSystem()
. add code of win/lose at ShowBattleScore()
@date 20130206 by NDark . replace m_CurrentModeSelectSceneString by m_InformationSceneEnd at Win()
@date 20130213 by NDark 
. add class member m_RealLose
. add code of check real lose at CloseSystem()
. add code of MessageCard_End 

*/
using UnityEngine;
using System.Collections.Generic;

// 目前關卡的勝利狀態
public enum VictoryState
{
	VictoryState_UnActive ,
	VictoryState_Win ,
	VictoryState_Lose ,
	ShowBattleScore ,
}

public class VictoryEventManager : MonoBehaviour {
	
	public VictoryState m_VictoryState ;
	public List< Condition > m_VictoryConditions = new List<Condition>() ;
	public List< Condition > m_LoseConditions = new List<Condition>() ;
	private bool m_RealLose = true ;
	
	public bool Debug_ToWin = false ;
	public bool Debug_ToLose = false ;
	
	public bool IsWinOrLose() 
	{
		return ( m_VictoryState != VictoryState.VictoryState_UnActive ) ;
	}
	
	public void ToLose() 
	{
		Lose() ;
		m_VictoryState = VictoryState.VictoryState_Lose ;
	}

	public void ToWin() 
	{
		Win() ;
		m_VictoryState = VictoryState.VictoryState_Win ;
	}
	
	// Use this for initialization
	void Start () 
	{
		ShowGUITexture.Show( "MessageCard_Lose" , false , true , true ) ;
		ShowGUITexture.Show( "MessageCard_Win" , false , true , true ) ;
		ShowGUITexture.Show( "MessageCard_End" , false , true , true ) ;
	}

	
	// Update is called once per frame
	void Update () 
	{
		
		switch( m_VictoryState )
		{
		case VictoryState.VictoryState_UnActive :
			if( true == Debug_ToWin )
			{
				ToWin() ;
				break ;
			}
			if( true == Debug_ToLose )
			{
				ToLose() ;
				break ;
			}			
			if( true == CheckLose() )
				m_VictoryState = VictoryState.VictoryState_Lose ;			
			else if( true == CheckWin() )
				m_VictoryState = VictoryState.VictoryState_Win ;
			break ;
			
		case VictoryState.VictoryState_Win :
			ShowBattleScore( true ) ;
			m_VictoryState = VictoryState.ShowBattleScore ;
			break ;
			
		case VictoryState.VictoryState_Lose :
			ShowBattleScore( false ) ;
			m_VictoryState = VictoryState.ShowBattleScore ;
			break ;
			
		case VictoryState.ShowBattleScore :
			break ;

		}
	}
	
	
	void Lose()
	{
		// disable some script
		CloseSystem() ;	
	}

	void Win()
	{
		// 檢察第九關勝利回到遊戲頭
		LevelGenerator levelGenerator = GlobalSingleton.GetLevelGeneratorComponent() ;
		if( null != levelGenerator )
		{
			if( -1 != levelGenerator.levelString.IndexOf( "Level09" ) )
			{
				GlobalSingleton.m_InformationSceneEnd = "Scene_Warning" ; 
			}
		}
		
		CloseSystem() ;
		
		Transform trans = this.gameObject.transform.FindChild( "BackgroundMusicObject" ) ;
		if( null != trans )
		{
			GameObject objBackgroundMusic = trans.gameObject ;
			objBackgroundMusic.GetComponent<AudioSource>().Stop() ;
		}
		
		AudioClip victory = ResourceLoad.LoadAudio( "Seven and the Doctor sing Klingon War Song-cut" ) ;
		if( null != victory )
		{
			GetComponent<AudioSource>().PlayOneShot( victory ) ;
		}
		
	}
	

	
	bool CheckLose()
	{
		foreach( Condition eCondition in m_LoseConditions )
		{
			if( false == eCondition.HasTriggered() &&
				true == eCondition.IsTrue() )
			{
				eCondition.Close() ;
				Lose() ;
				return true ;
			}
		}
		return false ;		
	}
	
	bool CheckWin() 
	{
		foreach( Condition eCondition in m_VictoryConditions )
		{
			if( false == eCondition.HasTriggered() &&
				true == eCondition.IsTrue() )
			{
				eCondition.Close() ;
				Win() ;
				return true ;
			}
		}
		return false ;
	}
	
	private void CloseSystem()
	{
		// 關閉教學 
		TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
		if( null != tutorialEvent )
		{
			tutorialEvent.CloseAll() ;
			tutorialEvent.enabled = false ;
		}
		
		// 關閉戰場特寫
		BattleEventCameraManager battleEventCameraManager = GlobalSingleton.GetBattleEventCameraManager() ;
		if( null != battleEventCameraManager )
		{
			battleEventCameraManager.Close() ;
			battleEventCameraManager.enabled = false ;
		}		
		
		// 關閉事件管理器
		UsualEventManager eventManager = GlobalSingleton.GetUsualEventManagerComponent() ;
		if( null != eventManager )
		{
			eventManager.enabled = false ;	
		}
		
		// 關閉關卡目標介面
		GameObject levelObjectiveSwitcher = GlobalSingleton.GetGUI_LevelObjectiveSwitcher() ;
		
		m_RealLose = true ;
		// 檢查是否有勝利目標,沒有的話失敗不算輸
		if( null != levelObjectiveSwitcher )
		{
			var guiTexture = levelObjectiveSwitcher.GetComponent<UnityEngine.UI.Image>() ;
			if( null != guiTexture &&
				false == guiTexture.enabled )
			{
				m_RealLose = false ;
			}		
		}
		
		ShowUGUI.Show( levelObjectiveSwitcher , false , false , false ) ;// 關閉關卡目標介面	
		ClickOnGUI_SwitchGUIObject switcher = GlobalSingleton.GetSwitchLevelObjective() ;
		if( null != switcher )
			switcher.enabled = false ;			
		
		// ## 關閉能源調整
		GUI_EnergyManipulator energyManipulator = GlobalSingleton.GetEnergyManipulator() ;
		if( null != energyManipulator )
		{
			energyManipulator.enabled = false ;
		}
		
		// draw minimap 
		GlobalSingleton.GetDrawMiniMap().enabled = false ;
		// camera follow main character
		GlobalSingleton.GetCameraFollowMainCharacter().enabled = false ;
		
		// gui update
		GlobalSingleton.GetGUIUpdateComponent().enabled = false ;
		
		// 關閉敵人產生
		GlobalSingleton.GetEnemyGeneratorComponent().enabled = false ;
		
		// 關閉玩家控制
		GlobalSingleton.GetMainCharacterControllerComponent().enabled = false ;		
		
		// 關閉更新
		GlobalSingleton.GetMainUpdateComponent().enabled = false ;
		
		// 玩家身上的聲音
		GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
		if( null != mainChar )
		{
			AudioSource [] audios = mainChar.GetComponentsInChildren<AudioSource>() ;
			foreach( AudioSource audio in audios )
			{
				audio.Stop() ;
			}
			
			// 玩家的UnitData(更新)
			UnitData unitData = mainChar.GetComponent<UnitData>() ;
			if( null != unitData )
				unitData.enabled = false ;
		}		
		
		// 關閉MENU
		GameObject battleMenuSwitcher = GlobalSingleton.GetGUI_BattleMenuSwitcher() ;
		ShowUGUI.Show( battleMenuSwitcher , false , false , false ) ;
		

		
		GameObject globalSingletonObj = GlobalSingleton.GetGlobalSingletonObj() ;
		if( null != globalSingletonObj )
		{
			// 關閉countdown manager
			CountDownTimeManager countDownTimeManager = globalSingletonObj.GetComponent<CountDownTimeManager>() ;
			if( null != countDownTimeManager )
				countDownTimeManager.Stop() ;
			
			// 關閉countdown manager
			ElapsedTimeManager elapsedTimeManager = globalSingletonObj.GetComponent<ElapsedTimeManager>() ;
			if( null != elapsedTimeManager )
				elapsedTimeManager.Stop() ;
		}

	}
	
	private void ShowBattleScore( bool _IsWin )
	{
		BattleScoreManager manager = GlobalSingleton.GetBattleScoreManager() ;
		if( null != manager )
		{
			manager.AddScore( ScoreType.ElapsedSec , Time.timeSinceLevelLoad ) ;
			manager.Active() ;
		}
		
		NamedObject battleScoreParentObj = new NamedObject( "GUI_BattleScore" ) ;
		if( null == battleScoreParentObj )
			return ;
		
		ShowUGUI.Show( battleScoreParentObj.Obj , true , true , true ) ;
		
		if( true == _IsWin )
		{
			// 關閉 lose gui object
			ShowUGUI.Show( "MessageCard_Lose" , false , true , true ) ;
			ShowUGUI.Show( "MessageCard_End" , false , true , true ) ;
		}
		else
		{
			// 關閉 win gui object
			ShowUGUI.Show( "MessageCard_Win" , false , true , true ) ;
			ShowUGUI.Show( "Seven and the Doctor sing Klingon War Song Object" , false , true , true ) ;
			
			if( true == m_RealLose )
				ShowUGUI.Show( "MessageCard_End" , false , true , true ) ;
			else
				ShowUGUI.Show( "MessageCard_Lose" , false , true , true ) ;
		}
	}
} // end of VictoryEvent

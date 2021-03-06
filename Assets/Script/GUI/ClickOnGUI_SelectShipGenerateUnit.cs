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
@file ClickOnGUI_SelectShipGenerateUnit.cs
@author NDark

# 點擊產生單位及UnitDataGUI
# 用在選擇主角船的頁面
# 按鈕後會依據參數產生單位即相對應的UnitDataGUI
# 仰賴特製的 關卡產生器 SelectShipLevelGenerator
# 仰賴 GUIUpdate
# 參數
## m_PrefabName 船隻的prefab參數
## m_UnitDataName 船隻的unit data參數
## m_RaceName 船隻的種族 (目前用不到)
## m_SideName 船隻的陣營 (目前用不到)
# 點選後會將資料記錄在 GlobalSingleton 中
## GlobalSingleton.m_CustomActive 的啟動是由選擇模式的 ClickOnGUI_ResetShipCustom.cs 來設定

@date 20130204 file started.
@date 20130204 by NDark . add setting of custom data at OnMouseDown()
@date 20130205 by NDark . comment.
@date 20130213 by NDark . remove code of GlobalSingleton.m_CustomActive at OnMouseDown()

*/
using UnityEngine;
using System.Collections.Generic ;

public class ClickOnGUI_SelectShipGenerateUnit : MonoBehaviour 
{
	[System.Serializable]
	public enum GenerateState
	{
		UnActive ,
		Destroy ,
		Generate ,
		End ,
	}
	
	public string m_PrefabName = "" ;
	public string m_UnitDataName = "" ;
	public string m_RaceName = "" ;
	public string m_SideName = "" ;
	
	private GenerateState m_State = GenerateState.End ;

	// Use this for initialization
	void Start ()
	{
		m_State = GenerateState.End ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject globalSingletonObj = GlobalSingleton.GetGlobalSingletonObj() ;
		
		switch( m_State )
		{
		case GenerateState.UnActive :
			m_State = GenerateState.Destroy ;
			break ;
		case GenerateState.Destroy :
			if( null != globalSingletonObj )
			{
				SelectShipLevelGenerator selectShipSceneGenerator = globalSingletonObj.GetComponent<SelectShipLevelGenerator>() ;
				GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
				if( null != guiUpdate )
					guiUpdate.DestroyMainCharacterUnitDataGUI() ;
				
				if( null != selectShipSceneGenerator &&
					null != selectShipSceneGenerator.m_GeneratedObj )
					GameObject.Destroy( selectShipSceneGenerator.m_GeneratedObj ) ;
			}			
			m_State = GenerateState.Generate ;
			break ;
		case GenerateState.Generate :
			if( null != globalSingletonObj )
			{			
				SelectShipLevelGenerator selectShipSceneGenerator = globalSingletonObj.GetComponent<SelectShipLevelGenerator>() ;
				GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
				if( null != selectShipSceneGenerator )
				{
					Dictionary<string,string> supplementalVec = new Dictionary<string, string>() ;
					selectShipSceneGenerator.GenerateUnit( "MainCharacter" ,
														   m_PrefabName ,
														   m_UnitDataName ,
														   m_RaceName ,
														   m_SideName , 
														   Vector3.zero ,
														   Quaternion.identity ,
														   supplementalVec ) ;
				}
				
				Camera.main.transform.rotation = Quaternion.LookRotation( Vector3.zero - Camera.main.transform.position ) ;
				
				if( null != guiUpdate )
				{
					guiUpdate.CreateMainCharacterUnitDataGUI() ;
					if( null != guiUpdate.m_MainCharacterGUIBackground.Obj )
					{
						Vector3 guiObjPos = guiUpdate.m_MainCharacterGUIBackground.Obj.transform.position ;
						guiObjPos.Set( 0.73f , 0.34f , 5.0f ) ;
						guiUpdate.m_MainCharacterGUIBackground.Obj.transform.position = guiObjPos ;
					}
				}	
			}
			m_State = GenerateState.End ;
			break ;
		case GenerateState.End :
			break ;
		}
	}
	
	void OnClick()
	{
		SelectShipGenerateUnit() ;
	}
	
	void OnMouseDown()
	{
		SelectShipGenerateUnit() ;
	}
	
	public void SelectShipGenerateUnit()
	{
		if( GenerateState.End == m_State )
		{
			m_State = GenerateState.UnActive ;
			GlobalSingleton.m_CustomPrefabName = m_PrefabName ;
			GlobalSingleton.m_CustomUnitDataName = m_UnitDataName ;
			
			// GlobalSingleton.m_CustomActive has already been set at ClickOnGUI_ResetShipCustom.cs
			#if DEBUG_LOG			
			Debug.Log( "GlobalSingleton.m_CustomActive = true ;" ) ;
			#endif
		}
	}
}

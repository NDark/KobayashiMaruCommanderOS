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
@file DrawMiniMap.cs
@brief 繪出小地圖
@author NDark
 
繪出小地圖

依照玩家的感測系統來更新小地圖
其中玩家物件是獨立創造出來(一次)
其他的物件則是根據玩家感測系統更新

小地圖物件都存在容器中 m_MiniMapUnits
# 更新內容如下
# CheckMinimapSize() 檢查小地圖是否需要調整範圍
# CheckForRemove() 檢查是否需要移除小地圖物件
# RemoveUnitInMinimap() 真正移除需要移除的物件
# CheckForAdd() 檢查增加小地圖物件
# UpdatePos() 更新所有小地圖物件的位置
# CreateAMiniMapObj() 產生一個小地圖物件

感測系統會定期檢查單位周圍的單位
而小地圖只取得玩家的感測系統來更新小地圖
# m_RefreshTimer 更新週期請參考 BaseDefine.DRAW_MINIMAP_REFRESH_SEC
# GenerateMiniMapForMainCharacter() 額外產生主角的物件(不是透過感測器產生)

@date 20121124 by NDark 
. add class member m_RefreshTimer for refresh time.
. add class member m_MiniMapParent to be parent of minimap
. add null checking at UpdatePos()
@date 20121125 by NDark 
. add class method GenerateMiniMapForMainCharacter()
. add null checking at CheckForAdd()
@date 20121204 by NDark . comment.
@date 20121219 by NDark . replace by ConstName.CreateMiniMapTextureResourcePath() at GenerateMiniMapForMainCharacter()
@date 20130109 by NDark . add class method CheckMinimapSize().
@date 20130113 by NDark . refactor and comment.
*/
using UnityEngine;
using System.Collections.Generic;

// 單位與小地圖物件的組合
[System.Serializable]
public class NamedObjectPair
{
	public NamedObject UnitObj = new NamedObject() ;
	public NamedObject MiniMapObj = new NamedObject() ;
	
	public NamedObjectPair()
	{
	}
	
	public NamedObjectPair( NamedObject _UnitObj , 
							NamedObject _MiniMapObj )
	{
		UnitObj = _UnitObj ;
		MiniMapObj = _MiniMapObj ;
	}
}

public class DrawMiniMap : MonoBehaviour {
	
	public CountDownTrigger m_RefreshTimer = new CountDownTrigger( BaseDefine.DRAW_MINIMAP_REFRESH_SEC ) ;// 更新的週期
	
	/* 
	 小地圖物件都存在容器中
	 依照物件名稱收集的所有小地圖物件
	 */
	public Dictionary<string, NamedObjectPair > m_MiniMapUnits = new Dictionary<string, NamedObjectPair>() ;
	
	private List<string> m_RemoveList = new List<string>() ;// 準備移除的清單
	private NamedObject m_MiniMapParent = new NamedObject( ConstName.MiniMapParentObjectName ) ;// 小地圖的parent object
	private bool m_MainCharacterIsCreated = false ;// 玩家的小地圖物件是否已經創造
	private NamedObject m_MainCharacter = new NamedObject() ;
	
	// Use this for initialization
	void Start () 
	{
		m_RefreshTimer.Rewind() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_RefreshTimer.IsCountDownToZero() )
			return ;
		
		if( false == m_MainCharacterIsCreated )
		{
			m_MainCharacterIsCreated = GenerateMiniMapForMainCharacter() ;
		}
		if( null == m_MainCharacter.GetObj() )
		{
			m_MainCharacter.Setup( ConstName.MainCharacterObjectName ,
								   GlobalSingleton.GetMainCharacterObj() ) ;
		}
		
		if( null == m_MainCharacter.GetObj() )
			return ;
		
		UnitSensorSystem sensorSys = m_MainCharacter.Obj.GetComponent<UnitSensorSystem>() ;
		if( null != sensorSys )
		{
			UpdateMiniMap( sensorSys ) ;
		}
	
	}
	
	private void UpdateMiniMap( UnitSensorSystem _sensorSys )
	{
		CheckMinimapSize( _sensorSys ) ;
		
		CheckForRemove( _sensorSys ) ;
		
		RemoveUnitInMinimap() ;
		
		CheckForAdd( _sensorSys ) ;
		
		UpdatePos( _sensorSys ) ;
	}	

	
	private void CheckMinimapSize( UnitSensorSystem _sensorSys )
	{
		float minimapCameraViewportSize = _sensorSys.m_SensorDistance ;
		
		// 超過就露餡了
		if( minimapCameraViewportSize > 200 )
			minimapCameraViewportSize = 200 ;
		else if( minimapCameraViewportSize < 1 )
			minimapCameraViewportSize = 1 ;
		
		GameObject miniMapCameraObj = GlobalSingleton.GetMiniMapCameraObj() ;
		if( null == miniMapCameraObj )
		{
			return ;
		}
		miniMapCameraObj.GetComponent<Camera>().orthographicSize = minimapCameraViewportSize ;
		
	}
	private void CheckForRemove( UnitSensorSystem _sensorSys )
	{
		// 先檢查有沒有已經消失需要移除的船隻
		Dictionary<string, NamedObjectPair>.Enumerator eInMinimap = m_MiniMapUnits.GetEnumerator() ;
		m_RemoveList.Clear() ;
		while( eInMinimap.MoveNext() )
		{
			string unitInMinimapName = eInMinimap.Current.Key ;
			bool found = false ;
			foreach( NamedObject eUnitInSensor in _sensorSys.m_SensorUnitList )
			{
				if( eUnitInSensor.Name == unitInMinimapName )
				{
					found = true ;
					break ;
				}
			}
			if( found == false )
			{
				// the unit should be remove from mini map
				m_RemoveList.Add( unitInMinimapName ) ;
			}
		}
		
	}
	
	private void RemoveUnitInMinimap()
	{

		// remove 
		List<string>.Enumerator eRemove = m_RemoveList.GetEnumerator() ;
		while( eRemove.MoveNext() )
		{
			string removeUnitName = eRemove.Current ;
			if( true == m_MiniMapUnits.ContainsKey( removeUnitName ) )
			{
				GameObject.Destroy( m_MiniMapUnits[ removeUnitName ].MiniMapObj.Obj ) ;
				m_MiniMapUnits.Remove( removeUnitName ) ;
			}
		}
		
	}	
	private void CheckForAdd( UnitSensorSystem _sensorSys )
	{
		
		// check for add
		// 檢查新增.
		foreach( NamedObject eUnitInSensor in _sensorSys.m_SensorUnitList )
		{
			bool found = false ;
			string unitNameInSensor = eUnitInSensor.Name ;
			Dictionary<string, NamedObjectPair>.Enumerator eInMinimap = m_MiniMapUnits.GetEnumerator() ;
			while( eInMinimap.MoveNext() )			
			{
				string unitNameInMiniMap = eInMinimap.Current.Key ;
				if( unitNameInSensor == unitNameInMiniMap )
				{
					found = true ;
					break ;// break while
				}
			}
			
			if( false == found )
			{
				GameObject anewObj = CreateAMiniMapObj( eUnitInSensor.Obj ) ;
				if( null != anewObj )
				{
					// Debug.Log( "anewObj = CreateAMiniMapObj" ) ;
					m_MiniMapUnits.Add( unitNameInSensor , 
						new NamedObjectPair( 
						eUnitInSensor ,
						new NamedObject( anewObj ) ) );				
				}
			}
		}
	}	
	
	
	private void UpdatePos( UnitSensorSystem _sensorSys )
	{
		Dictionary<string,NamedObjectPair>.Enumerator eInMinimap = m_MiniMapUnits.GetEnumerator() ;
		while( eInMinimap.MoveNext() )
		{
			// string unitName = eInMinimap.Current.Key ;
			GameObject unitObject = eInMinimap.Current.Value.UnitObj.GetObj() ;
			GameObject minimapObject = eInMinimap.Current.Value.MiniMapObj.GetObj() ;
			if( null != unitObject && 
				null != minimapObject )
			{
				minimapObject.transform.position = new Vector3(
					unitObject.transform.position.x ,
					minimapObject.transform.position.y ,
					unitObject.transform.position.z 
					 ) ;
			}
		}
	}
	
	private GameObject CreateAMiniMapObj( GameObject _UnitObject )
	{
		UnitData unitData = _UnitObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return null ;
		if( 0 == unitData.m_RaceName.Length )
			return null ;
		// Debug.Log( "unitData.m_RaceName" + unitData.m_RaceName ) ;
		string MiniObjName = ConstName.CreateMiniMapObjectName( _UnitObject.name ) ;
		GameObject MiniObj = PrefabInstantiate.Create( ConstName.MinimapUndefinedUnitTeamplateName , 
			MiniObjName ) ;
		
		if( null != MiniObj &&
			null != m_MiniMapParent.Obj )
		{
			string miniMapTextureName = ConstName.CreateMiniMapTextureResourcePath( unitData.m_RaceName ) ;
			MiniObj.GetComponent<Renderer>().material.mainTexture = ResourceLoad.LoadTexture( miniMapTextureName ) ;
			MiniObj.transform.parent = m_MiniMapParent.Obj.transform ;
		}
		
		return MiniObj ;
	}
	
	
	// 額外產生主角的物件(不是透過感測器產生)
	private bool GenerateMiniMapForMainCharacter()
	{
		GameObject mainCharObj = GlobalSingleton.GetMainCharacterObj() ;
		GameObject miniMapCameraObj = GlobalSingleton.GetMiniMapCameraObj() ;
		if( null == mainCharObj || 
			null == miniMapCameraObj )
		{
			Debug.Log( "GenerateMiniMapForMainCharacter() : null == mainCharObj || null == miniMapCameraObj" ) ;
			return false ;
		}
		
		UnitData unitData = mainCharObj.GetComponent<UnitData>() ;
		if( null == unitData )
			return false;
		if( 0 == unitData.m_RaceName.Length )
			return false;
		
		string MiniObjName = ConstName.CreateMiniMapObjectName( mainCharObj.name ) ;
		GameObject MiniObj = PrefabInstantiate.Create( ConstName.MinimapUndefinedUnitTeamplateName , 
													   MiniObjName ) ;
		
		if( null != MiniObj &&
			null != m_MiniMapParent.Obj )
		{
			MiniObj.GetComponent<Renderer>().material.mainTexture = 
				ResourceLoad.LoadTexture( ConstName.CreateMiniMapTextureResourcePath( unitData.m_RaceName ) ) ;
			MiniObj.transform.parent = miniMapCameraObj.transform ;
			MiniObj.transform.localPosition = new Vector3( 0.0f , 0.0f ,
														   MiniObj.transform.localPosition.z ) ;
		}
		return true ;
	}	
}

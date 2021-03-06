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
/**
@file MainUpdate.cs
@brief Main Update
@author NDark
 
說明
# 可以透過各後門來決定更新的步驟
# 有一註冊船隻名稱串列 List<string> UnitNamesList
用來讓產生船隻時把物件名稱註冊到此串列上一併更新
# 有一移除船隻名稱串列 List<string> removeUnitList
用來檢查船隻存活時記錄,檢查完畢後一併移除,避免邊檢查邊移除的窘境

更新 

根據註冊的船隻進行更新
# UpdateUnitAlive() 檢查船隻存活與否
# 從m_UnitNamesList清單中移除那些不需要存活的船隻
# UpdateSpeed() 更新速度
# UpdatePosition() 更新移動位置
## 取得船隻脈衝速度現在值計算下一個移動位置
## 根據場景檢查可移動範圍
## 移動物件
# DriftPosition() 更新漂移
# 清除漂移向量 ClearDriftVec()
# UpdateRotate() 更新旋轉

@date 20121109 by NDark . change type of m_UnitNamesList to List<NamedObject>.
@date 20121111 by NDark 
. add class member Debug_IfDriftPositon
. add class method ClearDriftVec()
. add class method DriftPosition()
@date 20121125 by NDark . add null checking at UpdateSpeed()
@date 20121204 by NDark 
. comment.
. 新增 假如引擎發動中就不會被推動 at DriftPosition()
@date 20121208 by NDark 
. modify code for standard speed of impulse engine at UpdateSpeed()
. modify code for angular impulse engine at UpdateRotate()
@date 20121209 by NDark 
. modify code for standard speed of impulse engine at UpdateSpeed()
. modify code for angular impulse engine at UpdateRotate()
@date 20121219 by NDark 
. add class method CheckUnitAlive()
. add class method RemoveDeadUnit()
. rename class member removeUnitList by m_RemoveUnitList
. add SystemLogManager at Start() and OnDestroy()
. add code of AutoPlayMachine at Start(), OnDestroy()
@date 20121220 by NDark . replace by GlobalSingleton.IsInAutoPlay() at Start()
@date 20121227 by NDark . replace by std at UpdateSpeed()
@date 20121228 by NDark . add class method GetUnit()
@date 20130109 by NDark 
. add code of delta time at UpdateRotate()
. add code of mass at DriftPosition()
@date 20130119 by NDark . modify code of translate by world coordinate at DriftPosition()


*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainUpdate : MonoBehaviour {
	
	public bool Debug_IfUpdateSpeed = true ;
	public bool Debug_IfUpdatePositon = true ;
	public bool Debug_IfDriftPositon = true ;
	public bool Debug_IfUpdateRotate = true ;
	public bool Debug_IfUpdateUnitAlive = true ;
	public bool Debug_IfUpdateWithTimer = true ;
	public float Debug_UpdateCustomElapsedSec = 0.03F ;
	public Vector3 DebugDirection ;
	
	/* 
	 * @brief 註冊船隻名稱串列
	 * 用來讓產生船隻時把物件名稱註冊到此串列上一併更新
	 */
	public List<NamedObject> m_UnitNamesList = new List<NamedObject>() ;
	
	/*
	 * @brief 移除船隻名稱串列
	 * 用來檢查船隻存活時記錄,檢查完畢後一併移除,避免邊檢查邊移除的窘境
	 */
	public List<string> m_RemoveUnitList ;
	
	public GameObject GetUnit( string _UnitName ) 
	{
		GameObject ret = null ;
		List<NamedObject>.Enumerator e = m_UnitNamesList.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( _UnitName == e.Current.Name )
			{
				ret = e.Current.Obj ;
				break ;
			}
		}
		return ret ;
	}
	
	// Use this for initialization
	void Start () 
	{
		if( false == GlobalSingleton.IsInAutoPlay() )
		{
			SystemLogManager.Initialize() ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckUnitAlive() ;
		
		// remove object
		RemoveDeadUnit() ;

		List<NamedObject>.Enumerator eUnit = m_UnitNamesList.GetEnumerator() ;
		while( eUnit.MoveNext() )
		{
			if( true == Debug_IfUpdateSpeed )
				UpdateSpeed( eUnit.Current.Obj ) ;			
			if( true == Debug_IfUpdatePositon )
				UpdatePosition( eUnit.Current.Obj ) ;
			if( true == Debug_IfDriftPositon )
				DriftPosition( eUnit.Current.Obj ) ;			
			
			ClearDriftVec( eUnit.Current.Obj ) ;// always clear if it exist
			
			if( true == Debug_IfUpdateRotate )
				UpdateRotate( eUnit.Current.Obj ) ;
		}		
	}
	
	void OnDestroy()
	{
		if( false == this.gameObject.GetComponent<AutoPlayMachine>().m_Active )
			SystemLogManager.ExportToFile() ;
	}
	
	
	/*
	速度計算式
	首先取得標準速度
	呼叫 UnitData::CalculateAliveImpulseEngineEffectRatio()
	計算 正常運作的脈衝引擎貢獻的功效比例 = Sum( 存活的脈衝引擎功效值 * 能源配給比例 ) / 脈衝引擎的最大值總和
	標準速度 = 脈衝速度最大值 * 正常運作的脈衝引擎貢獻的功效比例
	取得 脈衝引擎節流閥比例ImpulseEngineRatio
	目前速度 = 標準速度 * 脈衝引擎節流閥比例
	更新移動時就依照目前速度來更新.
	 */
	private void UpdateSpeed( GameObject unitObj ) 
	{
		if( null == unitObj )
			return ;
		UnitData unitData = unitObj.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		string IMPULSE_ENGINE_SPEED = ConstName.UnitDataComponentImpulseEngineSpeed ;		
		string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;		
		if( true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) &&
			true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_SPEED ) )
		{
			// impulse speed by all engines and under effect
			float aliveImpulseEngineRatio = unitData.CalculateAliveImpulseEngineEffectRatio() ;
			float impulseSpeedStandardNow = unitData.standardParameters[IMPULSE_ENGINE_SPEED].m_Std * aliveImpulseEngineRatio ;
			
			float controlRatio = unitData.standardParameters[ IMPULSE_ENGINE_RATIO ].now ;
			
			unitData.standardParameters[IMPULSE_ENGINE_SPEED].now = impulseSpeedStandardNow * controlRatio ;
			
			// Debug.Log( unitObj.name + " " + aliveImpulseEngineRatio +" " + controlRatio ) ;
		}
	}
	
	/*
	 * # 取得船隻脈衝速度現在值計算下一個移動位置
	 * # 根據場景檢查可移動範圍
	 * # 移動物件
	 */
	private void UpdatePosition( GameObject shipObject ) 
	{
		if( null == shipObject )
			return ;
		
		string IMPULSE_ENGINE_SPEED = ConstName.UnitDataComponentImpulseEngineSpeed ;
		
		UnitData shipData = shipObject.GetComponent<UnitData>() ;
		if( false == shipData.standardParameters.ContainsKey( IMPULSE_ENGINE_SPEED ) )
		{
			// Debug.Log( "UpdatePosition() : " + shipData.name + " no ContainsKey ImpulseEngineSpeed" ) ;
			return ;
		}
		
		float velocityNow = shipData.standardParameters[ IMPULSE_ENGINE_SPEED ].now ;	
				
		// direction		
		Vector3 direction_world = shipObject.transform.forward ;
		DebugDirection = direction_world ;
		direction_world.Normalize() ;
		if( true == Debug_IfUpdateWithTimer )
		{
			direction_world *= ( velocityNow * Time.deltaTime ) ;
		}
		else
		{
			// debug mode
			direction_world *= ( velocityNow * Debug_UpdateCustomElapsedSec ) ;
		}
		
		// check out of backgroundobj
		
		Vector3 newPosInWorld = shipObject.transform.position + direction_world ;
		
		
		bool outofSpace = false ;
		
		// check out of level space
		BackgroundObjInitialization bObjectScript = GlobalSingleton.GetBackgroundInit() ;
		if( null != bObjectScript )
			outofSpace = bObjectScript.IsOutofLevel( newPosInWorld ) ;

		
		if( false == outofSpace )
		{
			shipObject.transform.Translate( direction_world , Space.World ) ;
		}		
		
		
	}
	
	private void ClearDriftVec( GameObject unitObj ) 
	{
		if( null == unitObj )
			return ;
		
		UnitData unitData = unitObj.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		unitData.m_ForceToMoveVec = Vector3.zero ;
	}
	
	private void DriftPosition( GameObject unitObj ) 
	{
		if( null == unitObj )
			return ;
		
		UnitData unitData = unitObj.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;

		// 假如引擎發動中就不會被推動.
		string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;		
		if( true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) )
		{
			float ratio = unitData.standardParameters[ IMPULSE_ENGINE_RATIO ].now ;
			if( ratio != 0.0f )
				return ;
		}
				
		if( 0 == unitData.m_ForceToMoveVec.sqrMagnitude )
			return ;
		
		Rigidbody rbody = this.gameObject.GetComponentInChildren<Rigidbody>() ;
		float mass = 1.0f ;
		if( null != rbody )
			mass = rbody.mass ;
		unitData.m_ForceToMoveVec /= mass ;
		// Debug.Log( unitObj.name + " unitData.m_ForceToMoveVec=" + unitData.m_ForceToMoveVec + unitData.m_ForceToMoveVec.sqrMagnitude ) ;
		unitObj.transform.Translate( unitData.m_ForceToMoveVec , Space.World ) ;
	}
	
	private void UpdateRotate( GameObject shipObject ) 
	{
		if( null == shipObject )
			return ;
		
		UnitData unitData = shipObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		string ImpulseEngineAngularRatioStr = ConstName.UnitDataComponentImpulseEngineAngularRatio ;
		string ImpulseEngineAngularSpeedStr = ConstName.UnitDataComponentImpulseEngineAngularSpeed ;
		if( true == unitData.standardParameters.ContainsKey( ImpulseEngineAngularRatioStr ) &&
			true == unitData.standardParameters.ContainsKey( ImpulseEngineAngularSpeedStr ) )
		{
			float aliveRatio = unitData.CalculateAliveImpulseEngineEffectRatio() ;
			float controlRatio = unitData.standardParameters[ ImpulseEngineAngularRatioStr ].now ;
			
			StandardParameter impulseEngineAngularSpeed = unitData.standardParameters[ImpulseEngineAngularSpeedStr] ;
			
			impulseEngineAngularSpeed.now = impulseEngineAngularSpeed.max * controlRatio * aliveRatio ;
			float velocityNow = impulseEngineAngularSpeed.now ;	
			shipObject.transform.Rotate( 0 , velocityNow * Time.deltaTime , 0 ) ;
		}
	}
	
	public void UpdateUnitAlive( GameObject _unitObj )
	{
		if( null == _unitObj )
			return ;
		UnitData shipData = _unitObj.GetComponent<UnitData>() ;		
		if( shipData.m_UnitState.state == (int)UnitState.Dead )
		{
			m_RemoveUnitList.Add( _unitObj.name ) ;
			
			if( -1 != _unitObj.name.IndexOf( "Enemy_" ) )
			{
				// Debug.Log( "m_EnemyGeneratedTable.Remove" ) ;
				GlobalSingleton.GetEnemyGeneratorComponent().m_EnemyGeneratedTable.Remove( _unitObj.name ) ;				
			}
			
			GameObject.Destroy( _unitObj ) ;
		}
	}
	
	

	private void CheckUnitAlive()
	{
		List<NamedObject>.Enumerator eUnit = m_UnitNamesList.GetEnumerator() ;
		while( eUnit.MoveNext() )
		{
			if( true == Debug_IfUpdatePositon )
				UpdateUnitAlive( eUnit.Current.Obj ) ;
		}
	}
	
	private void RemoveDeadUnit()
	{
		foreach( string unitName in m_RemoveUnitList )
		{
			List<NamedObject>.Enumerator eRemoveUnit = m_UnitNamesList.GetEnumerator() ;
			while( eRemoveUnit.MoveNext() )
			{
				if( unitName == eRemoveUnit.Current.Name )
				{
					m_UnitNamesList.Remove( eRemoveUnit.Current ) ;
					break ;
				}
			}			
		}
		m_RemoveUnitList.Clear() ;
	}	
}

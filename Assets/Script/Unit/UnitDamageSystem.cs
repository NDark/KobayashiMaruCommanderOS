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
@file UnitDamageSystem.cs
@brief 單位的被傷害系統 
@author NDark

# 目前負責創造防護罩(3D)
# Update() 負責 更新/關閉 DamageEffect 
## 防護罩特效 
## 火花特效 
# SplitShield() 將 防護罩物件名稱切成 部件名稱
# 製造傷害 CauseDamageValueOut() 外部使用
## 檢查是否自動播放
## 將傳入的部件切為部件名稱
## 檢查防護罩的效果(傷害轉換率) 得到真正的傷害
## 呼叫 CauseDamageValueIn() 製造傷害
## 依照真正傷害值 製造傷害數字特效
# CauseDamageValueIn() 對指定部件製造傷害
# 傷害文字特效被綁定在製造傷害的函式內，使用變數 m_EffectGUI_DamageNumber 來控制
## 目前一個單位使用一個傷害文字。傷害文字不掛在物件下，所以船隻消滅時必須特地移除。
## 必須在製造傷害之前呼叫 ActiveDamageNumberEffectNextTime() 設定是否要顯示，以及顯示的時間。
## 每次顯示後，數值都會重置。讓下一次的傷害文字顯示正常。
# 傳回被擊中的部件 RetrieveHitComponent()
# ActiveDamageEffect() 依照部件啟動傷害特效
## ActiveDamageEffectByTime() 一定時間內依照部件啟動傷害特效
# ActiveGUI_DamageNumberEffectByTime() 啟動傷害文字特效
# StopEffectObject() 以名稱強制關閉指定特效
# CreateDamageReport() 製造傷害報告(目前關閉中) 
# CreateDamageSuffer() 紀錄被傷害值，只紀錄船殼被傷害值。
# RetrieveRealTargetComponent() 傳回真正被擊中的部件(包含單位的轉換)
## RetrieveHitComponent() 透過物理系統傳回真正被擊中的部件，可能單位已經不同了



@date 20121103 by NDark . fix an error of not stoping spark effect correctly by time at Update ().
...
@date 20130113 by NDark . refactor and comment.
@date 20130119 by NDark . remove InverseTransformDirection() at ActiveDamageEffect()
@date 20130120 by NDark . add class method SendComponentVibration()
@date 20130126 by NDark . add class member m_LastAttackerDisplayName

*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;

public class UnitDamageSystem : MonoBehaviour 
{
	// One damage number of one unit now.
	public NamedObject m_EffectGUI_DamageNumber = new NamedObject() ;
	
	public List<DamageEffect> m_DamageEffects = new List<DamageEffect>() ;
	
	public string m_LastAttackerName = "" ;
	public string m_LastAttackerDisplayName = "" ;
	public static bool m_ReportDamage = false ;	// 傷害的訊息通知,暫時關閉
	
	// Damage Number特效的啟動與否控制
	public bool m_DefaultActiveDamageEffect = true ;// 預設Damage Number特效是否要啟動
	public float m_DefaultDamageNumberElapsedSec = 1.0f ;	
	public bool m_ActiveDamageEffect = true ;// 下一次Damage Number特效是否要啟動,每次使用過就會回到預設值
	public float m_DamageNumberElapsedSec = 1.0f ;
	
	// 設定Damage Number的啟動及時間
	public void SetDamageNumberEffect( bool _Active , float _ElaspsedSec )
	{
		m_DefaultActiveDamageEffect = _Active ;
		m_DefaultDamageNumberElapsedSec = _ElaspsedSec ;
		ActiveDamageNumberEffectNextTime( _Active , _ElaspsedSec ) ;
	}
	
	// 設定 下一次 Damage Number 的啟動及時間
	public void ActiveDamageNumberEffectNextTime( bool _Active , float _ElaspsedSec )
	{
		m_ActiveDamageEffect = _Active ;
		m_DamageNumberElapsedSec = _ElaspsedSec ;
	}
	
	// Use this for initialization
	void Start () 
	{
		CreateShields() ;
	}
	
	void OnDestroy()
	{
		if( null != m_EffectGUI_DamageNumber.GetObj() )
		{
			// Debug.Log( "GameObject.Destroy( m_EffectGUI_DamageNumber.Obj )"  + m_EffectGUI_DamageNumber.Name ) ;
			GameObject.Destroy( m_EffectGUI_DamageNumber.Obj ) ;
		}
	}
	
	// 目前負責創造防護罩
	public void CreateShields()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>();
		if( null != unitData )
		{
			unitData.CreateShields() ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		List<DamageEffect> removeList = new List<DamageEffect>() ;
		foreach( DamageEffect e in m_DamageEffects )
		{
			e.Update() ;
			if( e.m_State == DamageState.NonActive )
				removeList.Add( e ) ;
		}
		
		// remove stoped
		foreach( DamageEffect e in removeList )
		{
			// Debug.Log( "m_DamageEffects.Remove() " + e.EffectName() ) ;
			m_DamageEffects.Remove( e ) ;
		}
	}
	
	public void StopEffectObject( string _DamageEffectName )
	{
		// find correct effect object 
		// stop effect obj
		// Debug.Log( "StopEffectObject() " + _DamageEffectName ) ;
		foreach( DamageEffect e in m_DamageEffects )
		{
			if( e.EffectName() == _DamageEffectName )
			{
				e.Stop() ;
				break ;
			}
		}
	}
	
	// 依照部件啟動傷害特效
	public string ActiveDamageEffect( GameObject _Attacker , string _ComponentObjName )
	{
		// Debug.Log( "ActiveDamageEffect() " + _ComponentObjName ) ;
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		
		// find correct effect object
		
		DamageEffect newDamage = null ;
		
		string [] strVec = ConstName.GetSplitVec( _ComponentObjName ) ;
		if( strVec.Length > 1 && 
			strVec[1] != ConstName.UnitDataComponentUnitIntagraty )
		{
			// it's a component , 
			// active shield effect
			string ShieldComponentName = strVec[ 1 ] ;
			
			if( null != unitData &&
				true == unitData.componentMap.ContainsKey( ShieldComponentName ) &&
				true == unitData.m_ShieldDataSet.ContainsKey( ShieldComponentName ) 
				 )
			{
				UnitComponentData componentData = unitData.componentMap[ ShieldComponentName ] ;
				ComponentDataSet shieldSet = unitData.m_ShieldDataSet[ ShieldComponentName ] ;
				if( false == componentData.IsOffline() )
				{
					newDamage = new DamageEffect() ;
					newDamage.m_EffectObj.Setup( shieldSet.Effect3DObject ) ;
					newDamage.IsDestroyAtEnd = false ;
					newDamage.Start() ;
					ObjectShakeEffect shake = shieldSet.Effect3DObject.GetComponent<ObjectShakeEffect>() ;
					if( null != shake )
					{
						shake.Active( true ) ;
					}
				}
			}
				
		}
		else
		{
			// it's unit
			// active sparks effect
			string sparksTemplateName = ConstName.EffectSparksTemplateName ;
			string effectObjName = ConstName.CreateEffectName( this.gameObject.name , sparksTemplateName , true ) ;
			GameObject effectObj = PrefabInstantiate.CreateByInit( sparksTemplateName , 
																   effectObjName ,
																   this.gameObject.transform.position ,
																   this.gameObject.transform.rotation ) ;
			// Debug.Log( "ActiveDamageEffect() effectObjName" + effectObjName ) ;
			if( null == effectObj )
			{
				Debug.Log( "ActiveDamageEffect() null == effectObj=" + sparksTemplateName ) ;
				return "" ;
			}
			
			// parent
			effectObj.transform.parent = this.gameObject.transform ;	
			
			newDamage = new DamageEffect() ;
			newDamage.m_EffectObj.Setup( effectObj ) ;
			newDamage.IsDestroyAtEnd = true ;
			newDamage.Start() ;
		}
		

		
		
		string retEffectName = "" ;
		
		
		if( null != newDamage )
		{
			this.m_DamageEffects.Add( newDamage ) ;
			retEffectName = newDamage.EffectName() ;
		}
		
		// push effect
		// try add shift effect
		if( null != unitData )
		{
			Vector3 ToAttacker = this.gameObject.transform.position - _Attacker.transform.position ;
			ToAttacker.y = 0 ;// do not move in y direction
			ToAttacker.Normalize() ;
			// Debug.Log( ToAttacker ) ;
			
			ShiftEffect shiftDamage = new ShiftEffect() ;
			shiftDamage.m_ShiftVec = ToAttacker ;
			
			shiftDamage.m_TargetObject = this.gameObject ;
			shiftDamage.StartByTime(1.0f) ;
			this.m_DamageEffects.Add( shiftDamage ) ;
			//*/
		}
		
		return retEffectName ;
	}
	
	// 一定時間內依照部件啟動傷害特效
	public void ActiveDamageEffectByTime( GameObject _Attacker , 
										  string _ComponentObjName ,
										  float _TotalTime )
	{
		string effectName = ActiveDamageEffect( _Attacker , _ComponentObjName ) ;
		if( 0 == effectName.Length )
			return ;
		
		foreach( DamageEffect e in m_DamageEffects )
		{
			if( e.EffectName() == effectName )
			{
				e.StartByTime( _TotalTime ) ;
				break ;
			}
		}
	}	
	
	// 啟動傷害文字特效
	public void ActiveGUI_DamageNumberEffectByTime( string _displayString , 
													float _TotalTime )
	{
		m_EffectGUI_DamageNumber.Name = ConstName.CreateGUIDamageNumberEffectObjectName( this.gameObject.name ) ;
		if( null == m_EffectGUI_DamageNumber.GetObj() )
		{
			m_EffectGUI_DamageNumber.Obj = PrefabInstantiate.Create( ConstName.GUIDamageNumberEffectTemplateName , 
																	 m_EffectGUI_DamageNumber.Name ) ;
			


		}
		
		if( null != m_EffectGUI_DamageNumber.GetObj() )
		{
			DamageNumber damageNum = m_EffectGUI_DamageNumber.Obj.GetComponent< DamageNumber >() ;
			if( null != damageNum )
			{
				damageNum.Setup( this.gameObject , _displayString ) ;
				damageNum.StartByTime( _TotalTime ) ;
			}		
		}
	}	
	
	
	public float CauseDamageValueOut( string _AttackerUnit , 
									  string _AttackerUnitDisplayName ,
									  float _Damage , 
									  string _ComponentObjectName )
	{
		if( GlobalSingleton.IsInAutoPlay() )
			return 0.0f ;
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		
		string DefenseUnitName = this.gameObject.name ;

		// split component object name to component name
		string ComponentName = "" ;
		ComponentName = ConstName.GetSplitVecConetent( _ComponentObjectName , 1 ) ;
		if( 0 == ComponentName.Length )
			ComponentName = ConstName.UnitDataComponentUnitIntagraty ;
		
		// check shield conver rate
		if( true == unitData.componentMap.ContainsKey( ComponentName ) )
		{
			UnitComponentData component = unitData.componentMap[ ComponentName ] ;
			
			float converRate = 1.0f ;
			float realDamage = _Damage ;
			if( -1 != ComponentName.IndexOf( ConstName.UnitDataComponentShieldPrefix ) )
			{
				// shield
				// Debug.Log( ConstName.UnitDataComponentShieldPrefix ) ;
				converRate = component.TotalEffect() ;
				if( converRate < 1.0f ) 
					converRate = 1.0f ;
			}
			realDamage = _Damage / converRate ;
			float absortDamage = _Damage - realDamage ;
			
			// Debug.Log( "converRate" + converRate ) ;
			CauseDamageValueIn( _AttackerUnit , 
								_AttackerUnitDisplayName ,
								DefenseUnitName  , 
								realDamage , 
								absortDamage , 
								ComponentName ) ;
			
			// Create damage number effect
			if( true == m_ActiveDamageEffect )
			{
				string damagestr = string.Format( "{0:0.00}" , realDamage ) ;
				ActiveGUI_DamageNumberEffectByTime( damagestr , m_DamageNumberElapsedSec ) ;
				
				// reset
				m_ActiveDamageEffect = m_DefaultActiveDamageEffect ;
				m_DamageNumberElapsedSec = m_DefaultDamageNumberElapsedSec ;
			}
			
			
			return realDamage ;
		}
		else
		{
			Debug.Log( "CauseDamageValueOut() no such component" + ComponentName ) ;
			return 0.0f ;
		}
		
	}
	
	// 製造傷害
	public void CauseDamageValueIn( string _AttackerUnit , 
									string _AttackerDisplayName , 
									string _DefenseUnit ,
									float _RealDamage ,
									float _AbsortDamage ,
									string _ComponentName )
	{
		/*
		Debug.Log( "CauseDamageValueIn() _AttackerUnit=" + _AttackerUnit +
			" _DefenseUnit=" + _DefenseUnit +
			" _Damage" + _RealDamage +
			" _AbsortDamage=" + _AbsortDamage +
			" _ComponentName=" + _ComponentName ) ;
		//*/
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		if( false == unitData.componentMap.ContainsKey( _ComponentName ) )
			return ;
		UnitComponentData component = unitData.componentMap[ _ComponentName ] ;
		// message log
		SystemLogManager.AddLog( SystemLogManager.SysLogType.Damage , 
			_AttackerUnit + ":" + 
			_AttackerDisplayName + ":" + 
			_RealDamage + ":" + 
			_AbsortDamage + ":"+ 
			_DefenseUnit + ":" + 
			_ComponentName ) ;
			
		CreateDamageReport( _AttackerUnit , _DefenseUnit , _RealDamage ) ;
		CreateDamageSuffer( _AttackerUnit , _DefenseUnit , _RealDamage , _ComponentName ) ;
		SendComponentVibration( _DefenseUnit , _ComponentName ) ;
		
		float hpnow = component.hp ;			
		hpnow -= _RealDamage ;			
		component.hp = hpnow ;	

		m_LastAttackerName = _AttackerUnit ;
		m_LastAttackerDisplayName = _AttackerDisplayName ;
	}
	
	public void CreateDamageReport( string _AttackerUnit , string _DefenseUnit , float _DamageValue )
	{
		if( false == m_ReportDamage )
			return ;
		
		string DamageStr = _DamageValue.ToString() ;
		MessageQueueManager manager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != manager )
		{
			if( _AttackerUnit == ConstName.MainCharacterObjectName )
			{
				string message = StrsManager.Get( 1033 ) ;
				message = message.Replace( "%s" , _DefenseUnit ) ;
				message = message.Replace( "%d" , DamageStr ) ;
				manager.AddMessage( message ) ;
			}
			else if( _DefenseUnit == ConstName.MainCharacterObjectName )
			{
				string message = StrsManager.Get( 1034 ) ;
				message = message.Replace( "%s" , _AttackerUnit ) ;
				message = message.Replace( "%d" , DamageStr ) ;				
				manager.AddMessage( message ) ;
			}
		}
			
	}
	
	public void CreateDamageSuffer( string _AttackerUnit , 
									string _DefenseUnit , 
									float _DamageValue , 
									string _HitComponent )
	{
		
		BattleScoreManager battleScore = GlobalSingleton.GetBattleScoreManager() ;
		if( null != battleScore )
		{
			if( _DefenseUnit == ConstName.MainCharacterObjectName && 
				_HitComponent == ConstName.UnitDataComponentUnitIntagraty )
			{
				battleScore.AddScore( ScoreType.DamageSuffer , _DamageValue ) ;
			}
		}		
	}	
	
	public bool RetrieveRealTargetComponent( GameObject _AttackerUnit , 
											 string _AssumeDefenderComponent ,
											 ref string _ResultDefenseUnitName ,
											 ref string _ResultDefenseComponentObjectName )
	{
		bool ret = false ;
		_ResultDefenseUnitName = this.gameObject.name ;
		_ResultDefenseComponentObjectName = this.gameObject.name ;
		string realTargetUnitComponentObjectName = this.RetrieveHitComponent( _AttackerUnit , 
																			  _AssumeDefenderComponent ) ;
		if( 0 == realTargetUnitComponentObjectName.Length )
		{
			// Debug.Log( "RetrieveRealTargetComponent() 0 == realTargetComponentName.Length=" + realTargetComponentName ) ;
			return ret ;
		}
		_ResultDefenseComponentObjectName = realTargetUnitComponentObjectName ;
		
		// 由於可能在途中命中其他部件,則必須檢查是否中途被截斷
		string newUnitName = ConstName.SplitComponentObjectNameToUnitName( realTargetUnitComponentObjectName ) ;
		// Debug.Log( "ActiveWeapon() realTargetUnitComponentObjectName" + realTargetUnitComponentObjectName ) ;
		if( newUnitName != this.gameObject.name )
		{
#if DEBUG_LOG			
			Debug.Log( "ActiveWeapon() Target is interfered, and change new unit name" + newUnitName ) ;
#endif			
			// 重新取得物件及傷害系統
			_ResultDefenseUnitName = newUnitName ;
		}		
		ret = true ;
		return ret ;
	}
	
	// 傳回被擊中的部件 
	private string RetrieveHitComponent( GameObject _Attacker , 
										string _AssumeDefenderComponent )
	{
		// Debug.Log( "RetrieveHitComponent() _Attacker=" + _Attacker.name + " _AssumeDefenderComponent=" + _AssumeDefenderComponent ) ;
		string hitComponentName = "" ;
		GameObject AttactObject = _Attacker ;
		if( null == AttactObject )
		{
			Debug.Log( "RetrieveHitComponent() : null == AttactObject" ) ;
			return hitComponentName ;
		}
		
		hitComponentName = this.gameObject.name ;
		GameObject defenseUnit = this.gameObject ;
		GameObject definseComponentObj = defenseUnit ;
		
		string assumeHitComponentObjectName = 
			ConstName.CreateComponent3DObjectName( this.gameObject.name , 
												   _AssumeDefenderComponent ) ;
		Transform trans = defenseUnit.transform.FindChild( assumeHitComponentObjectName ) ;
		if( null != trans )
		{
			definseComponentObj = trans.gameObject ;
			hitComponentName = definseComponentObj.name ;
		}
		// Debug.Log( "RetrieveHitComponent() hitComponentName:" + hitComponentName ) ;		
		
		Vector3 direction = definseComponentObj.transform.position - AttactObject.transform.position ;
		
		Ray ray = new Ray( AttactObject.transform.position , direction ) ;
		RaycastHit [] hitsInfo ;
		// 0.95 is for short , in case it reach to the rear shield
		hitsInfo = Physics.RaycastAll( ray , direction.magnitude * 0.95f ) ;
		if( 0 == hitsInfo.Length )
		{
			// Debug.Log( "0 == hitsInfo.Length" ) ;
			return hitComponentName ;
		}
		float minDistance = 99999.0f ;
		string hitObjectName = "" ;
		for( int i = 0 ; i < hitsInfo.Length ; ++i )
		{
			// Debug.Log( "RetrieveHitComponent() hitsInfo[i].collider.gameObject.name:" + hitsInfo[i].collider.gameObject.name ) ;		
			if( -1 != hitsInfo[i].collider.gameObject.name.IndexOf( _Attacker.name ) )
			{
				// can't shoot itself.
				continue ;
			}
			Vector3 distanceVec = hitsInfo[i].collider.gameObject.transform.position - AttactObject.transform.position ;
			if( distanceVec.sqrMagnitude < minDistance )
			{
				minDistance = distanceVec.sqrMagnitude ;
				hitObjectName = hitsInfo[i].collider.gameObject.name ; ;
				
			}
		}
		if( 0 != hitObjectName.Length )
		{
			hitComponentName = hitObjectName ;
		}
		
		// Debug.Log( "RetrieveHitComponent() hitComponentName:" + hitComponentName ) ;		
		return hitComponentName ;
		
		
	}
	
	// 傳回被擊中的部件 
	private string RetrieveHitComponent( string _AttackerName , 
										string _AssumeDefenderComponent )
	{
		string hitComponentName = "" ;
		GameObject AttactObject = GameObject.Find( _AttackerName ) ;
		if( null == AttactObject )
		{
			Debug.Log( "RetrieveHitComponent() : null == AttactObject" ) ;
			return hitComponentName ;
		}
		
		return RetrieveHitComponent( AttactObject , _AssumeDefenderComponent ) ;
	}
	
	private void SendComponentVibration( string _DefenseUnit ,									
										 string _ComponentName )
	{
		UnitDataGUIAnimationManager guiAnimator = GlobalSingleton.GetUnitDataGUIAnimationManager() ;
		if( null == guiAnimator )
			return ;
		
		guiAnimator.SetupVibration( _DefenseUnit , _ComponentName ) ;		
	}
}

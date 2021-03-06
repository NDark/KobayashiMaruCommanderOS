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
@file UnitData.cs
@brief 單位資料
@author NDark

請參考 文件 
# 單位的資料結構
# 部件的狀態
# 部件

# 掛在單位下。儲存單位的資料
# Update() 更新單位的狀態
## 生命值歸零馬上切換到死亡中
## 死亡中狀態會旋轉
## 呼叫 CreateDestroyReport() 製造被摧毀訊息
# UnitDataCompnentUpdate() 更新部件資料
## HP
## Reload
## 呼叫 SendReloadCompletenessMessage() 發送填充完畢訊息
# GetHPNow() 取得目前的船殼生命值
# AssignComponent() 新增部件
# AssignStandardParameter() 新增標準參數
# IsAlive() 函式用來提供外界查詢此單位是否正常運作.
不正常下大部分的運作都不該發生.
# CreateComponents() 創造除了武器及防護照之外的部件
# CreateShields() 創造防護罩相關物件集合
# CreateWeapons() 創造武器相關物件集合
# AngularRatioHeadTo() 設定轉向
# AllStop() 停止且不轉向
# AdjustImpulseEngineRatio() 調整脈衝比例
# FindShieldWeightDirection() 計算防護罩權重方向
# CalculateAliveImpulseEngineEffectRatio() 計算目前存活中的脈衝引擎功效比例
# GetAllTrackorBeamComponentNameVec() 取得牽引光束部件
# GetAllWeaponComponentNameVec() 取得武器部件
# GetAllComponentNameVecWithKeyword() 依照關鍵字取得部件
# GetAllShieldComponentNameVec() 取得防護罩部件
# GetSensorComponent() 取得感測器部件
# GetSensorComponents() 取得感測器部件
# GetAllImpulseEngineComponentNameVec() 取得引擎部件
# SendReloadCompletenessMessage() 送出填充完畢訊息 給 ReloadAnimationManager
# CreateDestroyReport() 製造被摧毀的訊息
# RetrieveAllComponentEnergy() 取得所有部件的目前能源及最大值
# TrySetImpulseEngineEnergy() 設定引擎的能源
# RetrieveImpulseEngineEnergy() 取得引擎的目前能源及最大值
# TrySetSensorEnergy() 設定感測器的能源
# RetrieveSensorEnergy() 取得感測器的目前能源與最大值
# TrySetWeaponEnergy() 設定武器的能源
# RetrieveWeaponEnergy() 取得武器的目前能源與最大值
# TrySetShieldEnergy() 設定防護罩的能源
# RetrieveShieldEnergy() 取得防護罩的能源
# TrySetComponentsEnergyToValue() 設定部件能源
# HasAuxiliaryEnergy() 是否存在補助能源
# GetAuxiliaryEnergyValue() 取得補助能源目前量及最大值
# SetAuxiliaryEnergyValue() 設定補助能源目前量及最大值


@date 20121109 by NDark . remove class method GetTotalShieldRatio()
...
@date 20130101 by NDark 
. add class method GetSensorComponents()
. add class method RetrieveAllComponentEnergy()
. change class method SetImpulseEngineEnergyToRatio() to TrySetImpulseEngineEnergy()
. change class method SetSensorEnergyToRatio() to TrySetSensorEnergy()
. change class method SetWeaponEnergyToRatio() to TrySetWeaponEnergy()
. change class method SetShieldEnergyToRatio() to TrySetShieldEnergy()
. add class method TrySetComponentsEnergyToValue()
. add class method HasAuxiliaryEnergy()
. add class method GetAuxiliaryEnergyValue()
. add class method GetAuxiliaryEnergyValue()
. add class method SetAuxiliaryEnergyValue()
. add code of rigidbody of dying animation at Update()
@date 20130107 by NDark . fix an error that will send empty message at CreateDestroyReport()
@date 20130109 by NDark . add class member m_SideName
@date 20130113 by NDark . comment.
@date 20130126 by NDark 
. add class member m_DisplayNameIndex
. add class method GetUnitDisplayName()

*/
using UnityEngine;
using System.Collections.Generic ;

// 單位的狀態
[System.Serializable]
public enum UnitState
{
	UnActive = 0 ,
	Corpse ,
	Borning,
	Alive,
	Dying,
	Dead,
} ;

public class UnitData : MonoBehaviour {
	
	public float Debug_HP = 0.0f ;
	
	public float m_TimeChangeState = 0.0f ;
	
	public string m_PrefabTemplateName = "" ; // prefab template name
	public string m_UnitDataTemplateName = "" ;// unit data template name at unit table
	public string m_UnitDataGUITextureName = "" ;// unit data GUI background texture name
	
	public int m_DisplayNameIndex = -1 ;// 單位顯示名稱Index
	
	public string m_RaceName = "" ;// 種族是用來標定小地圖的圖案
	public string m_SideName = "" ;// Ally / Enemy / Other...
	public StateIndex m_UnitState = new StateIndex() ;
	
	public Dictionary< string , StandardParameter > standardParameters = new Dictionary< string , StandardParameter >() ;
	public Dictionary< string , UnitComponentData > componentMap = new Dictionary< string , UnitComponentData >() ;
	public StandardParameter [] Debug_standardParameters  ;
	
	public Vector3 m_ForceToMoveVec = Vector3.zero ;
	
	public Dictionary<string , string> m_SupplementalVec = new Dictionary<string, string>() ;
	
	public Dictionary< string , ComponentDataSet > m_ShieldDataSet = 
		new Dictionary<string, ComponentDataSet>() ;
	
	public float GetHPNow()
	{
		if( true == this.componentMap.ContainsKey( ConstName.UnitDataComponentUnitIntagraty ) )
		{
			UnitComponentData obj = this.componentMap[ ConstName.UnitDataComponentUnitIntagraty ] ;
			return obj.hp ;
		}
		return 0.0f ;
	}
	
	public void AssignComponent( string _Key , 
								 UnitComponentData _component )
	{
		componentMap[ _Key ] = new UnitComponentData( _component ) ;
	}
	
	public void AssignStandardParameter( string _Key , 
								 	 	 StandardParameter _Parameter )
	{
		standardParameters[ _Key ] = new StandardParameter( _Parameter ) ;
	}
	
	// 函式用來提供外界查詢此單位是否正常運作
	public bool IsAlive()
	{
		return ( m_UnitState.state == (int)UnitState.Alive ) ;
	}
	

	public UnitComponentData GetSensorComponent()
	{
		UnitComponentData ret = null ;
		string [] keyarray = new string[ this.componentMap.Count ] ;
		this.componentMap.Keys.CopyTo( keyarray , 0 ) ;
		foreach( string eString in keyarray )
		{
			if( -1 != eString.IndexOf( ConstName.UnitDataComponentSensor ) )
			{
				ret = this.componentMap[eString] ;
			}
		}
		return ret ;
	}
	public List<string> GetSensorComponents()
	{
		List<string> ret = new List<string>() ;
		string [] keyarray = new string[ this.componentMap.Count ] ;
		this.componentMap.Keys.CopyTo( keyarray , 0 ) ;
		foreach( string eString in keyarray )
		{
			if( -1 != eString.IndexOf( ConstName.UnitDataComponentSensor ) )
			{
				ret.Add( eString ) ;
			}
		}
		return ret ;
	}	
	
	public List<string> GetAllImpulseEngineComponentNameVec()
	{
		List<string> ret = GetAllComponentNameVecWithKeyword( ConstName.UnitDataComponentImpulseEnginePrefix ) ;
		return ret ;
 	}
	
	public List<string> GetAllShieldComponentNameVec()
	{
		List<string> ret = GetAllComponentNameVecWithKeyword( ConstName.UnitDataComponentShieldPrefix ) ;
		return ret ;
 	}
	
	// 依照關鍵字取得部件
	public List<string> GetAllComponentNameVecWithKeyword( string _Keyword )
	{
		List<string> ret = new List<string>() ;
		string [] keyarray = new string[ this.componentMap.Count ] ;
		this.componentMap.Keys.CopyTo( keyarray , 0 ) ;
		foreach( string eString in keyarray )
		{
			if( -1 != eString.IndexOf( _Keyword ) )
			{
				ret.Add( eString ) ;
			}
		}
		return ret ;
	}
	
	// 取得武器部件
	public List<string> GetAllWeaponComponentNameVec()
	{
		List<string> weaponList = GetAllComponentNameVecWithKeyword( "Weapon_" ) ;
		return weaponList ;
	}
	
	// 取得牽引光束部件
	public List<string> GetAllTrackorBeamComponentNameVec()
	{
		List<string> trackorBeamList = GetAllComponentNameVecWithKeyword( "TrackorBeam" ) ;
		return trackorBeamList ;
	}
	
	public void SendReloadCompletenessMessage( UnitComponentData _componentData )
	{
		if( null == _componentData )
			return ;
		ReloadAnimationManager animator = GlobalSingleton.GetReloadAnimationManager() ;
		if( null == animator )
			return ;
		
		animator.Setup( this.gameObject.name , _componentData.m_Name ) ;	
	}
	
	// 創造武器相關物件集合
	public void CreateWeapons( ref Dictionary< string , WeaponDataSet > _WeaponDataMap )
	{
		_WeaponDataMap.Clear() ;
		
		// find all weapon component
		List<string> weaponList = this.GetAllWeaponComponentNameVec() ;
		foreach( string eWeapon in weaponList )
		{
			// Debug.Log( "CreateWeapons() eWeapon=" + eWeapon ) ;
			WeaponDataSet newWeapon = new WeaponDataSet( this.gameObject , eWeapon ) ;
			UnitComponentData componentData = this.componentMap[ eWeapon ] ;
			
			// type
			if( -1 != eWeapon.IndexOf( "Phaser" ) )
				newWeapon.m_WeaponType = WeaponType.Phaser ;
			else if( -1 != eWeapon.IndexOf( "DisruptorArray" ) )
				newWeapon.m_WeaponType = WeaponType.Phaser ;			
			else if( -1 != eWeapon.IndexOf( "Torpedo" ) )
				newWeapon.m_WeaponType = WeaponType.Torpedo ;
			else if( -1 != eWeapon.IndexOf( "DisruptorCannon" ) )
				newWeapon.m_WeaponType = WeaponType.Cannon ;			
			else if( -1 != eWeapon.IndexOf( "CylonCannon" ) )
				newWeapon.m_WeaponType = WeaponType.Cannon ;						
			else if( -1 != eWeapon.IndexOf( "BattleStarCannon" ) )
				newWeapon.m_WeaponType = WeaponType.Cannon ;				
			else if( -1 != eWeapon.IndexOf( "TrackorBeam" ) )
				newWeapon.m_WeaponType = WeaponType.TrakorBeam ;
			
			// create weapon 3d object
			{
				string TemplateName = ConstName.CreateComponent3DObjectTemplateName( eWeapon ) ;
				// Debug.Log( "CreateWeapons() TemplateName=" + TemplateName ) ;
				// Debug.Log( "CreateWeapons() newWeapon.UnitObjectName=" + newWeapon.UnitObjectName ) ;
				// Debug.Log( "CreateWeapons() newWeapon.Component3DObjectName=" + newWeapon.Component3DObjectName ) ;
				GameObject weapon3DObj = PrefabInstantiate.Create( TemplateName , 
																   newWeapon.Component3DObjectName ) ;
				
				if( null != weapon3DObj )
				{
					// Debug.Log( "weapon3DObj.name:" + weapon3DObj.name ) ;
					// add a short cut to unit data component
					componentData.m_ComponentParam.m_Component3DObject.Setup( weapon3DObj ) ;
					
					Vector3 localposition = weapon3DObj.transform.position ;
					Quaternion localrotation = weapon3DObj.transform.rotation ;
					
					newWeapon.Component3DObject = weapon3DObj ;
		
					weapon3DObj.transform.parent = this.transform ;
					weapon3DObj.transform.localPosition = localposition ;
					weapon3DObj.transform.localRotation = localrotation ;
				}
			}
			
			// create effect of this object
			{
				string effect3DObjectTemplateName = "" ;
				effect3DObjectTemplateName = componentData.m_ComponentParam.m_Effect3DObjectTemplateName ;
				
				GameObject effect3DObj = PrefabInstantiate.Create( effect3DObjectTemplateName , 
																   newWeapon.Effect3DObjectName ) ;
				
				if( null != effect3DObj )
				{
					// Debug.Log( "effect3DObj.name:" + effect3DObj.name ) ;
					newWeapon.Effect3DObject = effect3DObj ;
					
					effect3DObj.transform.rotation = this.transform.rotation ;
					effect3DObj.GetComponent<Renderer>().enabled = false ;
					effect3DObj.GetComponent<Collider>().enabled = false ;
				}
			}
			
			string FireAudioName = componentData.m_WeaponParam.m_FireAudioName ;				
			if( 0 != FireAudioName.Length )
			{
				newWeapon.m_FireAudio = ResourceLoad.LoadAudio( FireAudioName ) ;
			}
			
			// Debug.Log( "m_WeaponDataMap.Add:" + eWeapon ) ;
			_WeaponDataMap.Add( eWeapon , newWeapon ) ;
		}// end of foreach

	}
	
	// 創造防護罩相關物件
	public void CreateShields()
	{
		Dictionary< string , ComponentDataSet > _ShieldSet = this.m_ShieldDataSet ;
		_ShieldSet.Clear() ;
		
		// find all shield
		List<string> allShieldVec = this.GetAllShieldComponentNameVec() ;
		foreach( string eShield in allShieldVec )
		{
			ComponentDataSet newShield = new ComponentDataSet( this.gameObject , eShield ) ;
			UnitComponentData componentData = this.componentMap[ eShield ] ;
			// create corresponding 3D shield

			string TemplateName = ConstName.CreateComponent3DObjectTemplateName( eShield ) ;
			string shield3DObjName = ConstName.CreateComponent3DObjectName( this.gameObject.name ,
				eShield ) ;
			
			GameObject shield3DObj = PrefabInstantiate.Create( TemplateName , 
															   shield3DObjName ) ;
			
			if( null == shield3DObj )
			{
				Debug.Log( "CreateShields() : null == shield3DObj" ) ;
				continue ;
			}
			
			// a short cut to component param
			componentData.m_ComponentParam.m_Component3DObject.Setup( shield3DObj ) ;
			
			newShield.Component3DObject = shield3DObj ;
			
			
			Vector3 localposition = shield3DObj.transform.position ;
			Quaternion localrotation = shield3DObj.transform.rotation ;
				
			shield3DObj.transform.parent = this.transform ;
			shield3DObj.transform.localPosition = localposition ;
			shield3DObj.transform.localRotation = localrotation ;
			

			// create shield effect object by shield3DObj
			{
				string shieldEffectTemplateName = "" ;
				shieldEffectTemplateName = componentData.m_ComponentParam.m_Effect3DObjectTemplateName ;
				
				string shieldEffectObjName = ConstName.CreateComponentEffectObjectName( this.gameObject.name , 
																						eShield ) ;
				
				GameObject effectObj = PrefabInstantiate.CreateByInit( shieldEffectTemplateName , 
																	   shieldEffectObjName ,
																	   shield3DObj.transform.position ,
																	   shield3DObj.transform.rotation ) ;
				if( null == effectObj )
				{
					Debug.Log( "null == effectObj" ) ;
					continue ;
				}

				newShield.Effect3DObject = effectObj ;
				effectObj.transform.parent = shield3DObj.transform ;
				Renderer [] renderers = effectObj.GetComponentsInChildren<Renderer>() ;
				foreach( Renderer e in renderers )
				{
					e.enabled = false ;
				}
			}				
		
			
			_ShieldSet.Add( eShield , newShield ) ;
		}
	}
	
	// 創造除了武器及防護照之外的部件
	public void CreateComponents()
	{
		foreach( UnitComponentData componentData in this.componentMap.Values )
		{
			if( -1 != componentData.m_Name.IndexOf( "Weapon" ) ||
				-1 != componentData.m_Name.IndexOf( "Shield" ) )
				continue ;
			
			string componentName = componentData.m_Name ;
			
			string TemplateName = ConstName.CreateComponent3DObjectTemplateName( componentName ) ;
			string component3DObjName = ConstName.CreateComponent3DObjectName( this.gameObject.name ,
				componentName ) ;
			
			GameObject component3DObj = PrefabInstantiate.Create( TemplateName , 
																  component3DObjName ) ;

			if( null == component3DObj )
			{
				continue ;
			}
			
			// a short cut to component param
			componentData.m_ComponentParam.m_Component3DObject.Setup( component3DObj ) ;			
			// Debug.Log( "CreateComponents() : componentName" + componentName ) ;
			
			Vector3 localposition = component3DObj.transform.position ;
			Quaternion localrotation = component3DObj.transform.rotation ;
				
			component3DObj.transform.parent = this.transform ;
			component3DObj.transform.localPosition = localposition ;
			component3DObj.transform.localRotation = localrotation ;
			
		}
	}
	
	// 設定轉向
	public void AngularRatioHeadTo( float _AngleOfTarget , 
  								    float _DotOfUp , 
								    float _TurningChangeRatioFromMaximum )
	{
		StandardParameter angularRatio = null ;
		string IMPULSE_ENGINE_ANGULAR_RATIO = ConstName.UnitDataComponentImpulseEngineAngularRatio ;
		if( true == standardParameters.ContainsKey( IMPULSE_ENGINE_ANGULAR_RATIO ) )
		{
			angularRatio = standardParameters[ IMPULSE_ENGINE_ANGULAR_RATIO ] ;
		}
		if( null == angularRatio )
			return ;

		if( _AngleOfTarget < 5.0f )
		{
			// maintain direction, do not turn.
			angularRatio.now = 0 ;// not to min
		}
		else
		{
			float turnRatioDiff = angularRatio.max * _TurningChangeRatioFromMaximum ;
			if( _DotOfUp > 0.0f )
			{
				angularRatio.now -= ( turnRatioDiff ) ;
			}
			else
			{
				angularRatio.now += ( turnRatioDiff ) ;
			}
		}	
	}
	
	public void AllStop()
	{
		string IMPLUSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
		string IMPULSE_ENGINE_ANGULAR_RATIO = ConstName.UnitDataComponentImpulseEngineAngularRatio ;			
		if( true == this.standardParameters.ContainsKey( IMPLUSE_ENGINE_RATIO ) )
			this.standardParameters[ IMPLUSE_ENGINE_RATIO ].Clear() ;
		if( true == this.standardParameters.ContainsKey( IMPULSE_ENGINE_ANGULAR_RATIO ) )
			this.standardParameters[ IMPULSE_ENGINE_ANGULAR_RATIO ].now = 0 ;// do not turn
	}
	
	public void AdjustImpulseEngineRatio( float _AddRatio )
	{
		string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ; ;
		if( true == this.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) )
			this.standardParameters[ IMPULSE_ENGINE_RATIO ].now += ( _AddRatio ) ;	
		
	}	
	public Vector3 FindShieldWeightDirection()
	{
		Vector3 thisUnitPosition = this.gameObject.transform.position ;
		Vector3 ret = Vector3.zero ;
		List<string> shieldNames = GetAllShieldComponentNameVec() ;
		List<string>.Enumerator shieldName = shieldNames.GetEnumerator() ;
		while( shieldName.MoveNext() )
		{
			UnitComponentData component = componentMap[ shieldName.Current ] ;
			GameObject shield3DObj = component.m_ComponentParam.m_Component3DObject.Obj ;
			if( true == component.IsOffline() )
				continue ;
			Vector3 dir = ( shield3DObj.transform.position - thisUnitPosition ) ;
			dir.Normalize() ;
			ret += dir ;
		}
		ret.Normalize() ;
		return ret ;
	}
	
	/*
	  正常運作的脈衝引擎貢獻的功效比例 = Sum( 存活的脈衝引擎功效值 * 能源配給比例 ) / 脈衝引擎的最大值總和
	 */	
	public float CalculateAliveImpulseEngineEffectRatio()
	{
		float ret = 1.0f ;
		float aliveTotalEffect = 0.0f ;
		float totalStdEffect = 0.0f ;// max
		// set all effect max and now to the division of impulse engine speed
		List<string> impulseEngines = GetAllComponentNameVecWithKeyword( ConstName.UnitDataComponentImpulseEnginePrefix ) ;
		if( 0 != impulseEngines.Count )
		{
			foreach( string impulseEngineStr in impulseEngines )
			{
				if( false == componentMap[ impulseEngineStr ].IsOffline() )
				{
					aliveTotalEffect += componentMap[ impulseEngineStr ].TotalEffect() ;
					// Debug.Log( "componentMap[ impulseEngineStr ].TotalEffect()" + componentMap[ impulseEngineStr ].TotalEffect() ) ;
				}
				totalStdEffect += componentMap[ impulseEngineStr ].m_Effect.max ;
			}
			
			if( 0.0f != totalStdEffect )
				ret = aliveTotalEffect / totalStdEffect ;
		}
		
		// Debug.Log( ret ) ;
		return ret ;
	}
	
	public string GetUnitDisplayName()
	{
		string ret = "" ;
		if( -1 != this.m_DisplayNameIndex )
			ret = StrsManager.Get( this.m_DisplayNameIndex ) ;
		return ret ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_UnitState.state = (int) UnitState.UnActive ;
		
		CreateComponents() ;
	}
	
	

	// Update is called once per frame
	void Update () 
	{
		// Debug.Log( this.gameObject.name ) ;
		Debug_HP = GetHPNow() ;
//		Debug_standardParameters = new StandardParameter[ this.standardParameters.Count ] ;
//		this.standardParameters.Values.CopyTo( Debug_standardParameters , 0 ) ;
		
		switch( (UnitState)this.m_UnitState.state )
		{
		case UnitState.UnActive :
			if( true == componentMap.ContainsKey( ConstName.UnitDataComponentUnitIntagraty ) )
				this.m_UnitState.state = (int)UnitState.Borning ;
			else 
				this.m_UnitState.state = (int)UnitState.Corpse ;
			break ;
		case UnitState.Corpse :
			break ;
		case UnitState.Borning :
			this.m_UnitState.state = (int)UnitState.Alive ;
			break ;
		case UnitState.Alive :
			if( 0 >= Debug_HP )
			{
				CreateDestroyReport() ;
				
				this.m_UnitState.state = (int)UnitState.Dying ;				
			}
			else
			{
				UnitDataCompnentUpdate() ;
			}
			break ;
		case UnitState.Dying :
			
			// keep playing dying effect
			float rotateSpeed = 10.0f ;
			Rigidbody rigidBody = this.gameObject.GetComponentInChildren<Rigidbody>() ;
			if( null != rigidBody )
			{
				rotateSpeed /= rigidBody.mass ;
			}
			this.gameObject.transform.Rotate( Vector3.up , rotateSpeed , Space.Self ) ;
			this.gameObject.transform.localScale *= 0.999f ;			

			float DyingAnimationTime = 3.0f ;
			if( this.m_UnitState.ElapsedFromLast() > DyingAnimationTime )
			{
				this.m_UnitState.state = (int)UnitState.Dead ;
			}			
			break ;
		case UnitState.Dead :
			break ;			
		}
	}
	
	// 更新部件資料
	void UnitDataCompnentUpdate()
	{
		foreach( string eComponent in this.componentMap.Keys )
		{
			UnitComponentData componentData = this.componentMap[ eComponent ] ;
			// Debug.Log( "UnitDataCompnentUpdate " + eComponent ) ;
			
			// update HP
			componentData.UpdateHP() ;

			// weapon reload
			WeaponReloadStatus last = componentData.m_WeaponReloadStatus ;
			componentData.UpdateReload() ;
			if( WeaponReloadStatus.WeaponReloadStatus_Full == componentData.m_WeaponReloadStatus &&
				componentData.m_WeaponReloadStatus != last )
			{
				// reload complete. add a reload completeness animation for this
				// Debug.Log( last + " " + componentData.m_WeaponReloadStatus ) ;
				SendReloadCompletenessMessage( componentData ) ;
			}
			
		}
	}
		
	
	private void CreateDestroyReport()
	{
		UnitDamageSystem damageSys = this.GetComponent<UnitDamageSystem>() ;
		if( null == damageSys )
			return ;
		if( 0 == damageSys.m_LastAttackerName.Length )
			return ;
		
		MessageQueueManager manager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != manager )
		{
			string unitDisplayName = this.gameObject.name ;
			if( -1 != this.m_DisplayNameIndex )
				unitDisplayName = StrsManager.Get( this.m_DisplayNameIndex ) ;
		
			string message = "" ;
			// 你已被摧毀
			if( this.gameObject.name == ConstName.MainCharacterObjectName )
			{
				message = StrsManager.Get( 1031 ) ;
				message = message.Replace( "%s" , damageSys.m_LastAttackerDisplayName ) ;
			}
			// 你已摧毀
			else if( damageSys.m_LastAttackerName == ConstName.MainCharacterObjectName )
			{
				message = StrsManager.Get( 1032 ) ;
				message = message.Replace( "%s" , unitDisplayName ) ;
			}
			else
			{
				return ;
			}
			
			manager.AddMessage( message ) ;	
		}
		
		BattleScoreManager battleScore = GlobalSingleton.GetBattleScoreManager() ;
		if( null != battleScore )
		{
			if( damageSys.m_LastAttackerName == ConstName.MainCharacterObjectName )
			{
				battleScore.AddScore( ScoreType.DestroyNum , 1 ) ;
			}
		}
	}
	
	// energy
	public void RetrieveAllComponentEnergy( out float _NowTotal , 
									  		out float _MaximumTotal )
	{
		_NowTotal = 0 ;
		_MaximumTotal = 0 ;
		List<string> components = GetAllComponentNameVecWithKeyword( "" ) ;
		foreach( string componentStr in components )
		{
			// Debug.Log( componentStr ) ;
			UnitComponentData component = componentMap[ componentStr ] ;
			_NowTotal += component.m_Energy.now ;
			_MaximumTotal += component.m_Energy.max ;
		}
	}
	
	public void TrySetImpulseEngineEnergy( float _SetTotalEnergyValue )
	{
		List<string> impluseEngines = GetAllImpulseEngineComponentNameVec() ;
		TrySetComponentsEnergyToValue( impluseEngines , _SetTotalEnergyValue ) ;
	}
	public void RetrieveImpulseEngineEnergy( out float _NowTotal , 
									  		 out float _MaximumTotal )
	{
		_NowTotal = 0 ;
		_MaximumTotal = 0 ;
		List<string> impulseEngines = GetAllImpulseEngineComponentNameVec() ;
		foreach( string impuliseEngine in impulseEngines )
		{
			UnitComponentData component = componentMap[ impuliseEngine ] ;
			_NowTotal += component.m_Energy.now ;
			_MaximumTotal += component.m_Energy.max ;
		}
	}
		
	public void TrySetSensorEnergy( float _SetTotalEnergyValue )
	{
		List<string> sensors = GetSensorComponents() ;
		TrySetComponentsEnergyToValue( sensors , _SetTotalEnergyValue ) ;
	}		
	public void RetrieveSensorEnergy( out float _Now , 
									  out float _Maximum )
	{
		_Now = 0 ;
		_Maximum = 0 ;
		UnitComponentData component = GetSensorComponent() ;
		if( null != component )
		{
			_Now = component.m_Energy.now ;
			_Maximum = component.m_Energy.max ;
		}
	}
	
	public void TrySetWeaponEnergy( float _SetTotalEnergyValue )
	{
		List<string> weapons = GetAllWeaponComponentNameVec() ;
		TrySetComponentsEnergyToValue( weapons , _SetTotalEnergyValue ) ;
	}
	public void RetrieveWeaponEnergy( out float _NowTotal , 
									  		 out float _MaximumTotal )
	{
		_NowTotal = 0 ;
		_MaximumTotal = 0 ;
		List<string> weapons = GetAllWeaponComponentNameVec() ;
		foreach( string weapon in weapons )
		{
			UnitComponentData component = componentMap[ weapon ] ;
			_NowTotal += component.m_Energy.now ;
			_MaximumTotal += component.m_Energy.max ;			
		}
	}
	

	public void TrySetShieldEnergy( float _SetTotalEnergyValue )
	{
		List<string> shields = GetAllShieldComponentNameVec() ;
		TrySetComponentsEnergyToValue( shields , _SetTotalEnergyValue ) ;
	}
	public void RetrieveShieldEnergy( out float _NowTotal , 
									  		 out float _MaximumTotal )
	{
		_NowTotal = 0 ;
		_MaximumTotal = 0 ;
		List<string> shields = GetAllShieldComponentNameVec() ;
		foreach( string shield in shields )
		{
			UnitComponentData component = componentMap[ shield ] ;
			_NowTotal += component.m_Energy.now ;
			_MaximumTotal += component.m_Energy.max ;				
		}
	}
	
	private void TrySetComponentsEnergyToValue( List<string> _components , float _SetValue )
	{
		if( false == HasAuxiliaryEnergy() )
			return ;
		if( 0 == _components.Count )
			return ;
		
		// 先取得補助能源目前值
		float auxiliaryEnergyNow = GetAuxiliaryEnergyValue() ;
		
		// 取得部件能源總量 目前與最大值
		float NowTotal = 0 ;
		float MaximumTotal = 0 ;
		foreach( string componentStr in _components )
		{
			UnitComponentData component = componentMap[ componentStr ] ;
			NowTotal += component.m_Energy.now ;
			MaximumTotal += component.m_Energy.max ;				
		}
		// Debug.Log( "NowTotal" + NowTotal ) ;
		// Debug.Log( "MaximumTotal" + MaximumTotal ) ;
		
		// 設定值不可超過最大值
		if( _SetValue > MaximumTotal )
			_SetValue = MaximumTotal ;

		// 修改值不能 超過 補助能源
		float energyRaiseExpected = _SetValue - NowTotal ;// 能源提高量正為提高(會耗損補助能源),為負則會相反提高補助能源
		if( auxiliaryEnergyNow < energyRaiseExpected )// 提高量超出 補助能源 不可提高
		{
			energyRaiseExpected = auxiliaryEnergyNow ;// 至多只能耗盡補助能源
		}
		
		// Debug.Log( "energyRaiseExpected" + energyRaiseExpected ) ;		
		// Debug.Log( "auxiliaryEnergyNow - energyRaiseExpected" + (auxiliaryEnergyNow - energyRaiseExpected) ) ;		
		// 修改補助能源總量
		float auxiliaryEnergyExpected = auxiliaryEnergyNow - energyRaiseExpected ;
		float realRaiseInAxuiliary = SetAuxiliaryEnergyValue( auxiliaryEnergyExpected ) ;
		// Debug.Log( "realRaise" + realRaiseInAxuiliary ) ;		
		// 分給每個元件
		float energyRaiseToEachComponenetActually = -1 * realRaiseInAxuiliary / _components.Count ;
		foreach( string componentStr in _components )
		{
			UnitComponentData component = componentMap[ componentStr ] ;
			component.m_Energy.now += energyRaiseToEachComponenetActually ;			
		}
	}
	
	// Auxiliary Energy
	public bool HasAuxiliaryEnergy()
	{
		return ( true == this.standardParameters.ContainsKey( ConstName.UnitDataComponentAuxiliaryEnergy ) ) ;
	}
	public void GetAuxiliaryEnergyValue( out float _Now , out float _Max )
	{
		_Now = 0 ;
		_Max = 0 ;
		if( true == HasAuxiliaryEnergy() )
		{
			_Now = this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].now ;
			_Max = this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].max ;
		}
	}	
	public float GetAuxiliaryEnergyValue()
	{
		float Now = 0 ;
		float Max = 0 ;
		GetAuxiliaryEnergyValue( out Now , out Max ) ;
		return Now ;
	}
	public float SetAuxiliaryEnergyValue( float _SetValue )
	{
		float realRaise = 0.0f ;
		if( true == HasAuxiliaryEnergy() )
		{
			if( this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].max < _SetValue )
			{
				_SetValue = this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].max ;
			}
			realRaise = _SetValue - this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].now ;
			this.standardParameters[ ConstName.UnitDataComponentAuxiliaryEnergy ].now = _SetValue ;
		}
		return realRaise ;
	}	
}

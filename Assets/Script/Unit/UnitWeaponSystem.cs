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
@file UnitWeaponSystem.cs
@brief 單位的武器系統 
@author NDark

# CreateWeapons() 負責呼叫UnitData創造武器
# 創造後的資料集合放在 m_WeaponDataMap 容器中
# 結束時會去摧毀相關的物件
# m_WeaponRangeActiveTimer 紀錄顯示武器範圍的資料結構
# UpdateWeaponFire() 負責武器特效的發射狀態開始,更新與停止
# FindAbleToFireWeaponComponent() 只檢查是否有可以射擊的武器(不檢查是否可擊中)
# FindAbleToShootTargetWeaponComponent() 只檢查是否可以擊中的武器(不檢查是否填充完畢)
# FindAbleWeaponComponent() 取得可以發射且可以攻擊到目標的的武器部件
# ActiveWeapon() 發射武器
# ActiveEffect() 啟動(顯示)特效 
# StopEffect() 停止特效及發射狀態
# WeaponIsFiring() 檢查武器是否發射中
# FindMaximumWeaponRange() 取得各武器中最小攻擊距離
# ActiveWeaponRangeObject() 啟動武器距離物件的顯示
# UpdateWeaponRangeTimers() 更新武器範圍的計時器，如果計時結束則移除該些武器範圍物件以及該計時器
# WeaponReloadIsReady() 檢查武器填充狀態是否完成


@date 20121029 by NDark . move WeaponDataSet to WeaponDataSet.cs
...
@date 20130103 by NDark . change argument type of class method ActiveWeapon()
@date 20130105 by NDark . fix an error that will change _TargetUnit at ActiveWeapon()
@date 20130107 by NDark . close checking of targetobject at UpdateWeaponFire()
@date 20130110 by NDark . remove damage cause of phaser at UpdateWeaponFire()
@date 20130113 by NDark . comment.
@date 20130119 by NDark 
. change string of out of range at FindAbleWeaponComponent()
. remove code of determine that target does not exist at UpdateWeaponFire()
. move class method IsInScreen() to Mathmatic.cs
@date 20130204 by NDark
. modify code of m_RelativeDamageEffectNames at ActiveWeapon()
. modify code of m_RelativeDamageEffectNames at StopEffect()

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;


/*
 * @brief UnitWeaponSystem
 */
public class UnitWeaponSystem : MonoBehaviour {
	
	/* The all weapons ( and their */
	public Dictionary< string /*component name*/ , WeaponDataSet > m_WeaponDataMap = 
		new Dictionary<string, WeaponDataSet>() ;
	
	[System.Serializable]
	public class WeaponRangeActiveTimerStruct
	{
		public NamedObject m_WeaponRangeActiveObject = new NamedObject() ;
		public CountDownTrigger m_Timer = new CountDownTrigger() ;
	}
	public Dictionary< string , WeaponRangeActiveTimerStruct > m_WeaponRangeActiveTimer = new Dictionary<string, WeaponRangeActiveTimerStruct>() ;
		
	// 檢查武器是否發射中 
	public bool WeaponIsFiring( string _Keyword )
	{
		bool ret = false ;
		foreach( WeaponDataSet dataSet in m_WeaponDataMap.Values )
		{
			if( -1 != dataSet.ComponentName.IndexOf( _Keyword ) )
			{
				ret = ( dataSet.m_FireState != WeaponFireState.Ready ) ;
			}
		}
		return ret ;
	}
	
	// 停止特效及發射狀態
	public void StopEffect( string _Keyword )
	{
		foreach( WeaponDataSet dataSet in m_WeaponDataMap.Values )
		{
			if( -1 != dataSet.ComponentName.IndexOf( _Keyword ) )
			{
				if( dataSet.m_FireState == WeaponFireState.Firing )
				{
					StopEffect( dataSet , dataSet.TargetUnitObject ) ;
					dataSet.m_FireState = WeaponFireState.FireCompleting ;
				}
				return ;
			}
		}
	}
	
	// 娶得指定武器的最大距離
	public float FindMaximumWeaponRange( string _WeaponKeyword )
	{
		float minWeaponRange = 9999.0f ;
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return minWeaponRange ;
		
		List<string> WeaponList = unitData.GetAllWeaponComponentNameVec() ;
		foreach( string weaponstr in WeaponList )
		{
			UnitComponentData weaponcomponent = unitData.componentMap[ weaponstr ] ;
			if( true == m_WeaponDataMap.ContainsKey( weaponstr ) &&
				-1 != weaponstr.IndexOf( _WeaponKeyword ) )
			{
				float weaponDistance = weaponcomponent.m_WeaponParam.m_Range ;
				if( weaponDistance < minWeaponRange )
				{
					minWeaponRange = weaponDistance ;
				}
			}
		}
		return minWeaponRange ;
	}
	
	// 只檢查是否有可以射擊的武器(不檢查是否可擊中)
	public string FindAbleToFireWeaponComponent( string _WeaponKeyword ) 
	{
		string ret = "" ; 
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ret ;
		
		List<string> WeaponList = unitData.GetAllWeaponComponentNameVec() ;
		foreach( string weaponStr in WeaponList )
		{
			if( false == m_WeaponDataMap.ContainsKey( weaponStr ) ||
				-1 == weaponStr.IndexOf( _WeaponKeyword ) )
				continue ;
			
			UnitComponentData weaponComponent = unitData.componentMap[ weaponStr ] ;
			
			// check hp
			if( true == weaponComponent.IsOffline() ||
				false == WeaponReloadIsReady( weaponComponent ) )
				continue ;
					
			ret = weaponStr ;
			break ;
		}
		return ret ;
	}
	
	// 只檢查是否可以擊中的武器(不檢查是否填充完畢)
	public string FindAbleToShootTargetWeaponComponent( string _WeaponKeyword , 
														GameObject _TargetObject ) 
	{
		string ret = "" ; 
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData ||
			null == _TargetObject )
			return ret ;
		
		Vector3 vecToTar = Vector3.zero ;
		Vector3 vecToTarNor = Vector3.zero ;
		float angleToTar = 0.0f ;
		float crossOfUp = 0.0f ;
		
		List<string> WeaponList = unitData.GetAllWeaponComponentNameVec() ;
		foreach( string weaponStr in WeaponList )
		{
			if( false == m_WeaponDataMap.ContainsKey( weaponStr ) ||
				-1 == weaponStr.IndexOf( _WeaponKeyword ) )
				continue ;
			
			WeaponDataSet weaponSet = m_WeaponDataMap[ weaponStr ] ;
			UnitComponentData weaponComponent = unitData.componentMap[ weaponStr ] ;
			
			// check hp
			if( true == weaponComponent.IsDown() )
				continue ;

			float weaponAngle = weaponComponent.m_WeaponParam.m_Angle ;
			float weaponDistance = weaponComponent.m_WeaponParam.m_Range ;
			GameObject weapon3DObject = weaponSet.Component3DObject ;
			
			if( true == MathmaticFunc.FindUnitRelation( weapon3DObject , _TargetObject , 
				ref vecToTar , ref vecToTarNor , 
				ref angleToTar , ref crossOfUp ) )
			{
				// Debug.Log( "vecToTar.magnitude=" + vecToTar.magnitude + " angleToTar=" + angleToTar ) ;
				if( vecToTar.magnitude < weaponDistance && 
					angleToTar < weaponAngle )
				{
					ret = weaponStr ;
					break ;					
				}
			}
		

		}
		return ret ;
	}	
	
	public string FindAbleWeaponComponent( string _WeaponKeyword ,
										   string _TargetObjName ,
										   ref string _mainCause )
	{
		GameObject targetObj = GameObject.Find( _TargetObjName ) ;
		if( null == targetObj )
		{
			// Debug.Log( "null == targetObj" + _TargetObjName ) ;
			return "" ;
		}				
		return FindAbleWeaponComponent( _WeaponKeyword , 
										targetObj , 
										ref _mainCause ) ;
	}
	
	public string FindAbleWeaponComponent( string _WeaponKeyword ,
										   string _TargetObjName )
	{
		string mainCause = "" ;
		GameObject targetObj = GameObject.Find( _TargetObjName ) ;
		if( null == targetObj )
		{
			Debug.Log( "null == targetObj" + _TargetObjName ) ;
			return "" ;
		}				
		string ret = FindAbleWeaponComponent( _WeaponKeyword , 
										targetObj , 
										ref mainCause ) ;
		// Debug.Log( "mainCause=" + mainCause ) ;
		return ret ;		
	}	
	
	public string FindAbleWeaponComponent( string _WeaponKeyword ,
										   GameObject _TargetObj )
	{
		string mainCause = "" ;
		string ret = FindAbleWeaponComponent( _WeaponKeyword , _TargetObj , ref mainCause ) ;
		// Debug.Log( "mainCause=" + mainCause ) ;
		return ret ;
	}	
	
	/*
	 * @brief Find a wapon component can fire to the target.
	 */
	public string FindAbleWeaponComponent( string _WeaponKeyword ,
										   GameObject _TargetObj , 
										   ref string _mainCause )
	{
		/* find able weapon from keyword. */
		/*
		Debug.Log( "FindAbleWeaponComponent() _WeaponKeyword=" + 
					_WeaponKeyword + 
		 			" _TargetObjName=" + 
					_TargetObjName ) ;
		//*/
		_mainCause = "" ;
		string ret = "" ; 
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ret ;
		if( null == _TargetObj )
			return ret ;
		
		
		Vector3 vecToTar = Vector3.zero ;
		Vector3 vecToTarNor = Vector3.zero ;
		float angleToTar = 0.0f ;
		float crossOfUp = 0.0f ;		
		
		List<string> WeaponList = unitData.GetAllWeaponComponentNameVec() ;
		foreach( string weaponstr in WeaponList )
		{
			
			if( false == m_WeaponDataMap.ContainsKey( weaponstr ) ||
				-1 == weaponstr.IndexOf( _WeaponKeyword ) )
				continue ;
			
			UnitComponentData weaponcomponent = unitData.componentMap[ weaponstr ] ;
			WeaponDataSet weaponSet = m_WeaponDataMap[ weaponstr ] ;
			

			
			// check able fire
			float weaponAngle = weaponcomponent.m_WeaponParam.m_Angle ;
			float weaponDistance = weaponcomponent.m_WeaponParam.m_Range ;
			
			// Debug.Log( "weaponstr" + weaponstr + " weaponDistance=" + weaponDistance + " weaponAngle=" + weaponAngle ) ;
			
			GameObject weapon3DObject = weaponSet.Component3DObject ;
			
			if( true == MathmaticFunc.FindUnitRelation( weapon3DObject , _TargetObj , 
				ref vecToTar , ref vecToTarNor , 
				ref angleToTar , ref crossOfUp ) )
			{
				// Debug.Log( "vecToTar.magnitude=" + vecToTar.magnitude + " " + angleToTar ) ;
				if( vecToTar.magnitude < weaponDistance && 
					angleToTar < weaponAngle )
				{
					// check hp
					if( true == weaponcomponent.IsOffline() )
					{
						// Debug.Log( weaponcomponent.componentStatus ) ;
						_mainCause = StrsManager.Get( 1041 ) ;
						continue ;
					}
					
					// check reload energy
					if( false == WeaponReloadIsReady( weaponcomponent ) )
					{
						_mainCause = StrsManager.Get( 1042 ) ;
						continue ;
					}
							
					ret = weaponstr ;
					// Debug.Log( "ret =" + ret ) ;
					break ;
				}
				else if( 0 == _mainCause.Length ) 
				{
					_mainCause = StrsManager.Get( 1043 ) ;
				}
			}
		}

		return ret ;
	}

	
	// 發射武器 
	public bool ActiveWeapon( string _WeaponComponentName , 
					   		  NamedObject _TargetUnit , 
							  string _TargetComponentName )
	{
		/*
		Debug.Log( "ActiveWeapon()" + 
				   this.gameObject.name + " " + 
				   _WeaponComponentName + " " + 
				   _TargetUnit.Name + " "+ 
					_TargetComponentName ) ;
		//*/
		SystemLogManager.AddLog( SystemLogManager.SysLogType.FireWeapon , 
			this.gameObject.name + ":" + _WeaponComponentName + ":" + _TargetUnit.Name + ":" + _TargetComponentName ) ;
		
		// get weapon data
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
		{
			Debug.Log( "ActiveWeapon() null == unitData" ) ;
			return false ;
		}
		
		if( false == unitData.componentMap.ContainsKey( _WeaponComponentName ) )
		{
			Debug.Log( "ActiveWeapon() no such component:" + _WeaponComponentName + ".") ;
			return false ;
		}
		UnitComponentData weaponComponent = unitData.componentMap[ _WeaponComponentName ] ;
		
			
		WeaponDataSet weaponSet = null ;
		if( false == this.m_WeaponDataMap.ContainsKey( _WeaponComponentName ) )
		{
			Debug.Log( "false == this.m_WeaponDataMap.ContainsKey( _WeaponComponentName ):" + _WeaponComponentName ) ;
			return false ;
		}
		weaponSet = this.m_WeaponDataMap[ _WeaponComponentName ] ;
		WeaponParam weaponParam = weaponComponent.m_WeaponParam ;
		
		// weapon data
		float weapenDamage = weaponComponent.TotalEffect() ;
		// Debug.Log( "weapenDamage" + weapenDamage ) ;
		float weapenAccuracy = weaponParam.m_Accuracy ;// 0~1
		// Debug.Log( "weapenAccuracy" + weapenAccuracy ) ;
		float weaponMissRage = 1.0f - weapenAccuracy ;
		float sceneMaximumMissDistance = 1.0f ;	
		
		// check able to fire
		if( WeaponFireState.Ready != weaponSet.m_FireState )
		{
			// Debug.Log( "WeaponFireState.Ready != weaponSet.m_FireState"  ) ;
			return false ;
		}
		
		// get Target Unit Object : the unit contains data.
		if( null == _TargetUnit.Obj )
		{
			Debug.Log( "null == weaponSet.TargetUnitObject"  ) ;
			return false ;
		}
		
		// find out hit component name
		UnitDamageSystem unitDamageSys = _TargetUnit.Obj.GetComponent< UnitDamageSystem >() ;
		if( null == unitDamageSys )
		{
			Debug.Log( "ActiveWeapon() : null == unitDamageSys" ) ;			
			return false ;
		}
		
		// 取得被擊中的部件
		string realUnitName = "" ;
		string realComponentObjectName = "" ;
		if( false == unitDamageSys.RetrieveRealTargetComponent( this.gameObject , 
														 		_TargetComponentName ,
																ref realUnitName ,
																ref realComponentObjectName ) )
		{
			Debug.Log( "unitDamageSys.RetrieveRealTargetComponent fail" ) ;
			return false ;
		}
		if( _TargetUnit.Name != realUnitName )
		{
			NamedObject unit = new NamedObject( realUnitName ) ;
			unitDamageSys = unit.Obj.GetComponent< UnitDamageSystem >() ;
			if( null == unitDamageSys )
			{
				Debug.Log( "ActiveWeapon() : null == unitDamageSys realUnitName=" + realUnitName ) ;			
				return false ;
			}
		}
		weaponSet.SetupTarget( realUnitName , realComponentObjectName ) ;
		
		
		// true Hit Component Object , it may the same with target unit object.
		// Debug.Log( "weaponSet.TargetUnitName=" + weaponSet.TargetUnitName ) ;
		// Debug.Log( "weaponSet.TargetComponentObjectName=" + weaponSet.TargetComponentObjectName ) ;
		
		unitDamageSys = weaponSet.TargetUnitObject.GetComponent< UnitDamageSystem >() ;
		if( null == unitDamageSys )
		{
			// Debug.Log( "ActiveWeapon() : null == unitDamageSys" ) ;
			return false ;
		}			
		
		// Debug.Log( "ActiveWeapon() TargetComponentName" + realTargetComponentName ) ;
		
		// create corresponding weapong effect object
		if( null == weaponSet.Effect3DObject )
		{
			Debug.Log( "null == weaponSet.Effect3DObject:" + weaponSet.Effect3DObjectName ) ;
			return false ;
		}
		
		float damageCause = 0.0f ;
		// calculate displacement 
		{
			int randomMax = 100 ;
			int randomValue = Random.Range( 0 , randomMax ) ;
			
			int halfRandomValue = randomMax / 2 ;			
			float randomRatio = (randomMax - randomValue) / (float) randomMax ;
			damageCause = weapenDamage * ( weapenAccuracy + randomRatio * weaponMissRage ) ;
			
			float halfValue = (float)( randomValue - halfRandomValue ) / (float) ( randomMax );
			float randomDisplacement = 0.0f ;
			randomDisplacement = sceneMaximumMissDistance * halfValue ;
			
			float positiveInX = 1.0f;
			if( 1 == Random.Range( 0 , 1 ) )
				positiveInX = -1.0f ;
			
			float positiveInZ = 1.0f;
			if( 1 == Random.Range( 0 , 1 ) )
				positiveInZ = -1.0f ;
			
			weaponSet.m_Displacement = new Vector3( randomDisplacement * positiveInX , 
												  0 , 
												  randomDisplacement * positiveInZ ) ;
			
		}	
		
		weaponSet.m_CauseDamage = damageCause ; 
		
		if( weaponSet.m_WeaponType == WeaponType.Phaser )
		{
			weaponSet.m_FireTotalTime = 2.1f ;// 造成兩次觸發
			weaponSet.m_FireStartTime = Time.time ;
			
			WeaponPhaserEffect phaserEffect = weaponSet.Effect3DObject.GetComponent< WeaponPhaserEffect >() ;
			if( null != phaserEffect )
			{
				phaserEffect.Setup( weaponSet ) ;
			}


			
			// cause damage effect imidiatly
			if( null != unitDamageSys )
			{
				string damageEffectName = unitDamageSys.ActiveDamageEffect( this.gameObject , 
																			weaponSet.TargetComponentObjectName ) ;
				if( 0 != damageEffectName.Length )
				{
					RelativeDamageEffect relativeDamageEffect = new RelativeDamageEffect() ;
					relativeDamageEffect.m_TargetUnit = weaponSet.TargetUnitObject ;
					relativeDamageEffect.m_DamageEffectName = damageEffectName ;
					weaponSet.m_RelativeDamageEffectNames.Add( relativeDamageEffect ) ;
				}
			}
		}
		else if( weaponSet.m_WeaponType == WeaponType.TrakorBeam )
		{
			WeaponTrackorBeamEffect trackorBeamEffect = weaponSet.Effect3DObject.GetComponent< WeaponTrackorBeamEffect >() ;
			if( null != trackorBeamEffect )
			{
				weaponSet.m_CauseDamage = weaponComponent.TotalEffect() ;
				trackorBeamEffect.Setup( weaponSet ) ;
			}

			weaponSet.m_FireTotalTime = 999.0f ;
			weaponSet.m_FireStartTime = Time.time ;
		}
		else if( weaponSet.m_WeaponType == WeaponType.Torpedo )
		{
			weaponSet.Effect3DObject.transform.position = weaponSet.Component3DObject.transform.position ;
			weaponSet.m_TargetDirection = weaponSet.TargetComponentObject.transform.position - weaponSet.Effect3DObject.transform.position ;
			weaponSet.m_TargetDirection.Normalize() ;
			
			WeaponPhotonTorpedoEffect torpedoEffect = weaponSet.Effect3DObject.GetComponent< WeaponPhotonTorpedoEffect >() ;
			if( null != torpedoEffect )
			{
				torpedoEffect.Setup( weaponSet ) ;
			}

			weaponSet.m_FireTotalTime = 10.0f ;
			weaponSet.m_FireStartTime = Time.time ;				
		}
		else if( weaponSet.m_WeaponType == WeaponType.Cannon )
		{
			weaponSet.Effect3DObject.transform.position = weaponSet.Component3DObject.transform.position ;
			weaponSet.m_TargetDirection = weaponSet.TargetComponentObject.transform.position - weaponSet.Effect3DObject.transform.position ;
			weaponSet.m_TargetDirection.Normalize() ;
			
			WeaponCannonEffect cannonEffect = weaponSet.Effect3DObject.GetComponent< WeaponCannonEffect >() ;
			if( null != cannonEffect )
			{
				cannonEffect.Setup( weaponSet ) ;
			}

			weaponSet.m_FireTotalTime = 10.0f ;
			weaponSet.m_FireStartTime = Time.time ;				
		}		
		else 
		{
			Debug.Log( "weaponSet.m_WeaponType=" + weaponSet.m_WeaponType ) ;
		}
		
		
			
		ActiveEffect( weaponSet ) ;

		
		weaponSet.m_FireState = WeaponFireState.FireAnimating ;
		
		// weapn fire, clear the reload energy		
		weaponComponent.m_ReloadEnergy.Clear() ;
		
		return true ;
	}
	
	public void ActiveWeaponRangeObject( string _Keyword ) 
	{
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
			
		List<string> weaponList = unitData.GetAllWeaponComponentNameVec() ;
		foreach( string componentStr in weaponList )
		{
			if( -1 == componentStr.IndexOf( _Keyword ) )
				continue ;
			// Debug.Log( "componentStr" + componentStr ) ;
			UnitComponentData componentData = unitData.componentMap[ componentStr ] ;
			WeaponDataSet dataSet = m_WeaponDataMap[ componentStr ] ;
			WeaponParam weaponParam = componentData.m_WeaponParam ;
			string weaponRangeObjName = ConstName.CreateWeaponRangeObjectName( dataSet.Component3DObject.name ) ;
			if( true == m_WeaponRangeActiveTimer.ContainsKey( weaponRangeObjName ) )
			{
				m_WeaponRangeActiveTimer[ weaponRangeObjName ].m_Timer.Rewind() ;
			}
			else
			{
				GameObject weaponRangeObj = MathmaticFunc.CreateWeaponRangeObject( dataSet.Component3DObject.name , 
					weaponParam.m_Range , 
					weaponParam.m_Angle ) ;
				if( null != weaponRangeObj )
				{
					weaponRangeObj.transform.position = dataSet.Component3DObject.transform.position ;
					weaponRangeObj.transform.rotation = dataSet.Component3DObject.transform.rotation ;
					weaponRangeObj.transform.parent = dataSet.Component3DObject.transform ;
				}
				WeaponRangeActiveTimerStruct addTimer = new WeaponRangeActiveTimerStruct() ;
				addTimer.m_WeaponRangeActiveObject.Name = weaponRangeObj.name ;
				addTimer.m_WeaponRangeActiveObject.Obj = weaponRangeObj ;
				addTimer.m_Timer.Setup( 1.0f ) ;
				addTimer.m_Timer.Rewind() ;
			
				m_WeaponRangeActiveTimer[ weaponRangeObj.name ] = addTimer ;
			}
				
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		CreateWeapons() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Dictionary< string , WeaponDataSet >.Enumerator eNum = m_WeaponDataMap.GetEnumerator() ;
		while( eNum.MoveNext() )
		{
			UpdateWeaponFire( eNum.Current.Value ) ;
		}
		
		UpdateWeaponRangeTimers() ;
	}
	
	void OnDestroy()
	{
		// 結束時會去摧毀相關的物件
		foreach( WeaponDataSet weaponData in m_WeaponDataMap.Values )
		{
			// @todo check null
			if( null != weaponData.Effect3DObjectGetObj() )
			{
				// Debug.Log( "GameObject.Destroy( weaponData.Effect3DObject ) ;" ) ;
				GameObject.Destroy( weaponData.Effect3DObject ) ;
			}
		}
	}	
	
	// 負責呼叫UnitData創造武器
	private void CreateWeapons()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>();
		if( null == unitData )
		{
			Debug.Log( "CreateWeapons() : null == unitData" ) ;
			return ;
		}
		unitData.CreateWeapons( ref m_WeaponDataMap ) ;
	}
	
	// 啟動(顯示)特效 
	void ActiveEffect( WeaponDataSet _weaponSet )
	{
		if( null != _weaponSet.Effect3DObject )
		{
			_weaponSet.Effect3DObject.GetComponent<Renderer>().enabled = true ;
			_weaponSet.Effect3DObject.GetComponent<Collider>().enabled = true ;
		}
	}
	
	// 停止特效
	void StopEffect( WeaponDataSet _weaponset , GameObject _targetUnit )
	{

		
		// Debug.Log( "StopEffect() " + _weaponset.ComponentName ) ;
		// close weapon effect
		if( null != _weaponset.Effect3DObject )
		{
			_weaponset.Effect3DObject.GetComponent<Renderer>().enabled = false ;
			_weaponset.Effect3DObject.GetComponent<Collider>().enabled = false ;
		}
		
		// close relative damage effect ( if they exist )
		List<RelativeDamageEffect>.Enumerator e = _weaponset.m_RelativeDamageEffectNames.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( null != e.Current.m_TargetUnit )
			{
				UnitDamageSystem dmgSys = null ;
				dmgSys = e.Current.m_TargetUnit.GetComponent<UnitDamageSystem>() ;
				if( null != dmgSys )
				{
					dmgSys.StopEffectObject( e.Current.m_DamageEffectName ) ;
				}
			}
		}
		_weaponset.m_RelativeDamageEffectNames.Clear() ;
		
	}
	
	// 更新武器的發射狀態
	void UpdateWeaponFire( WeaponDataSet weaponset )
	{
		switch( weaponset.m_FireState )
		{
		case WeaponFireState.UnActive :
			weaponset.m_FireState = WeaponFireState.Ready ;
			break ;
		case WeaponFireState.Ready :
			// nothing and wait for fire
			break ;
		case WeaponFireState.FireAnimating :
			if( null != weaponset.m_FireAudio &&
				null != weaponset.Component3DObject &&
				null != weaponset.TargetUnitObject &&
				true == MathmaticFunc.IsInScreen( this.gameObject ) )
			{
			
				if( weaponset.m_WeaponType == WeaponType.TrakorBeam )
				{
					// Debug.Log( "weaponset.Component3DObject.audio.Play" ) ;
					weaponset.Component3DObject.GetComponent<AudioSource>().clip = weaponset.m_FireAudio ;
					weaponset.Component3DObject.GetComponent<AudioSource>().Play() ;
				}
				else
					weaponset.Component3DObject.GetComponent<AudioSource>().PlayOneShot( weaponset.m_FireAudio ) ;
			}
			
			weaponset.m_FireState = WeaponFireState.Firing ;
			break ;
		case WeaponFireState.Firing :
		
	 		if( null != weaponset.Effect3DObject )
			{
				if( false == weaponset.Effect3DObject.GetComponent<Renderer>().enabled )
				{
					// 特效物件自己關閉了
					// Debug.Log( "false == weaponset.Effect3DObject.renderer.enabled" ) ;
					StopEffect( weaponset , weaponset.TargetUnitObject ) ;
					weaponset.m_FireState = WeaponFireState.FireCompleting ;
				}


			}

			// check time is up
			if( Time.time - weaponset.m_FireStartTime > weaponset.m_FireTotalTime )
			{
				StopEffect( weaponset , weaponset.TargetUnitObject ) ;
				weaponset.m_FireState = WeaponFireState.FireCompleting ;
			}
			break ;
		case WeaponFireState.FireCompleting :
			if( null != weaponset.m_FireAudio &&
				null != weaponset.Component3DObject )
			{
				// Debug.Log( "weaponset.Component3DObject.audio.Stop" ) ;
				weaponset.Component3DObject.GetComponent<AudioSource>().Stop() ;
			}			
			weaponset.m_FireState = WeaponFireState.Recharging ;
			break ;	
		case WeaponFireState.Recharging :
			weaponset.m_FireState = WeaponFireState.Ready ;
			break ;
		}
	}
	
	void UpdateWeaponRangeTimers()
	{
		List<string> removeList = new List<string>() ;
		Dictionary< string , WeaponRangeActiveTimerStruct >.Enumerator timerEnum = m_WeaponRangeActiveTimer.GetEnumerator() ;
		while( timerEnum.MoveNext() )
		{
			if( true == timerEnum.Current.Value.m_Timer.IsCountDownToZero() )
			{
				GameObject.Destroy( timerEnum.Current.Value.m_WeaponRangeActiveObject.Obj ) ;
				removeList.Add( timerEnum.Current.Key ) ;
			}
		}
		
		foreach( string key in removeList ) 
		{
			m_WeaponRangeActiveTimer.Remove( key ) ;
		}
	}
	
	private bool WeaponReloadIsReady( UnitComponentData _WeaponComponent )
	{
		return ( _WeaponComponent.m_WeaponReloadStatus == WeaponReloadStatus.WeaponReloadStatus_Full ) ;
	}
	

	
}

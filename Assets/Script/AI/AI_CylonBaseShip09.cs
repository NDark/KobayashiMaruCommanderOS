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
@file AI_CylonBaseShip09.cs
@author NDark

第九關賽隆基地船的特製AI

# 會產生賽隆私掠者
# 會產生賽隆導彈
# 週期檢查產生固定三個私掠者
# 週期檢查產生兩個導彈
# ReplaceTargetName() 被外界呼叫更改目標，同時會更改存活中的私掠者與導彈的目標


@date 20130106 file started.
@date 20130106 by NDark 
. add class method ReplaceTargetName()
. add class member m_RaiderTimer
@date 20130107 by NDark
. add DetectRange of missile
. adjust damage of missile
. adjust launch timer of missile.
@date 20130119 by NDark
. change type of class member m_TargetUnit to UnitObject
. add SetState() at ReplaceTargetName()
@date 20130204 by NDark . refine code.

*/
using UnityEngine;
using System.Collections.Generic;

public class AI_CylonBaseShip09 : AI_Base 
{
	
	UnitObject m_TargetUnit = new UnitObject() ; // 敵人目標，會從參數取得
	
	// 目前存活的賽隆私掠者
	Dictionary<string , NamedObject > m_Raiders = new Dictionary<string, NamedObject>() ;
	CountDownTrigger m_RaiderTimer = new CountDownTrigger( 1.0f ) ;// 賽隆私掠者生產週期
	
	// 目前存活的塞隆基地船導彈
	Dictionary<string , NamedObject > m_Missile = new Dictionary<string, NamedObject>() ;
	CountDownTrigger m_MissileTimer = new CountDownTrigger( 20.0f ) ;// 賽隆基地船導彈生產週期
	
	// 被外界呼叫更改目標，同時會更改存活中的私掠者與導彈的目標
	public void ReplaceTargetName( string _Name )
	{
		m_TargetUnit.Setup( _Name , null  ) ;
		if( null != m_TargetUnit.Obj )
		{
			foreach( NamedObject obj in m_Raiders.Values )
			{
				if( 0 != obj.Name.Length &&
					null != obj.Obj )
				{
					AI_FireToTarget5 ai = obj.Obj.GetComponent<AI_FireToTarget5>() ;
					ai.ChangeTarget( m_TargetUnit ) ;
				}
			}
			foreach( NamedObject obj in m_Missile.Values )
			{
				if( 0 != obj.Name.Length &&
					null != obj.Obj )
				{
					AI_CollideTo ai = obj.Obj.GetComponent<AI_CollideTo>() ;
					ai.ChangeTarget( (NamedObject) m_TargetUnit ) ;
				}
			}
			this.SetState( AIBasicState.Active ) ;// 重新設定狀態，避免沒敵人了狀態掉到End
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		m_Raiders.Add( "CylonRaiderSpawnChildObject0" , new NamedObject() ) ;
		m_Raiders.Add( "CylonRaiderSpawnChildObject1" , new NamedObject() ) ;
		m_Raiders.Add( "CylonRaiderSpawnChildObject2" , new NamedObject() ) ;
		
		m_Missile.Add( "CylonMissileSpawnObject0" , new NamedObject() ) ;
		m_Missile.Add( "CylonMissileSpawnObject1" , new NamedObject() ) ;	
		
		m_MissileTimer.Rewind() ;
		m_RaiderTimer.Rewind() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( ref unitData ) )
			return ;
		
		m_State.Update() ;
		switch( (AIBasicState) m_State.state )
		{
		case AIBasicState.UnActive :
			SetState( AI_Base.AIBasicState.Initialized ) ;
			break ;
			
		case AIBasicState.Initialized :
			RetrieveData() ;
			SetState( AI_Base.AIBasicState.Active ) ;
			break ;
			
		case AIBasicState.Active :
			if( false == CheckTarget( m_TargetUnit ) )
			{
				SetState( AI_Base.AIBasicState.Closed ) ;
				break ;
			}
			
			// 檢察產生私掠者
			TryGenerateUnit( unitData ) ;
			
			// 檢察產生導彈
			TryLanunchMissile( unitData ) ;
			break ;
			
		case AIBasicState.Closed :
			break ;			
		}
	}
	
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		
		string TargetName = "" ;
		if( true == RetrieveParam( unitData , "TargetName" , ref TargetName ) )
		{
			m_TargetUnit.Setup( TargetName , null ) ;			
		}		
		return true ;
	}
		
	private void TryGenerateUnit( UnitData unitData ) 
	{
		if( false == m_RaiderTimer.IsCountDownToZero() )
			return ;		
#if DEBUG_LOG		
		Debug.Log( "TryGenerateUnit" ) ;
#endif
		
		Dictionary<string,NamedObject>.Enumerator e = m_Raiders.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( null == e.Current.Value.Obj )
			{
				Transform trans = this.gameObject.transform.FindChild( e.Current.Key ) ;
				if( null == trans )
					continue ;				
				GameObject swapnObj = trans.gameObject ;

				EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
				UnitGenerationData addData = new UnitGenerationData() ;
				addData.initPosition.Setup( swapnObj.transform.position ) ;
				addData.initOrientation = swapnObj.transform.rotation ;
				addData.raceName = unitData.m_RaceName ;
				addData.sideName = unitData.m_SideName ;
				
				addData.unitName = "Enemy_CylonRaider" + ConstName.GenerateIterateString() ;
				addData.prefabName = "Template_CylonRaider01" ;
				addData.unitDataTemplateName = "UnitDataTemplate_CylonRaider09" ;
				addData.time = Time.timeSinceLevelLoad ;
				
				addData.supplementalVec.Add( "TargetName" , m_TargetUnit.Name ) ;
				addData.supplementalVec.Add( "WaitForReloadSec" , "1.0" ) ;
				
				// Debug.Log( "enemyGenerator.InsertEnemyGenerationTable()" ) ;				
				enemyGenerator.InsertEnemyGenerationTable( addData ) ;
				e.Current.Value.Setup( addData.unitName , null ) ;
				break ;
			}
		}
		m_RaiderTimer.Rewind() ;
	}
	
	private void TryLanunchMissile( UnitData unitData ) 
	{
		if( false == m_MissileTimer.IsCountDownToZero() )
			return ;
#if DEBUG_LOG		
		Debug.Log( "TryLanunchMissile" ) ;
#endif		
		
		Dictionary<string,NamedObject>.Enumerator e = m_Missile.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( null == e.Current.Value.Obj )
			{
				// Debug.Log( "null == e.Current.Value.Obj" ) ;
				Transform trans = this.gameObject.transform.FindChild( e.Current.Key ) ;
				if( null == trans )
					continue ;				
				GameObject swapnObj = trans.gameObject ;				

				EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
				UnitGenerationData addData = new UnitGenerationData() ;
				addData.initPosition.Setup( swapnObj.transform.position ) ;
				addData.initOrientation = swapnObj.transform.rotation ;
				addData.raceName = unitData.m_RaceName ;
				addData.sideName = unitData.m_SideName ;
				
				addData.unitName = "Enemy_CylonBaseShipMissile" + ConstName.GenerateIterateString() ;
				addData.prefabName = "Template_CylonMissile01" ;
				addData.unitDataTemplateName = "UnitDataTemplate_CylonMissile09" ;
				addData.time = Time.timeSinceLevelLoad ;
				
				addData.supplementalVec.Add( "TargetName" , m_TargetUnit.Name ) ;
				addData.supplementalVec.Add( "ShockwaveDamage" , "30.0" ) ;
				addData.supplementalVec.Add( "DetectRange" , "10.0" ) ;
				
				// Debug.Log( "enemyGenerator.InsertEnemyGenerationTable()" ) ;				
				enemyGenerator.InsertEnemyGenerationTable( addData ) ;
				e.Current.Value.Setup( addData.unitName , null ) ;
				break ;
			}
		}
		
		m_MissileTimer.Rewind() ;
	}
}

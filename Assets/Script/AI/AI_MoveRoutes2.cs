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
@file AI_MoveRoutes2.cs
@author NDark 

移動並攻擊
 
# 繼承 AI_FireToClosestEnemy2 因此是攻擊系而非移動系
# 但是取得移動的參數
##  "RoutePosition{0}" 
## "judgeDistance"
# GoToDestination() 移動到位置
# 如果有敵人會先切換到攻擊，結束才回來繼續移動

@date 20130131 file started.
@date 20130131 by NDark . remove class method FindATarget()
@date 20130204 by NDark . refine code.

*/
using UnityEngine;
using System.Collections.Generic;

public class AI_MoveRoutes2 : AI_FireToClosestEnemy2 
{
	int m_RouteIndexNow = 0 ;
	int m_MaxRouteNum = 10 ;
	// param
	// "judgeDistance"	
	protected List<Vector3> m_RoutePositions = new List<Vector3>() ; // "RoutePosition{0}" 
	// param
	// "TargetName"
	protected float m_JudgeDistance = 0.0f ; // "judgeDistance"
	
	// Use this for initialization
	void Start () 
	{
		SetState( AIBasicState.UnActive ) ;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( ref unitData , ref weaponSys ) )
			return ;
		
		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			SetState( AI_Fire_State.CheckUnitData ) ;
			break ;	
		case AI_Fire_State.CheckUnitData :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
			}
			SetState( AI_Fire_State.MoveToTarget ) ;
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			GoToDestination() ;
			if( true == FindATarget( unitData ) )
			{
				SetState( AI_Fire_State.StopAndFireToTarget ) ;
			}
			
			break ;
			
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.MoveToTarget ) ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;

			break ;				
			
			
		case AI_Fire_State.WaitForReload :
			CheckReloadAndBackTo( AI_Fire_State.MoveToTarget ) ;
			break ;					
		}			
	}
	
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		// Debug.Log( "RetrieveData() "  ) ;
		if( null != unitData && 
			null != weaponSys )
		{
			RetrieveDataWeaponKeyword( unitData ) ;
			RetrieveDataMaximumWeaponRange( weaponSys , ref m_WeaponKeywords ) ;			
			
			RetrieveParam( unitData , "WaitForReloadSec" , ref m_WaitForReloadSec ) ;
			// Debug.Log( this.gameObject.name + " m_WaitForReloadSec=" + m_WaitForReloadSec ) ;
			
			m_RouteIndexNow = 0 ;
			m_RoutePositions.Clear() ;
			for( int i = 0 ; i < m_MaxRouteNum ; ++i )
			{
				Vector3 routeValue = Vector3.zero ;
				string routeKey = string.Format( "RoutePosition{0}" , i ) ;
				if( true == RetrieveParam( unitData , routeKey , ref routeValue ) )
				{
					m_RoutePositions.Add( routeValue ) ;
					// Debug.Log( "m_RoutePositions" + m_RoutePositions[ m_RoutePositions.Count - 1 ] );
				}
			}
			
			RetrieveParam( unitData , "judgeDistance" , ref m_JudgeDistance ) ;			
		}			
		return true ;
	}
	
	// 發射武器
	protected override void FireWeapon( UnitWeaponSystem _weaponSys ,
							   		   NamedObject _TargetObject )
	{
		string weaponComponent = "" ;
		// check phaser
		weaponComponent = _weaponSys.FindAbleWeaponComponent( RandomAWeaponKeyword() , 
															  _TargetObject.Name ) ;
		if( 0 == weaponComponent.Length )
		{
			// no available weapon
			this.SetState( AI_Fire_State.MoveToTarget ) ;			
			return ;
		}
		
		// fire
		if( true == _weaponSys.ActiveWeapon( weaponComponent , 
											 _TargetObject ,
											 ConstName.UnitDataComponentUnitIntagraty ) )
		{
			this.SetState( AI_Fire_State.WaitForReload ) ;			
		}
		
	}
		
	
	// 前往目的地
	void GoToDestination()
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( ref unitData ) ||
			m_RouteIndexNow >= m_RoutePositions.Count )
		{
			SetState( AIBasicState.Closed ) ;
			return ;
		}
		
		Vector3 targetPosition = m_RoutePositions[ m_RouteIndexNow ] ;
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;		
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 targetPosition ,
													 ref vecToTarget , 
													 ref norVecToTarget , 
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			unitData.AngularRatioHeadTo( angleOfTarget , 
										 dotOfUp , 
										 0.1f ) ;		
			
			// Debug.Log( "vecToTarget.magnitude" + vecToTarget.magnitude ) ;
			string IMPULSE_ENGINE_ANGULAR_RATIO = ConstName.UnitDataComponentImpulseEngineAngularRatio ;
			string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
			if( true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) &&
				true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_ANGULAR_RATIO ))
			{
				StandardParameter impulseRatio = unitData.standardParameters[ IMPULSE_ENGINE_RATIO ] ;
				if( vecToTarget.magnitude < m_JudgeDistance )
				{
					++m_RouteIndexNow ;
					impulseRatio.now = 0 ;

					unitData.standardParameters[ IMPULSE_ENGINE_ANGULAR_RATIO ].now = 0 ;
				}
				else
					impulseRatio.ToMax() ;
			}
		}
	}	
}

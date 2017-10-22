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
@file AI_FireToTarget3.cs
@author NDark
對目標開火 並攻擊障礙物
 
# 繼承 AI_FireToTarget2
# 判斷有無可攻擊的障礙物 IsAnyAttacableObstacleInfrontOfUs()
## 傳回目標單位
## 會判斷不是陣營才能開火

A AI based on AI_FireToTarget.
The advance part is this AI will attack the obstacle.

@date 20121206 by NDark . file started.
@date 20121209 by NDark . add checking of not attacking to the same race at Update()
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor.
@date 20130109 by NDark . move some class members to AI_FireToClosestEnemy.
@date 20130131 by NDark 
. change type of argument of class method IsAnyObstacleInfrontOfUs()
. rename class method IsAnyObstacleInfrontOfUs() to IsAnyAttacableObstacleInfrontOfUs()

*/
using UnityEngine;

public class AI_FireToTarget3 : AI_FireToTarget2 
{
	// param
	// "WaitForReloadSec"	
	// "TargetName"
	
	// Use this for initialization
	void Start () 
	{
		SetState( AI_Fire_State.UnActive ) ;
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
			
			if( m_State.IsFirstTime() )
				m_MaximumRetrieveDataTime.Rewind() ;
			
			KeepRetrieveData() ;
				
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			if( true == CheckWeaponAndMoveToTarget( unitData , 
													weaponSys , 
													m_TargetNow.Obj.transform.position ) )
			{
				
			}
			else if( true == IsAnyAttacableObstacleInfrontOfUs( m_TargetNow.Name ,
												       			ref m_SubTarget ) )
			{
				SetState( AI_Fire_State.StopAndFireToSubTarget ) ;				
				break ;
			}
			
			break ;
			
		case AI_Fire_State.StopAndFireToSubTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			if( false == CheckTarget( m_SubTarget ) )
			{
				SetState( AI_Fire_State.MoveToTarget ) ;				
				break ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;
					
			break ;
			
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;				
				break ;
			}
			
			FireWeapon( weaponSys , m_TargetNow ) ;
			
			break ;	
			
		case AI_Fire_State.WaitForReload :
			this.CheckReloadAndBackTo( AI_Fire_State.MoveToTarget ) ;						
			break ;			
			
		case AI_Fire_State.End :
			// nothing.
			break ;			
		}
	}
	
	// 判斷有無可攻擊的障礙物 
	protected virtual bool IsAnyAttacableObstacleInfrontOfUs( string _TargetName , 
												   	 		  ref UnitObject _SubTarget )
	{
		bool ret = false ;
		
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		
		UnitSensorSystem sensor = this.gameObject.GetComponent<UnitSensorSystem>() ;
		if( null != sensor )
		{
			foreach( NamedObject unit in sensor.m_SensorUnitList )
			{	
				if( unit.Name == _TargetName )
					continue ;
			
				// 會判斷不是陣營才能開火
				UnitData unitUnitData = unit.Obj.GetComponent<UnitData>() ;
				if( unitData.m_SideName == unitUnitData.m_SideName )
					continue ;
				
				if( false == MathmaticFunc.FindUnitRelation( this.gameObject , 
															 unit.Obj , 
															 ref vecToTarget ,
															 ref norVecToTarget ,
															 ref angleOfTarget ,
															 ref dotOfUp ) )
					continue ;
				
				if( vecToTarget.sqrMagnitude < m_BlockRangeSquare &&
					angleOfTarget < m_BlockAngle )
				{
					// Debug.Log( "block in the way" + unit.Name ) ;
					// Debug.Log( "_SubTargetPosition" + _SubTargetPosition ) ;
					_SubTarget.Setup( unit.Name , unit.Obj ) ;
					
					ret = true ;						
					break ;
				}				
			}
		}
		return ret ;
	}	
}

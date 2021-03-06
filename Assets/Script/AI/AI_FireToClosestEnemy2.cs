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
@file AI_FireToClosestEnemy2.cs
@author NDark

# 繼承 AI_FireToClosestEnemy
# 會閃避友軍的版本
# CheckMoveToTarget() 前往第二目標

@date 20130109 file started.
@date 20130131 by NDark
. move class method IsAnyObstacleInfrontOfUs() to AI_Fire_ToClosetEnemy.cs
. move class method CalculateSubTarget() to AI_Fire_ToClosetEnemy.cs

*/
using UnityEngine;

public class AI_FireToClosestEnemy2 : AI_FireToClosestEnemy 
{

	// Use this for initialization
	void Start () 
	{
		SetState( AI_Fire_State.UnActive ) ;
		m_ReFindTargetTimer.Rewind() ;	
		m_EvasionRotateAngle = 40.0f ;
		m_BlockAngle = 80.0f ;
		m_BlockRangeSquare = 100.0f ;			
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( ref unitData , ref weaponSys ) )
			return ;
		
		if( m_State.state > (int) AI_Fire_State.CheckUnitData
			&& true == m_ReFindTargetTimer.IsCountDownToZero() )
		{
			SetState( AI_Fire_State.FindATarget ) ;
			m_ReFindTargetTimer.Rewind() ;
			return ;
		}
		

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
			SetState( AI_Fire_State.FindATarget ) ;
			break ;
			
		case AI_Fire_State.FindATarget :
			if( true == FindATarget( unitData ) )
			{
				this.SetState( AI_Fire_State.MoveToTarget ) ;
			}					
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.FindATarget ) ;
				break ;
			}
			
			
			if( true == IsAnyObstacleInfrontOfUs( m_TargetNow.Name , 
												  ref m_SubTargetPosition ) )
			{
				m_State.state = (int)AI_Fire_State.MoveToSubTarget ;
				break ;
			}
			else if( true == CheckWeaponAndMoveToTarget( unitData , 
														 weaponSys , 
														 m_TargetNow.Obj.transform.position ) )
			{
			}			
			
			break ;
			
		case AI_Fire_State.MoveToSubTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			if( true == CheckMoveToTarget( unitData , weaponSys , m_SubTargetPosition , 0.2f ) )
			{
			}
			else if( true == IsAnyObstacleInfrontOfUs( m_TargetNow.Name , 
												  	   ref m_SubTargetPosition ) )
			{
				// Debug.Log( "true == IsAnyObstacleInfrontOfUs") ;
			}						
			break ;
			
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.FindATarget ) ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;

			break ;				
			
			
		case AI_Fire_State.WaitForReload :
			CheckReloadAndBackTo( AI_Fire_State.FindATarget ) ;
			break ;
		}		
	}
	

	

	
	// 前往第二目標 
	protected bool CheckMoveToTarget( UnitData _unitData , 
									  UnitWeaponSystem _weaponSys ,
									  Vector3 _TargetPosition , 
									  float _ImpulseSpeed = 1.0f )
	{
		
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		string IMPLUSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
	
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 _TargetPosition ,
													 ref vecToTarget ,
													 ref norVecToTarget ,
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			
			if( vecToTarget.sqrMagnitude < m_ReachDistanceSquare )
			{
				this.SetState( AI_Fire_State.MoveToTarget ) ;
				return true ;
			}
			
			if( true == _unitData.standardParameters.ContainsKey( IMPLUSE_ENGINE_RATIO ) )
			{
				_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].now =
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].max * _ImpulseSpeed ;
					
			}
			
			// angular speed
			_unitData.AngularRatioHeadTo( angleOfTarget ,
										  dotOfUp , 
										  0.1f ) ;

		}
		
		return false ;
	}	
}

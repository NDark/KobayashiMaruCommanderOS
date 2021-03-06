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
@file AI_FireToTarget5.cs
@author NDark
 

A AI based on AI_FireToTarget3.
對目標開火 並會閃避障礙物及攻擊障礙物

# 繼承 AI_FireToTarget3
# 判斷有無敵人或友軍障礙物 IsAnyEnemyOrObstacleInFrontOfUs()
## 如果不是友軍則傳回 攻擊單位
## 如果是友軍則傳回 障礙物位置
# 覆寫 計算移動目標 CalculateSubTarget()
會依照與障礙物的距離放大閃避距離


@date 20121206 by NDark . file started.
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor.
@date 20130105 by NDark . fix an error at StopAndFireToSubTarget of Update()
@date 20130131 by NDark . rename class method IsAnyObstacleInfrontOfUs() to IsAnyEnemyOrObstacleInFrontOfUs()
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

public class AI_FireToTarget5 : AI_FireToTarget3
{
	
	// param
	// "WaitForReloadSec"	
	// "TargetName"
	
	// Use this for initialization
	void Start () 
	{
		m_State.state = (int)AI_Fire_State.UnActive ;
		m_EvasionRotateAngle = 40.0f ;
		m_BlockAngle = 40.0f ;
		m_BlockRangeSquare = 400.0f ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( ref unitData , ref weaponSys ) )
			return ;		
		// Debug.Log( "m_TargetNow=" + m_TargetNow.Name ) ;
		// Debug.Log( "m_State=" + m_State.state ) ;
		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			m_State.state = (int)AI_Fire_State.CheckUnitData ;
			break ;		
			
		case AI_Fire_State.CheckUnitData :
			
			if( m_State.IsFirstTime() )
				m_MaximumRetrieveDataTime.Rewind() ;
			
			KeepRetrieveData() ;
			
			break ;	
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			if( true == IsAnyEnemyOrObstacleInFrontOfUs( m_TargetNow.Name , 
												  ref m_SubTarget ,
												  ref m_SubTargetPosition ) )
			{
				if( null == m_SubTarget )
					m_State.state = (int)AI_Fire_State.MoveToSubTarget ;
				else 
				{
					m_State.state = (int)AI_Fire_State.StopAndFireToSubTarget ;
				}
				
				break ;
			}
			else if( true == CheckWeaponAndMoveToTarget( unitData , weaponSys , 
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
			
			
			if( true == IsAnyEnemyOrObstacleInFrontOfUs( m_TargetNow.Name , 
												  ref m_SubTarget ,
												  ref m_SubTargetPosition ) )
			{
				if( null == m_SubTarget )
				{
				}
				else 
				{
					// attack
					m_State.state = (int)AI_Fire_State.StopAndFireToSubTarget ;
					break ;
				}
			}
			
			if ( true == CheckMoveToTarget( unitData , weaponSys , m_SubTargetPosition , 0.5f ) )
			{
			}			
			
			break ;			
		case AI_Fire_State.StopAndFireToSubTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			if( false == CheckTarget( m_SubTarget ) )
			{
				m_State.state = (int)AI_Fire_State.MoveToTarget ;
				break ;
			}			
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_SubTarget ) ;
					
			break ;
						
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
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
	
	
	// 判斷有無敵人或友軍障礙物 
	protected virtual bool IsAnyEnemyOrObstacleInFrontOfUs( string _TargetName , 
												    		ref UnitObject _ObstacleEnemy ,
												     		ref Vector3 _SubTargetPosition )
	{
		bool ret = false ;
		
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		
		UnitSensorSystem sensor = this.gameObject.GetComponent<UnitSensorSystem>() ;
		if( null != sensor )
		{
			GameObject clostUnitObj = sensor.GetClosestObj() ;
			if( null != clostUnitObj )
			{	
				if( clostUnitObj.name == _TargetName )
					return ret ;
				
				if( false == MathmaticFunc.FindUnitRelation( this.gameObject , 
															 clostUnitObj , 
															 ref vecToTarget ,
															 ref norVecToTarget ,
															 ref angleOfTarget ,
															 ref dotOfUp ) )
					return ret ;
				
				UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
				UnitData possibleSubTarget = clostUnitObj.GetComponent<UnitData>() ;
				
				if( null != unitData &&
					null != possibleSubTarget &&
					unitData.m_SideName != possibleSubTarget.m_SideName )
				{
					
					if( vecToTarget.sqrMagnitude < m_BlockRangeSquare &&
						angleOfTarget < m_BlockAngle )					
					{
						// attack the enemy
						// Debug.Log( "block in the way" + clostUnitObj.name ) ;
						_ObstacleEnemy = new UnitObject( clostUnitObj ) ;						
						ret = true ;										
					}
				}
				else 
				{
					_ObstacleEnemy = null ;
					if( vecToTarget.sqrMagnitude < m_BlockRangeSquare &&
						angleOfTarget < m_BlockAngle * 2.0f )					
					{
						CalculateSubTarget( this.gameObject , 
											clostUnitObj , 
											ref _SubTargetPosition ) ;
						
						// Debug.Log( "_SubTargetPosition" + _SubTargetPosition ) ;
						ret = true ;										
					}
				}				
			}
		}
		return ret ;
	}

	protected override void CalculateSubTarget( GameObject _SrcObj , 
												GameObject _ObstacleObj , 
												ref Vector3 _SubTarget )
	{
		Vector3 vecToTarget = _ObstacleObj.transform.position - _SrcObj.transform.position ;
		Vector3 vecCenterLine = _SrcObj.transform.forward ;
		float standarddistance = 20.0f ;
		if( vecToTarget.sqrMagnitude > standarddistance * standarddistance )
			vecCenterLine *= vecToTarget.magnitude ;
		else
			vecCenterLine *= vecToTarget.magnitude * 2.0f ;

		Quaternion rotate = Quaternion.identity ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle , _SrcObj.transform.up ) ;
		Vector3 newVec1 = rotate * vecCenterLine ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle * -1.0f , _SrcObj.transform.up ) ;
		Vector3 newVec2 = rotate * vecCenterLine ;

		Vector3 DestPos1 = _SrcObj.transform.position + newVec1 ;
		Vector3 DestPos2 = _SrcObj.transform.position + newVec2 ;
		Vector3 DistVec1 = DestPos1 - _ObstacleObj.transform.position ;
		Vector3 DistVec2 = DestPos2 - _ObstacleObj.transform.position ;
		if( DistVec1.sqrMagnitude > DistVec2.sqrMagnitude )
		{
			_SubTarget = DestPos1 ;
		}
		else
			_SubTarget = DestPos2 ;
		
	}
}

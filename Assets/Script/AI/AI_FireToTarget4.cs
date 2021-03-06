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
@file AI_FireToTarget4.cs
@author NDark
 
A AI based on AI_FireToTarget.
The advance part is this AI will turn to the the defense side.
對目標開火 填充時轉向

# 繼承 AI_FireToTarget
# 填充時轉向 RotateToDefenseSide()
會取得防護罩的權重方向來做轉向

@date 20121206 by NDark . file started.
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor.
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

public class AI_FireToTarget4 : AI_FireToTarget 
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
			{
				m_MaximumRetrieveDataTime.Rewind() ;
			}
			
			KeepRetrieveData() ;
			
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			CheckWeaponAndMoveToTarget( unitData , 
										weaponSys , 
										m_TargetNow.Obj.transform.position ) ;
			
			break ;	
			

		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;

			break ;				
			
		case AI_Fire_State.WaitForReload :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			this.CheckReloadAndBackTo( AI_Fire_State.MoveToTarget ) ;
			
			RotateToDefenseSide( unitData ) ;
			
			break ;			
			
		case AI_Fire_State.End :
			// nothing.
			break ;						
		}
	}
	
	// 填充時轉向 
	protected virtual void RotateToDefenseSide( UnitData _unitData )
	{
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		
		Vector3 shieldWeightDir = _unitData.FindShieldWeightDirection() ;
		if( shieldWeightDir != Vector3.zero )
		{
			// Debug.Log( "shieldWeightDir" + shieldWeightDir ) ;
			if( true == MathmaticFunc.FindTargetRelationWithForward( this.gameObject , 
														 shieldWeightDir ,
														 m_TargetNow.Obj.transform.position ,
														 ref vecToTarget ,
														 ref norVecToTarget ,
														 ref angleOfTarget ,
														 ref dotOfUp ) )
			{
				// angular speed
				_unitData.AngularRatioHeadTo( angleOfTarget ,
											 dotOfUp , 
											 0.1f ) ;
				
			}
			
		}
	}
		
}

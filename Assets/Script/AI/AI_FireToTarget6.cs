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
@file AI_FireToTarget6.cs
@author NDark
 
# 繼承 AI_FireToTarget5
# 會使用能源控制器
# 覆寫 CheckWeaponAndMoveToTarget() 檢查武器並移動 , 會檢查是在範圍內 還是不能發射
# 覆寫 FireWeapon() 會檢查是在範圍內還是不能發射
# ReRoutePowerToTheEngine() 能源轉向到引擎
# ReRoutePowerToTheWeapon() 能源轉向到武器

@date 20130102 by NDark . file started.
@date 20130204 by NDark . refine code.
*/
using UnityEngine;

public class AI_FireToTarget6 : AI_FireToTarget5 
{

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
	
		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			m_State.state = (int)AI_Fire_State.CheckUnitData ;
			break ;		
			
		case AI_Fire_State.CheckUnitData :
			
			if( m_State.IsFirstTime() )
			{
				m_MaximumRetrieveDataTime.Rewind() ;
				
			}
			
			KeepRetrieveData() ;
			
			break ;	
			
		case AI_Fire_State.MoveToTarget :
			
			if( m_State.IsFirstTime() )
			{
				ReRoutePowerToTheEngine( unitData ) ;
				break ;
			}
			
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
			
			if( m_State.IsFirstTime() )
			{
				ReRoutePowerToTheEngine( unitData ) ;
				break ;
			}
			
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
				ReRoutePowerToTheWeapon( unitData ) ;
				unitData.AllStop() ;
				break ;
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
				ReRoutePowerToTheWeapon( unitData ) ;
				unitData.AllStop() ;
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
	
	// 檢查武器並移動 , 會檢查是在範圍內 還是不能發射
	protected override bool CheckWeaponAndMoveToTarget( UnitData _unitData , 
													   UnitWeaponSystem _weaponSys ,
													   Vector3 _TargetPosition )
	{
		string weaponInRange = "" ;
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		string IMPLUSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
	
		// 剛轉換過來不要檢查武器
		if( m_State.ElapsedFromLast() > 0.5f )
		{
			// check fire phaser weapon is available
			weaponInRange = _weaponSys.FindAbleToShootTargetWeaponComponent( RandomAWeaponKeyword() , 
																  m_TargetNow.Obj ) ;
			// Debug.Log( "weaponInRange=" + weaponInRange) ;
			if( 0 != weaponInRange.Length )
			{
				this.SetState( AI_Fire_State.StopAndFireToTarget ) ;			
				return true ;
			}
		}
		
		// move to target
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 _TargetPosition , 
													 ref vecToTarget ,
													 ref norVecToTarget ,
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			if( true == _unitData.standardParameters.ContainsKey( IMPLUSE_ENGINE_RATIO ) )
			{
				// speed to maximum
				if( vecToTarget.sqrMagnitude > m_MaximumWeaponRange * m_MaximumWeaponRange )
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].ToMax() ;
				else
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].Clear() ;
			}
			
			// angular speed			
			_unitData.AngularRatioHeadTo( angleOfTarget ,
										  dotOfUp , 
										  0.1f ) ;
		}
		
		return false ;
	}
	
	protected override void FireWeapon( UnitWeaponSystem _weaponSys ,
							  			NamedObject _TargetUnit )
	{
		if( true == GlobalSingleton.GetGlobalSingletonObj().GetComponent<AutoPlayMachine>().m_Active )
			return ;
		
		string weaponComponent = "" ;
		string specifiedWeapon = RandomAWeaponKeyword() ;
		// 距離太遠
		string weaponInRange = _weaponSys.FindAbleToShootTargetWeaponComponent( specifiedWeapon , _TargetUnit.Obj ) ;
		// Debug.Log( "weaponInRange=" + weaponInRange ) ;
		if( 0 == weaponInRange.Length )
		{
			// no available weapon
			this.SetState( AI_Fire_State.MoveToTarget ) ;
			return ;
		}
		
		string weaponReady = _weaponSys.FindAbleToFireWeaponComponent( specifiedWeapon ) ;
#if DEBUG_LOG		
		Debug.Log( "FireWeapon() weaponReady=" + weaponReady ) ;
#endif		
		if( 0 == weaponReady.Length )
		{
			// no available weapon
			this.SetState( AI_Fire_State.WaitForReload ) ;
			return ;
		}
		
		// 檢查正在reload
		weaponComponent = _weaponSys.FindAbleWeaponComponent( specifiedWeapon , _TargetUnit.Obj ) ;		
		// Debug.Log( "weaponReady=" + weaponComponent ) ;
		if( 0 == weaponComponent.Length )
		{
			// no available weapon
			this.SetState( AI_Fire_State.WaitForReload ) ;
			return ;
		}
		
		// Debug.Log( "_weaponSys.ActiveWeapon" ) ;
		// fire
		if( true == _weaponSys.ActiveWeapon( weaponComponent , 
											_TargetUnit ,
											ConstName.UnitDataComponentUnitIntagraty ) )
		{
			this.SetState( AI_Fire_State.WaitForReload ) ;
		}
		
	}

	// 能源轉向到引擎
	private void ReRoutePowerToTheEngine( UnitData _unitData )
	{
		// Debug.Log( "ReRoutePowerToTheEngine" ) ;
		_unitData.TrySetWeaponEnergy( 0 ) ;
		float auxiliaryEnergy = _unitData.GetAuxiliaryEnergyValue() ;
		if( auxiliaryEnergy > 0 )
		{	
			float now = 0 ;
			float max = 0 ;
			_unitData.RetrieveImpulseEngineEnergy( out now , out max ) ;
			_unitData.TrySetImpulseEngineEnergy( now + auxiliaryEnergy ) ;
		}
	}
	
	// 能源轉向到武器
	private void ReRoutePowerToTheWeapon( UnitData _unitData )
	{
		// Debug.Log( "ReRoutePowerToTheWeapon" ) ;
		_unitData.TrySetImpulseEngineEnergy( 0 ) ;
		float auxiliaryEnergy = _unitData.GetAuxiliaryEnergyValue() ;
		if( auxiliaryEnergy > 0 )
		{		
			float now = 0 ;
			float max = 0 ;
			_unitData.RetrieveImpulseEngineEnergy( out now , out max ) ;
			_unitData.TrySetWeaponEnergy( now + auxiliaryEnergy ) ;
		}
	}	
}

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
@file AI_CollideTo.cs
@author NDark

朝向某目標並檢查碰撞
# 繼承 AI_TargetTo
# m_ShockwaveDamage 震波傷害值，可由參數中設定
# m_DetectRange 偵測距離，可由參數中設定
# CheckCollideWithSomeone() 會去檢查碰撞，使用 WhenCollideOnCollider
# 呼叫Suicide() 自我消滅 放出震波特效 Template_Effect_Weapon_Shockwave01


@date 20121228 by NDark . file started.
@date 20121228 by NDark 
. add code of shockwaveObject at Suicide()
. add class member m_ShockwaveDamage
@date 20130106 by NDark . add race checking at CheckCollideWithSomeone()
@date 20130107 by NDark . add class member m_DetectRange and parameter
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

public class AI_CollideTo : AI_TargetTo 
{
	public float m_ShockwaveDamage = 7.0f ;
	public float m_DetectRange = 15.0f ;
	
	// Use this for initialization
	void Start () 
	{
		SetState( AIBasicState.UnActive ) ;
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
			SetState( AIBasicState.Initialized ) ;
			break ;
		case AIBasicState.Initialized :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
			}
			SetState( AIBasicState.Active ) ;			
			break ;			
		case AIBasicState.Active :
			ChaseTarget() ;
			CheckCollideWithSomeone() ;
			break ;
		case AIBasicState.Closed :
			Suicide( unitData ) ;
			break ;			
		}	
	}
	
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			RetrieveParam( unitData , "ShockwaveDamage" , ref m_ShockwaveDamage ) ;
			RetrieveParam( unitData , "DetectRange" , ref m_DetectRange ) ;			
			
			string TargetName = "" ;
			if( true == RetrieveParam( unitData , "TargetName" , ref TargetName ) )
			{
				m_Target.Name = TargetName ;
				return true ;
			}
		}			
		return false ;
	}	
	
	private void CheckCollideWithSomeone()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( unitData == null )
			return ;
		
		Transform trans = this.gameObject.transform.FindChild( "CollideCube" ) ;
		if( null != trans )
		{
			GameObject collideCube = trans.gameObject ;
			WhenCollideOnCollider whenCollide = collideCube.GetComponent<WhenCollideOnCollider>() ;
			if( null != whenCollide )
			{
				if( true == whenCollide.m_IsCollide )
				{
					if( null != whenCollide.m_CollideUnitObject )
					{
						UnitData targetUnitData = whenCollide.m_CollideUnitObject.GetComponent<UnitData>() ;
						if( null != targetUnitData &&
							unitData.m_SideName == targetUnitData.m_SideName )
							return ;
					}
					
					SetState( AIBasicState.Closed ) ;
				}
			}
		}
	}
	
	private void Suicide( UnitData _unitData )
	{
		GameObject shockwaveObject = PrefabInstantiate.CreateByInit( "Template_Effect_Weapon_Shockwave01" , 
													this.gameObject.name + ":" + "Template_Effect_Weapon_Shockwave01" , 
													this.gameObject.transform.position ,
													this.gameObject.transform.rotation ) ;
		if( null != shockwaveObject )
		{
			ShockwaveEffect shockwaveEffect = shockwaveObject.GetComponent<ShockwaveEffect>() ;			
			shockwaveEffect.Active( m_ShockwaveDamage ,
									0.7f , 30 , m_DetectRange , 
									"Flashbang" , 2.5f , this.gameObject ) ;
		}
		
		_unitData.m_UnitState.state = (int) UnitState.Dead ;
	}
}

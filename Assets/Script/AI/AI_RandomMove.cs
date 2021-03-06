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
@file AI_RandomMove.cs
@brief 隨機移動AI
@author NDark

# 繼承 AI_Base
狀態分為 未啟動 移動 切換目標
# 每一段時間計算一次下次漂移的方向及轉速
# 漂移 直到下一個週期來到
# 目前將漂移的位移量存到 m_ForceToMoveVec 中 由 main update來更新位置
# m_ChangeCourseTrigger 改變目標的倒數計時
# 參數 
## DriftSpeed 飄移速度 m_DriftSpeed
## RotateAngularSpeed 旋轉速度 m_RotateAngularSpeed
# 周期的時間也加上隨機量

@date 20121108 by NDark . replace class member by m_ChangeCourseTrigger.
@date 20121111 by NDark . add code of unitData.m_ForceToMoveVec at Update()
@date 20121121 by NDark
. add class member m_DriftSpeed 
. add class member m_RotateAngularSpeed
. add class method ChangeRotate()
@date 20121203 by NDark . comment
@date 20121207 by NDark . add code of retreive data.
@date 20121210 by NDark . re-factor
@date 20130204 by NDark . refine code.

*/
// #define DEBUG_LOG
using UnityEngine;

public class AI_RandomMove : AI_Base 
{
	public enum AI_RandomMoveState
	{
		UnActive ,		// 未啟動
		KeepMoving ,	// 移動
		ChangeCourse ,	// 切換目標
	}	
	
	public float m_ChangeCourseBaseSec = 10.0f ;
	public CountDownTrigger m_ChangeCourseTrigger = new CountDownTrigger() ;

	public Vector3 m_DirectionNow = Vector3.zero ;
	public float m_RotateRandomRatio = 1.0f ;
	
	// parameter from m_SupplementalVec
	public float m_DriftSpeed = 0.1f ;			// "DriftSpeed"
	public float m_RotateAngularSpeed = 0.2f ;	// "RotateAngularSpeed"
	
	// Use this for initialization
	void Start () 
	{
		SetState( AI_RandomMoveState.UnActive ) ;		
		
		if( false == RetrieveData() )
		{
			Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
		}
		RecalculateCourse() ;		
	}

	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( ref unitData ) )
			return ;
		
		m_State.Update() ;
		switch( (AI_RandomMoveState)m_State.state )
		{
		case AI_RandomMoveState.UnActive :
			SetState( AI_RandomMoveState.KeepMoving ) ;			
			break ;
			
		case AI_RandomMoveState.KeepMoving :
			
			// move to the direction
			Vector3 TranslationVec = m_DirectionNow * ( m_DriftSpeed * Time.deltaTime ) ;
			if( null != unitData )
			{
				unitData.m_ForceToMoveVec += TranslationVec ;
			}
			
			this.gameObject.transform.RotateAroundLocal( this.gameObject.transform.up , 
				m_RotateAngularSpeed * m_RotateRandomRatio * Time.deltaTime ) ;
			
			// chech change course
			if( true == m_ChangeCourseTrigger.IsCountDownToZero() )
				SetState( AI_RandomMoveState.ChangeCourse ) ;
				
			break ;
			
		case AI_RandomMoveState.ChangeCourse :
			RecalculateCourse() ;
			SetState( AI_RandomMoveState.KeepMoving ) ;
			break ;
		}
	}
	protected void SetState( AI_RandomMoveState _Set )
	{
		m_State.state = (int) _Set ;
	}
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			RetrieveParam( unitData , "RotateAngularSpeed" , ref m_RotateAngularSpeed ) ;
			RetrieveParam( unitData , "DriftSpeed" , ref m_DriftSpeed ) ;
		}			
		return true ;
	}
	
	private void RecalculateCourse()
	{
		m_DirectionNow = ChangeCourse() ;
		m_RotateRandomRatio = ChangeRotate() ;		
#if DEBUG_LOG
		Debug.Log( "RecalculateCourse() m_DirectionNow=" + m_DirectionNow ) ;
#endif 
		// add randomize of nect changing course.
		m_ChangeCourseTrigger.Setup( m_ChangeCourseBaseSec + 
									 MathmaticFunc.RandomRatioValue( -0.5f , 0.5f )) ;
		m_ChangeCourseTrigger.Rewind() ;
	}
	
	private float ChangeRotate()
	{
		return MathmaticFunc.RandomRatioValue( -0.5f , 0.5f ) ;
	}		
	
	private Vector3 ChangeCourse()
	{
		return MathmaticFunc.RandomVector( 2 ) ;// clear in y:2
	}
	
}

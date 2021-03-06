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
@file Trigger.cs
@brief 觸發器
@author NDark

-# BasicTrigger : 基本觸發器
-# TimeTrigger : a trigger has start time and elapsed time to end.
-# CountDownTrigger : a trigger has only a countdown time.

@date 20121108 by NDark . add class CountDownTrigger.
@date 20121109 by NDark . add comment.
@date 20121125 by NDark 
. add class BasicTrigger
. add copy constructor of BasicTrigger.
. add copy constructor of TimeTrigger
@date 20121203 by NDark
. add comment.
. add inherate.
@date 20121221 by NDark 
. rename class method End()
. rename class method Start() to Active()
. rename class method IsStarted() to IsTimeStarted()
. rename class method IsEnded() to IsTimeEnded()

*/
using UnityEngine;
using System.Collections;

/*
觸發三態
最常見的作法是有幾種

標準觸發器

# 未啟動,還不接受命令/觸發
# 啟動中,可以接受命令/觸發
# 已結束,不再接受命令/觸發



啟動中有一段動畫時間的進行

# 未開始,可觸發
# 開始中,進行中
# 已結束,不再檢查觸發.

*/
[System.Serializable]
public enum TriggerState
{
	UnActive , // 未啟動
	Active , // 已啟動
	Closed , // 已結束
}
/*
 * BasicTrigger
 * 
 * -# Initialize() set state to UnActive ( IsReady() )
 * -# Active() set state to Active ( IsActive() )
 * -# Close() set state to Closed. ( IsClosed() )
 */
[System.Serializable]
public class BasicTrigger
{
	public TriggerState State
	{
		set
		{
			Debug_StateNow = value.ToString() ;
			m_State.state = (int) value ;			
		}
		get
		{
			return (TriggerState) m_State.state ;
		}
	}	
	public void Initialize()
	{
		this.State = TriggerState.UnActive ;
	}
	public void Active()
	{
		this.State = TriggerState.Active ;
	}
	public void Close()
	{
		this.State = TriggerState.Closed ;		
	}	
	
	public bool IsReady()
	{
		return m_State.state == (int) TriggerState.UnActive ;
	}
	public bool IsActive()
	{
		return m_State.state == (int) TriggerState.Active ;
	}
	public bool IsClosed()
	{
		return m_State.state == (int) TriggerState.Closed ;
	}
	
	public StateIndex m_State = new StateIndex() ;
	public string Debug_StateNow = "un-initialized" ;
	public BasicTrigger()
	{
		
	}
	public BasicTrigger( BasicTrigger _src )
	{
		m_State = new StateIndex( _src.m_State ) ;
		Debug_StateNow = _src.Debug_StateNow ;
	}	
}

/*
 * @brief Time trigger with start time and elapsed time to end.
 * 
 * This class use StateIndex to record time, 
 * so make sure call Initialize() at start.
 * 
 * -# IsTimeStarted() , time is pass the start time.
 * -# IsTimeEnded() , time is pass the elapsed time (from last change state).
 * -# call IsAboutToStart() to check is the first time to start, and make it start by argument.
 * -# call IsAboutToClose() to check is the only time to end after elapsed time from the first time, and make it end by argument.
 */
[System.Serializable]
public class TimeTrigger : BasicTrigger
{
	public float m_StartTime = 0.0f ;
	public float m_ElapsedTime = 0.0f ;
	
	public TimeTrigger() 
	{
	}

	public TimeTrigger( TimeTrigger _src ) 
	{
		m_StartTime = _src.m_StartTime ;
		m_ElapsedTime = _src.m_ElapsedTime ;
		m_State = _src.m_State ;
		Debug_StateNow = _src.Debug_StateNow ;
	}
	
	public TimeTrigger( float _StartTime , float _ElapsedTime ) 
	{
		m_StartTime = _StartTime ;
		m_ElapsedTime = _ElapsedTime ;
	}
	

	public void Setup( float _StartTime , float _ElapsedTime )
	{
		m_StartTime = _StartTime ;
		m_ElapsedTime = _ElapsedTime ;
		Initialize() ;
	}
	
	public bool IsTimeStarted()
	{
		return m_State.ElapsedFromLast() > m_StartTime ;
	}
	public bool IsTimeEnded()
	{
		return m_State.ElapsedFromLast() > m_ElapsedTime ;
	}
	
	/*
	 * @brief Is the first time to start?
	 * @param _IfTrue_Start 若開始,是否直接在函式內呼叫開始,外部不用再呼叫一次Start().
	 * 
	 */
	public bool IsAboutToStart( bool _IfTrue_Start )
	{
		bool ret = this.IsReady() && this.IsTimeStarted() ;
		if( true == _IfTrue_Start && 
			true == ret )
			Active() ;
		return ret ;
	}
	
	/*
	 * @brief Is the only time to end?
	 * @param _IfTrue_Close 若需結束,是否直接在函式內呼叫結束,外部不用再呼叫一次 Close().
	 * 
	 */	
	public bool IsAboutToClose( bool _IfTrue_Close )
	{
		bool ret = this.IsActive() && this.IsTimeEnded() ;
		if( true == _IfTrue_Close && 
			true == ret )
			Close() ;
		return ret ;
	}
}

/*
 * @brief a trigger has only a countdown time.
 * 
 * make sure call Rewind() before each using.
 * 
 * -# call IsCountDownToZero() to check it.
 */
[System.Serializable]
public class CountDownTrigger : BasicTrigger
{
	// 倒數秒數
	public float m_CountDownTime = 0.0f ;
	
	public CountDownTrigger() 
	{
	}
	public CountDownTrigger( float _CountDownTime ) 
	{
		Setup( _CountDownTime ) ;
	}
	public CountDownTrigger( CountDownTrigger _src ) 
	{
		Setup( _src.m_CountDownTime ) ;
	}

	public void Rewind()
	{
		Initialize() ;
	}
	
	public void Setup( float _CountDownTime )
	{
		m_CountDownTime = _CountDownTime ;
	}
	
	public bool IsCountDownToZero()
	{
		return m_State.ElapsedFromLast() > m_CountDownTime ;
	}

}
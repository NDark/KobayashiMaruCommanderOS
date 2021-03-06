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
@file MessageQueueManager.cs
@author NDark

# 將傳入的訊息一個個顯示出來
# 顯示有出現時間及淡出時間
# 如果有下一個訊息在等待就不淡出直接換
# 出現訊息時有聲音
# m_Text 顯示的GUI物件為 GUI_MessageQueueManagerText
# 紀錄上一次顯示的字串(串列最舊)
# 紀錄上一次加入的字串(串列最新)
# 與上一次顯示及上一次加入的字串相同的就不重複加入
# AddMessage() 新增顯示字串
# CloseMessageNow() 強制關閉目前字串
# 顯示的流程是
## 結束時檢查下一個並顯示
## 顯示直到啟動
## 啟動後進入淡出或是直接關閉
## 淡出完畢就關閉

@date 20121214 file started.
@date 20121219 by NDark 
. add class method StartShow()
. add class method UpdateFadeOut()
@date 20121225 by NDark
. add class member m_LastDisplay
. add class member m_LastQueueIn
@date 20130107 by NDark . add class method CloseMessageNow()

*/
using UnityEngine;
using System.Collections.Generic;

public class MessageQueueManager : MonoBehaviour 
{
	private Queue<string> m_MessageQueue = new Queue<string>() ;
	
	private TimeTrigger m_MessageShowTrigger = new TimeTrigger( BaseDefine.MESSAGE_QUEUE_SHOW_SEC , 
																BaseDefine.MESSAGE_QUEUE_FADEOUT_SEC ) ;
	private AudioClip m_Audio = null ;
	
	UnityEngine.UI.Text m_Text = null ;
	private NamedObject m_TextObj = new NamedObject( "GUI_MessageQueueManagerText" ) ;
	private string m_LastDisplay = "" ;
	private string m_LastQueueIn = "" ;
	
	public void AddMessage( string _Message ) 
	{
		// Debug.Log( "AddMessage() _Message=" + _Message ) ;
		if( false == m_MessageShowTrigger.IsClosed() &&
			m_LastDisplay == _Message )
		{
			return ;// 顯示中就捨棄
		}
		if( m_MessageQueue.Count > 0 &&
			m_LastQueueIn == _Message )
		{
			return ;// 疊加就捨棄
		}
		m_MessageQueue.Enqueue( _Message ) ;
		// Debug.Log( _Message ) ;
		m_LastQueueIn = _Message ;
	}
	
	public void CloseMessageNow( UnityEngine.UI.Text _guiText )
	{
		if( null == _guiText )
		{
			_guiText = m_TextObj.Obj.GetComponent<UnityEngine.UI.Text>() ;
		}
		
		if( null == _guiText )
			return ;		
		// Debug.Log( "CloseMessageNow" ) ;
		_guiText.enabled = false ;
		m_MessageShowTrigger.Close() ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_MessageShowTrigger.Close() ;
		m_Audio = ResourceLoad.LoadAudio( "alert12" ) ;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if( null == m_Text )
		{
			m_Text = m_TextObj.Obj.GetComponent<UnityEngine.UI.Text>() ;
		}
		
		if( null == m_Text )
			return ;

		// Debug.Log( "m_MessageShowTrigger.m_State.ElapsedFromLast()" + m_MessageShowTrigger.m_State.ElapsedFromLast() ) ;
		if( true == m_MessageShowTrigger.IsClosed() ) 
		{
			if( m_MessageQueue.Count > 0 )
			{
				// show another one
				string messageShow = m_MessageQueue.Dequeue() ;
				StartShow( m_Text , messageShow ) ;				
			}
		}
		else if( true == m_MessageShowTrigger.IsAboutToClose( true ) )
		{
			// Debug.Log( "m_MessageShowTrigger.IsAboutToClose" ) ;
			// just hide the text
			m_Text.enabled = false ;
		}		
		else if( true == m_MessageShowTrigger.IsActive() ||
				 true == m_MessageShowTrigger.IsAboutToStart( true ) )
		{
			if( m_MessageQueue.Count > 0 )
			{
				CloseMessageNow( m_Text ) ;
			}
			else
				UpdateFadeOut( m_Text ,
								m_MessageShowTrigger.m_State.ElapsedFromLast() ,
								m_MessageShowTrigger.m_ElapsedTime ) ;
		}
	}
	
	private void StartShow( UnityEngine.UI.Text _guiText , 
					   		string _ShowMessage )
	{
		// show another one
		_guiText.enabled = true ;
		Color color = _guiText.color ;
		color.a = 1.0f ;
		
		_guiText.color = color ;
		m_LastDisplay = _guiText.text = _ShowMessage ;
		m_MessageShowTrigger.Initialize() ;
		this.gameObject.GetComponent<AudioSource>().PlayOneShot( m_Audio ) ;
		// Debug.Log( "StartShow() _ShowMessage=" + _ShowMessage ) ;
		
	}
	
	private void UpdateFadeOut( UnityEngine.UI.Text _guiText , 
								float _ElapsedTime , 
								float _TotalFadeTime )
	{
		// fade out
		Color color = _guiText.color ;
		
		color.a = MathmaticFunc.Interpolate( 
			0.0f , 
			1.0f ,
			_TotalFadeTime ,
			0.0f , 
			_ElapsedTime ) ;
		// Debug.Log( color ) ;
		_guiText.color = color ;
	}
}

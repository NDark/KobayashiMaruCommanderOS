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
@file AudioPlayTimeEvent.cs
@author NDark

播放音效事件

參數
# 起始時間
# 結束時間
# m_AudioClipName 發出聲音名稱
# m_EventManagerObj 事件處理器 的物件(用來呼叫audio source)

@date 20130116 file started.
*/
// #define DEBUG

using UnityEngine;
using System.Xml;

/*
 * @brief 播放音效事件 AudioPlayTimeEvent
 */
[System.Serializable]
public class AudioPlayTimeEvent : TimeEvent
{
	private string m_AudioClipName = "" ; // 發出聲音名稱
	private AudioClip m_Audio = null ;
	private GameObject m_EventManagerObj = null ; // 事件處理器 的物件(用來呼叫audio source)
	
	public AudioPlayTimeEvent()
	{
	}
	
	public AudioPlayTimeEvent( AudioPlayTimeEvent _src ) : base(_src) 
	{
		m_AudioClipName = _src.m_AudioClipName ;
	}
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG_LOG		
		Debug.Log( "AudioPlayTimeEvent::ParseXML()" ) ;
#endif		
		if( false == base.ParseXML( _Node ) ||
			null == _Node.Attributes["AudioClipName"] )
		{
			return false ;
		}

		m_AudioClipName = _Node.Attributes["AudioClipName"].Value ;
#if DEBUG_LOG		
		Debug.Log( "AudioPlayTimeEvent::ParseXML() m_AudioClipName=" + m_AudioClipName ) ;
#endif	

		
		return true ;	
	}
		
	protected override void DoStartOfEvent()
	{
#if DEBUG_LOG			
		Debug.Log( "DoStartOfEvent()" ) ;
#endif 		
		if( null == m_EventManagerObj )
		{
			m_EventManagerObj = GlobalSingleton.GetUsualEventManagerObject() ;			
		}
		
		if( null == m_Audio )
		{
			m_Audio = ResourceLoad.LoadAudio( m_AudioClipName ) ;
		}
		
		
		if( null != m_Audio &&
			null != m_EventManagerObj )
			m_EventManagerObj.GetComponent<AudioSource>().PlayOneShot( m_Audio ) ;
	}	
	
	protected override void DoCloseOfEvent()
	{
#if DEBUG_LOG			
		Debug.Log( "DoCloseOfEvent()" ) ;
#endif
		if( null != m_EventManagerObj )			
			m_EventManagerObj.GetComponent<AudioSource>().Stop() ;
	}
		
}

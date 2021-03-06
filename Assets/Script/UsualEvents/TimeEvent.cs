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
@date TimeEvent.cs
@author NDark 

時間事件

# 有開始與結束兩個時間點的觸發事件
# 持續準備好
# 開始事件
# 持續啟動中
# 結束事件
# 持續結束中
# 必須指定參數
## StartSec
## ElapsedSec

@date 20121221 . file created.
@date 20130116 by NDark 
. add class method ParseTime()
. add class method SetupTime()

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic ;
using System.Xml;


public class TimeEvent : UsualEvent 
{
	protected TimeTrigger m_Trigger = new TimeTrigger() ; // 觸發開始與結束的計時器
	
	public override void Update()
	{
		if( m_Trigger.IsAboutToStart( true ) )
		{
			DoStartOfEvent() ;
		}		
		else if( m_Trigger.IsReady() )
		{
			DoKeepReady() ;
		}
		else if( m_Trigger.IsAboutToClose( true ) )
		{
			DoCloseOfEvent() ;
		}
		else if( m_Trigger.IsActive() )
		{
			DoKeepActive() ;
		}
		else if( m_Trigger.IsClosed() )
		{
			DoKeepClose() ;
		}
	}
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG_LOG		
		Debug.Log( "TimeTrigger::ParseXML()" ) ;
#endif		
		return ParseTime( _Node ) ;
	}
	
	public virtual bool ParseTime( XmlNode _Node )
	{
		if( null != _Node.Attributes["StartSec"] &&
			null != _Node.Attributes["ElapsedSec"] )
		{
			string startSecStr = _Node.Attributes["StartSec"].Value ;
			string elapsedSecStr = _Node.Attributes["ElapsedSec"].Value ;
			float StartSec = 0.0f ;
			float ElapsedSec = 0.0f ;
			float.TryParse( startSecStr , out StartSec ) ;
			float.TryParse( elapsedSecStr , out ElapsedSec ) ;			

			this.SetupTime( StartSec , ElapsedSec ) ;
			return true ;
		}
		return false ;
	}
	
	public virtual void SetupTime( float _StartSec , 
								   float _ElapsedSec )
	{
#if DEBUG_LOG		
		Debug.Log( "TimeTrigger::SetupTime() _StartSec=" + _StartSec + " _ElapsedSec=" + _ElapsedSec ) ;
#endif		
		m_Trigger.Setup( _StartSec , _ElapsedSec ) ;
	}
	
	public TimeEvent()
	{
		
	}
	
	public TimeEvent( TimeEvent _src )
	{
		m_Trigger = new TimeTrigger( _src.m_Trigger ) ;
	}
	
	protected virtual void DoKeepReady()
	{
		
	}	
	
	protected virtual void DoStartOfEvent()
	{
		
	}	
	
	protected virtual void DoKeepActive()
	{
		
	}	
	
	protected virtual void DoCloseOfEvent()
	{
		
	}	
	
	protected virtual void DoKeepClose()
	{
		
	}		
	
	
}

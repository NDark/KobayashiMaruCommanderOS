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
@date EnableMiniMapConditionEvent.cs
@author NDark 

啟動小地圖時間事件

# 啟動

@date 20121225 by NDark . file started.
@date 20130117 by NDark . rename file EnableMiniMapEvent to EnableMiniMapConditionEvent
*/
// #define DEBUG

using UnityEngine;
using System.Xml;

public class EnableMiniMapConditionEvent : ConditionEvent 
{
	bool m_Enable = false ;
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG_LOG				
		Debug.Log( "EnableMiniMapConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		if( null == _Node.Attributes["Enable"] )
		{
			return false ;
		}
		
		string enableStr = _Node.Attributes["Enable"].Value ;
		m_Enable = ( enableStr == "true" ) ? true : false ;
		return true ;
	}
	
	public EnableMiniMapConditionEvent()
	{
	}
	
	public EnableMiniMapConditionEvent( EnableMiniMapConditionEvent _src )
	{
		m_Enable = _src.m_Enable ;
	}
	
	public override void DoEvent()
	{
#if DEBUG_LOG				
		Debug.Log( "EnableMiniMapConditionEvent::DoEvent()" ) ;
#endif		
		EnableMiniMap() ;
	}	
		
	
	private void EnableMiniMap()
	{
		ClickOnGUI_SwitchMiniMap switcher = GlobalSingleton.GetSwitchMiniMap() ;
		if( null != switcher )
			switcher.EnableMiniMap( m_Enable ) ;
	}	
	
}

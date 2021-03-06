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
@file MoveObjectPositionConditionEvent.cs
@author NDark

移動物件的時間事件

# ObjectName 目標物件
# 目標地點 使用 Position3D 相同的標籤

@date 20130104 file started.
@date 20130117 rename MoveObjectPositionTimeEvent to MoveObjectPositionConditionEvent
*/
// #define DEBUG
using UnityEngine;
using System.Xml ;

public class MoveObjectPositionConditionEvent : ConditionEvent 
{
	private NamedObject m_TargetObject = new NamedObject() ;
	private Vector3 m_MoveToPosition = Vector3.zero ;
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG_LOG				
		Debug.Log( "MoveObjectPositionConditionEvent::ParseXML()" ) ;
#endif
		PosAnchor posAnchor = new PosAnchor() ;
		for( int i = 0 ; i < _Node.ChildNodes.Count ; ++i )
		{
			if( true == base.ParseXML( _Node.ChildNodes[ i ] ) )
			{
				
			}
			else if( true == XMLParseLevelUtility.ParsePosition( _Node.ChildNodes[ i ] ,
																 ref posAnchor ) )
			{
				m_MoveToPosition = posAnchor.GetPosition() ;
			}			
		}
		
		if( null == _Node.Attributes["ObjectName"] )
		{
			return false ;
		}
		
		m_TargetObject.Setup( _Node.Attributes["ObjectName"].Value , null );
		

		return true ;		
	}
	
	public MoveObjectPositionConditionEvent()
	{
	}
	
	public MoveObjectPositionConditionEvent( MoveObjectPositionConditionEvent _src )
	{
		m_TargetObject.Setup( _src.m_TargetObject ) ;
		m_MoveToPosition = _src.m_MoveToPosition ;
	}
	
	public override void DoEvent()
	{
#if DEBUG_LOG				
		Debug.Log( "MoveObjectPositionConditionEvent::DoEvent()" ) ;
#endif		
		MoveToPosition() ;
	}	
	
	private void MoveToPosition()
	{
		// Debug.Log( "DoStartOfEvent()" ) ;
		if( null != m_TargetObject.Obj )
		{
			m_TargetObject.Obj.transform.position = m_MoveToPosition ;
		}
	}	
}

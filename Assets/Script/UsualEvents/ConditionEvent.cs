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
@date ConditionEvent.cs
@author NDark

條件事件

# ParseForChildren() 會分析XML內的子節點，決定條件的類型

@date 20130118 file created and copy from UsualEvent.cs
@date 20130118 by NDark 
. change class member from m_Condition to m_Conditions.
. add class method ParseForChildren()

*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class ConditionEvent : UsualEvent
{
	public ConditionEvent()
	{
	}
	
	public ConditionEvent( ConditionEvent _src ) : base( _src )
	{
	}
	
	public virtual void SetupCondition( Condition _Condition )
	{
		m_Conditions.Add( _Condition ) ;
	}
	
	public virtual bool ParseForChildren( XmlNode _Node )
	{
		for( int i = 0 ; i < _Node.ChildNodes.Count ; ++i )
		{
			if( true == ParseXMLPrivate( _Node.ChildNodes[ i ] ) )
			{
				
			}
		}
		return ( m_Conditions.Count > 0 ) ;
	}
	
	private bool ParseXMLPrivate( XmlNode _Node )
	{
#if DEBUG_LOG				
		Debug.Log( "ConditionEvent::ParseXMLPrivate() _Node.Name=" + _Node.Name ) ;
#endif	

		if( null != _Node.Attributes[ "ConditionName" ] )
		{
			string ConditionName = _Node.Attributes[ "ConditionName" ].Value ;
			Condition addCondition = ConditionFactory.GetByString( ConditionName ) ;
			if( null == addCondition )
				Debug.Log( "ConditionEvent::ParseXML() null == addCondition , ConditionName=" + ConditionName ) ;
			else
			{
#if DEBUG_LOG				
				Debug.Log( "ConditionEvent::ParseXML() , ConditionName=" + ConditionName ) ;
#endif					
				if( true == addCondition.ParseXML( _Node ) )
				{
					SetupCondition( addCondition ) ;
					return true ;
				}
			}
		}
		return false ;
	}	
	public override bool ParseXML( XmlNode _Node )
	{
		return ParseXMLPrivate( _Node ) ;
	}
	
	public override void Update()
	{
		if( true == m_HasTriggered ||
			m_Conditions.Count == 0 )
			return ;

		List<Condition>.Enumerator eList = m_Conditions.GetEnumerator() ;
		while( eList.MoveNext() )
		{
			if( false == eList.Current.IsTrue() )
			{
				return ;
			}
		}
#if DEBUG_LOG		
		Debug.Log( "ConditionEvent::Update() DoEvent" ) ;
#endif
		DoEvent() ;			
		m_HasTriggered = true ;
	}
	
	public virtual void DoEvent()
	{
	}
	
	protected bool m_HasTriggered = false ;
	protected List<Condition> m_Conditions = new List<Condition>() ;
}
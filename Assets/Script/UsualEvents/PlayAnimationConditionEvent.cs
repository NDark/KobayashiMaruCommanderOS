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
@file PlayAnimationConditionEvent.cs
@author NDark

碰撞後播放動畫事件

# 播放物件 AnimationObjectName
# 子物件 AnimationChildObjectName
# 動畫名稱 AnimationName

@date 20130104 file started.
@date 20130117 by NDark . rename PlayAnimationCollisionEvent to PlayAnimationConditionEvent
*/
// #define DEBUG
using UnityEngine;
using System.Xml;

public class PlayAnimationConditionEvent : ConditionEvent 
{
	public NamedObject m_AnimationObject = new NamedObject() ;
	public string m_ChildName = "" ;
	public string m_AnimationName = "" ;
	
	public NamedObject m_CheckUnit = new NamedObject() ;
	public NamedObject m_CheckPosition = new NamedObject() ;
	public float m_JudgeDistance = 0.0f ;
	
	
	public PlayAnimationConditionEvent()
	{
		
	}
	
	public PlayAnimationConditionEvent( PlayAnimationConditionEvent _src )
	{
		
	}	
	
	/*
	<UsualEvent EventName="PlayAnimationConditionEvent"
			StartSec="0.0"
			AnimationObjectName="Unit_BlackHole001"
			AnimationChildObjectName="Dummy"
			AnimationName="BlackHole01"
			ObjectName="MainCharacter"
			LocationObjName="MainCharacterStartPosition"
			JudgeDistance="10"
			 />	
	 */
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG_LOG				
		Debug.Log( "PlayAnimationConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		if( null != _Node.Attributes["AnimationObjectName"] )
			m_AnimationObject.Setup( _Node.Attributes["AnimationObjectName"].Value , null ) ;
		
		if( null != _Node.Attributes["AnimationChildObjectName"] )
			m_ChildName = _Node.Attributes["AnimationChildObjectName"].Value ;
		
		if( null != _Node.Attributes["AnimationName"] )
			m_AnimationName = _Node.Attributes["AnimationName"].Value ;
		
		return true ;		
	}

	public override void DoEvent()
	{
#if DEBUG_LOG				
		Debug.Log( "PlayAnimationConditionEvent::DoEvent()" ) ;
#endif		
		ActiveAnimation() ;
	}	
	
	private void ActiveAnimation()
	{
		if( null != m_AnimationObject.Obj )
		{
			// Debug.Log( "m_ChildName=" + m_ChildName ) ;
			Transform trans = m_AnimationObject.Obj.transform.FindChild( m_ChildName ) ;
			if( null != trans )
			{
				Animation anim = trans.gameObject.GetComponent<Animation>() ;
				if( null != anim )
				{
					// Debug.Log( "m_AnimationName=" + m_AnimationName ) ;
					anim.Play( m_AnimationName ) ;
				}
			}
		}
	}
}

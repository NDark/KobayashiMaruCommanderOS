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
@file Conversation.cs
@author NDark
@date 20130103 file started.
@date 20130107 by NDark . add GUI_Conversation_TextBackground at EnableDialog()
@date 20130112 by NDark . re-facotr and comment.
@date 20130121 by NDark
. add class member m_Index of TalkStr
. remove class member m_EnglishStr of TalkStr
. remove class member m_ChineseStr of TalkStr
. remove class method TalksHasText()
@date 20130205 by NDark . comment

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;

/*
# 字串的指定編號.
*/
[System.Serializable]
public class TalkStr
{
	public int m_Index = 0 ;
	
	public TalkStr()
	{
	}
	
	public TalkStr( TalkStr _src )
	{
		m_Index = _src.m_Index ;
	}
}

/*
# 大頭像 指定大頭像的GUI物件名稱(左邊或右邊)以及要使用的貼圖名稱
*/
[System.Serializable]
public class Potrait
{
	public string m_PotraitName = "" ;
	public string m_TextureName = "" ;
	
	public Potrait()
	{
		
	}
	
	public Potrait( Potrait _src )
	{
		m_PotraitName = _src.m_PotraitName ;
		m_TextureName = _src.m_TextureName ;
	}
}

/*
# 對話包含
## 複數個大頭像
## 複數個字串
# EnableDialog() 指定大頭像及各顯示介面物件
# 呼叫 PlayNext() 依據對話的流程依序顯示各字串直到結束為止
# IsFinished() 檢查是否已經結束
*/
[System.Serializable]
public class Conversation 
{
	public List<Potrait> m_Potrits = new List<Potrait>() ;
	public List<TalkStr> m_Talks = new List<TalkStr>() ;
	public int m_CurrentIndex = 0 ;
	Dictionary<string,GameObject> m_GUIObjectListShare = null ;
	
	public bool IsFinished() 
	{
		bool ret = ( m_CurrentIndex >= m_Talks.Count ) ;
#if DEBUG_LOG
		Debug.Log( "Conversation::IsFinished() " + ret ) ;
#endif
		return ret ;
	}
	
	public void PlayNext() 
	{
		if( true == IsFinished() )
			return ;
		
		
		TalkStr talkStr = m_Talks[ m_CurrentIndex ] ;
		
		int Index = talkStr.m_Index ;
		string DisplayStr = StrsManager.Get( Index ) ;
#if DEBUG_LOG
		Debug.Log( "Conversation::PlayNext() DisplayStr=" + DisplayStr ) ;
#endif			
		
		string key = "GUI_Conversation_Text" ;
		GameObject textObj = m_GUIObjectListShare[ key ] ;
		if( null != textObj )
		{
			var guiText = textObj.GetComponent<UnityEngine.UI.Text>() ;
			if( null != guiText )
			{
				guiText.text = DisplayStr ;
			}
		}
		
		
		++m_CurrentIndex ;
#if DEBUG_LOG
		Debug.Log( "Conversation::PlayNext() m_CurrentIndex=" + m_CurrentIndex ) ;
#endif
	}
	
	// 指定大頭像及各顯示介面物件
	public void EnableDialog( ref Dictionary<string,GameObject> _GUIObjectList )
	{
		// 先關閉原本的
		foreach( var i in _GUIObjectList )
		{
			if( null == i.Value )
			{
				Debug.LogError("null == i.Value" + i.Key );
				continue ;
			}
			ShowUGUI.Show( i.Value , false , true , false ) ;
		}
		_GUIObjectList.Clear() ;
		
		// 檢查english text
		GameObject texObj = GlobalSingleton.GetGUI_ConversationTextObject() ;
		if( null == texObj )
		{
			Debug.LogError("null == texObj");
			return ;
		}
		else
		{
			ShowUGUI.Show( texObj , true , true , false ) ;
			_GUIObjectList[ texObj.name ] = texObj ;
		}

		// buttons
		var gui_Conversation_NextButtonBackgroundObj = GlobalSingleton.GetGUI_ConversationNextObject() ;
		if( null == gui_Conversation_NextButtonBackgroundObj )
		{
			Debug.LogError("null == gui_Conversation_NextButtonBackgroundObj");
		}
		else
		{
			var gui_Conversation_NextTrans = gui_Conversation_NextButtonBackgroundObj.transform.Find("GUI_Conversation_Next");
			if( null == gui_Conversation_NextTrans )
			{
				Debug.LogError("null == gui_Conversation_NextTrans");
			}
			else 
			{
				_GUIObjectList[ "GUI_Conversation_Next" ] = gui_Conversation_NextTrans.gameObject ;
			}
			_GUIObjectList[ "GUI_Conversation_NextButtonBackground" ] = gui_Conversation_NextButtonBackgroundObj ;
		}
		_GUIObjectList[ "GUI_Conversation_TextBackground" ] = GlobalSingleton.GetGUI_ConversationChildObject( "GUI_Conversation_TextBackground" ) ;
		
		// potraits
		foreach( Potrait potrait in m_Potrits )
		{
			string objName = "GUI_Conversation_" +  potrait.m_PotraitName ;
			_GUIObjectList[ objName ] = GlobalSingleton.GetGUI_ConversationChildObject( objName ) ;
			if( null != _GUIObjectList[ objName ] )
			{
				var image = _GUIObjectList[ objName ].GetComponent<UnityEngine.UI.Image>() ;
				if( null != image )
				{
					var tex = ResourceLoad.LoadTexture( potrait.m_TextureName ) as Texture2D;
					var rect = new Rect( 0 , 0 , tex.width , tex.height ) ;
					var sprite = Sprite.Create( tex , rect , CONST_Pivot ) ;
					image.sprite = sprite ;
				}
			}
		}
		
		m_GUIObjectListShare = _GUIObjectList ;
	}
	Vector2 CONST_Pivot = new Vector2( 0.5f , 0.5f ) ;
	
	public Conversation()
	{
	}

	public Conversation( Conversation _src )
	{
		m_Talks = _src.m_Talks ;
		m_Potrits = _src.m_Potrits ;
		m_CurrentIndex = _src.m_CurrentIndex ;
		m_GUIObjectListShare = _src.m_GUIObjectListShare ;
	}	
	
	
}

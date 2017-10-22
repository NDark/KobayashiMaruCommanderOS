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
@file ShowUGUI.cs
@brief 取得物件並顯示/關閉 GUITexture 
@author NDark
 

@date 20170621 by NDark . derived from ShowGUITexture

*/
// #define DEBUG 

using UnityEngine;

public static class ShowUGUI
{
	public static void Show( string _GUITextureObjName , bool _Show , bool _WithText , bool _WithChildren )
	{
		GameObject guiObj = GameObject.Find( _GUITextureObjName ) ;
		Show( guiObj , _Show , _WithText , _WithChildren ) ;		
	}
	
	public static void Show( GameObject _GUIObj , bool _Show , bool _WithText , bool _WithChildren )
	{
		if( null == _GUIObj )
		{
			Debug.Log( "ShowUGUI::Show() null == _GUIObj" ) ;
			return ;
		}
#if DEBUG_LOG
		Debug.Log( "ShowUGUI::Show()" + _GUIObj.name + " _Show=" + _Show + " _WithText=" + _WithText + " _WithChildren=" + _WithChildren ) ;
#endif		
		if( false == _WithChildren )
		{
			SetComponentEnable<UnityEngine.UI.Image>( _GUIObj , _Show ) ;
			SetComponentEnable<UnityEngine.UI.RawImage>( _GUIObj , _Show ) ;
			if( true == _WithText )
			{
				SetComponentEnable<UnityEngine.UI.Text>( _GUIObj , _Show ) ;
			}
		}
		else
		{
			SetComponentsInChildrenEnable<UnityEngine.UI.Image>( _GUIObj , _Show ) ;
			SetComponentsInChildrenEnable<UnityEngine.UI.RawImage>( _GUIObj , _Show ) ;
			if( true == _WithText )
			{
				SetComponentsInChildrenEnable<UnityEngine.UI.Text>( _GUIObj , _Show ) ;
			}
		}
	}
	
	public static void SetComponentEnable<T>( GameObject _Obj , bool _Set )
	{
		var component = _Obj.GetComponent<T>() as Behaviour ;
		if( null != component )
		{
			component.enabled = _Set ;
		}
	}
	public static void SetComponentsInChildrenEnable<T>( GameObject _Obj , bool _Set )
	{
		var components = _Obj.GetComponentsInChildren<T>() as Behaviour[] ;
		foreach( var c in components )
		{
			c.enabled = _Set ;
		}
	}
	
	public static void Switch( GameObject _Obj 
		, bool _WithText 
		, bool _WithChildren )
	{
		#if DEBUG_LOG
		Debug.Log( "ShowUGUI::Switch()" + _GUIObj.name ) ;
		#endif		
		
		if( null == _Obj )
			return ;
		
		var image = _Obj.GetComponentInChildren<UnityEngine.UI.Image>() ;
		var text = _Obj.GetComponentInChildren<UnityEngine.UI.Text>() ;
		bool enable = false ;
		if( null != image )
			enable = image.enabled ;
		else if( null != text )
			enable = text.enabled ;
		else
		{
			Debug.Log( "ShowUGUI::Switch() no Image and Text" ) ;
			return ;
		}		
		
		Show( _Obj , !enable , _WithText , _WithChildren ) ;
	}	
}

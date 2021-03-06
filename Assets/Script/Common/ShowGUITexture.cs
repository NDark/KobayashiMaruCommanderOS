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
@file ShowGUITexture.cs
@brief 取得物件並顯示/關閉 GUITexture 
@author NDark
 
取得物件並顯示/關閉 GUI 
## Show() 顯示關閉指定物件的 GUITexture , GUIText , 或是物件其下的子物件
## Switch() 切換指定物件其下所有的 GUITexture , GUIText , 或是物件其下的子物件

@date 20121109 by NDark . refine code.
@date 20121203 by NDark 
. comment.
. add class method Show()
. add class method ShowAndItsAllChildren()
@date 20121219 by NDark 
. add class method ShowAndItsAllChildrenWithText()
. add class method Switch()
@date 20121225 by NDark . add class method SwitchAndItsAllChildren()
@date 20130103 by NDark . add class method ShowWithText()
@date 20130115 by NDark . refactor and comment.

*/
// #define DEBUG 

using UnityEngine;

public static class ShowGUITexture
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
			Debug.Log( "ShowGUITexture::Show() null == _GUIObj" ) ;
			return ;
		}
#if DEBUG_LOG
		Debug.Log( "ShowGUITexture::Show()" + _GUIObj.name + " _Show=" + _Show + " _WithText=" + _WithText + " _WithChildren=" + _WithChildren ) ;
#endif		
		if( false == _WithChildren )
		{
			if( null != _GUIObj.GetComponent<GUITexture>() )
			{
				_GUIObj.GetComponent<GUITexture>().enabled = _Show ;
			}
			
			if( true == _WithText &&
				null != _GUIObj.GetComponent<GUIText>() )
			{
				_GUIObj.GetComponent<GUIText>().enabled = _Show ;
			}
		}
		else
		{
			GUITexture[] guiTextures = null ;
			guiTextures = _GUIObj.GetComponentsInChildren<GUITexture>() ;
			foreach( GUITexture guiTexture in guiTextures )
			{
				guiTexture.enabled = _Show ;
			}
			
			if( true == _WithText )
			{
				GUIText[] guiTexts = null ;
				guiTexts = _GUIObj.GetComponentsInChildren<GUIText>() ;
				foreach( GUIText guiText in guiTexts )
				{
					guiText.enabled = _Show ;
				}
			}
		}
	}
	
	public static void Switch( GameObject _GUIObj , bool _WithText , bool _WithChildren )
	{
#if DEBUG_LOG
		Debug.Log( "ShowGUITexture::Switch()" + _GUIObj.name ) ;
#endif		
		if( null == _GUIObj )
			return ;
		
		GUITexture guiTexture = _GUIObj.GetComponentInChildren<GUITexture>() ;
		GUIText guiText = _GUIObj.GetComponentInChildren<GUIText>() ;
		bool enable = false ;
		if( null != guiTexture )
			enable = guiTexture.enabled ;
		else if( null != guiText )
			enable = guiText.enabled ;
		else
		{
			Debug.Log( "ShowGUITexture::Switch() no GUITexture and GUIText" ) ;
			return ;
		}		
		
		Show( _GUIObj , !enable , _WithText , _WithChildren ) ;
	}
	
}

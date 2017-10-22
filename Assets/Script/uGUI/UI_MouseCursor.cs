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
@file UI_MouseCursor.cs
@author NDark

@date 2017826 file started.
*/
using UnityEngine;

public class UI_MouseCursor : GUI_MouseCursor 
{
	
	// Update is called once per frame
	void Update () 
	{
		if( null == m_MouseCursor.GetObj() )
		{
			Debug.LogWarning("null == m_MouseCursor.GetObj");
			return ;
		}
		
		// 滑鼠位置
		if( Input.mousePosition.x < 0 ||
			Input.mousePosition.x >= Screen.width ||
			Input.mousePosition.y < 0 ||
			Input.mousePosition.y >= Screen.height )
		{
			return ;
		}
		
		if( null == m_MouseCursorRect )
		{
			m_MouseCursorRect = m_MouseCursor.Obj.GetComponent<UnityEngine.RectTransform>() ;
		}
		if( null == m_MouseCursorImage )
		{
			m_MouseCursorImage = m_MouseCursor.Obj.GetComponent<UnityEngine.UI.Image>() ;
		}
		
		if( null == m_MouseCursorRect )
		{
			return ;
		}
		if( null == m_MouseCursorImage )
		{
			return ;
		}
		
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;		
		bool IsNoneFunction = ( controller.m_SelectModeNow == SelectFunctionMode.None ) ;
		bool IsAnimation = false ;
		string unitName = IsOverUnit( Input.mousePosition ) ;
		// Debug.Log( unitName ) ;
		if( unitName == ConstName.MainCharacterObjectName )
		{
			m_MouseCursorImage.enabled = false ;
		}
		else if( 0 == unitName.Length )
		{
			if( true == IsNoneFunction )
			{
				// 沒有功能 沒有單位 不顯示
				m_MouseCursorImage.enabled = false ;
			}
			else
			{
				// 有功能 沒有單位 顯示指定貼圖
				m_MouseCursorImage.enabled = true ;
			}
		}
		else if( true == IsSelection( unitName ) )
		{
			
			if( true == IsNoneFunction )
			{
				// 有單位 且鎖定 無功能 顯示黃框縮放動畫 
				m_MouseCursorImage.enabled = false ;
			}
			else
			{
				// 有單位 且鎖定 有功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				m_MouseCursorImage.enabled = true ;
			}
		}
		else
		{
			// 有單位 沒鎖定
			if( true == IsNoneFunction )
			{
				// 無功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				m_MouseCursorImage.enabled = true ;
			}
			else
			{
				// 有功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				m_MouseCursorImage.enabled = true ;
			}			
		}
		
		if( true == m_MouseCursorImage.enabled )
		{
			// animation resize
			if( true == IsAnimation )
			{
				AnimationResize( m_MouseCursorRect ) ;
			}
			else
			{
				ResetAnimationSize( m_MouseCursorRect ) ;
			}
			
			// position of mouse cursor
			m_MouseCursorRect.anchoredPosition = Input.mousePosition ;
			
			/*
				public static string GUIMouseCursor_PhaserTexturePath = "Common/Textures/UI_MouseCursor_Phaser" ;
				public static string GUIMouseCursor_TorpedoTexturePath = "Common/Textures/UI_MouseCursor_Torpedo" ;
				public static string GUIMouseCursor_TrakorBeamTexturePath = "Common/Textures/UI_MouseCursor_TrakorBeam" ;
			 */		
			Texture tex = null ;
			
			switch( controller.m_SelectModeNow )
			{
			case SelectFunctionMode.None :
				tex = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_SelectionTexturePath ) ;
				break ;
			case SelectFunctionMode.WeaponPhaser :
				tex = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_PhaserTexturePath ) ;
				break ;
			case SelectFunctionMode.WeaponTorpedo :
				tex = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_TorpedoTexturePath ) ;
				break ;
			case SelectFunctionMode.FunctionTrakorBeam :
				tex = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_TrakorBeamTexturePath ) ;
				break ;
			case SelectFunctionMode.SpecialModeMultipleAttack :
				tex = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_MultiAttackTexturePath ) ;
				break ;				
			}	
			
			if( null != tex )
			{
				Rect rect = new Rect( 0 , 0 , tex.width , tex.height ) ;
				m_MouseCursorImage.sprite = Sprite.Create( tex as Texture2D, rect , pivot ) ;
				// m_MouseCursorImage.SetNativeSize() ;
			}
		}
		else
		{
			ResetAnimationSize( m_MouseCursorRect ) ;
		}
	}
	Vector2 pivot = new Vector2( 0.5f , 0.5f ) ;

	void ResetAnimationSize( UnityEngine.RectTransform _Rect )
	{
		var size = _Rect.sizeDelta ;
		size.x = m_StandardSize ;
		size.y = m_StandardSize ;
		_Rect.sizeDelta = size ;
		
		m_SizeNow = m_StandardSize ;
		m_Direction = Mathf.Abs( m_Direction ) ;
	}
	
	void AnimationResize( UnityEngine.RectTransform _Rect )
	{
		
		var size = _Rect.sizeDelta ;
		size.x = m_SizeNow ;
		size.y = m_SizeNow ;
		_Rect.sizeDelta = size ;
		
		if( m_SizeNow > m_MaximumSize )
		{
			m_Direction *= -1 ;
		}
		else if( m_SizeNow < m_StandardSize )
		{
			m_Direction *= -1 ;
		}
		m_SizeNow += m_Direction ;
		
		
	}	
	
	UnityEngine.RectTransform m_MouseCursorRect = null ;
	UnityEngine.UI.Image m_MouseCursorImage = null ;
}

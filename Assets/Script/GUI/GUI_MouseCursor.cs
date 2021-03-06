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
@file GUI_MouseCursor.cs
@author NDark

# 更新MouseCursor的顯示及貼圖
# 玩家無功能的情形下不顯示貼圖
# 玩家無功能但是有在一個單位(滑鼠座標)上,
# 如果沒有鎖定該單位,顯示黑框並有放大縮小動畫
# 如果已經鎖定該單位,不顯示貼圖
# 如果有功能,則顯示對應的貼圖
# 如果有功能,且滑鼠在一個單位上,則有放大縮小動畫
# IsOverUnit() 滑鼠是否在MainUpdate已知的單位上
# IsSelection() 傳入字串是否是已經被選擇的單位
# ResetAnimationSize() 重置滑鼠游標動畫
# AnimationResize() 進行滑鼠游標動畫


@date 20121226 file started.
@date 20121227 by NDark 
. modify the animation parameter
. refactor class method IsOverUnit()
@date 20121229 by NDark . add checking of MainCharacterObjectName at Update()
@date 20121231 by NDark . add SpecialModeMultipleAttack at Update()
@date 20130113 by NDark . comment.

*/
using UnityEngine;

public class GUI_MouseCursor : MonoBehaviour 
{
	/*
	 * 滑鼠游標的物件
	 * 在啟動功能面板的功能時會有滑鼠游標
	 */
	protected NamedObject m_MouseCursor = new NamedObject( "GUI_MouseCursor" ) ;	
	protected float m_StandardSize = 48 ;
	protected float m_SizeNow = 48 ;
	protected float m_MaximumSize = 52 ;
	protected float m_Direction = 0.3f ;
	
	// Use this for initialization
	void Start () 
	{
		m_MouseCursor.ForceObj() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == m_MouseCursor.GetObj() )
		{
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
		
		GUITexture guiTexture = m_MouseCursor.Obj.GetComponent<GUITexture>() ;
		
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;		
		bool IsNoneFunction = ( controller.m_SelectModeNow == SelectFunctionMode.None ) ;
		bool IsAnimation = false ;
		string unitName = IsOverUnit( Input.mousePosition ) ;
		// Debug.Log( unitName ) ;
		if( unitName == ConstName.MainCharacterObjectName )
		{
			guiTexture.enabled = false ;
		}
		else if( 0 == unitName.Length )
		{
			if( true == IsNoneFunction )
			{
				// 沒有功能 沒有單位 不顯示
				guiTexture.enabled = false ;
			}
			else
			{
				// 有功能 沒有單位 顯示指定貼圖
				guiTexture.enabled = true ;
			}
		}
		else if( true == IsSelection( unitName ) )
		{
			
			if( true == IsNoneFunction )
			{
				// 有單位 且鎖定 無功能 顯示黃框縮放動畫 
				guiTexture.enabled = false ;
			}
			else
			{
				// 有單位 且鎖定 有功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				guiTexture.enabled = true ;
			}
		}
		else
		{
			// 有單位 沒鎖定
			if( true == IsNoneFunction )
			{
				// 無功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				guiTexture.enabled = true ;
			}
			else
			{
				// 有功能 顯示黃框縮放動畫 
				IsAnimation = true ;
				guiTexture.enabled = true ;
			}			
		}
		
		if( true == guiTexture.enabled )
		{
			// animation resize
			if( true == IsAnimation )
			{
				AnimationResize( guiTexture ) ;
			}
			else
			{
				ResetAnimationSize( guiTexture ) ;
			}
			
			// position of mouse cursor
			guiTexture.pixelInset = new Rect( Input.mousePosition.x - guiTexture.pixelInset.width / 2, 
											  Input.mousePosition.y - guiTexture.pixelInset.height / 2 , 
											  guiTexture.pixelInset.width , 
											  guiTexture.pixelInset.height ) ;
			
			/*
				public static string GUIMouseCursor_PhaserTexturePath = "Common/Textures/GUI_MouseCursor_Phaser" ;
				public static string GUIMouseCursor_TorpedoTexturePath = "Common/Textures/GUI_MouseCursor_Torpedo" ;
				public static string GUIMouseCursor_TrakorBeamTexturePath = "Common/Textures/GUI_MouseCursor_TrakorBeam" ;
			 */		
			switch( controller.m_SelectModeNow )
			{
			case SelectFunctionMode.None :
				guiTexture.texture = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_SelectionTexturePath ) ;
				break ;
			case SelectFunctionMode.WeaponPhaser :
				guiTexture.texture = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_PhaserTexturePath ) ;
				break ;
			case SelectFunctionMode.WeaponTorpedo :
				guiTexture.texture = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_TorpedoTexturePath ) ;
				break ;
			case SelectFunctionMode.FunctionTrakorBeam :
				guiTexture.texture = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_TrakorBeamTexturePath ) ;
				break ;
			case SelectFunctionMode.SpecialModeMultipleAttack :
				guiTexture.texture = ResourceLoad.LoadTexture( ConstName.GUIMouseCursor_MultiAttackTexturePath ) ;
				break ;				
			}			
		}
		else
			ResetAnimationSize( guiTexture ) ;
	}
	
	protected string IsOverUnit( Vector3 _mousePosition )
	{
		string ret = "" ;
		MainUpdate mainUpdate = GlobalSingleton.GetMainUpdateComponent() ;
		if( null == mainUpdate )
			return ret ;
		RaycastHit hit = new RaycastHit() ;
		Ray ray = Camera.main.ScreenPointToRay( _mousePosition ) ;
		if( Physics.Raycast( ray , out hit ) )
		{
			string parentName = "" ;
			if( -1 != hit.collider.name.IndexOf( "ClickCube" ) &&
				null != hit.collider.transform.parent )
			{
				parentName = hit.collider.transform.parent.gameObject.name ;
				ret = parentName ;
			}
			
		}
		
		return ret ;
	}

	protected bool IsSelection( string _unitName )
	{
		UnitSelectionSystem selectSys = GlobalSingleton.GetMainCharacterSelectionSystem() ;
		if( null == selectSys )
			return false ;
		return ( selectSys.GetPrimarySelectUnitName() == _unitName ) ;
	}
	
	void ResetAnimationSize( GUITexture _guiTexture )
	{
		Rect size = _guiTexture.pixelInset ;
		size.width = m_StandardSize ;
		size.height = m_StandardSize ;
		_guiTexture.pixelInset = size ;
		m_SizeNow = m_StandardSize ;
		m_Direction = Mathf.Abs( m_Direction ) ;
	}
	
	void AnimationResize( GUITexture _guiTexture )
	{
		Rect size = _guiTexture.pixelInset ;
		size.width = m_SizeNow ;
		size.height = m_SizeNow ;
		_guiTexture.pixelInset = size ;
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
}

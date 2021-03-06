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
@file ClickOnGUI_SwitchUnitDataGUI.cs
@author NDark

# 依照是否是主角部件來向 GUIUpdate 取得不同的background object
# 控制的部件集合background object 將其下所有物件都切換顯示
# 切換時切換顏色


@date 20121225 file started.
@date 20130112 by NDark 
. remove class method SetNotToMoveOfMainCharacter()
. refactor and comment.

*/
using UnityEngine;

public class ClickOnGUI_SwitchUnitDataGUI : MonoBehaviour 
{
	public bool m_IsMainCharacterUnitDataGUI = true ; // 依照是否是主角部件來向 GUIUpdate 取得不同的parent object
	protected NamedObject m_UnitDataGUIBackgroundObject = null ;// 控制的部件集合parent object
	
	public Color m_InitColor = Color.yellow ;
	public Color m_EnableColor = Color.yellow ;
	public Color m_DisableColor = Color.red ;	
	
	public virtual void ClickSwitch()
	{
		if( false == this.enabled )
			return ;
		
		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
		
		if( null != m_UnitDataGUIBackgroundObject &&
		   null != m_UnitDataGUIBackgroundObject.Obj )
		{
			GUITexture guiTexture = m_UnitDataGUIBackgroundObject.Obj.GetComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				bool enable = !guiTexture.enabled ;
				ShowGUITexture.Show( m_UnitDataGUIBackgroundObject.Obj , enable , true , true ) ;
				
				// 顏色
				EnableText( enable ) ;				
			}
		}
	}
	
	public virtual void EnableText( bool _Enalbe )
	{
		if( null != this.gameObject.GetComponent<GUIText>() )
			this.gameObject.GetComponent<GUIText>().material.color = ( _Enalbe ) ?
				m_EnableColor : m_DisableColor ;
	}
	
	
	// Use this for initialization
	void Start () 
	{
		if( null != this.gameObject.GetComponent<GUIText>() )
			this.gameObject.GetComponent<GUIText>().material.color = m_InitColor ;	
		
		FetchUnitDataGUIBackgroundObject() ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown()
	{
		ClickSwitch() ;
	}
	
	protected void FetchUnitDataGUIBackgroundObject()
	{
		GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
		if( null == guiUpdate )
			return ;
		
		if( true == m_IsMainCharacterUnitDataGUI )
			m_UnitDataGUIBackgroundObject = guiUpdate.m_MainCharacterGUIBackground ;
		else
			m_UnitDataGUIBackgroundObject = guiUpdate.m_SelectTargetGUIBackground ;
		
	}
	
}

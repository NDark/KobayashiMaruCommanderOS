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
@file MouseOver_UnitDataGUI.cs
@brief 滑鼠移動在 UnitDataGUI 的反應
@author NDark

# 掛在 HP 與 Label 上
# 更新 選擇部位選擇框的位置及顯示

@date 20121205 file started.
@date 20121205 by NDark . add visible of m_UnitDataSelectionUnActive

*/
using UnityEngine;

public class MouseOver_UnitDataGUI : MonoBehaviour 
{
	bool m_Active = true ;
	GameObject m_UnitDataSelectionUnActive = null ;

	// Use this for initialization
	void Start () 
	{
		// block the main character
		string parentName = GlobalSingleton.GetParentName( this.gameObject ) ;
		if( -1 != parentName.IndexOf( ConstName.MainCharacterObjectName ) )
		{
			m_Active = false ;
			return ;
		}
		
		m_UnitDataSelectionUnActive = GlobalSingleton.GetGUIUnitDataSelection_UnActive() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	
	void OnMouseOver()
	{
		if( false == m_Active )
			return ;
		if( null == m_UnitDataSelectionUnActive )
			return ;
		
		// retrieve 2d screen position
		Vector2 posOfOffset = Vector2.zero ;
		if( null != this.GetComponent<GUIText>() )
			posOfOffset = this.GetComponent<GUIText>().pixelOffset ;
		else if ( null != this.GetComponent<GUITexture>() )
		{
			posOfOffset = new Vector2( this.GetComponent<GUITexture>().pixelInset.x , 
									   this.GetComponent<GUITexture>().pixelInset.y ) ;
		}
		
		// selection un active
		ClickOn_UnitDataGUISelection_UnActive selectScript = m_UnitDataSelectionUnActive.GetComponent<ClickOn_UnitDataGUISelection_UnActive>() ;
		
		// find unit data gui component gui object
		GameObject parentObj = GlobalSingleton.GetParentObject( this.gameObject ) ;
		
		selectScript.m_SelectName = ConstName.SplitComponentObjectNameToComponentName( parentObj.name ) ;

		Vector3 screenPos = Camera.main.ViewportToScreenPoint( this.gameObject.transform.position ) ; 
		// Debug.Log( "parentObj.name=" + parentObj.name + " screenPos=" + screenPos ) ;
		GUITexture guiTexture = m_UnitDataSelectionUnActive.GetComponent<GUITexture>() ;
		if( null != guiTexture )
		{
			guiTexture.enabled = true ;
			guiTexture.pixelInset = new Rect( screenPos.x + posOfOffset.x , 
											  screenPos.y + posOfOffset.y - guiTexture.pixelInset.height / 2 , 
											  guiTexture.pixelInset.width , 
											  guiTexture.pixelInset.height ) ;
		}
	}
}

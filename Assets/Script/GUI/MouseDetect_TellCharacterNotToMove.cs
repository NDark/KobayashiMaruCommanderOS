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
@file MouseDetect_TellCharacterNotToMove.cs
@author NDark

範圍內通知主角不可因為點選而移動
 
# 會使用物件的位置來決定範圍
# 會使用GUITexture的pixelInset來決定範圍

@date 20121227 file started.
@date 20121229 by NDark . add texture checking at Update()
@date 20130119 by NDark 
. add class member m_GuiIsMoving
. add class member m_TextureRect
. add class method UpdateTextureRect()

*/
using UnityEngine;

public class MouseDetect_TellCharacterNotToMove : MonoBehaviour 
{
	
	public bool m_GuiIsMoving = false ;

	private GUITexture m_Texture = null ;
	private Rect m_TextureRect = new Rect() ;
	// Use this for initialization
	void Start () 
	{
		m_Texture = this.gameObject.GetComponent<GUITexture>() ;
		
		UpdateTextureRect() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_Texture &&
			true == m_Texture.enabled )
		{
			if( true == m_GuiIsMoving )
			{
				UpdateTextureRect() ;
			}
			
			// Debug.Log( this.gameObject.name + " " + textureRect ) ;
			Vector3 mousePosition = Input.mousePosition ;
			if( mousePosition.x > m_TextureRect.x &&
				mousePosition.y > m_TextureRect.y &&
				mousePosition.x <= m_TextureRect.x + m_TextureRect.width &&
				mousePosition.y <= m_TextureRect.y + m_TextureRect.height )
			{
				GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
			}
			
		}
	}
	
	protected virtual void UpdateTextureRect()
	{
		if( null != m_Texture )
		{
			Vector3 screenPos = Camera.main.ViewportToScreenPoint( this.gameObject.transform.position ) ;
			m_TextureRect = m_Texture.pixelInset ;
			m_TextureRect.x += screenPos.x ;
			m_TextureRect.y += screenPos.y ;			
		}
	}
}

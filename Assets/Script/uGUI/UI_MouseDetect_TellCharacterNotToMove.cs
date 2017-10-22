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
@file UI_MouseDetect_TellCharacterNotToMove.cs
@author NDark

@date 20171007 file started.

*/
using UnityEngine;

public class UI_MouseDetect_TellCharacterNotToMove : MouseDetect_TellCharacterNotToMove 
{

	RectTransform m_RectTransform = null ;
	UnityEngine.UI.Image m_Image = null ;
	// Use this for initialization
	void Start () 
	{
		m_RectTransform = this.gameObject.GetComponent<RectTransform>() ;
		m_Image= this.gameObject.GetComponent<UnityEngine.UI.Image>() ;

		UpdateTextureRect() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_Image &&
			true == m_Image.enabled )
		{
			if( true == m_GuiIsMoving )
			{
				UpdateTextureRect() ;
			}
			
			Vector3 mousePosition = Input.mousePosition ;

			m_RectTransform.GetWorldCorners( worldPos ) ;

			if( mousePosition.x > worldPos[0].x &&
				mousePosition.y > worldPos[0].y &&
				mousePosition.x <= worldPos[2].x &&
				mousePosition.y <= worldPos[2].y )
			{
				GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
			}
			
		}
	}
	
	protected override void UpdateTextureRect()
	{
	}

	Vector3[] worldPos = new Vector3[4] ;
}


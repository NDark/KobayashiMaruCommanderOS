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
@file UI_Click_SwitchMiniMap.cs
@author NDark

@date 20170806 file started.

*/
using UnityEngine;

public class UI_Click_SwitchMiniMap : ClickOnGUI_SwitchMiniMap 
{
	public RectTransform m_Rect = null ;
	public Vector2 m_Enable2DAnchorPos = Vector3.zero ;
	public Vector2 m_Disable2DAnchorPos = Vector3.zero ;
	
	public override void EnableMiniMap( bool _Enalbe )
	{
		Camera miniMapCamera = GetMiniMapCamera() ;
		if( null != miniMapCamera )
		{
			miniMapCamera.enabled = _Enalbe ;
			
			// 位置
			m_Rect.anchoredPosition = ( true == miniMapCamera.enabled ) ?
				m_Enable2DAnchorPos : m_Disable2DAnchorPos ;
			
			// 顏色
			var text = this.gameObject.GetComponent<UnityEngine.UI.Text>() ;
			if( null != text )
			{
				text.color = ( true == miniMapCamera.enabled ) ?
					m_EnableColor : m_DisableColor ;
			}
		}
	}
	
	
	
}

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
@file ClickOnGUI_SwitchMiniMap.cs
@author NDark


# 切換時按鈕會移動及變色
# EnableMiniMap() 開啟小地圖與否
# SwitchMiniMap() 切換小地圖開啟
# 預設是關閉


@date 20121225 file started.
@date 20130113 by NDark . refactor and comment.
@date 20130119 by NDark . add class method GetMiniMapEnable()

*/
using UnityEngine;

public class ClickOnGUI_SwitchMiniMap : MonoBehaviour 
{
	
	public Vector3 m_EnablePos = new Vector3( 0.22f , 0.999f , 5.0f ) ;
	public Color m_EnableColor = Color.yellow ;
	public Vector3 m_DisablePos = new Vector3( 0.0f , 0.999f , 5.0f ) ;
	public Color m_DisableColor = Color.red ;
	
	public void ClickSwitch()
	{
		SwitchMiniMap() ;
		
		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;		
	}
	
	public virtual void EnableMiniMap( bool _Enalbe )
	{
		Camera miniMapCamera = GetMiniMapCamera() ;
		if( null != miniMapCamera )
		{
			miniMapCamera.enabled = _Enalbe ;
			
			// 位置
			this.gameObject.transform.position = ( true == miniMapCamera.enabled ) ?
				m_EnablePos : m_DisablePos ;
			
			// 顏色
			if( null != this.gameObject.GetComponent<GUIText>() )
				this.gameObject.GetComponent<GUIText>().material.color = ( true == miniMapCamera.enabled ) ?
					m_EnableColor : m_DisableColor ;
		}
	}
	
	public bool GetMiniMapEnable() 
	{
		bool ret = false ;
		Camera miniMapCamera = GetMiniMapCamera() ;
		if( null != miniMapCamera )
		{
			ret = miniMapCamera.enabled ;
		}		
		return ret ;
	}
	
	// Use this for initialization
	void Start () 
	{
		EnableMiniMap( false ) ;		
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		ClickSwitch() ;
	}
	
	protected Camera GetMiniMapCamera()
	{
		Camera ret = null ;
		GameObject miniMapCameraObj = GlobalSingleton.GetMiniMapCameraObj() ;
		if( null != miniMapCameraObj )
		{
			ret = miniMapCameraObj.GetComponent<Camera>() ;
		}
		return ret ;
	}
	
	protected void SwitchMiniMap()
	{
		Camera miniMapCamera = GetMiniMapCamera() ;
		if( null != miniMapCamera )
			EnableMiniMap( !miniMapCamera.enabled ) ;			
	}	
}

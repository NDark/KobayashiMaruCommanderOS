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
@file ClickOnGUI_SwitchEnergyManipulator.cs
@author NDark
 
# 點選後切換能源模組顯示與否
# 切換後切換鈕會變色跟換位置
# EnableEnergyManipulator() 切換到指定狀態：開或關
# SwitchEnergyManipulator() 呼叫 GUI_EnergyManipulator 切換開或關
# 開啟時預設為關

@date 20130107 by NDark 
. move GetEnergyManipulatorParentObj() to GlobalSingleton.cs
. modify disable position m_DisablePos 
@date 20130112 by NDark . comment.
@date 20130205 by NDark . comment.

*/
using UnityEngine;

public class ClickOnGUI_SwitchEnergyManipulator : MonoBehaviour 
{
	public Vector3 m_EnablePos = new Vector3( 0.0f , 0.36f , 5.0f ) ;
	public Color m_EnableColor = Color.yellow ;
	public Vector3 m_DisablePos = new Vector3( 0.0f , 0.05f , 5.0f ) ;
	public Color m_DisableColor = Color.red ;
	
	public virtual void EnableEnergyManipulator( bool _Enalbe )
	{
		GameObject energyManipulatorObject = GlobalSingleton.GetEnergyManipulatorParentObj();
		if( null != energyManipulatorObject )
		{
			GUI_EnergyManipulator energyManipulator = energyManipulatorObject.GetComponent<GUI_EnergyManipulator>() ;
			if( null != energyManipulator )
			{
				energyManipulator.Active( _Enalbe ) ;
				energyManipulator.Show( _Enalbe ) ;
			}
			
			// 位置
			this.gameObject.transform.position = ( true == _Enalbe ) ?
				m_EnablePos : m_DisablePos ;
			
			// 顏色
			if( null != this.gameObject.GetComponent<GUIText>() )
				this.gameObject.GetComponent<GUIText>().material.color = ( true == _Enalbe ) ?
					m_EnableColor : m_DisableColor ;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		// # 開啟時預設為關
		EnableEnergyManipulator( false ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		// Debug.Log( "OnMouseDown" ) ;
		SwitchEnergyManipulator() ;

		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
	}
	
	protected virtual void SwitchEnergyManipulator()
	{
		GameObject energyManipulatorObject = GlobalSingleton.GetEnergyManipulatorParentObj() ;
		if( null != energyManipulatorObject )
		{
			// Debug.Log( "SwitchEnergyManipulator" ) ;
			GUITexture guiTexture = energyManipulatorObject.GetComponentInChildren<GUITexture>() ;
			if( guiTexture )
				EnableEnergyManipulator( !guiTexture.enabled ) ;
		}
	}		
}

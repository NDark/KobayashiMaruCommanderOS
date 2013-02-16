/*
IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

By downloading, copying, installing or using the software you agree to this license.
If you do not agree to this license, do not download, install, copy or use the software.

    License Agreement For Kobayashi Maru Commander Open Source

Copyright (C) 2013, Chih-Jen Teng(NDark) and Koguyue Entertainment, 
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
@file ClickOnThisCollider.cs
@brief 滑鼠點擊到3D物件
@author NDark

# 注意!!script要綁在有collider的物件下才會觸發。目前是綁在ClickCube這個子物件下。統一由此子物件來觸發點選單位。
# 檢查 OnMouseDown() / OnMouseUp() 。
# 假如曾經按下 mouse down 並在一定時間內放開才算一次的click。
# click call TellMainCharacterClick() 通知main character點選某物件。

@date 20121109 by NDark . refine code
...
@date 20130111 by NDark . re-factor and comment.

*/
// #define DEBUG

using UnityEngine;

public class ClickOnThisCollider : MonoBehaviour 
{
	private CountDownTrigger m_LeftClick = new CountDownTrigger( BaseDefine.CLICK_SEC ) ;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		m_LeftClick.Rewind() ;
	}
	
	void OnMouseUp()
	{
#if DEBUG		
		Debug.Log( "ClickOnThisCollider::OnMouseUp() this.gameObject.name=" + this.gameObject.name  ) ;
#endif
		// only for click left button
		if( false == m_LeftClick.IsCountDownToZero() )
		{
			// smaller than click
			string clickObjName = GlobalSingleton.GetParentName( this.gameObject ) ;
			// find parent name
			TellMainCharacterClick( clickObjName ) ;			
		}
	}	
	
	// 通知main character點選某物件
	private void TellMainCharacterClick( string _ClickObjName )
	{
#if DEBUG			
		Debug.Log( "ClickOnThisCollider::TellMainCharacterClick() _ClickObjName=" + _ClickObjName ) ;
#endif		
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null != controller )
			controller.ClickOnUnit( _ClickObjName ) ;
	}

}

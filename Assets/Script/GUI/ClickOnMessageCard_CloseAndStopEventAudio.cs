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
@file ClickOnMessageCard_CloseAndStopEventAudio.cs
@brief 點擊後關閉並停止音效播放
@author NDark

# 點擊後關閉並停止 UsualEventManagerObject 音效播放
# 掛載在目標事件的牌卡上
# 關閉此GUI物件
# 關閉UsualEventManager物件的音效播放


@date 20121109 by NDark . refine code
@date 20121127 by NDark 
. modify to GetUsualEventManagerObject()
. rename file to ClickOnMessageCard_CloseAndStopEventAudio
@date 20121204 by NDark . comment.
@date 20121209 by NDark . add SetClickOnNoMoveFuncThisFrame() at OnMouseDown()
@date 20121219 by NDark . replace by TellMainCharacterNotToTriggerOtherClick() at OnMouseDown()

*/
using UnityEngine;

public class ClickOnMessageCard_CloseAndStopEventAudio : MonoBehaviour 
{
	public virtual void Click()
	{
		GUITexture tcomponent = this.gameObject.GetComponent<GUITexture>() ;
		if( null != tcomponent )
		{
			tcomponent.enabled = false ;
		}
	
		GameObject usualEventManagerObject = GlobalSingleton.GetUsualEventManagerObject() ;
		if( null == usualEventManagerObject )
		{
			Debug.Log( "ClickOnMessageCard_CloseAndStopEventAudio : null == usualEventManagerObject" ) ;
		}
		else
		{
			usualEventManagerObject.GetComponent<AudioSource>().Stop() ;			
		}
		
		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown()
	{
		Click() ;
	}	
}

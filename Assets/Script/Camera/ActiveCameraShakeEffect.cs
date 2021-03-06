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
@file ActiveCameraShakeEffect.cs
@brief 火花特效 啟動攝影機震動特效 
@author NDark
 
# 裝在Template_Effect_PhaserSparks上.
# 當玩家被擊中，身上有火花時，觸發攝影機震動特效
# 只有火花在主角身上才可震動
# 找到攝影機的 CameraShakeEffect 來啟動震動效果
# 間隔一段時間 不斷觸發
# 目前觸發週期是 0.4s , 一次設定搖晃 0.5s 來達到不間斷的效果
# CheckWinOrLose() 檢查勝利之後不可繼續震動


@date 20121111 file created.
@date 20121112 by NDark 
. add code to check MainCharacter
. fix an error of check parent of sparks
@date 20121203 by NDark
. rename class member m_CountDownTrigger to m_ReActiveCycle
. add class method ShakeCamera()
@date 20121218 by NDark . add class method CheckWinOrLose()
@date 20130111 by NDark . refactor and comment.

*/
using UnityEngine;

public class ActiveCameraShakeEffect : MonoBehaviour 
{
	private bool m_Active = false ; // 不是每個火花都要震動
	private CountDownTrigger m_ReActiveCycle = new CountDownTrigger( 0.4f ); // 重新觸發的週期
	private float m_ShakeElapsedSec = 0.5f ;
	
	// Use this for initialization
	void Start () 
	{
		// 隨著火花物件的產生而發動
		// only happen on MainCharacter
		GameObject parentObj = GlobalSingleton.GetParentObject( this.gameObject ) ;
		m_Active = ( null != parentObj &&
					 parentObj.name == ConstName.MainCharacterObjectName ) ;

		if( true == m_Active )
		{
			ShakeCamera() ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_Active &&
			true == m_ReActiveCycle.IsCountDownToZero() )// 每一個週期檢查一次
		{
			if( true == CheckWinOrLose() )
			{
				m_Active = false ;
				return ;
			}
			
			ShakeCamera() ;
			m_ReActiveCycle.Rewind() ;// 間隔一段時間 不斷觸發
		}
	}
	
	private void ShakeCamera()
	{
		CameraShakeEffect shakeEffect = Camera.main.GetComponent<CameraShakeEffect>() ;
		if( null != shakeEffect )
			shakeEffect.ActiveCameraShakeEffect( true , m_ShakeElapsedSec ) ;
	}
	
	// 檢查勝利之後不可繼續震動
	private bool CheckWinOrLose()
	{
		VictoryEventManager victoryEvent = GlobalSingleton.GetVictoryEventManager() ;
		return ( null != victoryEvent &&
			     true == victoryEvent.IsWinOrLose() ) ;
	}
}

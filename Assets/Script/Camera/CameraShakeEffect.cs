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
@file CameraShakeEffect.cs
@brief 攝影機的振動特效
@author NDark

# 由目前的攝影機再加上一個亂數位移 做出不斷震動的效果
# 必須在玩家活著的時候才啟用
# 震動時間經過後就關閉.
# 掛在主攝影機上
# 目前有兩種方式觸發攝影機的搖晃特效
## 主角船撞到其他物件時
## 主角船掛上火花特效時
# 變數
## 啟動與否 m_Active
# KeepMinimapCameraUnderTheMap() 
因為移動了主攝影機，必須讓小地圖攝影機的位置持續保持在地圖下，
才能看到正確的小地圖。
每次呼叫設定主攝影機的位置 SetPosToCamera() 都必須連帶執行。

@date 20121108 by NDark . refine code.
@date 20121124 by NDark . stop shake in z direction.
@date 20121203 by NDark . comment
@date 20121204 by NDark . 修改歸零軸
@date 20121218 by NDark 
. comment.
. replace code by MathmaticFunc.RandomVector()
@date 20121227 by NDark . add class method SetPosToCamera() to keep MiniMapCamera under the scene.
@date 20130112 by NDark . refactor and comment.

*/
// #define DEBUG_LOG

using UnityEngine;

public class CameraShakeEffect : MonoBehaviour 
{
	private CountDownTrigger m_ShakeCountDown = new CountDownTrigger() ;
	private bool m_Active = false ;// 啟動與否	
	private Vector3 m_CameraAssumePos = Vector3.zero ;	// set by outside
	private Vector3 m_ShakeDisplacement = Vector3.zero ;// from random
	
	// 為了取得UnitData檢查玩家死了沒
	private NamedObject m_MainCharacter = new NamedObject();
	
	// Use this for initialization
	void Start () 
	{
		m_CameraAssumePos = this.gameObject.transform.position ;
		m_Active = false ;
	}
	
	public void ActiveCameraShakeEffect( bool _Active , float _Sec )
	{
#if DEBUG_LOG
		Debug.Log( "CameraShakeEffect::ActiveCameraShakeEffect() _Sec=" + _Sec ) ;
#endif			
		m_Active = _Active ;
		if( true == _Active )
		{
			m_ShakeCountDown.Setup( _Sec ) ;
			m_ShakeCountDown.Rewind() ;
			m_MainCharacter.Obj = GlobalSingleton.GetMainCharacterObj() ;
		}
	}
	
	public void SetCmaeraPos( Vector3 _pos ) 
	{
		m_CameraAssumePos = _pos ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_Active )
		{
			SetPosToCamera( m_CameraAssumePos ) ;// camera is not shaked.
		}
		else 
		{
			if( null != m_MainCharacter.Obj )
			{
				UnitData unitData = m_MainCharacter.Obj.GetComponent<UnitData>() ;
				if( null != unitData &&
					true == unitData.IsAlive() &&
					false == m_ShakeCountDown.IsCountDownToZero() )
				{
					// decide new shake sisplacement
					m_ShakeDisplacement = MathmaticFunc.RandomVector( 2 ) ;
					m_ShakeDisplacement *= BaseDefine.CAMERA_SHAKE_DISTANCE ;
					
					Vector3 cameraRealPos = m_CameraAssumePos + m_ShakeDisplacement ;
					SetPosToCamera( cameraRealPos ) ;
					
					return ;
				}
			}
			
			m_Active = false ;
		}
	}
	
	private void SetPosToCamera( Vector3 _pos )
	{
		this.gameObject.transform.position = _pos ;
#if DEBUG_LOG
		Debug.Log( "CameraShakeEffect::SetPosToCamera() _pos=" + _pos ) ;
#endif		
		KeepMinimapCameraUnderTheMap( _pos ) ;
	}
	
	private void KeepMinimapCameraUnderTheMap( Vector3 _pos )
	{
		Transform trans = this.gameObject.transform.FindChild( "MiniMapCamera" ) ;
		if( null != trans )
		{
			Vector3 pos = new Vector3( _pos.x , _pos.y , _pos.z ) ;
			pos.y = -1 ;// keep minimap camera under the map
			trans.gameObject.transform.position = pos ;
#if DEBUG_LOG			
			Debug.Log( "CameraShakeEffect::KeepMinimapCameraUnderTheMap() pos=" + pos ) ;
#endif			
		}
	}
}

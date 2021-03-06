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
@file GUI_ScaleInTime.cs
@author NDark

# 減緩不代表停止，此時的參數是希望遊戲依然進行，參數請參考 BaseDefine.SLOWMOTION_SCALE_IN_TIME
# GUITexture結束顯示時就會回復，正常運行的時間參數是 1.0 請參考 BaseDefine.NORMAL_SCALE_IN_TIME
# GUITexture顯示時就會設定其減緩。

@date 20121220 file started.
@date 20130113 by NDark . comment.
*/
using UnityEngine;

public class GUI_ScaleInTime : MonoBehaviour 
{
	public enum ScaleInTimeState
	{
		UnActive = 0 ,
		Active ,
		DoingActive ,
		DeActive ,
	}
	
	public float m_ScaleInSlowMotion = BaseDefine.SLOWMOTION_SCALE_IN_TIME ;
	protected StateIndex m_State = new StateIndex() ;
	
	// Use this for initialization
	void Start () 
	{
		SetState( ScaleInTimeState.UnActive ) ;
	}
	
	void OnDestroy()
	{
		Time.timeScale = BaseDefine.NORMAL_SCALE_IN_TIME ;// resume
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( (ScaleInTimeState) m_State.state )
		{
		case ScaleInTimeState.UnActive :
			if( true == this.gameObject.GetComponent<GUITexture>().enabled )
			{
				SetState( ScaleInTimeState.Active ) ;
			}			
			break ;
		case ScaleInTimeState.Active :
			// Debug.Log( "ScaleInTimeState.Active" + Time.realtimeSinceStartup ) ;
			Time.timeScale = m_ScaleInSlowMotion ;
			SetState( ScaleInTimeState.DoingActive ) ;
			break ;
		case ScaleInTimeState.DoingActive :
			if( false == this.gameObject.GetComponent<GUITexture>().enabled )
			{
				SetState( ScaleInTimeState.DeActive ) ; 
			}
			break ;
		case ScaleInTimeState.DeActive :
			// Debug.Log( "ScaleInTimeState.DeActive" + Time.realtimeSinceStartup ) ;
			Time.timeScale = BaseDefine.NORMAL_SCALE_IN_TIME ;// resume
			SetState( ScaleInTimeState.UnActive ) ;
			break ;			
		}

	
	}
	
	protected void SetState( ScaleInTimeState _Set )
	{
		m_State.state = (int) _Set ;
	}
}

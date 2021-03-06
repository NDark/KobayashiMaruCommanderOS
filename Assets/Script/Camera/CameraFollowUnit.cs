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
@file CameraFollowUnit.cs
@author NDark

# 攝影機追蹤一個單位
# 簡化版的 CameraFollowMainCharacter 
# 用在 戰場事件的攝影機上
# m_FollowUnit 追蹤的單位
# Setup() 時傳入單位並啟動
# Close() 時關閉
# UpdateCameraPosition() 更新攝影機位置

@date 20130120 file started.
@date 20130204 by NDark . commenet.

*/
using UnityEngine;

public class CameraFollowUnit : MonoBehaviour 
{
	// 與玩家的標準距離,取自 CameraInitialization
	private Vector3 m_DistanceVecFromObjectStandard = Vector3.zero ;
	private NamedObject m_FollowUnit = new NamedObject() ;
	private bool m_Active = false ;
	
	public void Setup( NamedObject _FollowUnit )
	{
		m_FollowUnit = new NamedObject( _FollowUnit ) ;	
		m_Active = true ;
	}
	
	public void Close()
	{
		m_FollowUnit.Clear() ;
		m_Active = false ;
	}	
	
	// Use this for initialization
	void Start () 
	{
		CameraInitialization camInitScript = this.gameObject.GetComponent<CameraInitialization>() ;
		if( null != camInitScript )
		{
			m_DistanceVecFromObjectStandard = camInitScript.InitializationCameraPosition ;
		}		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_Active )
			return ;
		
		if( null == m_FollowUnit.Obj )
		{
			// Debug.Log( "null == m_FollowUnit" ) ;
			return ;
		}		
		
		UpdateCameraPosition();	
	}
	
	private void UpdateCameraPosition()
	{
		// set camera position by shake effect
		Vector3 OrgPos = m_FollowUnit.Obj.transform.position ;
		Vector3 CamPosItShouldBe = OrgPos + m_DistanceVecFromObjectStandard ; 
		this.gameObject.transform.position = CamPosItShouldBe ;
	}
}

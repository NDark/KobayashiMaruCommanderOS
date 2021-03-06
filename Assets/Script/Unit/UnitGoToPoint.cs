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
@file UnitGoToPoint.cs
@brief 自動駕駛系統
@author NDark
 
# 掛載在單位上
# 朝向某個地點持續前進
# Setup() 啟動時傳入目標位置 以及最大持續時間
# Stop() 停止自動控制
# IsActiveAutoPilot() 檢查自動控制是否運作中
# ChangeImpuseEngineRatio() 依照距離來加減速 傳回true表示停止
# KeepDestinationAtScreen() 保持目標一直在螢幕點
# CheckOutOfSpace() 檢查目標點是否超過關卡範圍，並顯示靜止貼圖。

@date 20121124 by NDark . fix an error of impulse engine string.
...
@date 20130113 by NDark . comment.
@date 20130118 by NDark . fix an error of not set position of the destination object before show it at Setup()
@date 20130213 by NDark . add null checking at CheckOutOfSpace()

*/
using UnityEngine;

public class UnitGoToPoint : MonoBehaviour 
{
	Vector3 m_DestinationPoint = Vector3.zero ;// 目標點
	
	TimeTrigger m_StopGotoTimer = new TimeTrigger() ; // 最大持續時間,超過時間自動停止
	float m_MaxDistanceNormalized = 10.0f ;	// 超過此距離就是 極速 ( > 1.0 )
	float m_MinDistance = 3.0f ;	// 小於此距離就是 0
	
	bool m_IsRightClickToMove = false ; // tutorial

	NamedObject m_DestinationObj = new NamedObject( "GUI_Destination" ) ;// 顯示目標點的GUI
	RectTransform m_DestinationRect = null ;
	UnityEngine.UI.Image m_DestinationImage = null ;
	Sprite m_DestinationTexture = null ;
	Texture m_ForbidenTexture = null ;

	// 啟動時傳入目標位置 以及最大持續時間
	public void Setup( Vector3 _DestinationPosition , 
					   float _MaximumElapsedSec = 10.0f )
	{
		Vector3 posNow = this.gameObject.transform.position ;
		_DestinationPosition.y = posNow.y ;// y direction must be the same as ship.
		m_DestinationPoint = _DestinationPosition ;
	
		CheckOutOfSpace( _DestinationPosition ) ;

		KeepDestinationAtScreen( m_DestinationPoint ) ;
		ShowUGUI.Show( m_DestinationObj.Obj , true , false , false ) ;
		
		m_StopGotoTimer.Setup( 0 , _MaximumElapsedSec ) ;
		m_StopGotoTimer.Active() ;
	}
	
	public void Stop()
	{
		ShowUGUI.Show( m_DestinationObj.Obj , false , false , false ) ;
		m_StopGotoTimer.Close() ;
	}
	
	public bool IsActiveAutoPilot()
	{
		return m_StopGotoTimer.IsActive() ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_ForbidenTexture = null ;		
		if( null != m_DestinationObj.Obj )
		{
			m_DestinationImage = m_DestinationObj.Obj.GetComponent<UnityEngine.UI.Image>() ;
			m_DestinationRect =m_DestinationObj.Obj.GetComponent<RectTransform>() ;
			m_DestinationTexture = m_DestinationImage.sprite ;
			
		}
		m_StopGotoTimer.Close() ;
	}
		
	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;


		
		// check time
		if( true == m_StopGotoTimer.IsAboutToClose( true ) )
		{
			// Debug.Log( "m_StopGotoTimer.IsAboutToClose" ) ;				
		}		
		else if( true == m_StopGotoTimer.IsActive() )
		{
			KeepDestinationAtScreen( m_DestinationPoint ) ;
			
			Vector3 ToDestination = m_DestinationPoint - this.gameObject.transform.position ;			
			if( true == ChangeImpuseEngineRatio( unitData , ToDestination.magnitude ) )
			{
				unitData.AllStop() ;
				Stop() ;
			}
			else
			{
				// keep going
				Vector3 dirNow = this.gameObject.transform.forward ;
				ToDestination.Normalize() ;
				float AngleOfTarget = Vector3.Angle( dirNow , ToDestination ) ;
				// Debug.Log( "AngleOfTarget" + AngleOfTarget ) ;
				float DotOfUp = 0.0f ;	
				if( true == MathmaticFunc.FindDotofCrossAndUp( ToDestination , 
															   dirNow , 
															   this.gameObject.transform.up , 
															   ref DotOfUp ) )
				{
					unitData.AngularRatioHeadTo( AngleOfTarget , 
												 DotOfUp , 
												 0.1f ) ;
				}
			}
		}
	
	}
	
	// 檢查教學
	private void ConfirmTutorialRightClickToMove()
	{
		if( false == m_IsRightClickToMove )
		{
			m_IsRightClickToMove = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsRightClickToMove = m_IsRightClickToMove ;
		}	
	}	
	
	// 依照距離來加減速 傳回true表示停止
	protected bool ChangeImpuseEngineRatio( UnitData _unitData , float _Distance )
	{
		if( null == _unitData )
			return false ;
		
		float ratio = 0.0f ;
		string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
		if( true == _unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) )
		{
			if( _Distance >= m_MinDistance )
			{
				ratio = _Distance / m_MaxDistanceNormalized ;
				
				// check tutorial right click to move
				ConfirmTutorialRightClickToMove() ;			
			}
			_unitData.standardParameters[ IMPULSE_ENGINE_RATIO ].now = ratio ;
		}
		
		// close target
		if( ratio <= 1.0f )
		{
			ShowUGUI.Show( m_DestinationObj.Obj , false , false , false ) ;
		}
		
		// Debug.Log( ratio ) ;
		return ( ratio == 0.0f ) ;// is stop ?
	}	
	
	// Keep destination object at screen position by world position
	private void KeepDestinationAtScreen( Vector3 _WorldPos )
	{
		if( null != m_DestinationObj.Obj )
		{
			// keep destinatio update on screen.
			Vector3 screenPos = Camera.main.WorldToScreenPoint( _WorldPos ) ;			
			m_DestinationRect.anchoredPosition = screenPos ;
		}
		
	}
	
	private void CheckOutOfSpace( Vector3 _DestinationPosition )
	{
		// check out of level space
		bool outofSpace = false ;

		BackgroundObjInitialization bObjectScript = GlobalSingleton.GetBackgroundInit() ;
		if( null != bObjectScript )
			outofSpace = bObjectScript.IsOutofLevel( _DestinationPosition ) ;

		if( outofSpace )
		{
			// change texture
			if( null == m_ForbidenTexture )
				m_ForbidenTexture = ResourceLoad.LoadTexture( "GUI_Destination_Forbidden" ) ;
			
			if( null != m_ForbidenTexture )
			{
				if( null != m_DestinationImage )
				{
					Rect rect = new Rect( 0 , 0 , m_ForbidenTexture.width , m_ForbidenTexture.height ) ;
					Vector2 pivot = new Vector2( 0.5f , 0.5f ) ;
					m_DestinationImage.sprite = Sprite.Create( m_ForbidenTexture as Texture2D , rect , pivot ) ;
				}
			}
		}
		else
		{
			if( null != m_DestinationImage )
			{
				m_DestinationImage.sprite = m_DestinationTexture ;
			}
		}
		
	}
}

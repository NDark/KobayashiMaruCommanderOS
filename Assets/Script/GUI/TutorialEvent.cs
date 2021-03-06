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
@file TutorialEvent.cs
@brief 教學事件 
@author NDark

# 透過關卡檔把script掛在場景上，只有教學關才掛。
所以其他關卡如果去access就找不到此物件
# 目前有 
點選 
控制面版 
移動
# 每個教學都用 TimeTrigger 來控制教學的開啟與關閉
# 各項運作 會去存取此script 來達到強制關閉教學的功能
# 各事件的GUI對象也都是寫死是先創造好(非動態生成)

@date 20121201 by NDark . re-factor tutorial
@date 20121205 by NDark . comment.
@date 20121224 by NDark . fix an error of not initialize() of m_MessageCard_Tutorial_Guide01Trigger
@date 20130124 by NDark . add class method CloseAll()

*/
using UnityEngine;
using System.Collections;

public class TutorialEvent : MonoBehaviour 
{
	public bool m_IsPressedUpAndDown = false ;
	public bool m_IsPressedLeftAndRight = false ;
	public bool m_IsRightClickToMove = false ;
	
	public bool m_IsPressedSelectEnemy = false ;
	
	public bool m_IsActiveControlPanelFunction = false ;
	
	public bool m_IsPressedFirePhaser = false ;
	public bool m_IsPressedFireTorpedo = false ;
	public bool m_IsPressedTrackorBeam = false ;
	
	// 點選
	public TimeTrigger m_MessageCard_Tutorial_SelectUnitTrigger = new TimeTrigger( 3.0f , 6.0f ) ;
	public string m_MessageCard_Tutorial_SelectUnitName = "MessageCard_Tutorial_SelectUnit" ;
	
	// 控制面板
	public TimeTrigger m_MessageCard_Tutorial_ActiveControlPanel01Trigger = new TimeTrigger( 9.0f , 4.0f ) ;
	public string m_MessageCard_Tutorial_ActiveControlPanel01Name = "MessageCard_Tutorial_ActiveControlPanel01" ;
	public TimeTrigger m_MessageCard_Tutorial_ActiveControlPanel02Trigger = new TimeTrigger( 14.0f , 11.0f ) ;
	public string m_MessageCard_Tutorial_ActiveControlPanel02Name = "MessageCard_Tutorial_ActiveControlPanel02" ;	
	public TimeTrigger m_MessageCard_Tutorial_ActiveControlPanel04Trigger = new TimeTrigger( 26.0f , 7.0f ) ;
	public string m_MessageCard_Tutorial_ActiveControlPanel04Name = "MessageCard_Tutorial_ActiveControlPanel04" ;	
	
	// 移動教學
	public TimeTrigger m_MessageCard_Tutorial_MoveTrigger = new TimeTrigger( 35.0f , 7.0f ) ;
	public string m_MessageCard_Tutorial_MoveObjName = "MessageCard_Tutorial_Move" ;	
	
	// 環境教學
	public TimeTrigger m_MessageCard_Tutorial_Guide01Trigger = new TimeTrigger( 43.0f , 7.0f ) ;
	public string m_MessageCard_Tutorial_Guide01Name = "MessageCard_Tutorial_Guide01" ;		

	// 攻擊教學
	public TimeTrigger m_MessageCard_Tutorial_DstroyMeteorsTrigger = new TimeTrigger( 51.0f , 3.0f ) ;
	public string m_MessageCard_Tutorial_DstroyMeteorsName = "MessageCard_Tutorial_DestroyMeteors" ;		
	
	public void CloseAll()
	{
		ShowUGUI.Show( m_MessageCard_Tutorial_SelectUnitName , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_ActiveControlPanel01Name , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_ActiveControlPanel02Name , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_ActiveControlPanel04Name , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_MoveObjName , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_Guide01Name , false , true , true ) ;
		ShowUGUI.Show( m_MessageCard_Tutorial_DstroyMeteorsName , false , true , true ) ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_MessageCard_Tutorial_SelectUnitTrigger.Initialize() ;
		
		m_MessageCard_Tutorial_ActiveControlPanel01Trigger.Initialize() ;
		m_MessageCard_Tutorial_ActiveControlPanel02Trigger.Initialize() ;
		m_MessageCard_Tutorial_ActiveControlPanel04Trigger.Initialize() ;
		
		m_MessageCard_Tutorial_MoveTrigger.Initialize() ;
		m_MessageCard_Tutorial_Guide01Trigger.Initialize() ;
		m_MessageCard_Tutorial_DstroyMeteorsTrigger.Initialize() ;
	}
	
	// 依據條件來更新教學物件的顯示狀態
	public void UpdateTimeTrigger( ref TimeTrigger _TimeTrigger , 
								   bool _SpecialCondition , 
								   string _MessageCardObjName )
	{
		if( true == _TimeTrigger.IsClosed() )
			return ;
		
		if( true == _SpecialCondition &&
			true == _TimeTrigger.IsReady() )
		{
			_TimeTrigger.Close() ;// just close.
		}
		else if( true == _TimeTrigger.IsAboutToStart( true ) )
		{
			ShowUGUI.Show( _MessageCardObjName , true , true  , true ) ;
		}
		else if( true == _TimeTrigger.IsActive() &&
			     ( true == _SpecialCondition ||
		 	       true == _TimeTrigger.IsTimeEnded() ) )
		{
			ShowUGUI.Show( _MessageCardObjName , false , true , true ) ;
			_TimeTrigger.Close() ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_SelectUnitTrigger , 
						   true == m_IsPressedSelectEnemy , 
						   m_MessageCard_Tutorial_SelectUnitName ) ;
		
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_ActiveControlPanel01Trigger , 
						   true == m_IsActiveControlPanelFunction , 
						   m_MessageCard_Tutorial_ActiveControlPanel01Name ) ;
		
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_ActiveControlPanel02Trigger , 
						   true == m_IsActiveControlPanelFunction ,
						   m_MessageCard_Tutorial_ActiveControlPanel02Name ) ;	
		
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_ActiveControlPanel04Trigger , 
						   true == m_IsPressedFirePhaser ||
						   true == m_IsPressedFireTorpedo ||
						   true == m_IsPressedTrackorBeam , 
						   m_MessageCard_Tutorial_ActiveControlPanel04Name ) ;			
		
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_MoveTrigger , 
						   true == m_IsRightClickToMove ||
						  ( true == m_IsPressedUpAndDown && true == m_IsPressedLeftAndRight ), 
						   m_MessageCard_Tutorial_MoveObjName ) ;

		UpdateTimeTrigger( ref m_MessageCard_Tutorial_Guide01Trigger , 
						   false , 
						   m_MessageCard_Tutorial_Guide01Name ) ;	
		
		UpdateTimeTrigger( ref m_MessageCard_Tutorial_DstroyMeteorsTrigger , 
						   false , 
						   m_MessageCard_Tutorial_DstroyMeteorsName ) ;			
	}
}

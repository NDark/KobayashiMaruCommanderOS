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
@file CountDownTimeManager.cs
@author NDark

# 預設沒有掛上物件需要特別呼叫掛上
# Setup() 設定倒數的時間
# 倒數結束會停留在0
# 顯示時使用 00:00 的格式
# 使用的GUI物件 GUI_CountDownTimeText

@date 20121224 by NDark
@date 20130113 by NDark . comment.

*/
using UnityEngine;

public class CountDownTimeManager : MonoBehaviour 
{
	private CountDownTrigger m_Timer = new CountDownTrigger() ;
	private bool m_IsActive = false ;
	private float m_TotalTime = 0.0f ;
	private NamedObject m_GuiTextObj = new NamedObject( "GUI_CountDownTimeText" ) ;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	public void Setup( float _TotalTime )
	{
		m_IsActive = true ;
		m_TotalTime = _TotalTime ;
		m_Timer.Setup( m_TotalTime ) ;
		m_Timer.Rewind() ;
		ShowUGUI.Show( m_GuiTextObj.Obj , true , true , false ) ;
	}
	
	public void Stop()
	{
		m_IsActive = false ;
		ShowUGUI.Show( m_GuiTextObj.Obj , false , true , false ) ;
	}
	
	public bool IsCountDownToZero()
	{
		return m_Timer.IsCountDownToZero() ;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_IsActive &&
			null != m_GuiTextObj.Obj )
		{
			if( null == m_Text )
			{
				m_Text = m_GuiTextObj.Obj.GetComponent<UnityEngine.UI.Text>() ; 
			}
			
			
			if( null != m_Text )
			{
				if( true == m_Timer.IsCountDownToZero() )
				{
					m_Text.text = "00:00" ;
				}
				else
				{
					UpdateDisplayText( m_Text ) ;
				}
			}
		}
	}
	
	private void UpdateDisplayText( UnityEngine.UI.Text _guiText )
	{
		string minStr = "" ;
		string secStr = "" ;						 
		float remainingSec = m_TotalTime - m_Timer.m_State.ElapsedFromLast() ;
		minStr = string.Format( "{0:00}" , Mathf.Floor( remainingSec / 60 ) ) ;
		secStr = string.Format( "{0:00}" , Mathf.Floor( remainingSec % 60 ) ) ;
		_guiText.text = minStr + ":" + secStr ;		
	}
	
	UnityEngine.UI.Text m_Text = null ;
}

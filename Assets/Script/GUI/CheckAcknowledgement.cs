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
@file CheckAcknowledgement.cs
@author NDark

# 自GlobalSingleton.m_AcknowledgementGUIOBjectName取得參數播放該物件
# 結束時進入指定關卡
# 有播放才顯示SKIP按鈕

@date 20121226 file started.
@date 20130112 . remove class member m_WaitInSec

*/
using UnityEngine;

public class CheckAcknowledgement : MonoBehaviour 
{
	public CountDownTrigger m_Timer = new CountDownTrigger( 3.0f ) ;		// 等待時間
	public string m_LevelString = "" ;		// 指定關卡
	
	private NamedObject m_GUI_Skip = new NamedObject( ConstName.GUI_AcknowledgeScene_Skip ) ;
	
	// Use this for initialization
	void Start () 
	{
		// 檢查是否要播放Ackonwlegement
		// GlobalSingleton.m_AcknowledgementGUIOBjectName = "MessageCard_Acknowledgement:Level03_EscoreVessel" ;
	
		if( 0 != GlobalSingleton.m_AcknowledgementGUIOBjectName.Length )
		{
			
			ShowUGUI.Show( GlobalSingleton.m_AcknowledgementGUIOBjectName , true , true , true ) ;
			m_Timer.Rewind() ;
			
			ShowUGUI.Show( m_GUI_Skip.Obj , true , true , false ) ;
			
			// clear 
			GlobalSingleton.m_AcknowledgementGUIOBjectName = "" ;			
		}
		else
		{
			if( 0 != m_LevelString.Length )
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene( m_LevelString ) ;
			}						
		}
		

	}
	
	// Update is called once per frame
	void Update () 
	{
		if( 0 != m_LevelString.Length &&
			true == m_Timer.IsCountDownToZero() )
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene( m_LevelString ) ;
		}
	}
}

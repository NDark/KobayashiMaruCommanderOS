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
@file SystemLogManager.cs
@author NDark
 
# 記錄系統發生的訊息.必要時可以還原
# 目前尚未完成
# 不掛在物件上,所以必須由其他script來驅動.
# 註解型態為 [時間戳記]:[類型]:[內容]
# m_Active 是否啟動
# m_KeepWrite 是否持續寫入
# m_StartTime 開啟時間
# m_Logs 目前(未寫出)記錄
# m_SW 寫出的StreamWriter
# AddLog() 新增紀錄
# Initialize() 初始化
## 清除Log
## 重置時間
## 開啟檔案
# ExportToFile() 寫入檔案

@date 20121219 file started.
@date 20121219 by NDark 
. add class member m_Active
. add Delete() at Initialize()
. add enum element ClickOnUnit , FireWeapon of SysLogType
@date 20121220 by NDark . fix an error of no open file at non-auto play mode at ExportToFile()
@date 20121221 by NDark . fix an error of add a file when non-active.
@date 20130112 by NDark . comment.

*/
using UnityEngine;
using System.IO ;
using System.Collections.Generic;

public static class SystemLogManager
{
	public enum SysLogType
	{
		Log = 0 ,
		Control ,
		ClickOnUnit ,
		FireWeapon ,
		Damage ,		
	}
	
	public static bool m_Active = false ;
	public static bool m_KeepWrite = true ;
	public static float m_StartTime = 0 ;
	public static List<string> m_Logs = new List<string>() ;
	static StreamWriter m_SW = null ;
	
	public static void AddLog( SysLogType _type , string _AddLog )
	{
		if( false == m_Active )
			return ;
		
		string fullLogString = Time.timeSinceLevelLoad.ToString() + ":" 
			+ _type.ToString() + ":"
			+ _AddLog ;
		
		if( true == m_KeepWrite )
		{
			
			if( null != m_SW )
			{
				// Debug.Log( "m_SW.WriteLine( fullLogString ) ;" + fullLogString ) ;
				m_SW.WriteLine( fullLogString ) ;
				
			}			
		}
		else
			m_Logs.Add( fullLogString ) ;		
	}
	
	public static void Initialize()
	{
		if( false == m_Active )
			return ;
		
		m_Logs.Clear() ;
		m_StartTime = Time.timeSinceLevelLoad ;
		
		if( true == m_KeepWrite )
		{
			System.IO.File.Delete( "SystemLogKeep.txt" ) ;
			m_SW = new StreamWriter( "SystemLogKeep.txt" , true ) ;
		}
	}
	
	public static void ExportToFile()
	{
		if( false == m_Active )
			return ;
		
		if( true == m_KeepWrite )
		{
			if( null != m_SW )
				m_SW.Close() ;
		}
		else
		{
			StreamWriter SW = new StreamWriter( "SystemLog.txt" ) ;
			if( null != SW )
			{
				List<string>.Enumerator e = m_Logs.GetEnumerator() ;
				while( e.MoveNext() )
				{
					SW.WriteLine( e.Current ) ;
				}
				SW.Close() ;
			}
		}
	}
}

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
@file ComponentParam.cs
@brief 部件基本參數
@author NDark

# 定義在參數檔 ComponentParamTable.xml 中

@date 20121110 by NDark . add copy constructor.
@date 20121111 by NDark . add class member m_Component3DObject
@date 20121203 by NDark . comment
@date 20121211 by NDark . add class member m_Effect3DObjectTemplateName
@date 20121218 by NDark . refactor

*/
using UnityEngine;

/* 
# GUI參數 GUIParam
## GUIRect 位置定義
## DisplayName 顯示名稱
 */
[System.Serializable]
public class GUIParam
{
	public string DisplayName = "" ;	
	public Rect GUIRect = new Rect( 0 , 0 , 0 , 0 ) ;
	
	public GUIParam()
	{
	}
	public GUIParam( GUIParam _src )
	{
		DisplayName = _src.DisplayName ;
		GUIRect = new Rect( _src.GUIRect ) ;
	}
	public GUIParam( string _DisplayName , Rect _Rect )
	{
		DisplayName = _DisplayName ;
		GUIRect = new Rect( _Rect ) ;
	}	
}

/*
部件基本參數
# 定義在參數檔 ComponentParamTable.xml 中
# m_ComponentName 部件名稱
# m_Effect3DObjectTemplateName 指定要使用的特效樣板名稱
# m_Component3DObject 部件3D物件
# m_GUI : GUI參數 GUIParam
# m_IsFlip 是否要反向,會影響到傳回的 GUIRect()
 */
[System.Serializable]
public class ComponentParam 
{
	public string m_ComponentName = "" ;
	public string m_Effect3DObjectTemplateName = "" ;
	public NamedObject m_Component3DObject = new NamedObject() ;
	public bool m_IsFlip = false ;
	
	public string GUIDisplayName 
	{
		set
		{
			m_GUI.DisplayName = value ;
		}
		get
		{
			return m_GUI.DisplayName ;
		}
	}
	
	public Rect GUIRect
	{
		set 
		{
			m_GUI.GUIRect = value ;
		}
		get
		{
			Rect ret = new Rect( m_GUI.GUIRect ) ;
			if( true == m_IsFlip )
			{
				ret.x = ret.width - ret.x ;
				ret.y = ret.height - ret.y ;
			}
			return ret ;
		}
	}
	

	public ComponentParam()
	{
	}
	
	public ComponentParam( ComponentParam _src )
	{
		m_ComponentName = _src.m_ComponentName ;
		m_Effect3DObjectTemplateName = _src.m_Effect3DObjectTemplateName ;
		m_GUI = new GUIParam( _src.m_GUI ) ;
		m_IsFlip = _src.m_IsFlip ;
		m_Component3DObject.Setup( _src.m_Component3DObject ) ;
	}
	
	private GUIParam m_GUI = new GUIParam() ;
	
}

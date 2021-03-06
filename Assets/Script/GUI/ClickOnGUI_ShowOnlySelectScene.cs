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
@file ClickOnGUI_ShowOnlySelectScene.cs
@author NDark

點擊顯示對應關卡圖

# 顯示點選的紅點及正確位置
# 因為必須切換顯示對應關卡所以必須先關閉所有的關卡
# 只開啟對應的關卡
# 依照本物件的字串組合出關卡的物件字串
string GameObjectName = "GUI_SelectScene:" + levelString ;

@date 20121226 file started.
@date 20130111 by NDark . remove class method LevelString()
*/
using UnityEngine;

public class ClickOnGUI_ShowOnlySelectScene : MonoBehaviour 
{
	
	protected NamedObject m_SelectScenes = new NamedObject( "GUI_SelectScenes" ) ;// 關卡場景的parent
	protected NamedObject m_ChooseRedCircle = new NamedObject( "GUI_ChooseSceneRedCircle" ) ;// 選擇紅點
	
	public virtual void ShowOnlySelectScene()
	{
		// 關閉全部的SelectSceneGUI
		ShowGUITexture.Show( m_SelectScenes.Obj , false , true , true ) ;
		
		// 只啟動對應的SelectSceneGUI
		string levelString = ConstName.GetSplitVecConetent( this.gameObject.name , 1 ) ;
		if( 0 != levelString.Length )
		{
			string GameObjectName = "GUI_SelectScene:" + levelString ;
			// Debug.Log( GameObjectName ) ;
			Transform levelTrans = m_SelectScenes.Obj.transform.FindChild( GameObjectName ) ;
			if( null != levelTrans )
				ShowGUITexture.Show( levelTrans.gameObject , true , true , true ) ;
			
			if( null != m_ChooseRedCircle.Obj )
			{
				ShowGUITexture.Show( m_ChooseRedCircle.Obj , true , true , true ) ;
				// 正確位置
				Vector3 pos = this.gameObject.transform.position ;
				pos.z += 1 ;
				m_ChooseRedCircle.Obj.transform.position = pos ;
			}			
		}
	}
	
	void OnMouseDown()
	{
		ShowOnlySelectScene() ;
	}
	
}

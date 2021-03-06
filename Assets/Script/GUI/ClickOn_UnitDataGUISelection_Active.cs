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
@file ClickOn_UnitDataGUISelection_Active.cs
@brief 點擊 GUI_UnitDataSelection_Active 的反應
@author NDark

# 點擊 GUI_UnitDataSelection_Active 的反應
# 清除主角的部位點選
# 關閉自己

@date 20121205 file started.
@date 20121205 by NDark 
. add code of tell main character do not move by mouse click
. add code of ClearUnitComponent() at OnMouseDown()
@date 20130111 by NDark . refactor and comment.

*/
using UnityEngine;

public class ClickOn_UnitDataGUISelection_Active : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
		MainCharacterController controller = mainChar.GetComponent<MainCharacterController>() ;
		if( null != controller )
			controller.SetClickOnNoMoveFuncThisFrame( true ) ;
		
		// 清除主角的部位點選		
		UnitSelectionSystem selectSys = mainChar.GetComponent<UnitSelectionSystem>() ;
		if( null != selectSys )
			selectSys.ClearUnitComponent() ;
		
		// 關閉自己
		this.GetComponent<GUITexture>().enabled = false ;			
	}
}

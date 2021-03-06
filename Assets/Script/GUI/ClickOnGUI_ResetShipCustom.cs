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
@file ClickOnGUI_ResetShipCustom.cs
@author NDark

點擊時重置客製化主角船資訊

# m_ClickReset 是否重置(預設為true)
# 重置就會清空 GlobalSingleton.m_CustomActive 為false(使用關卡設定的主角船)
# 如果不重置,就會直接設定GlobalSingleton.m_CustomActive 為真(確保挑戰關卡正確創造主角船)


@date 20130204 file started.
@date 20130213 by NDark 
. add class member m_ClickReset
. add code of GlobalSingleton.m_CustomActive at OnMouseDown()

*/
using UnityEngine;

public class ClickOnGUI_ResetShipCustom : MonoBehaviour 
{
	public bool m_ClickReset = true ;

	public void ResetShipCustom()
	{
		if( true == m_ClickReset )
		{
			// do not use custom
			// Debug.Log( "GlobalSingleton.ResetCustomData() ;" ) ;
			GlobalSingleton.ResetCustomData() ;
		}
		else
		{
			// custom
			GlobalSingleton.m_CustomActive = true ;
		}
	}
	
	void OnMouseDown()
	{
		ResetShipCustom() ;
	}	
}

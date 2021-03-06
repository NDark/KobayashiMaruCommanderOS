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
@file WeaponParam.cs
@brief 武器參數
@authro NDark 

# m_ComponentName 武器部件名稱
# m_Angle 武器可發射角度
# m_Range 武器可發射距離
# m_Accuracy 武器準確度
# m_FireAudioName 武器發射音效名稱

@date 20121110 . file created.
. add class member m_Angle.
. add class member m_Range.
@date 20121111 by NDark . add class member m_FireAudioName
@date 20121203 by NDark . comment.
@date 20130119 by NDark . add class member m_Accuracy of WeaponParam
@date 20130205 by NDark . comment.

*/
using UnityEngine;

[System.Serializable]
public class WeaponParam 
{
	public string m_ComponentName = "" ; // 武器部件名稱
	public float m_Angle = 0.0f ;		// 武器可發射角度
	public float m_Range = 0.0f ;		// 武器可發射距離
	public float m_Accuracy = 1.0f ;		// 武器準確度
	public string m_FireAudioName = "" ;	// 武器發射音效名稱
	
	public WeaponParam()
	{
		
	}
	public WeaponParam( WeaponParam _src )
	{
		m_ComponentName = _src.m_ComponentName ;
		m_Angle = _src.m_Angle ;
		m_Range = _src.m_Range ;
		m_Accuracy = _src.m_Accuracy ;
		m_FireAudioName = _src.m_FireAudioName ;
	}	
}

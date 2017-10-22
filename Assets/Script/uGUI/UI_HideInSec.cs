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
@file UI_HideInSec.cs
@brief 一定時間後隱藏GUITexture
@author NDark
 
@date 20170624 file started.

*/
using UnityEngine;

public class UI_HideInSec : GUI_HideInSec 
{

	// Update is called once per frame
	void Update () 
	{
		if( null == m_Image )
		{
			m_Image = this.gameObject.GetComponent<UnityEngine.UI.Image>() ;
		}
		
		if( null == m_RawImage )
		{
			m_RawImage = this.gameObject.GetComponent<UnityEngine.UI.RawImage>() ;
		}
		
		if( false == m_Show )
		{
			if( IsEnable() == true )
			{
				m_Timer.Rewind() ;// start counting.
				m_Show = true ;
			}
		}
		else
		{
			if( true == m_Timer.IsCountDownToZero() )
			{
				SetEnable( false ) ;
				m_Show = false ;
			}
		}
	}
	
	bool IsEnable()
	{
		bool ret = false ;
		if( null != m_Image )
		{
			ret = m_Image.enabled ;
		}
		if( null != m_RawImage )
		{
			ret = m_RawImage.enabled ;
		}
		return ret ;
	}
	
	
	void SetEnable( bool _Set )
	{
		if( null != m_Image )
		{
			m_Image.enabled = _Set ;
		}
		if( null != m_RawImage )
		{
			m_RawImage.enabled = _Set ;
		}
	}
	UnityEngine.UI.Image m_Image = null ;
	UnityEngine.UI.RawImage m_RawImage = null ;
	
}

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
@file GUI_RetrieveTextAtEnable.cs
@author NDark

顯示時取得文字

# 會依據指定的索引值向 StrsManager 取得文字內容
# 每次顯示時都會檢查一次

@date 20130122 file started.
@date 20130123 by NDark . add class member m_TextColor.
*/
using UnityEngine;
using System.Collections;

public class GUI_RetrieveTextAtEnable : MonoBehaviour 
{
	public int m_TextIndex = -1 ;
	public Color m_TextColor = Color.black;
	
	protected bool m_Show = false ;
	private GUIText m_GUIText = null ;
	
	public virtual void RetrieveText()
	{
		if( -1 == m_TextIndex ||
			null == m_GUIText )
			return ;
		
		m_GUIText.text = StrsManager.Get( m_TextIndex ) ;
		
	}
	
	// Use this for initialization
	void Start () 
	{
		m_GUIText = this.gameObject.GetComponent<GUIText>() ;
		if( null != m_GUIText )
			m_GUIText.material.color = m_TextColor ;
		
		StrsManager.Register( this.gameObject ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == m_GUIText )
			return ;
		
		if( false == m_Show )
		{
			if( true == m_GUIText.enabled )
			{
				RetrieveText() ;
				m_Show = true ;
			}
		}
		else
		{
			if( false == m_GUIText.enabled )
			{
				m_Show = false ;
			}
		}		
	
	}
}

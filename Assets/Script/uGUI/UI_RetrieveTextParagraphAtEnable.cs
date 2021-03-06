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
@file UI_RetrieveTextParagraphAtEnable.cs
@author NDark 
@date 20170622 file started

*/
using UnityEngine;

public class UI_RetrieveTextParagraphAtEnable : GUI_RetrieveTextParagraphAtEnable 
{
	public bool m_IsShowEmptyIndex = false ;
	public override void RetrieveText()
	{
		if( null == m_UITextParagraph )
			return ;
		
		if( m_UITextParagraph.m_StrArray.Length != m_TextIndice.Length )
		{
			m_UITextParagraph.m_StrArray = new string[ m_TextIndice.Length ] ;
		}
		
		for( int i = 0 ; i < m_TextIndice.Length && 
		    i < m_UITextParagraph.m_StrArray.Length ; ++i )
		{
			m_UITextParagraph.m_StrArray[ i ] = StrsManager.Get( m_TextIndice[ i ] ) ;
			if( true == m_IsShowEmptyIndex && 
			0 == m_UITextParagraph.m_StrArray[ i ].Length )
			{
				m_UITextParagraph.m_StrArray[ i ] = m_TextIndice[ i ].ToString() ;
			}
		}
		m_UITextParagraph.m_TextColor = m_TextColor ;
		m_UITextParagraph.CreateParagraph() ;
	}	
	
	// Use this for initialization
	void Start () 
	{
		
		m_UITextParagraph = this.gameObject.GetComponentInChildren<UITextParagraph>() ;
		RetrieveText() ;
		StrsManager.Register( this.gameObject ) ;
	}
	
	
	protected override bool IsEnable()
	{
		m_Texts = this.gameObject.GetComponentsInChildren<UnityEngine.UI.Text>() ;
		bool ret = false ;
		for( int i = 0 ; i < m_Texts.Length ; ++i )
		{
			// Debug.Log( "m_GUITexts[ i ].text=" + m_GUITexts[ i ].text ) ;
			if( m_Texts[ i ].text != "Test" )
			{
				ret = m_Texts[ i ].enabled ;
				break ;
			}
		}
		// Debug.Log( "IsEnable=" + ret ) ;
		return ret ;
	}
	
	private UnityEngine.UI.Text [] m_Texts = null ;
	private UITextParagraph m_UITextParagraph = null ;
}

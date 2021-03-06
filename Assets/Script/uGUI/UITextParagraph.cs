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
@file UITextParagraph.cs
@author NDark 

a copy of GUI_TextParagraph to affect UnityEngine.UI.Text
 
@date 20170620 file started.

*/
using UnityEngine;
using System.Collections.Generic;

public class UITextParagraph : GUI_TextParagraph 
{
	
	public override void CreateParagraph() 
	{
		// Debug.Log( "CreateParagraph" ) ;

		UnityEngine.UI.Text firstGUIText = this.GetComponent<UnityEngine.UI.Text>();
		if( null == firstGUIText )
			return; 
		
		
		foreach( var obj in m_ChildList )
		{
			GameObject.Destroy( obj ) ;
		}
		
		RectTransform firstRect = firstGUIText.gameObject.GetComponent<RectTransform>() ;
		
		Vector2 tempPos = Vector2.zero ;
		for( int i = 0 ; i < m_StrArray.Length ; ++i )
		{
			// for other paragraph
			GameObject obj = new GameObject( "Paragraph" + i ) ;
			obj.transform.SetParent( this.gameObject.transform ) ;
			
			UnityEngine.UI.Text text = obj.AddComponent<UnityEngine.UI.Text>() ;
			text.enabled = firstGUIText.enabled = true;
			
			RectTransform rect = obj.GetComponent<RectTransform>() ;
			
			
			rect.anchorMin = firstRect.anchorMin ;
			rect.anchorMax = firstRect.anchorMax ;
			rect.sizeDelta = firstRect.sizeDelta ;
			
			text.horizontalOverflow = firstGUIText.horizontalOverflow ;
			text.verticalOverflow = firstGUIText.verticalOverflow ;
			text.font = firstGUIText.font ;
			text.fontSize = firstGUIText.fontSize ;
			text.fontStyle = firstGUIText.fontStyle ;
			text.alignment = firstGUIText.alignment ;
			
			text.color = m_TextColor ;
				
			text.text = m_StrArray[ i ] ;
			tempPos.y -= firstGUIText.fontSize ;
			rect.anchoredPosition = tempPos ;
			
			if( true == m_AutoDetectAnimatinMax )
			{
				m_AnimationMaximum.x = Mathf.Abs( rect.anchoredPosition.x ) ;
				m_AnimationMaximum.y = Mathf.Abs( rect.anchoredPosition.y ) ;			
			}
			
			m_TransFormList.Add( rect ) ;
			m_ChildList.Add( obj ) ;
		}
		
		firstGUIText.enabled = false ;
	}
	
	
	protected override void UpdateAnimation()
	{
		
		Vector2 speedNow = m_AnimationSpeed * Time.deltaTime ;
		foreach( var rect in m_TransFormList )
		{
			if( null != rect )
			{
				Vector2 pos = rect.anchoredPosition ;
				pos += speedNow ;
				rect.anchoredPosition = pos ;
				
				if( true == m_ActiveUpperBound )
				{
					if( pos.x >= m_ExceedUpperBound.x &&
						pos.y >= m_ExceedUpperBound.y )
					{
						rect.gameObject.SetActive( false ) ;
					}
				}				
			}
		}

		m_AnimationSum += speedNow ;
		if( m_AnimationSum.magnitude > this.m_AnimationMaximum.magnitude )
		{
			m_AnimationIsEnd = true ;
		}
		//*/
	}
	
	
	List<RectTransform> m_TransFormList = new List<RectTransform>() ;
	
}

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
@file UIFadeInOut.cs
@author NDark
 
@date 20170621 file started and derived from GUI_FadeInOut

*/
using UnityEngine;

public class UIFadeInOut : GUI_FadeInOut 
{

	// Use this for initialization
	void Start () 
	{
		m_Texts = this.gameObject.GetComponentsInChildren<UnityEngine.UI.Text>() ;
		m_Images = this.gameObject.GetComponentsInChildren<UnityEngine.UI.Image>() ;
		m_RawImages = this.gameObject.GetComponentsInChildren<UnityEngine.UI.RawImage>() ;
		
		if( true == m_FadeInValid )
		{
			foreach( var rawImage in m_RawImages )
			{
				rawImage.color = new Color( rawImage.color.r , 
				                           rawImage.color.g , 
				                           rawImage.color.b , 
					0 ) ;
			}
			
			foreach( var image in m_Images )
			{
				image.color = new Color( image.color.r , 
				                        image.color.g , 
				                        image.color.b , 
					0 ) ;
			}			
			
			foreach( var text in m_Texts )
			{
				text.color = new Color( text.color.r , 
				                       text.color.g , 
				                       text.color.b , 
				                        0 ) ;
			}			
			
			ShowUGUI.Show( this.gameObject , false , true , true ) ;
		}
		
		m_State.state = (int)FadeState.UnActive ;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float currentAlpha = 0 ;
		m_State.Update() ;
		switch( (FadeState)m_State.state )
		{
		case FadeState.UnActive :
			if( m_State.ElapsedFromLast() > m_StartSec )
			{
				m_State.state = (int)FadeState.FadeIn ;			
			}
			break ;
		case FadeState.FadeIn :
			if( true == m_State.IsFirstTime() )
				ShowUGUI.Show( this.gameObject , true , true ,true ) ;
			
			if( false == m_FadeInValid )
				m_State.state = (int)FadeState.Steady ;				
			
			currentAlpha = GetAlpha() ;
			float timeRemain = m_FadeInSec - m_State.ElapsedFromLast() ;
			if( timeRemain < 0.0f )
			{
				currentAlpha = 1.0f ;
				m_State.state = (int) FadeState.Steady ;
			}
			else
			{
				currentAlpha = m_State.ElapsedFromLast() / m_FadeInSec ;
			}
			
			ApplyAlpha( currentAlpha ) ;	
			
			
			break ;
			
		case FadeState.Steady :
			if( m_State.ElapsedFromLast() > m_SteadySec )
				m_State.state = (int)FadeState.FadeOut ;
			break ;
		case FadeState.FadeOut :
			if( false == m_FadeOutValid )
				
				m_State.state = (int) FadeState.End ;
			
			currentAlpha = GetAlpha() ;
			timeRemain = m_FadeOutSec - m_State.ElapsedFromLast() ;
			if( timeRemain < 0.0f )
			{
				currentAlpha = 0.0f ;
				m_State.state = (int) FadeState.End ;
			}
			else
			{
				currentAlpha = timeRemain / m_FadeOutSec ;
			}

			ApplyAlpha( currentAlpha ) ;	
			
			break ;
		case FadeState.End :
			if( true == m_IsLoop )
			{
				m_State.state = (int) FadeState.FadeIn ;
			}
			else if( true == m_State.IsFirstTime() )
			{
				ShowUGUI.Show( this.gameObject , false , true , true ) ;
			}
			break ;			
		}
	
	}
	
	protected override float GetAlpha()
	{
		float ret = 0 ;
		if( m_Images.Length > 0 )
			ret = m_Images[ 0 ].color.a ;
		else if( m_RawImages.Length > 0 )
			ret = m_RawImages[ 0 ].color.a ;
		return ret ;
	}
	
	protected override void ApplyAlpha( float _Alpha )
	{
		// Debug.Log( "ApplyAlpha() _Alpha=" + _Alpha ) ;
		
		foreach( var image in m_Images )
		{
			image.color = new Color( image.color.r , 
			                        image.color.g , 
			                        image.color.b , 
				_Alpha ) ;
		}
		
		foreach( var raw in m_RawImages )
		{
			raw.color = new Color( raw.color.r , 
			                      raw.color.g , 
			                      raw.color.b , 
			                        _Alpha ) ;
		}
		
		foreach( var text in m_Texts )
		{
			text.color = new Color( text.color.r , 
			                       text.color.g , 
			                       text.color.b , 
				_Alpha ) ;			
		}		
	}
	
	protected UnityEngine.UI.RawImage [] m_RawImages = null ;
	protected UnityEngine.UI.Image [] m_Images = null ;
	protected UnityEngine.UI.Text [] m_Texts = null ;
	
}

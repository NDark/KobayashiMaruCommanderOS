/*
IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

By downloading, copying, installing or using the software you agree to this license.
If you do not agree to this license, do not download, install, copy or use the software.

    License Agreement For Kobayashi Maru Commander Open Source

Copyright (C) 2013, Chih-Jen Teng(NDark) and Koguyue Entertainment, 
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
@file XMLParseComponentParam.cs
@brief 分析XML傳出 ComponentParam 資料
@author NDark 

@date 20121110 . file started.
@date 20121204 by NDark . comment.
@date 20121211 by NDark . add parsing of effect3DObjectTemplateName

*/
using UnityEngine;
using System.Xml;

public static class XMLParseComponentParam 
{
	public static bool Parse( /*in*/ XmlNode _ComponentNode ,
							  out ComponentParam _Result )
	{
		_Result = new ComponentParam() ;
		if( null == _ComponentNode )
			return false ;
		
		if( null != _ComponentNode.Attributes[ "componentName" ] )
			_Result.m_ComponentName = _ComponentNode.Attributes[ "componentName" ].Value ;
		
		if( null != _ComponentNode.Attributes[ "effect3DObjectTemplateName" ] )
			_Result.m_Effect3DObjectTemplateName = _ComponentNode.Attributes[ "effect3DObjectTemplateName" ].Value ;		
		
		if( null != _ComponentNode.Attributes[ "displayName" ] )
			_Result.GUIDisplayName = _ComponentNode.Attributes[ "displayName" ].Value ;
		if( true == _ComponentNode.HasChildNodes )
		{
			for( int i = 0 ; i < _ComponentNode.ChildNodes.Count ; ++i )
			{
				XmlNode childNode = _ComponentNode.ChildNodes[ i ] ;
				if( "GUIPosition" == childNode.Name )
				{
					if( null != childNode.Attributes[ "x" ] && 
						null != childNode.Attributes[ "y" ] &&
						null != childNode.Attributes[ "width" ] &&
						null != childNode.Attributes[ "height" ] )
					{
						float x = 0.0f ;
						float y = 0.0f ;
						float width = 0.0f ;
						float height = 0.0f ;
						float.TryParse( childNode.Attributes[ "x" ].Value , out x ) ;
						float.TryParse( childNode.Attributes[ "y" ].Value , out y ) ;
						float.TryParse( childNode.Attributes[ "width" ].Value , out width ) ;
						float.TryParse( childNode.Attributes[ "height" ].Value , out height ) ;
						_Result.GUIRect = new Rect( x , y , width , height ) ;
					}
				}
			}
		}
		return ( 0 != _Result.m_ComponentName.Length ) ;
	}
}

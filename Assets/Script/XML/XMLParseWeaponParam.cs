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
@file XMLParseWeaponParam.cs
@brief 分析 XML 傳出WeaponParam資料
@author NDark 

@date 20121110 . file started.
. add paring of angle and range
. add parsing of fireAudioName
@date 20121204 by NDark . comment.
@date 20130119 by NDark . add parsing accuracy at Parse()

*/
using UnityEngine;
using System.Xml;

public static class XMLParseWeaponParam 
{
	public static bool Parse( /*in*/ XmlNode _WeaponParamNode ,
							  out WeaponParam _Result )
	{
		_Result = new WeaponParam() ;
		if( null == _WeaponParamNode )
			return false ;
		
		if( null != _WeaponParamNode.Attributes[ "componentName" ] )
			_Result.m_ComponentName = _WeaponParamNode.Attributes[ "componentName" ].Value ;

		if( null != _WeaponParamNode.Attributes[ "angle" ] )
		{
			float tmp = 0.0f ;
			float.TryParse( _WeaponParamNode.Attributes[ "angle" ].Value  , out tmp ) ;
			_Result.m_Angle = tmp ;
		}

		if( null != _WeaponParamNode.Attributes[ "range" ] )
		{
			float tmp = 0.0f ;
			float.TryParse( _WeaponParamNode.Attributes[ "range" ].Value , out tmp ) ;
			_Result.m_Range = tmp ;
		}
		
		if( null != _WeaponParamNode.Attributes[ "accuracy" ] )
		{
			float accuracy = 0.0f ;
			float.TryParse( _WeaponParamNode.Attributes[ "accuracy" ].Value , out accuracy ) ;
			_Result.m_Accuracy = accuracy ;
		}	
		
		if( null != _WeaponParamNode.Attributes[ "fireAudioName" ] )
			_Result.m_FireAudioName = _WeaponParamNode.Attributes[ "fireAudioName" ].Value ;
		
		return ( 0 != _Result.m_ComponentName.Length ) ;
	}
}

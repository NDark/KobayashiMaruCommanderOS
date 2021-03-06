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
@file PosRoute.cs
@author NDark
 
移動節點 

## m_Destination 目標
## m_MoveTime 移動時間
## m_WaitTime 等待時間
## m_MoveDetectGUIObject 移動時偵測的GUI物件 
## m_WaitDetectGUIObject 等待時偵測的GUI物件 

@date 20121224 file started.
@date 20121224 by NDark . add class member m_DetectGUIObject
@date 20121225 by NDark 
. rename m_DetectGUIObject to m_MoveDetectGUIObject
. add class member m_WaitDetectGUIObject
@date 20130112 by NDark . comment.

*/
using UnityEngine;

[System.Serializable]
public class PosRoute 
{
	public PosAnchor m_Destination = new PosAnchor() ;
	public float m_MoveTime = 0.0f ;
	public float m_WaitTime = 0.0f ;
	public NamedObject m_MoveDetectGUIObject = null ;
	public NamedObject m_WaitDetectGUIObject = null ;
	
	public PosRoute()
	{}
	
	public PosRoute( PosRoute _src )
	{
		m_Destination = new PosAnchor( _src.m_Destination ) ;
		m_MoveTime = _src.m_MoveTime ;
		m_WaitTime = _src.m_WaitTime ;
		if( null != _src.m_MoveDetectGUIObject )
			m_MoveDetectGUIObject = new NamedObject( _src.m_MoveDetectGUIObject ) ;
		if( null != _src.m_WaitDetectGUIObject )
			m_WaitDetectGUIObject = new NamedObject( _src.m_WaitDetectGUIObject ) ;		
	}
	
}

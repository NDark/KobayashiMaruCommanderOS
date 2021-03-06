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
@file UnitClickPlaneAutoAlignCamera.cs
@author NDark

單位的點選物件自動對攝影機校正

# 讓點選物件與攝影機維持一直線
# 距離鎖定在6.0，超過碰撞的子物件5.0。

@date 20121225 file started.
@date 20121227 by NDark . modify the y value of click cube to 5.1 (above collide cube)

*/
using UnityEngine;

public class UnitClickPlaneAutoAlignCamera : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == this.transform.parent )
			return ;
		
		GameObject parentObj = this.transform.parent.gameObject ;
		Vector3 parentPos = parentObj.transform.position ;
		Vector3 camPos = Camera.main.transform.position ;
		Vector3 vecToCam = camPos - parentPos ;
		vecToCam.Normalize() ;
		vecToCam *= 5 ;
		vecToCam.y = 6.0f ;
		this.transform.position = parentPos + vecToCam ;
	}
}

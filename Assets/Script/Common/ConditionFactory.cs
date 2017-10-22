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
@file ConditionFactory.cs
@author NDark
@date 20130115 file started.
*/
using UnityEngine;

[System.Serializable]
public static class ConditionFactory
{
	public static Condition GetByString( string _ConditionName )
	{
		if( _ConditionName == "Condition_Time" )
			return new Condition_Time() ;
		else if( _ConditionName == "Condition_Collision" )
			return new Condition_Collision() ;		
		else if( _ConditionName == "Condition_RemainingEnemyIsZero" )
			return new Condition_RemainingEnemyIsZero() ;
		else if( _ConditionName == "Condition_UnitIsDead" )
			return new Condition_UnitIsDead() ;
		else if( _ConditionName == "Condition_MainCharacterIsDead" )
			return new Condition_MainCharacterIsDead() ;
		else if( _ConditionName == "Condition_UnitIsAlive" )
			return new Condition_UnitIsAlive() ;
		else
			return null ;
	}

}

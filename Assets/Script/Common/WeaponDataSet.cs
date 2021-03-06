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
@file WeaponDataSet.cs
@brief 武器運作相關物件集
@date 20121029 . file started.
 
# m_WeaponType 武器種類
# m_FireState 目前發射狀態
# m_TargetComponentObject 目標部件物件
# m_TargetUnitObject 目標單位 
# m_TargetDirection 目標方向
# m_Displacement 誤差向量
# m_FireStartTime 開火時間
# m_FireTotalTime 開火總時間
# m_FireAudio 發射的音效
# m_CauseDamage 製造傷害量
# m_RelativeDamageEffectNames 開火過程觸發的特效清單

	
@date 20121103 by NDark 
. change m_TargetComponentName to private , and add its property.
. add class member m_TargetDirection to WeaponDataSet.
@date 2012110 by NDark . change m_TargetUnitName to private , and add its property.
@date 20121109 by NDark 
. use NamedObject to replace string and GameObject.
. refine code.
@date 20121111 by NDark . add class member m_RelativeDamageEffectNames
@date 20121112 by NDark . inheritate ComponentDataSet
@date 20121121 by NDark . add WeaponType_TrakorBeam
@date 20121123 by NDark . rename to m_UnitGameObject
@date 20121203 by NDark . comment.
@date 20121211 by NDark . add WeaponType_Cannon
@date 20130110 by NDark . rename class method TargetComponentName() to TargetComponentObjectName()
@date 20130112 by NDark
. add class method SetupTarget()
. refactor and comment.
. remove redundent WeaponFireState_ of WeaponFireState
. remove redundent WeaponType_ of WeaponType
@date 20130204 by NDark
. add class RelativeDamageEffect
. change type of class member m_RelativeDamageEffectNames

*/
using UnityEngine;
using System.Collections.Generic;

/*
 * @enum WeaponFireState
 */
[System.Serializable]
public enum WeaponFireState
{
	UnActive = 0 ,
	Ready ,
	FireAnimating ,
	Firing ,
	FireCompleting ,
	Recharging ,	
}

/*
 * @enum WeaponType
 */
[System.Serializable]
public enum WeaponType
{
	None = 0 ,
	Phaser ,
	Torpedo ,
	Cannon ,
	TrakorBeam ,
}

[System.Serializable]
public class RelativeDamageEffect
{
	public GameObject m_TargetUnit = null ;
	public string m_DamageEffectName = "" ;
	public RelativeDamageEffect()
	{
	}
	public RelativeDamageEffect( RelativeDamageEffect _src )
	{
		m_TargetUnit = _src.m_TargetUnit ;
		m_DamageEffectName = _src.m_DamageEffectName ;
	}
}

/* 
 * @brief WeaponDataSet 
 * 
 * A data set which a weapon will used.
 * It include:
 * -# WeaponType of this weapon
 * 
 * -# GameObjectName (UnitObjectName) and its Object of the weapon
 * -# ComponentName of this weapon in UnitData.
 * -# Weapon (3D) Object Name and its Object of the weapon under GameObject.
 * -# Effect Object Name and its Object of the weapon.
 * 
 * -# TargetUnitName and its object of this weapon.
 * -# TargetComponentName and its object of this weapon.
 * 
 * -# TargetDirection of this fire
 * -# Displacement of this fire
 * -# FireStartTime of this fire
 * -# FireTotalTime of this fire
 * -# CauseDamage of this fire
 * 
 * -# FireAudio of this weapon
 */
[System.Serializable]
public class WeaponDataSet : ComponentDataSet
{
	public WeaponType m_WeaponType = WeaponType.None ; // 武器種類
	public WeaponFireState m_FireState = WeaponFireState.UnActive ; // 目前發射狀態
	
	public Vector3 m_TargetDirection = Vector3.zero ; // 目標方向
	public Vector3 m_Displacement = Vector3.zero ; // 誤差向量
	
	public float m_FireStartTime = 0.0f ; // 開火時間
	public float m_FireTotalTime = 0.0f ; // 開火總時間
	
	public AudioClip m_FireAudio = null ; // 發射的音效
	public float m_CauseDamage = 0.0f ; // 製造傷害量
	
	public List<RelativeDamageEffect> m_RelativeDamageEffectNames = new List<RelativeDamageEffect>() ; // 開火過程觸發的特效清單
	
	public void SetupTarget( GameObject _TargetUnitObject , 
							 GameObject _TargetComponentObject )
	{
		m_TargetUnitObject.Setup( _TargetUnitObject ) ;
		m_TargetComponentObject.Setup( _TargetComponentObject ) ;
	}
	public void SetupTarget( string _TargetUnitName , 
							 string _TargetComponentName )
	{
		m_TargetUnitObject.Name = _TargetUnitName ;
		m_TargetComponentObject.Name = _TargetComponentName ;
	}	
	
	public string TargetUnitName
	{
		get 
		{
			return m_TargetUnitObject.Name ;
		}
	}
	public GameObject TargetUnitObject
	{
		get
		{
			return m_TargetUnitObject.Obj ;
		}
	}
	
	
	public string TargetComponentObjectName
	{
		get
		{
			return m_TargetComponentObject.Name ;
		}
	}
	public GameObject TargetComponentObject
	{
		get
		{
			return m_TargetComponentObject.Obj ;
		}
	}	
	
	public WeaponDataSet()
	{
	}
	public WeaponDataSet( GameObject _UnitGameObject , 
						  string _ComponentName ) : base( _UnitGameObject , _ComponentName )
	{
	}
	public WeaponDataSet( WeaponDataSet _Src ) : base( _Src )
	{
		m_WeaponType = _Src.m_WeaponType ;
		m_FireState = _Src.m_FireState ;
		
		m_TargetComponentObject.Setup( _Src.m_TargetComponentObject ) ;
		m_TargetDirection = _Src.m_TargetDirection ;
		m_Displacement = _Src.m_Displacement ;
		m_FireStartTime = _Src.m_FireStartTime ;
		m_FireTotalTime = _Src.m_FireTotalTime ;
		
		m_FireAudio = _Src.m_FireAudio ;
		m_CauseDamage = _Src.m_CauseDamage ;
		
		m_RelativeDamageEffectNames = _Src.m_RelativeDamageEffectNames	 ;
	}
	
	private UnitObject m_TargetUnitObject = new UnitObject() ; // 目標單位
	private NamedObject m_TargetComponentObject = new NamedObject() ; // 目標部件物件 
	

}


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
@file CollideOnThisCollider.cs
@brief 船隻碰撞處理 
@author NDark

# 注意，此script是綁在底層的碰撞物件CollideCube上，而非船隻的物件上，所以要找資料要再往上一層。
# 檢查被碰撞(自己)的單位
# 檢查碰撞上的單位
# 非存活的單位不碰撞
# 如果沒有UnitData則不會碰撞
# CauseCameraShakeEffect() 如果主角撞到，則開啟攝影機震動特效。
# CauseDamage() 給予傷害 的條件是 有傷害系統
# CreateForce() 製造推開的力量（直接作用在物件，不透過MoveVec）
# 推開的力量向量依照物件的重量而決定 請參考 BaseDefine.STANDARD_COLLIDE_PUSH_SPEED
# 碰撞製造的傷害值 請參考 BaseDefine.COLLIDE_DAMAGE_IN_SEC
# 傷害值顯示的持續時間是 1.0秒
# 製造攝影機震動效果的持續時間是0.5

@date 20121124 by NDark . remove checking Unit ( MainCharacter doesn't with it )
@date 20121203 by NDark 
. comment.
. add class method CauseDamage()
@date 20121218 by NDark 
. refactor and comment.
. add class member m_CameraShakeEffectElapsedSec
. add class method CauseCameraShakeEffect()
@date 20121220 by NDark . add argument of CauseDamage()
@date 20130109 by NDark . add class method CreateForce()
@date 20130111 by NDark . refactor and comment.
@date 20130119 by NDark . add checking of shield at OnCollisionStay()
*/
// #define DEBUG

using UnityEngine;

public class CollideOnThisCollider : MonoBehaviour 
{
	public float m_DamageWordElapsedSec = 1.0f ;
	public float m_CameraShakeEffectElapsedSec = 0.5f ;
	
	private float m_DamageValueASec = BaseDefine.COLLIDE_DAMAGE_IN_SEC ;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnCollisionStay( Collision collision )  
	{
		GameObject targetUnit = GlobalSingleton.GetParentObject( collision.gameObject ) ;
		GameObject thisUnit = GlobalSingleton.GetParentObject( this.gameObject ) ;		
		if( -1 != collision.gameObject.name.IndexOf( "Shield" ) )
		{
			// Debug.Log( "CollideOnThisCollider::OnCollisionStay() collision.gameObject.name=" + collision.gameObject.name ) ;
			// 碰到防護罩不去算碰撞
			return ;
		}
		
		if( null != targetUnit &&
			null != thisUnit )
		{
			string TargetName = targetUnit.name ;
			string ThisName = thisUnit.name ;
			if( ThisName == TargetName )// do not collide with my self
				return ;
#if DEBUG_LOG			
			Debug.Log( "CollideOnThisCollider::OnCollisionStay() ThisName=" + ThisName + 
				" TargetName=" + TargetName ) ;
#endif			

			// do not collide with the unit is not alive.			
			UnitData targetUnitData = targetUnit.GetComponent< UnitData >() ;
			if( null == targetUnitData ||				
				false == targetUnitData.IsAlive() )
				return ;
			
			CauseDamage( thisUnit , targetUnit ) ;
			
			CreateForce( thisUnit , targetUnit ) ;
			
			if( ThisName == ConstName.MainCharacterObjectName )
			{
				CauseCameraShakeEffect() ;
			}

		}
	}
	
	// 製造推開的力量
	private void CreateForce( GameObject _ThisUnit , GameObject _AttackerUnit )
	{
		if( null == _ThisUnit ||	
			null == _AttackerUnit )
			return ;		
		
		UnitData unitData = _ThisUnit.GetComponent< UnitData >() ;
		if( null == unitData )
			return ;
		
#if DEBUG_LOG				
		Debug.Log( "CollideOnThisCollider::CreateForce() _ThisUnit=" + _ThisUnit.name + 
			" _AttackerUnit=" + _AttackerUnit.name ) ;
#endif		
				
		float targetMass = 1.0f ;
		Rigidbody rbody = _AttackerUnit.GetComponentInChildren<Rigidbody>() ;
		if( null != rbody )
			targetMass = rbody.mass ;

		float thisMass = 1.0f ;		
		rbody = _ThisUnit.GetComponentInChildren<Rigidbody>() ;
		if( null != rbody )
			thisMass = rbody.mass ;
		
		Vector3 ToThisUnitVec = _ThisUnit.transform.position - _AttackerUnit.transform.position ;
		ToThisUnitVec.y = 0 ;// do not move in y direction
		ToThisUnitVec.Normalize() ;
		
		float PushSpeedInSec = BaseDefine.STANDARD_COLLIDE_PUSH_SPEED * targetMass / thisMass ;


		Vector3 moveVecThisFrame = ToThisUnitVec * PushSpeedInSec * Time.deltaTime ;
		
#if DEBUG_LOG				
		Debug.Log( "CollideOnThisCollider::CreateForce() moveVecThisFrame.magnitude=" + moveVecThisFrame.magnitude ) ;
#endif				
		
		_ThisUnit.transform.Translate( moveVecThisFrame , Space.World ) ;
	}
	
	// 如果主角撞到，則開啟攝影機震動特效
	private void CauseCameraShakeEffect()
	{
		CameraShakeEffect effectobj = Camera.main.GetComponent<CameraShakeEffect>() ;
		if( null != effectobj )
			effectobj.ActiveCameraShakeEffect( true , m_CameraShakeEffectElapsedSec ) ;
	}
	
	// 給予傷害 
	private void CauseDamage( GameObject _ThisUnit , GameObject _AttackerUnit )
	{
		if( null == _ThisUnit ||	
			null == _AttackerUnit )
			return ;	
		
		// 取得攻擊者的顯示字串
		string attackerUnitDisplayName = _AttackerUnit.name ;
		UnitData attackerUnitData = _AttackerUnit.GetComponent<UnitData>() ;
		if( null != attackerUnitData &&
			-1 != attackerUnitData.m_DisplayNameIndex )
			attackerUnitDisplayName = StrsManager.Get( attackerUnitData.m_DisplayNameIndex ) ;
		
		UnitDamageSystem damageSys = _ThisUnit.GetComponent<UnitDamageSystem>() ;
		if( null == damageSys )
			return ;
		
		float damageValueThisFrame = m_DamageValueASec * Time.deltaTime ;
#if DEBUG_LOG				
		Debug.Log( "CollideOnThisCollider::CauseDamage() _AttackerUnit=" + _AttackerUnit.name + 
			" damageValueThisFrame=" + damageValueThisFrame + 
			" _ThisUnit=" + _ThisUnit.name ) ;
#endif
		damageSys.SetDamageNumberEffect( true , m_DamageWordElapsedSec ) ;
		damageSys.CauseDamageValueOut( _AttackerUnit.name ,
									   attackerUnitDisplayName ,
									   damageValueThisFrame , 
									   _ThisUnit.name ) ;
	}
}

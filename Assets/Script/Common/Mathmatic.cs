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
@file MathMatic.cs
@brief 數學運算 
@author NDark
 
## FindDotofCrossAndUp() 計算 外積與左右轉參數
## FindTargetRelationWithForward() 計算物件的相對關係
## FindTargetRelation() 計算物件的相對關係
## FindUnitRelation() 計算物件的相對關係
## Interpolate() 計算內差
## RandomVector() 隨機向量(可抹除某軸向的直)
## RandomRatioValue() 隨機值(最小到最大，可指定某精度)
## CreateWeaponRangeObject() 計算並產生武器範圍的顯示物件
## IsInScreen() 檢查物件是否在螢幕內
## IsTooFarFromScreen() 檢查物件是否在離開螢幕太遠(需要刪除)

@date 20121109 by NDark . refine code.
@date 20121120 by NDark . add class method FindDotofCrossAndUp()
@date 20121123 by NDark . add class method Interpolate()
@date 20121203 by NDark . comment.
@date 20121205 by NDark . add class method FindTargetRelation()
@date 20121206 by NDark . add class method FindTargetRelationWithForward()
@date 20121210 by NDark 
. add class method RandomVector()
. add class method RandomRatioValue()
@date 20121212 by NDark . add class method CreateWeaponRangeObject()
@date 20121218 by NDark . comment.
@date 20130119 by NDark 
. add class method IsInScreen()
. add class method IsTooFarFromScreen()
@date 20130205 by NDark . comment.

*/
using UnityEngine;
using System.Collections.Generic;

public class MathmaticFunc
{
	/*
	 * @brief Find dot value of cross vector of _Vec1 and _Vec2 with _Up
	 */
	public static bool FindDotofCrossAndUp( Vector3 _Vec1 , 
											Vector3 _Vec2 , 
											Vector3 _Up , 
											ref float _DotOfCrossAndUp )
	{
		Vector3 crossVec = Vector3.Cross( _Vec1 , _Vec2 ) ;
		_DotOfCrossAndUp = Vector3.Dot( crossVec , _Up ) ;
		return true ;
	}
	
	/*
	 * @brief Find relation between 1 src game objects, specified forward, and 1 target position.
	 * @param _VecToTar vector from _src to _tar
	 * @param _VecToTarNormalized normalized _VecToTar
	 * @param _AngleTarget angle between forward of _src and _VecToTar
	 * @param _DotOfCrossAndUp dot value of cross product of ( forward of _src and _VecToTar ) and up of _src
	 * @return valid calculation.
	 */	
	public static bool FindTargetRelationWithForward( GameObject _src , 
														Vector3 _srcForward ,
													    Vector3 _tarPosition ,
													    ref Vector3 _VecToTar ,
													    ref Vector3 _VecToTarNormalized ,
													    ref float _AngleTarget ,
													    ref float _DotOfCrossAndUp )
	{
		if( null == _src )
			return false ;

		Vector3 srcObjPos = _src.transform.position ;

		_VecToTar = _tarPosition - srcObjPos ;
		_VecToTarNormalized = _VecToTar ;
		_VecToTarNormalized.Normalize() ;		

		_AngleTarget = Vector3.Angle( _VecToTarNormalized , _srcForward ) ;

		if( false == FindDotofCrossAndUp( _VecToTarNormalized , 
										 _srcForward , 
										 _src.transform.up , 
										 ref _DotOfCrossAndUp ) )
		{
			return false ;
		}

		return true ;
	}
	
	/*
	 * @brief Find relation between 1 src game objects and 1 target position
	 * @param _VecToTar vector from _src to _tar
	 * @param _VecToTarNormalized normalized _VecToTar
	 * @param _AngleTarget angle between forward of _src and _VecToTar
	 * @param _DotOfCrossAndUp dot value of cross product of ( forward of _src and _VecToTar ) and up of _src
	 * @return valid calculation.
	 */	
	public static bool FindTargetRelation( GameObject _src , 
									     Vector3 _tarPosition ,
									     ref Vector3 _VecToTar ,
									     ref Vector3 _VecToTarNormalized ,
									     ref float _AngleTarget ,
									     ref float _DotOfCrossAndUp )
	{
		if( null == _src )
			return false ;
		return FindTargetRelationWithForward( _src ,
			_src.transform.forward , 
			_tarPosition ,
			ref _VecToTar , ref _VecToTarNormalized ,
			ref _AngleTarget , ref _DotOfCrossAndUp ) ;
	}
	
	/*
	 * @brief Find relation between 2 game objects.
	 * @param _VecToTar vector from _src to _tar
	 * @param _VecToTarNormalized normalized _VecToTar
	 * @param _AngleTarget angle between forward of _src and _VecToTar
	 * @param _DotOfCrossAndUp dot value of cross product of ( forward of _src and _VecToTar ) and up of _src
	 * @return valid calculation.
	 */
	public static bool FindUnitRelation( GameObject _src , 
									     GameObject _tar ,
									     ref Vector3 _VecToTar ,
									     ref Vector3 _VecToTarNormalized ,
									     ref float _AngleTarget ,
									     ref float _DotOfCrossAndUp ) 
	{
		if( null == _src || 
			null == _tar )
			return false ;

		Vector3 targetPos = _tar.transform.position ;

		return FindTargetRelation( _src , targetPos , 
			ref _VecToTar , ref _VecToTarNormalized ,
			ref _AngleTarget , ref _DotOfCrossAndUp ) ;
	}
	
	// 計算內差
	public static float Interpolate( float _index1 , float _value1 , 
					   				 float _index2 , float _value2 , 
					   				 float _indexInput )
	{
		float diffInIndex = _index2 - _index1 ;
		float diffInValue = _value2 - _value1 ;
		float ratio = ( _indexInput - _index1 ) / diffInIndex ;
		float ret = _value1 + diffInValue * ratio ;
		return ret ;
	}
	
	// return a vector which clear in 0 none, 1: x , 2:y, 3:z
	public static Vector3 RandomVector( int _ClearAxis = 0 )
	{
		Vector3 ret = Random.insideUnitSphere ;
		switch( _ClearAxis )
		{
		case 0 :
			break ;
		case 1 :
			ret.x = 0 ;
			break ;
		case 2 :
			ret.y = 0 ;
			break ;
		case 3 :
			ret.z = 0 ;
			break ;			
		}
		ret.Normalize() ;
		return ret ;
	}
	
	// return a value from _minValue to _maxValue
	public static float RandomRatioValue( float _minValue , float _maxValue , int _precisionNum = 100 )
	{
		float ret = 0.0f ;
		if( 0 != _precisionNum )
		{
			float vec = _maxValue - _minValue ;
			float ratio = Random.Range( 0 , _precisionNum ) / _precisionNum ;// from 0 to 1
			ret = _minValue + vec * ratio ;
		}
		return ret ;
	}
	
	// 計算並產生武器範圍的顯示物件
	public static GameObject CreateWeaponRangeObject( string _WeaponObjectName , 
													  float _WeaponRange , 
													  float _WeaponAngle )
	{
		List<Vector3> vList = new List<Vector3>() ;
		List<Vector2> uvList = new List<Vector2>() ;
		List<int> triList = new List<int>() ;
		GameObject ret = PrefabInstantiate.Create( "Template_Unit_WeaponRange" , 
												   ConstName.CreateWeaponRangeObjectName( _WeaponObjectName ) ) ;
		if( null != ret )
		{
			MeshFilter filter = ret.GetComponent<MeshFilter>() ;
			if( null != filter )
			{
				Mesh weaponRangeMesh = new Mesh() ;
				weaponRangeMesh.name = ConstName.CreateWeaponRangeMeshName(  _WeaponObjectName ) ;
				float startAngle = 90.0f - _WeaponAngle ;
				float endAngle = 90.0f + _WeaponAngle ;
				float angleNow = startAngle ;
				vList.Add( new Vector3( 0 , 0 , 0 ) ) ;
				uvList.Add( new Vector2( 0 , 0 ) );
				while( angleNow <= endAngle )
				{
					float xNor = Mathf.Cos( angleNow * Mathf.Deg2Rad ) ;
					float yNor = Mathf.Sin( angleNow * Mathf.Deg2Rad ) ;
					float x = _WeaponRange * xNor ;
					float y = _WeaponRange * yNor ;
					
					vList.Add( new Vector3( x , 0 , y ) ) ;
					uvList.Add( new Vector2( x , y ) ) ;
					angleNow += 5.0f ;// 以五度為單位
				}
				
				for( int i = vList.Count-1 ; i > 1 ; --i )
				{
					triList.Add( 0 ) ;
					triList.Add( i ) ;
					triList.Add( i-1 ) ;
				}
				weaponRangeMesh.vertices = vList.ToArray() ;
				weaponRangeMesh.uv = uvList.ToArray() ;
				weaponRangeMesh.triangles = triList.ToArray() ;
				weaponRangeMesh.RecalculateNormals() ;
				filter.mesh = weaponRangeMesh ;
			}
		}
		return ret ;
	}
	
	// ## IsInScreen() 檢查物件是否在螢幕內
	public static bool IsInScreen( GameObject _Obj )
	{
		bool ret = true ;
		if( null == _Obj )
			return ret ;
		Vector3 screenPosition = Camera.main.WorldToViewportPoint( _Obj.transform.position ) ;		
		if( screenPosition.x < 0 ||
			screenPosition.x > 1 ||
			screenPosition.y < 0 ||
			screenPosition.y > 1 )
			ret = false ;
		return ret ;
	}
	
	// ## IsTooFarFromScreen() 檢查物件是否在離開螢幕太遠(需要刪除)
	public static bool IsTooFarFromScreen( GameObject _Obj )
	{
		bool ret = false ;
		if( null == _Obj )
			return ret ;
		Vector3 screenPosition = Camera.main.WorldToViewportPoint( _Obj.transform.position ) ;		
		if( screenPosition.x < -0.5f ||
			screenPosition.x > 1.5f ||
			screenPosition.y < -0.5f ||
			screenPosition.y > 1.5f )
			ret = true ;
		return ret ;
	}		
}

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
@file UnitDataGUIAnimationManager.cs
@author NDark

# CONST_VibrationDistance 震動大小
# CONST_VibrationSec 震動秒數
# m_VibrationMap 震動的動畫軌，以介面的物件名稱做為索引
# SetupVibration() 設定介面物件震動，需要傳入單位與部件名稱
# UpdateVibration() 更新各動畫軌的震動與移除
# SetPosition() 設定物件的2D位置


@date 20130120 file started.
*/
using UnityEngine;
using System.Collections.Generic;

public class UnitDataGUIAnimationManager : MonoBehaviour 
{
	// GUIObject Name , and correspond GUI Object
	public class AnimationTrack 
	{
		public NamedObject m_Object = new NamedObject() ;
		public float m_TimeStart = 0.0f ;
		public Vector2 m_ResumePosition = Vector2.zero ;
	}
	
	public const float CONST_VibrationDistance = 5.0f ;
	public const float CONST_VibrationSec = 1.0f ;
	public Dictionary< string /*GUIObjectName*/  ,AnimationTrack> m_VibrationMap = new Dictionary<string, AnimationTrack>();
	
	public void SetupVibration( string _UnitName , string _ComponentName )
	{
		GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
		if( null == guiUpdate )
			return ;
		
		GUIObjNameSet guiSet = null ;
		if( _UnitName == guiUpdate.m_SelectTargetName )
			guiSet = guiUpdate.m_SelectTargetComponentGUIObjNames ;
		else if( _UnitName == ConstName.MainCharacterObjectName )
		{
			guiSet = guiUpdate.m_MainCharacterComponentGUIObjNames ;
		}
		else
			return ;
		
		if( false == guiSet.m_ObjMap.ContainsKey( _ComponentName ) )
			return ;
		
		AnimationTrack addTrack = new AnimationTrack() ;
		addTrack.m_Object.Setup( guiSet.m_ObjMap[ _ComponentName ] );
		addTrack.m_TimeStart = Time.time ;
		if( null != guiSet.m_ObjMap[ _ComponentName ].Obj )
		{
			GUIText guiText = guiSet.m_ObjMap[ _ComponentName ].Obj.GetComponentInChildren<GUIText>() ;
			if( null != guiText )
				addTrack.m_ResumePosition = guiText.pixelOffset ;
		}
		
		m_VibrationMap[ guiSet.m_ObjMap[ _ComponentName ].Name ] = addTrack ;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateVibration() ;
	}
	
	private void UpdateVibration()
	{
		List<string> removeList = new List<string>() ;
		foreach( string key in m_VibrationMap.Keys )
		{
			// resume the font size
			Vector2 position = Vector2.zero ;
			
			AnimationTrack track = m_VibrationMap[ key ] ;
			float stopTime = track.m_TimeStart + CONST_VibrationSec ;
			
			if( null == track.m_Object.Obj )
			{
				removeList.Add( key ) ;
				continue ;
			}
			
			if( Time.time > stopTime )
			{
				// resume position
				position = track.m_ResumePosition ;
				removeList.Add( key ) ;
			}
			else
			{
				// calculate vibration position
				position.x = track.m_ResumePosition.x + Random.Range( -CONST_VibrationDistance , CONST_VibrationDistance ) ;
				position.y = track.m_ResumePosition.y + Random.Range( -CONST_VibrationDistance , CONST_VibrationDistance ) ;
			}
			
			// Debug.Log( track.m_Object.Name + " " + nowSize ) ;
			// set the current font size			
			SetPosition( track.m_Object.Obj , position ) ;
		}
	
		foreach( string removeKey in removeList )
		{
			m_VibrationMap.Remove( removeKey ) ;
		}
	}
	
	private void SetPosition( GameObject _GUIObj , Vector2 _Position )
	{
		if( null == _GUIObj )
			return ;
		GUIText guiText = _GUIObj.GetComponentInChildren<GUIText>() ;
		if( null == guiText )
			return ;
		guiText.pixelOffset = _Position ;
	}	
}

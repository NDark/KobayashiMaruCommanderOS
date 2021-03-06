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
@file ReloadAnimationManager.cs
@author NDark

# 掛載在 ReloadCompletenessAnimator 物件下
# UnitDataGUI 武器的填充完畢的動畫 管理器
# Setup() 呼叫時新增指定的單位及部件，然後透過GUIUpdate找到指定的UnitDataGUI的物件名稱做為索引放到m_AnimationMap中
# 播放完畢就移除
# CONST_AlertFontSize 警告的大小
# CONST_NormalFontSize 正常的大小
# CONST_ElapsedSecSteady 穩定的經過時間
# CONST_ElapsedSecTotal 總共的經過時間
# m_ReloadCompleteAudio 播放的聲音

@date 20121212 by NDark
@date 20121213 by NDark . add null checking at Update()
@date 20121219 by NDark . comment.
@date 20130113 by NDark . comment.

*/
using UnityEngine;
using System.Collections.Generic;

public class ReloadAnimationManager : MonoBehaviour 
{
	// GUIObject Name , and correspond GUI Object
	public class ReloadCompletenessAnimaionTrack 
	{
		public NamedObject m_Object = new NamedObject() ;
		public float m_TimeStart = 0.0f ;
	}
	
	public int CONST_AlertFontSize = 24;
	public int CONST_NormalFontSize = 12 ;
	public float CONST_ElapsedSecSteady = 1.0f ;
	public float CONST_ElapsedSecTotal = 3.0f ;
	public Dictionary<string,ReloadCompletenessAnimaionTrack> m_AnimationMap = new Dictionary<string, ReloadCompletenessAnimaionTrack>();
	public AudioClip m_ReloadCompleteAudio = null ;
	
	public void Setup( string _UnitName , string _ComponentName )
	{
		
		GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
		if( null == guiUpdate )
			return ;
		
		GUIObjNameSet guiSet = null ;
		if( _UnitName == guiUpdate.m_SelectTargetName )
			guiSet = guiUpdate.m_SelectTargetComponentGUIObjNames ;
		else if( _UnitName == ConstName.MainCharacterObjectName )
		{
			this.gameObject.GetComponent<AudioSource>().PlayOneShot( m_ReloadCompleteAudio ) ;
			guiSet = guiUpdate.m_MainCharacterComponentGUIObjNames ;
		}
		else
			return ;
		
		if( false == guiSet.m_ObjMap.ContainsKey( _ComponentName ) )
			return ;
		
		ReloadCompletenessAnimaionTrack addTrack = new ReloadCompletenessAnimaionTrack() ;
		addTrack.m_Object.Setup( guiSet.m_ObjMap[ _ComponentName ] );
		addTrack.m_TimeStart = Time.time ;
		m_AnimationMap[ guiSet.m_ObjMap[ _ComponentName ].Name ] = addTrack ;
		
		
			
	}
	// Use this for initialization
	void Start () 
	{
		m_ReloadCompleteAudio = ResourceLoad.LoadAudio( "ReloadComplete" ) ;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		List<string> removeList = new List<string>() ;
		foreach( string key in m_AnimationMap.Keys )
		{
			// resume the font size
			float nowSize = CONST_NormalFontSize ;
			
			ReloadCompletenessAnimaionTrack track = m_AnimationMap[ key ] ;
			float stopTime = track.m_TimeStart + CONST_ElapsedSecTotal ;
			float startShrinkTime = track.m_TimeStart + CONST_ElapsedSecSteady ;
			if( null == track.m_Object.Obj )
			{
				removeList.Add( key ) ;
				continue ;
			}
			if( Time.time > stopTime )
			{
				removeList.Add( key ) ;
			}
			else if( Time.time < startShrinkTime )
			{
				nowSize = CONST_AlertFontSize ;
			}
			else
			{
				nowSize = MathmaticFunc.Interpolate( startShrinkTime , (float) CONST_AlertFontSize ,
					stopTime , (float) CONST_NormalFontSize , 
					Time.time ) ;
				
			}
			
			// Debug.Log( track.m_Object.Name + " " + nowSize ) ;
			// set the current font size			
			SetSize( track.m_Object.Obj , (int) nowSize ) ;
		}
	
		foreach( string removeKey in removeList )
		{
			m_AnimationMap.Remove( removeKey ) ;
		}
	}
	
	private void SetSize( GameObject _GUIObj , int _SizeNow )
	{
		if( null == _GUIObj )
			return ;
		GUIText guiText = _GUIObj.GetComponentInChildren<GUIText>() ;
		if( null == guiText )
			return ;
		guiText.fontSize = _SizeNow ;
	}
}

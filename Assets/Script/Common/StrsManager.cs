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
@file StrsManager.cs
@author NDark
 
語言管理器

# 透過索引來取得目前語言的字串內容
# m_Filepath 使用檔案名稱，需設定在正確資料夾中
# Register() 註冊物件，當語言更換時會去 呼叫 NotifyObservers() 重置字串
# Get() 依照索引取得字串
# Setup() 依照檔案名稱讀入指定語言的文字設定檔

@date 20130121 file started.
@date 20130122 by NDark 
. add class method Setup()
. add class method Register()
. add class member m_Observers
@date 20130123 by NDark . fix an error of add of map at Register()
@date 20130205 by NDark . comment.

*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;

public static class StrsManager 
{
	
	public static bool m_Initialized = false ;
	public static Dictionary< int , string > m_Strs = new Dictionary<int, string>() ;
	public static Dictionary<string , GameObject> m_Observers = new Dictionary<string, GameObject>() ;
	
	private static string m_Filepath = "Strs.txt" ;
	
	public static void Register( GameObject _Obj )
	{
		if( true == m_Observers.ContainsKey( _Obj.name ) )
			m_Observers[ _Obj.name ] = _Obj ;
		else
			m_Observers.Add( _Obj.name , _Obj ) ;
	}
	
	public static string Get( int _Index ) 
	{
		if( false == m_Strs.ContainsKey( _Index ) )
			return "" ;
		return m_Strs[ _Index ] ;
	}
	
	public static void Setup()
	{
		Setup( m_Filepath ) ;
	}
	
	public static void Setup( string _StrsFilepath )
	{
		if( 0 == _StrsFilepath.Length )
			return ;
		
		m_Filepath = _StrsFilepath ;
		m_Strs.Clear() ;
		string ret = LoadDataToXML.LoadToString( _StrsFilepath , true ) ;
		string [] strVec = ret.Split( '\n' ) ;
		for( int i = 0 ; i < strVec.Length ; ++i )
		{
			string [] IDandStr = strVec[ i ].Split( '|' ) ;
			
			if( IDandStr.Length >= 3 )
			{
#if DEBUG				
				Debug.Log( "StrsManager::Setup()" + strVec[ i ] ) ;
#endif				
				int id = 0 ;
				string idstr = IDandStr[ 1 ] ;
				int.TryParse( idstr , out id ) ;
				string ContentStr = IDandStr[ 2 ] ;
				m_Strs[ id ] = ContentStr ;
			}
		}
#if DEBUG		
		Debug.Log( "StrsManager::Setup()" + m_Strs.Keys.Count ) ;
#endif		
		NotifyObservers() ;
		m_Initialized = true ;
	}
	
	private static void NotifyObservers()
	{
		List<string> removeList = new List<string>() ;
		Dictionary<string , GameObject >.Enumerator e = m_Observers.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( null == e.Current.Value )
			{
				removeList.Add( e.Current.Key ) ;
			}
			else
			{
				GUI_RetrieveTextAtEnable retrieveText = e.Current.Value.GetComponent<GUI_RetrieveTextAtEnable>() ;
				if( null != retrieveText )
					retrieveText.RetrieveText() ;
				
				GUI_RetrieveTextParagraphAtEnable retrieveTextParagraph = e.Current.Value.GetComponent<GUI_RetrieveTextParagraphAtEnable>() ;
				if( null != retrieveTextParagraph )
					retrieveTextParagraph.RetrieveText() ;				
			}
		}
		
		foreach( string key in removeList )
		{
			m_Observers.Remove( key ) ;
		}
	}
}

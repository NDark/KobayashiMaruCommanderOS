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
@file LoadDataToXML.cs
@author NDark

# 使用外部資料時切換 m_UseExternalData 為 true
## 此時讀取就會用全檔路徑(包含副檔名)
## 其中路徑為 執行檔下的Data/
# 不使用外部資料時，讀檔時就會去掉副檔名，讀取資源路徑內的資料。
# LoadToXML() 讀到XML節點
# LoadToString() 讀出字串內容

@date 20121210 file started.
@date 20121214 by NDark . fix an error of using without extension file at external data.
@date 20121219 by NDark . comment.
@date 20130121 by NDark
. add class method LoadByLanguage()
. add class method LoadInner()
@date 20130126 by NDark
. rename class method Load() to LoadToXML()
. rename class method LoadInner() to LoadToString()
. remove class method LoadByLanguage()
. remove class method Load()
. add code of common at Load()

*/
using UnityEngine;
using System.IO ;
using System.Xml;

public static class LoadDataToXML
{
	public static bool m_UseExternalData = false ;
	public static string CONST_ExeDataPrefix = "Data/" ;
	
	public static bool LoadToXML( ref XmlDocument _Doc , string _DataPath )
	{
		bool ret = false ;
		
		if( null != _Doc )
		{
			if( m_UseExternalData )
			{
				// 因為在此直接IO所以直接組合檔案路徑
				string filepath = "" ;
				filepath = ResourceLoad.CONST_CommonPrefix + CONST_ExeDataPrefix + _DataPath ;				
				// Debug.Log( filepath ) ;
				_Doc.Load( filepath ) ;
				ret = true ;
			}
			else
			{
				string pathWoExt = ExtRemove( _DataPath ) ;
				TextAsset textAsset = ResourceLoad.LoadDataToTextAsset( pathWoExt , false );// 裡面會加common/data
				if( null != textAsset )
				{
					_Doc.LoadXml( textAsset.text ) ;
					ret = true ;
				}
			}
		}
		return ret;
	}
	
	public static string LoadToString( string _DataPathwExt , bool _byLanguage )
	{
		string ret = "" ;
		
		if( m_UseExternalData )
		{
			StreamReader textReader = null ;
			string filepath = "" ;
			
			// 因為在此直接IO所以直接組合檔案路徑
			filepath = ResourceLoad.GetDataPath( _DataPathwExt , _byLanguage ) ;

			// Debug.Log( filepath ) ;
			textReader = new StreamReader( filepath ) ;
				
			if( null != textReader )
			{
				ret = textReader.ReadToEnd() ;
			}
		}
		else
		{
			string pathWoExt = ExtRemove( _DataPathwExt ) ;
			
			// 請 ResourceLoad 
			TextAsset textAsset = null ;			
			textAsset = ResourceLoad.LoadDataToTextAsset( pathWoExt , _byLanguage );
			
			if( null != textAsset )
			{
				ret = textAsset.text ;
			}
		}
		return ret;
	}		
	
	private static string ExtRemove( string _Path )
	{
		string ret = _Path ;
		int index = _Path.LastIndexOf( "." ) ;
		if( -1 != index )
		{
			ret = _Path.Substring( 0 , index ) ;
		}
		return ret ;
	}
}

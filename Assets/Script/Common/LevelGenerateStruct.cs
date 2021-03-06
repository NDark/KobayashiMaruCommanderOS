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
@file LevelGenerateStruct.cs
@author NDark


@date 20121219 file started.
@date 20121219 by NDark . add class member UnitTemplateData :: unitDataGUITextureName
@date 20130109 by NDark . add class member sideName of UnitInitializationData
@date 20130112 by NDark 
. comment.
. rename class member prefabName to prefabName.
@date 20130126 by NDark 
. add class member unitDisplayNameIndex of UnitTemplateData
. add copy constructor of UnitInitializationData
. add copy constructor of UnitGenerationData
@date 20130203 by NDark
. change type of class member initPosition

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
@brief Unit Initialization Data
單位初始資料 包含 單位名稱 樣板名稱 初始位置 初始轉向

# unitName 單位名稱 
# prefabName prefab樣板名稱 
# unitDataTemplateName 單位資料樣板名稱 
# raceName 種族名稱(小地圖標示)
# sideName 陣營名稱
# initPosition 初始位置 
# initOrientation 初始轉向
# supplementalVec 補充資料
*/
[System.Serializable]
public class UnitInitializationData
{
	public UnitInitializationData()
	{
	
	}
	
	public UnitInitializationData( UnitInitializationData _src ) 		
	{
		unitName = _src.unitName ;
		prefabName = _src.prefabName ;
		unitDataTemplateName = _src.unitDataTemplateName ;
		raceName = _src.raceName ;
		sideName = _src.sideName ;
		initPosition = _src.initPosition ;
		initOrientation = _src.initOrientation ;
		supplementalVec = _src.supplementalVec ;
	}
	
	public UnitInitializationData( string _UnitName , 
								   string _PrefabTemplateName ,
								   string _UnitDataTemplateName ,
								   string _RaceName ,
								   string _SideName ,
								   PosAnchor _InitPosition , 
								   Quaternion _InitOrientation ,
								   Dictionary<string , string > _SupplementalVec ) 		
	{
		unitName = _UnitName ;
		prefabName = _PrefabTemplateName ;
		unitDataTemplateName = _UnitDataTemplateName ;
		raceName = _RaceName ;
		sideName = _SideName ;
		initPosition = _InitPosition ;
		initOrientation = _InitOrientation ;
		supplementalVec = _SupplementalVec ;
	}
	public string unitName = "" ;
	public string prefabName = "" ;
	public string unitDataTemplateName = "" ;
	public string raceName = "" ;
	public string sideName = "" ;
	public PosAnchor initPosition = new PosAnchor() ;
	public Quaternion initOrientation = Quaternion.identity ;
	public Dictionary<string , string> supplementalVec = new Dictionary<string, string>() ;
}

/*
 * @brief UnitGenerationData 比UnitInitializationData多出時間
 */
[System.Serializable]
public class UnitGenerationData : UnitInitializationData
{
	public UnitGenerationData()
	{
	
	}
	
	public UnitGenerationData( UnitGenerationData _src ) : base( _src )
	{
		time = _src.time ;
	}
	
	public UnitGenerationData( string _UnitName , 
								string _PrefabTemplateName ,
								string _UnitDataTemplateName ,
								string _RaceName ,
								string _SideName ,
								PosAnchor _InitPosition , 
								Quaternion _InitOrientation ,
								Dictionary<string , string > _SupplementalVec ,
								float _time ) : base( _UnitName , 
														_PrefabTemplateName ,
														_UnitDataTemplateName ,
														_RaceName ,
														_SideName ,
														_InitPosition , 
														_InitOrientation ,
														_SupplementalVec )
	{
		time = _time ;
	}
	
	
			
	public float time = 0.0f ;
}	


/*
@brief Unit Template Data.
儲存單位樣版資料
每次單位要產生時就把單位樣本的資料複製到物件上.
 
# unitDataTemplateName 樣板名稱
# unitDataGUITextureName 使用的GUI貼圖名稱
# m_AddComponentList 開啟/加入元件名稱
# m_standardParameters 標準資料清單
# unitComponents 單位部件清單
# unitDisplayNameIndex 顯示名稱的索引
 */
[System.Serializable]
public class UnitTemplateData
{
	public string unitDataTemplateName = "" ;
	public string unitDataGUITextureName = "" ;
	public int unitDisplayNameIndex = -1 ;
	public List<string> m_AddComponentList = new List<string>() ;
	public Dictionary<string , StandardParameter> m_standardParameters = new Dictionary<string , StandardParameter>() ;
	public List<UnitComponentData> unitComponents = new List<UnitComponentData>() ;

	public int Debug_InitSize_StandardParameters = 0 ;

	// property
	public Dictionary<string , StandardParameter> standardParameters
	{
		get{ return m_standardParameters ; }
		set
		{ 
			m_standardParameters = value ;
#if DEBUG_LOG			
			Debug_InitSize_StandardParameters = value.Count ;
#endif			
		}
	}
	
	public UnitTemplateData()
	{
		
	}
	
	public UnitTemplateData( UnitTemplateData _src )
	{
		unitDataTemplateName = _src.unitDataTemplateName ;
		unitDataGUITextureName = _src.unitDataGUITextureName ;
		unitDisplayNameIndex = _src.unitDisplayNameIndex ;
		standardParameters = _src.standardParameters ;
		unitComponents = _src.unitComponents ;
		m_AddComponentList = _src.m_AddComponentList ;
	}
}

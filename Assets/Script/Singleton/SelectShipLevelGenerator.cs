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
@file SelectShipLevelGenerator.cs
@author NDark

選擇船艦時的關卡讀檔

# 是一個簡易版本的讀檔
# 沒有建出場景，僅讀入基本設定資料
# 目前使用與LevelGenerator相同的程式碼

@date 20130204 file started.
*/
// #define DEBUG_LOG
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class SelectShipLevelGenerator : MonoBehaviour 
{
	// component param
	public string componentParamTableFilepath = "ComponentParamTable.xml" ;
	public Dictionary< string , ComponentParam > componentParamTable = new Dictionary<string, ComponentParam>() ;
	public ComponentParam [] Debug_componentParamTable_valueVec ;
	
	// weapon param
	public string weaponParamTableFilepath = "WeaponParamTable.xml" ;
	public Dictionary< string , WeaponParam > weaponParamTable = new Dictionary<string, WeaponParam>() ;
	public WeaponParam [] Debug_weaponParamTable_valueVec ;	
	
	// unit template table filepath
	public string unitTemplateTableFilepath = "UnitTable.xml" ;

	// unit template table read from unitTemplateTableFilepath
	public Dictionary< string , UnitTemplateData > unitTemplateTable = new Dictionary< string , UnitTemplateData > () ;
	
	public int Debug_LevelUnitTemplateTableSize = 0 ; 
	
	public GameObject m_GeneratedObj = null ;
	/*
	 * @brief Generate the specified Unit.
	 * 
	 */
	public GameObject GenerateUnit( string _UnitName , 
									string _PrefabeTemplateName , 
									string _UnitDataTemplateName ,
									string _RaceName ,		
									string _SideName ,	
									Vector3 _InitPos , 
									Quaternion _InitOreintation , 
									Dictionary<string,string> _SupplementalVec )
	{
		bool GUIIsFlip = ( _UnitName != ConstName.MainCharacterObjectName ) ;
		
		// Debug.Log( "GenerateUnit:" + unitName.ToString() ) ;
		GameObject unitObj = PrefabInstantiate.CreateByInit( _PrefabeTemplateName ,
														     _UnitName,
															 _InitPos,
															 _InitOreintation ) ;
		
		// confirm init position and orientation
		UnitInitialization unitInitScript = unitObj.GetComponent<UnitInitialization>() ;
		if( null == unitInitScript )
		{
			Debug.Log( "GenerateUnit() : null == unitInitScript" ) ;
		}
		else
		{
			unitInitScript.UnitInitializationPoint = _InitPos ;
			unitInitScript.UnitInitializationOrientation = _InitOreintation ;
		}
		
		// copy unit data 
		UnitData unitData = unitObj.GetComponent<UnitData>() ;
		unitData.m_PrefabTemplateName = _PrefabeTemplateName ;
		unitData.m_UnitDataTemplateName = _UnitDataTemplateName ;
		unitData.m_RaceName = _RaceName ;
		unitData.m_SideName = _SideName ;
		unitData.m_SupplementalVec = _SupplementalVec ;		
		
		// retrieve unit data from unitTemplateTable to component of UnitData
		if( false == this.unitTemplateTable.ContainsKey( _UnitDataTemplateName ) )
		{
			Debug.Log( "GenerateUnit() : false == this.unitTemplateTable.ContainsKey()=" + _UnitDataTemplateName ) ;
		}
		else if( null == unitData )
		{
			Debug.Log( "GenerateUnit() : null == unitData" ) ;
		}
		else
		{
			UnitTemplateData unitDataTemplateData = this.unitTemplateTable[ _UnitDataTemplateName ] ;
			unitData.m_UnitDataGUITextureName = unitDataTemplateData.unitDataGUITextureName ;
			unitData.m_DisplayNameIndex = unitDataTemplateData.unitDisplayNameIndex ;
			
			// assign component
			for( int i = 0 ; i < unitDataTemplateData.unitComponents.Count ; ++i )
			{
				// Debug.Log( "unitData.AssignComponent " + unitDataTemplateData.unitComponents[ i ].m_Name ) ;
				string componentName = unitDataTemplateData.unitComponents[ i ].m_Name ;
				unitData.AssignComponent( componentName ,
										  unitDataTemplateData.unitComponents[ i ] ) ;
				
				// set gui flip flag at component added.
				unitData.componentMap[ componentName ].m_ComponentParam.m_IsFlip = GUIIsFlip ;
			}
			
			// assign standard parameters
			string [] keyarray = new string[ unitDataTemplateData.standardParameters.Count ];
			unitDataTemplateData.standardParameters.Keys.CopyTo( keyarray , 0 ) ;
			
			for( int j = 0 ; j < unitDataTemplateData.standardParameters.Count ; ++j )
			{				
				unitData.AssignStandardParameter( keyarray[j] , 
												  unitDataTemplateData.m_standardParameters[ keyarray[j] ] ) ;				
			}
			
			// add component
			for( int i = 0 ; i < unitDataTemplateData.m_AddComponentList.Count ; ++i )
			{
				string componentName = unitDataTemplateData.m_AddComponentList[ i ] ;
				if( 0 == componentName.IndexOf( "AI" ) )
				{
					AIAddCompontInterface.AddComponent( unitObj , componentName ) ;                            
					// Debug.LogWarning("AIAddCompontInterface=" +  componentName ) ;
				}
				else if( 0 == componentName.IndexOf( "Nebula" ) )
				{
					LevelGenerationAddCompontInterface.AddComponent( unitObj , componentName ) ;                            
					// Debug.LogWarning("LevelGenerationAddCompontInterface=" +  componentName ) ;
				}
				else
				{
					Debug.LogError("SelectShipLevelGenerator AddComponent=" +  componentName ) ;
				}
			}
		}
			
		m_GeneratedObj = unitObj ;
		return unitObj ;
	}
	
	// Use this for initialization
	void Start () 
	{
		LoadComponentParamTable() ;
		LoadWeaponParamTable() ;
		LoadUnitTemplateTable() ;
	}
	
	// Update is called once per frame
	void Update () 
	{

		
	}
	
	// load component param table
	private void LoadComponentParamTable()
	{
		XmlDocument doc = new XmlDocument() ;		
		if( false == LoadDataToXML.LoadToXML( ref doc , this.componentParamTableFilepath ) )
			return ;
		
		XmlNode root = doc.FirstChild ;
		if( "ComponentParamTable" != root.Name )
		{
			return ;
		}
		
		if( false == root.HasChildNodes )
		{
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode unitNode = root.ChildNodes[ i ] ;
			if( "ComponentParam" == unitNode.Name )
			{
				ComponentParam componentParamResult ;
				if( true == XMLParseComponentParam.Parse( unitNode , 
														  out componentParamResult ) )
				{
					/*
					Debug.Log( componentParamResult.m_ComponentName ) ;
					Debug.Log( componentParamResult.GUIDisplayName ) ;
					Debug.Log( componentParamResult.GUIRect ) ;
					//*/
					string Key = componentParamResult.m_ComponentName ;
					if( true == this.componentParamTable.ContainsKey( Key ) )
					{
						Debug.Log( "LoadComponentParamTable() already contain=" + Key ) ;
					}
					else
					{
						this.componentParamTable.Add( Key ,
													  new ComponentParam( componentParamResult ) ) ;
					}
				}
			}
		}
		
#if DEBUG_LOG		
		Debug_componentParamTable_valueVec = new ComponentParam[ this.componentParamTable.Count ] ;
		this.componentParamTable.Values.CopyTo( Debug_componentParamTable_valueVec , 0 ) ;
#endif 
	}	
	
	// load weapon param table
	private void LoadWeaponParamTable()
	{
		XmlDocument doc = new XmlDocument() ;		
		if( false == LoadDataToXML.LoadToXML( ref doc , this.weaponParamTableFilepath ) )
			return ;		
		
		XmlNode root = doc.FirstChild ;
		if( "WeaponParamTable" != root.Name )
		{
			return ;
		}
		
		if( false == root.HasChildNodes )
		{
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode unitNode = root.ChildNodes[ i ] ;
			if( "WeaponParam" == unitNode.Name )
			{
				WeaponParam weaponParamResult ;
				if( true == XMLParseWeaponParam.Parse( unitNode , 
													   out weaponParamResult ) )
				{
					/*
					Debug.Log( weaponParamResult.m_ComponentName ) ;
					//*/
					string Key = weaponParamResult.m_ComponentName ;
					if( true == this.weaponParamTable.ContainsKey( Key ) )
					{
						Debug.Log( "LoadWeaponParamTable() already contain=" + Key ) ;
					}
					else
					{
						this.weaponParamTable.Add( Key ,
												   new WeaponParam( weaponParamResult ) ) ;
					}
				}
			}
		}
#if DEBUG_LOG
		Debug_weaponParamTable_valueVec = new WeaponParam[ this.weaponParamTable.Count ] ;
		this.weaponParamTable.Values.CopyTo( Debug_weaponParamTable_valueVec , 0 ) ;
#endif
	}	
	
	/*
	 * @brief Load unit template table to unitTemplateTable
	 */
	private void LoadUnitTemplateTable()
	{
		XmlDocument doc = new XmlDocument() ;
		if( false == LoadDataToXML.LoadToXML( ref doc , this.unitTemplateTableFilepath ) )
			return ;
		XmlNode root = doc.FirstChild ;
		if( false == root.HasChildNodes )
		{
			Debug.Log( "LoadUnitTemplateTable() : false == root.HasChildNodes" ) ;
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			if( "Unit" == root.ChildNodes[ i ].Name )
			{
				XmlNode unitNode = root.ChildNodes[ i ] ;
				
				// parse unit
				UnitTemplateData newData = null ;
				if( false == ParseUnitTemplateData( unitNode , 
												    out newData ) )
				{
					continue ;
				}
				
				// add into unitTemplateTable
				if( true == unitTemplateTable.ContainsKey( newData.unitDataTemplateName ) )
				{
					Debug.Log( "LoadUnitTemplateTable() has already the same unitDataTemplateName=" + newData.unitDataTemplateName ) ;
				}
				else 
				{
					unitTemplateTable.Add( newData.unitDataTemplateName , newData ) ;
				}
				Debug_LevelUnitTemplateTableSize = unitTemplateTable.Count ;
			}
		}
	}	

	/* 
	 * @brief Parse unit from xml
	 */
	private bool ParseUnitTemplateData( XmlNode _UnitNode , 
								out UnitTemplateData _Data )
	{
		_Data = new UnitTemplateData() ;
		
		if( null == _UnitNode.Attributes[ "unitDataTemplateName" ] )
			return false ;
		
		_Data.unitDataTemplateName = _UnitNode.Attributes[ "unitDataTemplateName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "unitDataGUITextureName" ] )
			_Data.unitDataGUITextureName = _UnitNode.Attributes[ "unitDataGUITextureName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "UnitDisplayNameIndex" ] )
		{
			string UnitDisplayNameIndexStr = _UnitNode.Attributes[ "UnitDisplayNameIndex" ].Value ;
			int.TryParse( UnitDisplayNameIndexStr , out _Data.unitDisplayNameIndex ) ;
		}
		
		for( int i = 0 ; i < _UnitNode.ChildNodes.Count ; ++i )
		{
			if( -1 != _UnitNode.ChildNodes[ i ].Name.IndexOf( "comment" ) )
			{
				// comment
			}
			else if( "AddComponent" == _UnitNode.ChildNodes[ i ].Name )
			{
				if( null != _UnitNode.ChildNodes[ i ].Attributes["name"] )
				{
					_Data.m_AddComponentList.Add( _UnitNode.ChildNodes[ i ].Attributes["name"].Value ) ;
				}
			}
			else if( "Component" == _UnitNode.ChildNodes[ i ].Name )
			{
				// parse compoent
				UnitComponentData componentData ;
				if( true == ParseComponent( _UnitNode.ChildNodes[ i ] , 
											out componentData ) )
				{
					_Data.unitComponents.Add( componentData ) ;
				}
			}
			else 
			{
				// Debug.Log( "_UnitNode.ChildNodes[ i ]" + _UnitNode.ChildNodes[ i ].Name ) ;
				// parse standard parameter
				string name ;
				StandardParameter param ;
				if( true == XMLParseLevelUtility.ParseStandardParameter( _UnitNode.ChildNodes[ i ] ,
																		out name , 
																		out param ) )
				{
					_Data.standardParameters.Add( name ,param ) ;
				}
			}
		}
		return true ;
	}	
	
	/* 
	 * @brief Parse component from xml 
	 */
	private bool ParseComponent( XmlNode _ComponentNode ,
						 out UnitComponentData _Component )
	{

		_Component = new UnitComponentData() ;
		
		if( null == _ComponentNode )
			return false ;
	
		if( null == _ComponentNode.Attributes[ "name" ] )
			return false ;
		
		string name = _ComponentNode.Attributes[ "name" ].Value ;
		if( 0 == name.Length )
			return false ;
		
		// assign component parameter from table componentParamTable[].
		if( true == this.componentParamTable.ContainsKey( name ) )
		{
			_Component.m_ComponentParam = new ComponentParam( this.componentParamTable[ name ] ) ;
		}

		// assign weapon parameter from table weaponParamTable[].
		if( true == this.weaponParamTable.ContainsKey( name ) )
		{
			_Component.m_WeaponParam = new WeaponParam( this.weaponParamTable[ name ] ) ;
		}
		
		_Component.m_Name = name ;
		for( int i = 0 ; i < _ComponentNode.ChildNodes.Count ; ++i )
		{
			string label ;
			StandardParameter newStandardParameter ;			
			XMLParseLevelUtility.ParseStandardParameter( _ComponentNode.ChildNodes[ i ] ,
														out label , 
														out newStandardParameter ) ;
			
			/*
				public StandardParameter m_HP = new StandardParameter() ;
				public StandardParameter m_Generation = new StandardParameter() ;		
				public StandardParameter m_Energy = new StandardParameter() ;
				public StandardParameter m_Effect = new StandardParameter() ;	
				
				public ComponentStatus m_Status = ComponentStatus.ComponentStatus_Normal ;
				public StatusDescription m_StatusDescription = new StatusDescription() ;
				public InterpolateTable m_Effect_HP_Curve = new InterpolateTable() ;
				
				// weapon only
				public StandardParameter m_ReloadEnergy = new StandardParameter() ;
				public StandardParameter m_ReloadGeneration = new StandardParameter() ;
				public WeaponStatus m_WeaponStatus ;
			 
			 */
			if( "hp" == label )
			{
				_Component.m_HP = new StandardParameter( newStandardParameter ) ;
			}
			else if( "energy" == label )
			{
				_Component.m_Energy = new StandardParameter( newStandardParameter ) ;
			}
			else if( "generation" == label )
			{
				_Component.m_Generation = new StandardParameter( newStandardParameter ) ;
			}
			else if( "effect" == label )
			{
				_Component.m_Effect = new StandardParameter( newStandardParameter ) ;
			}
			
			else if( "reloadEnergy" == label )
			{
				_Component.m_ReloadEnergy = new StandardParameter( newStandardParameter ) ;
			}
			else if( "reloadGeneration" == label )
			{
				_Component.m_ReloadGeneration = new StandardParameter( newStandardParameter ) ;
			}
			
		}
		
		return ( 0 != _Component.m_Name.Length ) ;
	}	
}

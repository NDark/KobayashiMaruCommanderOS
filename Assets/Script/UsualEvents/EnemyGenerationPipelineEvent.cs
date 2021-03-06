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
@date EnemyGenerationPipelineEvent.cs
@author NDark 

敵人產生的排隊事件

# 會依照指定的對象排隊在該對象之後產生
# CheckPipelineUnitName 檢查的目標
# 要產生的單位 使用EnemyGeneration的標籤

@date 20130102 . file created.
@date 20130102 by NDark . change the way judging of unit state at DoKeepActive()

*/
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class EnemyGenerationPipelineEvent : TimeEvent 
{
	
	NamedObject m_CheckPipelineUnit = new NamedObject() ;
	List<UnitGenerationData> m_GenerateList = new List<UnitGenerationData>() ;
	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes["CheckPipelineUnitName"] ||
			null == _Node.Attributes["StartSec"] ||
			null == _Node.Attributes["ElapsedSec"] )
		{
			return false ;
		}
		
		string checkPipelineUnitName = _Node.Attributes["CheckPipelineUnitName"].Value ;
		string startSecStr = _Node.Attributes["StartSec"].Value ;
		string elapsedSecStr = _Node.Attributes["ElapsedSec"].Value ;
		
		float startSec = 0.0f ;
		float.TryParse( startSecStr , out startSec ) ;
		
		float elapsedSec = 0.0f ;
		float.TryParse( elapsedSecStr , out elapsedSec ) ;
		
		List<UnitGenerationData> addList = new List<UnitGenerationData>() ;
		if( _Node.HasChildNodes )
		{
			for( int i = 0 ; i < _Node.ChildNodes.Count ; ++i )
			{
				XmlNode unitNode = _Node.ChildNodes[ i ] ;
				string unitName ;
				string prefabTemplateName ;
				string unitDataTemplateName ;
				string raceName ;
				string sideName ; 
				PosAnchor posAnchor ;
				Quaternion orientation ;
				Dictionary<string,string> supplemental ;
				float time ;
					
				if( true == XMLParseLevelUtility.ParseEnemyGeneration( unitNode ,
					out unitName , 
					out prefabTemplateName , 
					out unitDataTemplateName ,
					out raceName ,
					out sideName ,
					out posAnchor ,
					out orientation , 
					out supplemental ,
					out time ) )
				{
					// Debug.Log( "addList.Add" + unitName ) ;
					addList.Add( new UnitGenerationData( unitName ,
											prefabTemplateName ,
											unitDataTemplateName ,
											raceName ,
											sideName ,
											posAnchor ,
											orientation , 
											supplemental ,
											time ) ) ;
				}
					
			}
		}
		this.Setup( startSec , 
					elapsedSec , 
					checkPipelineUnitName , 
					addList ) ;
		return true ;			
	}
		
	public void Setup( float _startTime , 
					   float _elapsedTime , 
					   string _CheckPipelineUnitName ,
					   List<UnitGenerationData> _GenerateList )
	{
		m_Trigger.Setup( _startTime , _elapsedTime ) ;		
		m_CheckPipelineUnit.Setup( _CheckPipelineUnitName , null ) ;
		m_GenerateList = _GenerateList ;
	}
	
	public EnemyGenerationPipelineEvent()
	{
		
	}
	
	public EnemyGenerationPipelineEvent( EnemyGenerationPipelineEvent _src )
	{
		m_CheckPipelineUnit = _src.m_CheckPipelineUnit ;
		m_GenerateList = _src.m_GenerateList ;
	}
	
	protected override void DoKeepActive()
	{
		UnitData unitData = null ;
		if( /*( null == m_CheckPipelineUnit.Obj ) ||*/
				( 
					null != m_CheckPipelineUnit.Obj &&
					( null != ( unitData = m_CheckPipelineUnit.Obj.GetComponent<UnitData>() ) ) &&
					( unitData.m_UnitState.state > (int) UnitState.Alive )
				)
		   )
		{
			// Debug.Log( "被強制關閉了()" ) ;
			// 被強制關閉了
			InssertGenerationUnitListToEnemyGenerator() ;
			m_Trigger.Close() ;
		}
	}
	
	private void InssertGenerationUnitListToEnemyGenerator() 
	{
		EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
		if( null == enemyGenerator )
			return ;
		foreach( UnitGenerationData insertData in m_GenerateList )
		{
			Debug.Log( "enemyGenerator.InsertEnemyGenerationTable" + insertData.unitName ) ;
			insertData.time = Time.timeSinceLevelLoad ;
			enemyGenerator.InsertEnemyGenerationTable( insertData ) ;
		}
	}
}

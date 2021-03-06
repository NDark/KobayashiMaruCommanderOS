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
@file Condition.cs
@author NDark
@date 20130115 by NDark.
@date 20130117 by NDark . add m_TimeTrigger.Rewind at ParseXML()
@date 20130119 by NDark . add class Condition_UnitIsAlive.
@date 20130205 by NDark . commment.

*/
// #define DEBUG_LOG
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

/*
# 條件觸發 基本物件
# IsTrue() 是否可以觸發
# HasTriggered() 是否已經處發 
# ParseXML() 分析參數
*/
[System.Serializable]
public class Condition
{
	public virtual void Start()
	{
		m_HasTriggered = false ;
	}	
	public virtual bool ParseXML( XmlNode _Node )
	{
		return false ;
	}
	public virtual bool HasTriggered()
	{
		return m_HasTriggered ;
	}
	public virtual void Close()
	{
		m_HasTriggered = true ;
	}
	public virtual bool IsTrue()
	{
		return false ;
	}
	
	public Condition()
	{
	}
	public Condition( Condition _src )
	{
		m_HasTriggered = _src.m_HasTriggered ;
	}	
	private bool m_HasTriggered = false ;
}

/*
時間條件
# 參數
## StartTime
*/
[System.Serializable]
public class Condition_Time : Condition
{
	public override void Start()
	{
		base.Start() ;
		m_TimeTrigger.Rewind() ;
	}
/*
	<Condition ConditionName="Condition_Time" 
		StartTime="126.0" />	
 */	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes[ "StartTime" ] )
			return false ;
		
		string StartTimeStr =_Node.Attributes[ "StartTime" ].Value ;
		float StartTime = 0.0f ;
		if( true == float.TryParse( StartTimeStr , out StartTime ) )
		{
			m_TimeTrigger.Setup( StartTime ) ;
			m_TimeTrigger.Rewind() ;
		}
		
		return true ;
	}
	public override bool IsTrue()
	{
		return m_TimeTrigger.IsCountDownToZero() ;
	}

	public Condition_Time()
	{
	}
	public Condition_Time( Condition_Time _src )
	{
		m_TimeTrigger = new CountDownTrigger( _src.m_TimeTrigger ) ;
	}
	
	private CountDownTrigger m_TimeTrigger = new CountDownTrigger() ;
	
}

/*
碰撞條件

# 參數
## JudgeDistance 判斷距離
## TestObjectName 碰撞的測試物
## TestObjectName{0} 碰撞的測試物(複數)
## PosAnchor 碰撞的檢查點
*/
[System.Serializable]
public class Condition_Collision : Condition
{
/*
	<Condition ConditionName="Condition_Collision" 
		TestObjectName="Unit_NealsonCargoship04" 
		PosAnchor="DestinationObj"
		JudgeDistance="30" />
		
	<Condition ConditionName="Condition_Collision" 
		TestObjectName0="Unit_RescureUnit01" 
		TestObjectName1="Unit_RescureUnit02" 
		TestObjectName2="Unit_RescureUnit03" 
		PosAnchor="DestinationObj"
		JudgeDistance="40" />	
 */	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes[ "JudgeDistance" ] )
			return false ;
		
		if( false == XMLParseLevelUtility.ParseAnchor( _Node , ref m_PosAnchor ) )// "PosAnchor" 
			return false ;
		
		if( null != _Node.Attributes[ "TestObjectName" ] )
		{
			string TestObjectName = _Node.Attributes[ "TestObjectName" ].Value ;
			m_TestObject.Setup( TestObjectName , null ) ;
		}
		
		for( int i = 0 ; i < 10 ; ++i )
		{
			string key = string.Format( "TestObjectName{0}" , i ) ;
			if( null == _Node.Attributes[ key ] )
				break ;
			m_TestObjects.Add( new NamedObject( _Node.Attributes[ key ].Value ) ) ;
		}	
		
		float JudgeDistance = 0.0f ;
		string JudgeDistanceStr =_Node.Attributes[ "JudgeDistance" ].Value ;
		if( true == float.TryParse( JudgeDistanceStr , out JudgeDistance ) )
		{
			m_JudgeDistance = JudgeDistance ;
		}
		return true ;
	}
	
	public override bool IsTrue()
	{
		bool ret = false ;
		
		if( 0 != m_TestObject.Name.Length &&
			null != m_TestObject.Obj )
		{
			Vector3 DistVec = m_TestObject.Obj.transform.position - 
				m_PosAnchor.GetPosition() ;
			ret = ( DistVec.magnitude < m_JudgeDistance ) ;
		}
		else if( 0 < m_TestObjects.Count )
		{
			ret = true ;
			foreach( NamedObject obj in m_TestObjects )
			{
				Vector3 DistVec = obj.Obj.transform.position - 
					m_PosAnchor.GetPosition() ;

				if( DistVec.magnitude > m_JudgeDistance ) 
					ret = false ;

			}
		}
		return ret ;
	}

	public Condition_Collision()
	{
	}
	public Condition_Collision( Condition_Collision _src )
	{
		m_TestObject = new NamedObject( _src.m_TestObject ) ;
		m_TestObjects = new List<NamedObject>( _src.m_TestObjects ) ;
		m_PosAnchor = new PosAnchor( _src.m_PosAnchor ) ;
		m_JudgeDistance = _src.m_JudgeDistance ;
	}
	
	private NamedObject m_TestObject = new NamedObject() ;
	private List<NamedObject> m_TestObjects = new List<NamedObject>() ;
	private PosAnchor m_PosAnchor = new PosAnchor() ;
	float m_JudgeDistance = 0.0f ;

}



// 檢查目前剩下敵人數目是否等於0
[System.Serializable]
public class Condition_RemainingEnemyIsZero : Condition
{
	public override bool ParseXML( XmlNode _Node )
	{
		return true ;
	}
	public override bool IsTrue()
	{
		bool ret = false ;
		EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
		if( null != enemyGenerator )
		{
			ret = ( 0 == enemyGenerator.GetRemainingEnemyNum() ) ;
		}
		return ret ;
	}

	public Condition_RemainingEnemyIsZero()
	{
	}
	public Condition_RemainingEnemyIsZero( Condition_RemainingEnemyIsZero _src )
	{
	}
	
	
}

/*
檢查指定單位是否死亡

# 參數
## UnitName
*/
[System.Serializable]
public class Condition_UnitIsDead : Condition
{
	/*
		<Condition ConditionName="Condition_UnitIsDead" 
			UnitName="Unit_NealsonCargoship04" />
	*/	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes["UnitName"] )
			return false ;
		
		string UnitName = _Node.Attributes["UnitName"].Value ;
		m_Unit.Setup( UnitName , null ) ;
		// Debug.Log( "ParseXML=" + m_Unit.Name ) ;
		return true ;
	}
	
	public override bool IsTrue()
	{
		bool ret = false ;
		// Debug.Log( "IsTrue=" + m_Unit.Name ) ;
		if( 0 == m_Unit.Name.Length )
			return ret ;
		UnitData unitData = m_Unit.ObjUnitData ;
		if( null != unitData )
		{
			ret = ( unitData.m_UnitState.state == (int) UnitState.Dead ) ;
		}
#if DEBUG_LOG
		Debug.Log( "Condition_UnitIsDead::IsTrue() m_Unit.Name=" + m_Unit.Name + " ret=" + ret ) ;
#endif
		return ret ;
	}

	public Condition_UnitIsDead()
	{
	}
	public Condition_UnitIsDead( Condition_UnitIsDead _src )
	{
		m_Unit = new UnitObject( _src.m_Unit ) ;
	}
	
	protected UnitObject m_Unit = new UnitObject() ;
}

/*
檢查主角是否死亡
# 繼承 Condition_UnitIsDead
*/
[System.Serializable]
public class Condition_MainCharacterIsDead : Condition_UnitIsDead
{
	/*
	<Condition ConditionName="Condition_MainCharacterIsDead" />
	 */
	public override bool ParseXML( XmlNode _Node )
	{
		m_Unit.Setup( "MainCharacter" , null ) ;
		//Debug.Log( "ParseXML=" + m_Unit.Name ) ;
		return true ;
	}
	public override bool IsTrue()
	{
		return base.IsTrue() ;
	}
	public Condition_MainCharacterIsDead()
	{
	}
	public Condition_MainCharacterIsDead( Condition_MainCharacterIsDead _src ) : base( _src ) 
	{
	}
	
}

/*
檢察單位是否存活
# 繼承 Condition_UnitIsDead
*/
[System.Serializable]
public class Condition_UnitIsAlive : Condition_UnitIsDead
{
	/*
		<Condition ConditionName="Condition_UnitIsAlive" 
			UnitName="Unit_BattleStarGalactica" />	
	*/	
	public override bool IsTrue()
	{
		bool ret = false ;
		
		if( 0 == m_Unit.Name.Length )
		{
			Debug.Log( "Condition_UnitIsAlive::IsTrue() 0 == m_Unit.Name.Length ret=" + ret ) ;
			return ret ;
		}
	

				
		UnitData unitData = m_Unit.ObjUnitData ;
		if( null != unitData )
		{
			ret = unitData.IsAlive() ;
		}
#if DEBUG_LOG		
		Debug.Log( "Condition_UnitIsAlive::IsTrue() m_Unit.Name=" + m_Unit.Name + " ret=" + ret ) ;
#endif
		return ret ;
	}

	public Condition_UnitIsAlive()
	{
	}
	public Condition_UnitIsAlive( Condition_UnitIsAlive _src ) :base( _src )
	{
	}
}

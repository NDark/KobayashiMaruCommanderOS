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
@file ChallangeLevel01Manager.cs
@author NDark

挑戰關卡1的管理器

# 由於挑戰關卡1與一般關卡不同，需要有特殊的管理器來管理
## 一般敵艦的產生
## 波士敵艦的產生與消滅

@date 20130202 file started.
@date 20130204 by NDark . add class method RewindBossCountDown()
*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;

public class ChallangeLevel01Manager : MonoBehaviour 
{
	public bool m_IsActive = true ;
	
	// 小兵產生樣本
	List<UnitGenerationData> m_EnemyTemplates = new List<UnitGenerationData>() ;
	
	// 小兵產生倒數器
	CountDownTrigger m_EnemyGenerationTimer = new CountDownTrigger() ;
	
	// 小兵第一波
	CountDownTrigger m_EnemyGenerationFirstBurstTimer = new CountDownTrigger() ;
	bool m_FirstBurst = false ;
	
	// 波士產生樣本
	int m_BossIndex = 0 ;
	List<UnitGenerationData> m_BossTemplates = new List<UnitGenerationData>() ;

	// 當波士要出生的時候把小兵暫停,必須跟波士的週期一致
	TimeTrigger m_StopEnemyGenerationTimer = new TimeTrigger() ;
	
	// 波士產生倒數器
	CountDownTrigger m_BossGenerationTimer = new CountDownTrigger() ;
	
	// 波士目前啟動中
	bool m_BossIsAlive = false ;
	
	// 波士
	UnitObject m_BossNow = new UnitObject() ;
	
	// 波士的目標
	NamedObject m_BossDestination = new NamedObject() ;

	// 波士出生紅色警戒聲音播放
	bool m_AudioIsPlay = false ;
	
	// 波士出生紅色警戒聲音播放停止器
	CountDownTrigger m_PlayAudioStopTimer = new CountDownTrigger() ;
	
	// 波士出生紅色警戒
	AudioClip m_RedAlert = null ;
	
	// 出生位置
	List<GameObject> m_SpawnPosition = new List<GameObject>() ;
	
	
	
	public void Active( bool _Active )
	{
		m_IsActive = _Active ; 
	}
	
	// Use this for initialization
	void Start () 
	{
		m_RedAlert = ResourceLoad.LoadAudio( "Star Trek Sound Effects - Red Alert Klaxon" ) ;
		m_PlayAudioStopTimer.Setup( 4 ) ;
		
		SetupShuffleSpawnPosEvent() ;
		
		m_FirstBurst = false ;
		m_EnemyGenerationFirstBurstTimer.Setup( 5 ) ;
		m_EnemyGenerationFirstBurstTimer.Rewind() ;
		m_EnemyGenerationTimer.Setup( 60 ) ;
		m_EnemyGenerationTimer.Rewind() ;
		SetupSpawnEnemyData() ;
		
		m_StopEnemyGenerationTimer.Setup( 250 , 50 ) ;
		m_BossGenerationTimer.Setup( 300 ) ;
		
		RewindBossCountDown() ;
		
		SetupSpawnBossData() ;
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_IsActive )
			return ;
		
		CheckEnemyGeneartion() ;

		CheckBoseGeneration() ;

		CheckAudioPlay() ;
	}
	
	private void CheckEnemyGeneartion()
	{
		
		if( true == m_StopEnemyGenerationTimer.IsAboutToStart( true ) )
		{
		}
		else if( true == m_StopEnemyGenerationTimer.IsAboutToClose( true ) )
		{
		}		
		else if( true == m_StopEnemyGenerationTimer.IsReady() )
		{
			if( false == m_FirstBurst && 
				true == m_EnemyGenerationFirstBurstTimer.IsCountDownToZero() )
			{
#if DEBUG_LOG
				Debug.Log( "true == m_EnemyGenerationFirstBurstTimer.IsCountDownToZero()()" ) ;
#endif				
				SpawnEnemyDoEvent() ;
				m_FirstBurst = true ;
				m_EnemyGenerationTimer.Rewind() ;
			}
			else if( true == m_EnemyGenerationTimer.IsCountDownToZero() )
			{
				SpawnEnemyDoEvent() ;
				m_EnemyGenerationTimer.Rewind() ;
			}		
		}
	}	
	
	private void CheckBoseGeneration()
	{
		if( false == m_BossIsAlive )
		{
			if(	true == m_BossGenerationTimer.IsCountDownToZero() )
			{
				SpawnBossDoEvent() ;
				RewindBossCountDown() ;
				m_StopEnemyGenerationTimer.Initialize() ;
			}				
		}
		else
		{
			CheckRemoveBoss() ;
		}
		

	}
	
	private void CheckAudioPlay()
	{
		if( true == m_AudioIsPlay )
		{
			if( true == m_PlayAudioStopTimer.IsCountDownToZero() )
			{
				this.GetComponent<AudioSource>().Stop() ;
				m_AudioIsPlay = false ;
			}
		}		
	}
		
		
	
	private void SetupSpawnEnemyData()
	{
		UnitGenerationData addGenData = null ;
		
		addGenData = CreateEnemyGeneration( "Template_FederationDefiant01" ,
											"UnitDataTemplate_FederationDefiant_C01" ,
											"Federation" ) ;
		m_EnemyTemplates.Add ( addGenData );
				
		addGenData = CreateEnemyGeneration( "Template_KlingonBirdofPrey01" ,
											"UnitDataTemplate_KlingonBirdofPrey_C01" ,
											"KlingonEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );
		
		addGenData = CreateEnemyGeneration( "Template_KlingonWarbird" ,
											"UnitDataTemplate_KlingonWarbird_C01" ,
											"KlingonEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );
		
		
		addGenData = CreateEnemyGeneration( "Template_RomulanWarbird01" ,
											"UnitDataTemplate_RomulanWarbird_C01" ,
											"RomulanEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );	
		
		
		addGenData = CreateEnemyGeneration( "Template_RomulanValdore01" ,
											"UnitDataTemplate_RomulanValdore_C01" ,
											"RomulanEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );		
		
		
		addGenData = CreateEnemyGeneration( "Template_FerengiD'Kora01" ,
											"UnitDataTemplate_FerengiDKora_C01" ,
											"FerengiAlliance" ) ;		
		m_EnemyTemplates.Add ( addGenData );
		 
		
		
		addGenData = CreateEnemyGeneration( "Template_CardassianGalor01" ,
											"UnitDataTemplate_CardassianGalor_C01" ,
											"CardassianEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );
		
		
		addGenData = CreateEnemyGeneration( "Template_CardassianDreadNought" ,
											"UnitDataTemplate_CardassianDreadNought_C01" ,
											"CardassianEmpire" ) ;		
		m_EnemyTemplates.Add ( addGenData );		
	}
	
	private void SpawnEnemyDoEvent()
	{
		
		EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
		if( null == enemyGenerator )
			return ;
#if DEBUG_LOG
		Debug.Log( "SpawnEnemyDoEvent()" ) ;
#endif

		UnitGenerationData addGenData = RandomFromVec( m_EnemyTemplates ) ;
		
		addGenData.unitName = addGenData.unitName + ConstName.GenerateIterateString() ;
		if( 0 < m_SpawnPosition.Count )
		{
			int index = Random.Range( 0 , m_SpawnPosition.Count ) ;
			addGenData.initPosition.Setup( m_SpawnPosition[ index ].transform.position ) ;
			addGenData.initOrientation = m_SpawnPosition[ index ].transform.rotation ;
		}
		addGenData.time = Time.timeSinceLevelLoad ;
		
		enemyGenerator.InsertEnemyGenerationTable( addGenData ) ;
	}
	
	private void SetupSpawnBossData()
	{
		UnitGenerationData addGenData = null ;
		
		addGenData = CreateBossGeneration( "Template_BorgCube01" ,
										   "UnitDataTemplate_BorgCube_C01" ,
										   "Borg" ) ;
		
		m_BossTemplates.Add ( addGenData );
		

		addGenData = CreateBossGeneration( "Template_RomulanScimitar01" ,
										   "UnitDataTemplate_RomulanScimitar_C01" ,
										   "RomulanEmpire" ) ;
		
		m_BossTemplates.Add ( addGenData );
		
		addGenData = CreateBossGeneration( "Template_RomulanNarada01" ,
										   "UnitDataTemplate_RomulanNarada_C01" ,
										   "RomulanEmpire" ) ;
		m_BossTemplates.Add ( addGenData );

	}
	
	private void SpawnBossDoEvent()
	{
		
		EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
		if( null == enemyGenerator )
			return ;
#if DEBUG_LOG
		Debug.Log( "SpawnBossDoEvent()" ) ;
#endif

		UnitGenerationData addGenData = ChooseBySequence( m_BossTemplates ) ;
		addGenData.unitName = addGenData.unitName + ConstName.GenerateIterateString() ;
		addGenData.time = Time.timeSinceLevelLoad ;
		
		if( 0 < m_SpawnPosition.Count )
		{
			int index = Random.Range( 0 , m_SpawnPosition.Count ) ;
			addGenData.initPosition.Setup( m_SpawnPosition[ index ].transform.position ) ;
			addGenData.initOrientation = m_SpawnPosition[ index ].transform.rotation ;
#if DEBUG_LOG			
			Debug.Log( m_SpawnPosition[ index ].name + " " + addGenData.initPosition ) ;
#endif			
			
			// 計算destination  (相對面的spawnposition)
			index = m_SpawnPosition.Count - index - 1 ;
			m_BossDestination = new NamedObject( m_SpawnPosition[ index ] ) ;
#if DEBUG_LOG						
			Debug.Log( m_SpawnPosition[ index ].name + " " + m_BossDestination ) ;
#endif			
			Vector3 pos = m_BossDestination.Obj.transform.position ;
			addGenData.supplementalVec[ "RoutePosition0" ] = 
				string.Format( "{0},{1},{2}" , pos.x , pos.y , pos.z ) ;
		}
		
		
		m_BossNow = new UnitObject( addGenData.unitName ) ;
		
		enemyGenerator.InsertEnemyGenerationTable( addGenData ) ;
		
		m_BossIsAlive = true ;
		
		PlayeAudio() ;
	}
	
	private void CheckRemoveBoss()
	{
		if( null == m_BossNow.Obj )
		{
			m_BossIsAlive = false ;
		}
		else
		{
			Vector3 bossPos = m_BossNow.Obj.transform.position ;
			Vector3 distVec = bossPos - m_BossDestination.Obj.transform.position ;
			if( distVec.magnitude < 10 )
			{
				UnitDamageSystem bossDmgSys = m_BossNow.Obj.GetComponent<UnitDamageSystem>() ;
				if( null != bossDmgSys )
				{
					bossDmgSys.m_LastAttackerName = "" ;
				}
				
				UnitData bossUnitData = m_BossNow.ObjUnitData ;
				if( null != bossUnitData )
				{
					bossUnitData.m_UnitState.state = (int) UnitState.Dead ;
				}
				
			}
		}
	}
	
	private void SetupShuffleSpawnPosEvent()
	{
#if DEBUG_LOG
		Debug.Log( "SetupShuffleSpawnPosEvent()" ) ;
#endif
		for( int i = 0 ; i < 9 ; ++i )
		{
			if( 4 == i )
				continue ;
			string Key = string.Format( "SpawnPosition{0:00}" , i ) ;
			NamedObject spawnObj = new NamedObject( Key ) ;
			m_SpawnPosition.Add( spawnObj.Obj ) ;
		}
	}
	
	private UnitGenerationData RandomFromVec( List<UnitGenerationData> _Template )
	{
		int index = Random.Range( 0 , _Template.Count ) ;
		return new UnitGenerationData( _Template[ index ] ) ;
	}
	
	private UnitGenerationData ChooseBySequence( List<UnitGenerationData> _Template )
	{
		if( 0 == _Template.Count )
			return null ;
		
		if( m_BossIndex >= _Template.Count )
			m_BossIndex = 0 ;		
		
		return new UnitGenerationData( _Template[ m_BossIndex++ ] ) ;
	}
	
	private void PlayeAudio()
	{
		m_AudioIsPlay = true ;
		m_PlayAudioStopTimer.Rewind() ;
		this.gameObject.GetComponent<AudioSource>().PlayOneShot( m_RedAlert ) ;
	}
	
	
	UnitGenerationData CreateEnemyGeneration( string _PrefabName ,
											  string _UnitDataTemplateName , 
											  string _MinimapName )
	{
		UnitGenerationData ret = null ;
		
		ret = new UnitGenerationData() ;
		
		ret.unitName = "Enemy_SpawnUnit" ;
		ret.prefabName = _PrefabName ;
		ret.unitDataTemplateName = _UnitDataTemplateName ;
		ret.raceName = _MinimapName ;	
		ret.sideName = "Enemy" ;	
		ret.supplementalVec[ "TargetName" ] = "MainCharacter" ;
		ret.supplementalVec[ "WaitForReloadSec" ] = "2.0" ;	
		
		return ret ;
	}
	
	UnitGenerationData CreateBossGeneration( string _PrefabName ,
											  string _UnitDataTemplateName , 
											  string _MinimapName )
	{
		UnitGenerationData ret = null ;
		
		ret = new UnitGenerationData() ;
		
		ret.unitName = "Netrual_SpawnBoss" ;
		ret.prefabName = _PrefabName ;
		ret.unitDataTemplateName = _UnitDataTemplateName ;
		ret.raceName = _MinimapName ;
		ret.sideName = "Nutral" ;		
		ret.supplementalVec[ "WaitForReloadSec" ] = "1.0" ;		
		ret.supplementalVec[ "RoutePosition0" ] = "50,0,20" ;
		ret.supplementalVec[ "judgeDistance" ] = "10" ;		
		
		return ret ;
	}	
	
	private void RewindBossCountDown()
	{
		m_BossGenerationTimer.Rewind() ;
		
		GameObject globalSingletonObj = GlobalSingleton.GetGlobalSingletonObj();
		if( null != globalSingletonObj )
		{
			CountDownTimeManager countDownTimeManager = globalSingletonObj.GetComponent<CountDownTimeManager>() ;
			if( null != countDownTimeManager )
			{
				countDownTimeManager.Setup( 300 ) ;				
			}
		}		
	}
}

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
@file UnitComponentData.cs
@brief 單位部件資料
@author NDark

# all components in UnitData will be the kind of type.
## UnitIntagraty
## Weapons
## Shields
## Sensor
## Engines
## Transporter
## LifeSupport
# Their attributes are...
## HP
## Generation
## Energy
## Effect
## Status
## ReloadEnergy
## ReloadGeneration
## WeaponReloadStatus
## ComponentParam
## WeaponParam
## StatusDescription
## Effect-HP Curve
# IsOffline() Is this component offline or not including ForceOffline.
# IsDown() Is this component offline or not , not includeing ForceOffline.
# SetEnergyRatio() m_Energy.now = _Ratio * m_Energy.m_Std ;
# EnergyRatio() : m_Energy.Ratio() ;
# TotalGeneration() : m_Generation.m_Now * EnergyRatio() ;
# TotalReloadGeneration() : m_ReloadGeneration.m_Now * EnergyRatio() ;
# TotalEffect() : m_Effect.m_Now * EnergyRatio() ;
# UpdateHP() Update hp and status of this component.
# UpdateReload() Update m_ReloadEnergy and reload status of this weapon component.

@date 20121111 file created.
@date 20121203 by NDark . comment.
@date 20121209 by NDark . add ParticleEmitter at UpdateHP()
@date 20121212 by NDark . add max checking at UpdateReload()
@date 20121213 by NDark 
. add force offline at UpdateHP()
. modify force offline at UpdateHP()
@date 20121226 by NDark . add class method SetEnergyRatio()
@date 20130102 by NDark . add class method IsDown()
@date 20130112 by NDark 
. comment.
. add checking of IsOffline() at UpdateReload()

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic ;


[System.Serializable]
public class UnitComponentData
{
	public string m_Name = "undefined" ;
	
	public StandardParameter m_HP = new StandardParameter() ;
	public StandardParameter m_Generation = new StandardParameter() ;		
	public StandardParameter m_Energy = new StandardParameter() ;
	public StandardParameter m_Effect = new StandardParameter() ;	
	
	private ComponentStatus m_Status = ComponentStatus.Normal ;
	public StatusDescription m_StatusDescription = new StatusDescription() ;
	public InterpolateTable m_Effect_HP_Curve = new InterpolateTable() ;
	
	// weapon only
	public StandardParameter m_ReloadEnergy = new StandardParameter() ;
	public StandardParameter m_ReloadGeneration = new StandardParameter() ;
	public WeaponReloadStatus m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Full ;
	
	// The component parameter 
	public ComponentParam m_ComponentParam = new ComponentParam() ;
	
	// The weapon parameter
	public WeaponParam m_WeaponParam = new WeaponParam() ;
	
	public UnitComponentData()
	{}
	public UnitComponentData( UnitComponentData _src )
	{
		this.m_Name = _src.m_Name ;
		this.m_HP = new StandardParameter( _src.m_HP ) ;
		this.m_Energy = new StandardParameter( _src.m_Energy ) ;
		this.m_Generation = new StandardParameter( _src.m_Generation ) ;
		this.m_Effect = new StandardParameter( _src.m_Effect ) ;
		
		this.m_Status = _src.m_Status ;
		this.m_StatusDescription = new StatusDescription( _src.m_StatusDescription ) ;
		this.m_Effect_HP_Curve = new InterpolateTable( _src.m_Effect_HP_Curve ) ;
		
		this.m_ReloadEnergy = new StandardParameter( _src.m_ReloadEnergy ) ;
		this.m_ReloadGeneration = new StandardParameter( _src.m_ReloadGeneration ) ;
		this.m_WeaponReloadStatus = _src.m_WeaponReloadStatus ;
		
		this.m_ComponentParam = new ComponentParam( _src.m_ComponentParam ) ;
		this.m_WeaponParam = new WeaponParam( _src.m_WeaponParam ) ;
	}
	
	public UnitComponentData( string _Name )
	{
		this.m_Name = _Name ;
	}
	
	// property
	public float hp
	{
		get{ return this.m_HP.now ; }
		set
		{ 
			this.m_HP.now = value ; 
			this.m_HP.CheckRange() ;
		}
	}
	public float hpMax
	{
		get
		{ 
			return this.m_HP.max ; 
		}
		set
		{
			this.m_HP.max = value ;
			this.m_HP.CheckRange() ;
		}
	}
	
	public float energy
	{
		get{ return this.m_Energy.now ; }
		set
		{
			this.m_Energy.now = value ; 
			this.m_Energy.CheckRange() ;
		}
	}
	
	public float generation
	{
		get{ return this.m_Generation.now ; }
		set
		{ 
			this.m_Generation.now = value ; 
			this.m_Generation.CheckRange() ;
		}
	}
	public ComponentStatus componentStatus
	{
		get
		{
			return m_Status ;
		}
		set
		{
			// Debug.Log( "component.m_Status = " + value ) ;
			// Debug.Log( m_Name + "=" + hp ) ;
			m_Status = value ;			
		}
	}
	
	// Is this component offline or not.
	public bool IsOffline()
	{
		return ( m_Status == ComponentStatus.Offline ||
				 m_Status == ComponentStatus.OfflineAble ||
				 m_Status == ComponentStatus.OfflineRepair ||
				 m_Status == ComponentStatus.ForceOffline ) ;
	}
	
	// Is this component offline or not.
	public bool IsDown()
	{
		return ( m_Status == ComponentStatus.Offline ||
				 m_Status == ComponentStatus.OfflineAble ||
				 m_Status == ComponentStatus.OfflineRepair ) ;
	}	
	
	public void SetEnergyRatio( float _Ratio )
	{
		m_Energy.now = _Ratio * m_Energy.m_Std ;
	}	
	public float EnergyRatio()
	{
		return m_Energy.Ratio() ;
	}
	
	public float TotalGeneration()
	{
		return m_Generation.m_Now * EnergyRatio() ;
	}
	
	public float TotalReloadGeneration()
	{
		return m_ReloadGeneration.m_Now * EnergyRatio() ;
	}	
	
	public float TotalEffect()
	{
		return m_Effect.m_Now * EnergyRatio() ;
	}
	
	public void UpdateHP()
	{
		GameObject component3DObj = null ;
		if( 0 != this.m_ComponentParam.m_Component3DObject.Name.Length )
			component3DObj = this.m_ComponentParam.m_Component3DObject.Obj ;
			
		float hpnow = this.hp ;

		
		switch( this.componentStatus )
		{
		case ComponentStatus.Normal :
		case ComponentStatus.Holding :
		case ComponentStatus.Danger :
			if( this.energy == 0 && this.m_Energy.max != 0 ) 
			{
				// 強制下線
				if( null != component3DObj )
				{
					// Debug.Log( "ForceOffline=" + component3DObj.name ) ;
					component3DObj.GetComponent<Collider>().enabled = false ;				
					ParticleEmitter emitter = component3DObj.GetComponentInChildren<ParticleEmitter>() ;
					if( null != emitter )
					{
						emitter.emit = true ;
					}
				}
				this.componentStatus = ComponentStatus.ForceOffline ;			
			}
			else if( this.m_HP.max == hpnow )
			{
				this.componentStatus = ComponentStatus.Normal ;
			}
			else if( this.m_HP.min == hpnow )
			{
				// go offline
				this.componentStatus = ComponentStatus.Offline ;
			}
			else if( this.m_HP.max != hpnow &&
					 this.m_HP.Ratio() > 0.2f )
			{
				this.componentStatus = ComponentStatus.Holding ;
			}
			else if( this.m_HP.max != hpnow &&
					 this.m_HP.Ratio() <= 0.2f )
			{
				this.componentStatus = ComponentStatus.Danger ;
			}

			break ;
			
		case ComponentStatus.ForceOffline :			
			
			if( this.energy != 0 ) 
			{
				// Debug.Log( "this.energy != 0 " + this.m_Name ) ;
				this.componentStatus = ComponentStatus.Online ;
			}			
			
			break ;
			
		case ComponentStatus.Offline :
			// 強制下線
			if( null != component3DObj )
			{
				component3DObj.GetComponent<Collider>().enabled = false ;				
				ParticleEmitter emitter = component3DObj.GetComponentInChildren<ParticleEmitter>() ;
				if( null != emitter )
				{
					emitter.emit = true ;
				}
			}
			this.componentStatus = ComponentStatus.OfflineRepair ;
			break ;
		case ComponentStatus.OfflineRepair :
		case ComponentStatus.OfflineAble :
			if( this.m_HP.max == hpnow )
			{
				// go online
				this.componentStatus = ComponentStatus.Online ;
			}
			break ;	
		case ComponentStatus.Online :
			// 強迫上線
			
			if( null != component3DObj )
			{
				// Debug.Log( "Online=" + component3DObj.name ) ;
				component3DObj.GetComponent<Collider>().enabled = true ;
				ParticleEmitter emitter = component3DObj.GetComponentInChildren<ParticleEmitter>() ;
				if( null != emitter )
				{
					emitter.emit = false ;
				}
			}
			this.componentStatus = ComponentStatus.Normal ;
			break ;
		}

		// 先切換狀態再回復 避免永遠不會下線
		// generation to hp
		float totalGenerationOfSec = this.TotalGeneration() ;
		this.hp += ( totalGenerationOfSec * Time.deltaTime ) ;
		
	}
	public void UpdateReload()
	{
		// weapon hp update
		if( 0 == this.m_ReloadEnergy.max )
			return ;
		
		// check offline
		if( true == this.IsOffline() )
			return ;
		
		switch( this.m_WeaponReloadStatus )
		{
		case WeaponReloadStatus.WeaponReloadStatus_Full :
			if( this.m_ReloadEnergy.now == this.m_ReloadEnergy.min )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Empty ;				
			else if( this.m_ReloadEnergy.now != this.m_ReloadEnergy.max )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Reload ;
			break ;
		case WeaponReloadStatus.WeaponReloadStatus_Empty :
			if( this.m_ReloadEnergy.now == this.m_ReloadEnergy.max )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Full ;
			else if( this.m_ReloadEnergy.now != this.m_ReloadEnergy.min )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Reload ;				
			break ;
		case WeaponReloadStatus.WeaponReloadStatus_Reload :
			if( this.m_ReloadEnergy.now == this.m_ReloadEnergy.max )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Full ;
			else if( this.m_ReloadEnergy.now == this.m_ReloadEnergy.min )
				this.m_WeaponReloadStatus = WeaponReloadStatus.WeaponReloadStatus_Empty ;					
			break ;				
		}	
		
		
		// generation to reload
		float totalGenerationOfSec = this.TotalReloadGeneration() ;
		// Debug.Log( this.m_Name + "totalGenerationOfSec" + TotalReloadGeneration() ) ;
		this.m_ReloadEnergy.now += ( totalGenerationOfSec * Time.deltaTime ) ;			

	}	
}



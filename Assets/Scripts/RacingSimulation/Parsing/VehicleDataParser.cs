using System.Xml;
using UnityEngine;
using RacingSimulation.Data;
using System;

namespace RacingSimulation.Parsing
{
    public static class VehicleDataParser
    {
        public static ClutchData ParseClutchData(XmlElement data)
        {
            float clutchDamping = 0f, clutchTorqueCapacity = 0f, clutchStiffness = 0f;
            foreach (var childNode in data.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;
                switch (element.Name)
                {
                    case "ClutchDamping":
                        clutchDamping = float.Parse(element.InnerText);
                        break;
                    case "ClutchTorqueCapacity":
                        clutchTorqueCapacity = float.Parse(element.InnerText);
                        break;
                    case "ClutchStiffness":
                        clutchStiffness = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }

            }
            ClutchData clutchData = ScriptableObject.CreateInstance<ClutchData>();
            clutchData.Initialize(clutchDamping, clutchTorqueCapacity, clutchStiffness);
            return clutchData;
        }
        
        public static EngineData ParseEngineData(XmlElement data)
        {
            float idleThrottle = 0f, throttleSensitivity = 0f, idleRpm = 0f, redlineRpm = 0f, throttleCutoffDuration = 0f, starterTorque = 0f, initialFrictionTorque = 0f, frictionLossCoefficient = 0f, inertia =0f;
            foreach (var childNode in data.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;
                switch (element.Name)
                {
                    case "IdleThrottle":
                        idleThrottle = float.Parse(element.InnerText);
                        break;
                    case "ThrottleSensitivity":
                        throttleSensitivity = float.Parse(element.InnerText);
                        break;
                    case "IdleRpm":
                        idleRpm = float.Parse(element.InnerText);
                        break;
                    case "RedlineRpm":
                        redlineRpm = float.Parse(element.InnerText);
                        break;
                    case "ThrottleCutoffDuration":
                        throttleCutoffDuration = float.Parse(element.InnerText);
                        break;
                    case "StarterTorque":
                        starterTorque = float.Parse(element.InnerText);
                        break;
                    case "InitialFrictionTorque":
                        initialFrictionTorque = float.Parse(element.InnerText);
                        break;
                    case "FrictionLossCoefficient":
                        frictionLossCoefficient = float.Parse(element.InnerText);
                        break;
                    case "Inertia":
                        inertia = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }

            }
            EngineData engineData = ScriptableObject.CreateInstance<EngineData>();
            engineData.Init(idleThrottle, throttleSensitivity, idleRpm, redlineRpm, throttleCutoffDuration, starterTorque, initialFrictionTorque, frictionLossCoefficient, inertia);
            return engineData;
        }
        
        public static GearboxData ParseGearboxData(XmlElement data)
        {
            float[] gearRatios = new float[8];
            float shiftDuration = 0f;
            int gearCount = 0;
            foreach (var childNode in data.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;
                switch (element.Name)
                {
                    case "GearRatios":
                        gearRatios[gearCount++] = float.Parse(element.InnerText);
                        break;
                    case "ShiftDuration":
                        shiftDuration = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }

            }
            GearboxData gearboxData = ScriptableObject.CreateInstance<GearboxData>();
            gearboxData.Initialize(gearRatios, shiftDuration);
            return gearboxData;
        }

        public static SteeringData ParseSteeringData(XmlElement data)
        {
            Define.SteeringBehavior steeringBehavior = Define.SteeringBehavior.Disabled;
            float wheelbase = 0f, rearTrackLength = 0f, turningRadius = 0f;
            foreach (var childNode in data.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;
                switch (element.Name)
                {
                    case "SteeringBehavior":
                        steeringBehavior = (Define.SteeringBehavior)Enum.Parse(typeof(Define.SteeringBehavior), element.InnerText, true);
                        break;
                    case "Wheelbase":
                        wheelbase = float.Parse(element.InnerText);
                        break;
                    case "RearTrackLength":
                        rearTrackLength = float.Parse(element.InnerText);
                        break;
                    case "TurningRadius":
                        turningRadius = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }

            }
            SteeringData steeringData = ScriptableObject.CreateInstance<SteeringData>();
            steeringData.Initialize(steeringBehavior, wheelbase, rearTrackLength, turningRadius);
            return steeringData;            
        }
        
        public static VehicleData ParseVehicleData(XmlDocument data)
        {
            EngineData engine = null;
            ClutchData clutch = null;
            GearboxData gearbox = null;
            WheelData[] wheels = new WheelData[4];
            SteeringData[] steerings = new SteeringData[4];
            int wheelcount = 0, steeringcount = 0;
            float throttleSensitivity = 0f, brakeSensitivity = 0f, clutchSensitivity = 0f, steeringSensitivity = 0f, finalDriveRatio = 0f;

            XmlElement nodes = data["Vehicle"];

            foreach (var childNode in nodes.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;

                switch (element.Name)
                {
                    case "Engine":
                        engine = ParseEngineData(element);
                        break;
                    case "Clutch":
                        clutch = ParseClutchData(element);
                        break;
                    case "Gearbox":
                        gearbox = ParseGearboxData(element);
                        break;
                    case "Wheel":
                        wheels[wheelcount++] = ParseWheelData(element);
                        break;
                    case "Steering":
                        steerings[steeringcount++] = ParseSteeringData(element);
                        break;
                    case "ThrottleSensitivity":
                        throttleSensitivity = float.Parse(element.InnerText);
                        break;
                    case "BrakeSensitivity":
                        brakeSensitivity = float.Parse(element.InnerText); 
                        break;
                    case "ClutchSensitivity":
                        clutchSensitivity = float.Parse(element.InnerText);
                        break;
                    case "SteeringSensitivity":
                        steeringSensitivity = float.Parse(element.InnerText);
                        break;
                    case "FinalDriveRatio":
                        finalDriveRatio = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }
            }

            VehicleData vehicleData = ScriptableObject.CreateInstance<VehicleData>();
            vehicleData.Initialize(engine, clutch, gearbox, wheels, steerings, throttleSensitivity, brakeSensitivity, clutchSensitivity, steeringSensitivity, finalDriveRatio);

            return vehicleData;
        }

        public static WheelData ParseWheelData(XmlElement data)
        {
            LayerMask layerMask = 0;
            float restLength = 0f, springStiffness = 0f, damperStiffness = 0f, wheelRadius = 0f, wheelInertia = 0f, maxBrakeTorque = 0f;
            foreach (var childNode in data.ChildNodes)
            {
                XmlElement element = childNode as XmlElement;
                switch (element.Name)
                {
                    case "LayerMask":
                        layerMask = int.Parse(element.InnerText);
                        break;
                    case "RestLength":
                        restLength = float.Parse(element.InnerText);
                        break;
                    case "SpringStiffness":
                        springStiffness = float.Parse(element.InnerText);
                        break;
                    case "DamperStiffness":
                        damperStiffness = float.Parse(element.InnerText);
                        break;
                    case "WheelRadius":
                        wheelRadius = float.Parse(element.InnerText);
                        break;
                    case "WheelInertia":
                        wheelInertia = float.Parse(element.InnerText);
                        break;
                    case "MaxBrakeTorque":
                        maxBrakeTorque = float.Parse(element.InnerText);
                        break;
                    default:
                        Debug.Log(element.Name + " not recognized");
                        break;
                }

            }
            WheelData wheelData = ScriptableObject.CreateInstance<WheelData>();
            wheelData.Initialize(layerMask, restLength, springStiffness, damperStiffness, wheelRadius, wheelInertia, maxBrakeTorque);
            return wheelData;
        }
    }
}
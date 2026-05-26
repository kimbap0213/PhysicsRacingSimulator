using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using RacingSimulation.Data;

namespace RacingSimulation.Utils
{
    public static class DataParserUtil
    {
        // 1. JSON 파싱
        public static VehicleParameters ParseJSON(string jsonContent)
        {
            try
            {
                return JsonUtility.FromJson<VehicleParameters>(jsonContent);
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON 파싱 오류: {e.Message}");
                return null;
            }
        }

        // 2. XML 파싱
        public static VehicleParameters ParseXML(string xmlContent)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VehicleParameters));
                using (StringReader reader = new StringReader(xmlContent))
                {
                    return (VehicleParameters)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"XML 파싱 오류: {e.Message}");
                return null;
            }
        }

        // 3. CSV 파싱 (헤더: Mass, CenterOfGravityY, FinalDriveRatio, GripMultiplier)
        public static VehicleParameters ParseCSV(string csvContent)
        {
            try
            {
                VehicleParameters parameters = new VehicleParameters();
                string[] lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length >= 2) // 헤더(0)와 데이터(1) 라인이 있어야 함
                {
                    string[] headers = lines[0].Split(',');
                    string[] values = lines[1].Split(',');

                    for (int i = 0; i < headers.Length; i++)
                    {
                        string header = headers[i].Trim();
                        float value = float.Parse(values[i].Trim());

                        switch (header)
                        {
                            case "Mass": parameters.Mass = value; break;
                            case "CenterOfGravityY": parameters.CenterOfGravityY = value; break;
                            case "FinalDriveRatio": parameters.FinalDriveRatio = value; break;
                            case "GripMultiplier": parameters.GripMultiplier = value; break;
                        }
                    }
                }
                return parameters;
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV 파싱 오류: {e.Message}");
                return null;
            }
        }
    }
}
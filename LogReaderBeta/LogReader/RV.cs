using System.Collections.Generic;

namespace LogReader
{
    public class RV : SRID
    {
        public RV()
        {

        }

        public RV(List<SRID> sRIDLIST,
            string rVNumber,
            string mBNumber,
            List<SatteliteInfo> satelliteInfoList,
            List<DeviceInfo> deviceInfoList,
            List<MB> mBInfoList,
            List<MK> mKInfoList,
            List<Emission_server_settings> emission_Server_SettingsList,
            List<Registration_server_settings> registration_Server_SettingsList,
            string rVLocation,
            long telegramID)
        {
            SRIDLIST = sRIDLIST;
            RVNumber = rVNumber;
            MBNumber = mBNumber;
            SatelliteInfoList = satelliteInfoList;
            DeviceInfoList = deviceInfoList;
            MBInfoList = mBInfoList;
            MKInfoList = mKInfoList;
            Emission_Server_SettingsList = emission_Server_SettingsList;
            Registration_Server_SettingsList = registration_Server_SettingsList;
            RVLocation = rVLocation;
            TelegramID = telegramID;
        }

        /// <summary>
        /// SRID[]
        /// </summary>
        public List<SRID> SRIDLIST { get; set; }
        /// <summary>
        /// RVNumber
        /// </summary>
        public string RVNumber { get; set; }
        /// <summary>
        /// MBNumber
        /// </summary>
        public string MBNumber { get; set; }
        /// <summary>
        /// Инфо по спутникам
        /// </summary>
        public List<SatteliteInfo> SatelliteInfoList { get; set; }
        /// <summary>
        /// Инфо об устройстве
        /// </summary>
        public List<DeviceInfo> DeviceInfoList { get; set; }
        /// <summary>
        /// Инфо об МБ
        /// </summary>
        public List<MB> MBInfoList { get; set; }
        /// <summary>
        /// Инфо об МК
        /// </summary>
        public List<MK> MKInfoList { get; set; }
        /// <summary>
        /// Инфо о параметрах сервера эмиссии
        /// </summary>
        public List<Emission_server_settings> Emission_Server_SettingsList { get; set; }
        /// <summary>
        /// Инфо о параметрах сервера регистрации
        /// </summary>
        public List<Registration_server_settings> Registration_Server_SettingsList { get; set; }
        /// <summary>
        /// Локация использования РВ
        /// </summary>
        public string RVLocation { get; set; }
        /// <summary>
        /// ID пользователя при телеграм запросе
        /// </summary>
        public long TelegramID { get; set; }
    }



}

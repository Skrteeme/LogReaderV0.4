using System;

namespace LogReader
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {

        }

        public DeviceInfo(DateTime dateTime,
        string sERIAL_NUMBER,
        string mODEL_NAME,
        string mODEL,
        string fIRMWARE_VERSION,
        string aPP_VERSION,
        string bOOT_VERSION,
        string cONTROL_UNIT_VERSION,
        string rTC_VOLTAGE,
        string sOURCE_OF_POWER_VOLTAGE)
        
        {
            DateTime = dateTime;
            SERIAL_NUMBER = sERIAL_NUMBER;
            MODEL_NAME = mODEL_NAME;
            MODEL = mODEL;
            FIRMWARE_VERSION = fIRMWARE_VERSION;
            APP_VERSION = aPP_VERSION;
            BOOT_VERSION = bOOT_VERSION;
            CONTROL_UNIT_VERSION = cONTROL_UNIT_VERSION;
            RTC_VOLTAGE = rTC_VOLTAGE;
            SOURCE_OF_POWER_VOLTAGE = sOURCE_OF_POWER_VOLTAGE;
            
        }
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        public string SERIAL_NUMBER { get; set; }
        /// <summary>
        /// Модель устройства
        /// </summary>
        public string MODEL_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MODEL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FIRMWARE_VERSION { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string APP_VERSION { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOOT_VERSION { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CONTROL_UNIT_VERSION { get; set; }
        /// <summary>
        /// Напряжение на батарейке CR2032
        /// </summary>
        public string RTC_VOLTAGE { get; set; }
        /// <summary>
        /// Напряжение на аккумуляторе
        /// </summary>
        public string SOURCE_OF_POWER_VOLTAGE { get; set; }
        

        



    }


}

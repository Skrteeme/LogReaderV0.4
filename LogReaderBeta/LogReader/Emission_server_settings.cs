using System;

namespace LogReader
{
    public class Emission_server_settings
    {
        public Emission_server_settings()
        {

        }
        /// <summary>
        /// Класс параметры сервера эмиссии
        /// </summary>
        /// <param name="sGTINNumber"></param>
        /// <param name="originalSGTINNumber"></param>
        public Emission_server_settings(DateTime dateTime,
            string pARAM_HOST,
            string pARAM_PORT)
        {
            DateTime = dateTime;
            PARAM_HOST = pARAM_HOST;
            PARAM_PORT = pARAM_PORT;
        }

        /// <summary>
        /// Дата лога
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Адрес сервера эмиссии
        /// </summary>
        public string PARAM_HOST { get; set; }

        /// <summary>
        /// Порт сервера эмиссии
        /// </summary>
        public string PARAM_PORT { get; set; }
    }
}

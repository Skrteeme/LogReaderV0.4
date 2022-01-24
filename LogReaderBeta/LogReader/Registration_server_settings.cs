using System;

namespace LogReader
{
    public class Registration_server_settings
    {
        public Registration_server_settings()
        {

        }
        /// <summary>
        /// Класс Параметры сервера регистрации
        /// </summary>
        /// <param name="sGTINNumber"></param>
        /// <param name="originalSGTINNumber"></param>
        public Registration_server_settings(DateTime dateTime, 
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
        /// Адрес сервера регистрации
        /// </summary>
        public string PARAM_HOST { get; set; }

        /// <summary>
        /// Порт сервера регистрации
        /// </summary>
        public string PARAM_PORT { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class MK
    {
        /// <summary>
        /// Запросить состояние МК
        /// </summary>
        public MK()
        {

        }

        public MK(DateTime date,
            string iNITIALIZED,
            string mARK_CHECK_STARTED,
            string dISPOSAL_REPORT_IN_PROGRESS,
            string sU_EXCHANGE_SUSPENDED,
            string rECEIVED_DOCUMENTS_COUNT,
            string pROCESSED_DOCUMENTS_COUNT,
            string qUEUE_STATUS,
            string sERVER_AUTH_STATE,
            string pOOL_PROCESSING_STATE,
            string sU_IS_SLEEPING,
            string sU_POWER_IS_SUPPLIED_DURING_SLEEP,
            string nO_SERIAL_NUMBER,
            string rTC_FAULT,
            string sETTINGS_FAULT,
            string cOUNTERS_FAULT,
            string aTTRIBUTES_FAULT,
            string sU_FAULT,
            string iNVALID_SU,
            string hARD_FAULT,
            string mEMORY_MANAGER_FAULT,
            string wAIT_FOR_REBOOT,
            string sECURITY_SYSTEM_AUTH_FAULT,
            string sECURITY_SYSTEM_FAULT,
            string sU_STATE
            )
        {
            Date = date;
            INITIALIZED = iNITIALIZED;
            MARK_CHECK_STARTED = mARK_CHECK_STARTED;
            DISPOSAL_REPORT_IN_PROGRESS = dISPOSAL_REPORT_IN_PROGRESS;
            SU_EXCHANGE_SUSPENDED = sU_EXCHANGE_SUSPENDED;
            RECEIVED_DOCUMENTS_COUNT = rECEIVED_DOCUMENTS_COUNT;
            PROCESSED_DOCUMENTS_COUNT = pROCESSED_DOCUMENTS_COUNT;
            QUEUE_STATUS = qUEUE_STATUS;
            SERVER_AUTH_STATE = sERVER_AUTH_STATE;
            POOL_PROCESSING_STATE = pOOL_PROCESSING_STATE;
            SU_IS_SLEEPING = sU_IS_SLEEPING;
            SU_POWER_IS_SUPPLIED_DURING_SLEEP = sU_POWER_IS_SUPPLIED_DURING_SLEEP;
            NO_SERIAL_NUMBER = nO_SERIAL_NUMBER;
            RTC_FAULT = rTC_FAULT;
            SETTINGS_FAULT = sETTINGS_FAULT;
            COUNTERS_FAULT = cOUNTERS_FAULT;
            ATTRIBUTES_FAULT = aTTRIBUTES_FAULT;
            SU_FAULT = sU_FAULT;
            INVALID_SU = iNVALID_SU;
            HARD_FAULT = hARD_FAULT;
            MEMORY_MANAGER_FAULT = mEMORY_MANAGER_FAULT;
            WAIT_FOR_REBOOT = wAIT_FOR_REBOOT;
            SECURITY_SYSTEM_AUTH_FAULT = sECURITY_SYSTEM_AUTH_FAULT;
            SECURITY_SYSTEM_FAULT = sECURITY_SYSTEM_FAULT;
            SU_STATE = sU_STATE;
        }

        /// <summary>
        /// Дата лога
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Состояния процесса инициализации РВ
        /// 0 Бит: Инициализация: 0 – РВ инициализирован; 1 – РВ не инициализирован.
        /// 1 Бит: Ошибка инициализации РВ: 0 – нет ошибок; 1 – ошибка инициализации.
        /// </summary>
        public string INITIALIZED { get; set; }
        /// <summary>
        /// Состояние процесса проверки КМ
        /// 0 – не запущен процесс проверки КМ;
        /// 1 – запущен процесс проверки КМ.
        /// </summary>
        public string MARK_CHECK_STARTED { get; set; }
        /// <summary>
        /// Состояние процесса регистрации отчета о выбытии
        /// 0 – не запущен процесс регистрации отчета о выбытии;
        /// 1 – запущен процесс регистрации отчета о выбытии.
        /// </summary>
        public string DISPOSAL_REPORT_IN_PROGRESS { get; set; }
        /// <summary>
        /// Флаг обмена с СЭ
        /// 0 – обмен с СЭ осуществляется;
        /// 1 – обмен с СЭ приостановлен.
        /// * При отсутствии связи с СЭ в течении 5 суток автоматически переходит в состояние 1.
        /// </summary>
        public string SU_EXCHANGE_SUSPENDED { get; set; }
        /// <summary>
        /// Количество полученных документов для обработки в МБ РВ
        /// </summary>
        public string RECEIVED_DOCUMENTS_COUNT { get; set; }
        /// <summary>
        /// Количество обработанных и отправленных документов из МБ РВ
        /// </summary>
        public string PROCESSED_DOCUMENTS_COUNT { get; set; }
        /// <summary>
        /// Флаг отправки документов из очереди правка очереди
        /// 0 – не запущен процесс отправки документов из очереди.
        /// 1 - запущен процесс отправки документов из очереди.
        /// </summary>
        public string QUEUE_STATUS { get; set; }
        /// <summary>
        /// Флаг процесса аутентификации с сервером
        /// 0 - не запущен процесс аутентификации с сервером;
        /// 1 - запущен процесс аутентификации с сервером.
        /// </summary>
        public string SERVER_AUTH_STATE { get; set; }
        /// <summary>
        /// Процесс обработки пула КМ
        /// Всегда возвращается 0.
        /// </summary>
        public string POOL_PROCESSING_STATE { get; set; }
        /// <summary>
        /// Флаг нахождения МБ в спящем режиме
        /// 0 - МБ активен.
        /// 1 - МБ находится в спящем режиме.
        /// </summary>
        public string SU_IS_SLEEPING { get; set; }
        /// <summary>
        /// Флаг подачи питания на МБ в спящем режиме.
        /// 0 - питание не подается;
        /// 1 - питание подается.
        /// </summary>
        public string SU_POWER_IS_SUPPLIED_DURING_SLEEP { get; set; }
        /// <summary>
        /// Серийный номер не введен
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string NO_SERIAL_NUMBER { get; set; }
        /// <summary>
        /// Ошибка часов реального времени
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string RTC_FAULT { get; set; }
        /// <summary>
        /// Ошибка таблицы настроек
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string SETTINGS_FAULT { get; set; }
        /// <summary>
        /// Ошибка регистров и счетчиков
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string COUNTERS_FAULT { get; set; }
        /// <summary>
        /// Ошибка реквизитов
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string ATTRIBUTES_FAULT { get; set; }
        /// <summary>
        /// Фатальная ошибка МБ
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string SU_FAULT { get; set; }
        /// <summary>
        /// Установленный МБ зарегистрирован в другом устройстве
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string INVALID_SU { get; set; }
        /// <summary>
        /// Фатальная аппаратная ошибка
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string HARD_FAULT { get; set; }
        /// <summary>
        /// Ошибка диспетчера памяти
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string MEMORY_MANAGER_FAULT { get; set; }
        /// <summary>
        /// Ожидание перезагрузки
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string WAIT_FOR_REBOOT { get; set; }
        /// <summary>
        /// Ошибка проверки в системе безопасности
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string SECURITY_SYSTEM_AUTH_FAULT { get; set; }
        /// <summary>
        /// Сбой системы безопасности
        /// 0 - нет ошибки
        /// 1 - ошибка.
        /// </summary>
        public string SECURITY_SYSTEM_FAULT { get; set; }
        /// <summary>
        /// Состояние инициализации МБ РВ
        /// Номер ошибки
        /// 0х00 - не инициализирован;
        /// 0x01 – проведена инициализация;
        /// 0x02 – данные регистрации загружены;
        /// 0x03 – сертификат загружен;
        /// 0x04 – активирован;
        /// 0x05 – истек срок использования МБ РВ;
        /// 0x06 – ключ подписи блокирован;
        /// 0x07 – не функционален;
        /// 0xFF – МБ РВ отсутствует.
        /// </summary>
        public string SU_STATE { get; set; }
    }
}


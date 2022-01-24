using System;

namespace LogReader
{
    public class MB
    {
        /// <summary>
        /// Запросить состояние МБ РВ
        /// </summary>
        public MB()
        {

        }

        public MB(DateTime date,
            string sU_STATE,
            string sU_SUSPENDED,
            string sERVER_AUTH_STATE,
            string uSER_AUTH_STATE,
            string qUEUE_STATUS,
            string dATE_TIME,
            string gNSS_READY,
            string cC_READY,
            string sU_ARCHIVE_READY,
            string sU_READY,
            string pRIVILEGES_PRESENTED,
            string sS_AUTH_READY,
            string sS_AUTH_SUCCESS,
            string sS_INIT_READY,
            string sU_RESOURCE,
            string cOMMAND_CODE,
            string sU_REBOOT_COUNTER,
            string sU_LAST_REBOOT_TIME,
            string sU_REBOOT_REASON,
            string sU_VERSION)
        {
            Date = date;
            SU_STATE = sU_STATE;
            SU_SUSPENDED = sU_SUSPENDED;
            SERVER_AUTH_STATE = sERVER_AUTH_STATE;
            USER_AUTH_STATE = uSER_AUTH_STATE;
            QUEUE_STATUS = qUEUE_STATUS;
            DATE_TIME = dATE_TIME;
            GNSS_READY = gNSS_READY;
            CC_READY = cC_READY;
            SU_ARCHIVE_READY = sU_ARCHIVE_READY;
            SU_READY = sU_READY;
            PRIVILEGES_PRESENTED = pRIVILEGES_PRESENTED;
            SS_AUTH_READY = sS_AUTH_READY;
            SS_AUTH_SUCCESS = sS_AUTH_SUCCESS;
            SS_INIT_READY = sS_INIT_READY;
            SU_RESOURCE = sU_RESOURCE;
            COMMAND_CODE = cOMMAND_CODE;
            SU_REBOOT_COUNTER = sU_REBOOT_COUNTER;
            SU_LAST_REBOOT_TIME = sU_LAST_REBOOT_TIME;
            SU_REBOOT_REASON = sU_REBOOT_REASON;
            SU_VERSION = sU_VERSION;
        }

        /// <summary>
        /// Дата лога
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Состояние МБ РВ
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
        /// <summary>
        /// Флаг остановки выполнения основных функций МБ РВ по причине отсутствия связи со спутником
        /// 0 – основные функции МБ РВ доступны;
        /// 1 – основные функции МБ РВ приостановлены.
        /// </summary>
        public string SU_SUSPENDED { get; set; }
        /// <summary>
        /// Флаг аутентификации с сервером
        /// 0 - аутентификация не проведена;
        /// 1 - аутентификация проведена.
        /// </summary>
        public string SERVER_AUTH_STATE { get; set; }
        /// <summary>
        /// Флаг аутентификации с пользователем:
        /// 0 - аутентификация не проведена;
        /// 1 - аутентификация проведена.
        /// </summary>
        public string USER_AUTH_STATE { get; set; }
        /// <summary>
        /// Флаг наличия документов в очереди
        /// 0 - нет данных в очереди;
        /// 1 - есть данные в очереди.
        /// </summary>
        public string QUEUE_STATUS { get; set; }
        /// <summary>
        /// Дата окончания срока службы МБ
        /// </summary>
        public string DATE_TIME { get; set; }
        /// <summary>
        /// Флаг готовности ГНСС
        /// 0 - ГНСС не готова;
        /// 1 - ГНСС готова.
        /// </summary>
        public string GNSS_READY { get; set; }
        /// <summary>
        /// Флаг готовности криптосопроцессора
        /// 0 - КС не готов;
        /// 1 - КС готов.
        /// </summary>
        public string CC_READY { get; set; }
        /// <summary>
        /// Флаг готовности архива МБ РВ
        /// 0 - архив МБ РВ не готов;
        /// 1 - архив МБ РВ готов.
        /// </summary>
        public string SU_ARCHIVE_READY { get; set; }
        /// <summary>
        /// Флаг готовности МБ РВ
        /// 0 - МБ РВ не готов;
        /// 1 - МБ РВ готов.
        /// </summary>
        public string SU_READY { get; set; }
        /// <summary>
        /// Флаг подтверждения административных привилегий
        /// 0 - административные привилегии доступа не предъявлены
        /// 1 - предъявлены административные привилегии доступа.
        /// </summary>
        public string PRIVILEGES_PRESENTED { get; set; }
        /// <summary>
        /// Флаг проведения аутентификации МБ
        /// 0 - аутентификация не проведена;
        /// 1 - аутентификация проведена.
        /// </summary>
        public string SS_AUTH_READY { get; set; }
        /// <summary>
        /// Флаг успешности проведения аутентификации МБ
        /// 0 - аутентификация не успешна или не проведена;
        /// 1 - аутентификация проведена успешно
        /// </summary>
        public string SS_AUTH_SUCCESS { get; set; }
        /// <summary>
        /// Флаг проведения инициализации МБ
        /// 0 - инициализация не проведена;
        /// 1 - инициализация проведена.
        /// </summary>
        public string SS_INIT_READY { get; set; }
        /// <summary>
        /// Процент использованного ресурса накопителя МБ РВ
        /// </summary>
        public string SU_RESOURCE { get; set; }
        /// <summary>
        /// Код команды с превышением буфера
        /// </summary>
        public string COMMAND_CODE { get; set; }
        /// <summary>
        /// Счетчик перезагрузок МБ
        /// </summary>
        public string SU_REBOOT_COUNTER { get; set; }
        /// <summary>
        /// Время последней перезагрузки МБ
        /// </summary>
        public string SU_LAST_REBOOT_TIME { get; set; }
        /// <summary>
        /// Причина перезагрузки МБ
        /// </summary>
        public string SU_REBOOT_REASON { get; set; }
        /// <summary>
        /// Версия прошивки МБ
        /// </summary>
        public string SU_VERSION { get; set; }
    }
}

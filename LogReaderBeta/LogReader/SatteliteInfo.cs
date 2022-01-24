using System;

namespace LogReader
{
    public class SatteliteInfo
    {
        public SatteliteInfo()
        {

        }

        public SatteliteInfo(DateTime dateTime,
        bool sATELLITES_FOUND,
        DateTime dATE_TIME,
        string sPEED,
        string eLEVATION,
        string lATITUDE,
        string lONGITUDE,
        string hDOP,
        string vDOP,
        string sATELLITES_COUNT,
        string aZIMUTH,
        string gNSS_STATUS,
        string sTANDART_DEVIATION,
        string cARRIER_TO_NOISE_RATION)
        {
            DateTime = dateTime;
            SATELLITES_FOUND = sATELLITES_FOUND;
            DATE_TIME = dATE_TIME;
            SPEED = sPEED;
            ELEVATION = eLEVATION;
            LATITUDE = lATITUDE;
            LONGITUDE = lONGITUDE;
            HDOP = hDOP;
            VDOP = vDOP;
            SATELLITES_COUNT = sATELLITES_COUNT;
            AZIMUTH = aZIMUTH;
            GNSS_STATUS = gNSS_STATUS;
            STANDART_DEVIATION = sTANDART_DEVIATION;
            CARRIER_TO_NOISE_RATION = cARRIER_TO_NOISE_RATION;
        }

        /// <summary>
        /// Дата лога
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Флаг успешного решения навигационной задачи МБ РВ, 0 – передается время, количество спутников, статус антенны, стандартное отклонение и среднее соотношение сигнал-шум, остальные параметры нулевые, 
        ///1 – передаются все параметры.
        /// </summary>
        public bool SATELLITES_FOUND { get; set; }
        /// <summary>
        /// Дата и время по данным модуля ГНСС
        /// </summary>
        public DateTime DATE_TIME { get; set; }
        /// <summary>
        /// Скорость в км/ч
        /// </summary>
        public string SPEED { get; set; }
        /// <summary>
        /// Высота над уровнем моря в метрах
        /// </summary>
        public string ELEVATION { get; set; }
        /// <summary>
        /// Широта в градусах
        /// </summary>
        public string LATITUDE { get; set; }
        /// <summary>
        /// Долгота в градусах
        /// </summary>
        public string LONGITUDE { get; set; }
        /// <summary>
        /// Значение фактора ухудшения точности в плане, Старшая тетрада - целая часть, младшая тетрада - дробная часть.
        /// </summary>
        public string HDOP { get; set; }
        /// <summary>
        /// Значение фактора ухудшения точности по высоте, Старшая тетрада - целая часть, младшая тетрада - дробная часть.
        /// </summary>
        public string VDOP { get; set; }
        /// <summary>
        /// Количество спутников
        /// </summary>
        public string SATELLITES_COUNT { get; set; }
        /// <summary>
        /// Курсовой угол в градусах, Цена младшего разряда – 1 градус.
        /// </summary>
        public string AZIMUTH { get; set; }
        /// <summary>
        /// Состояние ВЧ тракта приёмника ГЛОНАСС, Битовое поле:
        ///Биты 0, 1 – резерв;
        ///Биты 2,3:
        ///00 – состояние антенны не определялось;
        ///01 – короткое замыкание;
        ///10 – антенна не подсоединена;
        ///11 – антенна исправно работает;
        ///Биты 4-7 – резерв.
        /// </summary>
        public string GNSS_STATUS { get; set; }
        /// <summary>
        /// Стандартное отклонение, Если значение параметра больше 255, то оно устанавливается в значение 255.
        /// </summary>
        public string STANDART_DEVIATION { get; set; }
        /// <summary>
        /// Среднее соотношение сигнал-шум, Если значение параметра больше 255, то оно устанавливается в значение 255.
        /// </summary>
        public string CARRIER_TO_NOISE_RATION { get; set; }


        


    }

    
}

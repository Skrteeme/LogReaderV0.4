using System;
using System.Collections.Generic;

namespace LogReader
{
    public class SRID : SGTIN
    {
        
        public SRID()
        {

        }
        
        /// <summary>
        /// Class SRID
        /// </summary>
        /// <param name="sGTINLIST"></param>
        /// <param name="sRID"></param>
        /// <param name="sUID"></param>
        /// <param name="dONM"></param>
        /// <param name="dODT"></param>
        /// <param name="dOSE"></param>
        /// <param name="disposalStartDate"></param>
        /// <param name="disposalEndDate"></param>
        /// <param name="registrationServerIP"></param>
        /// <param name="registrationServerPort"></param>
        /// <param name="emissionServerIP"></param>
        /// <param name="emissionServerPort"></param>
        /// <param name="disposalReport"></param>
        /// <param name="reportNumber"></param>
        /// <param name="satellitesFound"></param>
        /// <param name="lastSatellitesFoundDate"></param>
        public SRID(List<SGTIN> sGTINLIST,
            string sRIDNumber,
            string sUID,
            string dONM,
            string dODT,
            string dOSE,
            DateTime disposalStartDate,
            DateTime disposalEndDate,
            string registrationServerIP,
            string registrationServerPort,
            string emissionServerIP,
            string emissionServerPort,
            bool disposalReport,
            string reportNumber,
            bool satellitesFound,
            DateTime lastSatellitesFoundDate)
        {
            SGTINLIST = sGTINLIST;
            SRIDNumber = sRIDNumber;
            SUID = sUID;
            DONM = dONM;
            DODT = dODT;
            DOSE = dOSE;
            DisposalStartDate = disposalStartDate;
            DisposalEndDate = disposalEndDate;
            RegistrationServerIP = registrationServerIP;
            RegistrationServerPort = registrationServerPort;
            EmissionServerIP = emissionServerIP;
            EmissionServerPort = emissionServerPort;
            DisposalReport = disposalReport;
            ReportNumber = reportNumber;
            SatellitesFound = satellitesFound;
            LastSatellitesFoundDate = lastSatellitesFoundDate;
        }

        /// <summary>
        /// SGTIN
        /// </summary>
        public List<SGTIN> SGTINLIST { get; set; }
        /// <summary>
        /// SRIDNumber
        /// </summary>
        public string SRIDNumber { get; set; }
        /// <summary>
        /// SUID
        /// </summary>
        public string SUID { get; set; }
        /// <summary>
        /// DONM
        /// </summary>
        public string DONM { get; set; }
        /// <summary>
        /// DODT
        /// </summary>
        public string DODT { get; set; }
        /// <summary>
        /// DOSE
        /// </summary>
        public string DOSE { get; set; }
        /// <summary>
        /// DisposalStartDate
        /// </summary>
        public DateTime DisposalStartDate { get; set; }
        /// <summary>
        /// DisposalEndDate
        /// </summary>
        public DateTime DisposalEndDate { get; set; }
        /// <summary>
        /// RegistrationServerIP
        /// </summary>
        public string RegistrationServerIP { get; set; }
        /// <summary>
        /// RegistrationServerPort
        /// </summary>
        public string RegistrationServerPort { get; set; }
        /// <summary>
        /// EmissionServerIP
        /// </summary>
        public string EmissionServerIP { get; set; }
        /// <summary>
        /// EmissionServerPort
        /// </summary>
        public string EmissionServerPort { get; set; }
        /// <summary>
        /// DisposalReport
        /// </summary>
        public bool DisposalReport { get; set; }
        /// <summary>
        /// ReportNumber
        /// </summary>
        public string ReportNumber { get; set; }
        /// <summary>
        /// SatellitesFound
        /// </summary>
        public bool SatellitesFound { get; set; }
        /// <summary>
        /// LastSatellitesFoundDate
        /// </summary>
        public DateTime LastSatellitesFoundDate { get; set; }

    }
}

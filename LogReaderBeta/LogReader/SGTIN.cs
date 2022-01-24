namespace LogReader
{
    public class SGTIN
    {

        public SGTIN()
        {

        }
        /// <summary>
        /// Класс SGTIN
        /// </summary>
        /// <param name="sGTINNumber"></param>
        /// <param name="originalSGTINNumber"></param>
        public SGTIN(string sGTINNumber,
            string originalSGTINNumber)
        {
            SGTINNumber = sGTINNumber;
            OriginalSGTINNumber = originalSGTINNumber;
        }

        /// <summary>
        /// SGTIN
        /// </summary>
        public string SGTINNumber { get; set; }

        /// <summary>
        /// OriginalSGTIN
        /// </summary>
        public string OriginalSGTINNumber { get; set; }


    }
}

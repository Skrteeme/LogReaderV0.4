using Dadata;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Exceptions;

namespace LogReader
{
    class TelegramClient
    {
        public TelegramClient()
        {

        }

        static TelegramBotClient bot;
        RV RV;
        List<RV> RVList = new List<RV>();
        List<SRID> SRIDLIST;
        List<SGTIN> SGTINLIST;
        List<SatteliteInfo> SatteliteInfoList;
        List<DeviceInfo> DeviceInfoList;
        List<MB> MBInfoList;
        List<MK> MKInfoList;
        List<Emission_server_settings> Emission_Server_SettingsList;
        List<Registration_server_settings> Registration_Server_SettingsList;
        string[] TextFromFile;
        string OutPath = @".\Temp\";
        string Path7z = @".\7z\7za.exe";
        bool DownloadProcess = false;
        bool UnzipProcess = false;
        bool BDLoadingProcess = false;

        [Obsolete]
        public void TelegramBot()
        {
            string token = @"1709295883:AAHgq1V_pxjAbPx6SUHGSkRJy9GHt1hwoUA";
            bot = new TelegramBotClient(token);
            var StartUrl = $@"https://api.telegram.org/bot{token}/";
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
        }

        [Obsolete]
        public async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                ClearTempFolder(e.Message.Chat.Id);
                foreach (var item in RVList)
                {
                    if (e.Message.Chat.Id == item.TelegramID)
                    {
                        DeleteRV(item);
                    }
                }
                Directory.CreateDirectory(OutPath + e.Message.Chat.Id);

                DownLoad(e.Message.Document.FileId, OutPath + e.Message.Chat.Id + @"\" + e.Message.Document.FileName, e);
                while (DownloadProcess)
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Ожидаем завершения скачивания файла...");
                    Thread.Sleep(200);
                }
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Распаковка архивов...");
                UnzipRARand7z(e.Message.Chat.Id);
                while (UnzipProcess)
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Ожидаем распаковки файлов...");
                    Thread.Sleep(200);
                }

                RVList.Add(CreateRV(e.Message.Chat.Id));
                foreach (var item in RVList)
                {
                    if (item.TelegramID == e.Message.Chat.Id)
                    {
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, "Загрузка данных в БД...");
                        SearchInFiles(item, e.Message.Chat.Id);
                        break;
                    }
                }
                while (BDLoadingProcess)
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Ожидаем загрузки БД...");
                    Thread.Sleep(200);
                }
                ClearTempFolder(e.Message.Chat.Id);

                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Введите SGTINы:");

            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.Text.Length == 2)
            {
                string Num = e.Message.Text;
                DateTime date = DateTime.Now.ToUniversalTime();
                string Hoursstring = $"{date:HH}";
                if (Hoursstring.Length < 2) Hoursstring = $"0{Hoursstring}";
                string Daysstring = $"{date:dd}";
                if (Hoursstring.Length < 2) Daysstring = $"0{Daysstring}";
                string PredPass = $"{Hoursstring}{Num}{Daysstring}";
                int PredPassInt = Convert.ToInt32(PredPass);
                string Pass = Convert.ToString(PredPassInt, 8);
                if (Pass.Length < 6) Pass = $"0{Pass}";
                string SGTINEnter = $"Пароль тех. поддержки = {Pass}";
                await bot.SendTextMessageAsync(e.Message.Chat.Id, SGTINEnter);
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.Text.Length != 2)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Идёт поиск в БД...");
                List<SGTIN> BadSGTINLIST = new List<SGTIN>();
                List<SRID> GoodSRIDLIST = new List<SRID>();
                string ErrorReport = "";
                
                foreach (var item in RVList)
                {
                    if (item.TelegramID == e.Message.Chat.Id)
                    {
                        SearchSGTINInBD(LoadSGTINList(e.Message.Text), GoodSRIDLIST, BadSGTINLIST, item);
                        ErrorReport += MBErrorHandler(item);
                        ErrorReport += MKErrorHandler(item);
                        ErrorReport += SatteliteHandler(item);
                        ErrorReport += DeviceInfoHandler(item);
                        if (item.RVNumber != "")
                        {
                            using (var stream = File.OpenRead(SaveDataToFile(InfoAboutSRIDonTelegramMass(GoodSRIDLIST, BadSGTINLIST), e.Message.Chat.Id, item.RVNumber)))
                            {
                                await bot.SendDocumentAsync(e.Message.Chat, new InputOnlineFile(stream, $"Анализ логов по РВ {item.RVNumber}.txt"), $"Анализ логов по РВ {item.RVNumber}");
                            }

                            using (var stream = File.OpenRead(SaveDataToFile(ErrorReport, e.Message.Chat.Id, item.RVNumber)))
                            {
                                await bot.SendDocumentAsync(e.Message.Chat, new InputOnlineFile(stream, $"Анализ ошибок РВ {item.RVNumber}.txt"), $"Анализ ошибок по РВ {item.RVNumber}");
                            }
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Архив не содержит логи!!!");
                        }
                        break;
                    }
                }
                ClearTempFolder(e.Message.Chat.Id);
            }

        }
        /// <summary>
        /// заполнение массива искомых SGTINов
        /// </summary>
        public string[] LoadSGTINList(string EnterBox)
        {
            char[] separators = new char[] { ' ', '\n', '\r', '.', ',' , ':', ';' };
            string[] SGTINList = EnterBox.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return SGTINList;
        }
        /// <summary>
        /// Универсальный распаковщик архивов
        /// </summary>
        public void UnzipRARand7z(long ChatID)
        {
            UnzipProcess = true;
            string ChatIDStr = ChatID.ToString();
            string[] FileName = Directory.GetFiles(OutPath + ChatIDStr);
            foreach (var item in FileName)
            {
                if (item.Contains(".rar"))
                {
                    using (Stream stream = File.OpenRead(item))
                    {
                        using (var reader = ReaderFactory.Open(stream))
                        {
                            while (reader.MoveToNextEntry())
                            {
                                if (!reader.Entry.IsDirectory)
                                {
                                    Console.WriteLine(reader.Entry.Key);
                                    reader.WriteEntryToDirectory(OutPath + ChatIDStr, new ExtractionOptions()
                                    {
                                        ExtractFullPath = false,
                                        Overwrite = true
                                    });
                                }
                            }

                        }

                    }
                }
                else
                {
                    ProcessStartInfo pro = new ProcessStartInfo();
                    pro.WindowStyle = ProcessWindowStyle.Hidden;
                    pro.FileName = Path7z;
                    pro.Arguments = string.Format($"e \"{item}\" -y -o\"{OutPath + ChatIDStr}\"");
                    Process x = Process.Start(pro);
                    x.WaitForExit();
                }
            }
            DirectoryInfo directorySelected = new DirectoryInfo(OutPath + ChatIDStr);

            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.gz"))
            {
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    string currentFileName = fileToDecompress.FullName;
                    string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
            }
            UnzipProcess = false;
        }
        /// <summary>
        /// Очистить папку Temp
        /// </summary>
        public void ClearTempFolder(long ChatID)
        {
            string ChatIDStr = ChatID.ToString();
            string[] TempFolderLook = Directory.GetDirectories(OutPath);
            foreach (var item in TempFolderLook)
            {
                if (item.Contains(ChatIDStr))
                {
                    string[] ReultSearchDir = Directory.GetDirectories(OutPath + ChatIDStr);
                    string[] ReultSearchFiles = Directory.GetFiles(OutPath + ChatIDStr);
                    foreach (var Dir in ReultSearchDir)
                    {
                        Directory.Delete(Dir);
                    }
                    foreach (var Files in ReultSearchFiles)
                    {
                        File.Delete(Files);
                    }
                }
            }
            

        }
        /// <summary>
        /// Обработчик DeviceInfo
        /// </summary>
        /// <returns></returns>
        public string DeviceInfoHandler(RV RV)
        {
            string Result = "";
            int sum = 0;
            int max;
            int maxCount = 0;
            int min;
            int minCount = 0;
            int[] MinNumbers = new int[RV.DeviceInfoList.Count];
            int[] MaxNumbers = new int[RV.DeviceInfoList.Count];
            if (RV.DeviceInfoList.Count > 0)
            {
                for (int i = 0; i < RV.DeviceInfoList.Count; i++)
                {

                    if ( RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE != "" && Convert.ToInt32(RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE) < 3000)
                    {
                        MinNumbers[minCount] = Convert.ToInt32(RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE);
                        minCount++;
                    }
                    if (RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE != "" && Convert.ToInt32(RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE) > 3000)
                    {
                        MaxNumbers[maxCount] = Convert.ToInt32(RV.DeviceInfoList[i].SOURCE_OF_POWER_VOLTAGE);
                        maxCount++;
                    }
                }
                min = MaxNumbers[0];
                max = MaxNumbers[0];
                foreach (var item in MaxNumbers)
                {
                    if (item < min && item != 0) min = item;
                    if (item > max && item != 0) max = item;
                    sum += item;
                }
                if (maxCount != 0)
                {
                    sum /= maxCount;
                }
                Result += $"Информация по питанию:\n\n";
                Result += $"Среднее значение на зарядке = {sum}\n";
                Result += $"Минимальное значение на зарядке = {min}\n";
                Result += $"Максимальное значение на зарядке = {max}\n\n";
                min = MinNumbers[0];
                max = MinNumbers[0];
                sum = 0;
                foreach (var item in MinNumbers)
                {
                    if (item < min && item != 0) min = item;
                    if (item > max && item != 0) max = item;
                    sum += item;
                }
                if (minCount != 0)
                {
                    sum /= minCount;
                }
                
                Result += $"Среднее значение без ЗУ = {sum}\n";
                Result += $"Минимальное значение без ЗУ = {min}\n";
                Result += $"Максимальное значение без ЗУ = {max}\n\n";
            }
            return Result;
        }
        /// <summary>
        /// Выводит ошибки по МК
        /// </summary>
        /// <returns></returns>
        public string MKErrorHandler(RV RV)
        {
            string ResultBox = "Ошибки СБ:\n\n";
            foreach (var item in RV.MKInfoList)
            {
                int i = 0;
                if (item.INITIALIZED == "false") { ResultBox += "РВ не инициализирован\n"; i++; }
                if (item.SU_IS_SLEEPING == "true") { ResultBox += "МБ находится в спящем режиме\n"; i++; }
                if (item.SU_POWER_IS_SUPPLIED_DURING_SLEEP == "false") { ResultBox += "Питание на МБ не подается\n"; i++; }
                if (item.NO_SERIAL_NUMBER == "true") { ResultBox += "Серийный номер не введен\n"; i++; }
                if (item.RTC_FAULT == "true") { ResultBox += "Ошибка часов реального времени\n"; i++; }
                if (item.SETTINGS_FAULT == "true") { ResultBox += "Ошибка таблицы настроек\n"; i++; }
                if (item.COUNTERS_FAULT == "true") { ResultBox += "Ошибка регистров и счетчиков\n"; i++; }
                if (item.ATTRIBUTES_FAULT == "true") { ResultBox += "Ошибка реквизитов\n"; i++; }
                if (item.SU_FAULT == "true") { ResultBox += "Фатальная ошибка МБ\n"; i++; }
                if (item.INVALID_SU == "true") { ResultBox += "Установленный МБ зарегистрирован в другом устройстве\n"; i++; }
                if (item.HARD_FAULT == "true") { ResultBox += "Фатальная аппаратная ошибка\n"; i++; }
                if (item.MEMORY_MANAGER_FAULT == "true") { ResultBox += "Ошибка диспетчера памяти\n"; i++; }
                if (item.WAIT_FOR_REBOOT == "true") { ResultBox += "Ожидание перезагрузки - ошибка\n"; i++; }
                if (item.SECURITY_SYSTEM_AUTH_FAULT == "true") { ResultBox += "Ошибка проверки в системе безопасности\n"; i++; }
                if (item.SECURITY_SYSTEM_FAULT == "true") { ResultBox += "Сбой системы безопасности\n"; i++; }
                if (i > 0)
                {
                    ResultBox += $"Дата = {item.Date}\n\n";
                }
            }
            return ResultBox;
        }
        /// <summary>
        /// Обработчик ошибок перезагрузки МБ
        /// </summary>
        /// <param name="SU_REBOOT_REASON"></param>
        /// <returns></returns>
        public string SU_REBOOT_REASONHeader(string SU_REBOOT_REASON)
        {
            string TempError = "";
            if (SU_REBOOT_REASON != "")
            {
                TempError = Convert.ToString(Convert.ToInt32(SU_REBOOT_REASON), 16);
            }
            string Result = TempError;
            if (TempError == "7312") Result = "Команда не поддерживается. Неверное состояние МБ РВ(Команда не разрешена)";
            if (TempError == "7313") Result = "Ошибка проверки контрольной суммы, ошибка формата и т.д.";
            if (TempError == "7315") Result = "Некорректные входные данные";
            if (TempError == "7316") Result = "Неверная длина входных данных";
            if (TempError == "7317") Result = "Буфер заполнен";
            if (TempError == "7318") Result = "Некорректный параметр команды ParamChain";
            if (TempError == "7319") Result = "Некорректный параметр команды ParamMode.Режим не поддерживается";
            if (TempError == "7321") Result = "Пул КМ не найден";
            if (TempError == "7322") Result = "Некорректное состояние пула КМ";
            if (TempError == "7323") Result = "Нет данных для выдачи(в пуле КМ закончились коды маркировки)";
            if (TempError == "7324") Result = "Недостаточно памяти для сохранения пула КМ";
            if (TempError == "7325") Result = "Запрошено слишком много данных(в пуле КМ нет запрошенного количества КМ)";
            if (TempError == "7327") Result = "Все данные выданы. Цепочка завершена";
            if (TempError == "7328") Result = "Требуется завершить цепочку";
            if (TempError == "7329") Result = "Есть данные, цепочка не может быть завершена";
            if (TempError == "7331") Result = "Формат сертификата неверен";
            if (TempError == "7332") Result = "Срок действия сертификата истёк";
            if (TempError == "7333") Result = "Ошибка проверки криптограммы";
            if (TempError == "7334") Result = "Превышено количество использований сеансового ключа";
            if (TempError == "7335") Result = "Требуется ключ для проверки";
            if (TempError == "7341") Result = "Не была проведена авторизация пользователя";
            if (TempError == "7342") Result = "Не была проведена аутентификация с удаленным сервером";
            if (TempError == "7343") Result = "Нарушена последовательность команд аутентификации, транзакции";
            if (TempError == "7344") Result = "Некорректное сообщение(ошибка структуры, некорректный тип данных, размер переданных данных не соответствует указанному в заголовке сообщения)";
            if (TempError.Contains("73c")) Result = $"Проверка кода(ПИН / КРП) неуспешна, осталось {TempError[3]}-попыток";
            if (TempError == "7375") Result = "Устройства МБ РВ не готовы к работе";
            if (TempError == "7377") Result = "Сообщение «PERR»";
            if (TempError == "7381") Result = "Требуется сменить ПИН-код или значение нового ПИН-кода некорректно(= «0 0 0 0»)";
            if (TempError == "7383") Result = "Не все отчеты выданы";
            if (TempError == "7385") Result = "Сертификат не найден";
            if (TempError == "7386") Result = "Команда не разрешена. Связь со спутником отсутствует более 24 часов";
            if (TempError == "7387") Result = "Команда не разрешена. ПИН заблокирован";
            if (TempError == "73e0") Result = "Рассинхронизация между элементами МБ РВ";
            if (TempError == "73e1") Result = "Внутренняя ошибка МБ РВ при передаче данных";
            if (TempError == "73e2") Result = "Недостаточное напряжение питания МБ РВ";
            if (TempError == "7430") Result = "Ошибка интерфейса МБ";
            if (TempError == "7431") Result = "Ошибка интерфейса МБ при посылке команды";
            if (TempError == "7432") Result = "Неверный CRC в ответе МБ";
            if (TempError == "7433") Result = "Неверный PCB в ответе МБ";
            if (TempError == "7434") Result = "Неверная длина ответа МБ";
            if (TempError == "7435") Result = "Неверные данные в заголовке ответа МБ";
            if (TempError == "7436") Result = "Превышение таймаута на команду";


            return Result;
        }

        /// <summary>
        /// Выводит ошибки по МБ
        /// </summary>
        /// <returns></returns>
        public string MBErrorHandler(RV RV)
        {
            string ResultBox = "Ошибки МБ:\n\n";
            foreach (var item in RV.MBInfoList)
            {
                int i = 0;
                if (item.SU_STATE == "0" || item.SU_STATE == "5" || item.SU_STATE == "6" || item.SU_STATE == "7" || item.SU_STATE == "255")
                {
                    ResultBox += $"\n Дата = {item.Date}\n";
                    ResultBox += $"Дата окончания срока службы МБ = {item.DATE_TIME}\n";
                    ResultBox += $"Процент использованного ресурса накопителя МБ РВ = {item.SU_RESOURCE}\n";
                    ResultBox += $"Код команды с превышением буфера = {item.COMMAND_CODE}\n";
                    ResultBox += $"Счетчик перезагрузок МБ = {item.SU_REBOOT_COUNTER}\n";
                    ResultBox += $"Время последней перезагрузки МБ = {item.SU_LAST_REBOOT_TIME}\n";
                    ResultBox += $"Причина перезагрузки МБ = {SU_REBOOT_REASONHeader(item.SU_REBOOT_REASON)}\n";
                    ResultBox += $"Версия прошивки МБ = {item.SU_VERSION}\n";
                }
                if (item.SU_STATE == "0")
                {
                    ResultBox += "МБ РВ не инициализирован\n";
                    i++;
                }

                if (item.SU_STATE == "5")
                {
                    ResultBox += "Истек срок использования МБ РВ\n";
                    i++;
                }

                if (item.SU_STATE == "6")
                {
                    ResultBox += "Состояние МБ РВ - ключ подписи блокирован\n";
                    i++;
                }
                if (item.SU_STATE == "7")
                {
                    ResultBox += "МБ РВ не функционален\n";
                    i++;
                }
                if (item.SU_STATE == "255")
                {
                    ResultBox += "МБ РВ отсутствует\n";
                    i++;
                }
                if (item.SU_SUSPENDED == "true")
                {
                    ResultBox += "Основные функции МБ РВ приостановлены по причине отсутствия связи со спутником\n";
                    i++;
                }
                //if (item.SERVER_AUTH_STATE == "false")
                //{
                //    ResultBox += "Аутентификация с сервером не проведена\n";
                //    i++;
                //}
                if (item.GNSS_READY == "false")
                {
                    ResultBox += "ГНСС не готова\n";
                    i++;
                }
                if (item.CC_READY == "false")
                {
                    ResultBox += "Криптосопроцессор не готов\n";
                    i++;
                }
                if (item.SU_ARCHIVE_READY == "false")
                {
                    ResultBox += "Архив МБ РВ не готов\n";
                    i++;
                }
                if (item.SU_READY == "false")
                {
                    ResultBox += "МБ РВ не готов\n";
                    i++;
                }
                if (item.PRIVILEGES_PRESENTED == "true")
                {
                    ResultBox += "Предъявлены административные привилегии доступа\n";
                    i++;
                }
                if (item.SS_AUTH_READY == "false" && item.SS_AUTH_SUCCESS == "false")
                {
                    ResultBox += "Аутентификация МБ не успешна или не проведена\n";
                    i++;
                }
                if (item.SS_INIT_READY == "false")
                {
                    ResultBox += "Инициализация МБ не проведена\n";
                    i++;
                }
                if (i > 0)
                {
                    ResultBox += $"Дата = {item.Date}\n\n";
                }
            }
            return ResultBox;
        }
        /// <summary>
        /// Выводит инфо по спутникам
        /// </summary>
        /// <returns></returns>
        public string SatteliteHandler(RV RV)
        {
            string ResultBox = "Ошибки по спутникам:\n\n";
            foreach (var item in RV.SatelliteInfoList)
            {
                if (item.GNSS_STATUS == "0")
                {
                    ResultBox += $"Дата = {item.DateTime}\n";
                    ResultBox += "Состояние антенны не определялось\n\n";
                }

                if (item.GNSS_STATUS == "1")
                {
                    ResultBox += $"Дата = {item.DateTime}\n";
                    ResultBox += "Короткое замыкание\n\n";
                }

                if (item.GNSS_STATUS == "2")
                {
                    ResultBox += $"Дата = {item.DateTime}\n";
                    ResultBox += "Антенна не подсоединена\n\n";
                }
            }
            return ResultBox;
        }
        /// <summary>
        /// Очистка памяти
        /// </summary>
        public void DeleteRV(RV RV)
        {
            RV = null;
            GC.Collect();
        }
        /// <summary>
        /// Сохранение отчёта в файл
        /// </summary>
        /// <param name="OutPath"></param>
        public string SaveDataToFile(string Report, long ChatID, string RVNumber)
        {
            string ChatIDStr = ChatID.ToString();
            string Name = $@"{OutPath + ChatIDStr}\Анализ логов РВ {RVNumber}.txt";
            File.WriteAllText(Name, Report);
            return Name;
        }
        /// <summary>
        /// Формирование ответа по всем GOODSRID
        /// </summary>
        public string InfoAboutSRIDonTelegramMass(List<SRID> GoodSRIDLIST, List<SGTIN> BadSGTINLIST)
        {
            string Result = $"Отчёты с данными SGTIN:\n\n";
            foreach (var item in GoodSRIDLIST)
            {
                Result += OutputInfoAboutSRIDonTelegram(item);
            }
            if (BadSGTINLIST.Count > 0)
            {
                Result += $"SGTIN выбытия которых отсутствуют:\n";
                foreach (var item in BadSGTINLIST)
                {
                    Result += $"{item.SGTINNumber}\n";
                }
            }
            return Result;
        }

        /// <summary>
        /// Формирование ответа по 1 SRID
        /// </summary>
        /// <param name="SRID"></param>
        /// <returns></returns>
        public string OutputInfoAboutSRIDonTelegram(SRID SRID)
        {
            string Result = $"Отчёт номер = {SRID.SRIDNumber}\n" +
                $"Дата отправки отчёта из МБ в ИС МДЛП = {SRID.DisposalEndDate}\n" +
                $"Идентификатор места деятельности в ИС МДЛП на момент выбытия = {SRID.SUID}\n";
            if (SRID.SatellitesFound == false)
            {
                Result += $"МБ заблокирована по спутникам!!!\n";
            }
            if (SRID.RegistrationServerIP != "78.142.221.102" && SRID.RegistrationServerIP != "")
            {
                Result += $"Адреса сервера регистрации и эмиссии на момент выбытия не верны!!! = { SRID.RegistrationServerIP }\n";
            }
            if (SRID.RegistrationServerPort != "21401" && SRID.RegistrationServerPort != "")
            {
                Result += $"Порт сервера регистрации на момент выбытия не верен!!! = { SRID.RegistrationServerPort }\n";
            }
            if (SRID.EmissionServerPort != "21301" && SRID.EmissionServerPort != "")
            {
                Result += $"Порт сервера регистрации на момент выбытия не верен!!! = { SRID.RegistrationServerPort }\n";
            }
            if (SRID.DisposalReport == false || SRID.DisposalEndDate == DateTime.MinValue)
            {
                Result += $"Отчёт НЕ был отправлен в ИС МДЛП!!!\nПодробности:\n\n";
                Result += OutputInfoAboutSRID(SRID);
            }

            Result += "\n";
            return Result;
        }
        /// <summary>
        /// Вывод информации по отчёту из GoodSRIDLIST
        /// </summary>
        public string OutputInfoAboutSRID(SRID SRID)
        {
            string ResultBox = $"Отчёт номер = {SRID.SRIDNumber}\n" +
                $"Идентификатор места деятельности в ИС МДЛП = {SRID.SUID}\n" +
                $"Номер отчёта введёный УОТ = {SRID.DONM}\n" +
                $"Дата отчёта введённая УОТ = {SRID.DODT}\n" +
                $"Серия отчёта введённая УОТ = {SRID.DOSE}\n" +
                $"Дата начала выбытия отчёта  = {SRID.DisposalStartDate}\n" +
                $"Дата отправки отчёта из МБ в ИС МДЛП = {SRID.DisposalEndDate}\n" +
                $"Адрес сервера регистрации на момент выбытия = {SRID.RegistrationServerIP}\n" +
                $"Порт сервера регистрации на момент выбытия = {SRID.RegistrationServerPort}\n" +
                $"Адрес сервера эмиссии на момент выбытия = {SRID.EmissionServerIP}\n" +
                $"Порт сервера эмиссии на момент выбытия = {SRID.EmissionServerPort}\n" +
                $"Статус отчёта о выбытии = {SRID.DisposalReport}\n" +
                $"Номер отчёта в журнале РВ = {SRID.ReportNumber}\n" +
                $"МБ разблокирована по спутникам = {SRID.SatellitesFound}\n" +
                $"Дата разблокировки МБ по спутникам = {SRID.LastSatellitesFoundDate}\n" +
                $"Номер регистратора = {RV.RVNumber}\n" +
                $"Номер МБ = {RV.MBNumber}\n" +
                $"Версия ПО на момент выбытия = {SearchSoftVersion(SRID)}\n" +
                $"Выбытие производилось с адреса = {RV.RVLocation}\n" +
                $"\nОтчёт содержит SGTINы: \n";
            foreach (var item in SRID.SGTINLIST)
            {
                ResultBox += $"{item.SGTINNumber}\n";
            }
            ResultBox += "\nСостояние МБ :\n\n";
            for (int i = 0; i <= RV.MBInfoList.Count; i++)
            {
                if (RV.MBInfoList[i].Date >= SRID.DisposalStartDate)
                {
                    ResultBox += HandlerMB(RV.MBInfoList[i]);
                    break;
                }
            }
            ResultBox += "\nСостояние СБ:\n\n";
            for (int i = 0; i <= RV.MKInfoList.Count; i++)
            {
                if (RV.MKInfoList[i].Date >= SRID.DisposalStartDate)
                {
                    ResultBox += HandlerMK(RV.MKInfoList[i]);
                    break;
                }
            }

            return ResultBox;
        }
        /// <summary>
        /// Обработчик состояния МБ РВ
        /// </summary>
        /// <param name="MB"></param>
        /// <returns></returns>
        public string HandlerMB(MB MB)
        {
            string ResultBox = "";
            if (MB.SU_STATE == "0")
            {
                ResultBox += "МБ РВ не инициализирован\n";
            }
            if (MB.SU_STATE == "1")
            {
                ResultBox += "Состояние МБ РВ - проведена инициализация\n";
            }
            if (MB.SU_STATE == "2")
            {
                ResultBox += "Состояние МБ РВ - данные регистрации загружены\n";
            }
            if (MB.SU_STATE == "3")
            {
                ResultBox += "Состояние МБ РВ - сертификат загружен\n";
            }
            if (MB.SU_STATE == "4")
            {
                ResultBox += "МБ РВ активирован\n";
            }
            if (MB.SU_STATE == "5")
            {
                ResultBox += "Истек срок использования МБ РВ\n";
            }
            if (MB.SU_STATE == "6")
            {
                ResultBox += "Состояние МБ РВ - ключ подписи блокирован\n";
            }
            if (MB.SU_STATE == "7")
            {
                ResultBox += "МБ РВ не функционален\n";
            }
            if (MB.SU_STATE == "255")
            {
                ResultBox += "МБ РВ отсутствует\n";
            }
            if (MB.SU_SUSPENDED == "false")
            {
                ResultBox += "Основные функции МБ РВ доступны\n";
            }
            if (MB.SU_SUSPENDED == "true")
            {
                ResultBox += "Основные функции МБ РВ приостановлены по причине отсутствия связи со спутником\n";
            }
            if (MB.SERVER_AUTH_STATE == "false")
            {
                ResultBox += "Аутентификация с сервером не проведена\n";
            }
            if (MB.SERVER_AUTH_STATE == "true")
            {
                ResultBox += "Аутентификация с сервером проведена\n";
            }
            if (MB.USER_AUTH_STATE == "false")
            {
                ResultBox += "Аутентификация с пользователем не проведена\n";
            }
            if (MB.USER_AUTH_STATE == "true")
            {
                ResultBox += "Аутентификация с пользователем проведена\n";
            }
            if (MB.QUEUE_STATUS == "false")
            {
                ResultBox += "Нет данных в очереди\n";
            }
            if (MB.QUEUE_STATUS == "true")
            {
                ResultBox += "Есть данные в очереди\n";
            }
            ResultBox += $"Дата окончания срока службы МБ = {MB.DATE_TIME}\n";
            if (MB.GNSS_READY == "false")
            {
                ResultBox += "ГНСС не готова\n";
            }
            if (MB.GNSS_READY == "true")
            {
                ResultBox += "ГНСС готова\n";
            }
            if (MB.CC_READY == "false")
            {
                ResultBox += "Криптосопроцессор не готов\n";
            }
            if (MB.CC_READY == "true")
            {
                ResultBox += "Криптосопроцессор готов\n";
            }
            if (MB.SU_ARCHIVE_READY == "false")
            {
                ResultBox += "Архив МБ РВ не готов\n";
            }
            if (MB.SU_ARCHIVE_READY == "true")
            {
                ResultBox += "Архив МБ РВ готов\n";
            }
            if (MB.SU_READY == "false")
            {
                ResultBox += "МБ РВ не готов\n";
            }
            if (MB.SU_READY == "true")
            {
                ResultBox += "МБ РВ готов\n";
            }
            if (MB.PRIVILEGES_PRESENTED == "false")
            {
                ResultBox += "Административные привилегии доступа не предъявлены\n";
            }
            if (MB.PRIVILEGES_PRESENTED == "true")
            {
                ResultBox += "Предъявлены административные привилегии доступа\n";
            }
            if (MB.SS_AUTH_READY == "false")
            {
                ResultBox += "Аутентификация МБ не проведена\n";
            }
            if (MB.SS_AUTH_READY == "true")
            {
                ResultBox += "Аутентификация МБ проведена\n";
            }
            if (MB.SS_AUTH_SUCCESS == "false")
            {
                ResultBox += "Аутентификация МБ не успешна или не проведена\n";
            }
            if (MB.SS_AUTH_SUCCESS == "true")
            {
                ResultBox += "Аутентификация МБ проведена успешно\n";
            }
            if (MB.SS_INIT_READY == "false")
            {
                ResultBox += "Инициализация МБ не проведена\n";
            }
            if (MB.SS_INIT_READY == "true")
            {
                ResultBox += "Инициализация МБ проведена\n";
            }
            ResultBox += $"Процент использованного ресурса накопителя МБ РВ = {MB.SU_RESOURCE}\n";
            ResultBox += $"Код команды с превышением буфера = {MB.COMMAND_CODE}\n";
            ResultBox += $"Счетчик перезагрузок МБ = {MB.SU_REBOOT_COUNTER}\n";
            ResultBox += $"Время последней перезагрузки МБ = {MB.SU_LAST_REBOOT_TIME}\n";
            ResultBox += $"Причина перезагрузки МБ = {SU_REBOOT_REASONHeader(MB.SU_REBOOT_REASON)}\n";
            ResultBox += $"Версия прошивки МБ = {MB.SU_VERSION}\n\n";
            return ResultBox;
        }
        /// <summary>
        /// Обработчик состояния МК
        /// </summary>
        /// <param name="MK"></param>
        /// <returns></returns>
        public string HandlerMK(MK MK)
        {
            string ResultBox = "";
            if (MK.INITIALIZED == "true")
            {
                ResultBox += "РВ инициализирован\n";
            }
            if (MK.INITIALIZED == "false")
            {
                ResultBox += "РВ не инициализирован\n";
            }
            if (MK.MARK_CHECK_STARTED == "false")
            {
                ResultBox += "Не запущен процесс проверки КМ\n";
            }
            if (MK.MARK_CHECK_STARTED == "true")
            {
                ResultBox += "Запущен процесс проверки КМ\n";
            }
            if (MK.DISPOSAL_REPORT_IN_PROGRESS == "false")
            {
                ResultBox += "Не запущен процесс регистрации отчета о выбытии\n";
            }
            if (MK.DISPOSAL_REPORT_IN_PROGRESS == "true")
            {
                ResultBox += "Запущен процесс регистрации отчета о выбытии\n";
            }
            if (MK.SU_EXCHANGE_SUSPENDED == "false")
            {
                ResultBox += "Обмен с СЭ осуществляется\n";
            }
            if (MK.SU_EXCHANGE_SUSPENDED == "true")
            {
                ResultBox += "Обмен с СЭ приостановлен\n";
            }
            ResultBox += $"Количество полученных документов для обработки в МБ РВ = {MK.RECEIVED_DOCUMENTS_COUNT}\n";
            ResultBox += $"Количество обработанных и отправленных документов из МБ РВ = {MK.PROCESSED_DOCUMENTS_COUNT}\n";
            if (MK.QUEUE_STATUS == "false")
            {
                ResultBox += "Не запущен процесс отправки документов из очереди\n";
            }
            if (MK.QUEUE_STATUS == "true")
            {
                ResultBox += "Запущен процесс отправки документов из очереди\n";
            }
            if (MK.SERVER_AUTH_STATE == "false")
            {
                ResultBox += "Не запущен процесс аутентификации с сервером\n";
            }
            if (MK.SERVER_AUTH_STATE == "true")
            {
                ResultBox += "Запущен процесс аутентификации с сервером\n";
            }
            if (MK.POOL_PROCESSING_STATE == "0")
            {
                ResultBox += "Процесс обработки пула КМ\n";
            }
            if (MK.SU_IS_SLEEPING == "false")
            {
                ResultBox += "МБ активен\n";
            }
            if (MK.SU_IS_SLEEPING == "true")
            {
                ResultBox += "МБ находится в спящем режиме\n";
            }
            if (MK.SU_POWER_IS_SUPPLIED_DURING_SLEEP == "false")
            {
                ResultBox += "Питание на МБ не подается\n";
            }
            if (MK.SU_POWER_IS_SUPPLIED_DURING_SLEEP == "true")
            {
                ResultBox += "Питание на МБ подается\n";
            }
            if (MK.NO_SERIAL_NUMBER == "false")
            {
                ResultBox += "Серийный номер введен\n";
            }
            if (MK.NO_SERIAL_NUMBER == "true")
            {
                ResultBox += "Серийный номер не введен\n";
            }
            if (MK.RTC_FAULT == "false")
            {
                ResultBox += "Нет ошибки часов реального времени\n";
            }
            if (MK.RTC_FAULT == "true")
            {
                ResultBox += "Ошибка часов реального времени\n";
            }
            if (MK.SETTINGS_FAULT == "false")
            {
                ResultBox += "Нет ошибки таблицы настроек\n";
            }
            if (MK.SETTINGS_FAULT == "true")
            {
                ResultBox += "Ошибка таблицы настроек\n";
            }
            if (MK.COUNTERS_FAULT == "false")
            {
                ResultBox += "Нет ошибки регистров и счетчиков\n";
            }
            if (MK.COUNTERS_FAULT == "true")
            {
                ResultBox += "Ошибка регистров и счетчиков\n";
            }
            if (MK.ATTRIBUTES_FAULT == "false")
            {
                ResultBox += "Нет ошибки реквизитов\n";
            }
            if (MK.ATTRIBUTES_FAULT == "true")
            {
                ResultBox += "Ошибка реквизитов\n";
            }
            if (MK.SU_FAULT == "false")
            {
                ResultBox += "Нет фатальной ошибки МБ\n";
            }
            if (MK.SU_FAULT == "true")
            {
                ResultBox += "Фатальная ошибка МБ\n";
            }
            if (MK.INVALID_SU == "false")
            {
                ResultBox += "Установленный МБ не зарегистрирован в другом устройстве\n";
            }
            if (MK.INVALID_SU == "true")
            {
                ResultBox += "Установленный МБ зарегистрирован в другом устройстве\n";
            }
            if (MK.HARD_FAULT == "false")
            {
                ResultBox += "Нет фатальной аппаратной ошибки\n";
            }
            if (MK.HARD_FAULT == "true")
            {
                ResultBox += "Фатальная аппаратная ошибка\n";
            }
            if (MK.MEMORY_MANAGER_FAULT == "false")
            {
                ResultBox += "Нет ошибки диспетчера памяти\n";
            }
            if (MK.MEMORY_MANAGER_FAULT == "true")
            {
                ResultBox += "Ошибка диспетчера памяти\n";
            }
            if (MK.WAIT_FOR_REBOOT == "false")
            {
                ResultBox += "Ожидание перезагрузки - нет ошибки\n";
            }
            if (MK.WAIT_FOR_REBOOT == "true")
            {
                ResultBox += "Ожидание перезагрузки - ошибка\n";
            }
            if (MK.SECURITY_SYSTEM_AUTH_FAULT == "false")
            {
                ResultBox += "Нет ошибки проверки в системе безопасности\n";
            }
            if (MK.SECURITY_SYSTEM_AUTH_FAULT == "true")
            {
                ResultBox += "Ошибка проверки в системе безопасности\n";
            }
            if (MK.SECURITY_SYSTEM_FAULT == "false")
            {
                ResultBox += "Нет сбоя системы безопасности\n";
            }
            if (MK.SECURITY_SYSTEM_FAULT == "true")
            {
                ResultBox += "Сбой системы безопасности\n";
            }
            ResultBox += $"Состояние инициализации МБ РВ = {MK.SU_STATE}\n\n";


            return ResultBox;
        }
        /// <summary>
        /// Определение версии ПО на момент выбытия
        /// </summary>
        /// <param name="SRID"></param>
        /// <returns></returns>
        public string SearchSoftVersion(SRID SRID)
        {
            string ResultBox = "";
            foreach (var item in RV.DeviceInfoList)
            {
                if (item.DateTime <= SRID.DisposalStartDate)
                {
                    ResultBox = item.APP_VERSION;
                }
            }
            if (ResultBox == "1.7.6")
            {
                ResultBox = "1.9.1.1362";
            }
            if (ResultBox == "1.6.4")
            {
                ResultBox = "1.5";
            }
            return ResultBox;
        }
        /// <summary>
        /// Поиск SGTINов в БД
        /// </summary>
        /// <param name="SGTINList"></param>
        /// <param name="GoodSRIDLIST"></param>
        /// <param name="BadSGTINLIST"></param>
        /// <returns></returns>
        public string SearchSGTINInBD(string[] SGTINList, List<SRID> GoodSRIDLIST, List<SGTIN> BadSGTINLIST, RV RV)
        {
            int RepCount;
            string ResultBox = "";

            for (int sgtin = 0; sgtin < SGTINList.Length; sgtin++)
            {
                RepCount = 0;
                for (int SRID = 0; SRID < RV.SRIDLIST.Count; SRID++)
                {
                    for (int SGTIN = 0; SGTIN < RV.SRIDLIST[SRID].SGTINLIST.Count; SGTIN++)
                    {
                        if (RV.SRIDLIST[SRID].SGTINLIST[SGTIN].SGTINNumber.Equals(SGTINList[sgtin]))
                        {
                            if (GoodSRIDLIST.Contains(RV.SRIDLIST[SRID]) == false)
                            {
                                GoodSRIDLIST.Add(RV.SRIDLIST[SRID]);
                            }

                            ResultBox += OutputData(RV.SRIDLIST[SRID], RV.SRIDLIST[SRID].SGTINLIST[SGTIN]);
                            RepCount++;
                        }
                    }

                }

                if (RepCount == 0)
                {
                    ResultBox += $"Отчётов о выбытии SGTIN = {SGTINList[sgtin]} НЕ НАЙДЕНО!!!\n\n";
                    BadSGTINLIST.Add(new SGTIN(SGTINList[sgtin], ""));
                }
            }


            return ResultBox;
        }
        /// <summary>
        /// Вывод данных о SGTIN
        /// </summary>
        public string OutputData(SRID SRID, SGTIN SGTIN)
        {
            string ResultBox = $"SGTIN = {SGTIN.SGTINNumber}\n" +
                $"Оригинальный SGTIN  = {SGTIN.OriginalSGTINNumber}\n" +
                $"Выбывался в отчёте номер = {SRID.SRIDNumber}\n" +
                $"Идентификатор места деятельности в ИС МДЛП = {SRID.SUID}\n" +
                $"Номер отчёта введёный УОТ = {SRID.DONM}\n" +
                $"Дата отчёта введённая УОТ = {SRID.DODT}\n" +
                $"Серия отчёта введённая УОТ = {SRID.DOSE}\n" +
                $"Дата начала выбытия отчёта  = {SRID.DisposalStartDate}\n" +
                $"Дата отправки отчёта из МБ в ИС МДЛП = {SRID.DisposalEndDate}\n" +
                $"Адрес сервера регистрации на момент выбытия = {SRID.RegistrationServerIP}\n" +
                $"Порт сервера регистрации на момент выбытия = {SRID.RegistrationServerPort}\n" +
                $"Адрес сервера эмиссии на момент выбытия = {SRID.EmissionServerIP}\n" +
                $"Порт сервера эмиссии на момент выбытия = {SRID.EmissionServerPort}\n" +
                $"Статус отчёта о выбытии = {SRID.DisposalReport}\n" +
                $"Номер отчёта в журнале РВ = {SRID.ReportNumber}\n" +
                $"МБ разблокирована по спутникам = {SRID.SatellitesFound}\n" +
                $"Дата разблокировки МБ по спутникам = {SRID.LastSatellitesFoundDate}\n\n";

            return ResultBox;
        }

        /// <summary>
        /// Скачивание файла c Telegram
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        /// <param name="e"></param>
        [Obsolete]
        public async void DownLoad(string fileId, string path, Telegram.Bot.Args.MessageEventArgs e)
        {
            DownloadProcess = true;
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            
            fs.Close();
            fs.Dispose();
            DownloadProcess = false;
        }

        /// <summary>
        /// Создание нового РВ
        /// </summary>
        public RV CreateRV(long TelegramID)
        {
            RV = new RV(SRIDLIST = new List<SRID>(),
                "",
                "",
                SatteliteInfoList = new List<SatteliteInfo>(),
                DeviceInfoList = new List<DeviceInfo>(),
                MBInfoList = new List<MB>(),
                MKInfoList = new List<MK>(),
                Emission_Server_SettingsList = new List<Emission_server_settings>(),
                Registration_Server_SettingsList = new List<Registration_server_settings>(),
                "",
                TelegramID);
            return RV;
        }
        /// <summary>
        /// Перебор файлов с логами
        /// </summary>
        /// <param name="Path"></param>
        public void SearchInFiles(RV RV, long ChatID)
        {
            BDLoadingProcess = true;

            string ChatIDStr = ChatID.ToString();
            DirectoryInfo directorySelected = new DirectoryInfo(OutPath + ChatIDStr);

            int SRIDTempNum = 0;

            foreach (FileInfo LogFile in directorySelected.GetFiles())
            {
                if (LogFile.Extension.Contains(".gz")) continue;
                if (LogFile.Name.Contains("librv.log"))
                {
                    ReadFile(LogFile.FullName);


                    for (int Line = 0; Line < TextFromFile.Length; Line++)
                    {
                        if (TextFromFile[Line].Contains("librv_get_device_info()")) LoadDeviceInfo(Line, RV);
                        if (TextFromFile[Line].Contains("librv_get_last_reliable_satellites_info()")) LoadSatteliteInfo(Line, RV);
                        if (TextFromFile[Line].Contains("librv_get_security_unit_status()")) LoadMBInfo(Line, RV);
                        if (TextFromFile[Line].Contains("librv_get_mc_status()")) LoadMKInfo(Line, RV);
                        if (TextFromFile[Line].Contains("librv_read_emission_server_settings()")) LoadEmission_server_settings(Line, RV);
                        if (TextFromFile[Line].Contains("librv_read_registration_server_settings()")) LoadRegistration_server_settings(Line, RV);
                    }
                }
            }


            if (RV.DeviceInfoList.Count>0)
            {
                RV.RVNumber = RV.DeviceInfoList[0].SERIAL_NUMBER;
            }
            if (RV.SatelliteInfoList.Count>0)
            {
                SearchDisposalLocationAsync(RV);
            }
            

            RV.DeviceInfoList = RV.DeviceInfoList.OrderBy(x => x.DateTime).ToList();
            RV.SatelliteInfoList = RV.SatelliteInfoList.OrderBy(x => x.DateTime).ToList();
            RV.MBInfoList = RV.MBInfoList.OrderBy(x => x.Date).ToList();
            RV.MKInfoList = RV.MKInfoList.OrderBy(x => x.Date).ToList();
            RV.Emission_Server_SettingsList = RV.Emission_Server_SettingsList.OrderBy(x => x.DateTime).ToList();
            RV.Registration_Server_SettingsList = RV.Registration_Server_SettingsList.OrderBy(x => x.DateTime).ToList();

            foreach (FileInfo LogFile in directorySelected.GetFiles())
            {
                if (LogFile.Extension.Contains(".gz")) continue;
                if (LogFile.Name.Contains("librv.log"))
                {
                    ReadFile(LogFile.FullName);
                    LoadSRID(ref SRIDTempNum, RV);
                }
            }
            BDLoadingProcess = false;
        }
        /// <summary>
        /// Чтение SGTIN
        /// </summary>
        public string ParseSGTINFromOriginalSGTIN(string OriginalSGTIN)
        {
            string SGTINNumber = "";
            for (int i = 0; i < OriginalSGTIN.Length; i++)
            {
                if (i == 2 || i == 3 || i == 4 || i == 5 || i == 6 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14 || i == 15 || i == 18 || i == 19 || i == 20 || i == 21 || i == 22 || i == 23 || i == 24 || i == 25 || i == 26 || i == 27 || i == 28 || i == 29 || i == 30)
                {
                    SGTINNumber += OriginalSGTIN[i];
                }
            }
            return SGTINNumber;
        }
        /// <summary>
        /// Поиск адреса откуда совершалось выбытие
        /// </summary>
        /// <param name="SRID"></param>
        /// <returns></returns>
        public async void SearchDisposalLocationAsync(RV RV)
        {
            string LONGITUDE = "";
            string LATITUDE = "";
            foreach (var item in RV.SatelliteInfoList)
            {
                if (item.LONGITUDE != "" && item.LATITUDE != "")
                {
                    LONGITUDE = $"{item.LONGITUDE}";
                    LATITUDE = $"{item.LATITUDE}";
                    break;
                }
            }
            CultureInfo temp_culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            double lon = double.Parse(LONGITUDE);
            double lat = double.Parse(LATITUDE);
            Thread.CurrentThread.CurrentCulture = temp_culture;

            string token = "bd9d5b3910ab1cf76b119d60b0315b4d45993764";
            SuggestClientAsync api = new SuggestClientAsync(token);
            var result = await api.Geolocate(lat, lon, 50, 1);
            if (result.suggestions.Count != 0)
            {
                RV.RVLocation = result.suggestions[0].unrestricted_value.ToString();
            }
        }
        /// <summary>
        /// Парс оригинального SGTIN
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        public string SGTINLogParse(string Log)
        {
            int StartCount = 0;
            char StartVerify = Convert.ToChar(";");
            char EndVerify = Convert.ToChar("=");
            string OriginalSGTIN = "";
            for (int i = 0; i < Log.Length; i++)
            {
                if (StartCount == 3) OriginalSGTIN += Log[i];
                if (Log[i] == StartVerify) StartCount++;
                if (Log[i] == EndVerify) break;
            }
            return OriginalSGTIN;
        }
        /// <summary>
        /// Чтение TextFromFile и загрузка данных о SRID в базу
        /// </summary>
        public void LoadSRID(ref int SRIDTempNum, RV RV)
        {
            int Line;

            for (Line = 0; Line < TextFromFile.Length; Line++)
            {
                if (TextFromFile[Line].Contains("librv_begin_disposal_report()"))
                {
                    string[] TempDisposalReport = new string[600];
                    Line = LoadTempDisposalReport(Line, TempDisposalReport);
                    RV.SRIDLIST.Add(CreateSRID());
                    LoadDataAboutSRID(SRIDTempNum, TempDisposalReport, RV);
                    LoadDataAboutVerifySRID(Line, SRIDTempNum, RV);
                    LoadDataAboutIP(SRIDTempNum, RV);
                    LoadSatteliteData(SRIDTempNum, RV);
                    SRIDTempNum++;
                }
            }
        }
        /// <summary>
        /// Создание нового SRID
        /// </summary>
        /// <returns></returns>
        public SRID CreateSRID()
        {
            SRID SRID = new SRID(SGTINLIST = new List<SGTIN>(), "", "", "", "", "", DateTime.MinValue, DateTime.MinValue, "", "", "", "", false, "", false, DateTime.MinValue);
            return SRID;
        }

        /// <summary>
        /// Создание нового SGTIN
        /// </summary>
        /// <returns></returns>
        public SGTIN CreateSGTIN()
        {
            SGTIN SGTIN = new SGTIN("", "");
            return SGTIN;
        }

        /// <summary>
        /// Чтение файла и запись в TextFromFile
        /// </summary>
        public void ReadFile(string Path)
        {
            TextFromFile = new string[1];
            
            using (Stream fStream = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                TextFromFile = File.ReadAllLines(Path, Encoding.UTF8);
            }
            
        }
        /// <summary>
        /// Загрузка куска логов с отчётом о выбытии в TempDisposalReport
        /// </summary>
        public int LoadTempDisposalReport(int Line, string[] TempDisposalReport)
        {
            int TempLineEnd;
            int TempLine;
            int i = 0;

            for (TempLineEnd = Line; TempLineEnd < TextFromFile.Length; TempLineEnd++)
            {
                if (TextFromFile[TempLineEnd].Contains("Bad file descriptor") || TextFromFile[TempLineEnd].Contains("Связь со спутником отсутствует более 24 часов") || TextFromFile[TempLineEnd].Contains("МБ не инициализирован")) 
                {
                    break;
                }
                if (TextFromFile[TempLineEnd].Contains("librv_close_disposal_report()")) break;
            }

            for (TempLine = Line; TempLine < TempLineEnd; TempLine++)
            {
                TempDisposalReport[i] = TextFromFile[TempLine];
                i++;
            }
            return TempLineEnd;
        }

        /// <summary>
        /// Заполнение данными SRIDа (SRID, SUID, DONM, DODT, DOSE, SGTINNumber, OriginalSGTIN)
        /// </summary>
        /// <param name="SRIDTempNum"></param>
        public void LoadDataAboutSRID(int SRIDTempNum, string[] TempDisposalReport, RV RV)
        {
            for (int i = 0; i < TempDisposalReport.Length; i++)
            {
                if (TempDisposalReport[i] != null)
                {
                    if (TextFromFile[i].Contains("МБ не инициализирован") || TextFromFile[i].Contains("Связь со спутником отсутствует более 24 часов") || TextFromFile[i].Contains("Bad file descriptor"))
                    {
                        break;
                    }
                    if (TempDisposalReport[i].Contains("librv_begin_disposal_report()"))
                    {
                        RV.SRIDLIST[SRIDTempNum].DisposalStartDate = ParseDate(DateParse(TempDisposalReport[i]));
                    }
                    if (TempDisposalReport[i].Contains("LIBRV_PARAM_BASE_DOCUMENT_FIELD_ID (65611) = SRID"))
                    {
                        RV.SRIDLIST[SRIDTempNum].SRIDNumber = EndLogParse(TempDisposalReport[i + 1]);
                    }
                    if (TempDisposalReport[i].Contains("LIBRV_PARAM_BASE_DOCUMENT_FIELD_ID (65611) = SUID"))
                    {
                        RV.SRIDLIST[SRIDTempNum].SUID = EndLogParse(TempDisposalReport[i + 1]);
                    }
                    if (TempDisposalReport[i].Contains("LIBRV_PARAM_BASE_DOCUMENT_FIELD_ID (65611) = DONM"))
                    {
                        RV.SRIDLIST[SRIDTempNum].DONM = EndLogParse(TempDisposalReport[i + 1]);
                    }
                    if (TempDisposalReport[i].Contains("LIBRV_PARAM_BASE_DOCUMENT_FIELD_ID (65611) = DODT"))
                    {
                        RV.SRIDLIST[SRIDTempNum].DODT = EndLogParse(TempDisposalReport[i + 1]);
                    }
                    if (TempDisposalReport[i].Contains("LIBRV_PARAM_BASE_DOCUMENT_FIELD_ID (65611) = DOSE"))
                    {
                        RV.SRIDLIST[SRIDTempNum].DOSE = EndLogParse(TempDisposalReport[i + 1]);
                    }
                    if (TempDisposalReport[i].Contains("librv_add_marking_code()"))
                    {
                        string OriginalSGTINNumber = SGTINLogParse(TempDisposalReport[i + 1]);
                        string SGTINNumber = ParseSGTINFromOriginalSGTIN(OriginalSGTINNumber);
                        SGTIN SGTIN = new SGTIN(SGTINNumber, OriginalSGTINNumber);
                        RV.SRIDLIST[SRIDTempNum].SGTINLIST.Add(SGTIN);
                    }
                }
            }

        }

        /// <summary>
        /// Проверка выбылся ли отчёт и когда
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="SRIDTempNum"></param>
        public void LoadDataAboutVerifySRID(int Line, int SRIDTempNum, RV RV)
        {
            bool Flag = false;

            for (int i = Line; i < TextFromFile.Length; i++)
            {
                if (TextFromFile[i].Contains("МБ не инициализирован") || TextFromFile[i].Contains("Связь со спутником отсутствует более 24 часов") || TextFromFile[i].Contains("Bad file descriptor"))
                {
                    break;
                }
                if (TextFromFile[i].Contains("LIBRV_PARAM_DISPOSAL_REPORT_STATUS (65650) = 2") && Flag == false)
                {
                    RV.SRIDLIST[SRIDTempNum].ReportNumber = EndLogParse(TextFromFile[i + 1]);
                    Flag = true;
                }
                if (Flag)
                {
                    foreach (var item in RV.MKInfoList)
                    {
                        if (Convert.ToInt32(item.PROCESSED_DOCUMENTS_COUNT) >= Convert.ToInt32(RV.SRIDLIST[SRIDTempNum].ReportNumber))
                        {
                            RV.SRIDLIST[SRIDTempNum].DisposalReport = true;
                            RV.SRIDLIST[SRIDTempNum].DisposalEndDate = item.Date;
                            break;
                        }
                    }
                    break;
                }

            }
        }

        /// <summary>
        /// Заполнение данных по спутникам в момент выбытия
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="SRIDTempNum"></param>
        public void LoadSatteliteData(int SRIDTempNum, RV RV)
        {
            foreach (var item in RV.SatelliteInfoList)
            {
                if (item.DateTime <= RV.SRIDLIST[SRIDTempNum].DisposalStartDate || RV.SRIDLIST[SRIDTempNum].LastSatellitesFoundDate == DateTime.MinValue)
                {
                    RV.SRIDLIST[SRIDTempNum].LastSatellitesFoundDate = item.DATE_TIME;
                }
            }
            foreach (var item in RV.MBInfoList)
            {
                if (item.Date <= RV.SRIDLIST[SRIDTempNum].DisposalStartDate && item.SU_SUSPENDED == "false")
                {
                    RV.SRIDLIST[SRIDTempNum].SatellitesFound = true;
                }
            }
        }

        /// <summary>
        /// Заполнение данных по IP
        /// </summary>
        public void LoadDataAboutIP(int SRIDTempNum, RV RV)
        {
            foreach (var item in RV.Emission_Server_SettingsList)
            {
                if (RV.SRIDLIST[SRIDTempNum].DisposalStartDate <= item.DateTime || RV.SRIDLIST[SRIDTempNum].EmissionServerIP == "")
                {
                    RV.SRIDLIST[SRIDTempNum].EmissionServerIP = item.PARAM_HOST;
                    RV.SRIDLIST[SRIDTempNum].EmissionServerPort = item.PARAM_PORT;
                }
            }
            foreach (var item in RV.Registration_Server_SettingsList)
            {
                if (RV.SRIDLIST[SRIDTempNum].DisposalStartDate <= item.DateTime || RV.SRIDLIST[SRIDTempNum].RegistrationServerIP == "")
                {
                    RV.SRIDLIST[SRIDTempNum].RegistrationServerIP = item.PARAM_HOST;
                    RV.SRIDLIST[SRIDTempNum].RegistrationServerPort = item.PARAM_PORT;
                }
            }
        }
        /// <summary>
        /// Загрузка данных о сервере эмиссии
        /// </summary>
        /// <param name="Line"></param>
        public void LoadEmission_server_settings(int Line, RV RV)
        {
            if (Line + 7 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 7; i++)
                {
                    if (TextFromFile[i].Contains("LIBRV_PARAM_HOST") && TextFromFile[i + 7] != "")
                    {
                        RV.Emission_Server_SettingsList.Add(new Emission_server_settings(ParseDate(DateParse(TextFromFile[i])),
                            EndLogParse(TextFromFile[i]),
                            EndLogParse(TextFromFile[i + 1])));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузка данных о сервере эмиссии
        /// </summary>
        /// <param name="Line"></param>
        public void LoadRegistration_server_settings(int Line, RV RV)
        {
            if (Line + 7 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 7; i++)
                {
                    if (TextFromFile[i].Contains("LIBRV_PARAM_HOST") && TextFromFile[i + 1] != "")
                    {
                        RV.Registration_Server_SettingsList.Add(new Registration_server_settings(ParseDate(DateParse(TextFromFile[i])),
                            EndLogParse(TextFromFile[i]),
                            EndLogParse(TextFromFile[i + 1])));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузка данных об устройстве в БД
        /// </summary>
        public void LoadDeviceInfo(int Line, RV RV)
        {
            if (Line + 30 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 30; i++)
                {
                    if (TextFromFile[i].Contains("INFO  [MarkingRegistrar] < LIBRV_PARAM_SERIAL_NUMBER (65547)") && TextFromFile[i + 8] != "")
                    {
                        RV.DeviceInfoList.Add(new DeviceInfo(ParseDate(DateParse(TextFromFile[i])),
                            EndLogParse(TextFromFile[i]),
                            EndLogParse(TextFromFile[i + 1]),
                            EndLogParse(TextFromFile[i + 2]),
                            EndLogParse(TextFromFile[i + 3]),
                            EndLogParse(TextFromFile[i + 4]),
                            EndLogParse(TextFromFile[i + 5]),
                            EndLogParse(TextFromFile[i + 6]),
                            EndLogParse(TextFromFile[i + 7]),
                            EndLogParse(TextFromFile[i + 8]))
                            );
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Загрузка данных о спутниках
        /// </summary>
        public void LoadSatteliteInfo(int Line, RV RV)
        {
            if (Line + 18 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 18; i++)
                {
                    if (TextFromFile[i].Contains("INFO  [MarkingRegistrar] < LIBRV_PARAM_SATELLITES_FOUND (65638)") && TextFromFile[i + 12] != "")
                    {
                        RV.SatelliteInfoList.Add(new SatteliteInfo(ParseDate(DateParse(TextFromFile[i])),
                            Convert.ToBoolean(EndLogParse(TextFromFile[i])),
                            ParseDate(EndLogParse(TextFromFile[i + 1])),
                            EndLogParse(TextFromFile[i + 2]),
                            EndLogParse(TextFromFile[i + 3]),
                            EndLogParse(TextFromFile[i + 4]),
                            EndLogParse(TextFromFile[i + 5]),
                            EndLogParse(TextFromFile[i + 6]),
                            EndLogParse(TextFromFile[i + 7]),
                            EndLogParse(TextFromFile[i + 8]),
                            EndLogParse(TextFromFile[i + 9]),
                            EndLogParse(TextFromFile[i + 10]),
                            EndLogParse(TextFromFile[i + 11]),
                            EndLogParse(TextFromFile[i + 12])
                            ));
                        break;
                    }

                }
            }

        }

        /// <summary>
        /// Загрузить данные о МБ
        /// </summary>
        /// <param name="Line"></param>
        public void LoadMBInfo(int Line, RV RV)
        {
            if (Line + 24 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 24; i++)
                {
                    if (TextFromFile[i].Contains("INFO  [MarkingRegistrar] < LIBRV_PARAM_SU_STATE (65536)") && TextFromFile[i + 19] != "")
                    {
                        RV.MBInfoList.Add(new MB(ParseDate(DateParse(TextFromFile[i])),
                            EndLogParse(TextFromFile[i]),
                            EndLogParse(TextFromFile[i + 1]),
                            EndLogParse(TextFromFile[i + 2]),
                            EndLogParse(TextFromFile[i + 3]),
                            EndLogParse(TextFromFile[i + 4]),
                            EndLogParse(TextFromFile[i + 5]),
                            EndLogParse(TextFromFile[i + 6]),
                            EndLogParse(TextFromFile[i + 7]),
                            EndLogParse(TextFromFile[i + 8]),
                            EndLogParse(TextFromFile[i + 9]),
                            EndLogParse(TextFromFile[i + 10]),
                            EndLogParse(TextFromFile[i + 11]),
                            EndLogParse(TextFromFile[i + 12]),
                            EndLogParse(TextFromFile[i + 13]),
                            EndLogParse(TextFromFile[i + 14]),
                            EndLogParse(TextFromFile[i + 15]),
                            EndLogParse(TextFromFile[i + 16]),
                            EndLogParse(TextFromFile[i + 17]),
                            EndLogParse(TextFromFile[i + 18]),
                            EndLogParse(TextFromFile[i + 19])));
                        break;
                    }

                }
            }

        }

        /// <summary>
        /// Загрузить данные о МК
        /// </summary>
        /// <param name="Line"></param>
        public void LoadMKInfo(int Line, RV RV)
        {
            if (Line + 33 < TextFromFile.Length)
            {
                for (int i = Line; i < Line + 33; i++)
                {
                    if (TextFromFile[i].Contains("INFO  [MarkingRegistrar] < LIBRV_PARAM_INITIALIZED (65559)") && TextFromFile[i + 23] != "")
                    {
                        RV.MKInfoList.Add(new MK(ParseDate(DateParse(TextFromFile[i])),
                            EndLogParse(TextFromFile[i]),
                            EndLogParse(TextFromFile[i + 1]),
                            EndLogParse(TextFromFile[i + 2]),
                            EndLogParse(TextFromFile[i + 3]),
                            EndLogParse(TextFromFile[i + 4]),
                            EndLogParse(TextFromFile[i + 5]),
                            EndLogParse(TextFromFile[i + 6]),
                            EndLogParse(TextFromFile[i + 7]),
                            EndLogParse(TextFromFile[i + 8]),
                            EndLogParse(TextFromFile[i + 9]),
                            EndLogParse(TextFromFile[i + 10]),
                            EndLogParse(TextFromFile[i + 11]),
                            EndLogParse(TextFromFile[i + 12]),
                            EndLogParse(TextFromFile[i + 13]),
                            EndLogParse(TextFromFile[i + 14]),
                            EndLogParse(TextFromFile[i + 15]),
                            EndLogParse(TextFromFile[i + 16]),
                            EndLogParse(TextFromFile[i + 17]),
                            EndLogParse(TextFromFile[i + 18]),
                            EndLogParse(TextFromFile[i + 19]),
                            EndLogParse(TextFromFile[i + 20]),
                            EndLogParse(TextFromFile[i + 21]),
                            EndLogParse(TextFromFile[i + 22]),
                            EndLogParse(TextFromFile[i + 23])));
                        break;
                    }

                }
            }

        }
        /// <summary>
        /// Парс даты и времени в начале лога
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        public string DateParse(string Log)
        {
            string OutLog = $"{Log[0]}{Log[1]}{Log[2]}{Log[3]}{Log[4]}{Log[5]}{Log[6]}{Log[7]}{Log[8]}{Log[9]}{Log[10]}{Log[11]}{Log[12]}{Log[13]}{Log[14]}{Log[15]}{Log[16]}{Log[17]}{Log[18]}";

            return OutLog;
        }
        /// <summary>
        /// Парс конца лога по знаку = 
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        public string EndLogParse(string Log)
        {
            char EndVeryfy = Convert.ToChar("=");
            bool Flag = false;
            string EndLog = "";
            for (int i = 0; i < Log.Length; i++)
            {
                if (Flag) EndLog += Log[i];
                if (Log[i] == EndVeryfy)
                {
                    i++;
                    Flag = true;
                }
            }
            return EndLog;
        }
        /// <summary>
        /// Парс даты и времени в DateTime
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        public DateTime ParseDate(string Log)
        {
            DateTime Date = DateTime.Parse(Log);

            return Date;
        }
    }
}

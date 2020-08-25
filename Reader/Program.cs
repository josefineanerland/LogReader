using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reader
{
    class Program
    {

        static void Main(string[] args)
        {
            ReadLog(@"C:\Users\Svea User\Desktop\Test-logfile.txt");
        }

        static void ReadLog(string file)
        {
            List<LogModel> logList = new List<LogModel>();
            List<ObservationModel> obsList = new List<ObservationModel>();

            try
            {
                // read file
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {

                        string line;
                        // read and split all rows
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!line.StartsWith("#") && line.Length >= 3)
                            {
                                // split each line of text-file
                                string[] split = line.Split('\t');
                                if (split.Length == 4)
                                {
                                    LogModel logModel = new LogModel();
                                    logModel.TimeStamp = TimeSpan.Parse(split[0]);
                                    //check if sessionID contains letters, otherwise skip
                                    if (Int32.TryParse(split[1], out int s))
                                    {
                                        logModel.SessionId = s;
                                    }
                                    logModel.Event = split[2];
                                    logModel.Data = split[3];

                                    logList.Add(logModel);
                                }
                                else
                                // in case of whitespace between values instead of tab
                                {
                                    LogModel logModel = new LogModel();
                                    char[] chars = { '\t', ' ' };
                                    string time = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(time.Length + 1);
                                    string sess = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(sess.Length + 1);
                                    string ev = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(ev.Length + 1);
                                    string data = line;

                                    logModel.TimeStamp = TimeSpan.Parse(time);
                                    logModel.SessionId = Int32.Parse(sess);
                                    logModel.Event = ev;
                                    logModel.Data = data;
                                    logList.Add(logModel);
                                }
                            }

                        }
                    }
                }
                // observation-list

                IEnumerable<int> sessionList = logList.Select(s => s.SessionId).Distinct().ToList();

                foreach (int sess in sessionList)
                {
                    List<LogModel> logs = logList.FindAll(l => l.SessionId == sess);

                    for (int i = 0; i < logs.Count(); i++)
                    {
                        if (logs[i].Event == "SUCCESS" || logs[i].Event == "FAIL" && logs.FindAll(x => x.Event == "CONNECT").Count >= 1)
                        {
                            ObservationModel observation = new ObservationModel();
                            observation.IPadress = logs.Find(o => o.Event == "CONNECT").Data;
                            observation.UserName = logs[i - 1].Data;
                            observation.OutCome = logs[i].Event;
                            observation.TimeStamp = logs[i].TimeStamp;

                            obsList.Add(observation);
                        }
                    }

                }
            }

            // in case of error
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {

                //print observations
                foreach (ObservationModel item in obsList)
                {
                    Console.WriteLine(item.TimeStamp + " " + item.OutCome + " " + item.IPadress + " " + item.UserName);
                }

            }
        }
    }

}

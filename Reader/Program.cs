﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reader
{
    class Program
    {

        static void ReadLog(string file)
        {
            List<logModel> logList = new List<logModel>();
            List<observationModel> obsList = new List<observationModel>();

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
                                    logModel logModel = new logModel();
                                    logModel.timeStamp = split[0];
                                    logModel.sessionId = Int32.Parse(split[1]);
                                    logModel.Event = split[2];
                                    logModel.Data = split[3];

                                    logList.Add(logModel);
                                }
                                else
                                // in case of whitespace between values instead of tab
                                {
                                    logModel logModel = new logModel();
                                    char[] chars = { '\t', ' ' };
                                    string time = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(time.Length + 1);
                                    string sess = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(sess.Length + 1);
                                    string ev = line.Substring(0, line.IndexOfAny(chars));
                                    line = line.Substring(ev.Length + 1);
                                    string data = line;

                                    logModel.timeStamp = time;
                                    logModel.sessionId = Int32.Parse(sess);
                                    logModel.Event = ev;
                                    logModel.Data = data;
                                    logList.Add(logModel);
                                }
                            }

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
                // observation-list

                IEnumerable<int> sessionList = logList.Select(s => s.sessionId).Distinct().ToList();


                foreach (int sess in sessionList)
                {
                    
                    List<logModel> logs = logList.FindAll(l => l.sessionId == sess);

                    for (int i = 0; i < logs.Count(); i++)
                    {
                        if (logs[i].Event == "SUCCESS" || logs[i].Event == "FAIL" && logs.FindAll(x => x.Event == "CONNECT").Count >= 1)
                        {
                            observationModel observation = new observationModel();
                            observation.IPadress = logs.Find(o => o.Event == "CONNECT").Data;
                            observation.userName = logs[i - 1].Data;
                            observation.outCome = logs[i].Event;
                            observation.timeStamp = logs[i].timeStamp;

                            obsList.Add(observation);
                        }
                    }

                }
                //print observations
                foreach (observationModel item in obsList)
                {
                    Console.WriteLine(item.timeStamp + " " + item.outCome + " " + item.IPadress + " " + item.userName);
                }

            }
        }

        static void Main(string[] args)
        {
            ReadLog(@"C:\Users\Svea User\Desktop\Test-logfile.txt");
        }
    }

}

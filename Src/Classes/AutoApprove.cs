using System;
using SimplifikasiFID.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using System.Data.Entity;

namespace SimplifikasiFID.Classes
{
    public class AutoApprove : IJob
    {
        SimplyFIDEntities db = new SimplyFIDEntities();
        public Task Execute(IJobExecutionContext context)
        {
            //AutoAppv();
            var task = Task.Run(() => logfile(DateTime.Now.ToString("dd-MMMM-yyyy HH':'mm':'ss")));
            return task;

        }

        public void logfile(string time)
        {
            string path = "D:\\APP_DATA.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(time);
                writer.Close();
            }
        }

        public void GetList()
        {
            try
            {
                var query = db.T_FID.OrderByDescending(x => x.CreatedDate).ToList();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string>AutoAppv()
        {
            try
            {
                //logfile.WriteFileLog(DateTime.Now + " Service Is Started.");
                var query = await db.T_FID.Where(x => x.Status >= 5 && x.Status <= 9).ToListAsync();

                foreach (var day in query)
                {
                    var today = DateTime.Now.ToString("ddd");
                    var days = day.UpdateDate.Value.ToString("ddd");

                    var datediff = DateTime.Now.Subtract(day.UpdateDate ?? DateTime.Now).Days;

                    switch (today)
                    {
                        case ("Mon"):
                            Sunday(day, datediff);
                            break;
                        case ("Tue"):
                            Monday(day, datediff);
                            break;
                        case ("Wed"):
                            WeekDays(day, datediff);
                            break;
                        case ("Thu"):
                            WeekDays(day, datediff);
                            break;
                        case ("Fri"):
                            WeekDays(day, datediff);
                            break;

                    }
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return "not Ok";
                //log.WriteFileLog(DateTime.Now + " Exception : " + ex.Message.ToString());
            }
            //finally
            //{
            //    //log.WriteFileLog(DateTime.Now + " Service Is Closed.");
            //    Environment.Exit(-1);
            //}
        }

        public void Sunday(T_FID query, int datediff)
        {
            if (datediff == 4)
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var tes = query;
                        var nofid = query.NoFID;

                        query.UpdateDate = DateTime.Now;
                        query.UpdateUser = "SYSTEM";


                        if (query.Jenisfid == "BD")
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 6;
                            }
                            else if (query.Status == 6)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 9;
                            }
                        }
                        else
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 9;
                            }
                            else if (query.Status == 9)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 6;
                            }
                        }

                        T_Catatan notes = new T_Catatan();
                        notes.NoFID = nofid;
                        notes.Catatan = "Approved by SYSTEM";
                        notes.CreatedDate = DateTime.Now;
                        notes.CreatedUser = "SYSTEM";


                        db.T_Catatan.Add(notes);
                        db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new Exception("Error : " + ex.Message);
                    }

                }

            }

        }

        public void Monday(T_FID query, int datediff)
        {
            if (datediff == 4)
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var tes = query;
                        var nofid = query.NoFID;

                        query.UpdateDate = DateTime.Now;
                        query.UpdateUser = "SYSTEM";

                        if (query.Jenisfid == "BD")
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 6;
                            }
                            else if (query.Status == 6)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 9;
                            }
                        }
                        else
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 9;
                            }
                            else if (query.Status == 9)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 6;
                            }
                        }

                        T_Catatan notes = new T_Catatan();
                        notes.NoFID = nofid;
                        notes.Catatan = "Approved by SYSTEM";
                        notes.CreatedDate = DateTime.Now;
                        notes.CreatedUser = "SYSTEM";


                        db.T_Catatan.Add(notes);
                        db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new Exception("Error : " + ex.Message);
                    }

                }
            }

        }

        public void WeekDays(T_FID query, int datediff)
        {
            if (datediff == 2)
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var tes = query;
                        var nofid = query.NoFID;

                        query.UpdateDate = DateTime.Now;
                        query.UpdateUser = "SYSTEM";

                        if (query.Jenisfid == "BD")
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 6;
                            }
                            else if (query.Status == 6)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 9;
                            }
                        }
                        else
                        {
                            if (query.Status == 5)
                            {
                                query.Status = 9;
                            }
                            else if (query.Status == 9)
                            {
                                query.Status = 7;
                            }
                            else if (query.Status == 7)
                            {
                                query.Status = 8;
                            }
                            else if (query.Status == 8)
                            {
                                query.Status = 6;
                            }
                        }

                        T_Catatan notes = new T_Catatan();
                        notes.NoFID = nofid;
                        notes.Catatan = "Approved by SYSTEM";
                        notes.CreatedDate = DateTime.Now;
                        notes.CreatedUser = "SYSTEM";


                        db.T_Catatan.Add(notes);
                        db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new Exception("Error : " + ex.Message);
                    }

                }
            }
        }

    }
}
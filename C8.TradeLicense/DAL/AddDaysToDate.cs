using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Models;
using System.Globalization;

namespace C8.TradeLicense.DAL
{
    public class AddDaysToDate
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public string GetMonth(int monthNo)
        {
            monthNo = monthNo - 1;
            List<string> monthList = new List<string>();
            monthList.Add("January");
            monthList.Add("February");
            monthList.Add("March");
            monthList.Add("April");
            monthList.Add("May");
            monthList.Add("June");
            monthList.Add("July");
            monthList.Add("August");
            monthList.Add("September");
            monthList.Add("October");
            monthList.Add("November");
            monthList.Add("December");

            return monthList[monthNo];
        }
        public string SwapSystemDate(DateTime dateToSwap)
        {
            string Day = dateToSwap.Day.ToString();
            string Mon = dateToSwap.Month.ToString();
            string Year = dateToSwap.Year.ToString();
            if (Mon.Length == 1)
            {
                Mon = "0" + Mon;
            }
            if (Day.Length == 1)
            {
                Day = "0" + Day;
            }

            return Day + "/" + Mon + "/" + Year; ;

        }

        public string AddDays(DateTime dateToAddDays, int daysToAdd)
        {
            var newdate = "";
            //dateToAddDays = dateToAddDays.AddDays(1);
            try
            {
                DateTime DueDate = dateToAddDays.Date.AddDays(daysToAdd);
                List<DateTime> dates = new List<DateTime>();

                List<string> excludepublicHolidays = new List<string>(); // list of public holidays 


                var holidays = db.PublicHolidays;
                foreach (var item in holidays)
                {
                    excludepublicHolidays.Add(item.Date + " " + DateTime.Now.Year.ToString());
                }
                for (var dt = dateToAddDays; dt <= DueDate; dt = dt.AddDays(1))
                {
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        DueDate = DueDate.AddDays(1);

                    }
                    else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        DueDate = DueDate.AddDays(1);

                    }
                    if (dt.DayOfWeek != DayOfWeek.Saturday)
                    {
                        string currentDayMonth = dt.Day + " " + GetMonth(dt.Month) + " " + dt.Year;
                        var matches = excludepublicHolidays.Where(v => excludepublicHolidays.Equals(currentDayMonth)).Count();
                        DueDate = DueDate.AddDays(matches);

                    }
                    dates.Add(dt);
                }
                newdate = DueDate.ToString();



            }
            catch (Exception ex)
            {
                var e = ex.Message;
                //newdate = null;
            }
            return newdate;
        }
  
    }
    


}
using Newtonsoft.Json;
using p002.Models;
using p002.Models.Data;
using p002.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ActionNameAttribute = System.Web.Mvc.ActionNameAttribute;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace p002.Controllers
{
    public class DataController : Controller
    {
        private readonly CovidApiService _covidApiService;
        public DataController(
            CovidApiService covidApiService
            )
        {
            _covidApiService = covidApiService;
        }


        [HttpPost]
        [ActionName("getDailyByCountry")]
        public ActionResult DailyByCountry(DailyByCountryRequest request)
        {
            var response = new DailyByCountryResponse();
            request.StartDate = request.StartDate.AddDays(-1);
            if(request.EndDate.Date < DateTime.Now.Date)
            {
                request.EndDate.AddDays(1);
            }
            response.Value = _covidApiService.DailyByCountryGet(request);
            

            if(response.Value == null)
            {
                response.ErrorList.Add("Bir hata yasandi!");
            }
            else{
                List<int> caseList = new List<int>();

                for (int i = 1; i < response.Value.Count - 1; i++)
                {
                    caseList.Add(response.Value[i].Cases - response.Value[i - 1].Cases);
                }
                for (int i = 1; i < response.Value.Count - 1; i++)
                {
                    response.Value[i].Cases = caseList[i - 1];
                }
                if (response.Value.Count<2)
                {
                    response.ErrorList.Add("Yeterli veri bulunamadi!");
                }
                else
                {
                    response.Value = response.Value.GetRange(1, response.Value.Count - 2);
                }
                
            }
            return Json(JsonConvert.SerializeObject(response), JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        [ActionName("getCountries")]
        public ActionResult GetCountries()
        {
            var response = new GetCountriesResponse();
            response.Value = _covidApiService.GetCountries();
            if(response.Value == null)
            {
                response.ErrorList.Add("Bir hata yasandi!");
            }
            return Json(JsonConvert.SerializeObject(response), JsonRequestBehavior.AllowGet);
        }

        //{{baseUrl}}/live/country/{{country}}/status/{{status}}
        [HttpPost]
        [ActionName("getLiveByCountryAllStatus")]
        public ActionResult GetLiveByCountryAllStatus(GetLiveByCountryAllStatusRequest request)
        {
            var response = new GetLiveByCountryAllStatusResponse();

            response.Value = _covidApiService.GetLiveByCountryAllStatuses(request);
            if (response.Value == null || response.Value.Count == 0)
            {
                response.ErrorList.Add("Bir hata yasandi!");
            }
            else
            {
                var returnList = new List<GetLiveByCountryAllStatus>();

                response.Value.ForEach(val =>
                {
                    var foundedVal = returnList.FindIndex(r => r.Date.Date == val.Date.Date);
                    if (foundedVal != -1)
                    {
                        returnList.ElementAt(foundedVal).Active += val.Active;
                        returnList.ElementAt(foundedVal).Deaths += val.Deaths;
                        returnList.ElementAt(foundedVal).Recovered += val.Recovered;
                    }
                    else
                    {
                        returnList.Add(val);
                    }
                });

                if(request.GraphType == GraphTypes.LastThirtyDaysDeath || request.GraphType == GraphTypes.LastThirtyDaysRecovery)
                {
                    List<GetLiveByCountryAllStatus> caseList = new List<GetLiveByCountryAllStatus>();

                    for (int i = 1; i < returnList.Count; i++)
                    {
                        caseList.Add(new Models.Data.GetLiveByCountryAllStatus
                        {
                            Active = returnList[i].Active - returnList[i - 1].Active,
                            Recovered = returnList[i].Recovered - returnList[i - 1].Recovered,
                            Deaths = returnList[i].Deaths - returnList[i - 1].Deaths
                        });
                    }
                    for (int i = 1; i < response.Value.Count && 1 >=response.Value.Count; i++)
                    {
                        returnList[i].Active = caseList[i - 1].Active;
                        returnList[i].Recovered = caseList[i - 1].Recovered;
                        returnList[i].Deaths = caseList[i - 1].Deaths;
                    }
                    if (response.Value.Count < 1)
                    {
                        response.ErrorList.Add("Yeterli veri bulunamadi!");
                    }
                    else
                    {
                        response.Value = returnList.GetRange(1, response.Value.Count - 1);
                    }
                }
                else
                {
                    response.Value = returnList;
                }

                var deneme = "";
            }
            return Json(JsonConvert.SerializeObject(response), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("getTotalDailyByCountry")]
        public ActionResult GetTotalDailyByCountry(DailyByCountryRequest request)
        {
            var response = new ByCountryTotalAllStatusResponse();
            request.StartDate = request.StartDate.Date.AddDays(-1);
            request.StartDate.Subtract(TimeSpan.FromDays(1));
            if (request.EndDate.Date < DateTime.Now.Date)
            {
                request.EndDate.AddDays(1);
            }
            response.Value = _covidApiService.ByCountryTotalAllStatus(request);


            if (response.Value == null)
            {
                response.ErrorList.Add("Bir hata yasandi!");
            }
            else
            {
                var returnList = new List<ByCountryTotalAllStatus>();

                response.Value.ForEach(val =>
                {
                    var foundedVal = returnList.FindIndex(r => r.Date.Date == val.Date.Date);
                    if (foundedVal != -1)
                    {
                        returnList.ElementAt(foundedVal).Active += val.Active;
                        returnList.ElementAt(foundedVal).Deaths += val.Deaths;
                        returnList.ElementAt(foundedVal).Recovered += val.Recovered;
                    }
                    else
                    {
                        returnList.Add(val);
                    }
                });

                if (request.GraphType == GraphTypes.AllDaysDeath || request.GraphType == GraphTypes.AllDaysRecovery)
                {
                    var caseList = new List<ByCountryTotalAllStatus>();

                    for (int i = 1; i < response.Value.Count; i++)
                    {
                        caseList.Add(new Models.Data.ByCountryTotalAllStatus
                        {
                            Active = response.Value[i].Active - response.Value[i - 1].Active,
                            Recovered = response.Value[i].Recovered - response.Value[i - 1].Recovered,
                            Deaths = response.Value[i].Deaths - response.Value[i - 1].Deaths
                        });
                    }
                    for (int i = 1; i < response.Value.Count; i++)
                    {
                        response.Value[i].Active = caseList[i - 1].Active;
                        response.Value[i].Recovered = caseList[i - 1].Recovered;
                        response.Value[i].Deaths = caseList[i - 1].Deaths;
                    }
                    if (response.Value.Count < 1)
                    {
                        response.ErrorList.Add("Yeterli veri bulunamadi!");
                    }
                    else
                    {
                        response.Value = returnList.GetRange(1, response.Value.Count - 1);
                    }
                   
                }
                else
                {
                    response.Value = returnList;
                }
            }
            return Json(JsonConvert.SerializeObject(response), JsonRequestBehavior.AllowGet);
        }

    }
}
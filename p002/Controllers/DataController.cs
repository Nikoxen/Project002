using Newtonsoft.Json;
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
                    var deneme = i;
                    var curr = response.Value[i].Cases;
                    var prev = response.Value[i-1].Cases;
                    caseList.Add(response.Value[i].Cases - response.Value[i - 1].Cases);
                }
                for (int i = 1; i < response.Value.Count - 1; i++)
                {
                    response.Value[i].Cases = caseList[i - 1];
                }

                response.Value = response.Value.GetRange(1, response.Value.Count - 2);
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
            if (response.Value == null)
            {
                response.ErrorList.Add("Bir hata yasandi!");
            }
            return Json(JsonConvert.SerializeObject(response), JsonRequestBehavior.AllowGet);
        }
    }
}
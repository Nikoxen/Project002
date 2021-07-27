using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace p002.Models.Data
{
    public class DailyByCountryRequest
    {
        public string CountryCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GraphTypes GraphType { get; set; }
    }

    public class DailyByCountryApiResponse
    {
        public int Cases { get; set; }
        public DateTime Date { get; set; }
    }

    public class DailyByCountryResponse : BaseResponse<List<DailyByCountryApiResponse>>
    {
        
    }

    public class ByCountryTotalAllStatusResponse : BaseResponse<List<ByCountryTotalAllStatus>> { }

    public class SingleCountry
    {
        public string Country { get; set; }
        public string Slug { get; set; }
        public string ISO2 { get; set; }
    }

    public class GetCountriesResponse : BaseResponse<List<SingleCountry>> { }


    public class GetLiveByCountryAllStatus
    {
        public int Active { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public string CountryCode { get; set; }
        public DateTime Date { get; set; }

    }

    public class GetLiveByCountryAllStatusRequest
    {
        public string CountryCode { get; set; }
        public GraphTypes GraphType { get; set; }
    }

    public class GetLiveByCountryAllStatusResponse : BaseResponse<List<GetLiveByCountryAllStatus>> { }

    public class ByCountryTotalAllStatus : GetLiveByCountryAllStatus { }

}
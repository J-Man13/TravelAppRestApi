using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TravelAppRestApi.Models;

using System.Net.Http;
using TravelAppRestApi.Models.TeleportApiModels;

namespace TravelAppRestApi.Services
{
    public class TeleportLocationService
    {
        private ApplicationProperties applicationProperties { get; set; }
        private static HttpClient HTTPClient { get; } = new HttpClient();

        public TeleportLocationService(IOptions<ApplicationProperties> ioptions)
        {
            applicationProperties = ioptions.Value;
        }

        public ICollection<TeleportLocationModel> GetLocationsByLocationName(string locationName)
        {
            LinkedList<TeleportLocationModel> teleportLocationModels = new LinkedList<TeleportLocationModel>();
            try
            {
                JObject jObject = GetJObject(applicationProperties.TeleportCitySearchWebApiURL + locationName);

                List<Task> taskList = new List<Task>();

                foreach (JObject j in (jObject["_embedded"])["city:search-results"] as JArray)
                {
                    Task task = new Task(() => {
                        string infoGeonameIdLink = ((j["_links"])["city:item"])["href"].Value<String>();
                        string fullName = j["matching_full_name"].Value<String>();

                        JObject jObjectGeonameId = GetJObject(infoGeonameIdLink);
                        double lattitude = ((jObjectGeonameId["location"])["latlon"])["latitude"].Value<double>();
                        double longtitude = ((jObjectGeonameId["location"])["latlon"])["longitude"].Value<double>();

                        string countryLink = ((jObjectGeonameId["_links"])["city:country"])["href"].Value<string>();

                        string countrySalariesLink = "";

                        string urbanAreaLink = "";
                        string urbanAreaSalariesLink = "";
                        string urbanAreaDetailsLink = "";
                        string urbanAreaScoresLink = "";
                        string urbanAreaImagesLink = "";

                        Task linksSetterTask = new Task(() => {
                            if ((jObjectGeonameId["_links"])["city:urban_area"] != null)
                            {
                                urbanAreaLink = ((jObjectGeonameId["_links"])["city:urban_area"])["href"].Value<string>();
                                JObject jObjectUrbanArea = GetJObject(urbanAreaLink);
                                urbanAreaSalariesLink = ((jObjectUrbanArea["_links"])["ua:salaries"])["href"].Value<string>();
                                urbanAreaDetailsLink = ((jObjectUrbanArea["_links"])["ua:details"])["href"].Value<string>();
                                urbanAreaScoresLink = ((jObjectUrbanArea["_links"])["ua:scores"])["href"].Value<string>();
                                urbanAreaImagesLink = ((jObjectUrbanArea["_links"])["ua:images"])["href"].Value<string>();
                            }
                        });

                        Task countriesLinkGetterTask = new Task(() => {
                            JObject jObjectCountry = GetJObject(countryLink);
                            countrySalariesLink = ((jObjectCountry["_links"])["country:salaries"])["href"].Value<string>();
                        });

                        List<Task> innerTaskList = new List<Task>();
                        linksSetterTask.Start();
                        countriesLinkGetterTask.Start();
                        innerTaskList.Add(linksSetterTask);
                        innerTaskList.Add(countriesLinkGetterTask);

                        Task.WaitAll(innerTaskList.ToArray());

                        lock (teleportLocationModels)
                        {
                            teleportLocationModels.AddLast(new TeleportLocationModel()
                            {
                                InfoGeonameIdLink = infoGeonameIdLink,
                                Lattitude = lattitude,
                                FullName = fullName,
                                Longtitude = longtitude,
                                CountryLink = countryLink,
                                CountrySalariesLink = countrySalariesLink,
                                UrbanAreaLink = urbanAreaLink,
                                UrbanAreaSalariesLink = urbanAreaSalariesLink,
                                UrbanAreaDetailsLink = urbanAreaDetailsLink,
                                UrbanAreaScoresLink = urbanAreaScoresLink,
                                UrbanAreaImagesLink = urbanAreaImagesLink
                            });
                        }

                    });
                    task.Start();
                    lock (taskList){
                        taskList.Add(task);
                    }                        
                }
                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception)
            {
                return null;
            }
            return teleportLocationModels;
        }

        public TeleportLocationCountryModel GetTeleportLocationCountryModel(string countryLink)
        {
            TeleportLocationCountryModel teleportCountryInfoModel = new TeleportLocationCountryModel();
            try
            {
                JObject jObjectCountry = GetJObject(countryLink);
                teleportCountryInfoModel.Name = jObjectCountry["name"].Value<string>();
                teleportCountryInfoModel.CurrencyCode = jObjectCountry["currency_code"].Value<string>();
                teleportCountryInfoModel.Iso_alpha2 = jObjectCountry["iso_alpha2"].Value<string>();
                teleportCountryInfoModel.Iso_alpha3 = jObjectCountry["iso_alpha3"].Value<string>();
                teleportCountryInfoModel.Population = jObjectCountry["population"].Value<int>();
                teleportCountryInfoModel.TeleportCountrySalaries = GetTeleportLocationCountrySalaryModels(((jObjectCountry["_links"])["country:salaries"])["href"].Value<string>());

            }
            catch (Exception )
            {
                return null;
            }
            return teleportCountryInfoModel;
        }

        public ICollection<TeleportLocationCountrySalaryModel> GetTeleportLocationCountrySalaryModels(string countrySalariesLink)
        {
            LinkedList<TeleportLocationCountrySalaryModel> teleportLocationCountrySalaryModels = new LinkedList<TeleportLocationCountrySalaryModel>();
            try
            {
                JObject jObjectCountrySalaries = GetJObject(countrySalariesLink);
                JArray jArraySalaries = jObjectCountrySalaries["salaries"] as JArray;
                foreach (JObject j in jArraySalaries)
                {
                    TeleportLocationCountrySalaryModel teleportLocationCountrySalaryModel = new TeleportLocationCountrySalaryModel();
                    teleportLocationCountrySalaryModel.SpecialtyName = (j["job"])["title"].Value<string>();
                    teleportLocationCountrySalaryModel.SalaryPerMonth = (int)((j["salary_percentiles"])["percentile_75"].Value<double>() / 12);
                    teleportLocationCountrySalaryModels.AddLast(teleportLocationCountrySalaryModel);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return teleportLocationCountrySalaryModels;
        }


        public ICollection<TeleportLocationDetailsModel> GetTeleportLocationDetailsModels(string urbanAreaDetailsLink)
        {
            LinkedList<TeleportLocationDetailsModel> teleportLocationDetailsModels = new LinkedList<TeleportLocationDetailsModel>();

            try
            {

                JArray jArrayCategoriesTypes = GetJObject(urbanAreaDetailsLink)["categories"] as JArray;

                foreach (JObject CategoryTypesJobject in jArrayCategoriesTypes)
                {
                    TeleportLocationDetailsModel teleportLocationDetailsModel = new TeleportLocationDetailsModel();
                    teleportLocationDetailsModel.DetailType = CategoryTypesJobject["label"].Value<string>();

                    LinkedList<TeleportLocationDetailModel> teleportLocationDetailModels = new LinkedList<TeleportLocationDetailModel>();

                    foreach (JObject jCategory in CategoryTypesJobject["data"] as JArray)
                    {
                        TeleportLocationDetailModel teleportLocationDetailModel = new TeleportLocationDetailModel();
                        teleportLocationDetailModel.Label = jCategory["label"].Value<String>();
                        teleportLocationDetailModel.Value = jCategory[jCategory["type"].Value<String>() + "_value"].Value<String>();
                        teleportLocationDetailModels.AddLast(teleportLocationDetailModel);
                    }

                    teleportLocationDetailsModel.TeleportLocationDetailModels = teleportLocationDetailModels;
                    teleportLocationDetailsModels.AddLast(teleportLocationDetailsModel);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return teleportLocationDetailsModels;
        }

        public TeleportLocationScoresModel GetTeleportLocationScoresModel(string urbanAreaScoresLink)
        {
            TeleportLocationScoresModel teleportLocationScoresModel = new TeleportLocationScoresModel();

            try
            {
                JObject jObject = GetJObject(urbanAreaScoresLink);

                String summary = jObject["summary"].Value<string>().Replace("<p>", "").Replace("</p>", "").Replace("<i>", "").Replace("</i>", "")
                .Replace("</b>", "").Replace("\n", "").Replace("<b>", "").Replace("  ", "").Replace(".", " . ").Replace("<br>", "").Replace("</br>", "");
                teleportLocationScoresModel.Summary = summary;
                teleportLocationScoresModel.CityScore = jObject["teleport_city_score"].Value<int>();

                LinkedList<TeleportLocationScoreModel> teleportSearchedCityDistrictScores = new LinkedList<TeleportLocationScoreModel>();

                foreach (JObject category in jObject["categories"] as JArray)
                {
                    TeleportLocationScoreModel teleportSearchedCityDistrictScore = new TeleportLocationScoreModel();
                    teleportSearchedCityDistrictScore.Color = category["color"].Value<String>();
                    teleportSearchedCityDistrictScore.ScoreName = category["name"].Value<String>();
                    teleportSearchedCityDistrictScore.ScoreOutOfTen = category["score_out_of_10"].Value<int>();
                    teleportSearchedCityDistrictScores.AddLast(teleportSearchedCityDistrictScore);
                }
                teleportLocationScoresModel.TeleportSearchedCityDistrictScores = teleportSearchedCityDistrictScores;
            }
            catch (Exception)
            {
                return null;
            }
            return teleportLocationScoresModel;
        }

        public byte[] GetSearchedCityImageBytes(string urbanAreaImagesLink)
        {
            try
            {
                string imageUrl = (((((GetJObject(urbanAreaImagesLink))["photos"] as JArray)[0] as JObject)["image"]) as JObject)["mobile"].Value<String>();
                return new WebClient().DownloadData(imageUrl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private JObject GetJObject(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string jsonString = webClient.DownloadString(url);
            return JsonConvert.DeserializeObject(jsonString) as JObject;
        }
    }


}

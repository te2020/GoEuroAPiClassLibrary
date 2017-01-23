using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace GoEuroAPiClassLibrary
{
    public static class GOEuroApi_Requests
    {
        #region Main

        /// <summary>
        /// Das Objekt, welches an das FrontEnd geschickt wird.
        /// </summary>
        /// <param name="departurePosition"></param>
        /// <param name="arrivalPosition"></param>
        ///// <param name="departureDate"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        public static async Task<string> BaueFrontEndObject(DateTime fromDate, DateTime toDate, string departurePosition = "377001"/*Nürnberg*/, string arrivalPosition = "403838"/*Moskau*/, string day = "onlyFriday")
        {
            try
            {
              
                string JSONResultAlsString = "{";

                int AnfrageNummer = 0;

                IEnumerable<DateTime> dateResult = DateRange(fromDate, toDate);

                switch (day)
                {
                    case "onlyMonday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Monday);
                        break;
                    case "onlyTuesday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Tuesday);
                        break;
                    case "onlyWednesday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Wednesday);
                        break;
                    case "onlyThursday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Thursday);
                        break;
                    case "onlyFriday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Friday);
                        break;
                    case "onlySaturday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Saturday);
                        break;
                    case "onlySunday":
                        dateResult = dateResult.Where(d => d.DayOfWeek == DayOfWeek.Sunday);
                        break;

                    default:
                        break;
                }

                //Start "root"-Objekt
                JSONResultAlsString +=
                    @"
                     ""Search_Ids"": [
                    ";

                StringBuilder strb = new StringBuilder();

                foreach (DateTime date in dateResult)
                {
                    AnfrageNummer++;

                    var result = await Get_SearchId_WithoutRoundTrip(departurePosition: departurePosition, arrivalPosition: arrivalPosition, departureDate: date.ToString("yyyy-MM-dd"), anfrageNo: AnfrageNummer);

                    //todo: "," außer beim leztzen Element
                    //todo:  "AnfrageNummer" noch irgendwie zur search_id reinpacken

                    //Testing
                    //JSONResultAlsString.Add(result.ToString());
                    strb.Append(result + ",");

                }

                JSONResultAlsString +=
                @"
                  " + strb.ToString().Remove(strb.Length - 1)/* Letzes "," entfernt */ + @"
                ],";

                //Ergebnis an das FrontEnd:
                // checksum: dateResult.Count<DateTime>(); //für die Progressbar ?!
                // AnfrageNummer
                // SearchId
                //latitude(´longitude: http://www.latlong.net/Show-Latitude-Longitude.html
                //https://support.google.com/maps/answer/18539?co=GENIE.Platform%3DDesktop&hl=en

                //dynamic d = JObject.Parse("{ Checksum:" + dateResult.Count<DateTime>() + ", AnfrageNummer:" + 0 + ", Search_Ids: [" + strb.ToString() + "]},Sonstiges:'string'");

                //Debug.WriteLine("# "+d.number);
                //Debug.WriteLine("# "+d.str);
                //Debug.WriteLine("# "+d.array.Count);

                //Letzer Wert in der 
                JSONResultAlsString +=
                @"""Checksum"":" + dateResult.Count<DateTime>();

                //Ende "root"-Objekt
                JSONResultAlsString +=
                "}";

                //dynamic d = JObject.Parse(JSONResultAlsString);

                return JSONResultAlsString.ToString();

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Ergebnisausgabe in "Debug.WriteLine(...)" 
        /// </summary>
        /// <param name="departurePosition"></param>
        /// <param name="arrivalPosition"></param>
        /// <param name="departureDate"></param>
        /// <param name="anfrageNo"></param>
        /// <returns></returns>
        private static async Task<string> Get_SearchId_WithoutRoundTrip(string departurePosition, string arrivalPosition, string departureDate, int anfrageNo)
        {
            try
            {


                #region BackUp(original)

                //var jsonString1 = @"{
                //                  ""searchOptions"": {
                //                        ""departurePosition"": { ""id"": 377001 },
                //                        ""arrivalPosition"": { ""id"": 376422 },
                //                        ""travelModes"": [ ""Flight"", ""Train"", ""Bus"" ],
                //                        ""departureDate"": ""2017-01-15"",
                //                        ""passengers"": [
                //                          {
                //                            ""age"": 12,
                //                            ""discountCards"": [ ]
                //                          }
                //                        ],
                //                        ""userInfo"": {
                //                          ""identifier"": ""0.jhvlf8amtgk"",
                //                          ""domain"": "".com"",
                //                          ""locale"": ""en"",
                //                          ""currency"": ""EUR""
                //                        },
                //                        ""abTestParameters"": [ ]
                //                  }
                //                }";

                #endregion

                #region Baue Parameter zusammen

                //var jsonString = @"{
                //                  searchOptions: {
                //                        departurePosition: { id:" + departurePosition + @"},
                //                        arrivalPosition: {id: " + arrivalPosition + @"},
                //                        travelModes: [ 'Flight', 'Train', 'Bus' ],
                //                        departureDate': '" + departureDate + @"',
                //                        passengers: [
                //                          {
                //                            age: 12,
                //                            discountCards: [ ]
                //                          }
                //                        ],
                //                        userInfo: {
                //                          identifier: '0.jhvlf8amtgk',
                //                          domain: '.com',
                //                          locale: 'en',
                //                          currency: 'EUR'
                //                        },
                //                        abTestParameters: [ ]
                //                  }
                //                }";
                #endregion

                #region Baue Parameter zusammen

                string string_departurePosition = @"{
                              ""searchOptions"": {
                                    ""departurePosition"": { ""id"": " + departurePosition;

                string string_arrivalPosition = @"},
                                    ""arrivalPosition"": { ""id"": " + arrivalPosition;


                string string_departureDate = @"             },
                                    ""travelModes"": [ ""Flight"", ""Train"", ""Bus"" ],
                                    ""departureDate"": """ + departureDate;

                string string_rest = @""",
                                    ""passengers"": [
                                      {
                                        ""age"": 12,
                                        ""discountCards"": [ ] 
                                     }
                                    ],
                                    ""userInfo"": {
                                      ""identifier"": ""0.jhvlf8amtgk"",
                                      ""domain"": "".com"",
                                      ""locale"": ""en"",
                                      ""currency"": ""EUR""
                                    },
                                    ""abTestParameters"": [ ]
                              }
                            }";

                string jsonString = string_departurePosition + string_arrivalPosition + string_departureDate + string_rest;

                #endregion

                #region Bereite die Anfrage vor.

                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");

                httpClient.BaseAddress = new Uri("https://www.goeuro.com/");


                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/GoEuroAPI/rest/api/v5/searches");
                request.Content = new StringContent(jsonString,
                                                    Encoding.UTF8,
                                                    "application/json");//CONTENT-TYPE header

                #endregion

                #region Hole Daten vom Server :)

                var response = await httpClient.SendAsync(request);

                Debug.WriteLine("# " + anfrageNo + ".  zum " + departureDate + " => " + await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();

                #endregion

                #region Interessant, wofür ?!

                //using (var responseStream = await response.Content.ReadAsStreamAsync())
                //using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                //using (var streamReader = new StreamReader(decompressedStream))
                //{
                //    return streamReader.ReadToEnd();
                //} 

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Getting all DateTimes between two 'DateTime's 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> DateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                        .Select(d => fromDate.AddDays(d));
        }

        #endregion

        #region Testing

        public static void ConsoleStarteTest()
        {
            //Init
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;

            //Datum
            DateTime fromDate = new DateTime(2017, 1, 16);
            DateTime toDate = new DateTime(2017, 2, 16);

            string departurePosition = "377001";
            string arrivalPosition = "376422";
            //string departureDate = "";

            //var result = Get_SearchId_WithoutRoundTrip(departurePosition: "377001", arrivalPosition: "376422", departureDate: date.ToString("yyyy-MM-dd"), anfrageNo: AnfrageNummer);

            BaueFrontEndObject(departurePosition: departurePosition, arrivalPosition: arrivalPosition,/* departureDate: departureDate,*/ fromDate: fromDate, toDate: toDate);

            Console.ReadLine();
        }

        #endregion

    }
}

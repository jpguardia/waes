using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WaesAssignment.DataServices;
using WaesAssignment.Dto;
using JsonDiffPatch;
using Json.Comparer;
using System.Threading.Tasks;

namespace WaesAssignment.Services
{
    public class DiffService : IDiffService
    {
        private IMemoryCacheDataService _memoryCacheDataService;

        public DiffService(IMemoryCacheDataService memoryCacheDataService)
        {
            _memoryCacheDataService = memoryCacheDataService;
        }

        public ServiceResultWrapper Add(int id, string diffType, string base64Data)
        {
            ServiceResultWrapper result = new ServiceResultWrapper();

            if (!IsBase64String(base64Data))
            {
                result.Code = ServiceResultCode.BadRequest;
                return result;
            }

            var jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Data));

            if (!IsValidJson(jsonString))
            {
                result.Code = ServiceResultCode.BadRequest;
                return result;
            }

            if (_memoryCacheDataService.Any($"{diffType}_{id}"))
            {
                _memoryCacheDataService.Delete($"{diffType}_{id}");
            }

            _memoryCacheDataService.Add($"{diffType}_{id}", base64Data, DateTime.UtcNow.AddHours(1));

            result.Code = ServiceResultCode.Ok;

            return result;
        }

        public ServiceResultWrapper<JObject> GetById(string diffType, int id)
        {
            ServiceResultWrapper<JObject> result = new ServiceResultWrapper<JObject>();
            var base64Data = _memoryCacheDataService.Get($"{diffType}_{id}").ToString();
            var jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Data));

            var json = JObject.Parse(jsonString);

            result.Data = json;
            result.Code = ServiceResultCode.Ok;

            return result;
        }


        public ServiceResultWrapper<DiffResultDto> ComputeDifference(int id)
        {
            ServiceResultWrapper<DiffResultDto> result = new ServiceResultWrapper<DiffResultDto>();            
            DiffResultDto diffResult = new DiffResultDto();

            //Check if the record Left exist in memory
            if (!_memoryCacheDataService.Any($"{DiffTypes.Left}_{id}"))
            {
                result.Code = ServiceResultCode.NotFound;
                diffResult.Code = DiffResultTypes.Unavailable;
                diffResult.Description = "Left not found";
                result.Data = diffResult;
                return result;
            }
            //Check if the record Right exist in memory
            if (!_memoryCacheDataService.Any($"{DiffTypes.Right}_{id}"))
            {
                result.Code = ServiceResultCode.NotFound;
                diffResult.Code = DiffResultTypes.Unavailable;
                diffResult.Description = "Right not found";
                result.Data = diffResult;
                return result;
            }
            
            //Load Left
            var leftBase64Data = _memoryCacheDataService.Get($"{DiffTypes.Left}_{id}").ToString();

            //Load Right
            var rightBase64Data = _memoryCacheDataService.Get($"{DiffTypes.Right}_{id}").ToString();


            // Same string
            if (leftBase64Data.Equals(rightBase64Data))
            {
                diffResult.Code = DiffResultTypes.Match;
                diffResult.Description = "100% match.";
                result.Code = ServiceResultCode.Ok;
                result.Data = diffResult;

                return result;
            }

            //Different length
            if (leftBase64Data.Length != rightBase64Data.Length)
            {
                diffResult.Code = DiffResultTypes.NoMatch;
                diffResult.Description = $"Left Size: {leftBase64Data.Length} vs Righ Size: {rightBase64Data.Length}";
                result.Code = ServiceResultCode.Ok;
                result.Data = diffResult;

                return result;
            }

            //Different string but same size scenario.

            var leftJson = ConvertToJson(leftBase64Data);
            var rightJson = ConvertToJson(rightBase64Data);

            diffResult.Code = DiffResultTypes.NoMatch;            
            diffResult.Description = $"Same Size. Data Differences detected";
            
            //Basic comparion. 
            diffResult.Details = Compare(leftJson, rightJson);            
            
            result.Code = ServiceResultCode.Ok;
            result.Data = diffResult;
            return result;
        }


        /// <summary>
        /// Comparing two json objects.
        /// Limitations : Basic comparison for arrays. 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private JObject Compare(JObject left, JObject right)
        {
            JObject result = new JObject();

            foreach (KeyValuePair<string, JToken> leftProp in left)
            {
                JProperty rightProp = right.Property(leftProp.Key);
                if (rightProp == null)
                {
                    result.Add(leftProp.Key, "not found");
                    continue;
                }
                if (!JToken.DeepEquals(leftProp.Value, rightProp.Value))
                {
                    result.Add(leftProp.Key, $"Previous Length:{leftProp.Value.ToString().Length}. New Length: {rightProp.Value.ToString().Length}");
                }
            }
            return result;
        }


        private JObject ConvertToJson(string base64Data)
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Data));

            return JObject.Parse(jsonString);
        }


        private string GetKey(string diffType, int id)
        {
            return $"{diffType}_{id}";
        }

        private bool IsValidJson(string json)
        {
            try
            {
                JObject.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

    }
}
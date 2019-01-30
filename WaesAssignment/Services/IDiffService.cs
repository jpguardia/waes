using Newtonsoft.Json.Linq;
using WaesAssignment.Dto;

namespace WaesAssignment.Services
{
    public interface IDiffService
    {
        ServiceResultWrapper<JObject> GetById(string diff, int id);
        ServiceResultWrapper Add(int id, string diffType, string base64Data);
        ServiceResultWrapper<DiffResultDto> ComputeDifference(int id);


    }
}
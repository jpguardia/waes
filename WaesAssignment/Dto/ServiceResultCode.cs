namespace WaesAssignment.Dto
{
    /// <summary>
    /// All service methods respond with a ServiceResultWrapper.
    /// The ServiceResultCode provides information to the calling layer 
    /// regarding the success of the call.  
    /// </summary>
    public enum ServiceResultCode
    {
        Ok,
        BadRequest,
        Unauthorized,
        NotFound,
        ServerError
    }
}
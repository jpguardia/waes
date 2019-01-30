namespace WaesAssignment.Dto
{
    using System;

    public class ServiceResultWrapper
    {
        public ServiceResultCode Code { get; set; }
        public Exception Exception { get; set; }

        public string Message { get; set; }

        //  Although there's a mingling here of controller layer stuff, it can be easily replaced with our own set of
        //  codes.  However, since the Api is meant to be an Api, I see no harm in allowing an HttpStatusCode attribute to
        //  exist at this level.
        public bool Success => Code == ServiceResultCode.Ok;
    }

    public class ServiceResultWrapper<T> : ServiceResultWrapper
    {
        public T Data { get; set; }
    }
}
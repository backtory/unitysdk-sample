public enum BacktoryHttpStatusCode
    {
        AccessRevoked = 101,

        OK = 200,
        Created = 201,
        Accepted = 202,

        Redirect = 302, /* original name is "Found" */

        BadRequest = 400,
        Unauthorized = 401,
        NotEnoughCredit = 402,
        Forbidden = 403,
        NotFound = 404,
        RequestTimeout = 408,
        Conflict = 409,
        PreconditionFailed = 412,
        ExpectationFailed = 417,
        RunFailed = 420,

        InternalServerError = 500,
        NotImplemented = 501,
        ServiceUnavailable = 503,

        Unknown = 999,
        InternetAccessProblem =1000,
        Timeout = 1001
    }


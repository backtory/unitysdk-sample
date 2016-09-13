namespace Assets.BacktorySDK.core
{
    public abstract class BacktoryResponse
    {
        protected BacktoryResponse(int code, string message, bool successful)
        {
            Code = code;
            Message = message;
            Successful = successful;
            //ErrorException = errorException;
        }

        protected BacktoryResponse(int code, bool successful) : this(code, null, successful) { }

        public int Code { get; private set; }
        private string _message;

        /// <summary>
        /// Indicates if request was successful by checking response code to be in [200, 300) interval
        /// </summary>
        public bool Successful { get; private set; }
        // not stable
        //internal Exception ErrorException { get; private set; }

        /// <summary>
        /// In case of a unsuccessful response this message shows the error cause.
        /// If server provides error description takes that value, 
        /// if not the above and If a network error occurs (e.g. time out) takes the exception message as value
        /// if not the above just takes the corresponding HTTP status representation.
        /// </summary>
        public string Message
        {
            get
            {
                return _message.IsEmpty() ? ((BacktoryHttpStatusCode)Code).ToString() : _message;
            }
            private set { _message = value; }
        }

        /// <summary>
        /// Create a new response from another, converting generic type
        /// </summary>
        /// <typeparam name="RAW"></typeparam>
        /// <typeparam name="TRANSFORMED"></typeparam>
        /// <param name="backtoryResponse"></param>
        /// <returns></returns>
        public static BacktoryResponse<TRANSFORMED> Error<RAW, TRANSFORMED>(BacktoryResponse<RAW> backtoryResponse)
            where TRANSFORMED : class
            where RAW : class
        {
            return new BacktoryResponse<TRANSFORMED>(backtoryResponse.Code, 
                backtoryResponse.Message,/*backtoryResponse.Message*/ null, false);
        }

        
    }

    public class BacktoryResponse<T> : BacktoryResponse where T : class
    {
        public BacktoryResponse(int code, string message, T body, bool successful) :
            base(code, message, successful)
        {
            Body = body;
        }

        public BacktoryResponse(int code, T body, bool successful) : this(code, null, body, successful) { }

        public T Body { get; private set; }


        public static BacktoryResponse<T> Error(int code, string message)
        {
            return new BacktoryResponse<T>(code, message, null, false);
        }

        public static BacktoryResponse<T> Error(int code)
        {
            return Error(code, null);
        }

        public static BacktoryResponse<T> Success(int code, T body)
        {
            return new BacktoryResponse<T>(code, body, true);
        }

        public static BacktoryResponse<T> Unknown()
        {
            return Unknown(null);
        }

        public static BacktoryResponse<T> Unknown(string message)
        {
            return new BacktoryResponse<T>((int)BacktoryHttpStatusCode.Unknown, message, null, false);
        }

    }
}
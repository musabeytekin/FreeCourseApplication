using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.DTOs
{
    public class ResponseDto<T>
    {
        public T Data { get; private set; }

        [JsonIgnore]
        public int StatusCode { get; private set; }

        [JsonIgnore]
        public bool IsSuccess { get; private set; }

        public List<string> Errors { get; set; }

        public static ResponseDto<T> Success(T data, int statusCode)
        {
            return new ResponseDto<T> { Data = data, StatusCode = statusCode, IsSuccess = true };
        }

        public static ResponseDto<T> Success(int statusCode)
        {
            return new ResponseDto<T> { Data = default(T), StatusCode = statusCode, IsSuccess = true };
        }

        public static ResponseDto<T> Fail(int statusCode, List<string> errors)
        {
            return new ResponseDto<T> { Errors = errors, StatusCode = statusCode, IsSuccess = false };
        }

        public static ResponseDto<T> Fail(int statusCode, string error)
        {
            return new ResponseDto<T> { Errors = new List<string> { error }, StatusCode = statusCode, IsSuccess = false };
        }
    }
}


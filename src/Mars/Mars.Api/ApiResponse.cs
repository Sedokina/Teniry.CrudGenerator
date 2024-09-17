namespace Mars.Api;

// Required for generated endpoints api response
public class ApiResponse {
    public string Message { get; set; }

    public ApiResponse(string message) {
        Message = message;
    }
}

public class ApiResponse<T> {
    public string Message { get; set; }
    public T      Value   { get; set; }

    public ApiResponse(string message, T value) {
        Message = message;
        Value   = value;
    }
}
namespace ProductApi.DTOs;

public record AddProductRequest(string ProductCode);

public record ProductResponse(int Id, string ProductCode, string BarcodeBase64, DateTime CreatedAt);

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username);

public record DeleteConfirmRequest(int Id);

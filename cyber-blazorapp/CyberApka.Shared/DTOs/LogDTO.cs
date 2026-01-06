namespace CyberApka.Shared.DTOs;

public record LogDto(int Id, string Action, string? Details, string? Username, DateTime TimeStamp);
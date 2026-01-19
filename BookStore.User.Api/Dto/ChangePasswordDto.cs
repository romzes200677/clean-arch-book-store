namespace BookStore.User.Api.Dto;

public record ChangePasswordDto(string OldPassword, string NewPassword);

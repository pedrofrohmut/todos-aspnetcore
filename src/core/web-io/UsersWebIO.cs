using Todos.Core.Dtos;
using Todos.Core.Exceptions;
using Todos.Core.UseCases.Users;

namespace Todos.Core.WebIO; 

public class UsersWebIO
{
    public WebResponseDto SignUpUser(ISignUpUseCase signUpUseCase, WebRequestDto request)
    {
        try {
            signUpUseCase.Execute((CreateUserDto) request.Body!);
            return new WebResponseDto() { Message = "User created", Status = 201 };
        } catch (Exception e) {
            if (e is InvalidUserException || e is EmailAlreadyTakenException) {
                return new WebResponseDto() { Message = e.Message, Status = 400 };
            }
            throw;
        }
    }

    public WebResponseDto SignInUser(ISignInUseCase signInUseCase, WebRequestDto req)
    {
        try {
            var signedUser = signInUseCase.Execute((UserCredentialsDto) req.Body!);
            return new WebResponseDto() { Body = signedUser, Status = 200 };
        } catch (Exception e) {
            if (e is InvalidUserException || 
                e is UserNotFoundException || 
                e is PasswordAndHashNotMatchException) 
            {
                return new WebResponseDto() { Message = e.Message, Status = 400 };
            }
            throw;
        }
    }

    // TODO:
    public WebResponseDto Verify(WebRequestDto req)
    {
        return new WebResponseDto() {
            Body = true,
            Status = 200
        };
    }
}

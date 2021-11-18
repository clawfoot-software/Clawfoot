using Clawfoot.Status;
using Clawfoot.Status.Interfaces;
using Clawfoot.TestUtilities.Performance;
using System;

namespace TestingProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            // TestStatus();
            TestTestUtilityClock();

            Console.ReadLine();
        }

        static void TestTestUtilityClock()
        {
            Action<ClockTestState> action = (ClockTestState state) =>
            {
                for(int i = 0; i < state.Iterations; i++)
                {
                    string thing = "asdkjfaslkdjfaskljdfadsdfsdfsdfaweradfasdfasdfasdfasdfasdfasdfadf.awekljfalskjdfalksdjfhlaksdjfhsfahsdf";
                    thing.IndexOf('6');
                }
            };

            Func<ClockTestState> setup = () =>
           {
               return new ClockTestState()
               {
                   Iterations = 750
               };
           };

            var perfResults = Clock.BenchmarkTime(action, setup, 1000);
        }

        static void TestStatus()
        {
            IStatus status = new Status();

            status.AddError(Error.From(AuthError.RegistrationError,"some technical jargon", "An unexpected error has occurred"));
            status.AddError(Error.From(AuthError.UserAlreadyExists, new []{ "gg@gg.com" }));
            status.AddError(Error.From(AuthError.InvalidCredentials));

            Console.WriteLine(status.ToString());
            Console.WriteLine();
            Console.WriteLine(status.ToUserFriendlyString());
        }
    }

    class ClockTestState
    {
        public int Iterations { get; set; }
    }

    public enum AuthError
    {
        [Error(Code = (int)InvalidCredentials, GroupName = "Login", Message = "Invalid Credentials")]
        InvalidCredentials = 101,

        [Error(Code = (int)RegistrationError, GroupName = "Registration")]
        RegistrationError = 200,
        [Error(Code = (int)UserAlreadyExists, GroupName = "Registration", Message = "User with this email address {0} already exists")]
        UserAlreadyExists = 201,
    }
}
